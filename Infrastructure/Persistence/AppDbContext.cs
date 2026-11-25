using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{

    public DbSet<DashboardAccount> DashboardAccounts { get; set; }
    public DbSet<Visitor> Visitors { get; set; }
    public DbSet<Registration_Request> Registration_Requests{ get; set; }
    public DbSet<Lab> Labs { get; set; }
    public DbSet<EntryLog> EntryLogs { get; set; }
    public DbSet<Warning> Warnings { get; set; }
    public DbSet<LogRotationConfig> LogRotationConfigs { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // A visitor and dashboard account represent unique identities. Keep
        // these unique even when a record is deactivated (soft-deleted).
        modelBuilder.Entity<Visitor>()
            .HasIndex(visitor => visitor.MemberId)
            .IsUnique();
        modelBuilder.Entity<Visitor>()
            .HasIndex(visitor => visitor.Email)
            .IsUnique();
        modelBuilder.Entity<DashboardAccount>()
            .HasIndex(account => account.Email)
            .IsUnique();

        // Keep active registration submissions unique, but allow a rejected
        // or accepted request to remain in history while a later request is made.
        modelBuilder.Entity<Registration_Request>()
            .HasIndex(request => request.MemberId)
            .IsUnique()
            .HasFilter("[status] = 0");
        modelBuilder.Entity<Registration_Request>()
            .HasIndex(request => request.Email)
            .IsUnique()
            .HasFilter("[status] = 0");

        modelBuilder.Entity<EntryLog>()
            .HasOne(e => e.Visitor)
            .WithMany()
            .HasForeignKey(e => e.VisitorId)
            .OnDelete(DeleteBehavior.SetNull); // <-- SET NULL


        modelBuilder.Entity<EntryLog>()
            .HasOne(e => e.Lab)
            .WithMany()
            .HasForeignKey(e => e.LabId)
            .OnDelete(DeleteBehavior.SetNull); // <-- SET NULL ON LAB DELETE


        modelBuilder.Entity<Warning>()
            .HasOne(w => w.Lab)
            .WithMany()
            .HasForeignKey(w => w.LabId)
            .OnDelete(DeleteBehavior.SetNull); // <
    }




}
