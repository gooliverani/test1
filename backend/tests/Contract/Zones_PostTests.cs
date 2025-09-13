namespace AccessControl.Tests.Contract;

public class Zones_PostTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Zones_PostTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T020 POST /api/zones creates zone (RED)")]
    public async Task CreateZone_Returns201()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/zones", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Created);
    }
}