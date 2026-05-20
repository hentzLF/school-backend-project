using AgriMarket.BLL.Contracts;
using AgriMarket.BLL.Dtos.Auth;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AgriMarket.BLL.Services;

public class AuthService(
    IAppUserRepository appUsers,
    IRepository<UserProfile> userProfiles,
    IRepository<UserRole> userRoles,
    IRefreshTokenRepository refreshTokens,
    IUnitOfWork uow,
    ITokenService tokenService,
    IPasswordHasher passwordHasher,
    IConfiguration config,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task RegisterAsync(RegisterRequest request)
    {
        if (await appUsers.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Email already in use.");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHasher.Hash(request.Password),
        };

        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            AppUserId = user.Id,
        };

        var farmerRole = new UserRole
        {
            Id = Guid.NewGuid(),
            AppUserId = user.Id,
            Role = RoleType.Farmer,
        };

        var providerRole = new UserRole
        {
            Id = Guid.NewGuid(),
            AppUserId = user.Id,
            Role = RoleType.Provider,
        };

        await uow.BeginTransactionAsync();
        try
        {
            appUsers.Add(user);
            userProfiles.Add(profile);
            userRoles.Add(farmerRole);
            userRoles.Add(providerRole);
            await uow.SaveChangesAsync();
            await uow.CommitTransactionAsync();
        }
        catch
        {
            await uow.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await appUsers.GetByEmailWithProfilesAsync(request.Email);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Failed login attempt for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var profile = user.Profile
            ?? throw new InvalidOperationException("User has no profile.");
        var roles = user.Roles?.Select(r => r.Role).ToList()
            ?? throw new InvalidOperationException("User has no assigned roles.");
        var refreshToken = await IssueRefreshTokenAsync(user.Id);

        return new TokenResponse
        {
            AccessToken = tokenService.GenerateAccessToken(user, profile, roles),
            RefreshToken = refreshToken,
        };
    }

    public async Task<TokenResponse> RefreshAsync(string refreshToken)
    {
        var stored = await refreshTokens.GetByTokenWithUserAsync(refreshToken);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow || stored.AppUser is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        await uow.BeginTransactionAsync();
        try
        {
            stored.IsRevoked = true;

            var user = stored.AppUser;
            var profile = user.Profile
                ?? throw new InvalidOperationException("User has no profile.");
            var roles = user.Roles?.Select(r => r.Role).ToList()
                ?? throw new InvalidOperationException("User has no assigned roles.");
            var newRefreshToken = await IssueRefreshTokenAsync(user.Id);

            await uow.SaveChangesAsync();
            await uow.CommitTransactionAsync();

            return new TokenResponse
            {
                AccessToken = tokenService.GenerateAccessToken(user, profile, roles),
                RefreshToken = newRefreshToken,
            };
        }
        catch
        {
            await uow.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var stored = await refreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
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
