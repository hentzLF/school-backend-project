using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> AppUsers {get; set;} = default!;
    public DbSet<UserProfile> UserProfiles {get; set;} = default!;
    public DbSet<ProfileRole> ProfileRoles {get; set;} = default!;
    public DbSet<Location> Locations  {get; set;} = default!;
    public DbSet<ServiceCategory> ServiceCategories {get; set;} = default!;
    public DbSet<OAuthAccount> OAuthAccounts { get; set; } = default!;
    public DbSet<ServiceListing> ServiceListings { get; set; } = default!;
    public DbSet<Equipment> Equipments { get; set; } = default!;
    public DbSet<Availability> Availabilities { get; set; } = default!;
    public DbSet<Booking> Bookings { get; set; } = default!;
    public DbSet<Payment> Payments { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;
    public DbSet<Conversation> Conversations { get; set; } = default!;
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; } = default!;
    public DbSet<Message> Messages { get; set; } = default!;
    public DbSet<MessageRead> MessageReads { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; } = default!;
}
