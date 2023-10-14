using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Account.Permissions;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserAccessor _userAccessor;

        public PermissionRepository(ApplicationDbContext context, IHttpContextAccessor contextAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<NestedPermissions> GetNestedPermissionsAsync(CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();

            var blockedType = companyId.HasValue ? PermissionType.System : PermissionType.Company;

            var permissions = await _context.Permissions
                .Where(b =>
                    b.Discriminator == nameof(PermissionContainer) &&
                    b.Type != blockedType)
                .Include(b => ((PermissionContainer)b).Childrens)
                .ToListAsync(cancellationToken);

            var containerList = permissions.Select(b => (PermissionContainer)b).ToList();
            var parent = containerList.First(b => b.Id == SD.TopPermissionId);

            return ConvertToSelectListPermissions(containerList, parent, blockedType);
        }


        private NestedPermissions ConvertToSelectListPermissions(List<PermissionContainer> permissionContainers, PermissionContainer permission, PermissionType blockedType)
        {
            var selectListPermission = new NestedPermissions(permission.Id, permission.Title);

            foreach (var pe in permission.Childrens)
            {
                if (pe.Discriminator == nameof(PermissionContainer) && pe.Type != blockedType)
                {
                    var convertedChild =
                        ConvertToSelectListPermissions(
                            permissionContainers,
                            permissionContainers.First(b => b.Id == pe.Id),
                            blockedType);
                    selectListPermission.Childs.Add(convertedChild);
                }
                else if (pe.Type != blockedType)
                    selectListPermission.Childs.Add(new NestedPermissions(pe.Id, pe.Title));
            }

            return selectListPermission;
        }
    }
}
