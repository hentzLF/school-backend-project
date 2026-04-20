## Context

AgriMarket.Api is a .NET 10 REST API with PostgreSQL via EF Core. It has no authentication today — all endpoints are open. The domain already has `AppUser` (email + BCrypt password hash + LockoutEnd), `UserProfile`, and `ProfileRole` (Farmer/Provider/Admin). A user can have multiple profiles with different roles. BCrypt.Net is already used in the seed layer. The API needs JWT-based auth before any routes can be protected.

## Goals / Non-Goals

**Goals:**
- Register new users with a single role (Farmer or Provider)
- Two-step login: credential check → profile selection (auto-skip if one profile)
- Issue short-lived JWT access tokens and long-lived DB-backed refresh tokens
- Refresh token rotation (each use issues a new token, old one is revoked)
- Logout revokes the refresh token in DB
- ASP.NET JWT bearer middleware wired up so `[Authorize]` works on any controller

**Non-Goals:**
- Locking down existing endpoints (done in a follow-up change)
- OAuth / social login (separate change, `OAuthAccount` entity already exists)
- Email verification
- Password reset flow
- Lockout enforcement (field exists, logic deferred)
- Multi-profile switching after login (future feature)

## Decisions

### D1: Two-step login with session token for multi-profile users

**Decision**: Step 1 returns either full tokens (single profile) or a short-lived JWT session token + profile list (multiple profiles). Step 2 exchanges session token + profileId for full tokens.

**Rationale**: Avoids a separate DB table for pending sessions. A short-lived JWT (2-min expiry) with only `userId` in claims is self-contained and verifiable without any state.

**Alternative considered**: Store a pending session UUID in DB — more auditable but adds a table and DB write for a transient state.

### D2: JWT claims carry userId + profileId + role

**Decision**: Access token payload: `sub` (userId), `profileId`, `role` (RoleType string).

**Rationale**: Controllers can read role and profileId directly from claims without a DB lookup on every request. The tradeoff (role changes not reflected until token expiry) is acceptable given short access token lifetime (15 min).

**Alternative considered**: Store only `userId` in token, resolve profile on every request — simpler token but adds latency and DB load.

### D3: DB-backed refresh tokens with rotation

**Decision**: `RefreshToken` entity stored in PostgreSQL. Each `/auth/refresh` call validates the token, issues a new one, and revokes the old one. `/auth/logout` marks the token revoked.

**Rationale**: Revocable, auditable, supports logout. Rotation limits the damage window if a refresh token is stolen.

**Alternative considered**: Stateless refresh tokens (signed JWTs) — no DB needed but impossible to revoke before expiry.

### D4: BCrypt for password hashing (existing approach)

**Decision**: Continue using `BCrypt.Net.BCrypt.HashPassword` / `BCrypt.Net.BCrypt.Verify`.

**Rationale**: Already in use in the seed layer. Consistent. Default work factor (11) is acceptable.

### D5: AuthService as a scoped service in AgriMarket.Api

**Decision**: Introduce `IAuthService` / `AuthService` in `AgriMarket.Api` (not a separate class library). Similarly `ITokenService` / `TokenService` for JWT generation.

**Rationale**: Auth logic is tightly coupled to the API layer and HTTP concerns. No other project needs it. Keeping it in `AgriMarket.Api` avoids premature layering.

### D6: ClockSkew set to zero

**Decision**: Set `ClockSkew = TimeSpan.Zero` in `TokenValidationParameters`.

**Rationale**: The default ASP.NET clock skew is 5 minutes, meaning a 15-minute access token is actually valid for up to 20 minutes. With `ClockSkew = Zero`, expiry is exact and the stated 15-minute lifetime is enforced. The tradeoff (minor clock drift between client and server can cause premature rejection) is acceptable — servers should be NTP-synced, and the refresh flow handles seamless renewal.

**Alternative considered**: Leave at default — simpler but undermines the stated token lifetime, which matters for the role-change mitigation in D2.

### D7: JWT config in appsettings.json

**Decision**: Store `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:AccessTokenExpiryMinutes`, `Jwt:RefreshTokenExpiryDays` in `appsettings.json` / environment variables.

**Rationale**: Standard ASP.NET configuration pattern. Key should be overridden via environment variable in production.

## Risks / Trade-offs

- **Short access token lifetime (15 min)** → clients must implement refresh logic; acceptable for a proper API client
- **Role in token** → role change (e.g., admin grants Provider role) won't take effect until access token expires → Mitigation: keep access token short (15 min)
- **Refresh token in DB** → DB write on every refresh call → Mitigation: acceptable at this scale; can add caching later
- **Session token for profile selection** → if user loses it (2 min window), they must re-login → acceptable UX tradeoff for statelessness

## Migration Plan

1. Add `RefreshToken` entity to `AgriMarket.Domain`
2. Register `DbSet<RefreshToken>` in `AppDbContext`
3. Generate and apply EF migration: `dotnet ef migrations add AddRefreshTokens`
4. Add `Microsoft.AspNetCore.Authentication.JwtBearer` package
5. Configure JWT in `appsettings.json` (dev key only — prod key via env var)
6. Wire middleware in `Program.cs` (`AddAuthentication` + `UseAuthentication` + `UseAuthorization`)
7. Implement `TokenService`, `AuthService`, `AuthController`
8. Verify with Swagger (lock icon appears on endpoints)

**Rollback**: Remove migration, remove package, remove middleware — no existing endpoints are affected.

## Open Questions

- Should `RefreshToken` be cleaned up on a schedule (e.g., delete expired tokens)? Deferred — add a background job later.
- Should Admin users be blocked from registering via `/auth/register`? Yes — Admin role is seeded only, not self-assignable.
