using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Admin;

public sealed class AdminLoginPage : PageBase
{
    public AdminLoginPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Admin/Account/Login");

    public async Task LoginAsync(string email, string password)
    {
        await Page.FillAsync("input[name='Email']", email);
        await Page.FillAsync("input[name='Password']", password);
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> HasErrorAsync()
    {
        var errorDiv = await Page.QuerySelectorAsync(".text-danger:not(:empty)");
        return errorDiv is not null;
    }

    public async Task<bool> IsOnLoginPageAsync()
    {
        var path = await GetCurrentPathAsync();
        return path.Contains("/Admin/Account/Login", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<bool> IsOnDashboardAsync()
    {
        var path = await GetCurrentPathAsync();
        return path.Contains("/Admin/Dashboard", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/Admin", StringComparison.OrdinalIgnoreCase)
            || path.Equals("/Admin/", StringComparison.OrdinalIgnoreCase);
    }
}
