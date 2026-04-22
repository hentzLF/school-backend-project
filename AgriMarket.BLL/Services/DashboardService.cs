using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class DashboardService(
    IRepository<AppUser> appUsers,
    IRepository<ServiceListing> serviceListings,
    IRepository<Booking> bookings,
    IRepository<Payment> payments,
    ILogger<DashboardService> logger) : IDashboardService
{
    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfWeek = now.AddDays(-7);

        var totalUsers = await appUsers.CountAsync(_ => true);
        var newUsersThisMonth = await appUsers.CountAsync(u => u.CreatedAt >= startOfMonth);
        var newUsersThisWeek = await appUsers.CountAsync(u => u.CreatedAt >= startOfWeek);

        var totalListings = await serviceListings.CountAsync(_ => true);
        var activeListings = await serviceListings.CountAsync(l => l.IsActive);
        var inactiveListings = await serviceListings.CountAsync(l => !l.IsActive);

        var totalBookings = await bookings.CountAsync(_ => true);
        var bookingsStatusCounts = await bookings.Query()
            .GroupBy(b => b.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync();

        var bookingsByStatus = Enum.GetValues<BookingStatus>()
            .ToDictionary(s => s, s => bookingsStatusCounts.FirstOrDefault(b => b.Key == s)?.Count ?? 0);

        var totalRevenue = await payments.Query().SumAsync(p => p.Amount);
        var totalPlatformFees = await payments.Query().SumAsync(p => p.PlatformFee);
        var revenueThisMonth = await payments.Query()
            .Where(p => p.CreatedAt >= startOfMonth)
            .SumAsync(p => p.Amount);

        var activeDisputes = await payments.CountAsync(p => p.Status == PaymentStatus.Disputed);
        var resolvedDisputes = await payments.CountAsync(p => p.Status == PaymentStatus.Released || p.Status == PaymentStatus.Refunded);

        var recentBookings = await bookings.Query()
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
