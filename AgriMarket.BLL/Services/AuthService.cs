using AgriMarket.BLL.Dtos.Auth;
using AgriMarket.DAL;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AgriMarket.BLL.Services;

public class AuthService(
    IRepository<AppUser> appUsers,
    IRepository<UserProfile> userProfiles,
    IRepository<ProfileRole> profileRoles,
    IRepository<RefreshToken> refreshTokens,
    IUnitOfWork uow,
    ITokenService tokenService,
    IConfiguration config) : IAuthService
{
    public async Task RegisterAsync(RegisterRequest request)
    {
        if (request.Role == RoleType.Admin)
            throw new InvalidOperationException("Admin role cannot be self-assigned.");

        if (await appUsers.AnyAsync(u => u.Email == request.Email))
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

        appUsers.Add(user);
        userProfiles.Add(profile);
        profileRoles.Add(role);
        await uow.SaveChangesAsync();
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        var user = await appUsers.Query()
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var profiles = user.Profiles!.ToList();

        if (profiles.Count == 1)
        {
            var profile = profiles[0];
            var role = profile.Roles?.FirstOrDefault()?.Role
                ?? throw new InvalidOperationException("Profile has no assigned role.");
            var refreshToken = await IssueRefreshTokenAsync(user.Id);

            return new LoginResult
            {
                Tokens = new TokenResponse
                {
                    AccessToken = tokenService.GenerateAccessToken(user, profile, role),
                    RefreshToken = refreshToken,
                },
            };
        }

        return new LoginResult
        {
            ProfileSelection = new ProfileSelectionResponse
            {
                SessionToken = tokenService.GenerateSessionToken(user.Id),
                Profiles = profiles.Select(p => new ProfileSummary
                {
                    ProfileId = p.Id,
                    FullName = $"{p.FirstName} {p.LastName}",
                    Role = p.Roles?.FirstOrDefault()?.Role ?? throw new InvalidOperationException("Profile has no assigned role."),
                }).ToList(),
            },
        };
    }

    public async Task<TokenResponse> SelectProfileAsync(SelectProfileRequest request)
    {
        var userId = tokenService.ValidateSessionToken(request.SessionToken);
        if (userId is null)
            throw new UnauthorizedAccessException("Invalid or expired session token.");

        var user = await appUsers.Query()
            .Include(u => u.Profiles!)
                .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            throw new UnauthorizedAccessException("User not found.");

        var profile = user.Profiles!.FirstOrDefault(p => p.Id == request.ProfileId);
        if (profile is null)
            throw new UnauthorizedAccessException("Profile does not belong to this user.");

        var role = profile.Roles?.FirstOrDefault()?.Role
            ?? throw new InvalidOperationException("Profile has no assigned role.");
        var refreshToken = await IssueRefreshTokenAsync(user.Id);

        return new TokenResponse
        {
            AccessToken = tokenService.GenerateAccessToken(user, profile, role),
            RefreshToken = refreshToken,
        };
    }

    public async Task<TokenResponse> RefreshAsync(string refreshToken)
    {
        var stored = await refreshTokens.Query()
            .Include(rt => rt.AppUser!)
                .ThenInclude(u => u.Profiles!)
                    .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow || stored.AppUser is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        stored.IsRevoked = true;

        var user = stored.AppUser;
        var profile = user.Profiles?.FirstOrDefault()
            ?? throw new InvalidOperationException("User has no profile.");
        var role = profile.Roles?.FirstOrDefault()?.Role
            ?? throw new InvalidOperationException("Profile has no assigned role.");
        var newRefreshToken = await IssueRefreshTokenAsync(user.Id);

        await uow.SaveChangesAsync();

        return new TokenResponse
        {
            AccessToken = tokenService.GenerateAccessToken(user, profile, role),
            RefreshToken = newRefreshToken,
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var stored = await refreshTokens.Query().FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (stored is null)
            return;

        stored.IsRevoked = true;
        await uow.SaveChangesAsync();
    }

    private async Task<string> IssueRefreshTokenAsync(Guid userId)
    {
        var token = tokenService.GenerateRefreshToken();
        var expiryDays = int.Parse(config["Jwt:RefreshTokenExpiryDays"] ?? "7");

        refreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            AppUserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
        });

        await uow.SaveChangesAsync();
        return token;
    }
}
