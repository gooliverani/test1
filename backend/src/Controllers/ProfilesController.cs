
using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Contracts;
using AccessControl.Api.Models;
using AccessControl.Api.Services;

namespace AccessControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly ProfileService _profiles;
    private readonly CredentialService _credentials;
    private readonly ZonePermissionService _zonePermissions;
    private readonly TemplateService _templates;

    public ProfilesController(ProfileService profiles, CredentialService credentials, ZonePermissionService zonePermissions, TemplateService templates)
    {
        _profiles = profiles;
        _credentials = credentials;
        _zonePermissions = zonePermissions;
        _templates = templates;
    }


    [HttpPost]
    public async Task<ActionResult<ProfileResponse>> CreateProfile([FromBody] CreateProfileRequest request)
    {
        var profile = new PersonProfile
        {
            ExternalId = request.ExternalId,
            Type = Enum.TryParse<ProfileType>(request.Type, true, out var type) ? type : ProfileType.Employee,
            Status = ProfileStatus.Active,
            DisplayName = request.DisplayName,
            Email = request.Email,
            Department = request.Department,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var created = await _profiles.CreateProfileAsync(profile);
        return CreatedAtAction(nameof(GetProfile), new { id = created.Id }, ToProfileResponse(created));
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProfileResponse>> GetProfile(Guid id)
    {
        var profile = await _profiles.GetProfileAsync(id);
        if (profile == null) return NotFound();
        return ToProfileResponse(profile);
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfileResponse>>> ListProfiles()
    {
        var profiles = await _profiles.ListProfilesAsync();
        return Ok(profiles.Select(ToProfileResponse));
    }


    [HttpPatch("{id}")]
    public async Task<ActionResult<ProfileResponse>> UpdateProfileStatus(Guid id, [FromBody] UpdateProfileStatusRequest request)
    {
        if (!Enum.TryParse<ProfileStatus>(request.Status, true, out var status))
            return BadRequest("Invalid status");
        var updated = await _profiles.UpdateProfileStatusAsync(id, status);
        return ToProfileResponse(updated);
    }
    private static ProfileResponse ToProfileResponse(PersonProfile p) => new()
    {
        Id = p.Id,
        ExternalId = p.ExternalId,
        Type = p.Type.ToString(),
        Status = p.Status.ToString(),
        DisplayName = p.DisplayName,
        Email = p.Email,
        Department = p.Department,
        CreatedAt = p.CreatedAt,
        ModifiedAt = p.ModifiedAt
    };

    [HttpPost("{id}/credentials")]
    public IActionResult IssueCredential(Guid id) => throw new NotImplementedException();

    [HttpPost("{id}/zone-permissions")]
    public IActionResult GrantZonePermission(Guid id) => throw new NotImplementedException();

    [HttpPost("{id}/apply-template/{templateId}")]
    public IActionResult ApplyTemplate(Guid id, Guid templateId) => throw new NotImplementedException();
}