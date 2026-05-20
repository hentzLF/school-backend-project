using AgriMarket.E2E.Infrastructure;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class SmokeTests
{
    private readonly E2EFixture _fixture;

    public SmokeTests(E2EFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HomePage_ReturnsSuccessStatusCode()
    {
        var page = await _fixture.CreatePageAsync();
        var response = await page.GotoAsync(_fixture.BaseUrl);

        response!.Status.Should().Be(200);
        await page.Context.DisposeAsync();
    }
}
