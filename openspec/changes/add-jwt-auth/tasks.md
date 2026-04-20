## 1. Domain — RefreshToken Entity

- [ ] 1.1 Create `AgriMarket.Domain/Entities/RefreshToken.cs` with fields: `Id` (Guid), `Token` (string), `AppUserId` (Guid), `ExpiresAt` (DateTime), `IsRevoked` (bool), `CreatedAt` (DateTime)
- [ ] 1.2 Add navigation property `AppUser? AppUser` to `RefreshToken`
- [ ] 1.3 Add `ICollection<RefreshToken>? RefreshTokens` navigation to `AppUser`
- [ ] 1.4 Register `DbSet<RefreshToken>` in `AppDbContext`
- [ ] 1.5 Generate EF migration: `dotnet ef migrations add AddRefreshTokens --project AgriMarket.DAL --startup-project AgriMarket.Api`
- [ ] 1.6 Verify migration applies cleanly: `dotnet ef database update`

## 2. Packages and Configuration

- [ ] 2.1 Add `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package to `AgriMarket.Api.csproj`
- [ ] 2.2 Add JWT config section to `appsettings.json`: `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:AccessTokenExpiryMinutes` (15), `Jwt:RefreshTokenExpiryDays` (30)
- [ ] 2.3 Add JWT config section to `appsettings.Development.json` with a dev-only signing key (min 32 chars)

## 3. Token Service

- [ ] 3.1 Create `AgriMarket.Api/Services/ITokenService.cs` with methods: `GenerateAccessToken(AppUser, UserProfile, RoleType)`, `GenerateSessionToken(Guid userId)`, `GenerateRefreshToken()`, `ValidateSessionToken(string token)` returning userId or null
- [ ] 3.2 Create `AgriMarket.Api/Services/TokenService.cs` implementing `ITokenService` — access token carries `sub`, `profileId`, `role` claims; session token carries only `sub`; both signed with key from config
- [ ] 3.3 Register `ITokenService` / `TokenService` as scoped in `Program.cs`

## 4. Auth Service

- [ ] 4.1 Create `AgriMarket.Api/Services/IAuthService.cs` with methods: `RegisterAsync`, `LoginAsync`, `SelectProfileAsync`, `RefreshAsync`, `LogoutAsync`
- [ ] 4.2 Implement `RegisterAsync` — validate no duplicate email, hash password with BCrypt, create `AppUser` + `UserProfile` + `ProfileRole`, reject Admin role requests
- [ ] 4.3 Implement `LoginAsync` — verify BCrypt hash, load profiles; if one profile return tokens directly, if multiple return session token + profile list
- [ ] 4.4 Implement `SelectProfileAsync` — validate session token (2-min expiry, no profileId claim), verify profileId belongs to user, return tokens
- [ ] 4.5 Implement `RefreshAsync` — find non-expired non-revoked refresh token in DB; return 401 if not found, expired, revoked, or if the linked `AppUser` no longer exists; revoke old token and issue new access + refresh token pair
- [ ] 4.6 Implement `LogoutAsync` — find refresh token in DB, mark `IsRevoked = true`, return; no-op if not found
- [ ] 4.7 Register `IAuthService` / `AuthService` as scoped in `Program.cs`

## 5. DTOs

- [ ] 5.1 Create `Dtos/Auth/RegisterRequest.cs` with: `Email`, `Password`, `FirstName`, `LastName`, `Role` (RoleType)
- [ ] 5.2 Create `Dtos/Auth/LoginRequest.cs` with: `Email`, `Password`
- [ ] 5.3 Create `Dtos/Auth/SelectProfileRequest.cs` with: `SessionToken`, `ProfileId`
- [ ] 5.4 Create `Dtos/Auth/RefreshRequest.cs` with: `RefreshToken`
- [ ] 5.5 Create `Dtos/Auth/LogoutRequest.cs` with: `RefreshToken`
- [ ] 5.6 Create `Dtos/Auth/TokenResponse.cs` with: `AccessToken`, `RefreshToken`
- [ ] 5.7 Create `Dtos/Auth/ProfileSelectionResponse.cs` with: `SessionToken`, `Profiles` (list of `{ ProfileId, FullName, Role }`)

## 6. AuthController

- [ ] 6.1 Create `Controllers/AuthController.cs` with `[ApiController]` and `[Route("api/auth")]`
- [ ] 6.2 Implement `POST /api/auth/register` → calls `AuthService.RegisterAsync`, returns 201 on success, 409 on duplicate email, 400 on validation errors
- [ ] 6.3 Implement `POST /api/auth/login` → calls `AuthService.LoginAsync`, returns 200 with `TokenResponse` or `ProfileSelectionResponse`; 401 on bad credentials
- [ ] 6.4 Implement `POST /api/auth/select-profile` → calls `AuthService.SelectProfileAsync`, returns 200 with `TokenResponse`; 401/403 on invalid token or profile
- [ ] 6.5 Implement `POST /api/auth/refresh` → calls `AuthService.RefreshAsync`, returns 200 with `TokenResponse`; 401 on invalid token
- [ ] 6.6 Implement `POST /api/auth/logout` → calls `AuthService.LogoutAsync`, returns 204

## 7. Middleware Wiring

- [ ] 7.1 Add `AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` to `Program.cs` — configure `TokenValidationParameters` with issuer, audience, signing key, and `ClockSkew = TimeSpan.Zero`
- [ ] 7.2 Add `UseAuthentication()` before `UseAuthorization()` in `Program.cs` pipeline (before `MapControllers`)
- [ ] 7.3 Add `UseAuthorization()` to `Program.cs` if not already present
- [ ] 7.4 Throw on missing `Jwt:Key` at startup (guard in `Program.cs` before building the app)

## 8. Verification

- [ ] 8.1 `dotnet build` succeeds for full solution
- [ ] 8.2 Swagger UI loads at `/swagger` and shows lock icons on `[Authorize]` endpoints (add `[Authorize]` to one existing controller temporarily to verify)
- [ ] 8.3 Register a new user via Swagger, confirm 201
- [ ] 8.4 Login with the new user, confirm token response
- [ ] 8.5 Use access token in Swagger Authorize, confirm protected endpoint works
- [ ] 8.6 Call `/api/auth/refresh` with the refresh token, confirm new token pair returned
- [ ] 8.7 Call `/api/auth/logout`, confirm 204, confirm subsequent refresh returns 401
- [ ] 8.8 Login with seed admin (`admin@agrimarket.ee` / `Admin123!`), confirm auto-skip of profile selection and direct token response
