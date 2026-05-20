using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class EquipmentAssignPage : PageBase
{
    public EquipmentAssignPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync(int equipmentId) =>
        await NavigateToAsync($"/Client/Equipment/Assign/{equipmentId}");

    public async Task SelectListingAsync(int index)
    {
        var checkboxes = await Page.QuerySelectorAllAsync("input[name='selectedEquipmentIds']");
        if (index < checkboxes.Count)
            await checkboxes[index].CheckAsync();
    }

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
