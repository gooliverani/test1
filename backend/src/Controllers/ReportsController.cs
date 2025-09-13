using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportingService _reporting;

    public ReportsController(ReportingService reporting)
    {
        _reporting = reporting;
    }

    [HttpGet("access-summary")]
    public IActionResult GetAccessSummary() => throw new NotImplementedException();

    [HttpGet("zone-access")]
    public IActionResult GetZoneAccess() => throw new NotImplementedException();
}