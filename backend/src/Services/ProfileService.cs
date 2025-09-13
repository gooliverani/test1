using AccessControl.Api.Models;

namespace AccessControl.Api.Services;

public class ProfileService
{
    public ProfileService(ApplicationDbContext db)
    {
        // Inject context or other dependencies
    }

    public Task<PersonProfile?> GetProfileAsync(Guid id) => throw new NotImplementedException();
    public Task<IEnumerable<PersonProfile>> ListProfilesAsync() => throw new NotImplementedException();
    public Task<PersonProfile> CreateProfileAsync(PersonProfile profile) => throw new NotImplementedException();
    public Task<PersonProfile> UpdateProfileStatusAsync(Guid id, ProfileStatus status) => throw new NotImplementedException();
    public Task<PersonProfile> UpsertFromWorkdayAsync(object workdayPayload) => throw new NotImplementedException();
}