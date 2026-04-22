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

        var totalUsers = await _db.AppUsers.CountAsync();
        var newUsersThisMonth = await _db.AppUsers.CountAsync(u => u.CreatedAt >= startOfMonth);
        var newUsersThisWeek = await _db.AppUsers.CountAsync(u => u.CreatedAt >= startOfWeek);

        var totalListings = await _db.ServiceListings.CountAsync();
        var activeListings = await _db.ServiceListings.CountAsync(l => l.IsActive);
        var inactiveListings = await _db.ServiceListings.CountAsync(l => !l.IsActive);

        var totalBookings = await _db.Bookings.CountAsync();
        var bookingsStatusCounts = await _db.Bookings
            .GroupBy(b => b.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();
        
        var bookingsByStatus = Enum.GetValues<BookingStatus>()
            .ToDictionary(s => s, s => bookingsStatusCounts.FirstOrDefault(b => b.Key == s)?.Count ?? 0);

        var totalRevenue = await _db.Payments.SumAsync(p => p.Amount);
        var totalPlatformFees = await _db.Payments.SumAsync(p => p.PlatformFee);
        var revenueThisMonth = await _db.Payments
            .Where(p => p.CreatedAt >= startOfMonth)
            .SumAsync(p => p.Amount);

        var activeDisputes = await _db.Payments.CountAsync(p => p.Status == PaymentStatus.Disputed);
        var resolvedDisputes = await _db.Payments.CountAsync(p => p.Status == PaymentStatus.Released || p.Status == PaymentStatus.Refunded);

        var recentBookings = await _db.Bookings
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .OrderByDescending(b => b.CreatedAt)
            .Take(10)
            .ToListAsync();

        return new DashboardStats
        {
            TotalUsers = totalUsers,
            NewUsersThisMonth = newUsersThisMonth,
            NewUsersThisWeek = newUsersThisWeek,

            TotalListings = totalListings,
            ActiveListings = activeListings,
            InactiveListings = inactiveListings,

            TotalBookings = totalBookings,
            BookingsByStatus = bookingsByStatus,

            TotalRevenue = totalRevenue,
            TotalPlatformFees = totalPlatformFees,
            RevenueThisMonth = revenueThisMonth,

            ActiveDisputes = activeDisputes,
            ResolvedDisputes = resolvedDisputes,

            RecentBookings = recentBookings
        };
    }
}
