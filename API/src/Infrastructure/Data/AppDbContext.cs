using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<JournalDetail> JournalDetails { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<DashboardSettings> DashboardSettings { get; set; }
        public DbSet<BudgetAccount> BudgetAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Period>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Account>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Journal>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<JournalDetail>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<CostCenter>().HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<JournalDetail>()
            .HasOne(d => d.Journal)
            .WithMany(j => j.JournalDetails)
            .HasForeignKey(d => d.JournalId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Journal>()
              .HasMany(j => j.JournalDetails)
              .WithOne(d => d.Journal)
              .HasForeignKey(d => d.JournalId)
              .IsRequired(true)
              .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
