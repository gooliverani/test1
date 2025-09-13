using Microsoft.AspNetCore.Mvc.Testing;

namespace AccessControl.Tests.Integration;

public class ApiFactory : WebApplicationFactory<Program>
{
    // Later: override ConfigureWebHost to inject test services, seed data, etc.
}