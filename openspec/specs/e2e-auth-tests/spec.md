## Purpose

E2E tests for client and admin authentication flows: login, registration, logout, and access denied scenarios.

## Requirements

### Requirement: Client registration test
The test suite SHALL verify that a new user can register via `/Client/Account/Register`.

#### Scenario: Successful registration
- **WHEN** a user fills the register form with a unique email, valid password, first name, and last name and submits
- **THEN** the browser redirects to the login page

#### Scenario: Registration with duplicate email
- **WHEN** a user submits the register form with `provider@agrimarket.ee` (existing email)
- **THEN** the page displays an error message and stays on the register page

#### Scenario: Registration with empty fields
- **WHEN** a user submits the register form with empty required fields
- **THEN** validation error messages are displayed

### Requirement: Client login test
The test suite SHALL verify login via `/Client/Account/Login`.

#### Scenario: Successful login
- **WHEN** a user enters `farmer@agrimarket.ee` / `Farmer123!` and submits
- **THEN** the browser redirects to `/Client/Listings`

#### Scenario: Login with wrong password
- **WHEN** a user enters `farmer@agrimarket.ee` / `WrongPass!` and submits
- **THEN** the page displays an error message and stays on the login page

#### Scenario: Login with non-existent email
- **WHEN** a user enters `nobody@test.ee` / `Pass123!` and submits
- **THEN** the page displays an error message

### Requirement: Client logout test
The test suite SHALL verify logout.

#### Scenario: Successful logout
- **WHEN** a logged-in user clicks the logout button/link
- **THEN** the browser redirects to the login page
- **AND** navigating to `/Client/Bookings` redirects back to login

### Requirement: Admin login test
The test suite SHALL verify admin login via `/Admin/Account/Login`.

#### Scenario: Successful admin login
- **WHEN** `admin@agrimarket.ee` / `Admin123!` is submitted on the admin login form
- **THEN** the browser redirects to `/Admin/Dashboard`

#### Scenario: Non-admin login attempt on admin form
- **WHEN** `farmer@agrimarket.ee` / `Farmer123!` is submitted on the admin login form
- **THEN** the page displays an error message (non-admin user cannot access admin area)

### Requirement: Admin registration test
The test suite SHALL verify that an authenticated admin can register new admin users via `/Admin/Account/Register`.

#### Scenario: Admin creates new admin user
- **WHEN** a logged-in admin fills the admin register form and submits
- **THEN** the new admin user is created and the admin is redirected appropriately
