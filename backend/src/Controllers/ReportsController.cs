using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet("access-summary")]
    public IActionResult GetAccessSummary() => throw new NotImplementedException();

    [HttpGet("zone-access")]
    public IActionResult GetZoneAccess() => throw new NotImplementedException();
}