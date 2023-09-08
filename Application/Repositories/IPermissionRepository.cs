using Domain.Dtos.Account.Permissions;

namespace Application.Repositories
{
    public interface IPermissionRepository
    {
        Task<NestedPermissions> GetNestedPermissionsAsync(CancellationToken cancellationToken);
    }
}
