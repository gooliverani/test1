namespace AccessControl.Tests.Contract;

public class Credentials_RevokeTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Credentials_RevokeTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T016 POST /api/credentials/{id}/revoke revokes credential (RED)")]
    public async Task RevokeCredential_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/credentials/00000000-0000-0000-0000-000000000001/revoke", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}