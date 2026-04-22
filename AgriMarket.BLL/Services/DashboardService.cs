using AgriMarket.DAL;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.BLL.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfWeek = now.AddDays(-7);

        var users = await _db.AppUsers.ToListAsync();
        var listings = await _db.ServiceListings.ToListAsync();
        var bookings = await _db.Bookings
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .ToListAsync();
        var payments = await _db.Payments.ToListAsync();

        return new DashboardStats
        {
            TotalUsers = users.Count,
            NewUsersThisMonth = users.Count(u => u.CreatedAt >= startOfMonth),
            NewUsersThisWeek = users.Count(u => u.CreatedAt >= startOfWeek),

            TotalListings = listings.Count,
            ActiveListings = listings.Count(l => l.IsActive),
            InactiveListings = listings.Count(l => !l.IsActive),

            TotalBookings = bookings.Count,
            BookingsByStatus = Enum.GetValues<BookingStatus>()
                .ToDictionary(s => s, s => bookings.Count(b => b.Status == s)),

            TotalRevenue = payments.Sum(p => p.Amount),
            TotalPlatformFees = payments.Sum(p => p.PlatformFee),
            RevenueThisMonth = payments
                .Where(p => p.CreatedAt >= startOfMonth)
                .Sum(p => p.Amount),

            ActiveDisputes = payments.Count(p => p.Status == PaymentStatus.Disputed),
            ResolvedDisputes = payments.Count(p =>
                p.Status == PaymentStatus.Released || p.Status == PaymentStatus.Refunded),

            RecentBookings = bookings
                .OrderByDescending(b => b.CreatedAt)
                .Take(10)
        };
    }
}
