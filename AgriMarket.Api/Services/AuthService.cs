using AgriMarket.Api.Dtos.Auth;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AgriMarket.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, ITokenService tokenService, IConfiguration config)
    {
        _db = db;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        if (request.Role == RoleType.Admin)
            throw new InvalidOperationException("Admin role cannot be self-assigned.");

        if (await _db.AppUsers.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email already in use.");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            AppUserId = user.Id,
        };

        var role = new ProfileRole
        {
            Id = Guid.NewGuid(),
            UserProfileId = profile.Id,
            Role = request.Role,
        };

        _db.AppUsers.Add(user);
        _db.UserProfiles.Add(profile);
        _db.ProfileRoles.Add(role);
        await _db.SaveChangesAsync();
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        var user = await _db.AppUsers
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var profiles = user.Profiles!.ToList();

        if (profiles.Count == 1)
        {
            var profile = profiles[0];
            var role = profile.Roles!.First().Role;
            var refreshToken = await IssueRefreshTokenAsync(user.Id);

            return new LoginResult
            {
                Tokens = new TokenResponse
                {
                    AccessToken = _tokenService.GenerateAccessToken(user, profile, role),
                    RefreshToken = refreshToken,
                },
            };
        }

        return new LoginResult
        {
            ProfileSelection = new ProfileSelectionResponse
            {
                SessionToken = _tokenService.GenerateSessionToken(user.Id),
                Profiles = profiles.Select(p => new ProfileSummary
                {
                    ProfileId = p.Id,
                    FullName = $"{p.FirstName} {p.LastName}",
                    Role = p.Roles!.First().Role,
                }).ToList(),
            },
        };
    }

    public async Task<TokenResponse> SelectProfileAsync(SelectProfileRequest request)
    {
        var userId = _tokenService.ValidateSessionToken(request.SessionToken);
        if (userId is null)
            throw new UnauthorizedAccessException("Invalid or expired session token.");

        var user = await _db.AppUsers
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            throw new UnauthorizedAccessException("User not found.");

        var profile = user.Profiles!.FirstOrDefault(p => p.Id == request.ProfileId);
        if (profile is null)
            throw new UnauthorizedAccessException("Profile does not belong to this user.");

        var role = profile.Roles!.First().Role;
        var refreshToken = await IssueRefreshTokenAsync(user.Id);

        return new TokenResponse
        {
            AccessToken = _tokenService.GenerateAccessToken(user, profile, role),
            RefreshToken = refreshToken,
        };
    }

    public async Task<TokenResponse> RefreshAsync(string refreshToken)
    {
        var stored = await _db.RefreshTokens
            .Include(rt => rt.AppUser!)
                .ThenInclude(u => u.Profiles!)
                    .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow || stored.AppUser is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        stored.IsRevoked = true;

        var user = stored.AppUser;
        var profile = user.Profiles!.First();
        var role = profile.Roles!.First().Role;
        var newRefreshToken = await IssueRefreshTokenAsync(user.Id);

        await _db.SaveChangesAsync();

        return new TokenResponse
        {
            AccessToken = _tokenService.GenerateAccessToken(user, profile, role),
            RefreshToken = newRefreshToken,
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (stored is null)
            return;

        stored.IsRevoked = true;
        await _db.SaveChangesAsync();
    }

    private async Task<string> IssueRefreshTokenAsync(Guid userId)
    {
        var token = _tokenService.GenerateRefreshToken();
        var expiryDays = _config.GetValue<int>("Jwt:RefreshTokenExpiryDays");

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            AppUserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
        });

        await _db.SaveChangesAsync();
        return token;
    }
}
