namespace AgriMarket.Domain.Entities;

public class Availability
{
    public Guid Id { get; set; }

    public Guid ServiceListingId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public bool IsBooked { get; set; }

    // Navigation
    public ServiceListing? ServiceListing { get; set; }
}