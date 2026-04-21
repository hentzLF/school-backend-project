## ADDED Requirements

### Requirement: Listing list view
The system SHALL provide a list of all `ServiceListing` records at `/Admin/Listings` with columns: Title, Provider name, Category, PricePerHectare, IsActive status. The view SHALL use `ListingListViewModel`.

#### Scenario: View all listings
- **WHEN** an admin navigates to `/Admin/Listings`
- **THEN** the system displays a table of all listings with their details

#### Scenario: Filter by active status
- **WHEN** an admin filters by active/inactive
- **THEN** the system displays only listings matching the filter

### Requirement: Listing detail view
The system SHALL provide a detail view at `/Admin/Listings/Details/{id}` showing full listing information including provider, category, location, equipment, availabilities, and booking count.

#### Scenario: View listing details
- **WHEN** an admin navigates to `/Admin/Listings/Details/{id}`
- **THEN** the system displays the listing's full information

#### Scenario: Listing not found
- **WHEN** an admin navigates to a non-existent listing
- **THEN** the system returns a 404 Not Found page

### Requirement: Listing edit
The system SHALL provide an edit form at `/Admin/Listings/Edit/{id}` allowing the admin to update: Title, Description, PricePerHectare, IsActive, ServiceCategoryId. The form SHALL use `ListingEditViewModel`.

#### Scenario: Edit listing successfully
- **WHEN** an admin submits valid changes
- **THEN** the system saves changes and redirects to the listing detail page

### Requirement: Listing activate/deactivate
The system SHALL allow admins to toggle `IsActive` status directly from the listing list or detail view.

#### Scenario: Deactivate listing
- **WHEN** an admin deactivates a listing
- **THEN** `IsActive` is set to false and the listing no longer appears to public users

#### Scenario: Activate listing
- **WHEN** an admin activates a listing
- **THEN** `IsActive` is set to true

### Requirement: Delete listing
The system SHALL allow admins to delete a listing at `/Admin/Listings/Delete/{id}` with a confirmation page.

#### Scenario: Delete listing with confirmation
- **WHEN** an admin confirms listing deletion
- **THEN** the listing is removed from the database
