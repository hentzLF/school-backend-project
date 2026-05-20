## ADDED Requirements

### Requirement: Public listing index test
The test suite SHALL verify that `/Client/Listings` displays active listings.

#### Scenario: Active listings are visible
- **WHEN** a user (authenticated or not) navigates to `/Client/Listings`
- **THEN** the page displays listing cards with title, price, and category

### Requirement: Listing detail test
The test suite SHALL verify listing detail page content.

#### Scenario: Detail page shows full listing information
- **WHEN** a user navigates to a listing's detail page
- **THEN** the page displays title, description, price per hectare, category, equipment list (if any), reviews (if any), and available time slots (if any)

### Requirement: Provider creates listing
The test suite SHALL verify listing creation via `/Client/MyListings/Create`.

#### Scenario: Successful listing creation
- **WHEN** a provider fills the create form with title, description, price, and category and submits
- **THEN** the listing appears in `/Client/MyListings`

#### Scenario: Create listing with missing required fields
- **WHEN** a provider submits the create form with empty title
- **THEN** validation errors are displayed

### Requirement: Provider edits listing
The test suite SHALL verify listing editing.

#### Scenario: Successful listing edit
- **WHEN** a provider changes the title of an existing listing and saves
- **THEN** the updated title is visible on the listing details page

### Requirement: Provider deletes listing
The test suite SHALL verify listing deletion.

#### Scenario: Delete listing without bookings
- **WHEN** a provider confirms deletion of a listing with no active bookings
- **THEN** the listing is removed from `/Client/MyListings`

### Requirement: Provider toggles listing active status
The test suite SHALL verify the toggle active functionality.

#### Scenario: Deactivate listing
- **WHEN** a provider toggles an active listing to inactive
- **THEN** the listing status changes to inactive on `/Client/MyListings`
- **AND** the listing no longer appears on the public `/Client/Listings` page

#### Scenario: Reactivate listing
- **WHEN** a provider toggles an inactive listing to active
- **THEN** the listing status changes to active and it reappears on the public listing page

### Requirement: Provider manages availabilities
The test suite SHALL verify availability CRUD on a listing.

#### Scenario: Add availability
- **WHEN** a provider adds a new availability with start and end times
- **THEN** the availability appears in the listing's availability list

#### Scenario: Remove availability
- **WHEN** a provider deletes an unbooked availability
- **THEN** the availability is removed from the list

### Requirement: Provider assigns equipment to listing
The test suite SHALL verify equipment assignment.

#### Scenario: Assign equipment
- **WHEN** a provider assigns an equipment item to a listing
- **THEN** the equipment appears on the listing's detail page
