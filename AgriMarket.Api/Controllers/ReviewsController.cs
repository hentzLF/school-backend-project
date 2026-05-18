using AgriMarket.Api.Mappers;
using AgriMarket.BLL;
using AgriMarket.BLL.Dtos;
using AgriMarket.BLL.Dtos.Reviews;
using AgriMarket.BLL.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/reviews")]
public class ReviewsController(IReviewService reviewService) : ApiControllerBase
{
    private readonly IReviewService _reviewService = reviewService;

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ReviewDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var result = await _reviewService.GetAllAsync(page, pageSize);
        return Ok(new PaginatedResponse<ReviewDto>
        {
            Items = result.Items,
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReviewDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Review {id} not found.");

        return Ok(review);
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), 201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto req)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            var review = await _reviewService.CreateAsync(userId, req);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 422, title: "Unprocessable Entity", detail: ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ReviewDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewDto req)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            var review = await _reviewService.UpdateAsync(userId, req.WithRouteId(id));
            return Ok(review);
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 403, title: "Forbidden", detail: ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "Invalid user identity.");

        try
        {
            await _reviewService.DeleteAsync(userId, id);
            return NoContent();
        }
        catch (BusinessRuleException ex)
        {
            return Problem(statusCode: 403, title: "Forbidden", detail: ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: ex.Message);
        }
    }

    [HttpGet("booking/{bookingId:guid}")]
    [ProducesResponseType(typeof(ReviewDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByBooking(Guid bookingId)
    {
        var review = await _reviewService.GetByBookingAsync(bookingId);
        if (review is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"No review found for booking {bookingId}.");

        return Ok(review);
    }

    [HttpGet("profile/{profileId:guid}")]
    [ProducesResponseType(typeof(PaginatedResponse<ReviewDto>), 200)]
    public async Task<IActionResult> GetByProfile(Guid profileId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var result = await _reviewService.GetByProfileAsync(profileId, page, pageSize);
        return Ok(new PaginatedResponse<ReviewDto>
        {
            Items = result.Items,
            Page = page,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        });
    }
}
