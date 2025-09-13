using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZonesController : ControllerBase
{
    // If a ZoneService is created, inject here. For now, stub DI pattern.
    public ZonesController() { }

    [HttpGet]
    public IActionResult ListZones() => throw new NotImplementedException();

    [HttpPost]
    public IActionResult CreateZone() => throw new NotImplementedException();
}