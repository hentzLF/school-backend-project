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
    public async Task<ReviewDto?> GetByBookingAsync(Guid bookingId)
    {
        var review = await reviews.FirstOrDefaultAsync(r => r.BookingId == bookingId);
        return review is null ? null : ToReviewDto(review);
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
        var reviewerProfile = await ResolveProfileOrThrow(userId);

        var booking = await bookings.GetByIdWithDetailsAsync(dto.BookingId);
        if (booking == null)
            throw new KeyNotFoundException("Booking not found.");

        if (booking.ClientProfileId != reviewerProfile.Id)
            throw new BusinessRuleException("Only the client can review a booking.");

        if (booking.Status != BookingStatus.ClientConfirmed && booking.Status != BookingStatus.ProviderCompleted)
            throw new BusinessRuleException("Cannot review a booking that is not completed.");

        var alreadyReviewed = await reviews.AnyAsync(r => r.BookingId == dto.BookingId);
        if (alreadyReviewed)
            throw new BusinessRuleException("A review already exists for this booking.");

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

    public async Task<(IEnumerable<ReviewDto> Items, int TotalCount)> GetByProfileAsync(Guid profileId, int page, int pageSize)
    {
        var query = reviews.Query().Where(r => r.ReviewedProfileId == profileId);
        var totalCount = await mat.CountAsync(query);
        var items = await mat.ToListAsync(
            query.OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize));

        return (items.Select(ToReviewDto), totalCount);
    }

    public async Task<ReviewDto> UpdateAsync(Guid userId, UpdateReviewDto dto)
    {
        var profile = await ResolveProfileOrThrow(userId);

        var review = await reviews.FirstOrDefaultAsync(r => r.Id == dto.Id)
            ?? throw new KeyNotFoundException($"Review {dto.Id} not found.");

        if (review.ReviewerProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this review.");

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        await uow.SaveChangesAsync();
        return ToReviewDto(review);
    }

    public async Task DeleteAsync(Guid userId, Guid reviewId)
    {
        var profile = await ResolveProfileOrThrow(userId);

        var review = await reviews.FirstOrDefaultAsync(r => r.Id == reviewId)
            ?? throw new KeyNotFoundException($"Review {reviewId} not found.");

        if (review.ReviewerProfileId != profile.Id)
            throw new BusinessRuleException("You do not own this review.");

        reviews.Remove(review);
        await uow.SaveChangesAsync();
    }

    public async Task<RatingStatsDto> GetRatingStatsForProfileAsync(Guid profileId)
    {
        var query = reviews.Query().Where(r => r.ReviewedProfileId == profileId);
        return await ComputeRatingStats(query);
    }

    public async Task<RatingStatsDto> GetRatingStatsForListingAsync(Guid listingId)
    {
        var query = reviews.Query().Where(r => r.Booking!.ServiceListingId == listingId);
        return await ComputeRatingStats(query);
    }

    private async Task<RatingStatsDto> ComputeRatingStats(IQueryable<Review> query)
    {
        var count = await mat.CountAsync(query);
        if (count == 0)
            return new RatingStatsDto { AverageRating = 0, ReviewCount = 0 };

        var sum = await mat.SumAsync(query, r => (decimal?)r.Rating);
        return new RatingStatsDto
        {
            AverageRating = Math.Round((double)sum / count, 2),
            ReviewCount = count
        };
    }

    private async Task<UserProfile> ResolveProfileOrThrow(Guid userId)
    {
        return await userProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId)
            ?? throw new BusinessRuleException("User profile not found.");
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
            ReviewerProfileId = review.ReviewerProfileId,
            ReviewedProfileId = review.ReviewedProfileId
        };
    }
}
