using AgriMarket.BLL;
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
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var result = await _reviewService.GetAllAsync(page, pageSize);
        return Ok(new { items = result.Items, page, pageSize, totalCount = result.TotalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Review {id} not found.");

        return Ok(review);
    }

    [Authorize]
    [HttpPost]
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

}
