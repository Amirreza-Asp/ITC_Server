using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class PracticalActionIndicator
    {
        public Guid PracticalActionId { get; set; }
        public Guid IndicatorId { get; set; }


        [ForeignKey(nameof(PracticalActionId))]
        public PracticalAction PracticalAction { get; set; }

        [ForeignKey(nameof(IndicatorId))]
        public Indicator Indicator { get; set; }
    }
}
