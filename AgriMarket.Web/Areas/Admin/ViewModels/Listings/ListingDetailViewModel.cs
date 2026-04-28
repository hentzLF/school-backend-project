namespace AgriMarket.Web.Areas.Admin.ViewModels;

public class ListingDetailViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal PricePerHectare { get; set; }
    public bool IsActive { get; set; }
    public string ProviderName { get; set; } = default!;
    public Guid ProviderProfileId { get; set; }
    public string CategoryName { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public int BookingsCount { get; set; }
    public IEnumerable<ListingEquipmentViewModel> Equipments { get; set; } = [];
    public IEnumerable<ListingAvailabilityViewModel> Availabilities { get; set; } = [];
}

public class ListingEquipmentViewModel
{
    public string Name { get; set; } = default!;
    public string? Model { get; set; }
    public int? ManufactureYear { get; set; }
}

public class ListingAvailabilityViewModel
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }
}
