using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Admin;

public sealed class AdminRegisterPage : PageBase
{
    public AdminRegisterPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Admin/Account/Register");

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
}
