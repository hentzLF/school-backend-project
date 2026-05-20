using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class AvailabilitiesPage : PageBase
{
    public AvailabilitiesPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int listingId) =>
        await NavigateToAsync($"/Client/MyListings/Availabilities/{listingId}");

    public async Task AddAvailabilityAsync(DateTime startTime, DateTime endTime)
    {
        await Page.FillAsync("input[name='AddStartTime']", startTime.ToString("yyyy-MM-ddTHH:mm"));
        await Page.FillAsync("input[name='AddEndTime']", endTime.ToString("yyyy-MM-ddTHH:mm"));
        await Page.ClickAsync("form[action*='AddAvailability'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task<int> GetAvailabilityCountAsync()
    {
        var rows = await Page.QuerySelectorAllAsync("table tbody tr, .list-group-item");
        return rows.Count;
    }

    public async Task DeleteFirstAvailabilityAsync()
    {
        Page.Dialog += (_, dialog) => dialog.AcceptAsync();
        var deleteBtn = await Page.QuerySelectorAsync("form[action*='DeleteAvailability'] button[type='submit']");
        if (deleteBtn is not null)
        {
            await deleteBtn.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    public async Task<bool> HasErrorAsync() => await HasValidationErrorsAsync();
}
