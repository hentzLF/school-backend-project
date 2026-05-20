using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class ClientLoginTests
{
    private readonly E2EFixture _fixture;

    public ClientLoginTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Login_ValidCredentials_RedirectsToListings()
    {
        var page = await _fixture.CreatePageAsync();
        var loginPage = new ClientLoginPage(page, _fixture.BaseUrl);
        await loginPage.NavigateAsync();

        await loginPage.LoginAsync(SeedData.FarmerEmail, SeedData.FarmerPassword);

        var path = page.Url;
        path.Should().Contain("/Client");
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task Login_WrongPassword_ShowsError()
    {
        var page = await _fixture.CreatePageAsync();
        var loginPage = new ClientLoginPage(page, _fixture.BaseUrl);
        await loginPage.NavigateAsync();

        await loginPage.LoginAsync(SeedData.FarmerEmail, "WrongPassword123!");

        (await loginPage.IsOnLoginPageAsync()).Should().BeTrue();
        (await loginPage.HasErrorAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task Login_NonExistentEmail_ShowsError()
    {
        var page = await _fixture.CreatePageAsync();
        var loginPage = new ClientLoginPage(page, _fixture.BaseUrl);
        await loginPage.NavigateAsync();

        await loginPage.LoginAsync("nobody@test.ee", "SomePass123!");

        (await loginPage.IsOnLoginPageAsync()).Should().BeTrue();
        (await loginPage.HasErrorAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }
}
