## 1. Domain Entities

- [x] 1.1 Create `UserRole` entity (`Id`, `AppUserId` FK, `Role` RoleType) in `AgriMarket.Domain/Entities/UserRole.cs`
- [x] 1.2 Update `AppUser`: change `ICollection<UserProfile>? Profiles` to `UserProfile? Profile`, add `ICollection<UserRole>? Roles` navigation
- [x] 1.3 Update `UserProfile`: remove `ICollection<ProfileRole>? Roles` navigation property
- [x] 1.4 Delete `AgriMarket.Domain/Entities/ProfileRole.cs`
- [x] 1.5 Git commit: `refactor: replace ProfileRole with UserRole entity on AppUser`

## 2. DbContext and Migration

- [x] 2.1 Replace `DbSet<ProfileRole>` with `DbSet<UserRole>` in `AppDbContext`
- [x] 2.2 Remove `ProfileRole` unique index config, add `UserRole` unique index on `(AppUserId, Role)`
- [x] 2.3 Add unique index on `UserProfiles.AppUserId` to enforce 1:1 relationship
- [x] 2.4 Update `AppUser → UserProfile` relationship config from 1:N to 1:1
- [x] 2.5 Create EF Core migration with SQL data migration: consolidate multi-profile users, migrate `ProfileRoles` data to `UserRoles`, drop `ProfileRoles` table
- [x] 2.6 Verify migration applies cleanly with `dotnet ef database update`
- [x] 2.7 Git commit: `refactor: migrate ProfileRoles to UserRoles and enforce 1:1 UserProfile`

## 3. Repository and Contracts

- [x] 3.1 Update `IAppUserRepository` methods (`GetByEmailWithProfilesAsync`, `GetByIdWithProfilesAsync`) to include `UserRole` instead of `ProfileRole` and load single profile
- [x] 3.2 Update `AppUserRepository` implementations to use `.Include(u => u.Roles)` and `.Include(u => u.Profile)` instead of `.Include(u => u.Profiles).ThenInclude(p => p.Roles)`
- [x] 3.3 Replace `IRepository<ProfileRole>` injection with `IRepository<UserRole>` where used
- [x] 3.4 Git commit: `refactor: update repositories for unified role model`

## 4. Token Service

- [x] 4.1 Change `TokenService.GenerateAccessToken` signature from `(AppUser, UserProfile, RoleType)` to `(AppUser, UserProfile, IEnumerable<RoleType>)` — emit one `role` claim per role
- [x] 4.2 Update `ITokenService` interface to match new signature
- [x] 4.3 Remove `GenerateSessionToken` and `ValidateSessionToken` from `TokenService` and `ITokenService`
- [x] 4.4 Remove `SessionTokenExpiryMinutes` config key from `appsettings.json` (if present)
- [x] 4.5 Git commit: `refactor: emit multiple role claims and remove session tokens`

## 5. Auth Service

- [x] 5.1 Update `RegisterAsync`: remove `Role` parameter handling, create `UserRole` entries for both Farmer and Provider, remove `ProfileRole` creation
- [x] 5.2 Simplify `LoginAsync`: always return `TokenResponse` directly (remove profile selection branch), collect all roles from `user.Roles`, pass to `GenerateAccessToken`
- [x] 5.3 Remove `SelectProfileAsync` method from `AuthService` and `IAuthService`
- [x] 5.4 Update `RefreshAsync`: load roles from `user.Roles` instead of `profile.Roles`, pass all roles to `GenerateAccessToken`
- [x] 5.5 Git commit: `refactor: simplify auth service for unified role model`

## 6. Auth DTOs

- [x] 6.1 Update `RegisterRequest`: remove `Role` property
- [x] 6.2 Update `LoginResult`: remove `ProfileSelection` property, make `Tokens` non-nullable (always returned)
- [x] 6.3 Delete `ProfileSelectionResponse.cs`, `ProfileSummary.cs`, `SelectProfileRequest.cs`
- [x] 6.4 Git commit: `refactor: remove profile selection DTOs`

## 7. API Controllers

- [x] 7.1 Remove `SelectProfile` endpoint from `AuthController`
- [x] 7.2 Update `AuthController.Login` to always return `TokenResponse`
- [x] 7.3 Update `AuthController.Register` to match new `RegisterRequest` (no role field)
- [x] 7.4 Git commit: `refactor: remove select-profile endpoint from API`

## 8. Booking Service Role-Gating

- [x] 8.1 Update `BookingService` status transition logic: determine caller role by booking relationship (client = profileId matches ClientProfileId, provider = profileId matches listing.UserProfileId) instead of single JWT role claim
- [x] 8.2 Verify self-booking prevention still works with unified roles (user cannot book own listing)
- [x] 8.3 Git commit: `refactor: update booking role-gating for unified role model`

## 9. Web Controllers and Policies

- [x] 9.1 Update `Program.cs` (API): update authorization policies to work with multiple role claims
- [x] 9.2 Update `Program.cs` (Web): update `ProviderOnly` and `ClientOnly` policies for multi-role users
- [x] 9.3 Update Client `AccountController` login: load single profile via `AppUser.Profile` instead of `Profiles` collection, load roles from `AppUser.Roles` instead of `ProfileRole`, add all roles as claims
- [x] 9.4 Update Admin `AccountController` login: load single profile via `AppUser.Profile`, load roles from `AppUser.Roles` instead of `ProfileRole`
- [x] 9.5 Git commit: `refactor: update auth policies and web login for multi-role users`

## 10. Seed Data

- [x] 10.1 Update `AppDbSeeder`: create `UserRole` entries instead of `ProfileRole`, give provider and farmer test users both roles
- [x] 10.2 Ensure admin test user retains Admin role only
- [x] 10.3 Verify seeding works with `dotnet run` on fresh database
- [x] 10.4 Git commit: `refactor: update seed data for unified role model`

## 11. User Service and DTOs

- [x] 11.1 Update `UserProfileDto` to load roles from `AppUser.Roles` (via `UserProfile.AppUser.Roles`) instead of `ProfileRole`
- [x] 11.2 Update `UserService` or mapping logic to populate roles from the new path
- [x] 11.3 Git commit: `refactor: update user DTOs for unified role model`

## 12. Build Verification

- [x] 12.1 Run `dotnet build` and fix any remaining compilation errors
- [x] 12.2 Run `dotnet test` and fix any failing tests
- [x] 12.3 Git commit: `fix: resolve remaining build and test issues`
