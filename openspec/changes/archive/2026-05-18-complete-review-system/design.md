## Context

The AgriMarket backend has a partially implemented review system. The `Review` entity exists with proper DB relationships (1:1 with Booking, FK to ReviewerProfile and ReviewedProfile). `ReviewService` has basic Create/Read operations and `ReviewsController` exposes 3 endpoints (GET list, GET by ID, POST create). However, the system has 3 bugs and lacks Update/Delete, filtering endpoints, and rating statistics integration.

Current service dependencies:
- `ReviewService` → `IRepository<Review>`, `IRepository<UserProfile>`, `IBookingRepository`, `IUnitOfWork`, `IQueryMaterializer`
- `ListingService` → `IListingRepository`, `IRepository<UserProfile>`, `IRepository<Booking>`, `IAvailabilityRepository`, `IUnitOfWork`
- `UserService` → `IAppUserRepository`, `IUserProfileRepository`, `IRepository<ProfileRole>`, `IUnitOfWork`, + several repos for cascade delete

EF Core repository pattern: `EfRepository<T>` uses `DbSet<T>` directly (no `AsNoTracking`), so entities loaded via `FirstOrDefaultAsync` are tracked — direct property assignment works for updates without calling `Update()`.

## Goals / Non-Goals

**Goals:**
- Fix all 3 existing bugs (missing ReviewedProfileId, duplicate check, provider self-review)
- Add Update and Delete operations with ownership validation
- Add per-booking and per-profile query endpoints
- Add rating statistics (AverageRating, ReviewCount) to listing and user profile DTOs
- Achieve test coverage for all new functionality

**Non-Goals:**
- Review moderation/dispute system
- Admin-specific review management endpoints
- Caching of rating statistics
- Batch rating stats queries (N+1 is acceptable at current scale)
- Bidirectional reviews (provider reviewing client)
- Review response/reply system

## Decisions

### D1: Rating stats via IReviewService injection (not direct repository access)

`ListingService` and `UserService` will depend on `IReviewService` for rating statistics rather than accessing `IRepository<Review>` directly.

**Why**: Keeps aggregation logic in one place (ReviewService). If calculation logic changes (e.g., weighted ratings), only ReviewService changes. `UserService` already has `IRepository<Review>` for cascade delete — using `IReviewService` for reads cleanly separates the concerns.

**Alternative considered**: Direct `IRepository<Review>` queries in ListingService/UserService. Rejected because it duplicates aggregation logic.

**Circular dependency check**: No cycle exists. ReviewService depends on IRepository/IBookingRepository. ListingService/UserService depend on IReviewService. No reverse dependency.

### D2: N+1 queries for listing rating stats (accept for now)

When listing summaries are loaded, each listing triggers a separate `GetRatingStatsForListingAsync` call. This is an N+1 pattern.

**Why**: Matches existing project patterns (no batch operations exist elsewhere). The listing pages are paginated (max 100 items). At current scale, N+1 with simple COUNT/SUM queries is acceptable.

**Migration path**: If performance degrades, add `GetRatingStatsForListingsAsync(IEnumerable<Guid>)` with a single GroupBy query. No API changes needed.

### D3: Static-to-instance conversion for DTO mapping methods

`ListingService.ToListingDto` and `UserService.ToUserProfileDto` are currently `static` methods. They must become instance methods (or async wrappers) to access `IReviewService`.

**Why**: The mapping methods need to call `await reviewService.GetRatingStatsForListingAsync()`. Static methods cannot access instance fields.

**Approach**: Create async wrapper methods (`BuildListingDtoAsync`, `BuildListingSummaryDtoAsync`, `BuildUserProfileDtoAsync`) that call the static mapper for non-stats fields, then add stats. This preserves the static mappers for any test utilities while adding the async layer.

### D4: Ownership validation pattern for Update/Delete

Follow the `ListingService.UpdateAsync` pattern: resolve `UserProfile` from `userId` → load entity → check `ReviewerProfileId == profile.Id` → throw `BusinessRuleException` on mismatch.

**Why**: Consistent with existing codebase patterns. Returns 403-style errors via BusinessRuleException (caught in controller as 403).

### D5: GetByBooking returns single ReviewDto, not collection

The 1:1 unique constraint (BookingId) guarantees at most one review per booking. The endpoint returns a single `ReviewDto` or 404, not a paginated collection.

**Why**: More intuitive API design. The existing `GetByBookingAsync` service method returns `IEnumerable` but the controller can call `.FirstOrDefault()` and return a single item.

## Risks / Trade-offs

**[Risk] N+1 queries on listing pages** → Acceptable at current scale (max 100 per page). Mitigation: pagination caps query count. Future: batch method if needed.

**[Risk] Breaking change in DTO shapes** → Adding `AverageRating` and `ReviewCount` to existing DTOs changes the API response shape. Mitigation: fields are additive (new fields with default values), so existing consumers are unlikely to break.

**[Risk] ListingService/UserService constructor changes break existing tests** → Tests that construct these services will need the new `IReviewService` parameter. Mitigation: Pass `Mock<IReviewService>` with default returns in existing tests.

**[Risk] EF tracking assumption for UpdateAsync** → `FirstOrDefaultAsync` returns tracked entities because `EfRepository` doesn't use `AsNoTracking`. If this changes, updates would silently fail. Mitigation: Low risk — this is a core design choice of the repository, consistent with how `ListingService.UpdateAsync` works today.
