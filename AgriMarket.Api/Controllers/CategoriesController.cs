using AgriMarket.BLL.Dtos.Categories;
using AgriMarket.BLL.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/categories")]
public class CategoriesController(ICategoryService categoryService) : ApiControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        var result = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });
        return Ok(result);
    }
}
