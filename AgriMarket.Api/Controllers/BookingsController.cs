using AgriMarket.BLL;
using AgriMarket.BLL.Dtos.Bookings;
using AgriMarket.BLL.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/bookings")]
public class BookingsController(IBookingService bookingService) : ApiControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        if (!TryGetProfileId(out var callerProfileId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid profile identity.");

        var result = await _bookingService.GetAllForProfileAsync(callerProfileId, page, pageSize);
        return Ok(new { items = result.Items, page, pageSize, totalCount = result.TotalCount });
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetProfileId(out var callerProfileId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid profile identity.");

        var booking = await _bookingService.GetByIdAsync(id);
        if (booking is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Booking {id} not found.");

        if (booking.ClientProfileId != callerProfileId && booking.ProviderProfileId != callerProfileId)
            return Problem(statusCode: 403, title: "Forbidden", detail: "You are not a party to this booking.");

        return Ok(booking);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto req)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            var booking = await _bookingService.CreateAsync(userId, req);
            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 400, title: "Bad Request", detail: ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookingStatusRequest req)
    {
        if (!TryGetProfileId(out var callerProfileId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid profile identity.");

        try
        {
            var booking = await _bookingService.UpdateStatusAsync(id, req.Status, callerProfileId);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Problem(statusCode: 403, title: "Forbidden", detail: ex.Message);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 422, title: "Unprocessable Entity", detail: ex.Message);
        }
    }

}
