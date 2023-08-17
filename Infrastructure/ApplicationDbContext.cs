using Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BigGoal> BigGoals { get; set; }
        public DbSet<OperationalObjective> OperationalObjectives { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<HardwareEquipment> HardwareEquipment { get; set; }
        public DbSet<Domain.Entities.Business.System> Systems { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<PracticalAction> PracticalActions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
                .OwnsMany(b => b.Expertises, b =>
                {
                    b.Property(b => b.Title).IsRequired().HasColumnName("title");
                });

            modelBuilder.Entity<Project>()
                .OwnsMany(b => b.Financials, b =>
                {
                    b.Property(b => b.Title).IsRequired().HasColumnName("title");
                });

            modelBuilder.Entity<PracticalAction>()
                .OwnsMany(b => b.Financials, b =>
                {
                    b.Property(b => b.Title).IsRequired().HasColumnName("title");
                });
        }
    }
}
