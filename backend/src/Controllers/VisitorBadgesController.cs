using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitorBadgesController : ControllerBase
{
    private readonly VisitorBadgeService _badges;

    public VisitorBadgesController(VisitorBadgeService badges)
    {
        _badges = badges;
    }

    [HttpPost]
    public IActionResult IssueBadge() => throw new NotImplementedException();

    [HttpPost("{id}/revoke")]
    public IActionResult RevokeBadge(Guid id) => throw new NotImplementedException();
}