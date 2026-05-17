namespace AgriMarket.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    // Foreign Keys
    public Guid AppUserId { get; set; }

    // Navigation
    public AppUser? AppUser { get; set; }


    public ICollection<ProfileRole>? Roles { get; set; }
    public ICollection<ServiceListing>? ServiceListings { get; set; }
    public ICollection<Booking>? ClientBookings { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<ConversationParticipant>? ConversationParticipants { get; set; }
    public ICollection<Message>? SentMessages { get; set; }
    public ICollection<MessageRead>? MessageReads { get; set; }
}