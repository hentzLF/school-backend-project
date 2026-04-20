using AgriMarket.Api.Dtos.Bookings;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var query = _db.Bookings.AsNoTracking()
            .Include(b => b.ServiceListing)
            .Where(b => b.ClientProfileId == callerProfileId
                     || b.ServiceListing!.UserProfileId == callerProfileId);

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

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var booking = await _db.Bookings.AsNoTracking()
            .Include(b => b.ServiceListing)
            .Where(b => b.Id == id)
            .FirstOrDefaultAsync();

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

    [Authorize]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookingStatusRequest req)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var booking = await _db.Bookings
            .Include(b => b.ServiceListing)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Booking {id} not found.");

        var isClient = booking.ClientProfileId == callerProfileId;
        var isProvider = booking.ServiceListing?.UserProfileId == callerProfileId;

        if (!isClient && !isProvider)
            return Problem(statusCode: 403, title: "Forbidden", detail: "You are not a party to this booking.");

        var allowed = GetAllowedTransitions(booking.Status, isClient, isProvider);
        if (!allowed.Contains(req.Status))
            return Problem(statusCode: 422, title: "Unprocessable Entity",
                detail: $"Transition from {booking.Status} to {req.Status} is not permitted for your role.");

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

    private static IReadOnlySet<BookingStatus> GetAllowedTransitions(BookingStatus current, bool isClient, bool isProvider)
    {
        var result = new HashSet<BookingStatus>();

        if (isClient)
        {
            if (current == BookingStatus.Pending) result.Add(BookingStatus.Cancelled);
            if (current == BookingStatus.Confirmed) result.Add(BookingStatus.Cancelled);
            if (current == BookingStatus.ProviderCompleted) result.Add(BookingStatus.ClientConfirmed);
        }

        if (isProvider)
        {
            if (current == BookingStatus.Pending)
            {
                result.Add(BookingStatus.Confirmed);
                result.Add(BookingStatus.Cancelled);
            }
            if (current == BookingStatus.Confirmed) result.Add(BookingStatus.InProgress);
            if (current == BookingStatus.InProgress) result.Add(BookingStatus.ProviderCompleted);
            var terminal = new[] { BookingStatus.Cancelled, BookingStatus.ClientConfirmed, BookingStatus.Disputed };
            if (!terminal.Contains(current)) result.Add(BookingStatus.Disputed);
        }

        return result;
    }
}
