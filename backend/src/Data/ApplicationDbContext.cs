using Microsoft.EntityFrameworkCore;

namespace AccessControl.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DbSets will be added in model tasks (T042+)
}