using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Business
{
    public class HardwareEquipment : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public String BrandName { get; set; }

        [Range(0, int.MaxValue)]
        public int Count { get; set; }

        public String Description { get; set; }

        [Required]
        public String SupportType { get; set; }
    }
}
