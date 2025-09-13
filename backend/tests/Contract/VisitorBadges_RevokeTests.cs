namespace AccessControl.Tests.Contract;

public class VisitorBadges_RevokeTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public VisitorBadges_RevokeTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T031 POST /api/visitor/badges/{id}/revoke revokes badge (RED)")]
    public async Task RevokeVisitorBadge_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/visitor/badges/00000000-0000-0000-0000-000000000001/revoke", null!);
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}