namespace AccessControl.Api.Models;

public enum ProfileType { Employee, Contractor, Visitor }
public enum ProfileStatus { Active, Suspended, Deactivated }

public class PersonProfile
{
    public Guid Id { get; set; }
    public string? ExternalId { get; set; }
    public ProfileType Type { get; set; }
    public ProfileStatus Status { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Department { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }

    // Navigation
    public ICollection<Credential> Credentials { get; set; } = new List<Credential>();
    public ICollection<ZonePermission> ZonePermissions { get; set; } = new List<ZonePermission>();
}