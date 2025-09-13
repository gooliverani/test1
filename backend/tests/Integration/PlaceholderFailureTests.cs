using Xunit;
using FluentAssertions;

namespace AccessControl.Tests.Integration;

public class PlaceholderFailureTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    public PlaceholderFailureTests(DatabaseFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = "Placeholder test should fail until implementation logic added")]
    public void Should_Fail_Pending_Implementation()
    {
        // Intentionally failing assertion to enforce RED state before implementation tasks
        (1 + 1).Should().Be(3, "we want to ensure tests fail before implementation starts");
    }
}
