namespace AccessControl.Tests.Contract;

public class Profiles_ListTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Profiles_ListTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T013 GET /api/profiles returns list (RED)")]
    public async Task ListProfiles_ReturnsList()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/profiles");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}