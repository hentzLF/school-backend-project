## ADDED Requirements

### Requirement: Client can view payment receipt after successful payment
The system SHALL provide a receipt page at `/Client/Payments/Receipt/{id}` that displays the `PaymentReceiptDto` data returned by `IClientPaymentService.PayAsync()`. The receipt SHALL show payment ID, booking ID, amount, platform fee, total charged, payment method, status, and payment timestamp.

#### Scenario: View receipt after successful payment
- **WHEN** an authenticated client is redirected to the receipt page after a successful payment
- **THEN** the system displays the payment receipt with all payment details and a link back to booking details

#### Scenario: Receipt blocked for non-owned payment
- **WHEN** an authenticated client navigates to `/Client/Payments/Receipt/{id}` for a payment belonging to a different client
- **THEN** the system denies access and does not reveal payment details

#### Scenario: Receipt for non-existent payment
- **WHEN** an authenticated client navigates to `/Client/Payments/Receipt/{id}` for a payment that does not exist
- **THEN** the system returns a not-found response
