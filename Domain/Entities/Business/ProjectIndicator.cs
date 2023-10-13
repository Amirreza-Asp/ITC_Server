using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class ProjectIndicator
    {
        public Guid ProjectId { get; set; }
        public Guid IndicatorId { get; set; }


        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        [ForeignKey(nameof(IndicatorId))]
        public Indicator Indicator { get; set; }
    }
}
