using AgriMarket.BLL;
using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace AgriMarket.Tests.Services;

public class ReviewServiceTests
{
    private readonly Mock<IRepository<Review>> _reviews = new();
    private readonly Mock<IRepository<UserProfile>> _userProfiles = new();
    private readonly Mock<IBookingRepository> _bookings = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IQueryMaterializer> _mat = new();
    private readonly ReviewService _sut;

    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ReviewerProfileId = Guid.NewGuid();
    private static readonly Guid ReviewedProfileId = Guid.NewGuid();
    private static readonly Guid BookingId = Guid.NewGuid();
    private static readonly Guid ListingId = Guid.NewGuid();

    public ReviewServiceTests()
    {
        _sut = new ReviewService(
            _reviews.Object,
            _userProfiles.Object,
            _bookings.Object,
            _uow.Object,
            _mat.Object,
            NullLogger<ReviewService>.Instance);
    }

    private void SetupReviewerProfileExists()
    {
        _userProfiles
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserProfile, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { Id = ReviewerProfileId, AppUserId = UserId });
    }

    private Booking CreateCompletedBooking(Guid? clientProfileId = null)
    {
        return new Booking
        {
            Id = BookingId,
            ClientProfileId = clientProfileId ?? ReviewerProfileId,
            ServiceListingId = ListingId,
            Status = BookingStatus.ClientConfirmed,
            ServiceListing = new ServiceListing
            {
                Id = ListingId,
                UserProfileId = ReviewedProfileId,
                Title = "Test Service",
                PricePerHectare = 50m
            }
        };
    }

    private void SetupBookingExists(Booking booking)
    {
        _bookings
            .Setup(r => r.GetByIdWithDetailsAsync(BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);
    }

    private static CreateReviewDto ValidDto() => new()
    {
        BookingId = BookingId,
        Rating = 5,
        Comment = "Excellent service"
    };

    [Fact]
    public async Task CreateAsync_ValidClientReview_ReturnsReviewDto()
    {
        SetupReviewerProfileExists();
        SetupBookingExists(CreateCompletedBooking());

        var result = await _sut.CreateAsync(UserId, ValidDto());

        Assert.Equal(5, result.Rating);
        Assert.Equal("Excellent service", result.Comment);
        Assert.Equal(BookingId, result.BookingId);
        Assert.Equal(ReviewerProfileId, result.ReviewerProfileId);
        Assert.Equal(ReviewedProfileId, result.ReviewedProfileId);
        _reviews.Verify(r => r.Add(It.Is<Review>(rev =>
            rev.Rating == 5 &&
            rev.ReviewedProfileId == ReviewedProfileId)), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ProviderCompletedStatus_Succeeds()
    {
        SetupReviewerProfileExists();
        var booking = CreateCompletedBooking();
        booking.Status = BookingStatus.ProviderCompleted;
        SetupBookingExists(booking);

        var result = await _sut.CreateAsync(UserId, ValidDto());

        Assert.Equal(5, result.Rating);
    }

    [Fact]
    public async Task CreateAsync_UserProfileNotFound_ThrowsBusinessRuleException()
    {
        _userProfiles
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserProfile, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateAsync(UserId, ValidDto()));

        Assert.Equal("User profile not found.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_BookingNotFound_ThrowsKeyNotFoundException()
    {
        SetupReviewerProfileExists();
        _bookings
            .Setup(r => r.GetByIdWithDetailsAsync(BookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.CreateAsync(UserId, ValidDto()));

        Assert.Equal("Booking not found.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_NonClientUser_ThrowsBusinessRuleException()
    {
        SetupReviewerProfileExists();
        var unrelatedClientId = Guid.NewGuid();
        var booking = CreateCompletedBooking(clientProfileId: unrelatedClientId);
        SetupBookingExists(booking);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateAsync(UserId, ValidDto()));

        Assert.Equal("Only the client can review a booking.", ex.Message);
    }

    [Theory]
    [InlineData(BookingStatus.Pending)]
    [InlineData(BookingStatus.Confirmed)]
    [InlineData(BookingStatus.InProgress)]
    [InlineData(BookingStatus.Cancelled)]
    [InlineData(BookingStatus.Disputed)]
    [InlineData(BookingStatus.AwaitingPayment)]
    public async Task CreateAsync_BookingNotCompleted_ThrowsBusinessRuleException(BookingStatus status)
    {
        SetupReviewerProfileExists();
        var booking = CreateCompletedBooking();
        booking.Status = status;
        SetupBookingExists(booking);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateAsync(UserId, ValidDto()));

        Assert.Equal("Cannot review a booking that is not completed.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_BookingWithoutServiceListing_ThrowsBusinessRuleException()
    {
        SetupReviewerProfileExists();
        var booking = new Booking
        {
            Id = BookingId,
            ClientProfileId = ReviewerProfileId,
            Status = BookingStatus.ClientConfirmed,
            ServiceListing = null
        };
        SetupBookingExists(booking);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateAsync(UserId, ValidDto()));

        Assert.Equal("Cannot determine service provider for this booking.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_ProviderReviewsOwnBooking_ThrowsBusinessRuleException()
    {
        var providerProfileId = ReviewedProfileId;
        _userProfiles
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<UserProfile, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserProfile { Id = providerProfileId, AppUserId = UserId });

        var booking = new Booking
        {
            Id = BookingId,
            ClientProfileId = Guid.NewGuid(),
            ServiceListingId = ListingId,
            Status = BookingStatus.ClientConfirmed,
            ServiceListing = new ServiceListing
            {
                Id = ListingId,
                UserProfileId = providerProfileId,
                Title = "Test",
                PricePerHectare = 50m
            }
        };
        SetupBookingExists(booking);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateAsync(UserId, ValidDto()));

        Assert.Equal("Only the client can review a booking.", ex.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ReviewExists_ReturnsDto()
    {
        var reviewId = Guid.NewGuid();
        _reviews
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Review, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Review
            {
                Id = reviewId,
                Rating = 4,
                Comment = "Good",
                CreatedAt = DateTime.UtcNow,
                BookingId = BookingId,
                ReviewerProfileId = ReviewerProfileId
            });

        var result = await _sut.GetByIdAsync(reviewId);

        Assert.NotNull(result);
        Assert.Equal(reviewId, result!.Id);
        Assert.Equal(4, result.Rating);
        Assert.Equal("Good", result.Comment);
    }

    [Fact]
    public async Task GetByIdAsync_ReviewNotFound_ReturnsNull()
    {
        _reviews
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Review, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByBookingAsync_ReviewsExist_ReturnsOrderedByCreatedAtDesc()
    {
        var older = new Review
        {
            Id = Guid.NewGuid(), Rating = 3, CreatedAt = DateTime.UtcNow.AddDays(-1),
            BookingId = BookingId, ReviewerProfileId = ReviewerProfileId
        };
        var newer = new Review
        {
            Id = Guid.NewGuid(), Rating = 5, CreatedAt = DateTime.UtcNow,
            BookingId = BookingId, ReviewerProfileId = ReviewerProfileId
        };
        _reviews
            .Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<Review, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Review> { older, newer });

        var result = (await _sut.GetByBookingAsync(BookingId)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(newer.Id, result[0].Id);
        Assert.Equal(older.Id, result[1].Id);
    }

    [Fact]
    public async Task GetByBookingAsync_NoReviews_ReturnsEmpty()
    {
        _reviews
            .Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<Review, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Review>());

        var result = await _sut.GetByBookingAsync(BookingId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_MinimumRating_Succeeds()
    {
        SetupReviewerProfileExists();
        SetupBookingExists(CreateCompletedBooking());
        var dto = new CreateReviewDto { BookingId = BookingId, Rating = 1 };

        var result = await _sut.CreateAsync(UserId, dto);

        Assert.Equal(1, result.Rating);
        Assert.Null(result.Comment);
    }

    [Fact]
    public async Task CreateAsync_NullComment_Succeeds()
    {
        SetupReviewerProfileExists();
        SetupBookingExists(CreateCompletedBooking());
        var dto = new CreateReviewDto { BookingId = BookingId, Rating = 3, Comment = null };

        var result = await _sut.CreateAsync(UserId, dto);

        Assert.Null(result.Comment);
    }

    [Fact]
    public async Task CreateAsync_DuplicateReview_ThrowsBusinessRuleException()
    {
        SetupReviewerProfileExists();
        SetupBookingExists(CreateCompletedBooking());
        _reviews.Setup(r => r.AnyAsync(
            It.IsAny<Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.CreateAsync(UserId, ValidDto()));

        Assert.Equal("A review already exists for this booking.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_OwnerUpdatesReview_ReturnsUpdatedDto()
    {
        SetupReviewerProfileExists();
        var reviewId = Guid.NewGuid();
        _reviews.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Review
            {
                Id = reviewId, Rating = 3, Comment = "Old",
                BookingId = BookingId, ReviewerProfileId = ReviewerProfileId,
                ReviewedProfileId = ReviewedProfileId, CreatedAt = DateTime.UtcNow
            });

        var result = await _sut.UpdateAsync(UserId, new UpdateReviewDto { Id = reviewId, Rating = 5, Comment = "Updated" });

        Assert.Equal(5, result.Rating);
        Assert.Equal("Updated", result.Comment);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReviewNotFound_ThrowsKeyNotFoundException()
    {
        SetupReviewerProfileExists();
        _reviews.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.UpdateAsync(UserId, new UpdateReviewDto { Id = Guid.NewGuid(), Rating = 4 }));
    }

    [Fact]
    public async Task UpdateAsync_NotOwner_ThrowsBusinessRuleException()
    {
        SetupReviewerProfileExists();
        var reviewId = Guid.NewGuid();
        _reviews.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Review
            {
                Id = reviewId, Rating = 3, ReviewerProfileId = Guid.NewGuid(),
                BookingId = BookingId, ReviewedProfileId = ReviewedProfileId, CreatedAt = DateTime.UtcNow
            });

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.UpdateAsync(UserId, new UpdateReviewDto { Id = reviewId, Rating = 4 }));

        Assert.Equal("You do not own this review.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_NoProfile_ThrowsBusinessRuleException()
    {
        _userProfiles.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<UserProfile, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.UpdateAsync(UserId, new UpdateReviewDto { Id = Guid.NewGuid(), Rating = 4 }));

        Assert.Equal("User profile not found.", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_OwnerDeletesReview_Succeeds()
    {
        SetupReviewerProfileExists();
        var reviewId = Guid.NewGuid();
        var review = new Review
        {
            Id = reviewId, Rating = 3, ReviewerProfileId = ReviewerProfileId,
            BookingId = BookingId, ReviewedProfileId = ReviewedProfileId, CreatedAt = DateTime.UtcNow
        };
        _reviews.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(review);

        await _sut.DeleteAsync(UserId, reviewId);

        _reviews.Verify(r => r.Remove(review), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReviewNotFound_ThrowsKeyNotFoundException()
    {
        SetupReviewerProfileExists();
        _reviews.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((Review?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _sut.DeleteAsync(UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteAsync_NotOwner_ThrowsBusinessRuleException()
    {
        SetupReviewerProfileExists();
        var reviewId = Guid.NewGuid();
        _reviews.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<Review, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Review
            {
                Id = reviewId, Rating = 3, ReviewerProfileId = Guid.NewGuid(),
                BookingId = BookingId, ReviewedProfileId = ReviewedProfileId, CreatedAt = DateTime.UtcNow
            });

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.DeleteAsync(UserId, reviewId));

        Assert.Equal("You do not own this review.", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_NoProfile_ThrowsBusinessRuleException()
    {
        _userProfiles.Setup(r => r.FirstOrDefaultAsync(
            It.IsAny<Expression<Func<UserProfile, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _sut.DeleteAsync(UserId, Guid.NewGuid()));

        Assert.Equal("User profile not found.", ex.Message);
    }

    [Fact]
    public async Task GetByProfileAsync_ReturnsFilteredPaginatedResults()
    {
        var reviews = new List<Review>
        {
            new() { Id = Guid.NewGuid(), Rating = 4, CreatedAt = DateTime.UtcNow,
                BookingId = BookingId, ReviewerProfileId = ReviewerProfileId, ReviewedProfileId = ReviewedProfileId }
        };
        _mat.Setup(m => m.CountAsync(It.IsAny<IQueryable<Review>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mat.Setup(m => m.ToListAsync(It.IsAny<IQueryable<Review>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reviews);

        var (items, totalCount) = await _sut.GetByProfileAsync(ReviewedProfileId, 1, 10);
        var list = items.ToList();

        Assert.Equal(1, totalCount);
        Assert.Single(list);
        Assert.Equal(4, list[0].Rating);
    }

    [Fact]
    public async Task GetRatingStatsForProfileAsync_WithReviews_ReturnsStats()
    {
        _mat.Setup(m => m.CountAsync(It.IsAny<IQueryable<Review>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);
        _mat.Setup(m => m.SumAsync(
            It.IsAny<IQueryable<Review>>(),
            It.IsAny<Expression<Func<Review, decimal?>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(12m);

        var stats = await _sut.GetRatingStatsForProfileAsync(ReviewedProfileId);

        Assert.Equal(4.0, stats.AverageRating);
        Assert.Equal(3, stats.ReviewCount);
    }

    [Fact]
    public async Task GetRatingStatsForProfileAsync_NoReviews_ReturnsZero()
    {
        _mat.Setup(m => m.CountAsync(It.IsAny<IQueryable<Review>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var stats = await _sut.GetRatingStatsForProfileAsync(ReviewedProfileId);

        Assert.Equal(0, stats.AverageRating);
        Assert.Equal(0, stats.ReviewCount);
    }

    [Fact]
    public async Task GetRatingStatsForListingAsync_WithReviews_ReturnsStats()
    {
        _mat.Setup(m => m.CountAsync(It.IsAny<IQueryable<Review>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        _mat.Setup(m => m.SumAsync(
            It.IsAny<IQueryable<Review>>(),
            It.IsAny<Expression<Func<Review, decimal?>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(9m);

        var stats = await _sut.GetRatingStatsForListingAsync(ListingId);

        Assert.Equal(4.5, stats.AverageRating);
        Assert.Equal(2, stats.ReviewCount);
    }

    [Fact]
    public async Task GetRatingStatsForListingAsync_NoReviews_ReturnsZero()
    {
        _mat.Setup(m => m.CountAsync(It.IsAny<IQueryable<Review>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var stats = await _sut.GetRatingStatsForListingAsync(ListingId);

        Assert.Equal(0, stats.AverageRating);
        Assert.Equal(0, stats.ReviewCount);
    }
}
