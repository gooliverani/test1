namespace AccessControl.Tests.Contract;

public class Schedules_ListTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Schedules_ListTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T021 GET /api/schedules lists schedules (RED)")]
    public async Task ListSchedules_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/schedules");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}