namespace AccessControl.Api.Contracts;

public class CreateProfileRequest
{
    public string? ExternalId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Department { get; set; }
}

public class UpdateProfileStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class ProfileResponse
{
    public Guid Id { get; set; }
    public string? ExternalId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Department { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
}