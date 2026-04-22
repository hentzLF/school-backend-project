using AgriMarket.Api.Dtos.Bookings;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var result = await _bookingService.GetAllForProfileAsync(callerProfileId, page, pageSize);

        var items = result.Items.Select(b => new BookingResponse
        {
            Id = b.Id,
            Status = b.Status,
            TotalPrice = b.TotalPrice,
            AreaInHectares = b.AreaInHectares,
            CreatedAt = b.CreatedAt,
            Notes = b.Notes,
            ServiceListingId = b.ServiceListingId,
            ClientProfileId = b.ClientProfileId,
            AvailabilityId = b.AvailabilityId
        });

        return Ok(new { items, page, pageSize, totalCount = result.TotalCount });
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var booking = await _bookingService.GetByIdAsync(id);

        if (booking is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Booking {id} not found.");

        if (booking.ClientProfileId != callerProfileId
            && booking.ServiceListing?.UserProfileId != callerProfileId)
            return Problem(statusCode: 403, title: "Forbidden", detail: "You are not a party to this booking.");

        return Ok(new BookingResponse
        {
            Id = booking.Id,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            AreaInHectares = booking.AreaInHectares,
            CreatedAt = booking.CreatedAt,
            Notes = booking.Notes,
            ServiceListingId = booking.ServiceListingId,
            ClientProfileId = booking.ClientProfileId,
            AvailabilityId = booking.AvailabilityId
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest req)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = BookingStatus.Pending,
            TotalPrice = 0,
            AreaInHectares = req.AreaInHectares,
            CreatedAt = DateTime.UtcNow,
            Notes = req.Notes,
            ServiceListingId = req.ServiceListingId,
            ClientProfileId = callerProfileId,
            AvailabilityId = req.AvailabilityId
        };

        try
        {
            await _bookingService.CreateAsync(booking);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(statusCode: 400, title: "Bad Request", detail: ex.Message);
        }

        var response = new BookingResponse
        {
            Id = booking.Id,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            AreaInHectares = booking.AreaInHectares,
            CreatedAt = booking.CreatedAt,
            Notes = booking.Notes,
            ServiceListingId = booking.ServiceListingId,
            ClientProfileId = booking.ClientProfileId,
            AvailabilityId = booking.AvailabilityId
        };

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, response);
    }

    [Authorize]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookingStatusRequest req)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var booking = await _bookingService.GetByIdAsync(id);

        if (booking is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Booking {id} not found.");

        try
        {
            await _bookingService.UpdateStatusAsync(id, req.Status, callerProfileId);
            booking.Status = req.Status;
        }
        catch (UnauthorizedAccessException ex)
        {
            return Problem(statusCode: 403, title: "Forbidden", detail: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(statusCode: 422, title: "Unprocessable Entity", detail: ex.Message);
        }

        return Ok(new BookingResponse
        {
            Id = booking.Id,
            Status = booking.Status,
            TotalPrice = booking.TotalPrice,
            AreaInHectares = booking.AreaInHectares,
            CreatedAt = booking.CreatedAt,
            Notes = booking.Notes,
            ServiceListingId = booking.ServiceListingId,
            ClientProfileId = booking.ClientProfileId,
            AvailabilityId = booking.AvailabilityId
        });
    }
}
