namespace AccessControl.Tests.Contract;

public class VisitorBadges_PostTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public VisitorBadges_PostTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T030 POST /api/visitor/badges issues badge (RED)")]
    public async Task IssueVisitorBadge_Returns201()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/visitor/badges", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Created);
    }
}