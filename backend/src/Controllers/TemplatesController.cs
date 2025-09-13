using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly TemplateService _templates;

    public TemplatesController(TemplateService templates)
    {
        _templates = templates;
    }

    [HttpPost]
    public IActionResult CreateTemplate() => throw new NotImplementedException();
}