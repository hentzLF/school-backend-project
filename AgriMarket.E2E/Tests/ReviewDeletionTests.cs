using AgriMarket.E2E.Infrastructure;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class ReviewDeletionTests
{
    private readonly E2EFixture _fixture;

    public ReviewDeletionTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task ReviewDelete_InvalidReviewId_HandlesGracefully()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);

        var response = await page.GotoAsync($"{_fixture.BaseUrl}/Client/Reviews/Delete/99999");

        var isHandled = response!.Status >= 400
            || page.Url.Contains("Bookings")
            || page.Url.Contains("Error");
        isHandled.Should().BeTrue();

        await page.Context.DisposeAsync();
    }
}
