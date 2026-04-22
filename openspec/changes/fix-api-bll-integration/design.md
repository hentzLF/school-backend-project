## Context

The project has a complete BLL layer (`AgriMarket.BLL`) with 9 scoped services registered via `BllServiceExtensions.AddBllServices()`. The Web MVC project uses these correctly. The API project (`AgriMarket.Api`) does not call `AddBllServices()` in its `Program.cs`, so all four non-Auth API controllers inject `AppDbContext` directly and re-implement (or skip) business rules that already exist in the BLL.

Two critical business rules are currently absent from the API surface:
- `BookingService.CreateAsync` prevents a provider from booking their own listing — the API controller bypasses this
- `ReviewService.CreateAsync` validates booking state before creating a review — the API controller bypasses this

Additionally, `DashboardService.GetDashboardStatsAsync` loads all `AppUser`, `ServiceListing`, `Booking`, and `Payment` rows into memory before computing counts and sums.

## Goals / Non-Goals

**Goals:**
- Register BLL services in `AgriMarket.Api/Program.cs`
- Replace `AppDbContext` injection in the four API controllers with their corresponding BLL interfaces
- Ensure self-booking guard and review state validation fire on the API surface
- Rewrite `DashboardService` stats to aggregate in the database

**Non-Goals:**
- Changing any API endpoint routes, HTTP verbs, or response DTO shapes
- Adding new endpoints or changing authentication behavior
- Introducing a Repository pattern or additional abstraction layers
- Changing the Web MVC controllers (they already use BLL correctly)

## Decisions

### Decision 1: Register BLL via existing extension method in API Program.cs

The BLL project already provides `BllServiceExtensions.AddBllServices(IConfiguration)`. The API `Program.cs` simply needs to call this method. No new registration code is needed.

**Alternative considered**: Manually register only the services each API controller needs. Rejected — partial registration creates drift risk and the scoped lifetime is correct for all services.

### Decision 2: Controllers own DTO mapping; services own business logic

API controllers will continue to map between request DTOs and entity types before calling services, and map service results to response DTOs. This is the existing pattern in Auth — keep it consistent.

No new service methods need to be added for the wiring unless a required method is missing from an interface (e.g., `IReviewService` may need `GetByIdAsync`).

### Decision 3: DashboardService — replace table scans with DB aggregations

Replace the four `.ToListAsync()` calls with `CountAsync()`, `SumAsync()`, and `.GroupBy()...Select()` projections that execute in the database. For the "recent bookings" list (10 rows), `.Take(10)` with `.OrderByDescending()` is acceptable and stays as a materialized query.

**Alternative considered**: Caching the in-memory result. Rejected — it delays the problem and doesn't fix the root cause.

### Decision 4: Client/ListingsController inline Booking entity construction

The Web client controller already calls `bookingService.CreateAsync(booking)` after constructing the entity — the self-booking guard fires correctly. No change needed here beyond confirming the constructor sets `ClientProfileId` from the authenticated user's profile claim (not from a request body field), which aligns with the `booking-authz` spec.

## Risks / Trade-offs

- **IReviewService interface gaps** → Before refactoring `ReviewsController`, verify `IReviewService` exposes `GetAllAsync`, `GetByIdAsync(Guid)`, and `CreateAsync`. If any are missing, add them to the interface and implement in `ReviewService` first.
- **DashboardService GroupBy translation** → EF Core translates most GroupBy to SQL but complex projections can fall back to client evaluation. Verify the generated SQL after rewrite using logging or a profiler.
- **API BookingsController response shape** → The current controller hand-builds anonymous objects. After switching to `IBookingService`, the returned entity graph must include the same navigation properties (check `Include` chains in `BookingService.GetAllAsync` and `GetByIdAsync`).

## Migration Plan

1. Add `AgriMarket.BLL` project reference to `AgriMarket.Api.csproj` (if not already present)
2. Call `builder.Services.AddBllServices(builder.Configuration)` in `AgriMarket.Api/Program.cs`
3. Rewrite `DashboardService.GetDashboardStatsAsync` (self-contained, no controller changes)
4. Refactor API controllers one at a time: `UsersController` → `ListingsController` → `ReviewsController` → `BookingsController`
5. Run the test suite after each controller to catch regressions early

No database migrations required. No deployment coordination required — the change is internal to the API process.
