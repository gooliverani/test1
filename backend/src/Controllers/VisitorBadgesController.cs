using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitorBadgesController : ControllerBase
{
    [HttpPost]
    public IActionResult IssueBadge() => throw new NotImplementedException();

    [HttpPost("{id}/revoke")]
    public IActionResult RevokeBadge(Guid id) => throw new NotImplementedException();
}