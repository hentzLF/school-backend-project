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
    private readonly IConfiguration _config;

    public AuthController(IAuthService authService, IConfiguration config)
    {
        _authService = authService;
        _config = config;
    }

    [HttpPost("register")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return Created();
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
    [ProducesResponseType(typeof(AccessTokenResponse), 200)]
    [ProducesResponseType(typeof(ProfileSelectionResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            if (result.RequiresProfileSelection)
                return Ok(result.ProfileSelection!);

            SetRefreshTokenCookie(result.Tokens!.RefreshToken);
            return Ok(new AccessTokenResponse { AccessToken = result.Tokens.AccessToken });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Problem(statusCode: 401, title: "Unauthorized", detail: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(statusCode: 400, title: "Bad Request", detail: ex.Message);
        }
    }

    [HttpPost("select-profile")]
    [ProducesResponseType(typeof(AccessTokenResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> SelectProfile([FromBody] SelectProfileRequest request)
    {
        try
        {
            var tokens = await _authService.SelectProfileAsync(request);
            SetRefreshTokenCookie(tokens.RefreshToken);
            return Ok(new AccessTokenResponse { AccessToken = tokens.AccessToken });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Problem(statusCode: 401, title: "Unauthorized", detail: ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(statusCode: 400, title: "Bad Request", detail: ex.Message);
        }
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AccessTokenResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Problem(statusCode: 401, title: "Unauthorized", detail: "No refresh token.");

        try
        {
            var tokens = await _authService.RefreshAsync(refreshToken);
            SetRefreshTokenCookie(tokens.RefreshToken);
            return Ok(new AccessTokenResponse { AccessToken = tokens.AccessToken });
        }
        catch (UnauthorizedAccessException ex)
        {
            DeleteRefreshTokenCookie();
            return Problem(statusCode: 401, title: "Unauthorized", detail: ex.Message);
        }
    }

    [HttpPost("logout")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
            await _authService.LogoutAsync(refreshToken);

        DeleteRefreshTokenCookie();
        return NoContent();
    }

    private void SetRefreshTokenCookie(string token)
    {
        var expiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");
        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth",
            Expires = DateTimeOffset.UtcNow.AddDays(expiryDays),
        });
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth",
        });
    }
}
