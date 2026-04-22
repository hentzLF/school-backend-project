using AgriMarket.Api.Dtos.Reviews;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgriMarket.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var result = await _reviewService.GetAllAsync(page, pageSize);
        var items = result.Items.Select(r => new ReviewResponse
        {
            Id = r.Id,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            BookingId = r.BookingId,
            ReviewerProfileId = r.ReviewerProfileId
        });

        return Ok(new { items, page, pageSize, totalCount = result.TotalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _reviewService.GetByIdAsync(id);

        if (review is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Review {id} not found.");

        return Ok(new ReviewResponse
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            BookingId = review.BookingId,
            ReviewerProfileId = review.ReviewerProfileId
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest req)
    {
        var callerProfileId = Guid.Parse(User.FindFirstValue("profileId")!);

        var review = new Review
        {
            Id = Guid.NewGuid(),
            Rating = req.Rating,
            Comment = req.Comment,
            CreatedAt = DateTime.UtcNow,
            BookingId = req.BookingId,
            ReviewerProfileId = callerProfileId
        };

        try
        {
            await _reviewService.CreateAsync(review);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(statusCode: 422, title: "Unprocessable Entity", detail: ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }

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
