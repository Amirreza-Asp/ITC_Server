using Application.Services.Interfaces;
using Application.Utility;
using AutoMapper;
using Domain;
using Domain.Dtos.Account;
using Domain.Dtos.Account.Permissions;
using Domain.Dtos.Account.SSO;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public AuthService(ApplicationDbContext context, IMemoryCache cacheContext, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _context = context;
            _memoryCache = cacheContext;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public async Task<bool> CheckPermission(String nationalCode, String permissionFlag)
        {
            String permissionCacheKey = $"permissions-{nationalCode}";
            var permissionFlags = new List<string>();

            if (!_memoryCache.TryGetValue(permissionCacheKey, out permissionFlags))
            {
                permissionFlags = await GetFromContext(nationalCode);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _memoryCache.Set(permissionCacheKey, permissionFlags, cacheEntryOptions);
            }

            return permissionFlag != null && permissionFlags.Contains(permissionFlag);
        }

        public async Task<String> GetRoleAsync(string nationalCode)
        {
            return
                await _context.Users
                    .Where(b => b.NationalId == nationalCode)
                    .Select(b => b.Role.Title)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsAsync(String nationalId)
        {
            return await _context.Users.AnyAsync(b => b.NationalId == nationalId);
        }

        public async Task CreateAsync(ProfileRequest user)
        {
            var userRole = new User
            {
                Id = Guid.NewGuid(),
                NationalId = user.data.nationalId,
                IsActive = true,
                RoleId = SD.AdminRoleId
            };

            _context.Users.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task LoginAsync(String nationalId, String uswToken)
        {
            if (!await _context.Users.AnyAsync(b => b.NationalId == nationalId))
                await AddUserAsync(nationalId);

            var user =
                await _context.Users
                    .Include(b => b.Role)
                    .Where(b => b.NationalId == nationalId)
                    .AsNoTracking()
                    .FirstAsync();

            var ip = _contextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var token = JWTokenService.GenerateToken(nationalId, user.Role.Title, ip);
            var hashedToken = ProtectorData.Encrypt(token);

            await UpsertToken(ip, hashedToken, user.Id);
            var newRefreshTokenValue = await UpsertRefreshToken(user.Id);

            SetCookie(hashedToken, newRefreshTokenValue, uswToken);
        }

        public async Task<bool> LoginWithRefreshTokenAsync(Guid refreshToken)
        {
            var uswToken = _contextAccessor.HttpContext.Request.Cookies[SD.UswToken];

            if (uswToken == null)
                return false;

            var rfToken =
                await _context.RefreshTokens
                    .Where(b => b.Value == refreshToken)
                    .Include(b => b.User)
                        .ThenInclude(b => b.Token)
                    .Include(b => b.User)
                        .ThenInclude(b => b.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            if (rfToken == null || rfToken.IsActive == false || rfToken.Expiration < DateTime.Now || rfToken.User.IsActive == false)
                return false;

            var ip = _contextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var newToken = JWTokenService.GenerateToken(rfToken.User.NationalId, rfToken.User.Role.Title, ip);
            var tokenHash = ProtectorData.Encrypt(newToken);
            await UpsertToken(ip, tokenHash, rfToken.UserId);

            SetCookie(tokenHash, refreshToken, ProtectorData.Decrypt(uswToken));

            return true;
        }

        public async Task LogoutAsync()
        {
            var nationalId = _contextAccessor.HttpContext.User.Claims.First(b => b.Type == AppClaims.NationalId).Value;
            var token =
                await _context.Tokens
                    .Where(b => b.User.NationalId == nationalId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            if (token == null)
                return;

            token.IsActive = false;
            _context.Tokens.Update(token);
            _memoryCache.Remove($"user-{nationalId}");

            await _context.SaveChangesAsync();
        }

        public async Task<List<PermissionSummary>> GetPermissionsAsync(string nationalCode)
        {
            var permissons =
                await _context.Permissions
                    .Where(b => b.Discriminator == nameof(PermissionItem) &&
                                b.Roles.Any(b => b.Role.Users.Any(b => b.NationalId == nationalCode)))
                    .ToListAsync();

            List<PermissionSummary> data = new List<PermissionSummary>();
            permissons.ForEach(item =>
            {
                var permissinItem = (PermissionItem)item;
                data.Add(_mapper.Map<PermissionSummary>(permissinItem));
            });

            return data;
        }


        #region Utilities
        private void SetCookie(String hashedToken, Guid refreshToken, String uswToken)
        {
            var authInfo = new AuthInfo
            {
                Token = hashedToken,
                RefreshToken = ProtectorData.Encrypt(refreshToken.ToString()),
            };

            _contextAccessor.HttpContext.Response.Cookies.Append(SD.AuthInfo, JsonSerializer.Serialize(authInfo), new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

            _contextAccessor.HttpContext.Response.Cookies.Append(SD.UswToken, ProtectorData.Encrypt(uswToken), new CookieOptions()
            {
                Expires = DateTime.Now.AddMonths(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            });
        }

        private async Task AddUserAsync(String nationalId)
        {
            var userRole = new User
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                NationalId = nationalId,
                RoleId = SD.AdminRoleId
            };

            _context.Users.Add(userRole);
            await _context.SaveChangesAsync();
        }

        private async Task UpsertToken(String ip, String tokenHash, Guid userId)
        {
            var token =
                await _context.Tokens
                    .Where(b => b.UserId == userId)
                    .Include(b => b.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();



            if (token == null)
            {
                token = new Token
                {
                    Id = Guid.NewGuid(),
                    Expiration = DateTime.Now.AddMinutes(SD.ExpirationTime),
                    Ip = ip,
                    HashValue = tokenHash,
                    UserId = userId,
                    IsActive = true
                };

                _context.Tokens.Add(token);
            }
            else
            {
                _memoryCache.Remove($"user-{token.User.NationalId}");
                token.HashValue = tokenHash;
                token.Expiration = DateTime.Now.AddMinutes(SD.ExpirationTime);
                token.Ip = ip;
                token.IsActive = true;

                _context.Tokens.Update(token);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Guid> UpsertRefreshToken(Guid userId)
        {
            var refreshToken =
                await _context.RefreshTokens
                    .Where(b => b.UserId == userId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

            if (refreshToken == null)
            {
                refreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Expiration = DateTime.Now.AddMonths(1),
                    UserId = userId,
                    Value = Guid.NewGuid(),
                };

                _context.RefreshTokens.Add(refreshToken);
            }
            else
            {
                refreshToken.Value = Guid.NewGuid();
                refreshToken.Expiration = DateTime.Now.AddMonths(1);

                _context.RefreshTokens.Update(refreshToken);
            }

            await _context.SaveChangesAsync();

            return refreshToken.Value;
        }

        private async Task<List<String>> GetFromContext(String nationalCode)
        {
            // get user role
            var userRole = await _context.Users.FirstOrDefaultAsync(b => b.NationalId == nationalCode);

            if (userRole == null)
                return new List<String>();

            // return permissions
            var permissions =
                await _context.RolePermissions
                    .Where(b => b.RoleId == userRole.RoleId && b.Permission.Discriminator == nameof(PermissionItem))
                    .Select(b => (PermissionItem)b.Permission)
                    .ToListAsync();

            return permissions.Select(b => b.PageValue).ToList();
        }

        #endregion

    }

}
