# Capability: provider-listing-management

## Purpose
TBD

## Requirements

### Requirement: Provider can view own listings
The system SHALL provide a listing management index at `/Client/MyListings` that displays only the `ServiceListing` records owned by the authenticated Provider's `UserProfile`. Each item SHALL show title, category, price-per-hectare, and active status.

#### Scenario: View own listings
- **WHEN** an authenticated Provider navigates to `/Client/MyListings`
- **THEN** the system displays only listings whose `UserProfileId` matches the authenticated Provider's profile, with title, category, price-per-hectare, and active status

#### Scenario: No listings for provider
- **WHEN** an authenticated Provider navigates to `/Client/MyListings` and owns no listings
- **THEN** the system displays an empty-state message and a prompt to create the first listing

#### Scenario: Provider cannot see other providers' listings
- **WHEN** an authenticated Provider navigates to `/Client/MyListings`
- **THEN** the system does not display listings owned by other Provider profiles

### Requirement: Provider can create a listing
The system SHALL allow an authenticated Provider to create a new listing via a form at `/Client/MyListings/Create`. The controller SHALL map the `MyListingCreateViewModel` to a `CreateListingDto` via Web mapper modules and call `IListingService.CreateAsync(userId, dto)`. The controller SHALL NOT construct a `ServiceListing` entity directly.

#### Scenario: Create listing successfully
- **WHEN** an authenticated Provider submits valid listing data
- **THEN** the controller maps the ViewModel to `CreateListingDto`, the BLL service constructs the entity with `IsActive = false`, and the system redirects to the listing detail page

#### Scenario: Invalid listing input rejected
- **WHEN** an authenticated Provider submits listing data with missing required fields
- **THEN** the system redisplays the create form with validation errors and does not call the BLL service

### Requirement: Provider can view own listing details
The system SHALL provide a listing detail page at `/Client/MyListings/Details/{id}` showing full listing information for listings owned by the authenticated Provider.

#### Scenario: View own listing details
- **WHEN** an authenticated Provider opens `/Client/MyListings/Details/{id}` for a listing they own
- **THEN** the system displays title, description, category, price-per-hectare, active status, and total booking count

#### Scenario: Access denied for non-owned listing
- **WHEN** an authenticated Provider opens `/Client/MyListings/Details/{id}` for a listing they do not own
- **THEN** the system returns 404 and does not reveal listing details

### Requirement: Provider can edit own listing
The system SHALL allow an authenticated Provider to edit their own listings. The controller SHALL map the `MyListingEditViewModel` to an `UpdateListingDto` via Web mapper modules and call `IListingService.UpdateAsync(userId, dto)`. Ownership verification SHALL be performed by the BLL service.

#### Scenario: Edit listing successfully
- **WHEN** an authenticated Provider submits valid changes to a listing they own
- **THEN** the controller maps the ViewModel to `UpdateListingDto`, the BLL service verifies ownership and persists changes, and the system redirects to the listing detail page

#### Scenario: Edit rejected for invalid input
- **WHEN** an authenticated Provider submits invalid data (e.g., empty title, negative price)
- **THEN** the system redisplays the edit form with validation errors and does not persist changes

#### Scenario: Edit blocked for non-owned listing
- **WHEN** an authenticated Provider attempts to edit a listing they do not own
- **THEN** the BLL service throws `BusinessRuleException` and the controller returns 404

### Requirement: Provider can delete own listing
The system SHALL allow an authenticated Provider to delete their own listings, provided no active bookings exist. The controller SHALL call `IListingService.DeleteAsync(userId, listingId)`. The active-bookings check SHALL be performed by the BLL service.

#### Scenario: Delete listing with no active bookings
- **WHEN** an authenticated Provider confirms deletion of a listing they own with no active bookings
- **THEN** the BLL service deletes the listing and the system redirects to the listing index

#### Scenario: Delete blocked when active bookings exist
- **WHEN** an authenticated Provider attempts to delete a listing that has active bookings
- **THEN** the BLL service throws `BusinessRuleException` and the controller displays an error message

#### Scenario: Delete blocked for non-owned listing
- **WHEN** an authenticated Provider attempts to delete a listing they do not own
- **THEN** the system returns 404

### Requirement: Provider can toggle listing active status
The system SHALL allow an authenticated Provider to activate or deactivate their own listings from the listing detail page.

#### Scenario: Toggle listing active status
- **WHEN** an authenticated Provider toggles the active status of a listing they own
- **THEN** the system flips `IsActive` and redirects back to the listing detail page

### Requirement: Web controllers do not construct domain entities
No controller in `AgriMarket.Web` SHALL directly instantiate `ServiceListing`, `Booking`, `Availability`, or any other domain entity. Entity construction SHALL be delegated to BLL services via DTOs.

#### Scenario: No entity construction in Web controllers
- **WHEN** the Web project is compiled
- **THEN** no controller file contains `new ServiceListing`, `new Booking`, or `new Availability`
