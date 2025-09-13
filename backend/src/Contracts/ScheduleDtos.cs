namespace AccessControl.Api.Contracts;

public class CreateScheduleRequest
{
    public string Name { get; set; } = string.Empty;
    public string TimeRules { get; set; } = string.Empty; // JSON string
    public string Timezone { get; set; } = "UTC";
}

public class ScheduleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TimeRules { get; set; } = string.Empty;
    public string Timezone { get; set; } = "UTC";
}