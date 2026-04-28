## ADDED Requirements

### Requirement: Payment list view
The system SHALL provide a list of all `Payment` records at `/Admin/Payments` with columns: Booking ID, Amount, PlatformFee, Status, CreatedAt, ReleasedAt. The view SHALL use `PaymentListViewModel`.

#### Scenario: View all payments
- **WHEN** an admin navigates to `/Admin/Payments`
- **THEN** the system displays a table of all payments with their details

#### Scenario: Filter by status
- **WHEN** an admin filters payments by a specific `PaymentStatus`
- **THEN** the system displays only payments matching that status

#### Scenario: Filter disputes only
- **WHEN** an admin clicks "Disputes" filter
- **THEN** the system displays only payments with `PaymentStatus.Disputed`

### Requirement: Payment detail view
The system SHALL provide a detail view at `/Admin/Payments/Details/{id}` showing full payment information including the associated booking, client profile, provider profile, listing title, and payment timeline.

#### Scenario: View payment details
- **WHEN** an admin navigates to `/Admin/Payments/Details/{id}`
- **THEN** the system displays the payment's full information with related entities

#### Scenario: Payment not found
- **WHEN** an admin navigates to a non-existent payment
- **THEN** the system returns a 404 Not Found page

### Requirement: Dispute resolution
The system SHALL allow admins to resolve disputed payments from the payment detail view. Resolution options SHALL be: Release payment (set status to `Released` and `ReleasedAt` to UTC now) or Refund payment (set status to `Refunded`). The system SHALL only allow resolution of payments with `PaymentStatus.Disputed`.

#### Scenario: Release disputed payment
- **WHEN** an admin resolves a disputed payment by selecting "Release"
- **THEN** the payment status is set to `Released`, `ReleasedAt` is set to current UTC time, and the admin is redirected to the payment detail page

#### Scenario: Refund disputed payment
- **WHEN** an admin resolves a disputed payment by selecting "Refund"
- **THEN** the payment status is set to `Refunded` and the admin is redirected to the payment detail page

#### Scenario: Resolve non-disputed payment
- **WHEN** an admin attempts to resolve a payment that is not `Disputed`
- **THEN** the system displays an error "Only disputed payments can be resolved"
