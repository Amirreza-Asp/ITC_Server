using Domain.SubEntities;
using Domain.Utiltiy;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class PracticalAction : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        public String Contractor { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        public List<Financial> Financials { get; set; } = new List<Financial>();

        [NotMapped]
        public int Progress => Calculator.CalcProgress(Indicators.Select(e => e.Indicator));

        [ForeignKey(nameof(Leader))]
        public Guid LeaderId { get; set; }


        [ForeignKey(nameof(OperationalObjective))]
        public Guid OperationalObjectiveId { get; set; }

        public OperationalObjective OperationalObjective { get; set; }
        public Person Leader { get; set; }
        public ICollection<PracticalActionIndicator> Indicators { get; set; } = new List<PracticalActionIndicator>();
    }

}
