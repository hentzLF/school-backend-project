using Microsoft.Playwright;

namespace AgriMarket.E2E.Infrastructure;

public static class AuthHelper
{
    public static async Task<IPage> LoginAsClientAsync(
        E2EFixture fixture, string email, string password)
    {
        var page = await fixture.CreatePageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/Client/Account/Login",
            new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        await page.FillAsync("input[name='Email']", email);
        await page.FillAsync("input[name='Password']", password);
        await page.RunAndWaitForNavigationAsync(async () =>
        {
            await page.ClickAsync("button[type='submit']");
        }, new PageRunAndWaitForNavigationOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });
        return page;
    }

    public static async Task<IPage> LoginAsAdminAsync(
        E2EFixture fixture, string email, string password)
    {
        var page = await fixture.CreatePageAsync();
        await page.GotoAsync($"{fixture.BaseUrl}/Admin/Account/Login",
            new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        await page.FillAsync("input[name='Email']", email);
        await page.FillAsync("input[name='Password']", password);
        await page.RunAndWaitForNavigationAsync(async () =>
        {
            await page.ClickAsync("button[type='submit']");
        }, new PageRunAndWaitForNavigationOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });
        return page;
    }
}
