using Domain.Dtos.Shared;

namespace Domain.Dtos.OperationalObjectives
{
    public class OperationalObjectiveCard
    {
        public Guid Id { get; set; }
        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime GuaranteedFulfillmentAt { get; set; }
        public DateTime Deadline { get; set; }
        public int Progress { get; set; }

        public int IndicatorsCount { get; set; }

        public int RealProgress { get; set; }

        public bool Active { get; set; }

        public List<ProjectActionCard> Projects { get; set; } = new List<ProjectActionCard>();
        public List<ProjectActionCard> Actions { get; set; } = new List<ProjectActionCard>();
    }
}
