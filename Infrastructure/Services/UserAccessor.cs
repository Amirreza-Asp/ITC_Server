using Application.Services.Interfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCompanyId()
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            var companyId = claims.FindFirst(AppClaims.CompanyId).Value;
            if (Guid.Empty == Guid.Parse(companyId))
                return null;
            return Guid.Parse(companyId);
        }

        public string GetNationalId()
        {
            var claims = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            return claims.FindFirst(AppClaims.NationalId).Value;
        }
    }
}
