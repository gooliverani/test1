using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CredentialsController : ControllerBase
{
    private readonly CredentialService _credentials;

    public CredentialsController(CredentialService credentials)
    {
        _credentials = credentials;
    }

    [HttpPost("{id}/revoke")]
    public IActionResult RevokeCredential(Guid id) => throw new NotImplementedException();
}