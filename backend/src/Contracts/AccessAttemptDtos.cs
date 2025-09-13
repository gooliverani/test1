namespace AccessControl.Api.Contracts;

public class AccessAttemptResponse
{
    public long Id { get; set; }
    public Guid CredentialId { get; set; }
    public Guid ZoneId { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}