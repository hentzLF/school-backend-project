using AgriMarket.DAL;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class BookingsController : Controller
{
    private readonly AppDbContext _db;

    public BookingsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(BookingStatus? status)
    {
        var query = _db.Bookings
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        var bookings = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();

        var vm = new BookingListViewModel
        {
            TotalCount = bookings.Count,
            FilterStatus = status,
            Bookings = bookings.Select(b => new BookingListItemViewModel
            {
                Id = b.Id,
                ClientName = b.ClientProfile != null
                    ? $"{b.ClientProfile.FirstName} {b.ClientProfile.LastName}"
                    : "Unknown",
                ListingTitle = b.ServiceListing?.Title ?? "Unknown",
                Status = b.Status,
                TotalPrice = b.TotalPrice,
                AreaInHectares = b.AreaInHectares,
                CreatedAt = b.CreatedAt
            })
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var booking = await _db.Bookings
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .Include(b => b.Availability)
            .Include(b => b.Payment)
            .Include(b => b.Review)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();

        var vm = new BookingDetailViewModel
        {
            Id = booking.Id,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            AreaInHectares = booking.AreaInHectares,
            CreatedAt = booking.CreatedAt,
            Notes = booking.Notes,
            ClientName = booking.ClientProfile != null
                ? $"{booking.ClientProfile.FirstName} {booking.ClientProfile.LastName}"
                : "Unknown",
            ClientProfileId = booking.ClientProfileId,
            ListingTitle = booking.ServiceListing?.Title ?? "Unknown",
            ListingId = booking.ServiceListingId,
            AvailabilityStart = booking.Availability?.StartTime ?? default,
            AvailabilityEnd = booking.Availability?.EndTime ?? default,
            PaymentId = booking.Payment?.Id,
            PaymentAmount = booking.Payment?.Amount,
            PlatformFee = booking.Payment?.PlatformFee,
            PaymentStatus = booking.Payment?.Status,
            ReviewRating = booking.Review?.Rating,
            ReviewComment = booking.Review?.Comment,
            ReviewCreatedAt = booking.Review?.CreatedAt
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var booking = await _db.Bookings
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();

        var vm = new BookingEditViewModel
        {
            Id = booking.Id,
            Status = booking.Status,
            ListingTitle = booking.ServiceListing?.Title ?? "Unknown",
            ClientName = booking.ClientProfile != null
                ? $"{booking.ClientProfile.FirstName} {booking.ClientProfile.LastName}"
                : "Unknown",
            Statuses = GetStatusSelectList(booking.Status)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookingEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Statuses = GetStatusSelectList(vm.Status);
            return View(vm);
        }

        var booking = await _db.Bookings.FindAsync(vm.Id);
        if (booking == null) return NotFound();

        booking.Status = vm.Status;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var booking = await _db.Bookings
            .Include(b => b.ClientProfile)
            .Include(b => b.ServiceListing)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();

        var vm = new BookingListItemViewModel
        {
            Id = booking.Id,
            ClientName = booking.ClientProfile != null
                ? $"{booking.ClientProfile.FirstName} {booking.ClientProfile.LastName}"
                : "Unknown",
            ListingTitle = booking.ServiceListing?.Title ?? "Unknown",
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            AreaInHectares = booking.AreaInHectares,
            CreatedAt = booking.CreatedAt
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private static IEnumerable<SelectListItem> GetStatusSelectList(BookingStatus selected)
    {
        return Enum.GetValues<BookingStatus>().Select(s => new SelectListItem
        {
            Value = s.ToString(),
            Text = s.ToString(),
            Selected = s == selected
        });
    }
}
