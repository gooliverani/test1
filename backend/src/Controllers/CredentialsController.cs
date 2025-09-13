using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CredentialsController : ControllerBase
{
    [HttpPost("{id}/revoke")]
    public IActionResult RevokeCredential(Guid id) => throw new NotImplementedException();
}