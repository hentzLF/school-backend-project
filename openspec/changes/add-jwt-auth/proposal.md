## Why

The API currently has no authentication — all endpoints are publicly accessible. Users need to register, log in, and have their identity and role verified on protected routes before the application can be used in production.

## What Changes

- Add `POST /api/auth/register` — creates `AppUser` + `UserProfile` + `ProfileRole` (Farmer or Provider), returns 201
- Add `POST /api/auth/login` — verifies BCrypt password; if user has one profile returns tokens directly, if multiple profiles returns a short-lived session token + profile list
- Add `POST /api/auth/select-profile` — exchanges session token + chosen `profileId` for full JWT access + refresh tokens
- Add `POST /api/auth/refresh` — rotates refresh token, returns new access token
- Add `POST /api/auth/logout` — revokes refresh token in DB, returns 204
- Add `RefreshToken` entity to domain + migration
- Add `Microsoft.AspNetCore.Authentication.JwtBearer` package to `AgriMarket.Api`
- Wire `UseAuthentication()` + `UseAuthorization()` middleware in `Program.cs`
- JWT claims carry: `sub` (userId), `profileId`, `role`
- Short-lived session token (2 min) carries only `userId`, used exclusively for profile selection step

## Capabilities

### New Capabilities

- `auth-register`: User registration flow — create account with one role (Farmer or Provider)
- `auth-login`: Two-step login — credential verification + optional profile selection
- `auth-tokens`: JWT access token + DB-backed refresh token issuance, rotation, and revocation
- `auth-middleware`: ASP.NET JWT bearer middleware configuration and `[Authorize]` wiring

### Modified Capabilities

(none — existing endpoints are not yet protected, authorization will be added in a follow-up change)

## Impact

- **New entity**: `RefreshToken` in `AgriMarket.Domain`, registered in `AppDbContext`, requires a new EF migration
- **New project**: `AgriMarket.Api` — new `AuthController`, new `Dtos/Auth/` folder, new `Services/AuthService` or `TokenService`
- **New package**: `Microsoft.AspNetCore.Authentication.JwtBearer` added to `AgriMarket.Api.csproj`
- **Configuration**: JWT signing key, issuer, audience, expiry values added to `appsettings.json`
- **BCrypt**: Already present in `AgriMarket.DAL` via seed — consistent hashing approach maintained
- **No breaking changes** to existing listing, booking, user, or review endpoints
