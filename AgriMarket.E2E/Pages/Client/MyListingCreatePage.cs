using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class MyListingCreatePage : PageBase
{
    public MyListingCreatePage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/MyListings/Create");

    public async Task FillFormAsync(string title, string description, string pricePerHectare, int? categoryIndex = null)
    {
        await Page.FillAsync("input[name='Title']", title);
        await Page.FillAsync("textarea[name='Description']", description);
        await Page.FillAsync("input[name='PricePerHectare']", pricePerHectare);

        if (categoryIndex.HasValue)
        {
            var options = await Page.QuerySelectorAllAsync("select[name='ServiceCategoryId'] option:not([value=''])");
            if (categoryIndex.Value < options.Count)
            {
                var value = await options[categoryIndex.Value].GetAttributeAsync("value");
                await Page.SelectOptionAsync("select[name='ServiceCategoryId']", value!);
            }
        }
        else
        {
            var firstOption = await Page.QuerySelectorAsync("select[name='ServiceCategoryId'] option:not([value=''])");
            if (firstOption is not null)
            {
                var value = await firstOption.GetAttributeAsync("value");
                await Page.SelectOptionAsync("select[name='ServiceCategoryId']", value!);
            }
        }
    }

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("input[type='submit'][value='Create'], button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> HasErrorAsync() => await HasValidationErrorsAsync();
}
