using FluentAssertions;

namespace AccessControl.Tests.Integration;

public class AccessDecisionFlowTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiFactory>
{
    private readonly DatabaseFixture _db;
    private readonly ApiFactory _api;
    public AccessDecisionFlowTests(DatabaseFixture db, ApiFactory api) { _db = db; _api = api; }

    [Fact(DisplayName = "T033 Access decision flow ALLOW scenario (RED)")]
    public async Task AccessDecisionFlow_ShouldAllow()
    {
        var client = _api.CreateClient();
        // Outline: create profile -> issue credential -> create zone -> schedule -> grant permission -> simulate attempt.
        // RED: final expectation will assert success; currently assert opposite to keep failing.
        bool success = false; // placeholder result
        success.Should().BeTrue("flow not implemented yet");
    }
}