namespace AgriMarket.Api.Dtos.Reviews;

public class ReviewResponse
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid BookingId { get; set; }
    public Guid ReviewerProfileId { get; set; }
}
