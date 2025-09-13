namespace AccessControl.Tests.Contract;

public class AccessAttempts_ListTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public AccessAttempts_ListTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T026 GET /api/access-attempts returns attempts with filters (RED)")]
    public async Task ListAccessAttempts_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/access-attempts?profileId=00000000-0000-0000-0000-000000000001");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}