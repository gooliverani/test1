using AccessControl.Api.Models;
using AccessControl.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace AccessControl.Api.Services;

/// <summary>
/// Service for managing person profiles in the access control system
/// </summary>
public class ProfileService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ProfileService
    /// </summary>
    /// <param name="context">The database context</param>
    public ProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets an employee profile by ID
    /// </summary>
    /// <param name="id">The profile ID</param>
    /// <returns>The employee profile if found, null otherwise</returns>
    public async Task<EmployeeProfile?> GetEmployeeProfileAsync(int id)
    {
        return await _context.EmployeeProfiles
            .Include(p => p.Department)
            .Include(p => p.Team)
            .Include(p => p.Location)
            .Include(p => p.EmployeeAccess)
                .ThenInclude(ea => ea.AccessProfile)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Lists all employee profiles
    /// </summary>
    /// <returns>A collection of employee profiles</returns>
    public async Task<IEnumerable<EmployeeProfile>> ListEmployeeProfilesAsync()
    {
        return await _context.EmployeeProfiles
            .Include(p => p.Department)
            .Include(p => p.Team)
            .Include(p => p.Location)
            .Include(p => p.EmployeeAccess)
                .ThenInclude(ea => ea.AccessProfile)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new employee profile
    /// </summary>
    /// <param name="profile">The profile to create</param>
    /// <returns>The created profile</returns>
    public async Task<EmployeeProfile> CreateEmployeeProfileAsync(EmployeeProfile profile)
    {
        profile.CreatedAt = DateTimeOffset.Now;
        profile.UpdatedAt = DateTimeOffset.Now;
        
        _context.EmployeeProfiles.Add(profile);
        await _context.SaveChangesAsync();
        
        return profile;
    }

    /// <summary>
    /// Updates an employee profile's active status
    /// </summary>
    /// <param name="id">The profile ID</param>
    /// <param name="isActive">The new active status</param>
    /// <returns>The updated profile</returns>
    public async Task<EmployeeProfile> UpdateEmployeeStatusAsync(int id, bool isActive)
    {
        var profile = await _context.EmployeeProfiles.FindAsync(id);
        if (profile == null) 
            throw new ArgumentException("Employee profile not found", nameof(id));
        
        profile.IsActive = isActive;
        profile.UpdatedAt = DateTimeOffset.Now;
        
        await _context.SaveChangesAsync();
        return profile;
    }

    // Legacy methods for compatibility with existing PersonProfile-based code
    /// <summary>
    /// Gets a person profile by ID (legacy)
    /// </summary>
    /// <param name="id">The profile ID</param>
    /// <returns>The person profile if found, null otherwise</returns>
    public async Task<PersonProfile?> GetProfileAsync(Guid id)
    {
        return await _context.PersonProfiles
            .Include(p => p.Credentials)
            .Include(p => p.ZonePermissions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Lists all person profiles (legacy)
    /// </summary>
    /// <returns>A collection of person profiles</returns>
    public async Task<IEnumerable<PersonProfile>> ListProfilesAsync()
    {
        return await _context.PersonProfiles
            .Include(p => p.Credentials)
            .Include(p => p.ZonePermissions)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new person profile (legacy)
    /// </summary>
    /// <param name="profile">The profile to create</param>
    /// <returns>The created profile</returns>
    public async Task<PersonProfile> CreateProfileAsync(PersonProfile profile)
    {
        profile.Id = Guid.NewGuid();
        profile.CreatedAt = DateTimeOffset.Now;
        profile.ModifiedAt = DateTimeOffset.Now;
        
        _context.PersonProfiles.Add(profile);
        await _context.SaveChangesAsync();
        
        return profile;
    }

    /// <summary>
    /// Updates the status of a person profile (legacy)
    /// </summary>
    /// <param name="id">The profile ID</param>
    /// <param name="status">The new status</param>
    /// <returns>The updated profile</returns>
    public async Task<PersonProfile> UpdateProfileStatusAsync(Guid id, ProfileStatus status)
    {
        var profile = await _context.PersonProfiles.FindAsync(id);
        if (profile == null) 
            throw new ArgumentException("Profile not found", nameof(id));
        
        profile.Status = status;
        profile.ModifiedAt = DateTimeOffset.Now;
        
        await _context.SaveChangesAsync();
        return profile;
    }

    /// <summary>
    /// Upserts a profile from Workday payload
    /// </summary>
    /// <param name="workdayPayload">The Workday data payload</param>
    /// <returns>The upserted profile</returns>
    public async Task<PersonProfile> UpsertFromWorkdayAsync(object workdayPayload)
    {
        // Mock implementation for Workday integration - in real implementation,
        // parse workdayPayload and check for existing employee by external ID
        var profile = new PersonProfile
        {
            Id = Guid.NewGuid(),
            ExternalId = "WD" + DateTime.Now.Ticks.ToString()[^6..],
            Type = ProfileType.Employee,
            Status = ProfileStatus.Active,
            DisplayName = "Workday User",
            Email = "workday.user@company.com",
            Department = "Workday Import",
            CreatedAt = DateTimeOffset.Now,
            ModifiedAt = DateTimeOffset.Now
        };
        
        _context.PersonProfiles.Add(profile);
        await _context.SaveChangesAsync();
        
        return profile;
    }
}