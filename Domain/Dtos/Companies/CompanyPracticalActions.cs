using Domain.Dtos.PracticalActions;

namespace Domain.Dtos.Companies
{
    public class CompanyPracticalActions
    {
        public Guid CompanyId { get; set; }
        public String CompanyName { get; set; }
        public List<PracticalActionListDto> PracticalActions { get; set; }
    }
}
