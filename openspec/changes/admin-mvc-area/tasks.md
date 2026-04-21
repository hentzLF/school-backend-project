## 1. Domain & Database

- [x] 1.1 Add `CreatedAt` property to `AppUser` entity
- [x] 1.2 Create EF migration for `AppUser.CreatedAt` with default value for existing rows
- [x] 1.3 Update `AppDbSeeder` to set `CreatedAt` on seeded users

## 2. Web Project Authentication Setup

- [x] 2.1 Add cookie authentication configuration to `AgriMarket.Web/Program.cs` (scheme, login path, access denied path)
- [x] 2.2 Add `AdminOnly` authorization policy requiring Admin role claim
- [x] 2.3 Add `UseAuthentication()` middleware to the pipeline (before `UseAuthorization()`)
- [x] 2.4 Add area route registration for Admin area in `Program.cs`

## 3. Account Controller & Views

- [x] 3.1 Create `AccountController` with Login GET/POST, Register GET/POST, Logout actions
- [x] 3.2 Create `LoginViewModel` and `RegisterViewModel` with validation attributes
- [x] 3.3 Create Login view with form (email, password)
- [x] 3.4 Create Register view with form (email, password, first name, last name)
- [x] 3.5 Create AccessDenied view
- [x] 3.6 Implement BCrypt password verification and cookie claim issuance in login action
- [x] 3.7 Implement user creation with Admin role and auto-login in register action

## 4. Admin Area Structure & Layout

- [x] 4.1 Create `Areas/Admin/` directory structure (Controllers, Views, ViewModels)
- [x] 4.2 Create `_AdminLayout.cshtml` with Bootstrap sidebar (Dashboard, Users, Listings, Bookings, Categories, Payments links) and top bar with user info/sign-out
- [x] 4.3 Create `_ViewStart.cshtml` pointing to `_AdminLayout`
- [x] 4.4 Create `_ViewImports.cshtml` with Admin ViewModels namespace and tag helpers

## 5. Dashboard

- [x] 5.1 Create `DashboardViewModel` with all statistics properties (user counts, listing counts, booking breakdown, revenue, disputes, recent bookings)
- [x] 5.2 Create `DashboardController` with Index action that queries all statistics from DbContext
- [x] 5.3 Create Dashboard Index view with statistic cards and recent bookings table

## 6. User Management

- [x] 6.1 Create `UserListItemViewModel`, `UserListViewModel`, `UserDetailViewModel`, `UserEditViewModel`
- [x] 6.2 Create `UsersController` with Index, Details, Edit GET/POST, Delete GET/POST, Lock/Unlock actions
- [x] 6.3 Create Users Index view (table with email, profiles, roles, created, status)
- [x] 6.4 Create Users Details view (full profile info, associated data)
- [x] 6.5 Create Users Edit view (edit email, lockout)
- [x] 6.6 Create Users Delete confirmation view

## 7. Listing Management

- [ ] 7.1 Create `ListingListItemViewModel`, `ListingListViewModel`, `ListingDetailViewModel`, `ListingEditViewModel`
- [ ] 7.2 Create `ListingsController` with Index (with filter), Details, Edit GET/POST, Delete GET/POST, ToggleActive actions
- [ ] 7.3 Create Listings Index view (table with title, provider, category, price, status)
- [ ] 7.4 Create Listings Details view (full listing info with provider, equipment, availabilities)
- [ ] 7.5 Create Listings Edit view (title, description, price, category, active toggle)
- [ ] 7.6 Create Listings Delete confirmation view

## 8. Booking Management

- [ ] 8.1 Create `BookingListItemViewModel`, `BookingListViewModel`, `BookingDetailViewModel`, `BookingEditViewModel`
- [ ] 8.2 Create `BookingsController` with Index (with status filter), Details, Edit GET/POST (status update), Delete GET/POST actions
- [ ] 8.3 Create Bookings Index view (table with client, listing, status, price, date)
- [ ] 8.4 Create Bookings Details view (full booking info with client, listing, payment, review)
- [ ] 8.5 Create Bookings Edit view (status dropdown)
- [ ] 8.6 Create Bookings Delete confirmation view

## 9. Category Management

- [ ] 9.1 Create `CategoryListItemViewModel`, `CategoryListViewModel`, `CategoryCreateViewModel`, `CategoryEditViewModel`
- [ ] 9.2 Create `CategoriesController` with Index, Create GET/POST, Edit GET/POST, Delete GET/POST actions
- [ ] 9.3 Create Categories Index view (table with name, description, listings count)
- [ ] 9.4 Create Categories Create view (name, description form)
- [ ] 9.5 Create Categories Edit view (name, description form)
- [ ] 9.6 Create Categories Delete confirmation view (with listings guard)

## 10. Payment & Dispute Management

- [ ] 10.1 Create `PaymentListItemViewModel`, `PaymentListViewModel`, `PaymentDetailViewModel`, `DisputeResolveViewModel`
- [ ] 10.2 Create `PaymentsController` with Index (with status filter), Details, Resolve POST actions
- [ ] 10.3 Create Payments Index view (table with booking, amount, fee, status, dates)
- [ ] 10.4 Create Payments Details view (full payment info with booking, client, provider, resolution form for disputes)

## 11. Validation & Anti-Forgery

- [ ] 11.1 Add `[ValidateAntiForgeryToken]` to all POST actions across Admin controllers
- [ ] 11.2 Add `@Html.AntiForgeryToken()` to all form views
- [ ] 11.3 Verify all ViewModels have appropriate `[Required]`, `[EmailAddress]`, and other validation attributes
