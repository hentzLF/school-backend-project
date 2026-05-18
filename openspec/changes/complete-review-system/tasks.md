## 1. Bug fixes

- [ ] 1.1 Add `ReviewedProfileId` to `ReviewDto` and update `ToReviewDto` mapping in `ReviewService`
- [ ] 1.2 Add duplicate review check (`reviews.AnyAsync`) in `ReviewService.CreateAsync` before entity creation
- [ ] 1.3 Change participation check to client-only check in `ReviewService.CreateAsync` — block provider reviews
- [ ] 1.4 Update existing tests for changed error messages and new validation behavior

## 2. Update and Delete DTOs

- [ ] 2.1 Create `UpdateReviewDto` in `AgriMarket.BLL/Dtos/Reviews/`
- [ ] 2.2 Create `ReviewApiMapper` in `AgriMarket.Api/Mappers/` with `WithRouteId` extension method

## 3. Update and Delete service methods

- [ ] 3.1 Add `UpdateAsync(Guid userId, UpdateReviewDto dto)` to `IReviewService` and `ReviewService`
- [ ] 3.2 Add `DeleteAsync(Guid userId, Guid reviewId)` to `IReviewService` and `ReviewService`

## 4. Query service methods

- [ ] 4.1 Add `GetByProfileAsync(Guid profileId, int page, int pageSize)` to `IReviewService` and `ReviewService`

## 5. Controller endpoints

- [ ] 5.1 Add `PUT /{id}` endpoint to `ReviewsController` (Update, [Authorize])
- [ ] 5.2 Add `DELETE /{id}` endpoint to `ReviewsController` (Delete, [Authorize])
- [ ] 5.3 Add `GET /booking/{bookingId}` endpoint to `ReviewsController` (GetByBooking, public)
- [ ] 5.4 Add `GET /profile/{profileId}` endpoint to `ReviewsController` (GetByProfile, public, paginated)

## 6. Rating statistics

- [ ] 6.1 Create `RatingStatsDto` in `AgriMarket.BLL/Dtos/Reviews/`
- [ ] 6.2 Add `GetRatingStatsForProfileAsync(Guid profileId)` to `IReviewService` and `ReviewService`
- [ ] 6.3 Add `GetRatingStatsForListingAsync(Guid listingId)` to `IReviewService` and `ReviewService`

## 7. DTO extensions and service integration

- [ ] 7.1 Add `AverageRating` and `ReviewCount` fields to `ListingDto` and `ListingSummaryDto`
- [ ] 7.2 Add `AverageRating` and `ReviewCount` fields to `UserProfileDto`
- [ ] 7.3 Inject `IReviewService` into `ListingService` and create async DTO builders with stats
- [ ] 7.4 Inject `IReviewService` into `UserService` and create async DTO builder with stats
- [ ] 7.5 Update existing `ListingService` and `UserService` tests for new constructor parameter

## 8. Tests

- [ ] 8.1 Add unit tests for bug fixes (duplicate review, provider block) in `ReviewServiceTests`
- [ ] 8.2 Add unit tests for `UpdateAsync` (owner success, not found, not owner, no profile) in `ReviewServiceTests`
- [ ] 8.3 Add unit tests for `DeleteAsync` (owner success, not found, not owner, no profile) in `ReviewServiceTests`
- [ ] 8.4 Add unit tests for `GetByProfileAsync` in `ReviewServiceTests`
- [ ] 8.5 Add unit tests for rating stats methods in `ReviewServiceTests`
- [ ] 8.6 Add integration tests for Update, Delete, duplicate, provider block in `ReviewApiTests`
- [ ] 8.7 Add integration tests for GetByProfile and rating stats in `ReviewApiTests`

## 9. Verification

- [ ] 9.1 Run `dotnet build` — verify full solution compiles
- [ ] 9.2 Run `dotnet test` — verify all tests pass (old + new)
