using AccessControl.Api.Models;

using AccessControl.Api.Data;

namespace AccessControl.Api.Services;

public class TemplateService
{
    public TemplateService(ApplicationDbContext db)
    {
        // Inject context or other dependencies
    }

    public Task<AccessTemplate> CreateTemplateAsync(AccessTemplate template) => throw new NotImplementedException();
    public Task ApplyTemplateToProfileAsync(Guid profileId, Guid templateId) => throw new NotImplementedException();
}