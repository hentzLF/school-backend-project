using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ProfileEditPage : PageBase
{
    public ProfileEditPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Profile/Edit");

    public async Task SetFirstNameAsync(string firstName) =>
        await Page.FillAsync("input[name='FirstName']", firstName);

    public async Task SetLastNameAsync(string lastName) =>
        await Page.FillAsync("input[name='LastName']", lastName);

    public async Task SetBioAsync(string bio) =>
        await Page.FillAsync("textarea[name='Bio']", bio);

    public async Task SetAvatarUrlAsync(string url) =>
        await Page.FillAsync("input[name='AvatarUrl']", url);

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> HasErrorAsync() => await HasValidationErrorsAsync();
}
