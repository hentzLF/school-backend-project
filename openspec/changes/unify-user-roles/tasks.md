## 1. Domain Entities

- [ ] 1.1 Create `UserRole` entity (`Id`, `AppUserId` FK, `Role` RoleType) in `AgriMarket.Domain/Entities/UserRole.cs`
- [ ] 1.2 Update `AppUser`: change `ICollection<UserProfile>? Profiles` to `UserProfile? Profile`, add `ICollection<UserRole>? Roles` navigation
- [ ] 1.3 Update `UserProfile`: remove `ICollection<ProfileRole>? Roles` navigation property
- [ ] 1.4 Delete `AgriMarket.Domain/Entities/ProfileRole.cs`
- [ ] 1.5 Git commit: `refactor: replace ProfileRole with UserRole entity on AppUser`

## 2. DbContext and Migration

- [ ] 2.1 Replace `DbSet<ProfileRole>` with `DbSet<UserRole>` in `AppDbContext`
- [ ] 2.2 Remove `ProfileRole` unique index config, add `UserRole` unique index on `(AppUserId, Role)`
- [ ] 2.3 Add unique index on `UserProfiles.AppUserId` to enforce 1:1 relationship
- [ ] 2.4 Update `AppUser → UserProfile` relationship config from 1:N to 1:1
- [ ] 2.5 Create EF Core migration with SQL data migration: consolidate multi-profile users, migrate `ProfileRoles` data to `UserRoles`, drop `ProfileRoles` table
- [ ] 2.6 Verify migration applies cleanly with `dotnet ef database update`
- [ ] 2.7 Git commit: `refactor: migrate ProfileRoles to UserRoles and enforce 1:1 UserProfile`

## 3. Repository and Contracts

- [ ] 3.1 Update `IAppUserRepository` methods (`GetByEmailWithProfilesAsync`, `GetByIdWithProfilesAsync`) to include `UserRole` instead of `ProfileRole` and load single profile
- [ ] 3.2 Update `AppUserRepository` implementations to use `.Include(u => u.Roles)` and `.Include(u => u.Profile)` instead of `.Include(u => u.Profiles).ThenInclude(p => p.Roles)`
- [ ] 3.3 Replace `IRepository<ProfileRole>` injection with `IRepository<UserRole>` where used
- [ ] 3.4 Git commit: `refactor: update repositories for unified role model`

## 4. Token Service

- [ ] 4.1 Change `TokenService.GenerateAccessToken` signature from `(AppUser, UserProfile, RoleType)` to `(AppUser, UserProfile, IEnumerable<RoleType>)` — emit one `role` claim per role
- [ ] 4.2 Update `ITokenService` interface to match new signature
- [ ] 4.3 Remove `GenerateSessionToken` and `ValidateSessionToken` from `TokenService` and `ITokenService`
- [ ] 4.4 Remove `SessionTokenExpiryMinutes` config key from `appsettings.json` (if present)
- [ ] 4.5 Git commit: `refactor: emit multiple role claims and remove session tokens`

## 5. Auth Service

- [ ] 5.1 Update `RegisterAsync`: remove `Role` parameter handling, create `UserRole` entries for both Farmer and Provider, remove `ProfileRole` creation
- [ ] 5.2 Simplify `LoginAsync`: always return `TokenResponse` directly (remove profile selection branch), collect all roles from `user.Roles`, pass to `GenerateAccessToken`
- [ ] 5.3 Remove `SelectProfileAsync` method from `AuthService` and `IAuthService`
- [ ] 5.4 Update `RefreshAsync`: load roles from `user.Roles` instead of `profile.Roles`, pass all roles to `GenerateAccessToken`
- [ ] 5.5 Git commit: `refactor: simplify auth service for unified role model`

## 6. Auth DTOs

- [ ] 6.1 Update `RegisterRequest`: remove `Role` property
- [ ] 6.2 Update `LoginResult`: remove `ProfileSelection` property, make `Tokens` non-nullable (always returned)
- [ ] 6.3 Delete `ProfileSelectionResponse.cs`, `ProfileSummary.cs`, `SelectProfileRequest.cs`
- [ ] 6.4 Git commit: `refactor: remove profile selection DTOs`

## 7. API Controllers

- [ ] 7.1 Remove `SelectProfile` endpoint from `AuthController`
- [ ] 7.2 Update `AuthController.Login` to always return `TokenResponse`
- [ ] 7.3 Update `AuthController.Register` to match new `RegisterRequest` (no role field)
- [ ] 7.4 Git commit: `refactor: remove select-profile endpoint from API`

## 8. Web Controllers and Policies

- [ ] 8.1 Update `Program.cs` (API): update authorization policies to work with multiple role claims
- [ ] 8.2 Update `Program.cs` (Web): update `ProviderOnly` and `ClientOnly` policies for multi-role users
- [ ] 8.3 Update Client `AccountController` login: load roles from `AppUser.Roles` instead of `ProfileRole`, add all roles as claims
- [ ] 8.4 Update Admin `AccountController` login: load roles from `AppUser.Roles` instead of `ProfileRole`
- [ ] 8.5 Git commit: `refactor: update auth policies and web login for multi-role users`

## 9. Seed Data

- [ ] 9.1 Update `AppDbSeeder`: create `UserRole` entries instead of `ProfileRole`, give provider and farmer test users both roles
- [ ] 9.2 Ensure admin test user retains Admin role only
- [ ] 9.3 Verify seeding works with `dotnet run` on fresh database
- [ ] 9.4 Git commit: `refactor: update seed data for unified role model`

## 10. User Service and DTOs

- [ ] 10.1 Update `UserProfileDto` to load roles from `AppUser.Roles` (via `UserProfile.AppUser.Roles`) instead of `ProfileRole`
- [ ] 10.2 Update `UserService` or mapping logic to populate roles from the new path
- [ ] 10.3 Git commit: `refactor: update user DTOs for unified role model`

## 11. Build Verification

- [ ] 11.1 Run `dotnet build` and fix any remaining compilation errors
- [ ] 11.2 Run `dotnet test` and fix any failing tests
- [ ] 11.3 Git commit: `fix: resolve remaining build and test issues`
