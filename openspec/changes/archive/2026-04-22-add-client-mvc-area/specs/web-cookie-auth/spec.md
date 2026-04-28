## MODIFIED Requirements

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

