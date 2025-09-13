namespace AccessControl.Api.Models;

public class AuditLog
{
    public long Id { get; set; } // bigserial
    public Guid? ActorId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Data { get; set; } = "{}"; // JSONB
    public DateTimeOffset CreatedAt { get; set; }
}