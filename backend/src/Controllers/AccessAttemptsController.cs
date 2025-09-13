using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccessAttemptsController : ControllerBase
{
    [HttpGet]
    public IActionResult ListAccessAttempts() => throw new NotImplementedException();
}