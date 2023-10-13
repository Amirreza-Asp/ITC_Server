using Domain.Entities.Account;
using Domain.Entities.Static;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class BigGoal : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Range(0, 100)]
        [Required]
        public short Progress { get; set; }

        public Guid? ProgramYearId { get; set; }
        public ProgramYear ProgramYear { get; set; }

        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<OperationalObjective> OperationalObjectives { get; set; } = new List<OperationalObjective>();
    }
}
