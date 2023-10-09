using Domain.Entities.Account;
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
        public DbSet<ProgramYear> ProgramYears { get; set; }


        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        private DbSet<PermissionContainer> PermissionContainers { get; set; }
        private DbSet<PermissionItem> permissionItems { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserJoinRequest> UsersJoinRequests { get; set; }

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

            modelBuilder.Entity<User>()
                .HasIndex(b => b.NationalId).IsUnique(true);

            modelBuilder.Entity<BigGoal>()
                .HasOne(b => b.ProgramYear)
                .WithMany(b => b.BigGoals)
                .IsRequired(false);

            modelBuilder.Entity<PracticalAction>()
                .HasOne(b => b.Leader)
                .WithMany(b => b.PracticalActions)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Project>()
                .HasOne(b => b.Leader)
                .WithMany(b => b.Projects)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Role>()
                .Property(b => b.CompanyId)
                .IsRequired(false);

            modelBuilder.Entity<UserJoinRequest>()
                .Property(b => b.CompanyId)
                .IsRequired(false);
        }
    }
}
