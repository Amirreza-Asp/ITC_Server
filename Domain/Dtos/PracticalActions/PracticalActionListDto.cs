﻿namespace Domain.Dtos.PracticalActions
{
    public class PracticalActionListDto
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String BigGoal { get; set; }
        public int Progress { get; set; }
    }
}
