using AgriMarket.E2E.Infrastructure;
using AgriMarket.E2E.Pages.Admin;
using FluentAssertions;

namespace AgriMarket.E2E.Tests;

[Collection("E2E")]
public sealed class AdminRegistrationTests
{
    private readonly E2EFixture _fixture;

    public AdminRegistrationTests(E2EFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AdminRegister_AuthenticatedAdmin_CreatesNewAdmin()
    {
        var page = await _fixture.CreateAuthenticatedAdminPageAsync(
            SeedData.AdminEmail, SeedData.AdminPassword);

        var registerPage = new AdminRegisterPage(page, _fixture.BaseUrl);
        await registerPage.NavigateAsync();

        var uniqueEmail = $"admin-{Guid.NewGuid():N}@agrimarket.ee";
        await registerPage.FillFormAsync("New", "Admin", uniqueEmail, "NewAdmin123!");
        await registerPage.SubmitAsync();

        (await registerPage.HasErrorAsync()).Should().BeFalse();
        await page.Context.DisposeAsync();
    }
}
