## 1. Project Setup

- [x] 1.1 Confirm `AgriMarket.BLL` is listed as a project reference in `AgriMarket.Api/AgriMarket.Api.csproj`; add it if missing
- [x] 1.2 Add `builder.Services.AddBllServices(builder.Configuration)` to `AgriMarket.Api/Program.cs` before `builder.Build()`
- [x] 1.3 Remove any remaining `builder.Services.AddScoped<AppDbContext>` registrations that existed only to support the direct-DbContext controllers (keep only the EF registration)

## 2. DashboardService Performance Fix

- [x] 2.1 Rewrite user stats in `DashboardService.GetDashboardStatsAsync` using `CountAsync()` and date-filtered `CountAsync()` instead of `.ToListAsync()` + LINQ
- [x] 2.2 Rewrite listing stats (total, active, inactive) using `CountAsync()` with appropriate `Where` filters
- [x] 2.3 Rewrite booking stats using `CountAsync()` for total and a `GroupBy(b => b.Status).Select(g => new { g.Key, Count = g.Count() }).ToListAsync()` for the status breakdown
- [x] 2.4 Rewrite revenue stats using `SumAsync(p => p.Amount)` and `SumAsync(p => p.PlatformFee)` — scoped to the current month for monthly revenue
- [x] 2.5 Rewrite dispute stats using `CountAsync()` with status filters for active and resolved disputes
- [x] 2.6 Keep the recent bookings query as `Take(10).OrderByDescending(...).ToListAsync()` with required navigation property includes

## 3. IReviewService Interface Audit

- [x] 3.1 Check `IReviewService` for the methods needed by the API controller: `GetAllAsync()`, `GetByIdAsync(Guid)`, `CreateAsync(Review)`
- [x] 3.2 Add any missing method signatures to `IReviewService`
- [x] 3.3 Implement any newly added interface members in `ReviewService`

## 4. API Controller Refactoring

- [x] 4.1 Refactor `UsersController` — replace `AppDbContext` constructor injection with `IUserService`; map service results to existing response shapes
- [x] 4.2 Refactor `ListingsController` — replace `AppDbContext` injection with `IListingService`; keep ownership checks by comparing profileId claim to listing's `UserProfileId` before mutating
- [x] 4.3 Refactor `ReviewsController` — replace `AppDbContext` injection with `IReviewService`; the `CreateAsync` call in the service already validates booking state (422 on failure)
- [x] 4.4 Refactor `BookingsController` — replace `AppDbContext` injection with `IBookingService`; the `CreateAsync` call in the service already enforces the self-booking guard (400 on failure)
- [x] 4.5 Remove the inline `GetAllowedTransitions()` method from `BookingsController` — delegate status update logic to `IBookingService.UpdateStatusAsync`

## 5. Verification

- [x] 5.1 Build `AgriMarket.Api` — confirm zero compilation errors
- [x] 5.2 Build `AgriMarket.Web` — confirm no regressions introduced
- [x] 5.3 Run the test suite (`AgriMarket.Tests`) and confirm all tests pass
- [x] 5.4 Manually verify that a provider attempting to book their own listing via the API receives HTTP 400
- [x] 5.5 Manually verify that creating a review via API for an ineligible booking status returns HTTP 422
