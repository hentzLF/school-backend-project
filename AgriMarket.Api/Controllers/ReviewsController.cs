using AgriMarket.Api.Dtos.Reviews;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReviewsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var query = _db.Reviews.AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                BookingId = r.BookingId,
                ReviewerProfileId = r.ReviewerProfileId
            })
            .ToListAsync();

        return Ok(new { items, page, pageSize, totalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _db.Reviews.AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                BookingId = r.BookingId,
                ReviewerProfileId = r.ReviewerProfileId
            })
            .FirstOrDefaultAsync();

        if (review is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Review {id} not found.");

        return Ok(review);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest req)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            Rating = req.Rating,
            Comment = req.Comment,
            CreatedAt = DateTime.UtcNow,
            BookingId = req.BookingId,
            ReviewerProfileId = req.ReviewerProfileId
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var response = new ReviewResponse
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            BookingId = review.BookingId,
            ReviewerProfileId = review.ReviewerProfileId
        };

        return CreatedAtAction(nameof(GetById), new { id = review.Id }, response);
    }
}
