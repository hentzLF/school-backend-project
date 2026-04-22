## ADDED Requirements

### Requirement: Client registration page and account creation
The system SHALL provide a client registration page at `/Client/Account/Register` and create a client account by persisting an `AppUser`, `UserProfile`, and the role selected by the user during registration. The registration form SHALL present a role selector with exactly two options: `Farmer` and `Provider`. The selected role is assigned as `RoleType` on the created `UserProfile`. Registration SHALL reject duplicate email addresses.

#### Scenario: Successful client registration as Farmer
- **WHEN** a user submits valid registration data with a unique email and selects Farmer
- **THEN** the system creates an account with `RoleType.Farmer` and redirects to `/Client/Listings`

#### Scenario: Successful client registration as Provider
- **WHEN** a user submits valid registration data with a unique email and selects Provider
- **THEN** the system creates an account with `RoleType.Provider` and redirects to `/Client/Listings`

#### Scenario: Duplicate email during registration
- **WHEN** a user submits registration with an email already present in `AppUsers`
- **THEN** the system displays a validation error and does not create an account

#### Scenario: Role not selected during registration
- **WHEN** a user submits registration without selecting a role
- **THEN** the system displays a validation error and does not create an account

### Requirement: Client login and logout pages
The system SHALL provide client login and logout endpoints at `/Client/Account/Login` and `/Client/Account/Logout`. Client login SHALL validate credentials and require a client-facing role before issuing an authenticated principal for client area access.

#### Scenario: Successful client login
- **WHEN** a user submits valid credentials and has a client-facing role
- **THEN** the system signs in the user and redirects to the client area

#### Scenario: Login denied without client role
- **WHEN** a user submits valid credentials but has no client-facing role
- **THEN** the system displays an authorization error and does not sign in

#### Scenario: Successful client logout
- **WHEN** an authenticated client posts to `/Client/Account/Logout`
- **THEN** the system clears authentication state and redirects to `/Client/Account/Login`

### Requirement: Client can manage own profile
The system SHALL provide client profile management pages at `/Client/Profile` allowing authenticated clients to view and update their own `UserProfile` data. Editable fields are limited to those available on the existing `UserProfile` model (name, contact fields). The profile page SHALL display the user's assigned role as read-only.

#### Scenario: View profile
- **WHEN** an authenticated client navigates to `/Client/Profile`
- **THEN** the system displays that client's current profile fields and their assigned role (read-only)

#### Scenario: Update profile
- **WHEN** an authenticated client submits valid profile changes
- **THEN** the system persists the changes and redisplays the profile page with a success message

#### Scenario: Invalid profile update rejected
- **WHEN** an authenticated client submits profile changes that fail validation (e.g., empty required field)
- **THEN** the system redisplays the form with validation errors and does not persist changes

