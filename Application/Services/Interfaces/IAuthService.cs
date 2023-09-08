using Domain.Dtos.Account.Permissions;
using Domain.Dtos.Account.SSO;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> CheckPermission(string nationalCode, string permissionFlag);
        Task<List<PermissionSummary>> GetPermissionsAsync(String nationalCode);

        Task<String> GetRoleAsync(String nationalCode);
        Task LoginAsync(String nationalId, String uswToken);
        Task<bool> LoginWithRefreshTokenAsync(Guid refreshToken);
        Task LogoutAsync();

        Task<bool> ExistsAsync(String nationalId);
        Task CreateAsync(ProfileRequest user);
    }
}
