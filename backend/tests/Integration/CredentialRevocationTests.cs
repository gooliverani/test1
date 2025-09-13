using FluentAssertions;

namespace AccessControl.Tests.Integration;

public class CredentialRevocationTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiFactory>
{
    private readonly DatabaseFixture _db;
    private readonly ApiFactory _api;
    public CredentialRevocationTests(DatabaseFixture db, ApiFactory api) { _db = db; _api = api; }

    [Fact(DisplayName = "T034 Credential revocation prevents future access (RED)")]
    public async Task RevokedCredential_ShouldDeny()
    {
        var client = _api.CreateClient();
        bool denied = false; // will be determined after implementation
        denied.Should().BeTrue("revocation logic not implemented yet");
    }
}