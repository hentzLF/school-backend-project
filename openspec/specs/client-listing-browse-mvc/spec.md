# Spec: Client Listing Browse (MVC)

## Purpose
Defines public-facing service listing browse and detail pages in the Client MVC area, accessible to both authenticated and unauthenticated users.

## Requirements

### Requirement: Anyone can browse active listings
The system SHALL provide a public listing page at `/Client/Listings` accessible to both authenticated and unauthenticated users. The page SHALL display active `ServiceListing` records with key summary fields (title, category, provider, and price-per-hectare) and SHALL support functional pagination or bounded result display to keep response and render time predictable. No `[Authorize]` attribute is applied to the listings index or details actions. The controller SHALL call `IListingService.GetActiveListingsAsync()` which returns `IEnumerable<ListingSummaryDto>`, and map the DTOs to `ListingIndexItemViewModel` for rendering via Web mapper modules. The controller SHALL NOT access entity navigation properties directly.

#### Scenario: Unauthenticated user browses listings
- **WHEN** an unauthenticated user navigates to `/Client/Listings`
- **THEN** the system displays active service listings with summary information and links to details without requiring login

#### Scenario: Authenticated client browses listings
- **WHEN** an authenticated client navigates to `/Client/Listings`
- **THEN** the system displays active service listings with summary information and links to details

#### Scenario: Browse listings returns DTOs mapped to ViewModels
- **WHEN** any user navigates to `/Client/Listings`
- **THEN** the controller receives `ListingSummaryDto` items from the BLL service and maps them to `ListingIndexItemViewModel` with title, category name, provider name, and price

#### Scenario: No active listings available
- **WHEN** any user navigates to `/Client/Listings` and no active listings exist
- **THEN** the system displays an empty-state message and no booking actions

### Requirement: Anyone can view listing details
The system SHALL provide a listing detail page at `/Client/Listings/Details/{id}` accessible to both authenticated and unauthenticated users. The page SHALL display listing information needed to make a booking decision (description, pricing, available metadata). The booking action entry point SHALL only be visible to authenticated users; unauthenticated users SHALL see a prompt to log in before booking. The controller SHALL call `IListingService.GetByIdAsync(id)` which returns a `ListingDto`, and map it to `ListingDetailsViewModel` via Web mapper modules. Availability data SHALL be included in the `ListingDto` as nested `AvailabilityDto` items.

#### Scenario: Unauthenticated user views listing details
- **WHEN** an unauthenticated user opens `/Client/Listings/Details/{id}` for an active listing
- **THEN** the system displays listing details and shows a login prompt in place of the booking action

#### Scenario: Authenticated client views listing details
- **WHEN** an authenticated client opens `/Client/Listings/Details/{id}` for an active listing
- **THEN** the system displays listing details and the booking action entry point

#### Scenario: View listing details returns DTO mapped to ViewModel
- **WHEN** any user navigates to `/Client/Listings/Details/{id}` for an active listing
- **THEN** the controller receives a `ListingDto` with availability data from the BLL service and maps it to `ListingDetailsViewModel`

#### Scenario: Listing not found or inactive
- **WHEN** any user opens `/Client/Listings/Details/{id}` for a non-existent or inactive listing
- **THEN** the system returns not found behavior and does not expose booking actions
