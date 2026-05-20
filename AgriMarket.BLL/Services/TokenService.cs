using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AgriMarket.Domain.Entities;
using AgriMarket.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AgriMarket.BLL.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(AppUser user, UserProfile profile, IEnumerable<RoleType> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new("profileId", profile.Id.ToString()),
            new(JwtRegisteredClaimNames.GivenName, profile.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, profile.LastName),
        };

        foreach (var role in roles)
            claims.Add(new Claim("role", role.ToString()));

        var expiryMinutes = int.Parse(_config["Jwt:AccessTokenExpiryMinutes"] ?? "60");
        return CreateJwt(claims, TimeSpan.FromMinutes(expiryMinutes));
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private string CreateJwt(IEnumerable<Claim> claims, TimeSpan expiry)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(expiry),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
