using System.ComponentModel.DataAnnotations;

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

        public ICollection<OperationalObjective> OperationalObjectives { get; set; } = new List<OperationalObjective>();
    }
}
