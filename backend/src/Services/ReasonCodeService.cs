using AccessControl.Api.Models;

using AccessControl.Api.Data;

namespace AccessControl.Api.Services;

public class ReasonCodeService
{
    public ReasonCodeService(ApplicationDbContext db)
    {
        // Inject context or other dependencies
    }

    public Task<IEnumerable<ReasonCode>> ListReasonCodesAsync() => throw new NotImplementedException();
}