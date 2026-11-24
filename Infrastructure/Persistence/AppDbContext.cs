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