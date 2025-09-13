using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReasonCodesController : ControllerBase
{
    private readonly ReasonCodeService _reasonCodes;

    public ReasonCodesController(ReasonCodeService reasonCodes)
    {
        _reasonCodes = reasonCodes;
    }

    [HttpGet]
    public IActionResult ListReasonCodes() => throw new NotImplementedException();
}