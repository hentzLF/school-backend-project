using AgriMarket.BLL.Dtos.Auth;
using AgriMarket.BLL.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return StatusCode(201);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Email already in use.")
        {
            return Problem(statusCode: 409, title: "Conflict", detail: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(statusCode: 400, title: "Bad Request", detail: ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result.RequiresProfileSelection
                ? (object)result.ProfileSelection!
                : result.Tokens!);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Problem(statusCode: 401, title: "Unauthorized", detail: ex.Message);
        }
    }

    [HttpPost("select-profile")]
    public async Task<IActionResult> SelectProfile([FromBody] SelectProfileRequest request)
    {
        try
        {
            var tokens = await _authService.SelectProfileAsync(request);
            return Ok(tokens);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Problem(statusCode: 401, title: "Unauthorized", detail: ex.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var tokens = await _authService.RefreshAsync(request.RefreshToken);
            return Ok(tokens);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Problem(statusCode: 401, title: "Unauthorized", detail: ex.Message);
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        await _authService.LogoutAsync(request.RefreshToken);
        return NoContent();
    }
}
