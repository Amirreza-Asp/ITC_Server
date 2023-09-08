namespace Domain.Entities.Account
{
    public class Company : BaseEntity
    {
        public String Title { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
