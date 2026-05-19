## Why

The Client MVC area currently has no way for clients to leave or view reviews, despite `IReviewService` being fully implemented in the BLL with complete CRUD, query, and rating statistics support. Reviews are the final step of the booking lifecycle: after a client confirms completion (`ClientConfirmed`), they should be able to rate and comment on the service they received. Without a review UI, the booking journey is incomplete and provider reputation data cannot be surfaced to other clients browsing listings.

## What Changes

- Add a new `ReviewsController` in the Client MVC area with actions for creating, editing, and deleting reviews.
- Embed a review creation form in the Bookings/Details view when the booking is in `ClientConfirmed` status and no review exists yet; display the existing review when one is present.
- Show provider rating statistics (average rating and review count) on listing details pages and listing index cards.
- Add ViewModels, manual mappers, and Razor views for the review workflow.
- Add localization keys (EN + ET) for all review-related UI strings.

## Capabilities

### New Capabilities
- `client-review-submission`: Client can create, edit, and delete reviews for completed bookings via the Client MVC area.
- `client-review-display`: Provider rating statistics are displayed on listing pages, and reviews are listed on a provider's review page.

### Modified Capabilities
- `client-booking-management-mvc`: Booking details page now includes a review section — shows a review creation form when the booking is `ClientConfirmed` and no review exists, or displays the existing review with edit/delete actions.

## Impact

- Affected web project: `AgriMarket.Web` (new `Areas/Client/Controllers/ReviewsController`, new ViewModels, new Views, updated Bookings/Details and Listings views, new resx keys).
- Affected OpenSpec coverage: two new capability specs (`client-review-submission`, `client-review-display`) and one delta spec for `client-booking-management-mvc`.
- No database migration required — `IReviewService` and all underlying data access already exist.
- No API contract changes — this change is purely MVC/web surface.
