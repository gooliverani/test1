using FluentAssertions;

namespace AccessControl.Tests.Integration;

public class Reports_ZoneAccessExportTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiFactory>
{
    private readonly DatabaseFixture _db;
    private readonly ApiFactory _api;
    public Reports_ZoneAccessExportTests(DatabaseFixture db, ApiFactory api) { _db = db; _api = api; }

    [Fact(DisplayName = "T037 Zone access report export CSV structure (RED)")]
    public async Task ZoneAccessReport_ShouldExportCsv()
    {
        var client = _api.CreateClient();
        bool exported = false; // placeholder
        exported.Should().BeTrue("report export not implemented yet");
    }
}