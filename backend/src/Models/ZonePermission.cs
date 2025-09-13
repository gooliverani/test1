namespace AccessControl.Api.Models;

public class ZonePermission
{
    public Guid Id { get; set; }
    public Guid PersonProfileId { get; set; }
    public Guid ZoneId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }

    // Navigation
    public PersonProfile? PersonProfile { get; set; }
    public Zone? Zone { get; set; }
    public Schedule? Schedule { get; set; }
}