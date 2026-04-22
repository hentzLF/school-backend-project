using AgriMarket.BLL.Dtos.Bookings;
using AgriMarket.BLL;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class BookingService(
    IRepository<Booking> bookingRepo,
    IRepository<UserProfile> userProfiles,
    IRepository<ServiceListing> serviceListings,
    IRepository<Availability> availabilities,
    IUnitOfWork uow,
    ILogger<BookingService> logger) : IBookingService
{
    public async Task<IEnumerable<BookingDto>> GetAllAsync(BookingStatus? status = null)
    {
        var query = BuildBaseQuery();
        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        var bookings = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return bookings.Select(ToBookingDto);
    }

    public async Task<BookingDto?> GetByIdAsync(Guid id)
    {
        var booking = await BuildBaseQuery().FirstOrDefaultAsync(b => b.Id == id);
        return booking is null ? null : ToBookingDto(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetByClientAsync(Guid clientProfileId)
    {
        var bookings = await BuildBaseQuery()
            .Where(b => b.ClientProfileId == clientProfileId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookings.Select(ToBookingDto);
    }

    public async Task<IEnumerable<BookingDto>> GetByProviderAsync(Guid providerProfileId)
    {
        var bookings = await BuildBaseQuery()
            .Where(b => b.ServiceListing!.UserProfileId == providerProfileId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookings.Select(ToBookingDto);
    }

    public async Task<BookingDto> CreateAsync(Guid userId, CreateBookingDto dto)
    {
        var clientProfile = await userProfiles.Query().AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (clientProfile is null)
            throw new BusinessRuleException("User profile not found.");

        var listing = await serviceListings.Query().AsNoTracking().FirstOrDefaultAsync(l => l.Id == dto.ServiceListingId);
        if (listing is null)
            throw new KeyNotFoundException($"ServiceListing {dto.ServiceListingId} not found.");

        if (listing.UserProfileId == clientProfile.Id)
            throw new BusinessRuleException("Providers cannot book their own services.");

        var availability = await availabilities.Query().FirstOrDefaultAsync(a => a.Id == dto.AvailabilityId);
        if (availability is null || availability.ServiceListingId != dto.ServiceListingId)
            throw new BusinessRuleException("Availability does not belong to the selected listing.");

        if (availability.IsBooked)
            throw new BusinessRuleException("The selected availability is no longer available.");

        availability.IsBooked = true;
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = BookingStatus.Pending,
            TotalPrice = (decimal)dto.AreaInHectares * listing.PricePerHectare,
            AreaInHectares = dto.AreaInHectares,
            CreatedAt = DateTime.UtcNow,
            Notes = dto.Notes,
            ServiceListingId = dto.ServiceListingId,
            ClientProfileId = clientProfile.Id,
            AvailabilityId = dto.AvailabilityId
        };

        bookingRepo.Add(booking);
        await uow.SaveChangesAsync();
        return (await GetByIdAsync(booking.Id))!;
    }

    public async Task<BookingDto> UpdateStatusAsync(Guid id, BookingStatus status, Guid? callerProfileId = null)
    {
        var booking = await bookingRepo.Query()
            .Include(b => b.ServiceListing)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (booking is null)
            throw new KeyNotFoundException($"Booking {id} not found.");

        if (callerProfileId.HasValue)
        {
            var isClient = booking.ClientProfileId == callerProfileId.Value;
            var isProvider = booking.ServiceListing?.UserProfileId == callerProfileId.Value;

            if (!isClient && !isProvider)
                throw new UnauthorizedAccessException("You are not a party to this booking.");

            var allowed = GetAllowedTransitions(booking.Status, isClient, isProvider);
            if (!allowed.Contains(status))
                throw new BusinessRuleException($"Transition from {booking.Status} to {status} is not permitted for your role.");
        }

        booking.Status = status;
        await uow.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
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
        var booking = await bookingRepo.GetByIdAsync(id);
        if (booking != null)
        {
            bookingRepo.Remove(booking);
            await uow.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountByListingAsync(Guid listingId)
    {
        return await bookingRepo.CountAsync(b => b.ServiceListingId == listingId);
    }

    public async Task<bool> HasActiveBookingsAsync(Guid listingId)
    {
        var activeStatuses = new[] { BookingStatus.Pending, BookingStatus.Confirmed, BookingStatus.InProgress, BookingStatus.ProviderCompleted };
        return await bookingRepo.AnyAsync(b => b.ServiceListingId == listingId && activeStatuses.Contains(b.Status));
    }

    public async Task<IEnumerable<BookingSummaryDto>> GetByListingAsync(Guid listingId)
    {
        var items = await bookingRepo.Query()
            .AsNoTracking()
            .Include(b => b.ClientProfile)
            .Where(b => b.ServiceListingId == listingId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return items.Select(b => new BookingSummaryDto
        {
            Id = b.Id,
            ClientName = b.ClientProfile is null
                ? "Unknown"
                : $"{b.ClientProfile.FirstName} {b.ClientProfile.LastName}",
            Status = b.Status,
            AreaInHectares = b.AreaInHectares,
            TotalPrice = b.TotalPrice,
            CreatedAt = b.CreatedAt
        });
    }

    public async Task<(IEnumerable<BookingDto> Items, int TotalCount)> GetAllForProfileAsync(Guid profileId, int page, int pageSize)
    {
        var query = BuildBaseQuery()
            .Where(b => b.ClientProfileId == profileId || b.ServiceListing!.UserProfileId == profileId);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items.Select(ToBookingDto), totalCount);
    }

    private IQueryable<Booking> BuildBaseQuery()
    {
        return bookingRepo.Query()
            .AsNoTracking()
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .Include(b => b.Availability)
            .Include(b => b.Payment)
            .Include(b => b.Review);
    }

    private static BookingDto ToBookingDto(Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            AreaInHectares = booking.AreaInHectares,
            CreatedAt = booking.CreatedAt,
            Notes = booking.Notes,
            ServiceListingId = booking.ServiceListingId,
            ClientProfileId = booking.ClientProfileId,
            AvailabilityId = booking.AvailabilityId,
            ClientName = booking.ClientProfile is null
                ? "Unknown"
                : $"{booking.ClientProfile.FirstName} {booking.ClientProfile.LastName}",
            ListingTitle = booking.ServiceListing?.Title ?? "Unknown",
            ProviderProfileId = booking.ServiceListing?.UserProfileId ?? Guid.Empty,
            AvailabilityStart = booking.Availability?.StartTime ?? default,
            AvailabilityEnd = booking.Availability?.EndTime ?? default
        };
    }
}
