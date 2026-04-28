# Spec: Admin Area Layout

## Purpose
Defines the ASP.NET MVC Area structure, routing, shared layout, and view conventions for the Admin area.

## Requirements

### Requirement: MVC area structure
The system SHALL create an ASP.NET MVC Area at `Areas/Admin/` with `Controllers/`, `Views/`, and `ViewModels/` subdirectories. Area routing SHALL be registered in Program.cs with pattern `Admin/{controller=Dashboard}/{action=Index}/{id?}`. All Admin controllers SHALL use `[Area("Admin")]` attribute.

#### Scenario: Area route resolution
- **WHEN** a user navigates to `/Admin`
- **THEN** the system routes to `DashboardController.Index` in the Admin area

#### Scenario: Area controller routing
- **WHEN** a user navigates to `/Admin/Users`
- **THEN** the system routes to `UsersController.Index` in the Admin area

### Requirement: Admin layout with sidebar
The system SHALL provide `_AdminLayout.cshtml` as the layout for all Admin views. The layout SHALL include a fixed sidebar with navigation links to: Dashboard, Users, Listings, Bookings, Categories, Payments. The sidebar SHALL highlight the currently active section. The layout SHALL include a top bar showing the logged-in admin's name and a sign-out link.

#### Scenario: Sidebar navigation
- **WHEN** an admin views any Admin page
- **THEN** the sidebar is visible with links to all admin sections and the current section is highlighted

#### Scenario: Top bar user info
- **WHEN** an admin views any Admin page
- **THEN** the top bar displays the admin's name and a sign-out link

### Requirement: Admin ViewStart and ViewImports
The Admin area SHALL have its own `_ViewStart.cshtml` pointing to `_AdminLayout` and a `_ViewImports.cshtml` importing the Admin ViewModels namespace and tag helpers.

#### Scenario: Views use admin layout
- **WHEN** any Admin view is rendered
- **THEN** it uses `_AdminLayout.cshtml` by default without explicit layout declaration

### Requirement: No ViewBag or ViewData
All data passed to Admin views SHALL be via strongly-typed ViewModels. No controller in the Admin area SHALL use `ViewBag` or `ViewData`.

#### Scenario: Data binding via ViewModel
- **WHEN** an Admin controller renders a view
- **THEN** the view receives a strongly-typed ViewModel with `@model` directive
