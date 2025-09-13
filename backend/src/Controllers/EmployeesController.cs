using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Services;
using AccessControl.Api.Models;
using AccessControl.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace AccessControl.Api.Controllers;

/// <summary>
/// Controller for managing employee profiles
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ProfileService _profileService;
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of EmployeesController
    /// </summary>
    /// <param name="profileService">Profile service</param>
    /// <param name="context">Database context</param>
    public EmployeesController(ProfileService profileService, ApplicationDbContext context)
    {
        _profileService = profileService;
        _context = context;
    }

    /// <summary>
    /// Get all employee profiles
    /// </summary>
    /// <returns>List of employee profiles</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetEmployees()
    {
        var employees = await _profileService.ListEmployeeProfilesAsync();
        
        // Convert to a simplified response format
        var response = employees.Select(e => new
        {
            e.Id,
            e.CompId,
            e.FirstName,
            e.LastName,
            e.Email,
            e.HireDate,
            e.ExpireDate,
            e.IsActive,
            e.BadgeSerial,
            Department = e.Department?.Name,
            Team = e.Team?.Name,
            Location = e.Location?.Name,
            AccessProfiles = e.EmployeeAccess?.Where(ea => ea.Active).Select(ea => ea.AccessProfile?.Name),
            PhotoUrl = e.Photos?.FirstOrDefault(p => p.IsActive)?.PhotoUrl
        });
        
        return Ok(response);
    }

    /// <summary>
    /// Get a specific employee profile by ID
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Employee profile</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetEmployee(int id)
    {
        var employee = await _profileService.GetEmployeeProfileAsync(id);
        
        if (employee == null)
            return NotFound();
            
        var response = new
        {
            employee.Id,
            employee.CompId,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.HireDate,
            employee.ExpireDate,
            employee.IsActive,
            employee.BadgeSerial,
            Department = employee.Department?.Name,
            Team = employee.Team?.Name,
            Location = employee.Location?.Name,
            AccessProfiles = employee.EmployeeAccess?.Where(ea => ea.Active).Select(ea => ea.AccessProfile?.Name),
            PhotoUrl = employee.Photos?.FirstOrDefault(p => p.IsActive)?.PhotoUrl
        };
        
        return Ok(response);
    }

    /// <summary>
    /// Create a new employee profile
    /// </summary>
    /// <param name="request">Employee creation request</param>
    /// <returns>Created employee profile</returns>
    [HttpPost]
    public async Task<ActionResult<object>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        // Generate unique comp_id
        var initials = $"{request.FirstName.Substring(0, 1)}{request.LastName.Substring(0, 1)}".ToUpper();
        var random = new Random();
        string compId;
        do
        {
            var number = random.Next(100000, 999999);
            compId = $"{initials}{number}";
        } while (await _context.EmployeeProfiles.AnyAsync(e => e.CompId == compId));

        var employee = new EmployeeProfile
        {
            CompId = compId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            DepartmentId = request.DepartmentId,
            TeamId = request.TeamId,
            LocationId = request.LocationId,
            HireDate = request.HireDate,
            ExpireDate = request.ExpireDate,
            BadgeSerial = request.BadgeSerial,
            IsActive = request.IsActive,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.EmployeeProfiles.Add(employee);
        await _context.SaveChangesAsync();

        // Reload with related data
        var createdEmployee = await _context.EmployeeProfiles
            .Include(e => e.Department)
            .Include(e => e.Team)
            .Include(e => e.Location)
            .Include(e => e.EmployeeAccess)
                .ThenInclude(ea => ea.AccessProfile)
            .FirstOrDefaultAsync(e => e.Id == employee.Id);

        var response = new
        {
            createdEmployee!.Id,
            createdEmployee.CompId,
            createdEmployee.FirstName,
            createdEmployee.LastName,
            createdEmployee.Email,
            createdEmployee.HireDate,
            createdEmployee.ExpireDate,
            createdEmployee.IsActive,
            createdEmployee.BadgeSerial,
            Department = createdEmployee.Department?.Name,
            Team = createdEmployee.Team?.Name,
            Location = createdEmployee.Location?.Name,
            AccessProfiles = createdEmployee.EmployeeAccess?.Where(ea => ea.Active).Select(ea => ea.AccessProfile?.Name),
            PhotoUrl = (string?)null
        };

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, response);
    }

    /// <summary>
    /// Get employee profile photo
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Employee photo URL or data</returns>
    [HttpGet("{id}/photo")]
    public async Task<ActionResult<object>> GetEmployeePhoto(int id)
    {
        var photo = await _context.EmployeePhotos
            .Where(p => p.EmployeeId == id && p.IsActive)
            .FirstOrDefaultAsync();
            
        if (photo == null)
            return NotFound("No active photo found for employee");
            
        var response = new
        {
            photo.Id,
            photo.PhotoUrl,
            photo.PhotoFilename,
            photo.MimeType,
            photo.UploadedAt,
            photo.UploadedBy
        };
        
        return Ok(response);
    }

    /// <summary>
    /// Upload or update employee profile photo
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <param name="photoUrl">Photo URL</param>
    /// <param name="filename">Photo filename</param>
    /// <param name="mimeType">Photo MIME type</param>
    /// <param name="uploadedBy">Who uploaded the photo</param>
    /// <returns>Updated photo information</returns>
    [HttpPost("{id}/photo")]
    public async Task<ActionResult<object>> UploadEmployeePhoto(
        int id, 
        [FromBody] UploadPhotoRequest request)
    {
        var employee = await _context.EmployeeProfiles.FindAsync(id);
        if (employee == null)
            return NotFound("Employee not found");

        // Deactivate existing photos
        var existingPhotos = await _context.EmployeePhotos
            .Where(p => p.EmployeeId == id && p.IsActive)
            .ToListAsync();
            
        foreach (var existing in existingPhotos)
        {
            existing.IsActive = false;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
        }

        // Create new photo record
        var newPhoto = new EmployeePhoto
        {
            EmployeeId = id,
            PhotoUrl = request.PhotoUrl,
            PhotoFilename = request.Filename,
            MimeType = request.MimeType,
            UploadedBy = request.UploadedBy ?? "system",
            IsActive = true,
            UploadedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.EmployeePhotos.Add(newPhoto);
        await _context.SaveChangesAsync();

        var response = new
        {
            newPhoto.Id,
            newPhoto.PhotoUrl,
            newPhoto.PhotoFilename,
            newPhoto.MimeType,
            newPhoto.UploadedAt,
            newPhoto.UploadedBy
        };

        return Ok(response);
    }

    /// <summary>
    /// Get badge history for an employee
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Badge history records</returns>
    [HttpGet("{id}/badge-history")]
    public async Task<ActionResult<IEnumerable<object>>> GetEmployeeBadgeHistory(int id)
    {
        var badgeHistory = await _context.BadgeHistory
            .Where(bh => bh.EmployeeId == id)
            .OrderByDescending(bh => bh.IssuedDate)
            .Select(bh => new
            {
                bh.Id,
                bh.BadgeSerial,
                bh.ActionType,
                bh.PreviousBadgeSerial,
                bh.IssuedDate,
                bh.ExpiryDate,
                bh.IssuedBy,
                bh.Reason,
                bh.IsActive
            })
            .ToListAsync();

        return Ok(badgeHistory);
    }

    /// <summary>
    /// Get access history for an employee
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <returns>Access history records</returns>
    [HttpGet("{id}/access-history")]
    public async Task<ActionResult<IEnumerable<object>>> GetEmployeeAccessHistory(int id)
    {
        var accessHistory = await _context.AccessHistory
            .Include(ah => ah.AccessProfile)
            .Include(ah => ah.PreviousAccessProfile)
            .Where(ah => ah.EmployeeId == id)
            .OrderByDescending(ah => ah.EffectiveDate)
            .Select(ah => new
            {
                ah.Id,
                ah.ActionType,
                ah.EffectiveDate,
                ah.ExpiryDate,
                ah.GrantedBy,
                ah.Reason,
                AccessProfile = ah.AccessProfile != null ? new { ah.AccessProfile.Id, ah.AccessProfile.Name } : null,
                PreviousAccessProfile = ah.PreviousAccessProfile != null ? new { ah.PreviousAccessProfile.Id, ah.PreviousAccessProfile.Name } : null
            })
            .ToListAsync();

        return Ok(accessHistory);
    }

    /// <summary>
    /// Get swipe records for an employee
    /// </summary>
    /// <param name="id">Employee ID</param>
    /// <param name="days">Number of days to look back (default 30)</param>
    /// <returns>Swipe records</returns>
    [HttpGet("{id}/swipes")]
    public async Task<ActionResult<IEnumerable<object>>> GetEmployeeSwipes(int id, int days = 30)
    {
        var startDate = DateTimeOffset.UtcNow.AddDays(-days);
        
        var swipes = await _context.Swipes
            .Where(s => s.EmployeeId == id && s.SwipeTime >= startDate)
            .OrderByDescending(s => s.SwipeTime)
            .Select(s => new
            {
                s.Id,
                s.SwipeTime,
                s.CardNumber,
                s.AccessGranted,
                s.DenialReason,
                s.ReaderId
            })
            .ToListAsync();

        return Ok(swipes);
    }

    /// <summary>
    /// Get all readers
    /// </summary>
    /// <returns>List of readers</returns>
    [HttpGet("readers")]
    public async Task<ActionResult<IEnumerable<object>>> GetReaders()
    {
        var readers = await _context.Readers
            .Include(r => r.Location)
            .Include(r => r.AccessProfileReaders)
                .ThenInclude(apr => apr.AccessProfile)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.DeviceType,
                r.Capabilities,
                Location = r.Location != null ? r.Location.Name : null,
                AccessProfileCount = r.AccessProfileReaders.Count,
                AccessProfiles = r.AccessProfileReaders.Select(apr => new
                {
                    apr.AccessProfile!.Id,
                    apr.AccessProfile.Name,
                    apr.AccessProfile.Description
                })
            })
            .ToListAsync();

        return Ok(readers);
    }

    /// <summary>
    /// Get all access profiles with reader assignments
    /// </summary>
    /// <returns>List of access profiles</returns>
    [HttpGet("access-profiles")]
    public async Task<ActionResult<IEnumerable<object>>> GetAccessProfiles()
    {
        var accessProfiles = await _context.AccessProfiles
            .Include(ap => ap.EmployeeAccess)
            .Select(ap => new
            {
                ap.Id,
                ap.Name,
                ap.Description,
                ap.IsActive,
                ap.CreatedAt,
                EmployeeCount = ap.EmployeeAccess.Count(ea => ea.Active),
                ReaderCount = _context.AccessProfileReaders.Count(apr => apr.AccessProfileId == ap.Id)
            })
            .ToListAsync();

        return Ok(accessProfiles);
    }

    /// <summary>
    /// Get access profile readers relationships
    /// </summary>
    /// <returns>List of access profile reader assignments</returns>
    [HttpGet("access-profile-readers")]
    public async Task<ActionResult<IEnumerable<object>>> GetAccessProfileReaders()
    {
        var assignments = await _context.AccessProfileReaders
            .Include(apr => apr.AccessProfile)
            .Include(apr => apr.Reader)
                .ThenInclude(r => r!.Location)
            .Select(apr => new
            {
                AccessProfileId = apr.AccessProfileId,
                AccessProfileName = apr.AccessProfile!.Name,
                ReaderId = apr.ReaderId,
                ReaderName = apr.Reader!.Name,
                ReaderLocation = apr.Reader.Location != null ? apr.Reader.Location.Name : null,
                ReaderDeviceType = apr.Reader.DeviceType
            })
            .ToListAsync();

        return Ok(assignments);
    }

    /// <summary>
    /// Get detailed card field summary combining readers, profiles, and assignments
    /// </summary>
    /// <returns>Card field summary data</returns>
    [HttpGet("card-fields-summary")]
    public async Task<ActionResult<object>> GetCardFieldsSummary()
    {
        var summary = new
        {
            TotalReaders = await _context.Readers.CountAsync(),
            TotalAccessProfiles = await _context.AccessProfiles.CountAsync(),
            TotalAssignments = await _context.AccessProfileReaders.CountAsync(),
            ActiveProfiles = await _context.AccessProfiles.CountAsync(ap => ap.IsActive),
            ReadersWithoutProfiles = await _context.Readers
                .Where(r => !_context.AccessProfileReaders.Any(apr => apr.ReaderId == r.Id))
                .CountAsync(),
            ProfilesWithoutReaders = await _context.AccessProfiles
                .Where(ap => !_context.AccessProfileReaders.Any(apr => apr.AccessProfileId == ap.Id))
                .CountAsync()
        };

        return Ok(summary);
    }
}

/// <summary>
/// Request model for uploading employee photos
/// </summary>
public class UploadPhotoRequest
{
    public string PhotoUrl { get; set; } = string.Empty;
    public string? Filename { get; set; }
    public string? MimeType { get; set; }
    public string? UploadedBy { get; set; }
}