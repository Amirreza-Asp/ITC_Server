﻿namespace Domain.Dtos.Shared
{
    public class ProjectActionCard
    {
        public Guid Id { get; set; }
        public String Title { get; set; }

        public String Description { get; set; }

        public DateTime Deadline { get; set; }

        public bool Active { get; set; }

        public String Type { get; set; }

        public List<String> Financials { get; set; } = new List<String>();
    }
}
