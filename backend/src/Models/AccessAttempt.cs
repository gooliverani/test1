namespace AccessControl.Api.Models;

public enum AccessOutcome { Allow, Deny }

public class AccessAttempt
{
    public long Id { get; set; } // bigserial
    public Guid CredentialId { get; set; }
    public Guid ZoneId { get; set; }
    public AccessOutcome Outcome { get; set; }
    public string? ReasonCode { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    // Navigation
    public Credential? Credential { get; set; }
    public Zone? Zone { get; set; }
}