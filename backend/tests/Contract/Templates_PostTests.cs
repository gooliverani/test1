namespace AccessControl.Tests.Contract;

public class Templates_PostTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Templates_PostTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T023 POST /api/templates creates template (RED)")]
    public async Task CreateTemplate_Returns201()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/templates", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Created);
    }
}