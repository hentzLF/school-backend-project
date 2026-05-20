using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ReviewEditPage : PageBase
{
    public ReviewEditPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int reviewId) =>
        await NavigateToAsync($"/Client/Reviews/Edit/{reviewId}");

    public async Task SelectRatingAsync(int rating)
    {
        await Page.CheckAsync($"#rating_{rating}");
    }

    public async Task FillCommentAsync(string comment)
    {
        await Page.FillAsync("textarea[name='Comment']", comment);
    }

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
