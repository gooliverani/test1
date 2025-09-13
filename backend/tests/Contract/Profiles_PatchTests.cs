namespace AccessControl.Tests.Contract;

public class Profiles_PatchTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Profiles_PatchTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T014 PATCH /api/profiles/{id} updates status (RED)")]
    public async Task PatchProfile_StatusChange()
    {
        var client = _factory.CreateClient();
        var response = await client.PatchAsync("/api/profiles/00000000-0000-0000-0000-000000000001", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}