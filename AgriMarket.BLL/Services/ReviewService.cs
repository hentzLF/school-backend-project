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

    public async Task<IEnumerable<Review>> GetByBookingAsync(Guid bookingId)
    {
        return await _db.Reviews
            .Where(r => r.BookingId == bookingId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review> CreateAsync(Review review)
    {
        var booking = await _db.Bookings.FindAsync(review.BookingId);
        if (booking == null)
            throw new ArgumentException("Booking not found");

        if (booking.Status != BookingStatus.ClientConfirmed && booking.Status != BookingStatus.ProviderCompleted)
        {
            throw new InvalidOperationException("Cannot review a booking that is not completed.");
        }

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return review;
    }
}
