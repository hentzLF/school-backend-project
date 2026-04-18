namespace AgriMarket.Domain.Entities;

public class MessageRead
{
    public Guid Id { get; set; }

    public Guid UserProfileId { get; set; }

    public DateTime ReadAt { get; set; }

    // Fk
    public Guid MessageId { get; set; }

    // Navigation
    public Message? Message { get; set; }
    public UserProfile? UserProfile { get; set; }
}