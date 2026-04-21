using AgriMarket.DAL;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
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

        var vm = new DashboardViewModel
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
                .Select(b => new RecentBookingViewModel
                {
                    Id = b.Id,
                    ClientName = b.ClientProfile != null
                        ? $"{b.ClientProfile.FirstName} {b.ClientProfile.LastName}"
                        : "Unknown",
                    ListingTitle = b.ServiceListing?.Title ?? "Unknown",
                    Status = b.Status,
                    TotalPrice = b.TotalPrice,
                    CreatedAt = b.CreatedAt
                })
        };

        return View(vm);
    }
}
