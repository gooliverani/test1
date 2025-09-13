namespace AccessControl.Api.Contracts;

public class GrantZonePermissionRequest
{
    public Guid ZoneId { get; set; }
    public Guid ScheduleId { get; set; }
}

public class ZonePermissionResponse
{
    public Guid Id { get; set; }
    public Guid PersonProfileId { get; set; }
    public Guid ZoneId { get; set; }
    public Guid ScheduleId { get; set; }
    public DateTimeOffset GrantedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
}