using Domain.SubEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class Project : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        public String Contractor { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime GuaranteedFulfillmentAt { get; set; }

        public List<Financial> Financials { get; set; } = new List<Financial>();

        [Range(0, 100)]
        public short Progress { get; set; }

        [ForeignKey(nameof(Leader))]
        public Guid LeaderId { get; set; }


        [ForeignKey(nameof(OperationalObjective))]
        public Guid OperationalObjectiveId { get; set; }

        public OperationalObjective OperationalObjective { get; set; }
        public Person Leader { get; set; }
    }

}
