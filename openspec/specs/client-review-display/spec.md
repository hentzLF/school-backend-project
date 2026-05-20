# client-review-display Specification

## Purpose
TBD - created by archiving change client-reviews-mvc. Update Purpose after archive.
## Requirements
### Requirement: Provider rating stats displayed on listing details page
The system SHALL display the provider's average rating and review count on the listing details page at `/Client/Listings/Details/{id}`. The controller SHALL call `IReviewService.GetRatingStatsForListingAsync(listingId)` and include the result in the listing details ViewModel.

#### Scenario: Listing has reviews
- **WHEN** a user views `/Client/Listings/Details/{id}` for a listing whose bookings have received reviews
- **THEN** the page displays the listing's average rating (rounded to one decimal) and total review count

#### Scenario: Listing has no reviews
- **WHEN** a user views `/Client/Listings/Details/{id}` for a listing with no reviews
- **THEN** the page displays a "No reviews yet" indicator instead of rating stats

### Requirement: Provider rating stats displayed on listing index cards
The system SHALL display the average rating and review count on each listing card in the listing index at `/Client/Listings`. The controller SHALL retrieve rating stats for each listing and include them in the listing summary ViewModel.

#### Scenario: Listing card with reviews
- **WHEN** a user views `/Client/Listings` and a listing has received reviews
- **THEN** the listing card displays the average rating and review count as a compact badge

#### Scenario: Listing card with no reviews
- **WHEN** a user views `/Client/Listings` and a listing has no reviews
- **THEN** the listing card displays a "No reviews" label or omits the rating badge

### Requirement: Reviews listed on provider review page
The system SHALL provide a page at `/Client/Reviews/ForProvider/{profileId}` displaying a paginated list of reviews for a given provider profile. The controller SHALL call `IReviewService.GetByProfileAsync(profileId, page, pageSize)` and map results to `ReviewListViewModel`. This page SHALL be publicly accessible within the Client area (no authentication required beyond Client area access).

#### Scenario: Provider has reviews
- **WHEN** a user navigates to `/Client/Reviews/ForProvider/{profileId}` for a provider with reviews
- **THEN** the page displays a paginated list of reviews showing reviewer name, rating, comment, and date

#### Scenario: Provider has no reviews
- **WHEN** a user navigates to `/Client/Reviews/ForProvider/{profileId}` for a provider with no reviews
- **THEN** the page displays an empty-state message indicating no reviews exist

#### Scenario: Pagination
- **WHEN** a user navigates to `/Client/Reviews/ForProvider/{profileId}?page=2` and the provider has more reviews than one page
- **THEN** the page displays the second page of reviews with pagination controls

