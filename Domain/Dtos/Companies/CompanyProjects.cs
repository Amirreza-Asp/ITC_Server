using Domain.Dtos.Projects;

namespace Domain.Dtos.Companies
{
    public class CompanyProjects
    {
        public Guid CompanyId { get; set; }
        public String CompanyName { get; set; }
        public List<ProjectListDto> Projects { get; set; }
    }
}
