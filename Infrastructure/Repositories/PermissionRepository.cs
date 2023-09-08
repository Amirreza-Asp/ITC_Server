using Application.Repositories;
using Domain;
using Domain.Dtos.Account.Permissions;
using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NestedPermissions> GetNestedPermissionsAsync(CancellationToken cancellationToken)
        {
            var permissions =
                await _context.Permissions
                .Where(b => b.Discriminator == nameof(PermissionContainer))
                .Include(b => ((PermissionContainer)b).Childrens)
                .ToListAsync(cancellationToken);

            var containerList = permissions.Select(b => (PermissionContainer)b).ToList();
            var parent = containerList.First(b => b.Id == SD.TopPermissionId);

            return ConvertToSelectListPermissions(parent);
        }


        private NestedPermissions ConvertToSelectListPermissions(PermissionContainer permission)
        {
            var selectListPermission = new NestedPermissions(permission.Id, permission.Title);

            foreach (var pe in permission.Childrens)
            {
                if (pe.Discriminator == nameof(PermissionContainer))
                {
                    var convertedChild = ConvertToSelectListPermissions((PermissionContainer)pe);
                    selectListPermission.Childs.Add(convertedChild);
                }
                else
                    selectListPermission.Childs.Add(new NestedPermissions(pe.Id, pe.Title));
            }

            return selectListPermission;
        }
    }
}
