using AgriMarket.BLL.Dtos.Bookings;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers.Admin;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/admin/bookings")]
[Authorize(Policy = "AdminOnly")]
public class AdminBookingsController(IBookingService bookingService) : ApiControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] BookingStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var allItems = await _bookingService.GetAllAsync(status);
        var totalCount = allItems.Count();
        var items = allItems.Skip((page - 1) * pageSize).Take(pageSize);

        return Ok(new { items, page, pageSize, totalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Booking {id} not found.");

        return Ok(booking);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookingStatusRequest req)
    {
        try
        {
            var booking = await _bookingService.UpdateStatusAsync(id, req.Status);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var booking = await _bookingService.GetByIdAsync(id);
        if (booking is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Booking {id} not found.");

        await _bookingService.DeleteAsync(id);
        return NoContent();
    }
}
