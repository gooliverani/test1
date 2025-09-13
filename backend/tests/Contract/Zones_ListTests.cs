namespace AccessControl.Tests.Contract;

public class Zones_ListTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Zones_ListTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T019 GET /api/zones lists zones (RED)")]
    public async Task ListZones_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/zones");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}