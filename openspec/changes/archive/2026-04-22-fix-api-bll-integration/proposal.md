## Why

The API controllers (`BookingsController`, `ListingsController`, `ReviewsController`, `UsersController`) bypass the BLL entirely and access `AppDbContext` directly, causing business rules (self-booking prevention, review state validation) to be silently skipped for API clients. This creates a split where Web MVC enforces correctness but the API does not.

## What Changes

- Replace direct `AppDbContext` injection in `AgriMarket.Api` controllers (`BookingsController`, `ListingsController`, `ReviewsController`, `UsersController`) with their corresponding BLL service interfaces
- Remove business logic duplicated inline in API controllers (ownership checks, status transitions, entity construction)
- Ensure the self-booking guard (`BookingService.CreateAsync`) is enforced for API booking creation — it currently is not
- Ensure review creation via API enforces booking state validation (`ReviewService.CreateAsync`) — it currently does not
- Fix `DashboardService.GetDashboardStatsAsync` to aggregate counts/sums in the database instead of loading full entity tables into memory
- Move inline `Booking` entity construction out of `Client/ListingsController.Book()` — the controller builds the entity and then passes it to the service, bypassing any future service-level defaults

## Capabilities

### New Capabilities

_(none — no new product capabilities are introduced)_

### Modified Capabilities

- `booking-authz`: Self-booking prevention must be enforced uniformly across both the Web and API surfaces; the API currently skips this rule
- `reviews-api`: Review creation must validate that the associated booking is in an eligible state (`ClientConfirmed` or `ProviderCompleted`) regardless of surface — currently skipped in the API controller
- `admin-dashboard`: Dashboard stats aggregation must be performed in the database (GROUP BY / COUNT / SUM) rather than loading entire entity collections into application memory

## Impact

- `AgriMarket.Api/Controllers/BookingsController.cs` — refactored to use `IBookingService`
- `AgriMarket.Api/Controllers/ListingsController.cs` — refactored to use `IListingService`
- `AgriMarket.Api/Controllers/ReviewsController.cs` — refactored to use `IReviewService`
- `AgriMarket.Api/Controllers/UsersController.cs` — refactored to use `IUserService`
- `AgriMarket.Api/Program.cs` — must register BLL services via `BllServiceExtensions`
- `AgriMarket.BLL/Services/ReviewService.cs` — verify `CreateAsync` and `GetByBookingAsync` cover API needs; expose any missing methods on `IReviewService`
- `AgriMarket.BLL/Services/DashboardService.cs` — rewrite stats queries to aggregate in DB
- `AgriMarket.Web/Areas/Client/Controllers/ListingsController.cs` — remove inline `Booking` construction; delegate fully to `IBookingService`
- No breaking API contract changes — endpoints, routes, and response shapes remain identical
