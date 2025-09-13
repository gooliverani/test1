using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    // If a SchedulesService is created, inject here. For now, stub DI pattern.
    public SchedulesController() { }

    [HttpGet]
    public IActionResult ListSchedules() => throw new NotImplementedException();

    [HttpPost]
    public IActionResult CreateSchedule() => throw new NotImplementedException();
}