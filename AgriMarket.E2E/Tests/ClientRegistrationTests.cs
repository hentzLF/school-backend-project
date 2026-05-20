using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Client;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class ClientRegistrationTests
{
    private readonly E2EFixture _fixture;

    public ClientRegistrationTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Register_ValidData_RedirectsToLogin()
    {
        var page = await _fixture.CreatePageAsync();
        var registerPage = new ClientRegisterPage(page, _fixture.BaseUrl);
        await registerPage.NavigateAsync();

        var uniqueEmail = $"test-{Guid.NewGuid():N}@example.com";
        await registerPage.FillFormAsync("Test", "User", uniqueEmail, "TestPass123!");
        await registerPage.SubmitAsync();

        (await registerPage.IsOnLoginPageAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShowsError()
    {
        var page = await _fixture.CreatePageAsync();
        var registerPage = new ClientRegisterPage(page, _fixture.BaseUrl);
        await registerPage.NavigateAsync();

        await registerPage.FillFormAsync("Test", "User", SeedData.ProviderEmail, SeedData.ProviderPassword);
        await registerPage.SubmitAsync();

        (await registerPage.IsOnRegisterPageAsync()).Should().BeTrue();
        (await registerPage.HasErrorAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }

    [Fact]
    public async Task Register_EmptyFields_ShowsValidationErrors()
    {
        var page = await _fixture.CreatePageAsync();
        var registerPage = new ClientRegisterPage(page, _fixture.BaseUrl);
        await registerPage.NavigateAsync();

        await registerPage.FillFormAsync("", "", "", "");
        await registerPage.SubmitAsync();

        (await registerPage.HasErrorAsync()).Should().BeTrue();
        await page.Context.DisposeAsync();
    }
}
