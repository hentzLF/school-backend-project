using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class UnreadCountTests
{
    private readonly E2EFixture _fixture;

    public UnreadCountTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task MessagingIndex_PageAccessible()
    {
        var page = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);
        var messagingPage = new MessagingIndexPage(page, _fixture.BaseUrl);
        await messagingPage.NavigateAsync();

        page.Url.Should().Contain("/Client/Messaging");
        await page.Context.DisposeAsync();
    }
}
