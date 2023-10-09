using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Account
{
    public class User : BaseEntity
    {
        [Required]
        public String NationalId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; }

        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }

        [ForeignKey(nameof(Company))]
        public Guid? CompanyId { get; set; }

        public Role Role { get; set; }
        public Token Token { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public Company Company { get; set; }
    }
}
