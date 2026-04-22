using AgriMarket.BLL.Services;
using AgriMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgriMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class ListingsController(IListingService listingService, ICategoryService categoryService, IBookingService bookingService) : Controller
{
    public async Task<IActionResult> Index(bool? active)
    {
        var listings = active.HasValue && active.Value 
            ? await listingService.GetActiveListingsAsync()
            : await listingService.GetAllAsync();

        if (active.HasValue && !active.Value)
            listings = listings.Where(l => !l.IsActive).ToList();

        listings = listings.OrderBy(l => l.Title).ToList();

        var vm = new ListingListViewModel
        {
            TotalCount = listings.Count(),
            FilterActive = active,
            Listings = listings.Select(l => new ListingListItemViewModel
            {
                Id = l.Id,
                Title = l.Title,
                ProviderName = l.UserProfile != null
                    ? $"{l.UserProfile.FirstName} {l.UserProfile.LastName}"
                    : "Unknown",
                CategoryName = l.ServiceCategory?.Name ?? "Unknown",
                PricePerHectare = l.PricePerHectare,
                IsActive = l.IsActive
            })
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var listing = await listingService.GetByIdAsync(id);

        if (listing == null) return NotFound();

        var bookingsCount = await bookingService.GetCountByListingAsync(id);

        var vm = new ListingDetailViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            ProviderName = listing.UserProfile != null
                ? $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}"
                : "Unknown",
            ProviderProfileId = listing.UserProfileId,
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            CategoryId = listing.ServiceCategoryId,
            BookingsCount = bookingsCount,
            Equipments = listing.Equipments?.Select(e => new ListingEquipmentViewModel
            {
                Name = e.Name,
                Model = e.Model,
                ManufactureYear = e.ManufactureYear
            }) ?? [],
            Availabilities = listing.Availabilities == null ? [] : listing.Availabilities.Select(a => new ListingAvailabilityViewModel
            {
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                IsBooked = a.IsBooked
            }).OrderBy(a => a.StartTime)
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var listing = await listingService.GetByIdAsync(id);
        if (listing == null) return NotFound();

        var vm = new ListingEditViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            ServiceCategoryId = listing.ServiceCategoryId,
            Categories = await GetCategorySelectList(listing.ServiceCategoryId)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ListingEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await GetCategorySelectList(vm.ServiceCategoryId);
            return View(vm);
        }

        var listing = await listingService.GetByIdAsync(vm.Id);
        if (listing == null) return NotFound();

        listing.Title = vm.Title;
        listing.Description = vm.Description;
        listing.PricePerHectare = vm.PricePerHectare;
        listing.IsActive = vm.IsActive;
        listing.ServiceCategoryId = vm.ServiceCategoryId;
        await listingService.UpdateAsync(listing);

        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var listing = await listingService.GetByIdAsync(id);

        if (listing == null) return NotFound();

        var vm = new ListingListItemViewModel
        {
            Id = listing.Id,
            Title = listing.Title,
            ProviderName = listing.UserProfile != null
                ? $"{listing.UserProfile.FirstName} {listing.UserProfile.LastName}"
                : "Unknown",
            CategoryName = listing.ServiceCategory?.Name ?? "Unknown",
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive
        };

        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var listing = await listingService.GetByIdAsync(id);
        if (listing == null) return NotFound();

        await listingService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var listing = await listingService.GetByIdAsync(id);
        if (listing == null) return NotFound();

        await listingService.ToggleActiveAsync(id);

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<IEnumerable<SelectListItem>> GetCategorySelectList(Guid selectedId)
    {
        var categories = await categoryService.GetAllAsync();
        return categories.OrderBy(c => c.Name).Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
            Selected = c.Id == selectedId
        });
    }
}
