using AgriMarket.Api.Mappers;
using AgriMarket.BLL;
using AgriMarket.BLL.Dtos;
using AgriMarket.BLL.Dtos.Listings;
using AgriMarket.BLL.Dtos.Bookings;
using AgriMarket.BLL.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers;

public sealed record CreateAvailabilityRequest(DateTime StartTime, DateTime EndTime);

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/listings")]
public class ListingsController(IListingService listingService, IBookingService bookingService) : ApiControllerBase
{
    private readonly IListingService _listingService = listingService;
    private readonly IBookingService _bookingService = bookingService;

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ListingSummaryDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var allItems = await _listingService.GetAllAsync();
        var totalCount = allItems.Count();
        var items = allItems.Skip((page - 1) * pageSize).Take(pageSize);

        return Ok(new PaginatedResponse<ListingSummaryDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
    }

    [Authorize]
    [HttpGet("mine")]
    [ProducesResponseType(typeof(IEnumerable<ListingSummaryDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetMine()
    {
        if (!TryGetProfileId(out var profileId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid profile identity.");

        var listings = await _listingService.GetByProviderAsync(profileId);
        return Ok(listings);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ListingDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var listing = await _listingService.GetByIdAsync(id);

        if (listing is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"ServiceListing {id} not found.");

        return Ok(listing);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ListingDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Create([FromBody] CreateListingDto req)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            var listing = await _listingService.CreateAsync(userId, req);
            return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 422, title: "Unprocessable Entity", detail: ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ListingDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateListingDto req)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            var listing = await _listingService.UpdateAsync(userId, req.WithRouteId(id));
            return Ok(listing);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 403, title: "Forbidden", detail: ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            await _listingService.DeleteAsync(userId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 422, title: "Unprocessable Entity", detail: ex.Message);
        }
    }

    [HttpGet("{listingId:guid}/availabilities")]
    [ProducesResponseType(typeof(IReadOnlyList<AvailabilityDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAvailabilities(Guid listingId)
    {
        var listing = await _listingService.GetByIdAsync(listingId);
        if (listing is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"ServiceListing {listingId} not found.");

        return Ok(listing.Availabilities);
    }

    [Authorize]
    [HttpPost("{listingId:guid}/availabilities")]
    [ProducesResponseType(typeof(AvailabilityDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddAvailability(Guid listingId, [FromBody] CreateAvailabilityRequest req)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            var dto = new CreateAvailabilityDto
            {
                ListingId = listingId,
                StartTime = req.StartTime,
                EndTime = req.EndTime
            };
            var availability = await _listingService.AddAvailabilityAsync(userId, dto);
            return Created($"api/v1/listings/{listingId}/availabilities/{availability.Id}", availability);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: ex.Message.Contains("own") ? 403 : 400,
                title: ex.Message.Contains("own") ? "Forbidden" : "Bad Request",
                detail: ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{listingId:guid}/availabilities/{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAvailability(Guid listingId, Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            await _listingService.DeleteAvailabilityAsync(userId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: ex.Message.Contains("own") ? 403 : 400,
                title: ex.Message.Contains("own") ? "Forbidden" : "Bad Request",
                detail: ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(ListingDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            await _listingService.ToggleActiveAsync(userId, id);
            var listing = await _listingService.GetByIdAsync(id);
            return Ok(listing);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 403, title: "Forbidden", detail: ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{listingId:guid}/bookings")]
    [ProducesResponseType(typeof(IEnumerable<BookingSummaryDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetListingBookings(Guid listingId)
    {
        if (!TryGetProfileId(out var profileId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid profile identity.");

        var listing = await _listingService.GetByIdAsync(listingId);
        if (listing is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"ServiceListing {listingId} not found.");

        if (listing.UserProfileId != profileId)
            return Problem(statusCode: 403, title: "Forbidden", detail: "You do not own this listing.");

        var bookings = await _bookingService.GetByListingAsync(listingId);
        return Ok(bookings);
    }
}
