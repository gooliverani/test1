using AccessControl.Api.Models;

namespace AccessControl.Api.Services;

public class VisitorBadgeService
{
    public VisitorBadgeService(ApplicationDbContext db)
    {
        // Inject context or other dependencies
    }

    public Task<VisitorBadge> IssueBadgeAsync(Guid personProfileId, Guid hostProfileId, DateTimeOffset expiresAt) => throw new NotImplementedException();
    public Task<VisitorBadge> RevokeBadgeAsync(Guid badgeId) => throw new NotImplementedException();
    public Task<VisitorBadge> ExpireBadgeAsync(Guid badgeId) => throw new NotImplementedException();
}