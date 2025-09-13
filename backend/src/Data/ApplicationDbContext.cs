using Microsoft.EntityFrameworkCore;
using AccessControl.Api.Models;

namespace AccessControl.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<PersonProfile> PersonProfiles => Set<PersonProfile>();
    public DbSet<Credential> Credentials => Set<Credential>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<ZonePermission> ZonePermissions => Set<ZonePermission>();

    public DbSet<AccessAttempt> AccessAttempts => Set<AccessAttempt>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AccessTemplate> AccessTemplates => Set<AccessTemplate>();
    public DbSet<VisitorBadge> VisitorBadges => Set<VisitorBadge>();
    public DbSet<ReasonCode> ReasonCodes => Set<ReasonCode>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PersonProfile
        modelBuilder.Entity<PersonProfile>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.ExternalId).IsUnique();
            e.HasIndex(p => p.Email).IsUnique();
            e.Property(p => p.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
        });

        // Credential
        modelBuilder.Entity<Credential>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.Identifier); // partial unique active constraint not directly expressed w/o raw SQL migration
            e.HasOne(c => c.PersonProfile)
                .WithMany(p => p.Credentials)
                .HasForeignKey(c => c.PersonProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Zone
        modelBuilder.Entity<Zone>(e =>
        {
            e.HasKey(z => z.Id);
            e.HasIndex(z => z.Name).IsUnique();
        });

        // Schedule
        modelBuilder.Entity<Schedule>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.Name).IsUnique();
            e.Property(s => s.TimeRules).HasColumnType("jsonb");
        });

        // ZonePermission
        modelBuilder.Entity<ZonePermission>(e =>
        {
            e.HasKey(zp => zp.Id);
            e.HasOne(zp => zp.PersonProfile)
                .WithMany(p => p.ZonePermissions)
                .HasForeignKey(zp => zp.PersonProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(zp => zp.Zone)
                .WithMany(z => z.ZonePermissions)
                .HasForeignKey(zp => zp.ZoneId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(zp => zp.Schedule)
                .WithMany(s => s.ZonePermissions)
                .HasForeignKey(zp => zp.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);
            // Composite uniqueness (active): (PersonProfileId, ZoneId) when RevokedAt IS NULL
            // Represented via full index; partial filter to be added manually in migration placeholder.
        });

        // AccessAttempt
        modelBuilder.Entity<AccessAttempt>(e =>
        {
            e.HasKey(a => a.Id);
            e.HasOne(a => a.Credential)
                .WithMany()
                .HasForeignKey(a => a.CredentialId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(a => a.Zone)
                .WithMany()
                .HasForeignKey(a => a.ZoneId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(a => a.Timestamp);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Data).HasColumnType("jsonb");
            e.HasIndex(a => new { a.EntityType, a.EntityId });
            e.HasIndex(a => a.CreatedAt);
        });

        // AccessTemplate
        modelBuilder.Entity<AccessTemplate>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasIndex(t => t.Name).IsUnique();
            e.Property(t => t.TemplateData).HasColumnType("jsonb");
        });

        // VisitorBadge
        modelBuilder.Entity<VisitorBadge>(e =>
        {
            e.HasKey(v => v.Id);
            e.HasOne(v => v.PersonProfile)
                .WithMany()
                .HasForeignKey(v => v.PersonProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(v => v.HostProfile)
                .WithMany()
                .HasForeignKey(v => v.HostProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ReasonCode
        modelBuilder.Entity<ReasonCode>(e =>
        {
            e.HasKey(r => r.Code);
        });
    }
}