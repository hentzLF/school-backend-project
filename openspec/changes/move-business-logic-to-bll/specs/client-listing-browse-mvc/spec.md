## MODIFIED Requirements

### Requirement: Anyone can browse active listings
The system SHALL provide a public listing page at `/Client/Listings` that displays active listings. The controller SHALL call `IListingService.GetActiveListingsAsync()` which returns `IEnumerable<ListingSummaryDto>`, and map the DTOs to `ListingIndexItemViewModel` for rendering. The controller SHALL NOT access entity navigation properties directly.

#### Scenario: Browse listings returns DTOs mapped to ViewModels
- **WHEN** any user navigates to `/Client/Listings`
- **THEN** the controller receives `ListingSummaryDto` items from the BLL service and maps them to `ListingIndexItemViewModel` with title, category name, provider name, and price

### Requirement: Anyone can view listing details
The system SHALL provide a listing detail page at `/Client/Listings/Details/{id}` that displays listing information. The controller SHALL call `IListingService.GetByIdAsync(id)` which returns a `ListingDto`, and map it to `ListingDetailsViewModel`. Availability data SHALL be included in the `ListingDto` as nested `AvailabilityDto` items.

#### Scenario: View listing details returns DTO mapped to ViewModel
- **WHEN** any user navigates to `/Client/Listings/Details/{id}` for an active listing
- **THEN** the controller receives a `ListingDto` with availability data from the BLL service and maps it to `ListingDetailsViewModel`
