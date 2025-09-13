using AccessControl.Api.Models;

using AccessControl.Api.Data;

namespace AccessControl.Api.Services;

public class AccessDecisionService
{
    public AccessDecisionService(ApplicationDbContext db)
    {
        // Inject context or other dependencies
    }

    public Task<bool> EvaluateAccessAsync(Guid credentialId, Guid zoneId, DateTimeOffset timestamp)
    {
        // Evaluate access logic (stub)
        throw new NotImplementedException();
    }
}