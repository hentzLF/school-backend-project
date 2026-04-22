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
The system SHALL allow an authenticated Provider to create a new `ServiceListing` via a form at `/Client/MyListings/Create`. Required fields are title, category, and price-per-hectare. Description is optional. The listing SHALL be assigned to the authenticated Provider's `UserProfile` and created in an inactive state.

#### Scenario: Create listing successfully
- **WHEN** an authenticated Provider submits valid listing data (title, category, price-per-hectare)
- **THEN** the system creates a `ServiceListing` with `UserProfileId` set to the authenticated Provider's profile, `IsActive` set to false, and redirects to the listing detail page

#### Scenario: Invalid listing input rejected
- **WHEN** an authenticated Provider submits listing data with missing required fields or invalid values
- **THEN** the system redisplays the create form with validation errors and does not create a listing

### Requirement: Provider can view own listing details
The system SHALL provide a listing detail page at `/Client/MyListings/Details/{id}` showing full listing information for listings owned by the authenticated Provider.

#### Scenario: View own listing details
- **WHEN** an authenticated Provider opens `/Client/MyListings/Details/{id}` for a listing they own
- **THEN** the system displays title, description, category, price-per-hectare, active status, and total booking count

#### Scenario: Access denied for non-owned listing
- **WHEN** an authenticated Provider opens `/Client/MyListings/Details/{id}` for a listing they do not own
- **THEN** the system returns 404 and does not reveal listing details

### Requirement: Provider can edit own listing
The system SHALL allow an authenticated Provider to edit the title, description, category, price-per-hectare, and active status of their own listings via `/Client/MyListings/Edit/{id}`.

#### Scenario: Edit listing successfully
- **WHEN** an authenticated Provider submits valid changes to a listing they own
- **THEN** the system persists the changes and redirects to the listing detail page

#### Scenario: Edit rejected for invalid input
- **WHEN** an authenticated Provider submits invalid data (e.g., empty title, negative price)
- **THEN** the system redisplays the edit form with validation errors and does not persist changes

#### Scenario: Edit blocked for non-owned listing
- **WHEN** an authenticated Provider attempts to edit a listing they do not own
- **THEN** the system returns 404

### Requirement: Provider can delete own listing
The system SHALL allow an authenticated Provider to delete their own listings via a confirmation page at `/Client/MyListings/Delete/{id}`, provided no active bookings exist for that listing.

#### Scenario: Delete listing with no active bookings
- **WHEN** an authenticated Provider confirms deletion of a listing they own that has no active bookings
- **THEN** the system permanently removes the listing and redirects to the listing index

#### Scenario: Delete blocked when active bookings exist
- **WHEN** an authenticated Provider attempts to delete a listing that has one or more bookings in a non-terminal status
- **THEN** the system rejects the deletion and displays an error message indicating active bookings prevent deletion

#### Scenario: Delete blocked for non-owned listing
- **WHEN** an authenticated Provider attempts to delete a listing they do not own
- **THEN** the system returns 404

### Requirement: Provider can toggle listing active status
The system SHALL allow an authenticated Provider to activate or deactivate their own listings from the listing detail page.

#### Scenario: Toggle listing active status
- **WHEN** an authenticated Provider toggles the active status of a listing they own
- **THEN** the system flips `IsActive` and redirects back to the listing detail page
