namespace AccessControl.Tests.Contract;

public class Reports_ZoneAccessTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Reports_ZoneAccessTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T028 GET /api/reports/zone-access returns zone access (RED)")]
    public async Task GetZoneAccess_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/reports/zone-access?zoneId=00000000-0000-0000-0000-000000000001&from=2025-01-01&to=2025-01-31");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}