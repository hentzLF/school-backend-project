## Purpose

E2E tests for admin dashboard, user/listing/booking/payment/category management.

## Requirements

### Requirement: Admin dashboard test
The test suite SHALL verify the admin dashboard at `/Admin/Dashboard`.

#### Scenario: Dashboard displays statistics
- **WHEN** an admin navigates to `/Admin/Dashboard`
- **THEN** the page displays total users, new users this month/week, total listings, active listings, total bookings, revenue, platform fees, and dispute counts

### Requirement: Admin user management test
The test suite SHALL verify user management at `/Admin/Users`.

#### Scenario: User list displays all users
- **WHEN** an admin navigates to `/Admin/Users`
- **THEN** all users are listed with their names, emails, and roles

#### Scenario: View user details
- **WHEN** an admin clicks on a user
- **THEN** the detail page shows profile info, listing count, and booking count

#### Scenario: Lock user account
- **WHEN** an admin locks a user's account
- **THEN** the user's status shows as locked
- **AND** the locked user cannot log in

#### Scenario: Unlock user account
- **WHEN** an admin unlocks a previously locked account
- **THEN** the user can log in again

#### Scenario: Delete user
- **WHEN** an admin confirms deletion of a user
- **THEN** the user is removed from the user list

### Requirement: Admin listing management test
The test suite SHALL verify listing management at `/Admin/Listings`.

#### Scenario: View all listings with filter
- **WHEN** an admin navigates to `/Admin/Listings`
- **THEN** all listings are displayed with an option to filter by active/inactive

#### Scenario: Edit listing
- **WHEN** an admin edits a listing's details and saves
- **THEN** the changes are persisted

#### Scenario: Delete listing
- **WHEN** an admin deletes a listing
- **THEN** the listing is removed from the list

### Requirement: Admin booking management test
The test suite SHALL verify booking management at `/Admin/Bookings`.

#### Scenario: View bookings with status filter
- **WHEN** an admin navigates to `/Admin/Bookings`
- **THEN** all bookings are displayed with status filter options

#### Scenario: Update booking status
- **WHEN** an admin changes a booking's status
- **THEN** the new status is persisted and displayed

### Requirement: Admin payment management test
The test suite SHALL verify payment management at `/Admin/Payments`.

#### Scenario: View payment details
- **WHEN** an admin opens a payment's detail page
- **THEN** the page displays amount, fee, method, status, and associated booking

#### Scenario: Release funds
- **WHEN** an admin releases funds for a completed payment
- **THEN** the payment status changes to "Released"

#### Scenario: Refund payment
- **WHEN** an admin refunds a payment
- **THEN** the payment status changes to "Refunded"

### Requirement: Admin category management test
The test suite SHALL verify category CRUD at `/Admin/Categories`.

#### Scenario: View categories
- **WHEN** an admin navigates to `/Admin/Categories`
- **THEN** all 7 seed categories are displayed

#### Scenario: Create category
- **WHEN** an admin creates a new category with a unique name
- **THEN** the category appears in the list

#### Scenario: Create category with duplicate name
- **WHEN** an admin creates a category with an existing name
- **THEN** an error message is displayed

#### Scenario: Edit category
- **WHEN** an admin edits a category name and saves
- **THEN** the updated name is displayed

#### Scenario: Delete category
- **WHEN** an admin deletes a category
- **THEN** the category is removed from the list
