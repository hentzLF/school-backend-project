## 1. Authorization and Navigation

- [x] 1.1 Register a `ProviderOnly` authorization policy in `Program.cs` that requires `RoleType.Provider` (mirror the existing `ClientOnly` policy pattern).
- [x] 1.2 Update `_ClientLayout.cshtml` to conditionally render a `My Listings` nav link for authenticated users with the `Provider` role.

## 2. Provider Listing Management

- [x] 2.1 Create `Areas/Client/Controllers/MyListingsController` with `[Area("Client")]` and `[Authorize(Policy = "ProviderOnly")]`. Add a private `GetProviderProfileAsync()` helper that resolves the authenticated user's `UserProfile` from `ClaimTypes.NameIdentifier`.
- [x] 2.2 Implement `MyListingsController.Index` — queries `ServiceListings` filtered by `UserProfileId == providerProfile.Id`, ordered by title. Includes `ServiceCategory`. Shows empty-state when no listings exist.
- [x] 2.3 Implement `MyListingsController.Details` — loads a single listing by `id` filtered to the authenticated Provider's profile (404 if not found or not owned). Shows title, description, category, price, active status, and total booking count.
- [x] 2.4 Implement `MyListingsController.Create` (GET/POST) — form with title (required), description (optional), category selector (required), and price-per-hectare (required, > 0). On success, creates listing with `UserProfileId = providerProfile.Id` and `IsActive = false`, redirects to Details.
- [x] 2.5 Implement `MyListingsController.Edit` (GET/POST) — loads own listing, allows editing title, description, category, price-per-hectare, and `IsActive`. Returns 404 if listing not owned. On success redirects to Details.
- [x] 2.6 Implement `MyListingsController.Delete` (GET/POST) — confirmation page for own listings only (404 if not owned). On POST, check for active bookings (status not in `Archived`, `Cancelled`, `ClientConfirmed`); if found, return error. Otherwise permanently delete and redirect to Index.
- [x] 2.7 Implement `MyListingsController.ToggleActive` (POST) — flips `IsActive` on an owned listing (404 if not owned) and redirects to Details.
- [x] 2.8 Create ViewModels: `MyListingIndexViewModel`, `MyListingIndexItemViewModel`, `MyListingDetailsViewModel`, `MyListingCreateViewModel`, `MyListingEditViewModel` in `Areas/Client/ViewModels/MyListings/`.

## 3. Provider Listing Views

- [x] 3.1 Create `Areas/Client/Views/MyListings/Index.cshtml` — table of own listings with title, category, price-per-hectare, active status badge, and links to Details and Create.
- [x] 3.2 Create `Areas/Client/Views/MyListings/Details.cshtml` — full listing info, total booking count, Edit/Delete/ToggleActive actions, and a link to view bookings for this listing.
- [x] 3.3 Create `Areas/Client/Views/MyListings/Create.cshtml` — form with validation summary, title, description, category dropdown, and price-per-hectare fields.
- [x] 3.4 Create `Areas/Client/Views/MyListings/Edit.cshtml` — same fields as Create plus `IsActive` checkbox and validation summary.
- [x] 3.5 Create `Areas/Client/Views/MyListings/Delete.cshtml` — confirmation page showing listing title and active-bookings error if deletion is blocked.

## 4. Provider Booking Visibility

- [x] 4.1 Implement `MyListingsController.Bookings` (GET) — loads all `Booking` records for a listing owned by the authenticated Provider (404 if listing not owned). Projects to a view model with client name, status, area, total price, and creation date.
- [x] 4.2 Create `BookingsForListingViewModel` and `BookingsForListingItemViewModel` in `Areas/Client/ViewModels/MyListings/`.
- [x] 4.3 Create `Areas/Client/Views/MyListings/Bookings.cshtml` — table of bookings for the listing with client name, status badge, area, price, and date. Includes empty-state message and a back link to listing details.

## 5. Verification

- [x] 5.1 Verify `ProviderOnly` policy blocks `Farmer`-role users from accessing `/Client/MyListings` — should redirect to access denied.
- [x] 5.2 Verify a Provider cannot access, edit, or delete another Provider's listings (all such requests return 404).
- [x] 5.3 Verify delete is rejected when active bookings exist and succeeds when no active bookings exist.
- [x] 5.4 Verify the `My Listings` nav link appears only for authenticated Provider users and not for Farmers or unauthenticated users.
- [x] 5.5 Run a manual flow: register as Provider → login → create listing → view listing → edit listing → toggle active → view bookings (empty) → delete listing.
