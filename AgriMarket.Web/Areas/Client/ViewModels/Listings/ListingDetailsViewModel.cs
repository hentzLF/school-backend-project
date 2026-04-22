using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgriMarket.Web.Areas.Client.ViewModels.Listings;

public class ListingDetailsViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal PricePerHectare { get; set; }
    public string CategoryName { get; set; } = default!;
    public string ProviderName { get; set; } = default!;
    public IEnumerable<AvailabilityOptionViewModel> Availabilities { get; set; } = [];
    public bool IsOwnListing { get; set; }
}

public class AvailabilityOptionViewModel
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
