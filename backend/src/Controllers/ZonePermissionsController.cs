using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZonePermissionsController : ControllerBase
{
    [HttpDelete("{id}")]
    public IActionResult RevokeZonePermission(Guid id) => throw new NotImplementedException();
}