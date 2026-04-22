using AgriMarket.Api.Mappers;
using AgriMarket.BLL;
using AgriMarket.BLL.Dtos.Listings;
using AgriMarket.BLL.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/listings")]
public class ListingsController(IListingService listingService) : ApiControllerBase
{
    private readonly IListingService _listingService = listingService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (pageSize > 100) pageSize = 100;
        if (page < 1) page = 1;

        var allItems = await _listingService.GetAllAsync();
        var totalCount = allItems.Count();
        var items = allItems.Skip((page - 1) * pageSize).Take(pageSize);

        return Ok(new { items, page, pageSize, totalCount });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var listing = await _listingService.GetByIdAsync(id);

        if (listing is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"ServiceListing {id} not found.");

        return Ok(listing);
    }

    [Authorize]
    [HttpPost]
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
            return Problem(statusCode: 400, title: "Bad Request", detail: ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
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
            return Problem(statusCode: 400, title: "Bad Request", detail: ex.Message);
        }
    }

}
