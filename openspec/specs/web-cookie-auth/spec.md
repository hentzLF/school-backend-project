# Spec: Web Cookie Authentication

## Purpose
Defines cookie-based authentication for the AgriMarket.Web MVC project, covering admin login, registration, sign-out, and authorization policy enforcement.

## Requirements

### Requirement: Cookie authentication scheme
The system SHALL configure cookie-based authentication in AgriMarket.Web using `CookieAuthenticationDefaults.AuthenticationScheme`. The cookie SHALL be HTTP-only and secure in production.

The system SHALL implement audience-aware redirect behavior by overriding the `OnRedirectToLogin` and `OnRedirectToAccessDenied` event handlers on `CookieAuthenticationEvents`. Each handler SHALL inspect the request path and apply the following routing logic:

| Request path prefix | Unauthenticated redirect | Unauthorized redirect |
|---|---|---|
| `/Admin/` | `/Admin/Account/Login` | `/Admin/Account/AccessDenied` |
| `/Client/` | `/Client/Account/Login` | `/Client/Account/AccessDenied` |
| (other / fallback) | `/Client/Account/Login` | `/Client/Account/AccessDenied` |

`LoginPath` and `AccessDeniedPath` on `CookieAuthenticationOptions` SHALL be set to the client defaults; per-area overrides are handled exclusively in the event handlers.

#### Scenario: Unauthenticated user accesses protected admin page
- **WHEN** an unauthenticated user navigates to any Admin area page
- **THEN** the system redirects to the admin login endpoint

#### Scenario: Unauthenticated user accesses protected client page
- **WHEN** an unauthenticated user navigates to any Client area page
- **THEN** the system redirects to the client login endpoint

#### Scenario: Authenticated non-admin accesses admin page
- **WHEN** an authenticated user without Admin role navigates to an Admin area page
- **THEN** the system redirects to the admin access-denied endpoint

#### Scenario: Authenticated non-client accesses client page
- **WHEN** an authenticated user without client role navigates to a Client area page
- **THEN** the system redirects to the client access-denied endpoint

### Requirement: Admin login
The system SHALL provide a login form at `/Account/Login` accepting email and password. The system SHALL validate credentials against `AppUser.PasswordHash` using BCrypt. Upon successful login, the system SHALL check that the user has at least one `UserProfile` with a `ProfileRole` of `RoleType.Admin`. The system SHALL issue a cookie with claims: `ClaimTypes.NameIdentifier` (AppUser.Id), `ClaimTypes.Email`, `ClaimTypes.Name` (profile FirstName + LastName), and `ClaimTypes.Role` = "Admin".

#### Scenario: Successful admin login
- **WHEN** a user submits valid email and password AND has an Admin profile role
- **THEN** the system issues an auth cookie and redirects to `/Admin/Dashboard`

#### Scenario: Valid credentials but no admin role
- **WHEN** a user submits valid email and password BUT has no Admin profile role
- **THEN** the system displays an error "You do not have administrator access"

#### Scenario: Invalid credentials
- **WHEN** a user submits invalid email or password
- **THEN** the system displays an error "Invalid email or password"

#### Scenario: Locked out user
- **WHEN** a user submits valid credentials BUT `AppUser.LockoutEnd` is in the future
- **THEN** the system displays an error "Your account is locked"

### Requirement: Admin registration
The system SHALL provide a registration form at `/Account/Register` accepting email, password, first name, and last name. The system SHALL create an `AppUser` with BCrypt-hashed password, a `UserProfile`, and a `ProfileRole` with `RoleType.Admin`. After registration, the system SHALL automatically log the user in and redirect to `/Admin/Dashboard`.

#### Scenario: Successful registration
- **WHEN** a user submits valid registration data with a unique email
- **THEN** the system creates the user with Admin role and redirects to `/Admin/Dashboard` logged in

#### Scenario: Duplicate email
- **WHEN** a user submits registration data with an email that already exists
- **THEN** the system displays an error "An account with this email already exists"

### Requirement: Sign out
The system SHALL provide a sign-out action at `/Account/Logout` that clears the auth cookie and redirects to the login page.

#### Scenario: Sign out
- **WHEN** an authenticated user clicks sign out
- **THEN** the auth cookie is cleared and the user is redirected to `/Account/Login`

### Requirement: AdminOnly authorization policy
The system SHALL define an authorization policy named "AdminOnly" that requires `ClaimTypes.Role == "Admin"`. All Admin area controllers SHALL use this policy.

#### Scenario: Policy enforcement
- **WHEN** a request reaches an Admin area controller
- **THEN** the AdminOnly policy is evaluated and only users with the Admin role claim are allowed
