namespace AccessControl.Api.Contracts;

public class AccessSummaryReportResponse
{
    public int TotalAttempts { get; set; }
    public int Allowed { get; set; }
    public int Denied { get; set; }
    // Add more fields as needed
}

public class ZoneAccessReportResponse
{
    public Guid ZoneId { get; set; }
    public int TotalAttempts { get; set; }
    public int Allowed { get; set; }
    public int Denied { get; set; }
    // Add more fields as needed
}