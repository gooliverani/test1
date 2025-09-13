namespace AccessControl.Api.Contracts;

public class IssueVisitorBadgeRequest
{
    public Guid PersonProfileId { get; set; }
    public Guid HostProfileId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}

public class VisitorBadgeResponse
{
    public Guid Id { get; set; }
    public Guid PersonProfileId { get; set; }
    public Guid HostProfileId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public string Status { get; set; } = string.Empty;
}