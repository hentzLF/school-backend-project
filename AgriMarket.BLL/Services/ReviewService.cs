using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.BLL.Mappers;
using AgriMarket.BLL;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;

    public ReviewService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ReviewDto>> GetByBookingAsync(Guid bookingId)
    {
        var reviews = await _db.Reviews
            .AsNoTracking()
            .Where(r => r.BookingId == bookingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(r => r.ToReviewDto());
    }

    public async Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = _db.Reviews.AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items.Select(r => r.ToReviewDto()), totalCount);
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id)
    {
        var review = await _db.Reviews.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        return review?.ToReviewDto();
    }

    public async Task<ReviewDto> CreateAsync(Guid userId, CreateReviewDto dto)
    {
        var reviewerProfile = await _db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (reviewerProfile is null)
            throw new BusinessRuleException("User profile not found.");

        var booking = await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == dto.BookingId);
        if (booking == null)
            throw new KeyNotFoundException("Booking not found.");

        if (booking.Status != BookingStatus.ClientConfirmed && booking.Status != BookingStatus.ProviderCompleted)
            throw new BusinessRuleException("Cannot review a booking that is not completed.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            BookingId = dto.BookingId,
            ReviewerProfileId = reviewerProfile.Id
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return review.ToReviewDto();
    }
}
