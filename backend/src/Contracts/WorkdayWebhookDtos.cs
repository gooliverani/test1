namespace AccessControl.Api.Contracts;

public class WorkdayWebhookRequest
{
    public object Payload { get; set; } = new(); // Replace with actual schema if known
}

public class WorkdayWebhookResponse
{
    public string Status { get; set; } = string.Empty;
}