﻿using Domain.Dtos.Shared;

namespace Domain.Dtos.OperationalObjectives
{
    public class OperationalObjectiveDetails
    {
        public Guid Id { get; set; }
        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime Deadline { get; set; }

        public bool Active { get; set; }

        public List<ProjectActionCard> Projects { get; set; } = new List<ProjectActionCard>();
        public List<ProjectActionCard> Actions { get; set; } = new List<ProjectActionCard>();
    }
}
