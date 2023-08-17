namespace Domain.Dtos.OperationalObjectives
{
    public class OperationalObjectiveSummary
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime GuaranteedFulfillmentAt { get; set; }

        public DateTime Deadline { get; set; }

        public long Budget { get; set; }

        public Guid BigGoalId { get; set; }
    }
}
