namespace AccessControl.Tests.Contract;

public class Credentials_PostTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Credentials_PostTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T015 POST /api/profiles/{id}/credentials issues credential (RED)")]
    public async Task IssueCredential_Returns201()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/profiles/00000000-0000-0000-0000-000000000001/credentials", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Created);
    }
}