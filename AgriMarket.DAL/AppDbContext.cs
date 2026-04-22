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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // AppUser: email peab olema unikaalne
    modelBuilder.Entity<AppUser>()
        .HasIndex(u => u.Email)
        .IsUnique();

    // OAuthAccount: üks provider (nt Google) + providerAccountId peab olema unikaalne
    modelBuilder.Entity<OAuthAccount>()
        .HasIndex(o => new { o.Provider, o.ProviderAccountId })
        .IsUnique();

    // ConversationParticipant: sama kasutaja ei saa olla samas vestluses kaks korda
    modelBuilder.Entity<ConversationParticipant>()
        .HasIndex(cp => new { cp.ConversationId, cp.UserProfileId })
        .IsUnique();

    // MessageRead: sama kasutaja ei saa sama sõnumit kaks korda lugeda
    modelBuilder.Entity<MessageRead>()
        .HasIndex(mr => new { mr.MessageId, mr.UserProfileId })
        .IsUnique();

    // Booking → Payment: kui booking kustutatakse, kustuta ka payment
    modelBuilder.Entity<Payment>()
        .HasOne(p => p.Booking)
        .WithOne(b => b.Payment)
        .HasForeignKey<Payment>(p => p.BookingId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Review>()
        .HasOne(p => p.Booking)
        .WithOne(r => r.Review)
        .HasForeignKey<Review>(p => p.BookingId)
        .OnDelete(DeleteBehavior.Cascade);

    // ServiceListing → Booking: listing ei saa kustutada kui bookings on olemas
    modelBuilder.Entity<Booking>()
        .HasOne(b => b.ServiceListing)
        .WithMany()
        .HasForeignKey(b => b.ServiceListingId)
        .OnDelete(DeleteBehavior.Restrict);

    // Availability → Booking: availability ei saa kustutada kui booking on olemas
    modelBuilder.Entity<Booking>()
        .HasOne(b => b.Availability)
        .WithMany()
        .HasForeignKey(b => b.AvailabilityId)
        .OnDelete(DeleteBehavior.Restrict);

    // Booking.ClientProfileId → UserProfile: keela cascade (mitu teed UserProfile-ist)
    modelBuilder.Entity<Booking>()
        .HasOne(b => b.ClientProfile)
        .WithMany(up => up.ClientBookings)
        .HasForeignKey(b => b.ClientProfileId)
        .OnDelete(DeleteBehavior.Restrict);

    // Review.ReviewerProfileId → UserProfile: keela cascade
    modelBuilder.Entity<Review>()
        .HasOne(r => r.ReviewerProfile)
        .WithMany(up => up.Reviews)
        .HasForeignKey(r => r.ReviewerProfileId)
        .OnDelete(DeleteBehavior.Restrict);

    // Review.ReviewedProfileId → UserProfile: keela cascade
    modelBuilder.Entity<Review>()
        .HasOne(r => r.ReviewedProfile)
        .WithMany()
        .HasForeignKey(r => r.ReviewedProfileId)
        .OnDelete(DeleteBehavior.Restrict);

    // Message.SenderProfileId → UserProfile: keela cascade
    modelBuilder.Entity<Message>()
        .HasOne(m => m.SenderProfile)
        .WithMany(up => up.SentMessages)
        .HasForeignKey(m => m.SenderProfileId)
        .OnDelete(DeleteBehavior.Restrict);

    // Conversation.BookingId: nullable FK — booking kustutamisel jäta vestlus alles
    modelBuilder.Entity<Conversation>()
        .HasOne(c => c.Booking)
        .WithMany()
        .HasForeignKey(c => c.BookingId)
        .OnDelete(DeleteBehavior.SetNull);

    // ServiceListing → Equipment ja Availability: kustuta koos listinguga
    modelBuilder.Entity<Equipment>()
        .HasOne(e => e.ServiceListing)
        .WithMany(sl => sl.Equipments)
        .HasForeignKey(e => e.ServiceListingId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Availability>()
        .HasOne(a => a.ServiceListing)
        .WithMany(sl => sl.Availabilities)
        .HasForeignKey(a => a.ServiceListingId)
        .OnDelete(DeleteBehavior.Cascade);

    // ProfileRole: sama kasutaja ei saa sama rolli kaks korda
    modelBuilder.Entity<ProfileRole>()
        .HasIndex(pr => new { pr.UserProfileId, pr.Role })
        .IsUnique();

    // ServiceCategory: nimi peab olema unikaalne
    modelBuilder.Entity<ServiceCategory>()
        .HasIndex(sc => sc.Name)
        .IsUnique();

    // ConversationParticipant → UserProfile: keela cascade (mitu teed UserProfile-ist)
    modelBuilder.Entity<ConversationParticipant>()
        .HasOne(cp => cp.UserProfile)
        .WithMany(up => up.ConversationParticipants)
        .HasForeignKey(cp => cp.UserProfileId)
        .OnDelete(DeleteBehavior.Restrict);

    // MessageRead → UserProfile: keela cascade (mitu teed UserProfile-ist)
    modelBuilder.Entity<MessageRead>()
        .HasOne(mr => mr.UserProfile)
        .WithMany(up => up.MessageReads)
        .HasForeignKey(mr => mr.UserProfileId)
        .OnDelete(DeleteBehavior.Restrict);

    // Notification → UserProfile: keela cascade (mitu teed UserProfile-ist)
    modelBuilder.Entity<Notification>()
        .HasOne(n => n.UserProfile)
        .WithMany(up => up.Notifications)
        .HasForeignKey(n => n.UserProfileId)
        .OnDelete(DeleteBehavior.Restrict);

    // Conversation → Message: kustuta sõnumid koos vestlusega
    modelBuilder.Entity<Message>()
        .HasOne(m => m.Conversation)
        .WithMany(c => c.Messages)
        .HasForeignKey(m => m.ConversationId)
        .OnDelete(DeleteBehavior.Cascade);

    // Message → MessageRead: kustuta lugemismärgid koos sõnumiga
    modelBuilder.Entity<MessageRead>()
        .HasOne(mr => mr.Message)
        .WithMany(m => m.MessageReads)
        .HasForeignKey(mr => mr.MessageId)
        .OnDelete(DeleteBehavior.Cascade);

    // ServiceListing → ServiceCategory: kategooriat ei saa kustutada kui listingud on olemas
    modelBuilder.Entity<ServiceListing>()
        .HasOne(sl => sl.ServiceCategory)
        .WithMany()
        .HasForeignKey(sl => sl.ServiceCategoryId)
        .OnDelete(DeleteBehavior.Restrict);

    // ServiceListing → Location: nullable FK, kustutamisel jäta listing alles
    modelBuilder.Entity<ServiceListing>()
        .HasOne(sl => sl.Location)
        .WithMany()
        .HasForeignKey(sl => sl.LocationId)
        .OnDelete(DeleteBehavior.SetNull);

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
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
}
