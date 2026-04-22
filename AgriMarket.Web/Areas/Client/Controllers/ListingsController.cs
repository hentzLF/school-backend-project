using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Client.ViewModels.Bookings;
using AgriMarket.Web.Areas.Client.ViewModels.Listings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
public class ListingsController(IListingService listingService, IBookingService bookingService, IUserService userService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var listings = await listingService.GetActiveListingsAsync();

        var vm = new ListingIndexViewModel
        {
            Listings = listings.Select(l => new ListingIndexItemViewModel
            {
                Id = l.Id,
                Title = l.Title,
                CategoryName = l.ServiceCategory?.Name ?? "Unknown",
                ProviderName = l.UserProfile != null
                    ? $"{l.UserProfile.FirstName} {l.UserProfile.LastName}"
                    : "Unknown",
                PricePerHectare = l.PricePerHectare
            })
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || !listing.IsActive) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isOwnListing = userId != null && listing.UserProfile?.AppUserId.ToString() == userId;

        var vm = new ListingDetailsViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            ProviderName = listing.UserProfile != null
                ? $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}"
                : "Unknown",
            IsOwnListing = isOwnListing,
            Availabilities = listing.Availabilities?
                .Where(a => !a.IsBooked)
                .OrderBy(a => a.StartTime)
                .Select(a => new AvailabilityOptionViewModel
                {
                    Id = a.Id,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                }) ?? []
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ClientOnly")]
    public async Task<IActionResult> Book(CreateBookingViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new { id = model.ServiceListingId });

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var clientProfile = await userService.GetProfileByUserIdAsync(userId);
        if (clientProfile == null) return Unauthorized();

        var listing = await listingService.GetByIdAsync(model.ServiceListingId);
        if (listing == null || !listing.IsActive) return NotFound();

        if (listing.UserProfile?.AppUserId == userId)
            return RedirectToAction(nameof(Details), new { id = model.ServiceListingId });

        var availability = await listingService.GetAvailabilityByIdAsync(model.AvailabilityId);

        if (availability == null || availability.ServiceListingId != model.ServiceListingId || availability.IsBooked)
        {
            ModelState.AddModelError(string.Empty, "The selected availability is no longer available.");
            return RedirectToAction(nameof(Details), new { id = model.ServiceListingId });
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            ServiceListingId = model.ServiceListingId,
            ClientProfileId = clientProfile.Id,
            AvailabilityId = model.AvailabilityId,
            AreaInHectares = model.AreaInHectares,
            TotalPrice = (decimal)model.AreaInHectares * listing.PricePerHectare,
            Notes = model.Notes,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        availability.IsBooked = true;
        await listingService.UpdateAvailabilityAsync(availability);
        
        await bookingService.CreateAsync(booking);

        return RedirectToAction("Details", "Bookings", new { area = "Client", id = booking.Id });
    }
}
