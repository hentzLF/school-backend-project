## MODIFIED Requirements

### Requirement: Client can view own booking details
The system SHALL provide booking details at `/Client/Bookings/Details/{id}` for bookings owned by the authenticated client profile. When the booking is in `AwaitingPayment` status, the checkout card SHALL link to the dedicated checkout page (`/Client/Bookings/Checkout/{id}`) instead of submitting payment directly.

#### Scenario: View own booking details
- **WHEN** an authenticated client opens `/Client/Bookings/Details/{id}` for a booking they own
- **THEN** the system shows booking details and available client actions

#### Scenario: Access denied for non-owned booking
- **WHEN** an authenticated client opens `/Client/Bookings/Details/{id}` for a booking they do not own
- **THEN** the system denies access and does not reveal booking details

#### Scenario: Booking in AwaitingPayment shows checkout link
- **WHEN** an authenticated client views details of an owned booking in `AwaitingPayment` status
- **THEN** the checkout card displays the fee breakdown and a "Proceed to Checkout" button that links to `/Client/Bookings/Checkout/{id}`
