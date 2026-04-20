using AgriMarket.Domain.Enums;

namespace AgriMarket.Api.Dtos.Auth;

public sealed class ProfileSummary
{
    public Guid ProfileId { get; init; }
    public string FullName { get; init; } = default!;
    public RoleType Role { get; init; }
}
