using AgriMarket.BLL;
using AgriMarket.BLL.Services;
using AgriMarket.Domain.Entities;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers.Admin;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/admin/categories")]
[Authorize(Policy = "AdminOnly")]
public class AdminCategoriesController(ICategoryService categoryService) : ApiControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Category {id} not found.");

        return Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ServiceCategory category)
    {
        category.Id = Guid.NewGuid();
        await _categoryService.CreateAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ServiceCategory category)
    {
        var existing = await _categoryService.GetByIdAsync(id);
        if (existing is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Category {id} not found.");

        category.Id = id;
        await _categoryService.UpdateAsync(category);
        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _categoryService.GetByIdAsync(id);
        if (existing is null)
            return Problem(statusCode: 404, title: "Not Found", detail: $"Category {id} not found.");

        var listingCount = await _categoryService.GetListingCountAsync(id);
        if (listingCount > 0)
            return Problem(statusCode: 400, title: "Bad Request",
                detail: $"Cannot delete category with {listingCount} associated listing(s).");

        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}
