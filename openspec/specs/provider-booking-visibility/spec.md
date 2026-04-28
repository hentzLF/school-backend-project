# Capability: provider-booking-visibility

## Purpose
TBD

## Requirements

### Requirement: Provider can view bookings for own listings
The system SHALL provide a read-only bookings view at `/Client/MyListings/Bookings/{listingId}` that shows all `Booking` records for a specific listing owned by the authenticated Provider. Each entry SHALL show client name, booking status, area in hectares, total price, and creation date.

#### Scenario: View bookings for own listing
- **WHEN** an authenticated Provider navigates to `/Client/MyListings/Bookings/{listingId}` for a listing they own
- **THEN** the system displays all bookings for that listing with client name, status, area, total price, and creation date

#### Scenario: No bookings for listing
- **WHEN** an authenticated Provider navigates to `/Client/MyListings/Bookings/{listingId}` for a listing they own that has no bookings
- **THEN** the system displays an empty-state message

#### Scenario: Access denied for non-owned listing bookings
- **WHEN** an authenticated Provider navigates to `/Client/MyListings/Bookings/{listingId}` for a listing they do not own
- **THEN** the system returns 404 and does not reveal booking data
