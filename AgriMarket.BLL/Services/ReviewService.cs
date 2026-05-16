using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class ReviewService(
    IRepository<Review> reviews,
    IRepository<UserProfile> userProfiles,
    IBookingRepository bookings,
    IUnitOfWork uow,
    IQueryMaterializer mat,
    ILogger<ReviewService> logger) : IReviewService
{
    public async Task<IEnumerable<ReviewDto>> GetByBookingAsync(Guid bookingId)
    {
        var items = await reviews.FindAsync(r => r.BookingId == bookingId);
        return items.OrderByDescending(r => r.CreatedAt).Select(ToReviewDto);
    }

    public async Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
    {
        var query = reviews.Query();
        var totalCount = await mat.CountAsync(query);
        var items = await mat.ToListAsync(
            query.OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize));

        return (items.Select(ToReviewDto), totalCount);
    }

    public async Task<ReviewDto?> GetByIdAsync(Guid id)
    {
        var review = await reviews.FirstOrDefaultAsync(r => r.Id == id);
        return review is null ? null : ToReviewDto(review);
    }

    public async Task<ReviewDto> CreateAsync(Guid userId, CreateReviewDto dto)
    {
        var reviewerProfile = await userProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);
        if (reviewerProfile is null)
            throw new BusinessRuleException("User profile not found.");

        var booking = await bookings.GetByIdWithDetailsAsync(dto.BookingId);
        if (booking == null)
            throw new KeyNotFoundException("Booking not found.");

        if (booking.Status != BookingStatus.ClientConfirmed && booking.Status != BookingStatus.ProviderCompleted)
            throw new BusinessRuleException("Cannot review a booking that is not completed.");

        var reviewedProfileId = booking.ServiceListing?.UserProfileId
            ?? throw new BusinessRuleException("Cannot determine service provider for this booking.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            BookingId = dto.BookingId,
            ReviewerProfileId = reviewerProfile.Id,
            ReviewedProfileId = reviewedProfileId
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
