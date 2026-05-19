## Context

`AgriMarket.BLL` already provides a complete `IReviewService` with CRUD operations (`CreateAsync`, `UpdateAsync`, `DeleteAsync`), query methods (`GetByIdAsync`, `GetByBookingAsync`, `GetByProfileAsync`), and rating statistics (`GetRatingStatsForProfileAsync`, `GetRatingStatsForListingAsync`). The REST API layer exposes these via `ReviewsController` in the API project. However, the Client MVC area has no review functionality — clients cannot leave, view, edit, or delete reviews through the web interface, and provider ratings are not displayed on listing pages.

The booking lifecycle ends at `ClientConfirmed` status, after which the client should be able to leave a review. The `CreateReviewDto` requires a `BookingId`, `Rating` (1-5), and optional `Comment`. The BLL enforces that only the booking client can create a review, the booking must be in a completed state, and duplicate reviews for the same booking are rejected.

Constraints:
- Follow existing Client area conventions: ViewModels (no ViewBag/ViewData), `IStringLocalizer<SharedResource>` for i18n, manual mappers, area routing (`/Client/{controller}/{action}`).
- Reuse `IReviewService` directly — no new BLL methods needed.
- Keep controller actions thin: delegate all business logic to `IReviewService`.

## Goals / Non-Goals

**Goals:**
- Allow clients to create a review for a completed booking directly from the booking details page.
- Allow clients to edit and delete their own reviews.
- Display provider rating statistics (average rating, review count) on listing index cards and listing details pages.
- Show paginated reviews for a provider on a dedicated reviews page.
- Localize all review UI strings in English and Estonian.

**Non-Goals:**
- Admin review moderation UI (already exists in the Admin area).
- Review replies or threaded discussions.
- Photo/media attachments on reviews.
- Review reporting or flagging.
- Email notifications for new reviews.

## Decisions

### Decision: ReviewsController in Client area for CRUD operations
- **Choice:** Create `Areas/Client/Controllers/ReviewsController` with `Create` (GET/POST), `Edit` (GET/POST), and `Delete` (GET/POST) actions.
- **Rationale:** Follows existing Client area controller conventions. Separating review CRUD into its own controller keeps `BookingsController` focused on booking lifecycle and avoids bloating it with review logic.
- **Alternatives considered:**
  - Adding review actions to `BookingsController`: simpler routing but violates single-responsibility and grows an already substantial controller.
  - Using AJAX/partial endpoints instead of full page actions: better UX but inconsistent with the current server-rendered MVC pattern used across the Client area.

### Decision: Review form embedded in Bookings/Details via partial view
- **Choice:** When a booking is in `ClientConfirmed` status and has no existing review, render a review creation partial in the Bookings/Details view. When a review exists, render the review display partial with edit/delete links.
- **Rationale:** The review is contextually tied to a booking — showing it inline reduces navigation friction. Using a partial view keeps the Bookings/Details view clean and the review UI reusable.
- **Alternatives considered:**
  - Separate page for review creation with booking context passed via query string: more navigation steps, worse UX.
  - Modal dialog for review form: inconsistent with the rest of the MVC area which uses full-page forms.

### Decision: Rating stats displayed on Listings/Index and Listings/Details
- **Choice:** Call `IReviewService.GetRatingStatsForListingAsync` when building listing ViewModels and include `AverageRating` and `ReviewCount` in the listing display.
- **Rationale:** Rating information helps clients make informed booking decisions. The BLL already computes these statistics efficiently.
- **Alternatives considered:**
  - Separate API call from JavaScript: adds complexity and is inconsistent with server-rendered approach.
  - Caching rating stats in the listing entity: premature optimization, adds migration complexity.

### Decision: Manual mappers for review ViewModels
- **Choice:** Create a `ReviewViewModelMapper` static class following the existing mapper convention in the project (e.g., `BookingViewModelMapper`).
- **Rationale:** Consistent with existing codebase patterns. The project does not use AutoMapper.
- **Alternatives considered:**
  - Introduce AutoMapper: inconsistent with existing codebase decisions.
  - Inline mapping in controllers: violates separation of concerns and CLAUDE.md conventions.

## Risks / Trade-offs

- **[Risk] Bookings/Details view is also modified by `client-checkout-flow` change** --> **Mitigation:** The checkout flow adds a payment/checkout card section, while this change adds a review section. These are distinct view regions (checkout card appears for pending-payment bookings; review section appears for completed bookings). No structural conflict expected, but integration should be verified when both changes are merged.
- **[Risk] N+1 query when loading rating stats for listing index** --> **Mitigation:** The listing index already loads listings in a single query. Rating stats calls are per-listing, but the index is paginated (typically 10-20 items). If performance becomes an issue, a batch rating stats method can be added to `IReviewService` in a future change.
- **[Trade-off] Review creation requires page navigation to ReviewsController then redirect back** --> **Mitigation:** The Create POST redirects back to `Bookings/Details/{bookingId}` after success, so the user flow feels seamless despite the round-trip.

## Resolved Questions

- **Which booking statuses allow review creation?** `ClientConfirmed` only. The BLL service enforces this via `BusinessRuleException`.
- **Can a review be edited after creation?** Yes, the client who created the review can update rating and comment via `IReviewService.UpdateAsync`.
- **Can a review be deleted?** Yes, the client who created the review can delete it via `IReviewService.DeleteAsync`.
- **Where do reviews link to?** Reviews are created per-booking. Rating stats are aggregated per-provider-profile and per-listing by the BLL.
