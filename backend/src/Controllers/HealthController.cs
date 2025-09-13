using Microsoft.AspNetCore.Mvc;
using AccessControl.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace AccessControl.Api.Controllers;

/// <summary>
/// Health check controller for testing database connectivity
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of HealthController
    /// </summary>
    /// <param name="context">Database context</param>
    public HealthController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    public async Task<ActionResult<object>> GetHealth()
    {
        try
        {
            // Test database connectivity
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return StatusCode(503, new { status = "unhealthy", message = "Cannot connect to database" });
            }

            // Get some basic counts
            var employeeCount = await _context.EmployeeProfiles.CountAsync();
            var departmentCount = await _context.Departments.CountAsync();
            var locationCount = await _context.Locations.CountAsync();

            return Ok(new
            {
                status = "healthy",
                database = "connected",
                counts = new
                {
                    employees = employeeCount,
                    departments = departmentCount,
                    locations = locationCount
                },
                timestamp = DateTimeOffset.Now
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                status = "unhealthy", 
                message = ex.Message,
                timestamp = DateTimeOffset.Now
            });
        }
    }
}