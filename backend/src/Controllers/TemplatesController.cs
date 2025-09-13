using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateTemplate() => throw new NotImplementedException();
}