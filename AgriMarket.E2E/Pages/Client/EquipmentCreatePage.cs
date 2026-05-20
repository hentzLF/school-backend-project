using AgriMarket.E2E.Infrastructure;
using Microsoft.Playwright;

namespace AgriMarket.E2E.Pages.Client;

public sealed class EquipmentCreatePage : PageBase
{
    public EquipmentCreatePage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task NavigateAsync() => await NavigateToAsync("/Client/Equipment/Create");

    public async Task FillFormAsync(
        string name, string make, string model,
        string year, string horsePower, int conditionIndex = 0)
    {
        await Page.FillAsync("input[name='Name']", name);
        await Page.FillAsync("input[name='Make']", make);
        await Page.FillAsync("input[name='Model']", model);
        await Page.FillAsync("input[name='ManufactureYear']", year);
        await Page.FillAsync("input[name='HorsePower']", horsePower);

        var options = await Page.QuerySelectorAllAsync("select[name='Condition'] option");
        if (conditionIndex < options.Count)
        {
            var value = await options[conditionIndex].GetAttributeAsync("value");
            if (value is not null)
                await Page.SelectOptionAsync("select[name='Condition']", value);
        }
    }

    public async Task SubmitAsync()
    {
        await Page.ClickAsync("input[type='submit'][value='Create'], button[type='submit']");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<bool> HasErrorAsync() => await HasValidationErrorsAsync();
}
