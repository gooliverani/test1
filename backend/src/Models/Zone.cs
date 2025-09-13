namespace AccessControl.Api.Models;

public class Zone
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation
    public ICollection<ZonePermission> ZonePermissions { get; set; } = new List<ZonePermission>();
}