using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class ListingDetailPage : PageBase
{
    public ListingDetailPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int listingId) =>
        await NavigateToAsync($"/Client/Listings/Details/{listingId}");

    public async Task<string> GetTitleAsync() =>
        await Page.InnerTextAsync("h2, h1");

    public async Task<bool> HasEquipmentSectionAsync() =>
        await IsElementVisibleAsync(".equipment-card, [class*='equipment']");

    public async Task<bool> HasReviewSectionAsync() =>
        await IsElementVisibleAsync("[class*='review']");

    public async Task<bool> HasBookingFormAsync() =>
        await IsElementVisibleAsync("form[action*='Book']");

    public async Task FillBookingFormAsync(string areaInHectares)
    {
        var selectOption = await Page.QuerySelectorAsync("select[name='AvailabilityId'] option:not([value=''])");
        if (selectOption is not null)
        {
            var value = await selectOption.GetAttributeAsync("value");
            await Page.SelectOptionAsync("select[name='AvailabilityId']", value!);
        }
        await Page.FillAsync("input[name='AreaInHectares']", areaInHectares);
    }

    public async Task SubmitBookingAsync()
    {
        await Page.ClickAsync("form[action*='Book'] button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task<int> GetAvailabilityCountAsync()
    {
        var options = await Page.QuerySelectorAllAsync("select[name='AvailabilityId'] option:not([value=''])");
        return options.Count;
    }

    public async Task<IReadOnlyList<string>> GetEquipmentNamesAsync()
    {
        var items = await Page.QuerySelectorAllAsync(".equipment-card .card-title, [class*='equipment'] h5, [class*='equipment'] h6");
        var result = new List<string>();
        foreach (var item in items)
            result.Add(await item.InnerTextAsync());
        return result;
    }
}
