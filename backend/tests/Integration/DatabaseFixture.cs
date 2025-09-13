using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Xunit;

namespace AccessControl.Tests.Integration;

public sealed class DatabaseFixture : IAsyncLifetime
{
    public string ConnectionString => _container != null
        ? $"Host=localhost;Port={_container.GetMappedPublicPort(5432)};Username=postgres;Password=postgres;Database=testdb"
        : string.Empty;

    private IContainer? _container;

    public async Task InitializeAsync()
    {
        _container = new ContainerBuilder()
            .WithImage("postgres:15-alpine")
            .WithPortBinding(5432, assignRandomHostPort: true)
            .WithEnvironment("POSTGRES_PASSWORD", "postgres")
            .WithEnvironment("POSTGRES_DB", "testdb")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();
        await _container.StartAsync();

        // Smoke connectivity
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();
    }

    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }
}