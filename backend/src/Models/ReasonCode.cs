namespace AccessControl.Api.Models;

public class ReasonCode
{
    public string Code { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Active { get; set; }
}