using AccessControl.Api.Models;

namespace AccessControl.Api.Services;

public class ZonePermissionService
{
    public ZonePermissionService(ApplicationDbContext db)
    {
        // Inject context or other dependencies
    }

    public Task<ZonePermission> GrantPermissionAsync(Guid personProfileId, Guid zoneId, Guid scheduleId) => throw new NotImplementedException();
    public Task<ZonePermission> RevokePermissionAsync(Guid zonePermissionId) => throw new NotImplementedException();
}