namespace AccessControl.Api.Contracts;

public class IssueCredentialRequest
{
    public string Identifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset? ExpiresAt { get; set; }
}

public class RevokeCredentialRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class CredentialResponse
{
    public Guid Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
}