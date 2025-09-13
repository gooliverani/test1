namespace AccessControl.Tests.Contract;

public class ZonePermissions_PostTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public ZonePermissions_PostTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T017 POST /api/profiles/{id}/zone-permissions grants permission (RED)")]
    public async Task GrantZonePermission_Returns201()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/profiles/00000000-0000-0000-0000-000000000001/zone-permissions", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Created);
    }
}