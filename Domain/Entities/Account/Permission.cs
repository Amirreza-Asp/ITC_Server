using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Account
{
    public abstract class Permission : BaseEntity
    {
        [Required]
        public String Title { get; set; }
        public String Discriminator { get; set; }

        public ICollection<RolePermission> Roles { get; set; } = new List<RolePermission>();
    }

    public class PermissionContainer : Permission
    {
        readonly List<Permission> _childrens = new List<Permission>();
        public ICollection<Permission> Childrens => _childrens;

        public PermissionContainer(string title)
        {
            Id = Guid.NewGuid();
            Title = title;
        }

        public PermissionContainer(string title, List<Permission> childrens)
        {
            GuardAgaintSelfChildren(childrens.ToArray());

            Id = Guid.NewGuid();
            Title = title;
            _childrens = childrens;
        }

        public PermissionContainer(Guid id, string title, List<Permission> childrens)
        {
            GuardAgaintSelfChildren(childrens.ToArray());

            Id = id;
            Title = title;
            _childrens = childrens;
        }

        private PermissionContainer()
        { }

        public void Add(Permission permission)
        {
            GuardAgaintSelfChildren(permission);
            _childrens.Add(permission);
        }

        public void Remove(Permission permission)
        {
            _childrens.Remove(permission);
        }

        private void GuardAgaintSelfChildren(params Permission[] permissions)
        {
            if (permissions.Any(b => b.Id == Id))
                throw new Exception("نمیتوان یک سطح دسترسی را به عنوان فرزند به خودش داد");
        }

    }

    public class PermissionItem : Permission
    {
        public String PageValue { get; protected set; }

        public PermissionItem(string title, string pageValue)
        {
            Id = Guid.NewGuid();
            Title = title;
            PageValue = pageValue;
        }
        private PermissionItem()
        { }
    }

}
