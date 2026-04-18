namespace AgriMarket.Domain.Entities;

public class Equipment
{
    public Guid Id { get; set; }

    public Guid ServiceListingId { get; set; }

    public string Name { get; set; } = default!;

    public string? Model { get; set; }

    public int? ManufactureYear { get; set; }

    public string? Description { get; set; }

    // Navigation
    public ServiceListing? ServiceListing { get; set; }
}