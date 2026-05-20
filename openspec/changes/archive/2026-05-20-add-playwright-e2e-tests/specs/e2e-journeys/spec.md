## ADDED Requirements

### Requirement: Complete service booking journey
The test suite SHALL verify the full end-to-end service booking lifecycle in a single test.

#### Scenario: Provider creates listing, farmer books, lifecycle completes with payment and review
- **WHEN** a provider creates a listing with category, price, and availability
- **AND** the provider adds equipment and assigns it to the listing
- **AND** a farmer browses listings and finds the new listing
- **AND** the farmer creates a booking on an available time slot
- **AND** the provider confirms the booking (Pending → Confirmed)
- **AND** the provider starts work (Confirmed → InProgress)
- **AND** the provider completes work (InProgress → ProviderCompleted)
- **AND** the farmer confirms completion (ProviderCompleted → ClientConfirmed)
- **AND** the farmer makes a payment
- **AND** the farmer leaves a review (rating + comment)
- **THEN** the review is visible on the listing detail page with the correct rating
- **AND** the payment appears in the farmer's payment history

### Requirement: Messaging journey with booking context
The test suite SHALL verify the messaging flow tied to a booking.

#### Scenario: Farmer and provider exchange messages about a booking
- **WHEN** a farmer creates a booking
- **AND** the farmer starts a conversation with the provider (linked to the booking)
- **AND** the farmer sends a message
- **AND** the provider logs in and opens the conversation
- **AND** the provider sends a reply
- **AND** the farmer logs in and opens the conversation
- **THEN** both messages are visible in the conversation thread in chronological order

### Requirement: Admin dispute resolution journey
The test suite SHALL verify the admin's dispute handling flow.

#### Scenario: Booking is disputed and resolved by admin
- **WHEN** a farmer creates a booking and completes the lifecycle through payment
- **AND** the booking is marked as disputed
- **AND** an admin logs in and views the disputed booking/payment
- **AND** the admin resolves the dispute (refund or release)
- **THEN** the payment status reflects the resolution

### Requirement: User lockout journey
The test suite SHALL verify the account lockout flow.

#### Scenario: Admin locks user and user cannot log in
- **WHEN** an admin locks a user's account via `/Admin/Users/{id}`
- **AND** the locked user attempts to log in
- **THEN** the login fails
- **WHEN** the admin unlocks the account
- **AND** the user attempts to log in again
- **THEN** the login succeeds
