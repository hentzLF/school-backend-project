namespace AgriMarket.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    // FK
    public Guid? BookingId { get; set; }
    
    // Navigation
    public Booking? Booking { get; set; }
    public ICollection<ConversationParticipant>? Participants { get; set; }
    public ICollection<Message>? Messages { get; set; }
}