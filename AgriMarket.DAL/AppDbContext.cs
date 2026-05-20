using AgriMarket.DAL.Seeding;
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

    modelBuilder.Entity<ConversationParticipant>()
        .HasKey(cp => new { cp.ConversationId, cp.UserProfileId });

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

    // Equipment → UserProfile: kustuta koos profiiliga
    modelBuilder.Entity<Equipment>()
        .HasOne(e => e.UserProfile)
        .WithMany(up => up.Equipments)
        .HasForeignKey(e => e.UserProfileId)
        .OnDelete(DeleteBehavior.Cascade);

    // ServiceListingEquipment: N:M join tabel
    modelBuilder.Entity<ServiceListingEquipment>()
        .HasKey(sle => new { sle.ServiceListingId, sle.EquipmentId });

    modelBuilder.Entity<ServiceListingEquipment>()
        .HasOne(sle => sle.ServiceListing)
        .WithMany(sl => sl.ServiceListingEquipments)
        .HasForeignKey(sle => sle.ServiceListingId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<ServiceListingEquipment>()
        .HasOne(sle => sle.Equipment)
        .WithMany(e => e.ServiceListingEquipments)
        .HasForeignKey(sle => sle.EquipmentId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Availability>()
        .HasOne(a => a.ServiceListing)
        .WithMany(sl => sl.Availabilities)
        .HasForeignKey(a => a.ServiceListingId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Availability>()
        .Property(a => a.RowVersion)
        .IsRowVersion();

    // UserRole: sama kasutaja ei saa sama rolli kaks korda
    modelBuilder.Entity<UserRole>()
        .HasIndex(ur => new { ur.AppUserId, ur.Role })
        .IsUnique();

    // UserProfile: üks profiil kasutaja kohta (1:1)
    modelBuilder.Entity<UserProfile>()
        .HasIndex(up => up.AppUserId)
        .IsUnique();

    modelBuilder.Entity<AppUser>()
        .HasOne(u => u.Profile)
        .WithOne(p => p.AppUser)
        .HasForeignKey<UserProfile>(p => p.AppUserId);

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

    // County → Municipality: maakonda ei saa kustutada kui omavalitsused on olemas
    modelBuilder.Entity<County>()
        .HasIndex(c => c.EhakCode)
        .IsUnique();

    modelBuilder.Entity<Municipality>()
        .HasIndex(m => m.EhakCode)
        .IsUnique();

    modelBuilder.Entity<Municipality>()
        .HasOne(m => m.County)
        .WithMany(c => c.Municipalities)
        .HasForeignKey(m => m.CountyId)
        .OnDelete(DeleteBehavior.Restrict);

    // Municipality → Location: omavalitsust ei saa kustutada kui asukohad viitavad
    modelBuilder.Entity<Location>()
        .HasOne(l => l.Municipality)
        .WithMany(m => m.Locations)
        .HasForeignKey(l => l.MunicipalityId)
        .OnDelete(DeleteBehavior.Restrict);

    // ServiceListing → Location: kustuta location koos listinguga
    modelBuilder.Entity<ServiceListing>()
        .HasOne(sl => sl.Location)
        .WithMany()
        .HasForeignKey(sl => sl.LocationId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<County>().HasData(CountySeedData.GetAll());
    modelBuilder.Entity<Municipality>().HasData(MunicipalitySeedData.GetAll());

    modelBuilder.Entity<Booking>()
        .HasIndex(b => b.Status);

    modelBuilder.Entity<ServiceListing>()
        .HasIndex(sl => sl.IsActive);

    modelBuilder.Entity<Availability>()
        .HasIndex(a => a.IsBooked);

    modelBuilder.Entity<Message>()
        .HasIndex(m => m.SentAt);

}


    public DbSet<AppUser> AppUsers {get; set;} = default!;
    public DbSet<UserProfile> UserProfiles {get; set;} = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;
    public DbSet<County> Counties { get; set; } = default!;
    public DbSet<Municipality> Municipalities { get; set; } = default!;
    public DbSet<Location> Locations  {get; set;} = default!;
    public DbSet<ServiceCategory> ServiceCategories {get; set;} = default!;
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
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
    public DbSet<ServiceListingEquipment> ServiceListingEquipments { get; set; } = default!;
}
