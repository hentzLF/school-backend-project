using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.BLL;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class ReviewService(
    IRepository<Review> reviews,
    IRepository<UserProfile> userProfiles,
    IRepository<Booking> bookings,
    IUnitOfWork uow) : IReviewService
{
    public async Task<IEnumerable<ReviewDto>> GetByBookingAsync(Guid bookingId)
    {
        var items = await reviews.Query()
            .AsNoTracking()
            .Where(r => r.BookingId == bookingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return items.Select(ToReviewDto);
    }

    public async Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = reviews.Query().AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items.Select(ToReviewDto), totalCount);
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id)
    {
        var review = await reviews.Query().AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        return review is null ? null : ToReviewDto(review);
    }

    public async Task<ReviewDto> CreateAsync(Guid userId, CreateReviewDto dto)
    {
        var reviewerProfile = await userProfiles.Query().AsNoTracking().FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (reviewerProfile is null)
            throw new BusinessRuleException("User profile not found.");

        var booking = await bookings.Query().AsNoTracking().FirstOrDefaultAsync(b => b.Id == dto.BookingId);
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

        reviews.Add(review);
        await uow.SaveChangesAsync();
        return ToReviewDto(review);
    }

    private static ReviewDto ToReviewDto(Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            BookingId = review.BookingId,
            ReviewerProfileId = review.ReviewerProfileId
        };
    }
}
