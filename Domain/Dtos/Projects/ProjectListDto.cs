namespace Domain.Dtos.Projects
{
    public class ProjectListDto
    {
        public Guid Id { get; set; }
        public String Title { get; set; }
        public String BigGoal { get; set; }
        public int Progress { get; set; }
    }
}
