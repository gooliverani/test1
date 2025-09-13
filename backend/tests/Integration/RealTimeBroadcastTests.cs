using FluentAssertions;

namespace AccessControl.Tests.Integration;

public class RealTimeBroadcastTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiFactory>
{
    private readonly DatabaseFixture _db;
    private readonly ApiFactory _api;
    public RealTimeBroadcastTests(DatabaseFixture db, ApiFactory api) { _db = db; _api = api; }

    [Fact(DisplayName = "T038 Real-time broadcast latency <2s (RED)")]
    public async Task Broadcast_LatencyUnderThreshold()
    {
        var client = _api.CreateClient();
        bool underThreshold = false; // placeholder until SignalR + publisher implemented
        underThreshold.Should().BeTrue("real-time broadcast not implemented yet");
    }
}