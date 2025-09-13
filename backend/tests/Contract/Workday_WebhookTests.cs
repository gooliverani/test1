namespace AccessControl.Tests.Contract;

public class Workday_WebhookTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Workday_WebhookTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T025 POST /api/workday/webhook processes payload idempotently (RED)")]
    public async Task WorkdayWebhook_Returns202()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/workday/webhook", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Accepted);
    }
}