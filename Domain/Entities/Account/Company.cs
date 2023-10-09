using Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Account
{
    public class Company
    {
        [Key]
        public Guid Id { get; set; }

        public string NameUniversity { get; set; }
        public string LogoUniversity { get; set; }
        public string PortalUrl { get; set; }
        public string SingleWindowUrl { get; set; }
        public String NationalSerial { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CityName { get; set; }
        public string ProvinceName { get; set; }
        public string UniversityType { get; set; }
        public String Status { get; set; }
        public string CreateAt { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<BigGoal> BigGoals { get; set; } = new List<BigGoal>();
    }
}
