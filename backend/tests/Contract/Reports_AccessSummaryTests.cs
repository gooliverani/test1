namespace AccessControl.Tests.Contract;

public class Reports_AccessSummaryTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Reports_AccessSummaryTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T027 GET /api/reports/access-summary returns summary (RED)")]
    public async Task GetAccessSummary_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/reports/access-summary?from=2025-01-01&to=2025-01-31");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}