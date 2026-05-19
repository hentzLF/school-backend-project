## ADDED Requirements

### Requirement: Client can view checkout page for a booking awaiting payment
The system SHALL provide a checkout page at `/Client/Bookings/Checkout/{id}` that displays the booking summary, fee breakdown (service total, 5% platform fee, grand total), and a payment method selection form. The page SHALL only be accessible for bookings owned by the authenticated client that are in `AwaitingPayment` status.

#### Scenario: View checkout for own booking in AwaitingPayment status
- **WHEN** an authenticated client navigates to `/Client/Bookings/Checkout/{id}` for an owned booking in `AwaitingPayment` status
- **THEN** the system displays the checkout page with booking summary, fee breakdown, and payment method selection (Card, BankTransfer, Cash)

#### Scenario: Checkout blocked for non-AwaitingPayment booking
- **WHEN** an authenticated client navigates to `/Client/Bookings/Checkout/{id}` for an owned booking not in `AwaitingPayment` status
- **THEN** the system redirects to booking details without showing the checkout form

#### Scenario: Checkout blocked for non-owned booking
- **WHEN** an authenticated client navigates to `/Client/Bookings/Checkout/{id}` for a booking they do not own
- **THEN** the system denies access and does not reveal checkout details

### Requirement: Client can submit payment from the checkout page
The system SHALL process payment by calling `IClientPaymentService.PayAsync(callerProfileId, payRequest)` with the selected payment method and booking ID. On success, the system SHALL redirect to the payment receipt page.

#### Scenario: Successful payment with Card
- **WHEN** an authenticated client submits the checkout form with payment method `Card`
- **THEN** the system calls `IClientPaymentService.PayAsync()`, creates a `Payment` entity with status `Held`, transitions the booking to `Confirmed`, and redirects to the receipt page

#### Scenario: Successful payment with BankTransfer
- **WHEN** an authenticated client submits the checkout form with payment method `BankTransfer`
- **THEN** the system processes payment identically to Card and redirects to the receipt page

#### Scenario: Successful payment with Cash
- **WHEN** an authenticated client submits the checkout form with payment method `Cash`
- **THEN** the system processes payment identically to Card and redirects to the receipt page

#### Scenario: Payment fails due to invalid booking state
- **WHEN** an authenticated client submits the checkout form but the booking is no longer in `AwaitingPayment` status
- **THEN** the system catches `BusinessRuleException` and redirects to booking details with an error message

### Requirement: Checkout page displays fee breakdown
The checkout page SHALL display the service total (booking price), the platform fee (5% of service total), and the grand total (service total + platform fee). These values SHALL match the amounts calculated by `IClientPaymentService`.

#### Scenario: Fee breakdown matches service calculation
- **WHEN** a booking has a total price of 100.00
- **THEN** the checkout page displays service total 100.00, platform fee 5.00, and grand total 105.00
