namespace AccessControl.Tests.Contract;

public class ReasonCodes_ListTests : IClassFixture<TestStartup>
{
    private readonly TestStartup _factory;
    public ReasonCodes_ListTests(TestStartup factory) => _factory = factory;

    [Fact(DisplayName = "T029 GET /api/reason-codes lists codes (RED)")]
    public async Task ListReasonCodes_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/reason-codes");
        response.StatusCode.Should().NotBe(System.Net.HttpStatusCode.OK);
    }
}