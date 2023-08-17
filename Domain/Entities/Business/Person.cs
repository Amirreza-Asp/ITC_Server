using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Business
{
    public class Person : BaseEntity
    {
        [Required]
        public String Name { get; set; }

        [Required]
        public String Family { get; set; }

        [Required]
        public String JobTitle { get; set; }

        [Required]
        public String Education { get; set; }

        public List<Expertise> Expertises { get; set; } = new List<Expertise>();
    }

    public class Expertise
    {
        [Required]
        public String Title { get; set; }
    }
}
