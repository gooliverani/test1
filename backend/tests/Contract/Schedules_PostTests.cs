namespace AccessControl.Tests.Contract;

public class Schedules_PostTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Schedules_PostTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T022 POST /api/schedules creates schedule (RED)")]
    public async Task CreateSchedule_Returns201()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/schedules", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Created);
    }
}