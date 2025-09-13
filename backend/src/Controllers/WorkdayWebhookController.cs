using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/workday/webhook")]
public class WorkdayWebhookController : ControllerBase
{
    [HttpPost]
    public IActionResult ProcessWebhook() => throw new NotImplementedException();
}