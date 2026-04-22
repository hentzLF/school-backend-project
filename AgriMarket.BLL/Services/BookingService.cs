using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;

    public BookingService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync(BookingStatus? status = null)
    {
        var query = _db.Bookings
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        return await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _db.Bookings
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .Include(b => b.Availability)
            .Include(b => b.Payment)
            .Include(b => b.Review)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetByClientAsync(Guid clientProfileId)
    {
        return await _db.Bookings
            .Include(b => b.ServiceListing)
            .Where(b => b.ClientProfileId == clientProfileId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByProviderAsync(Guid providerProfileId)
    {
        return await _db.Bookings
            .Include(b => b.ServiceListing)
            .Where(b => b.ServiceListing!.UserProfileId == providerProfileId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        // Enforce provider self-booking rule
        var listing = await _db.ServiceListings.FindAsync(booking.ServiceListingId);
        if (listing != null && listing.UserProfileId == booking.ClientProfileId)
        {
            throw new InvalidOperationException("Providers cannot book their own services.");
        }

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateStatusAsync(Guid id, BookingStatus status, Guid? callerProfileId = null)
    {
        var booking = await _db.Bookings
            .Include(b => b.ServiceListing)
            .FirstOrDefaultAsync(b => b.Id == id);
            
        if (booking != null)
        {
            if (callerProfileId.HasValue)
            {
                var isClient = booking.ClientProfileId == callerProfileId.Value;
                var isProvider = booking.ServiceListing?.UserProfileId == callerProfileId.Value;

                if (!isClient && !isProvider)
                    throw new UnauthorizedAccessException("You are not a party to this booking.");

                var allowed = GetAllowedTransitions(booking.Status, isClient, isProvider);
                if (!allowed.Contains(status))
                    throw new InvalidOperationException($"Transition from {booking.Status} to {status} is not permitted for your role.");
            }

            booking.Status = status;
            await _db.SaveChangesAsync();
        }
    }

    private static IReadOnlySet<BookingStatus> GetAllowedTransitions(BookingStatus current, bool isClient, bool isProvider)
    {
        var result = new HashSet<BookingStatus>();

        if (isClient)
        {
            if (current == BookingStatus.Pending) result.Add(BookingStatus.Cancelled);
            if (current == BookingStatus.Confirmed) result.Add(BookingStatus.Cancelled);
            if (current == BookingStatus.ProviderCompleted) result.Add(BookingStatus.ClientConfirmed);
        }

        if (isProvider)
        {
            if (current == BookingStatus.Pending)
            {
                result.Add(BookingStatus.Confirmed);
                result.Add(BookingStatus.Cancelled);
            }
            if (current == BookingStatus.Confirmed) result.Add(BookingStatus.InProgress);
            if (current == BookingStatus.InProgress) result.Add(BookingStatus.ProviderCompleted);
            var terminal = new[] { BookingStatus.Cancelled, BookingStatus.ClientConfirmed, BookingStatus.Disputed };
            if (!terminal.Contains(current)) result.Add(BookingStatus.Disputed);
        }

        return result;
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking != null)
        {
            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountByListingAsync(Guid listingId)
    {
        return await _db.Bookings.CountAsync(b => b.ServiceListingId == listingId);
    }

    public async Task<bool> HasActiveBookingsAsync(Guid listingId)
    {
        var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.Confirmed, BookingStatus.InProgress, BookingStatus.ProviderCompleted };
        return await _db.Bookings.AnyAsync(b => b.ServiceListingId == listingId && activeStatuses.Contains(b.Status));
    }

    public async Task<IEnumerable<Booking>> GetByListingAsync(Guid listingId)
    {
        return await _db.Bookings
            .Include(b => b.ClientProfile)
            .Where(b => b.ServiceListingId == listingId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Booking> Items, int TotalCount)> GetAllForProfileAsync(Guid profileId, int page, int pageSize)
    {
        var query = _db.Bookings
            .Include(b => b.ServiceListing)
            .Where(b => b.ClientProfileId == profileId || b.ServiceListing!.UserProfileId == profileId)
            .AsNoTracking();

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
