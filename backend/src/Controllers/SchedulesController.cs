using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    [HttpGet]
    public IActionResult ListSchedules() => throw new NotImplementedException();

    [HttpPost]
    public IActionResult CreateSchedule() => throw new NotImplementedException();
}