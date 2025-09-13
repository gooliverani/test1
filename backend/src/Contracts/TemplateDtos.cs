namespace AccessControl.Api.Contracts;

public class CreateTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public int Version { get; set; }
    public string TemplateData { get; set; } = "[]"; // JSON array
}

public class ApplyTemplateRequest
{
    // No body, just route params
}

public class TemplateResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Version { get; set; }
    public string TemplateData { get; set; } = "[]";
}