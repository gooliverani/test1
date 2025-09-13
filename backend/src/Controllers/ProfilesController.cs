using Microsoft.AspNetCore.Mvc;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateProfile() => throw new NotImplementedException();

    [HttpGet("{id}")]
    public IActionResult GetProfile(Guid id) => throw new NotImplementedException();

    [HttpGet]
    public IActionResult ListProfiles() => throw new NotImplementedException();

    [HttpPatch("{id}")]
    public IActionResult UpdateProfileStatus(Guid id) => throw new NotImplementedException();

    [HttpPost("{id}/credentials")]
    public IActionResult IssueCredential(Guid id) => throw new NotImplementedException();

    [HttpPost("{id}/zone-permissions")]
    public IActionResult GrantZonePermission(Guid id) => throw new NotImplementedException();

    [HttpPost("{id}/apply-template/{templateId}")]
    public IActionResult ApplyTemplate(Guid id, Guid templateId) => throw new NotImplementedException();
}