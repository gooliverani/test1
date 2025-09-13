namespace AccessControl.Api.Services;

public class ReportingService
{
    public ReportingService(ApplicationDbContext db)
    {
        // Inject context or other dependencies
    }

    public Task<object> GetAccessSummaryAsync(DateTimeOffset from, DateTimeOffset to) => throw new NotImplementedException();
    public Task<object> GetZoneAccessReportAsync(Guid zoneId, DateTimeOffset from, DateTimeOffset to) => throw new NotImplementedException();
    public Task<string> ExportZoneAccessCsvAsync(Guid zoneId, DateTimeOffset from, DateTimeOffset to) => throw new NotImplementedException();
}