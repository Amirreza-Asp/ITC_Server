namespace Domain.Dtos.BigGoals
{
    public class BigGoalSummary
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public int OperationalObjectiveCount { get; set; }
        public int IndicatorsCount { get; set; }
    }
}
