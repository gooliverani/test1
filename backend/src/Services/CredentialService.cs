using AccessControl.Api.Models;

using AccessControl.Api.Data;

namespace AccessControl.Api.Services;

public class CredentialService
{
    public CredentialService(ApplicationDbContext db)
    {
        // Inject context or other dependencies
    }

    public Task<Credential> IssueCredentialAsync(Guid personProfileId, Credential credential) => throw new NotImplementedException();
    public Task<Credential> RevokeCredentialAsync(Guid credentialId, string reason) => throw new NotImplementedException();
}