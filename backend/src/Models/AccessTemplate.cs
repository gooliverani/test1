namespace AccessControl.Api.Models;

public class AccessTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Version { get; set; }
    public string TemplateData { get; set; } = "[]"; // JSONB array of zone+schedule references
}