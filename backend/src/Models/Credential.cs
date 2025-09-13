namespace AccessControl.Api.Models;

public enum CredentialType { Card, Mobile, TempVisitor }
public enum CredentialStatus { Active, Revoked, Expired }

public class Credential
{
    public Guid Id { get; set; }
    public Guid PersonProfileId { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public CredentialType Type { get; set; }
    public CredentialStatus Status { get; set; }
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }

    // Navigation
    public PersonProfile? PersonProfile { get; set; }
}