namespace AccessControl.Api.Models;

public class Schedule
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TimeRules { get; set; } = string.Empty; // stored as JSON string, map to jsonb
    public string Timezone { get; set; } = "UTC";

    // Navigation
    public ICollection<ZonePermission> ZonePermissions { get; set; } = new List<ZonePermission>();
}