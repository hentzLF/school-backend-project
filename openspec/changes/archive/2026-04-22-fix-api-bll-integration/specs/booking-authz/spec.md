## ADDED Requirements

### Requirement: Provider self-booking is prohibited
The system SHALL prevent a UserProfile that owns a ServiceListing from creating a Booking for that same listing. This rule MUST be enforced at the service layer (`BookingService.CreateAsync`) and therefore applies uniformly to both the Web MVC and API surfaces. A violating request MUST be rejected with HTTP 400.

#### Scenario: Provider attempts to book their own listing via API
- **WHEN** an authenticated user whose `profileId` matches the `UserProfileId` of a ServiceListing calls `POST /api/bookings` with that listing's ID
- **THEN** the system returns HTTP 400 and does not create a booking

#### Scenario: Provider attempts to book their own listing via Web
- **WHEN** an authenticated provider submits the booking form for their own listing
- **THEN** the system rejects the request and does not create a booking

#### Scenario: Client books a listing they do not own
- **WHEN** an authenticated user whose `profileId` does NOT match the listing's `UserProfileId` creates a booking
- **THEN** the booking is created successfully
