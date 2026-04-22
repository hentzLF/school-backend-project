using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;

namespace AgriMarket.BLL.Services;

public class DashboardStats
{
    public int TotalUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int TotalListings { get; set; }
    public int ActiveListings { get; set; }
    public int InactiveListings { get; set; }
    public int TotalBookings { get; set; }
    public Dictionary<BookingStatus, int>? BookingsByStatus { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalPlatformFees { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int ActiveDisputes { get; set; }
    public int ResolvedDisputes { get; set; }
    public IEnumerable<Booking>? RecentBookings { get; set; }
}

public interface IDashboardService
{
    Task<DashboardStats> GetDashboardStatsAsync();
}
