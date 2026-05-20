## 1. ViewModels and Mappers

- [ ] 1.1 Create `CreateReviewViewModel` with properties: `BookingId` (Guid), `Rating` (int, range 1-5), `Comment` (string, optional), `BookingTitle` (string, display-only for the form header).
- [ ] 1.2 Create `EditReviewViewModel` with properties: `ReviewId` (Guid), `BookingId` (Guid), `Rating` (int, range 1-5), `Comment` (string, optional), `BookingTitle` (string, display-only).
- [ ] 1.3 Create `DeleteReviewViewModel` with properties: `ReviewId` (Guid), `BookingId` (Guid), `Rating` (int), `Comment` (string), `ReviewerName` (string), `CreatedAt` (DateTime) for confirmation display.
- [ ] 1.4 Create `ReviewViewModel` with properties: `Id` (Guid), `BookingId` (Guid), `Rating` (int), `Comment` (string), `ReviewerName` (string), `CreatedAt` (DateTime) for displaying a single review.
- [ ] 1.5 Create `ReviewListViewModel` with properties: `Reviews` (list of `ReviewViewModel`), `ProfileId` (Guid), `ProviderName` (string), `AverageRating` (double), `ReviewCount` (int), `CurrentPage` (int), `TotalPages` (int).
- [ ] 1.6 Create `RatingStatsViewModel` with properties: `AverageRating` (double), `ReviewCount` (int) for embedding in listing ViewModels.
- [ ] 1.7 Create `ReviewViewModelMapper` static class with methods: `ToCreateDto(CreateReviewViewModel) -> CreateReviewDto`, `ToUpdateDto(EditReviewViewModel) -> UpdateReviewDto`, `ToViewModel(ReviewDto) -> ReviewViewModel`, `ToEditViewModel(ReviewDto, string bookingTitle) -> EditReviewViewModel`, `ToDeleteViewModel(ReviewDto) -> DeleteReviewViewModel`, `ToRatingStatsViewModel(RatingStatsDto) -> RatingStatsViewModel`.
- [ ] 1.8 Create `ToReviewListViewModel` mapper method that maps a paginated collection of `ReviewDto` plus `RatingStatsDto` to `ReviewListViewModel`.

> **GIT COMMIT:** `feat: add review ViewModels and mapper`

## 2. Controller

- [ ] 2.1 Create `Areas/Client/Controllers/ReviewsController` with constructor injection of `IReviewService` and `IStringLocalizer<SharedResource>`. Apply `[Area("Client")]` and `[Authorize(Policy = "ClientOnly")]`.
- [ ] 2.2 Implement `Create` GET action accepting `bookingId` query parameter. Load booking context for the form header and return `CreateReviewViewModel`.
- [ ] 2.3 Implement `Create` POST action accepting `CreateReviewViewModel`. Map to `CreateReviewDto`, call `IReviewService.CreateAsync`, handle `BusinessRuleException` by redirecting with localized error message, redirect to `Bookings/Details/{bookingId}` on success.
- [ ] 2.4 Implement `Edit` GET action accepting `id` (reviewId). Load review via `IReviewService.GetByIdAsync`, verify ownership, map to `EditReviewViewModel`, return view.
- [ ] 2.5 Implement `Edit` POST action accepting `EditReviewViewModel`. Map to `UpdateReviewDto`, call `IReviewService.UpdateAsync`, handle exceptions, redirect to `Bookings/Details/{bookingId}` on success.
- [ ] 2.6 Implement `Delete` GET action accepting `id` (reviewId). Load review, verify ownership, map to `DeleteReviewViewModel`, return confirmation view.
- [ ] 2.7 Implement `Delete` POST action accepting `id` (reviewId). Call `IReviewService.DeleteAsync`, handle exceptions, redirect to `Bookings/Details/{bookingId}` on success.
- [ ] 2.8 Implement `ForProvider` GET action accepting `profileId` and optional `page` parameter. Call `IReviewService.GetByProfileAsync` and `GetRatingStatsForProfileAsync`, map to `ReviewListViewModel`, return view.

> **GIT COMMIT:** `feat: implement ReviewsController with CRUD and provider listing`

## 3. Views

- [ ] 3.1 Create `Areas/Client/Views/Reviews/Create.cshtml` with rating selector (1-5 stars/radio buttons), optional comment textarea, hidden `BookingId`, validation summary, and submit button. Use `IStringLocalizer<SharedResource>` for all labels.
- [ ] 3.2 Create `Areas/Client/Views/Reviews/Edit.cshtml` with pre-populated rating selector, comment textarea, hidden `ReviewId` and `BookingId`, validation summary, and submit button.
- [ ] 3.3 Create `Areas/Client/Views/Reviews/Delete.cshtml` with review details display (rating, comment, reviewer, date), confirmation message, and confirm/cancel buttons.
- [ ] 3.4 Create `Areas/Client/Views/Reviews/ForProvider.cshtml` with provider name header, average rating display, paginated review list (reviewer name, rating, comment, date), empty-state message, and pagination controls.
- [ ] 3.5 Create `Areas/Client/Views/Shared/_ReviewSection.cshtml` partial view that conditionally renders: (a) review creation form link when booking is `ClientConfirmed` and no review exists, or (b) existing review display with edit/delete links when a review exists.
- [ ] 3.6 Create `Areas/Client/Views/Shared/_RatingBadge.cshtml` partial view that renders a compact rating display (star icon, average rating, review count) for use on listing cards and detail pages.

> **GIT COMMIT:** `feat: add review views and partials`

## 4. Update Existing Views

- [ ] 4.1 Update `Areas/Client/Views/Bookings/Details.cshtml` to include the `_ReviewSection` partial when the booking is in `ClientConfirmed` status. Pass the booking's review (if any) and booking ID to the partial.
- [ ] 4.2 Update `Areas/Client/Views/Listings/Details.cshtml` to include the `_RatingBadge` partial showing the listing's average rating and review count. Add a link to the provider's reviews page (`/Client/Reviews/ForProvider/{profileId}`).
- [ ] 4.3 Update `Areas/Client/Views/Listings/Index.cshtml` to include the `_RatingBadge` partial on each listing card showing the listing's average rating and review count.
- [ ] 4.4 Update `BookingsController.Details` action to load the booking's review (if any) via `IReviewService.GetByBookingAsync` and include it in the booking details ViewModel.
- [ ] 4.5 Update `ListingsController.Details` action to load rating stats via `IReviewService.GetRatingStatsForListingAsync` and include them in the listing details ViewModel.
- [ ] 4.6 Update `ListingsController.Index` action to load rating stats for each listing and include them in the listing summary ViewModels.

> **GIT COMMIT:** `feat: integrate reviews into bookings and listings views`

## 5. Localization

- [ ] 5.1 Add English resx keys to `SharedResource.en.resx`: `Reviews_Create_Title`, `Reviews_Create_Submit`, `Reviews_Edit_Title`, `Reviews_Edit_Submit`, `Reviews_Delete_Title`, `Reviews_Delete_Confirm`, `Reviews_Delete_Cancel`, `Reviews_Rating_Label`, `Reviews_Comment_Label`, `Reviews_Comment_Placeholder`, `Reviews_NoReviews`, `Reviews_LeaveReview`, `Reviews_AverageRating`, `Reviews_ReviewCount`, `Reviews_ForProvider_Title`, `Reviews_CreatedAt`, `Reviews_EditLink`, `Reviews_DeleteLink`, `Reviews_Error_BusinessRule`.
- [ ] 5.2 Add Estonian resx keys to `SharedResource.et.resx` with translations for all keys defined in 5.1.

> **GIT COMMIT:** `feat: add review localization strings`

## 6. Tests

- [ ] 6.1 Add unit tests for `ReviewViewModelMapper`: verify all mapping methods produce correct output for typical inputs, null/optional fields, and edge cases (rating boundary values 1 and 5).
- [ ] 6.2 Add unit tests for `ReviewsController.Create` POST: verify redirect on success, verify redirect with error on `BusinessRuleException`, verify form redisplay on invalid model state.
- [ ] 6.3 Add unit tests for `ReviewsController.Edit` POST: verify redirect on success, verify handling of `UnauthorizedBusinessException`, verify form redisplay on invalid model state.
- [ ] 6.4 Add unit tests for `ReviewsController.Delete` POST: verify redirect on success, verify handling of `UnauthorizedBusinessException`, verify 404 on non-existent review.
- [ ] 6.5 Add unit tests for `ReviewsController.ForProvider` GET: verify paginated result mapping, verify empty-state when no reviews exist.
- [ ] 6.6 Add integration test for review lifecycle: create booking -> confirm completion -> create review -> edit review -> delete review, verifying each step through the MVC pipeline.

> **GIT COMMIT:** `test: add ReviewsController and mapper unit tests`
