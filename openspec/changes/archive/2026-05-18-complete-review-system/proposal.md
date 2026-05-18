## Why

The review system has basic Create + Read functionality but is incomplete for production use. ReviewDto is missing the `ReviewedProfileId` field (consumers can't see who was reviewed), there's no duplicate review protection (DB exception instead of clean error), and providers can review their own bookings (should be client-only). Beyond bugs, the system lacks Update/Delete operations, per-booking and per-profile query endpoints, and rating statistics — making it impossible for the frontend to display provider ratings on listings or profiles.

## What Changes

- **Fix**: Add `ReviewedProfileId` to `ReviewDto` so API consumers know who was reviewed
- **Fix**: Add duplicate review check in service layer (clean `BusinessRuleException` instead of raw SQL exception)
- **Fix**: Block providers from reviewing — only the booking client can leave a review
- **Add**: `UpdateReviewDto` and `PUT /api/reviews/{id}` endpoint for review owners to edit their reviews
- **Add**: `DELETE /api/reviews/{id}` endpoint for review owners to delete their reviews
- **Add**: `GET /api/reviews/booking/{bookingId}` endpoint (service method exists, controller action missing)
- **Add**: `GET /api/reviews/profile/{profileId}` endpoint for paginated reviews by reviewed profile
- **Add**: `RatingStatsDto` with `AverageRating` + `ReviewCount` aggregation methods
- **Add**: `AverageRating` and `ReviewCount` fields to `ListingDto`, `ListingSummaryDto`, and `UserProfileDto` **BREAKING**
- **Add**: Comprehensive unit and integration tests for all new functionality

## Capabilities

### New Capabilities

- `review-crud`: Update and Delete operations for reviews, including ownership validation and new DTO
- `review-queries`: Per-booking and per-profile query endpoints with pagination
- `review-rating-stats`: Rating statistics aggregation (AverageRating, ReviewCount) and integration into Listing and UserProfile DTOs

### Modified Capabilities

- `reviews-api`: Fix ReviewDto shape (add ReviewedProfileId), add duplicate review check, restrict review creation to booking clients only, add new endpoints (PUT, DELETE, GET by booking, GET by profile)

## Impact

- **API contracts**: `ReviewDto` gains `ReviewedProfileId` field (additive). `ListingDto`, `ListingSummaryDto`, `UserProfileDto` gain `AverageRating` + `ReviewCount` fields (additive but changes response shape).
- **Services**: `ReviewService` gets 5 new methods. `ListingService` and `UserService` gain `IReviewService` dependency for rating stats.
- **Business rules**: Provider self-review is blocked (behavior change). Duplicate reviews return clean 422 instead of 500.
- **Files affected**: ~15 files across BLL (DTOs, services, interfaces), API (controller, mapper), and Tests.
