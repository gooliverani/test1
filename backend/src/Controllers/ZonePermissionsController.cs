using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZonePermissionsController : ControllerBase
{
    private readonly ZonePermissionService _zonePermissions;

    public ZonePermissionsController(ZonePermissionService zonePermissions)
    {
        _zonePermissions = zonePermissions;
    }

    [HttpDelete("{id}")]
    public IActionResult RevokeZonePermission(Guid id) => throw new NotImplementedException();
}