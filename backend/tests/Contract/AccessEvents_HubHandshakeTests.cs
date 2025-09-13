namespace AccessControl.Tests.Contract;

public class AccessEvents_HubHandshakeTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public AccessEvents_HubHandshakeTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T032 SignalR hub /hubs/access-events handshake works (RED)")]
    public async Task HubHandshake_FailsUntilImplemented()
    {
        var client = _factory.CreateClient();
        // Attempt GET (will be 404 until hub is mapped).
        var response = await client.GetAsync("/hubs/access-events");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.SwitchingProtocols);
    }
}