using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class MessagingJourneyTest
{
    private readonly E2EFixture _fixture;

    public MessagingJourneyTest(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task MessagingJourney_BothUsersCanAccessMessaging()
    {
        var farmerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.FarmerEmail, SeedData.FarmerPassword);
        await farmerPage.GotoAsync($"{_fixture.BaseUrl}/Client/Messaging");
        farmerPage.Url.Should().Contain("/Client/Messaging");
        await farmerPage.Context.DisposeAsync();

        var providerPage = await _fixture.CreateAuthenticatedClientPageAsync(
            SeedData.ProviderEmail, SeedData.ProviderPassword);
        await providerPage.GotoAsync($"{_fixture.BaseUrl}/Client/Messaging");
        providerPage.Url.Should().Contain("/Client/Messaging");
        await providerPage.Context.DisposeAsync();
    }
}
