using AgriMarket.Api.Dtos.Bookings;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public BookingsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var query = _db.Bookings.AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookingResponse
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
            })
            .ToListAsync();

        return Ok(new { items, page, pageSize, totalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var booking = await _db.Bookings.AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookingResponse
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
            })
            .FirstOrDefaultAsync();

        if (booking is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Booking {id} not found.");

        return Ok(booking);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest req)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = BookingStatus.Pending,
            TotalPrice = 0,
            AreaInHectares = req.AreaInHectares,
            CreatedAt = DateTime.UtcNow,
            Notes = req.Notes,
            ServiceListingId = req.ServiceListingId,
            ClientProfileId = req.ClientProfileId,
            AvailabilityId = req.AvailabilityId
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

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

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookingStatusRequest req)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Booking {id} not found.");

        booking.Status = req.Status;
        await _db.SaveChangesAsync();

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
