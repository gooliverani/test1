namespace AccessControl.Api.Contracts;

public class CreateZoneRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class ZoneResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}