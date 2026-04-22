using AgriMarket.Api.Dtos.ServiceListings;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Api.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController : ControllerBase
{
    private readonly IListingService _listingService;

    public ListingsController(IListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var allItems = await _listingService.GetAllAsync();
        var totalCount = allItems.Count();
        
        var items = allItems
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(sl => new ServiceListingResponse
            {
                Id = sl.Id,
                Title = sl.Title,
                Description = sl.Description,
                PricePerHectare = sl.PricePerHectare,
                IsActive = sl.IsActive,
                UserProfileId = sl.UserProfileId,
                ServiceCategoryId = sl.ServiceCategoryId,
                LocationId = sl.LocationId
            });

        return Ok(new { items, page, pageSize, totalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sl = await _listingService.GetByIdAsync(id);

        if (sl is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"ServiceListing {id} not found.");

        return Ok(new ServiceListingResponse
        {
            Id = sl.Id,
            Title = sl.Title,
            Description = sl.Description,
            PricePerHectare = sl.PricePerHectare,
            IsActive = sl.IsActive,
            UserProfileId = sl.UserProfileId,
            ServiceCategoryId = sl.ServiceCategoryId,
            LocationId = sl.LocationId
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListingRequest req)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var listing = new ServiceListing
        {
            Id = Guid.NewGuid(),
            Title = req.Title,
            Description = req.Description,
            PricePerHectare = req.PricePerHectare,
            IsActive = true,
            UserProfileId = callerProfileId,
            ServiceCategoryId = req.ServiceCategoryId,
            LocationId = req.LocationId
        };

        await _listingService.CreateAsync(listing);

        var response = new ServiceListingResponse
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            UserProfileId = listing.UserProfileId,
            ServiceCategoryId = listing.ServiceCategoryId,
            LocationId = listing.LocationId
        };

        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, response);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateListingRequest req)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var listing = await _listingService.GetByIdAsync(id);
        if (listing is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"ServiceListing {id} not found.");

        if (listing.UserProfileId != callerProfileId)
            return Problem(statusCode: 403, title: "Forbidden", detail: "You do not own this listing.");

        listing.Title = req.Title;
        listing.Description = req.Description;
        listing.PricePerHectare = req.PricePerHectare;
        listing.IsActive = req.IsActive;
        listing.ServiceCategoryId = req.ServiceCategoryId;
        listing.LocationId = req.LocationId;

        await _listingService.UpdateAsync(listing);

        return Ok(new ServiceListingResponse
        {
            Id = listing.Id,
            Title = listing.Title,
            Description = listing.Description,
            PricePerHectare = listing.PricePerHectare,
            IsActive = listing.IsActive,
            UserProfileId = listing.UserProfileId,
            ServiceCategoryId = listing.ServiceCategoryId,
            LocationId = listing.LocationId
        });
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var listing = await _listingService.GetByIdAsync(id);
        if (listing is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"ServiceListing {id} not found.");

        if (listing.UserProfileId != callerProfileId)
            return Problem(statusCode: 403, title: "Forbidden", detail: "You do not own this listing.");

        await _listingService.DeleteAsync(id);

        return NoContent();
    }
}
