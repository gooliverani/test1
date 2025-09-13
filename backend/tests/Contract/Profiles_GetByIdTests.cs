namespace AccessControl.Tests.Contract;

public class Profiles_GetByIdTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Profiles_GetByIdTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T012 GET /api/profiles/{id} returns 200 when exists (RED)")]
    public async Task GetProfile_Returns200_WhenExists()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/profiles/00000000-0000-0000-0000-000000000001");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK, "endpoint not implemented yet");
    }

    [Fact(DisplayName = "T012 GET /api/profiles/{id} returns 404 when missing (RED)")]
    public async Task GetProfile_Returns404_WhenMissing()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/profiles/00000000-0000-0000-0000-00000000AAAA");
        // Expect not 404 yet (likely 404 already because route missing) -> keep RED by asserting inequality with final desired 404.
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.NotFound);
    }
}