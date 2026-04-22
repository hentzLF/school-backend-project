## ADDED Requirements

### Requirement: Client MVC area structure
The system SHALL provide a new ASP.NET MVC area at `Areas/Client/` with `Controllers/`, `Views/`, and `ViewModels/` subdirectories. The system SHALL register area routing with pattern `Client/{controller=Listings}/{action=Index}/{id?}` and all client area controllers SHALL use `[Area("Client")]`.

#### Scenario: Client area default route resolution
- **WHEN** a user navigates to `/Client`
- **THEN** the system routes to `ListingsController.Index` in the Client area

#### Scenario: Client area controller route resolution
- **WHEN** a user navigates to `/Client/Bookings`
- **THEN** the system routes to `BookingsController.Index` in the Client area

### Requirement: Client layout conventions
The system SHALL provide a shared client layout (`_ClientLayout.cshtml`) and client area `_ViewStart.cshtml`/`_ViewImports.cshtml` so all client views use the same functional navigation and strongly-typed view model conventions.

`_ClientLayout.cshtml` SHALL include the following navigation elements:
- **Listings** link → `/Client/Listings` (always visible)
- **My Bookings** link → `/Client/Bookings` (visible to authenticated users only)
- **Profile** link → `/Client/Profile` (visible to authenticated users only)
- **Login** link → `/Client/Account/Login` (visible to unauthenticated users only)
- **Register** link → `/Client/Account/Register` (visible to unauthenticated users only)
- **Logout** form → POST `/Client/Account/Logout` (visible to authenticated users only)

#### Scenario: Client view uses client layout
- **WHEN** any view in `Areas/Client/Views` is rendered
- **THEN** it uses `_ClientLayout.cshtml` by default without per-view layout duplication

#### Scenario: Strongly typed client views
- **WHEN** a client controller renders a view
- **THEN** the view receives a strongly-typed model and does not rely on `ViewBag` or `ViewData`

#### Scenario: Navigation reflects authentication state
- **WHEN** an unauthenticated user views any client page
- **THEN** the layout shows Login and Register links and hides My Bookings, Profile, and Logout

#### Scenario: Navigation reflects authenticated state
- **WHEN** an authenticated client views any client page
- **THEN** the layout shows My Bookings, Profile, and Logout and hides Login and Register

