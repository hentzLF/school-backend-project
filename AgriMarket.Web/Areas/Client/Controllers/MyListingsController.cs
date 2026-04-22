using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using AgriMarket.Web.Areas.Client.ViewModels.MyListings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AgriMarket.Web.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Policy = "ProviderOnly")]
public class MyListingsController(IListingService listingService, ICategoryService categoryService, IBookingService bookingService, IUserService userService) : Controller
{
    private async Task<UserProfile?> GetProviderProfileAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return null;
        return await userService.GetProfileByUserIdAsync(userId);
    }

    public async Task<IActionResult> Index()
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listings = await listingService.GetByProviderAsync(profile.Id);

        var viewModel = new MyListingIndexViewModel
        {
            Listings = listings.Select(l => new MyListingIndexItemViewModel
            {
                Id = l.Id,
                Title = l.Title,
                CategoryName = l.ServiceCategory!.Name,
                PricePerHectare = l.PricePerHectare,
                IsActive = l.IsActive
            }).ToList()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        var bookingCount = await bookingService.GetCountByListingAsync(id);

        var viewModel = new MyListingDetailsViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            CategoryName = listing.ServiceCategory!.Name,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            TotalBookingCount = bookingCount
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await categoryService.GetAllAsync();

        var viewModel = new MyListingCreateViewModel
        {
            Categories = categories.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MyListingCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var categories = await categoryService.GetAllAsync();
            model.Categories = categories.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            return View(model);
        }

        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = new ServiceListing
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Description = model.Description,
            ServiceCategoryId = model.ServiceCategoryId,
            PricePerHectare = model.PricePerHectare,
            UserProfileId = profile.Id,
            IsActive = false
        };

        await listingService.CreateAsync(listing);

        return RedirectToAction(nameof(Details), new { id = listing.Id });
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        var categories = await categoryService.GetAllAsync();

        var viewModel = new MyListingEditViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            ServiceCategoryId = listing.ServiceCategoryId,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            Categories = categories.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, MyListingEditViewModel model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            var categories = await categoryService.GetAllAsync();
            model.Categories = categories.OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
            return View(model);
        }

        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        listing.Title = model.Title;
        listing.Description = model.Description;
        listing.ServiceCategoryId = model.ServiceCategoryId;
        listing.PricePerHectare = model.PricePerHectare;
        listing.IsActive = model.IsActive;

        await listingService.UpdateAsync(listing);

        return RedirectToAction(nameof(Details), new { id = listing.Id });
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        var hasActiveBookings = await bookingService.HasActiveBookingsAsync(id);

        ViewBag.HasActiveBookings = hasActiveBookings;

        return View(listing);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        var hasActiveBookings = await bookingService.HasActiveBookingsAsync(id);

        if (hasActiveBookings)
        {
            ViewBag.HasActiveBookings = true;
            return View("Delete", listing);
        }

        await listingService.DeleteAsync(listing.Id);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        await listingService.ToggleActiveAsync(id);

        return RedirectToAction(nameof(Details), new { id = listing.Id });
    }

    public async Task<IActionResult> Availabilities(Guid id)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        var availabilities = (listing.Availabilities ?? new List<Availability>())
            .OrderBy(a => a.StartTime)
            .Select(a => new AvailabilityItemViewModel
            {
                Id = a.Id,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                IsBooked = a.IsBooked
            })
            .ToList();

        var viewModel = new ManageAvailabilitiesViewModel
        {
            ListingId = listing.Id,
            ListingTitle = listing.Title,
            Availabilities = availabilities,
            AddStartTime = DateTime.Today.AddDays(1).AddHours(8),
            AddEndTime = DateTime.Today.AddDays(1).AddHours(17)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAvailability(Guid listingId, ManageAvailabilitiesViewModel model)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(listingId);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        if (model.AddStartTime >= model.AddEndTime)
        {
            ModelState.AddModelError(string.Empty, "Start time must be before end time.");
        }

        if (!ModelState.IsValid)
        {
            // Reload list to re-render
            var listingToReload = await listingService.GetByIdAsync(listingId);
            var availabilities = (listingToReload?.Availabilities ?? new List<Availability>())
                .OrderBy(a => a.StartTime)
                .Select(a => new AvailabilityItemViewModel
                {
                    Id = a.Id,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsBooked = a.IsBooked
                })
                .ToList();
            
            model.ListingId = listing.Id;
            model.ListingTitle = listing.Title;
            model.Availabilities = availabilities;

            return View("Availabilities", model);
        }

        var availability = new Availability
        {
            Id = Guid.NewGuid(),
            ServiceListingId = listingId,
            StartTime = DateTime.SpecifyKind(model.AddStartTime, DateTimeKind.Utc),
            EndTime = DateTime.SpecifyKind(model.AddEndTime, DateTimeKind.Utc),
            IsBooked = false
        };

        await listingService.AddAvailabilityAsync(availability);

        return RedirectToAction(nameof(Availabilities), new { id = listingId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAvailability(Guid id)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var availability = await listingService.GetAvailabilityByIdAsync(id);

        if (availability == null || availability.ServiceListing!.UserProfileId != profile.Id)
        {
            return NotFound();
        }

        if (availability.IsBooked)
        {
            TempData["ErrorMessage"] = "Cannot delete a booked availability slot.";
            return RedirectToAction(nameof(Availabilities), new { id = availability.ServiceListingId });
        }

        await listingService.DeleteAvailabilityAsync(id);

        return RedirectToAction(nameof(Availabilities), new { id = availability.ServiceListingId });
    }

    public async Task<IActionResult> Bookings(Guid id)
    {
        var profile = await GetProviderProfileAsync();
        if (profile == null) return NotFound();

        var listing = await listingService.GetByIdAsync(id);

        if (listing == null || listing.UserProfileId != profile.Id) return NotFound();

        var bookings = await bookingService.GetByListingAsync(id);

        var viewModel = new BookingsForListingViewModel
        {
            ListingId = listing.Id,
            ListingTitle = listing.Title,
            Bookings = bookings.Select(b => new BookingsForListingItemViewModel
            {
                Id = b.Id,
                ClientName = b.ClientProfile!.FirstName + " " + b.ClientProfile.LastName,
                Status = b.Status.ToString(),
                AreaInHectares = b.AreaInHectares,
                TotalPrice = b.TotalPrice,
                CreatedAt = b.CreatedAt
            }).ToList()
        };

        return View(viewModel);
    }
}
