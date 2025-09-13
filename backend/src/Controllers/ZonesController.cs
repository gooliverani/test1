using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZonesController : ControllerBase
{
    [HttpGet]
    public IActionResult ListZones() => throw new NotImplementedException();

    [HttpPost]
    public IActionResult CreateZone() => throw new NotImplementedException();
}