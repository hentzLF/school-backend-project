## ADDED Requirements

### Requirement: Booking creation test
The test suite SHALL verify that a farmer can create a booking.

#### Scenario: Successful booking creation
- **WHEN** a farmer selects an available time slot, enters area in hectares, and submits the booking form on a listing detail page
- **THEN** the browser redirects to the booking detail page with status "Pending" and the correct total price (price/ha * area)

#### Scenario: Booking with zero area
- **WHEN** a farmer submits a booking with 0 hectares
- **THEN** a validation error is displayed

### Requirement: Booking list test
The test suite SHALL verify the farmer's booking list at `/Client/Bookings`.

#### Scenario: Bookings are listed
- **WHEN** a farmer with bookings navigates to `/Client/Bookings`
- **THEN** the page displays bookings with status, listing title, and price

### Requirement: Booking detail test
The test suite SHALL verify the booking detail page.

#### Scenario: Booking details are displayed
- **WHEN** a farmer navigates to their booking's detail page
- **THEN** the page shows provider info, listing title, area, price, and current status

### Requirement: Booking status lifecycle test
The test suite SHALL verify the complete booking status transition flow.

#### Scenario: Full lifecycle from Pending to completion
- **WHEN** a provider confirms a pending booking
- **THEN** the status changes to "Confirmed"
- **WHEN** the provider starts work
- **THEN** the status changes to "InProgress"
- **WHEN** the provider marks work complete
- **THEN** the status changes to "ProviderCompleted"
- **WHEN** the farmer confirms completion
- **THEN** the status changes to "ClientConfirmed"

### Requirement: Booking cancellation test
The test suite SHALL verify booking cancellation.

#### Scenario: Cancel a pending booking
- **WHEN** a booking in "Pending" status is cancelled
- **THEN** the status changes to "Cancelled"

### Requirement: Double booking prevention test
The test suite SHALL verify that the same availability cannot be booked twice.

#### Scenario: Second booking on same availability fails
- **WHEN** farmer A books an availability slot
- **AND** farmer B attempts to book the same slot
- **THEN** the second booking attempt fails with an error message
