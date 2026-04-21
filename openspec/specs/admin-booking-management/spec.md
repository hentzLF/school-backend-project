# Spec: Admin Booking Management

## Purpose
Defines admin operations for viewing and managing bookings, including list with filters, detail view, status updates, and deletion.

## Requirements

### Requirement: Booking list view
The system SHALL provide a list of all `Booking` records at `/Admin/Bookings` with columns: Client name, Listing title, Status, TotalPrice, AreaInHectares, CreatedAt. The view SHALL use `BookingListViewModel`.

#### Scenario: View all bookings
- **WHEN** an admin navigates to `/Admin/Bookings`
- **THEN** the system displays a table of all bookings with their details

#### Scenario: Filter by status
- **WHEN** an admin filters bookings by a specific `BookingStatus`
- **THEN** the system displays only bookings matching that status

### Requirement: Booking detail view
The system SHALL provide a detail view at `/Admin/Bookings/Details/{id}` showing full booking information including client profile, service listing, availability, payment details, and review if present.

#### Scenario: View booking details
- **WHEN** an admin navigates to `/Admin/Bookings/Details/{id}`
- **THEN** the system displays the booking's full information with related entities

#### Scenario: Booking not found
- **WHEN** an admin navigates to a non-existent booking
- **THEN** the system returns a 404 Not Found page

### Requirement: Booking status update
The system SHALL allow admins to update a booking's `BookingStatus` from the detail view. The form SHALL use `BookingEditViewModel` with the current status and a dropdown of all valid statuses.

#### Scenario: Update booking status
- **WHEN** an admin changes a booking's status and submits
- **THEN** the system updates the status and redirects to the booking detail page

### Requirement: Delete booking
The system SHALL allow admins to delete a booking at `/Admin/Bookings/Delete/{id}` with a confirmation page.

#### Scenario: Delete booking with confirmation
- **WHEN** an admin confirms booking deletion
- **THEN** the booking is removed from the database
