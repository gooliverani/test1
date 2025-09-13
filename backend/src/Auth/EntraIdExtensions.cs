using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace AccessControl.Api.Auth;

public static class EntraIdExtensions
{
    public static IServiceCollection AddEntraIdAuth(this IServiceCollection services, IConfiguration config)
    {
        // Placeholder - to be configured with actual Azure AD (Entra ID) values later (T073)
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(config.GetSection("AzureAd"));
        return services;
    }
}