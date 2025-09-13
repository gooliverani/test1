using Microsoft.EntityFrameworkCore;
using AccessControl.Api.Models;

namespace AccessControl.Api.Data;

/// <summary>
/// Application database context for access control system
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext
    /// </summary>
    /// <param name="options">Database context options</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // New models matching PostgreSQL schema
    /// <summary>
    /// Employee profiles
    /// </summary>
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    
    /// <summary>
    /// Departments
    /// </summary>
    public DbSet<Department> Departments => Set<Department>();
    
    /// <summary>
    /// Teams
    /// </summary>
    public DbSet<Team> Teams => Set<Team>();
    
    /// <summary>
    /// Locations
    /// </summary>
    public DbSet<Location> Locations => Set<Location>();
    
    /// <summary>
    /// Access profiles
    /// </summary>
    public DbSet<AccessProfile> AccessProfiles => Set<AccessProfile>();
    
    /// <summary>
    /// Employee access assignments
    /// </summary>
    public DbSet<EmployeeAccess> EmployeeAccess => Set<EmployeeAccess>();
    
    /// <summary>
    /// Employee photos
    /// </summary>
    public DbSet<EmployeePhoto> EmployeePhotos => Set<EmployeePhoto>();
    
    /// <summary>
    /// Badge history records
    /// </summary>
    public DbSet<BadgeHistory> BadgeHistory => Set<BadgeHistory>();
    
    /// <summary>
    /// Access history records
    /// </summary>
    public DbSet<AccessHistory> AccessHistory => Set<AccessHistory>();
    
    /// <summary>
    /// Swipe records
    /// </summary>
    public DbSet<Swipe> Swipes => Set<Swipe>();
    
    /// <summary>
    /// Readers (card readers and access control devices)
    /// </summary>
    public DbSet<Reader> Readers => Set<Reader>();
    
    /// <summary>
    /// Access profile reader assignments
    /// </summary>
    public DbSet<AccessProfileReader> AccessProfileReaders => Set<AccessProfileReader>();

    // Legacy models (keeping for compatibility)
    /// <summary>
    /// Person profiles (legacy)
    /// </summary>
    public DbSet<PersonProfile> PersonProfiles => Set<PersonProfile>();
    
    /// <summary>
    /// Credentials
    /// </summary>
    public DbSet<Credential> Credentials => Set<Credential>();
    
    /// <summary>
    /// Zones
    /// </summary>
    public DbSet<Zone> Zones => Set<Zone>();
    
    /// <summary>
    /// Schedules
    /// </summary>
    public DbSet<Schedule> Schedules => Set<Schedule>();
    
    /// <summary>
    /// Zone permissions
    /// </summary>
    public DbSet<ZonePermission> ZonePermissions => Set<ZonePermission>();
    
    /// <summary>
    /// Access attempts
    /// </summary>
    public DbSet<AccessAttempt> AccessAttempts => Set<AccessAttempt>();
    
    /// <summary>
    /// Audit logs
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    
    /// <summary>
    /// Access templates
    /// </summary>
    public DbSet<AccessTemplate> AccessTemplates => Set<AccessTemplate>();
    
    /// <summary>
    /// Visitor badges
    /// </summary>
    public DbSet<VisitorBadge> VisitorBadges => Set<VisitorBadge>();
    
    /// <summary>
    /// Reason codes
    /// </summary>
    public DbSet<ReasonCode> ReasonCodes => Set<ReasonCode>();

    /// <summary>
    /// Configures the model for the database
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure EmployeeProfile entity to map to employee_profiles table
        modelBuilder.Entity<EmployeeProfile>(e =>
        {
            e.ToTable("employee_profiles");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.CompId).HasColumnName("comp_id").HasMaxLength(8).IsRequired();
            e.Property(p => p.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            e.Property(p => p.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            e.Property(p => p.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            e.Property(p => p.HireDate).HasColumnName("hire_date");
            e.Property(p => p.ExpireDate).HasColumnName("expire_date");
            e.Property(p => p.DepartmentId).HasColumnName("department_id");
            e.Property(p => p.TeamId).HasColumnName("team_id");
            e.Property(p => p.LocationId).HasColumnName("location_id");
            e.Property(p => p.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            e.Property(p => p.BadgeSerial).HasColumnName("badge_serial").HasMaxLength(20);
            e.Property(p => p.BadgePrinted).HasColumnName("badge_printed").HasDefaultValue(false);
            e.Property(p => p.BadgePrintedAt).HasColumnName("badge_printed_at");
            e.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasIndex(p => p.CompId).IsUnique();
            e.HasIndex(p => p.Email).IsUnique();
            
            // Foreign key relationships
            e.HasOne(p => p.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
                
            e.HasOne(p => p.Team)
                .WithMany(t => t.Employees)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
                
            e.HasOne(p => p.Location)
                .WithMany(l => l.Employees)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Department entity
        modelBuilder.Entity<Department>(e =>
        {
            e.ToTable("departments");
            e.HasKey(d => d.Id);
            e.Property(d => d.Id).HasColumnName("id");
            e.Property(d => d.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(d => d.Description).HasColumnName("description");
            e.Property(d => d.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(d => d.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasIndex(d => d.Name).IsUnique();
        });

        // Configure Team entity
        modelBuilder.Entity<Team>(e =>
        {
            e.ToTable("teams");
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).HasColumnName("id");
            e.Property(t => t.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(t => t.DepartmentId).HasColumnName("department_id");
            e.Property(t => t.Description).HasColumnName("description");
            e.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(t => t.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasIndex(t => new { t.Name, t.DepartmentId }).IsUnique();
            
            e.HasOne(t => t.Department)
                .WithMany(d => d.Teams)
                .HasForeignKey(t => t.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Location entity
        modelBuilder.Entity<Location>(e =>
        {
            e.ToTable("locations");
            e.HasKey(l => l.Id);
            e.Property(l => l.Id).HasColumnName("id");
            e.Property(l => l.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(l => l.Description).HasColumnName("description");
            e.Property(l => l.Address).HasColumnName("address");
            e.Property(l => l.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(l => l.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasIndex(l => l.Name).IsUnique();
        });

        // Configure AccessProfile entity
        modelBuilder.Entity<AccessProfile>(e =>
        {
            e.ToTable("access_profiles");
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasColumnName("id");
            e.Property(a => a.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(a => a.Description).HasColumnName("description");
            e.Property(a => a.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            e.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(a => a.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasIndex(a => a.Name).IsUnique();
        });

        // Configure EmployeeAccess entity
        modelBuilder.Entity<EmployeeAccess>(e =>
        {
            e.ToTable("employee_access");
            e.HasKey(ea => ea.Id);
            e.Property(ea => ea.Id).HasColumnName("id");
            e.Property(ea => ea.EmployeeId).HasColumnName("employee_id");
            e.Property(ea => ea.AccessProfileId).HasColumnName("access_profile_id");
            e.Property(ea => ea.AssignedDate).HasColumnName("assigned_date").HasDefaultValueSql("CURRENT_DATE");
            e.Property(ea => ea.ExpireDate).HasColumnName("expire_date");
            e.Property(ea => ea.Active).HasColumnName("active").HasDefaultValue(true);
            e.Property(ea => ea.AssignedVia).HasColumnName("assigned_via").HasMaxLength(20).HasDefaultValue("manual");
            e.Property(ea => ea.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(ea => ea.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasIndex(ea => new { ea.EmployeeId, ea.AccessProfileId }).IsUnique();
            
            e.HasOne(ea => ea.Employee)
                .WithMany(e => e.EmployeeAccess)
                .HasForeignKey(ea => ea.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            e.HasOne(ea => ea.AccessProfile)
                .WithMany(a => a.EmployeeAccess)
                .HasForeignKey(ea => ea.AccessProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure EmployeePhoto entity
        modelBuilder.Entity<EmployeePhoto>(e =>
        {
            e.ToTable("employee_photos");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.EmployeeId).HasColumnName("employee_id");
            e.Property(p => p.PhotoData).HasColumnName("photo_data");
            e.Property(p => p.PhotoUrl).HasColumnName("photo_url").HasMaxLength(500);
            e.Property(p => p.PhotoFilename).HasColumnName("photo_filename").HasMaxLength(255);
            e.Property(p => p.PhotoSize).HasColumnName("photo_size");
            e.Property(p => p.MimeType).HasColumnName("mime_type").HasMaxLength(100);
            e.Property(p => p.UploadedAt).HasColumnName("uploaded_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(p => p.UploadedBy).HasColumnName("uploaded_by").HasMaxLength(100);
            e.Property(p => p.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            e.Property(p => p.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasOne(p => p.Employee)
                .WithMany(e => e.Photos)
                .HasForeignKey(p => p.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure BadgeHistory entity
        modelBuilder.Entity<BadgeHistory>(e =>
        {
            e.ToTable("badge_history");
            e.HasKey(b => b.Id);
            e.Property(b => b.Id).HasColumnName("id");
            e.Property(b => b.EmployeeId).HasColumnName("employee_id");
            e.Property(b => b.BadgeSerial).HasColumnName("badge_serial").HasMaxLength(50).IsRequired();
            e.Property(b => b.ActionType).HasColumnName("action_type").HasMaxLength(50).IsRequired();
            e.Property(b => b.PreviousBadgeSerial).HasColumnName("previous_badge_serial").HasMaxLength(50);
            e.Property(b => b.IssuedDate).HasColumnName("issued_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(b => b.ExpiryDate).HasColumnName("expiry_date");
            e.Property(b => b.IssuedBy).HasColumnName("issued_by").HasMaxLength(100);
            e.Property(b => b.Reason).HasColumnName("reason").HasMaxLength(500);
            e.Property(b => b.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            e.Property(b => b.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasOne(b => b.Employee)
                .WithMany(e => e.BadgeHistory)
                .HasForeignKey(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure AccessHistory entity
        modelBuilder.Entity<AccessHistory>(e =>
        {
            e.ToTable("access_history");
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasColumnName("id");
            e.Property(a => a.EmployeeId).HasColumnName("employee_id");
            e.Property(a => a.AccessProfileId).HasColumnName("access_profile_id");
            e.Property(a => a.ActionType).HasColumnName("action_type").HasMaxLength(50).IsRequired();
            e.Property(a => a.EffectiveDate).HasColumnName("effective_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(a => a.ExpiryDate).HasColumnName("expiry_date");
            e.Property(a => a.GrantedBy).HasColumnName("granted_by").HasMaxLength(100);
            e.Property(a => a.Reason).HasColumnName("reason").HasMaxLength(500);
            e.Property(a => a.PreviousAccessProfileId).HasColumnName("previous_access_profile_id");
            e.Property(a => a.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasOne(a => a.Employee)
                .WithMany(e => e.AccessHistory)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
                
            e.HasOne(a => a.AccessProfile)
                .WithMany()
                .HasForeignKey(a => a.AccessProfileId)
                .OnDelete(DeleteBehavior.Restrict);
                
            e.HasOne(a => a.PreviousAccessProfile)
                .WithMany()
                .HasForeignKey(a => a.PreviousAccessProfileId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Swipe entity
        modelBuilder.Entity<Swipe>(e =>
        {
            e.ToTable("swipes");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("id");
            e.Property(s => s.EmployeeId).HasColumnName("employee_id");
            e.Property(s => s.ReaderId).HasColumnName("reader_id");
            e.Property(s => s.SwipeTime).HasColumnName("swipe_time").IsRequired();
            e.Property(s => s.CardNumber).HasColumnName("card_number").HasMaxLength(50);
            e.Property(s => s.AccessGranted).HasColumnName("access_granted");
            e.Property(s => s.DenialReason).HasColumnName("denial_reason").HasMaxLength(100);
            e.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasOne(s => s.Employee)
                .WithMany(e => e.Swipes)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Legacy entity configurations (keep existing)
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
            e.HasIndex(c => c.Identifier);
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

        // Configure Reader entity
        modelBuilder.Entity<Reader>(e =>
        {
            e.ToTable("readers");
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).HasColumnName("id");
            e.Property(r => r.LocationId).HasColumnName("location_id");
            e.Property(r => r.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            e.Property(r => r.DeviceType).HasColumnName("device_type").HasMaxLength(50);
            e.Property(r => r.Capabilities).HasColumnName("capabilities").HasColumnType("jsonb");
            e.Property(r => r.ChannelId).HasColumnName("channel_id");
            e.Property(r => r.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            e.Property(r => r.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            e.HasIndex(r => new { r.LocationId, r.Name }).IsUnique();
            
            // Foreign key relationships
            e.HasOne(r => r.Location)
                .WithMany()
                .HasForeignKey(r => r.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure AccessProfileReader entity (many-to-many junction table)
        modelBuilder.Entity<AccessProfileReader>(e =>
        {
            e.ToTable("access_profile_readers");
            e.HasKey(apr => new { apr.AccessProfileId, apr.ReaderId });
            e.Property(apr => apr.AccessProfileId).HasColumnName("access_profile_id");
            e.Property(apr => apr.ReaderId).HasColumnName("reader_id");
            
            // Foreign key relationships
            e.HasOne(apr => apr.AccessProfile)
                .WithMany()
                .HasForeignKey(apr => apr.AccessProfileId)
                .OnDelete(DeleteBehavior.Cascade);
                
            e.HasOne(apr => apr.Reader)
                .WithMany(r => r.AccessProfileReaders)
                .HasForeignKey(apr => apr.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}