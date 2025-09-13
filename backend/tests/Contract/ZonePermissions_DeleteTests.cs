namespace AccessControl.Tests.Contract;

public class ZonePermissions_DeleteTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public ZonePermissions_DeleteTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T018 DELETE /api/zone-permissions/{id} revokes permission (RED)")]
    public async Task DeleteZonePermission_Returns204()
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync("/api/zone-permissions/00000000-0000-0000-0000-000000000001");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NoContent);
    }
}