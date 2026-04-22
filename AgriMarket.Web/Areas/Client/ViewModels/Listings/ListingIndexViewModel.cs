namespace AgriMarket.Web.Areas.Client.ViewModels.Listings;

public class ListingIndexViewModel
{
    public IEnumerable<ListingIndexItemViewModel> Listings { get; set; } = [];
}

public class ListingIndexItemViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public string ProviderName { get; set; } = default!;
    public decimal PricePerHectare { get; set; }
}
