using Application.Repositories;
using Application.Utility;
using Domain;
using Domain.Dtos.Account.Permissions;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public PermissionRepository(ApplicationDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<NestedPermissions> GetNestedPermissionsAsync(CancellationToken cancellationToken)
        {
            var companyId = (_contextAccessor.HttpContext.User.Identity as ClaimsIdentity).GetCompanyId();

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
