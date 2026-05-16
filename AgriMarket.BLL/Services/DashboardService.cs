using AgriMarket.BLL.Contracts;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class DashboardService(
    IRepository<AppUser> appUsers,
    IRepository<ServiceListing> serviceListings,
    IRepository<Booking> bookings,
    IRepository<Payment> payments,
    IQueryMaterializer mat,
    ILogger<DashboardService> logger) : IDashboardService
{
    private const int RecentBookingsCount = 10;
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
        var bookingsStatusCounts = await mat.ToListAsync(
            bookings.Query()
                .GroupBy(b => b.Status)
                .Select(g => new { g.Key, Count = g.Count() }));

        var bookingsByStatus = Enum.GetValues<BookingStatus>()
            .ToDictionary(s => s, s => bookingsStatusCounts.FirstOrDefault(b => b.Key == s)?.Count ?? 0);

        var totalRevenue = await mat.SumAsync(payments.Query(), p => (decimal?)p.Amount);
        var totalPlatformFees = await mat.SumAsync(payments.Query(), p => (decimal?)p.PlatformFee);
        var revenueThisMonth = await mat.SumAsync(
            payments.Query().Where(p => p.CreatedAt >= startOfMonth),
            p => (decimal?)p.Amount);

        var activeDisputes = await payments.CountAsync(p => p.Status == PaymentStatus.Disputed);
        var resolvedDisputes = await payments.CountAsync(p => p.Status == PaymentStatus.Released || p.Status == PaymentStatus.Refunded);

        var recentBookings = await mat.ToListAsync(
            bookings.Query()
                .OrderByDescending(b => b.CreatedAt)
                .Take(RecentBookingsCount)
                .Select(b => new RecentBookingDto
                {
                    Id = b.Id,
                    Status = (int)b.Status,
                    TotalPrice = b.TotalPrice,
                    AreaInHectares = b.AreaInHectares,
                    CreatedAt = b.CreatedAt,
                    ClientProfile = b.ClientProfile != null
                        ? new ClientProfileDto
                        {
                            FirstName = b.ClientProfile.FirstName,
                            LastName = b.ClientProfile.LastName,
                        }
                        : null,
                    ServiceListing = b.ServiceListing != null
                        ? new ServiceListingDto { Title = b.ServiceListing.Title }
                        : null,
                }));

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
