## Purpose

E2E tests for role-based access control and cross-user data isolation.

## Requirements

### Requirement: Unauthenticated access redirect test
The test suite SHALL verify that unauthenticated users are redirected to login.

#### Scenario: Access protected client page without login
- **WHEN** an unauthenticated user navigates to `/Client/Bookings`
- **THEN** the browser redirects to `/Client/Account/Login`

#### Scenario: Access protected admin page without login
- **WHEN** an unauthenticated user navigates to `/Admin/Dashboard`
- **THEN** the browser redirects to `/Admin/Account/Login`

### Requirement: Role-based access control test
The test suite SHALL verify that users cannot access pages outside their role.

#### Scenario: Farmer cannot access provider pages
- **WHEN** a user with only Farmer role navigates to `/Client/MyListings/Create`
- **THEN** the page shows Access Denied or redirects to Access Denied page

#### Scenario: Farmer cannot access equipment management
- **WHEN** a user with only Farmer role navigates to `/Client/Equipment`
- **THEN** the page shows Access Denied

#### Scenario: Non-admin cannot access admin area
- **WHEN** a client user navigates to `/Admin/Dashboard`
- **THEN** the page shows Access Denied or redirects to Access Denied page

### Requirement: Cross-user data isolation test
The test suite SHALL verify that users cannot access other users' resources.

#### Scenario: User cannot view another user's booking
- **WHEN** user A navigates to the booking detail URL of user B's booking
- **THEN** access is denied (403 or redirect)

#### Scenario: User cannot edit another user's listing
- **WHEN** user A navigates to the edit URL of user B's listing
- **THEN** access is denied (403 or redirect)

#### Scenario: User cannot view another user's conversation
- **WHEN** user A navigates to the conversation detail URL of a conversation they are not a participant in
- **THEN** access is denied (403 or redirect)
