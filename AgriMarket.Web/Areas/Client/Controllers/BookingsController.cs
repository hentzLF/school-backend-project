using AgriMarket.BLL.Services;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Client.ViewModels.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Policy = "ClientOnly")]
public class BookingsController(IBookingService bookingService, IUserService userService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var clientProfile = await GetClientProfileAsync();
        if (clientProfile == null) return Unauthorized();

        var bookings = await bookingService.GetByClientAsync(clientProfile.Id);

        var vm = new BookingIndexViewModel
        {
            Bookings = bookings.Select(b => new BookingIndexItemViewModel
            {
                Id = b.Id,
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
        var clientProfile = await GetClientProfileAsync();
        if (clientProfile == null) return Unauthorized();

        var booking = await bookingService.GetByIdAsync(id);

        if (booking == null) return NotFound();

        if (booking.ClientProfileId != clientProfile.Id)
            return RedirectToAction("AccessDenied", "Account");

        var vm = new BookingDetailsViewModel
        {
            Id = booking.Id,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            AreaInHectares = booking.AreaInHectares,
            CreatedAt = booking.CreatedAt,
            Notes = booking.Notes,
            ListingTitle = booking.ServiceListing?.Title ?? "Unknown",
            ListingId = booking.ServiceListingId,
            AvailabilityStart = booking.Availability?.StartTime ?? default,
            AvailabilityEnd = booking.Availability?.EndTime ?? default
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmCompletion(Guid id)
    {
        var clientProfile = await GetClientProfileAsync();
        if (clientProfile == null) return Unauthorized();

        var booking = await bookingService.GetByIdAsync(id);

        if (booking == null) return NotFound();

        if (booking.ClientProfileId != clientProfile.Id)
            return RedirectToAction("AccessDenied", "Account");

        if (booking.Status != BookingStatus.ProviderCompleted)
            return RedirectToAction(nameof(Details), new { id });

        await bookingService.UpdateStatusAsync(id, BookingStatus.ClientConfirmed);

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<Domain.Entities.UserProfile?> GetClientProfileAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return null;
        return await userService.GetProfileByUserIdAsync(userId);
    }
}
