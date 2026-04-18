using AgriMarket.Domain.Enums;

namespace AgriMarket.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }

    public BookingStatus Status { get; set; }

    public decimal TotalPrice { get; set; }

    public double AreaInHectares { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Notes { get; set; }

    // Foreign Keys
    public Guid ServiceListingId { get; set; }

    public Guid ClientProfileId { get; set; }

    public Guid AvailabilityId { get; set; }
    // Navigation
    public ServiceListing? ServiceListing { get; set; }
    public UserProfile? ClientProfile { get; set; }
    public Availability? Availability { get; set; }
    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
}