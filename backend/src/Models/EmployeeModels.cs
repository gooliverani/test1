namespace AccessControl.Api.Models;

/// <summary>
/// Employee profile entity matching the PostgreSQL employee_profiles table
/// </summary>
public class EmployeeProfile
{
    /// <summary>
    /// Primary key identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Company ID (8 characters: first letter of first name + first letter of last name + 6 digits)
    /// </summary>
    public string CompId { get; set; } = string.Empty;
    
    /// <summary>
    /// Employee first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Employee last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Employee email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Date of hire
    /// </summary>
    public DateOnly HireDate { get; set; }
    
    /// <summary>
    /// Badge expiration date (optional)
    /// </summary>
    public DateOnly? ExpireDate { get; set; }
    
    /// <summary>
    /// Department identifier
    /// </summary>
    public int DepartmentId { get; set; }
    
    /// <summary>
    /// Team identifier (optional)
    /// </summary>
    public int? TeamId { get; set; }
    
    /// <summary>
    /// Location identifier
    /// </summary>
    public int LocationId { get; set; }
    
    /// <summary>
    /// Whether the employee is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Badge serial number (optional)
    /// </summary>
    public string? BadgeSerial { get; set; }
    
    /// <summary>
    /// Whether the badge has been printed
    /// </summary>
    public bool BadgePrinted { get; set; } = false;
    
    /// <summary>
    /// When the badge was printed (optional)
    /// </summary>
    public DateTimeOffset? BadgePrintedAt { get; set; }
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Department navigation property
    /// </summary>
    public Department? Department { get; set; }
    
    /// <summary>
    /// Team navigation property
    /// </summary>
    public Team? Team { get; set; }
    
    /// <summary>
    /// Location navigation property
    /// </summary>
    public Location? Location { get; set; }
    
    /// <summary>
    /// Employee access assignments
    /// </summary>
    public ICollection<EmployeeAccess> EmployeeAccess { get; set; } = new List<EmployeeAccess>();
    
    /// <summary>
    /// Employee photos
    /// </summary>
    public ICollection<EmployeePhoto> Photos { get; set; } = new List<EmployeePhoto>();
    
    /// <summary>
    /// Badge history for this employee
    /// </summary>
    public ICollection<BadgeHistory> BadgeHistory { get; set; } = new List<BadgeHistory>();
    
    /// <summary>
    /// Access history for this employee
    /// </summary>
    public ICollection<AccessHistory> AccessHistory { get; set; } = new List<AccessHistory>();
    
    /// <summary>
    /// Swipe records for this employee
    /// </summary>
    public ICollection<Swipe> Swipes { get; set; } = new List<Swipe>();
}

/// <summary>
/// Department entity
/// </summary>
public class Department
{
    /// <summary>
    /// Department identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Department name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Department description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Employees in this department
    /// </summary>
    public ICollection<EmployeeProfile> Employees { get; set; } = new List<EmployeeProfile>();
    
    /// <summary>
    /// Teams in this department
    /// </summary>
    public ICollection<Team> Teams { get; set; } = new List<Team>();
}

/// <summary>
/// Team entity
/// </summary>
public class Team
{
    /// <summary>
    /// Team identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Team name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Department this team belongs to
    /// </summary>
    public int DepartmentId { get; set; }
    
    /// <summary>
    /// Team description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Department navigation property
    /// </summary>
    public Department? Department { get; set; }
    
    /// <summary>
    /// Employees in this team
    /// </summary>
    public ICollection<EmployeeProfile> Employees { get; set; } = new List<EmployeeProfile>();
}

/// <summary>
/// Location entity
/// </summary>
public class Location
{
    /// <summary>
    /// Location identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Location name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Location description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Physical address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Employees at this location
    /// </summary>
    public ICollection<EmployeeProfile> Employees { get; set; } = new List<EmployeeProfile>();
}

/// <summary>
/// Access profile entity
/// </summary>
public class AccessProfile
{
    /// <summary>
    /// Access profile identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Access profile name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Access profile description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether the access profile is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Employee access assignments for this profile
    /// </summary>
    public ICollection<EmployeeAccess> EmployeeAccess { get; set; } = new List<EmployeeAccess>();
}

/// <summary>
/// Employee access assignment entity
/// </summary>
public class EmployeeAccess
{
    /// <summary>
    /// Assignment identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Employee identifier
    /// </summary>
    public int EmployeeId { get; set; }
    
    /// <summary>
    /// Access profile identifier
    /// </summary>
    public int AccessProfileId { get; set; }
    
    /// <summary>
    /// Date access was assigned
    /// </summary>
    public DateOnly AssignedDate { get; set; }
    
    /// <summary>
    /// Access expiration date (optional)
    /// </summary>
    public DateOnly? ExpireDate { get; set; }
    
    /// <summary>
    /// Whether the access is currently active
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// How the access was assigned (manual, automation, bulk_import)
    /// </summary>
    public string AssignedVia { get; set; } = "manual";
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Employee navigation property
    /// </summary>
    public EmployeeProfile? Employee { get; set; }
    
    /// <summary>
    /// Access profile navigation property
    /// </summary>
    public AccessProfile? AccessProfile { get; set; }
}

/// <summary>
/// Employee photo entity
/// </summary>
public class EmployeePhoto
{
    /// <summary>
    /// Photo identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Employee identifier
    /// </summary>
    public int EmployeeId { get; set; }
    
    /// <summary>
    /// Binary photo data (alternative to URL)
    /// </summary>
    public byte[]? PhotoData { get; set; }
    
    /// <summary>
    /// External photo URL (alternative to binary data)
    /// </summary>
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Original filename
    /// </summary>
    public string? PhotoFilename { get; set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public int? PhotoSize { get; set; }
    
    /// <summary>
    /// MIME type of the photo
    /// </summary>
    public string? MimeType { get; set; }
    
    /// <summary>
    /// When the photo was uploaded
    /// </summary>
    public DateTimeOffset UploadedAt { get; set; }
    
    /// <summary>
    /// Who uploaded the photo
    /// </summary>
    public string? UploadedBy { get; set; }
    
    /// <summary>
    /// Whether this is the active photo for the employee
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Employee navigation property
    /// </summary>
    public EmployeeProfile? Employee { get; set; }
}

/// <summary>
/// Badge history entity for tracking badge lifecycle
/// </summary>
public class BadgeHistory
{
    /// <summary>
    /// Badge history identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Employee identifier
    /// </summary>
    public int EmployeeId { get; set; }
    
    /// <summary>
    /// Badge serial number
    /// </summary>
    public string BadgeSerial { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of action (badge_issued, badge_updated, badge_deactivated, etc.)
    /// </summary>
    public string ActionType { get; set; } = string.Empty;
    
    /// <summary>
    /// Previous badge serial (if updating)
    /// </summary>
    public string? PreviousBadgeSerial { get; set; }
    
    /// <summary>
    /// Date the badge was issued
    /// </summary>
    public DateTimeOffset IssuedDate { get; set; }
    
    /// <summary>
    /// Badge expiration date
    /// </summary>
    public DateOnly? ExpiryDate { get; set; }
    
    /// <summary>
    /// Who issued the badge
    /// </summary>
    public string? IssuedBy { get; set; }
    
    /// <summary>
    /// Reason for the badge action
    /// </summary>
    public string? Reason { get; set; }
    
    /// <summary>
    /// Whether the badge is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Employee navigation property
    /// </summary>
    public EmployeeProfile? Employee { get; set; }
}

/// <summary>
/// Access history entity for tracking access profile changes
/// </summary>
public class AccessHistory
{
    /// <summary>
    /// Access history identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Employee identifier
    /// </summary>
    public int EmployeeId { get; set; }
    
    /// <summary>
    /// Access profile identifier
    /// </summary>
    public int AccessProfileId { get; set; }
    
    /// <summary>
    /// Type of action (access_granted, access_revoked, access_updated, etc.)
    /// </summary>
    public string ActionType { get; set; } = string.Empty;
    
    /// <summary>
    /// When the access change takes effect
    /// </summary>
    public DateTimeOffset EffectiveDate { get; set; }
    
    /// <summary>
    /// When the access expires (optional)
    /// </summary>
    public DateTimeOffset? ExpiryDate { get; set; }
    
    /// <summary>
    /// Who granted the access
    /// </summary>
    public string? GrantedBy { get; set; }
    
    /// <summary>
    /// Reason for the access change
    /// </summary>
    public string? Reason { get; set; }
    
    /// <summary>
    /// Previous access profile (if updating)
    /// </summary>
    public int? PreviousAccessProfileId { get; set; }
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Employee navigation property
    /// </summary>
    public EmployeeProfile? Employee { get; set; }
    
    /// <summary>
    /// Access profile navigation property
    /// </summary>
    public AccessProfile? AccessProfile { get; set; }
    
    /// <summary>
    /// Previous access profile navigation property
    /// </summary>
    public AccessProfile? PreviousAccessProfile { get; set; }
}

/// <summary>
/// Swipe entity for tracking door access attempts
/// </summary>
public class Swipe
{
    /// <summary>
    /// Swipe identifier
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Employee identifier (nullable for unknown cards)
    /// </summary>
    public int? EmployeeId { get; set; }
    
    /// <summary>
    /// Reader identifier
    /// </summary>
    public int ReaderId { get; set; }
    
    /// <summary>
    /// When the swipe occurred
    /// </summary>
    public DateTimeOffset SwipeTime { get; set; }
    
    /// <summary>
    /// Card number that was swiped
    /// </summary>
    public string? CardNumber { get; set; }
    
    /// <summary>
    /// Whether access was granted
    /// </summary>
    public bool AccessGranted { get; set; }
    
    /// <summary>
    /// Reason for denial (if access was denied)
    /// </summary>
    public string? DenialReason { get; set; }
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Employee navigation property
    /// </summary>
    public EmployeeProfile? Employee { get; set; }
    
    /// <summary>
    /// Reader navigation property
    /// </summary>
    public Reader? Reader { get; set; }
}

/// <summary>
/// Reader entity representing card readers and access control devices
/// </summary>
public class Reader
{
    /// <summary>
    /// Reader identifier
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Location where the reader is installed
    /// </summary>
    public int LocationId { get; set; }
    
    /// <summary>
    /// Reader name/description
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of device (card reader, biometric, etc.)
    /// </summary>
    public string? DeviceType { get; set; }
    
    /// <summary>
    /// Reader capabilities in JSON format
    /// </summary>
    public string? Capabilities { get; set; }
    
    /// <summary>
    /// Channel ID for hardware configuration
    /// </summary>
    public int? ChannelId { get; set; }
    
    /// <summary>
    /// When the record was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// When the record was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // Navigation properties
    /// <summary>
    /// Location where this reader is installed
    /// </summary>
    public Location? Location { get; set; }
    
    /// <summary>
    /// Access profile readers assignments
    /// </summary>
    public ICollection<AccessProfileReader> AccessProfileReaders { get; set; } = new List<AccessProfileReader>();
    
    /// <summary>
    /// Swipe records from this reader
    /// </summary>
    public ICollection<Swipe> Swipes { get; set; } = new List<Swipe>();
}

/// <summary>
/// Junction entity linking access profiles to readers
/// </summary>
public class AccessProfileReader
{
    /// <summary>
    /// Access profile identifier
    /// </summary>
    public int AccessProfileId { get; set; }
    
    /// <summary>
    /// Reader identifier
    /// </summary>
    public int ReaderId { get; set; }

    // Navigation properties
    /// <summary>
    /// Access profile navigation property
    /// </summary>
    public AccessProfile? AccessProfile { get; set; }
    
    /// <summary>
    /// Reader navigation property
    /// </summary>
    public Reader? Reader { get; set; }
}

/// <summary>
/// Request model for creating a new employee
/// </summary>
public class CreateEmployeeRequest
{
    /// <summary>
    /// Employee first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Employee last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Employee email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Department identifier
    /// </summary>
    public int DepartmentId { get; set; }
    
    /// <summary>
    /// Team identifier (optional)
    /// </summary>
    public int? TeamId { get; set; }
    
    /// <summary>
    /// Location identifier
    /// </summary>
    public int LocationId { get; set; }
    
    /// <summary>
    /// Date of hire
    /// </summary>
    public DateOnly HireDate { get; set; }
    
    /// <summary>
    /// Badge expiration date (optional)
    /// </summary>
    public DateOnly? ExpireDate { get; set; }
    
    /// <summary>
    /// Badge serial number (optional)
    /// </summary>
    public string? BadgeSerial { get; set; }
    
    /// <summary>
    /// Whether the employee profile is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}