using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Business
{
    public class ProgramYear : BaseEntity
    {
        [Required]
        public String Year { get; set; }

        public ICollection<BigGoal> BigGoals { get; set; } = new List<BigGoal>();
    }
}
