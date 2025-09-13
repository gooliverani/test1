using FluentAssertions;

namespace AccessControl.Tests.Integration;

public class WorkdayWebhookFlowTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiFactory>
{
    private readonly DatabaseFixture _db;
    private readonly ApiFactory _api;
    public WorkdayWebhookFlowTests(DatabaseFixture db, ApiFactory api) { _db = db; _api = api; }

    [Fact(DisplayName = "T035 Workday webhook upsert + idempotency (RED)")]
    public async Task WorkdayWebhook_Idempotent()
    {
        var client = _api.CreateClient();
        bool idempotent = false; // placeholder
        idempotent.Should().BeTrue("webhook logic not implemented yet");
    }
}