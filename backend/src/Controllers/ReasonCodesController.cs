using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReasonCodesController : ControllerBase
{
    [HttpGet]
    public IActionResult ListReasonCodes() => throw new NotImplementedException();
}