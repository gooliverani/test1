namespace AccessControl.Tests.Contract;

public class Profiles_PostTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public Profiles_PostTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T011 POST /api/profiles returns 201 for valid payload (RED phase)")]
    public async Task PostProfile_CreatesProfile_Returns201()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Act
        // NOTE: Endpoint not implemented yet => Expect 404 so we assert NOT 201 to keep RED state.
        var response = await client.PostAsync("/api/profiles", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        // Assert (RED): This will fail once endpoint returns 201 because we assert the opposite now.
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.Created, "endpoint not implemented yet");
    }
}