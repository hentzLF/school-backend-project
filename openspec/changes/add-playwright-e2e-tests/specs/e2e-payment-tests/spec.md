## ADDED Requirements

### Requirement: Payment checkout test
The test suite SHALL verify the payment checkout flow.

#### Scenario: Successful payment
- **WHEN** a farmer navigates to `/Client/Payments/Checkout/{bookingId}` for a booking in payable status, selects a payment method, and submits
- **THEN** the browser redirects to a receipt page showing amount, platform fee, and payment method

### Requirement: Payment history test
The test suite SHALL verify the payment history page.

#### Scenario: Payment appears in history
- **WHEN** a farmer navigates to `/Client/Payments/History` after making a payment
- **THEN** the payment is listed with amount, date, status, and associated booking info

### Requirement: Invalid payment attempt test
The test suite SHALL verify that payments are blocked for invalid booking states.

#### Scenario: Pay for non-payable booking
- **WHEN** a farmer attempts to navigate to checkout for a booking in "Pending" status
- **THEN** the page displays an error or redirects with an error message
