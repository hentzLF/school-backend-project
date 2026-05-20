using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ClientLoginPage : PageBase
{
    public ClientLoginPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Account/Login");

    public async Task FillEmailAsync(string email) =>
        await Page.FillAsync("input[name='Email']", email);

    public async Task FillPasswordAsync(string password) =>
        await Page.FillAsync("input[name='Password']", password);

    public async Task SubmitAsync() =>
        await Page.ClickAsync("button[type='submit']");

    public async Task LoginAsync(string email, string password)
    {
        await FillEmailAsync(email);
        await FillPasswordAsync(password);
        await SubmitAsync();
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task<bool> HasErrorAsync()
    {
        var errorDiv = await Page.QuerySelectorAsync(".text-danger:not(:empty)");
        return errorDiv is not null;
    }

    public async Task<string> GetErrorTextAsync()
    {
        var errorDiv = await Page.QuerySelectorAsync(".text-danger.mb-3");
        return errorDiv is not null ? await errorDiv.InnerTextAsync() : string.Empty;
    }

    public async Task<bool> IsOnLoginPageAsync()
    {
        var path = await GetCurrentPathAsync();
        return path.Contains("/Client/Account/Login", StringComparison.OrdinalIgnoreCase);
    }
}
