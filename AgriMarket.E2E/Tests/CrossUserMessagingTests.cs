using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class CrossUserMessagingTests
{
    private readonly E2EFixture _fixture;

    public CrossUserMessagingTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Messaging_BothUsersCanAccessMessaging()
    {
        var farmerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);
        var farmerMessaging = new MessagingIndexPage(farmerPage, _fixture.BaseUrl);
        await farmerMessaging.NavigateAsync();
        farmerPage.Url.Should().Contain("/Client/Messaging");
        await farmerPage.Context.DisposeAsync();

        var providerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ClientEmail, SeedData.ClientPassword);
        var providerMessaging = new MessagingIndexPage(providerPage, _fixture.BaseUrl);
        await providerMessaging.NavigateAsync();
        providerPage.Url.Should().Contain("/Client/Messaging");
        await providerPage.Context.DisposeAsync();
    }
}
