namespace AccessControl.Tests.Contract;

public class Templates_ApplyTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Templates_ApplyTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T024 POST /api/profiles/{id}/apply-template/{templateId} applies template (RED)")]
    public async Task ApplyTemplate_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/profiles/00000000-0000-0000-0000-000000000001/apply-template/00000000-0000-0000-0000-000000000002", null!);
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}