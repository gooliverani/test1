namespace AccessControl.Api.Models;

public enum VisitorBadgeStatus { Active, Expired, Revoked }

public class VisitorBadge
{
    public Guid Id { get; set; }
    public Guid PersonProfileId { get; set; }
    public Guid HostProfileId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public VisitorBadgeStatus Status { get; set; }

    // Navigation
    public PersonProfile? PersonProfile { get; set; }
    public PersonProfile? HostProfile { get; set; }
}