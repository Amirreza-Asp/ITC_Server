using Domain.Dtos.Shared;

namespace Domain.Dtos.BigGoals
{
    public class BigGoalWithOperationalObjectives
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public List<SelectSummary> OperationalObjectives { get; set; } = new List<SelectSummary>();
    }

}
