## ADDED Requirements

### Requirement: Provider can view availability slots for a listing
The system SHALL display all existing availability slots for a listing owned by the authenticated Provider, including their start time, end time, and whether they are booked.

#### Scenario: Provider views slots for their listing
- **WHEN** an authenticated Provider navigates to `GET /Client/MyListings/Availabilities/{id}`
- **THEN** the system returns a page listing all availability slots for that listing, with each slot showing StartTime, EndTime, and booking status

#### Scenario: Provider cannot view slots for another provider's listing
- **WHEN** an authenticated Provider navigates to `GET /Client/MyListings/Availabilities/{id}` where `id` belongs to a different provider
- **THEN** the system returns 404

#### Scenario: Empty state when no slots exist
- **WHEN** an authenticated Provider views availabilities for a listing with no slots
- **THEN** the page shows an empty-state message indicating no slots have been added yet

### Requirement: Provider can add an availability slot to a listing
The system SHALL allow a Provider to create a new availability slot with a start time and end time on a listing they own. The slot SHALL be created with `IsBooked = false`.

#### Scenario: Successful slot creation
- **WHEN** a Provider submits a valid `AddAvailability` POST with StartTime and EndTime where StartTime < EndTime
- **THEN** the system creates an `Availability` record with `IsBooked = false` and redirects back to the Availabilities page

#### Scenario: StartTime must be before EndTime
- **WHEN** a Provider submits `AddAvailability` with StartTime >= EndTime
- **THEN** the system returns a validation error and does not create the slot

#### Scenario: Provider cannot add a slot to another provider's listing
- **WHEN** a Provider POSTs `AddAvailability` with a `listingId` belonging to another provider
- **THEN** the system returns 404

### Requirement: Provider can delete an unbooked availability slot
The system SHALL allow a Provider to permanently delete an availability slot on their own listing, provided the slot has not been booked (`IsBooked = false`).

#### Scenario: Successful deletion of unbooked slot
- **WHEN** a Provider POSTs `DeleteAvailability` for a slot they own that has `IsBooked = false`
- **THEN** the system removes the `Availability` record and redirects back to the Availabilities page

#### Scenario: Cannot delete a booked slot
- **WHEN** a Provider POSTs `DeleteAvailability` for a slot with `IsBooked = true`
- **THEN** the system does not delete the slot and returns an error response (400 or redirect with error)

#### Scenario: Cannot delete another provider's slot
- **WHEN** a Provider POSTs `DeleteAvailability` for a slot whose listing belongs to a different provider
- **THEN** the system returns 404

### Requirement: Listing details page links to availability management
The system SHALL display a "Manage Slots" link on the `MyListings/Details` page so Providers can navigate to availability management.

#### Scenario: Manage Slots link is present on listing details
- **WHEN** a Provider views their listing details at `GET /Client/MyListings/Details/{id}`
- **THEN** the page contains a link to `GET /Client/MyListings/Availabilities/{id}`
