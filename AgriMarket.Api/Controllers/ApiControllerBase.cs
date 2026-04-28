using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace AgriMarket.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected bool TryGetUserId(out Guid userId)
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(sub, out userId);
    }

    protected bool TryGetProfileId(out Guid profileId)
    {
        var value = User.FindFirst("profileId")?.Value;
        return Guid.TryParse(value, out profileId);
    }

    protected Guid? GetCallerUserId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return Guid.TryParse(sub, out var userId) ? userId : null;
    }
}
