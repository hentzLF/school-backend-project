using AgriMarket.BLL.Dtos;
using AgriMarket.BLL.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/provider/dashboard")]
[Authorize]
public class ProviderDashboardController(IProviderDashboardService providerDashboardService) : ApiControllerBase
{
    private readonly IProviderDashboardService _providerDashboardService = providerDashboardService;

    [HttpGet]
    [ProducesResponseType(typeof(ProviderDashboardDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Get()
    {
        if (!TryGetProfileId(out var profileId))
            return Problem(statusCode: 403, title: "Forbidden", detail: "No provider profile found.");

        var stats = await _providerDashboardService.GetStatsAsync(profileId);
        return Ok(stats);
    }
}
