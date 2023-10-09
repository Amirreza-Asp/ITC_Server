using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class OperationalObjective : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public DateTime GuaranteedFulfillmentAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public long Budget { get; set; }


        [ForeignKey(nameof(BigGoal))]
        public Guid BigGoalId { get; set; }

        [Range(0, 100)]
        public short Progress { get; set; }

        public BigGoal BigGoal { get; set; }
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<PracticalAction> PracticalActions { get; set; } = new List<PracticalAction>();
    }
}
