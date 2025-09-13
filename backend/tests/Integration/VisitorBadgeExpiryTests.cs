using FluentAssertions;

namespace AccessControl.Tests.Integration;

public class VisitorBadgeExpiryTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiFactory>
{
    private readonly DatabaseFixture _db;
    private readonly ApiFactory _api;
    public VisitorBadgeExpiryTests(DatabaseFixture db, ApiFactory api) { _db = db; _api = api; }

    [Fact(DisplayName = "T036 Visitor badge auto-expire scenario (RED)")]
    public async Task VisitorBadge_ShouldExpire()
    {
        var client = _api.CreateClient();
        bool expired = false; // placeholder
        expired.Should().BeTrue("expiry logic not implemented yet");
    }
}