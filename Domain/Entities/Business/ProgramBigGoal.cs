using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class ProgramBigGoal
    {
        [ForeignKey(nameof(Program))]
        public Guid ProgramId { get; set; }
        [ForeignKey(nameof(BigGoal))]
        public Guid BigGoalId { get; set; }

        public Program Program { get; set; }
        public BigGoal BigGoal { get; set; }
    }
}
