## ADDED Requirements

### Requirement: Client can view their payment history
The system SHALL provide a payment history page at `/Client/Payments` that lists all payments associated with the authenticated client via `IClientPaymentService.GetHistoryAsync(callerProfileId)`. Each entry SHALL display listing title, amount, platform fee, payment method, status, and date.

#### Scenario: View payment history with payments
- **WHEN** an authenticated client navigates to `/Client/Payments` and has past payments
- **THEN** the system displays a list of payments ordered by most recent first

#### Scenario: View payment history with no payments
- **WHEN** an authenticated client navigates to `/Client/Payments` and has no past payments
- **THEN** the system displays an empty-state message

### Requirement: Payment history is accessible from client navigation
The client layout navigation SHALL include a link to the payment history page.

#### Scenario: Payment history link visible in navigation
- **WHEN** an authenticated client views any page within the Client area
- **THEN** the navigation bar includes a link to `/Client/Payments`
