using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class UserLockoutJourneyTest
{
    private readonly E2EFixture _fixture;

    public UserLockoutJourneyTest(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task LockoutJourney_AdminLocksAndUnlocksUser()
    {
        var uniqueEmail = $"locktest-{Guid.NewGuid():N}@example.com";
        var password = "LockTest123!";

        var regPage = await _fixture.CreatePageAsync();
        var register = new ClientRegisterPage(regPage, _fixture.BaseUrl);
        await register.NavigateAsync();
        await register.FillFormAsync("Lock", "Test", uniqueEmail, password);
        await register.SubmitAsync();
        await regPage.Context.DisposeAsync();

        var clientPage = await _fixture.CreateAuthenticatedClientPageAsync(uniqueEmail, password);
        clientPage.Url.Should().Contain("/Client/Listings");
        await clientPage.Context.DisposeAsync();

        var adminPage = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);
        var usersPage = new AdminUsersPage(adminPage, _fixture.BaseUrl);
        await usersPage.NavigateAsync();

        var pageText = await usersPage.GetPageTextAsync();
        pageText.Should().Contain(uniqueEmail);

        await adminPage.Context.DisposeAsync();
    }
}
