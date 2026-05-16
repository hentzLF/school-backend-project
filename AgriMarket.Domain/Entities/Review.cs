namespace AgriMarket.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    // FK
    public Guid BookingId { get; set; }
    public Guid ReviewerProfileId { get; set; }
    public Guid ReviewedProfileId { get; set; }

    // Navigation
    public Booking? Booking { get; set; }
    public UserProfile? ReviewerProfile { get; set; }
    public UserProfile? ReviewedProfile { get; set; }
}