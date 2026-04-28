# Spec: Admin User Management

## Purpose
Defines admin CRUD operations for managing platform users, including listing, viewing details, editing, locking/unlocking, and deleting user accounts.

## Requirements

### Requirement: User list view
The system SHALL provide a list of all `AppUser` records at `/Admin/Users` with columns: Email, Profiles count, Roles, CreatedAt, LockoutEnd status. The view SHALL use `UserListViewModel` containing a collection of `UserListItemViewModel`.

#### Scenario: View all users
- **WHEN** an admin navigates to `/Admin/Users`
- **THEN** the system displays a table of all users with their details

### Requirement: User detail view
The system SHALL provide a detail view at `/Admin/Users/Details/{id}` showing full user information including all profiles and their roles, bookings count, listings count, and account status.

#### Scenario: View user details
- **WHEN** an admin navigates to `/Admin/Users/Details/{id}`
- **THEN** the system displays the user's full profile information and associated data

#### Scenario: User not found
- **WHEN** an admin navigates to `/Admin/Users/Details/{id}` with a non-existent ID
- **THEN** the system returns a 404 Not Found page

### Requirement: User edit
The system SHALL provide an edit form at `/Admin/Users/Edit/{id}` allowing the admin to update: Email, LockoutEnd. The form SHALL use `UserEditViewModel` with validation.

#### Scenario: Edit user successfully
- **WHEN** an admin submits valid changes on the edit form
- **THEN** the system saves changes and redirects to the user detail page

#### Scenario: Edit with validation errors
- **WHEN** an admin submits the edit form with invalid data
- **THEN** the system re-displays the form with validation errors

### Requirement: Lock and unlock user accounts
The system SHALL allow admins to lock a user account by setting `LockoutEnd` to a future date, and unlock by clearing `LockoutEnd`.

#### Scenario: Lock user
- **WHEN** an admin locks a user account
- **THEN** `LockoutEnd` is set to a future date and the user cannot log in

#### Scenario: Unlock user
- **WHEN** an admin unlocks a user account
- **THEN** `LockoutEnd` is cleared and the user can log in again

### Requirement: Delete user
The system SHALL allow admins to delete a user account at `/Admin/Users/Delete/{id}` with a confirmation page.

#### Scenario: Delete user with confirmation
- **WHEN** an admin confirms user deletion
- **THEN** the user and associated data are removed from the database
