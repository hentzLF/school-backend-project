## 1. Bug fixes

- [x] 1.1 Add `ReviewedProfileId` to `ReviewDto` and update `ToReviewDto` mapping in `ReviewService`
- [x] 1.2 Add duplicate review check (`reviews.AnyAsync`) in `ReviewService.CreateAsync` before entity creation
- [x] 1.3 Change participation check to client-only check in `ReviewService.CreateAsync` — block provider reviews
- [x] 1.4 Update existing tests for changed error messages and new validation behavior
- [x] 1.5 Commit: `fix: address review system bugs (ReviewedProfileId, duplicate check, client-only reviews)`

## 2. Update and Delete DTOs

- [x] 2.1 Create `UpdateReviewDto` in `AgriMarket.BLL/Dtos/Reviews/`
- [x] 2.2 Create `ReviewApiMapper` in `AgriMarket.Api/Mappers/` with `WithRouteId` extension method

## 3. Update and Delete service methods

- [x] 3.1 Add `UpdateAsync(Guid userId, UpdateReviewDto dto)` to `IReviewService` and `ReviewService`
- [x] 3.2 Add `DeleteAsync(Guid userId, Guid reviewId)` to `IReviewService` and `ReviewService`
- [x] 3.3 Commit: `feat: add review update and delete operations`

## 4. Query service methods

- [x] 4.1 Add `GetByProfileAsync(Guid profileId, int page, int pageSize)` to `IReviewService` and `ReviewService`

## 5. Controller endpoints

- [x] 5.1 Add `PUT /{id}` endpoint to `ReviewsController` (Update, [Authorize])
- [x] 5.2 Add `DELETE /{id}` endpoint to `ReviewsController` (Delete, [Authorize])
- [x] 5.3 Add `GET /booking/{bookingId}` endpoint to `ReviewsController` (GetByBooking, public)
- [x] 5.4 Add `GET /profile/{profileId}` endpoint to `ReviewsController` (GetByProfile, public, paginated)
- [x] 5.5 Commit: `feat: add review query and CRUD controller endpoints`

## 6. Rating statistics

- [x] 6.1 Create `RatingStatsDto` in `AgriMarket.BLL/Dtos/Reviews/`
- [x] 6.2 Add `GetRatingStatsForProfileAsync(Guid profileId)` to `IReviewService` and `ReviewService`
- [x] 6.3 Add `GetRatingStatsForListingAsync(Guid listingId)` to `IReviewService` and `ReviewService`

## 7. DTO extensions and service integration

- [x] 7.1 Add `AverageRating` and `ReviewCount` fields to `ListingDto` and `ListingSummaryDto`
- [x] 7.2 Add `AverageRating` and `ReviewCount` fields to `UserProfileDto`
- [x] 7.3 Inject `IReviewService` into `ListingService` and create async DTO builders with stats
- [x] 7.4 Inject `IReviewService` into `UserService` and create async DTO builder with stats
- [x] 7.5 Update existing `ListingService` and `UserService` tests for new constructor parameter
- [x] 7.6 Commit: `feat: add rating statistics to listing and user profile DTOs`

## 8. Tests

- [x] 8.1 Add unit tests for bug fixes (duplicate review, provider block) in `ReviewServiceTests`
- [x] 8.2 Add unit tests for `UpdateAsync` (owner success, not found, not owner, no profile) in `ReviewServiceTests`
- [x] 8.3 Add unit tests for `DeleteAsync` (owner success, not found, not owner, no profile) in `ReviewServiceTests`
- [x] 8.4 Add unit tests for `GetByProfileAsync` in `ReviewServiceTests`
- [x] 8.5 Add unit tests for rating stats methods in `ReviewServiceTests`
- [x] 8.6 Add integration tests for Update, Delete, duplicate, provider block in `ReviewApiTests`
- [x] 8.7 Add integration tests for GetByProfile and rating stats in `ReviewApiTests`
- [x] 8.8 Commit: `test: add comprehensive review system tests`

## 9. Verification

- [x] 9.1 Run `dotnet build` — verify full solution compiles
- [x] 9.2 Run `dotnet test` — verify all tests pass (old + new)
