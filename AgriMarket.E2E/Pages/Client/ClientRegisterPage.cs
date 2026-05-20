using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ClientRegisterPage : PageBase
{
    public ClientRegisterPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Account/Register");

    public async Task FillFormAsync(string firstName, string lastName, string email, string password)
    {
        await Page.FillAsync("input[name='FirstName']", firstName);
        await Page.FillAsync("input[name='LastName']", lastName);
        await Page.FillAsync("input[name='Email']", email);
        await Page.FillAsync("input[name='Password']", password);
    }

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task<bool> HasErrorAsync() => await HasValidationErrorsAsync();

    public async Task<bool> IsOnRegisterPageAsync()
    {
        var path = await GetCurrentPathAsync();
        return path.Contains("/Client/Account/Register", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> IsOnLoginPageAsync()
    {
        var path = await GetCurrentPathAsync();
        return path.Contains("/Client/Account/Login", StringComparison.OrdinalIgnoreCase);
    }
}
