## 1. Project Setup & Infrastructure

- [x] 1.1 Create `AgriMarket.E2E` xUnit project with `Microsoft.Playwright`, `Microsoft.Playwright.Xunit` (if available) or raw Playwright, `Microsoft.AspNetCore.Mvc.Testing`, `Testcontainers.PostgreSql`, `FluentAssertions` dependencies
- [x] 1.2 Add project reference to `AgriMarket.Web` and add `AgriMarket.E2E` to the solution
- [x] 1.3 Create `E2EFixture` class implementing `IAsyncLifetime` that starts PostgreSQL Testcontainer, configures `WebApplicationFactory<Program>` with the container's connection string, runs migrations, and seeds data
- [x] 1.4 Create `E2ECollection` class with `[CollectionDefinition("E2E")]` that references `E2EFixture`
- [x] 1.5 Create `PageBase` class with navigation helpers, base URL access, and common assertion methods
- [x] 1.6 Create `AuthHelper` with `CreateAuthenticatedPage` method that logs in via the client or admin login form and returns an authenticated `IPage`
- [x] 1.7 Add Playwright browser install script/target (e.g., `pwsh bin/Debug/net10.0/playwright.ps1 install`)
- [x] 1.8 Verify fixture works: write a smoke test that boots the app and checks the home page returns 200
- [x] 1.9 Git commit: `test: add AgriMarket.E2E project with Playwright and Testcontainers infrastructure`

## 2. Page Object Models

- [x] 2.1 Create `ClientLoginPage` POM (navigate, fill email/password, submit, check error, check redirect)
- [x] 2.2 Create `ClientRegisterPage` POM (navigate, fill form, submit, check errors)
- [x] 2.3 Create `AdminLoginPage` POM (navigate, fill email/password, submit, check error)
- [x] 2.4 Create `ListingIndexPage` POM (navigate, get listing cards, click listing)
- [x] 2.5 Create `ListingDetailPage` POM (get title/price/category, get equipment list, get reviews, get availabilities, fill booking form, submit booking)
- [x] 2.6 Create `MyListingsIndexPage` POM (navigate, get listings, click create/edit/delete, toggle active)
- [x] 2.7 Create `MyListingCreatePage` POM (fill form, submit, check errors)
- [x] 2.8 Create `MyListingEditPage` POM (fill form, submit)
- [x] 2.9 Create `AvailabilitiesPage` POM (add availability, delete availability, get list)
- [x] 2.10 Create `BookingIndexPage` POM (navigate, get bookings)
- [x] 2.11 Create `BookingDetailPage` POM (get status/price/info)
- [x] 2.12 Create `MyListingBookingsPage` POM (get bookings, update status)
- [x] 2.13 Create `CheckoutPage` POM (select payment method, submit)
- [x] 2.14 Create `ReceiptPage` POM (get amount/fee/method)
- [x] 2.15 Create `PaymentHistoryPage` POM (get payment list)
- [x] 2.16 Create `ReviewCreatePage` POM (fill rating/comment, submit)
- [x] 2.17 Create `ReviewEditPage` POM (fill form, submit)
- [x] 2.18 Create `EquipmentIndexPage` POM (navigate, get equipment list, click create/edit/delete)
- [x] 2.19 Create `EquipmentCreatePage` POM (fill form, submit)
- [x] 2.20 Create `EquipmentAssignPage` POM (select listings, submit)
- [x] 2.21 Create `MessagingIndexPage` POM (navigate, get conversations, check unread)
- [x] 2.22 Create `ConversationDetailPage` POM (get messages, send message)
- [x] 2.23 Create `ProfilePage` POM (get info, navigate to edit)
- [x] 2.24 Create `ProfileEditPage` POM (fill form, submit)
- [x] 2.25 Create `AdminDashboardPage` POM (get stats)
- [x] 2.26 Create `AdminUsersPage` POM (get user list, lock/unlock/delete)
- [x] 2.27 Create `AdminListingsPage` POM (get listings, filter, edit, delete)
- [x] 2.28 Create `AdminBookingsPage` POM (get bookings, filter, update status)
- [x] 2.29 Create `AdminPaymentsPage` POM (get payments, release, refund)
- [x] 2.30 Create `AdminCategoriesPage` POM (get categories, create, edit, delete)
- [x] 2.31 Git commit: `test: add Page Object Models for all E2E test pages`

## 3. Authentication E2E Tests

- [x] 3.1 Implement `ClientRegistrationTests` (successful registration, duplicate email, empty fields)
- [x] 3.2 Implement `ClientLoginTests` (successful login, wrong password, non-existent email)
- [x] 3.3 Implement `ClientLogoutTests` (logout redirects to login, protected pages redirect after logout)
- [x] 3.4 Implement `AdminLoginTests` (successful admin login, non-admin rejected)
- [x] 3.5 Implement `AdminRegistrationTests` (admin creates new admin user)
- [x] 3.6 Git commit: `test: add authentication E2E tests`

## 4. Listing E2E Tests

- [x] 4.1 Implement `ListingBrowseTests` (listing index shows active listings, detail page shows full info)
- [x] 4.2 Implement `ListingCrudTests` (create listing, edit listing, delete listing without bookings)
- [x] 4.3 Implement `ListingToggleActiveTests` (deactivate hides from public, reactivate shows again)
- [x] 4.4 Implement `AvailabilityManagementTests` (add availability, remove availability)
- [x] 4.5 Implement `EquipmentAssignmentTests` (assign equipment to listing, equipment appears on detail)
- [x] 4.6 Git commit: `test: add listing E2E tests`

## 5. Booking E2E Tests

- [x] 5.1 Implement `BookingCreationTests` (successful booking, zero area validation)
- [x] 5.2 Implement `BookingListTests` (bookings page shows user's bookings with details)
- [x] 5.3 Implement `BookingLifecycleTests` (Pending → Confirmed → InProgress → ProviderCompleted → ClientConfirmed)
- [x] 5.4 Implement `BookingCancellationTests` (cancel pending booking)
- [x] 5.5 Implement `DoubleBookingTests` (second booking on same availability fails)
- [x] 5.6 Git commit: `test: add booking E2E tests`

## 6. Payment E2E Tests

- [x] 6.1 Implement `CheckoutTests` (successful payment, receipt display)
- [x] 6.2 Implement `PaymentHistoryTests` (payment appears in history)
- [x] 6.3 Implement `InvalidPaymentTests` (pay for non-payable booking fails)
- [x] 6.4 Git commit: `test: add payment E2E tests`

## 7. Review E2E Tests

- [x] 7.1 Implement `ReviewCreationTests` (successful review, invalid rating validation)
- [x] 7.2 Implement `ReviewEditTests` (edit rating and comment)
- [x] 7.3 Implement `ReviewDeletionTests` (delete review, verify removed from listing)
- [x] 7.4 Implement `RatingDisplayTests` (average rating updates on listing)
- [x] 7.5 Git commit: `test: add review E2E tests`

## 8. Equipment E2E Tests

- [x] 8.1 Implement `EquipmentCrudTests` (create, edit, delete equipment)
- [x] 8.2 Implement `EquipmentStatusTests` (change equipment status)
- [x] 8.3 Implement `EquipmentAssignTests` (assign to listing, unassign from listing)
- [x] 8.4 Git commit: `test: add equipment E2E tests`

## 9. Messaging E2E Tests

- [x] 9.1 Implement `ConversationListTests` (conversations page shows conversations)
- [x] 9.2 Implement `SendMessageTests` (send message, empty message handling)
- [x] 9.3 Implement `CrossUserMessagingTests` (farmer sends, provider sees)
- [x] 9.4 Implement `UnreadCountTests` (unread indicator after new message)
- [x] 9.5 Git commit: `test: add messaging E2E tests`

## 10. Admin E2E Tests

- [x] 10.1 Implement `AdminDashboardTests` (dashboard displays all stats)
- [x] 10.2 Implement `AdminUserManagementTests` (list, details, lock, unlock, delete)
- [x] 10.3 Implement `AdminListingManagementTests` (list with filter, edit, delete)
- [x] 10.4 Implement `AdminBookingManagementTests` (list with filter, update status)
- [x] 10.5 Implement `AdminPaymentManagementTests` (view details, release, refund)
- [x] 10.6 Implement `AdminCategoryManagementTests` (list, create, duplicate name, edit, delete)
- [x] 10.7 Git commit: `test: add admin panel E2E tests`

## 11. Authorization E2E Tests

- [x] 11.1 Implement `UnauthenticatedAccessTests` (protected pages redirect to login)
- [x] 11.2 Implement `RoleAccessTests` (farmer blocked from provider pages, client blocked from admin)
- [x] 11.3 Implement `DataIsolationTests` (user cannot view other user's booking/listing/conversation)
- [x] 11.4 Git commit: `test: add authorization E2E tests`

## 12. End-to-End Journey Tests

- [x] 12.1 Implement `ServiceBookingJourneyTest` (full lifecycle: create listing → book → status flow → payment → review)
- [x] 12.2 Implement `MessagingJourneyTest` (booking → conversation → message exchange)
- [x] 12.3 Implement `AdminDisputeJourneyTest` (booking → payment → dispute → admin resolution)
- [x] 12.4 Implement `UserLockoutJourneyTest` (admin locks → login fails → unlock → login succeeds)
- [x] 12.5 Git commit: `test: add end-to-end journey tests`

## 13. Final Validation

- [x] 13.1 Run full E2E test suite and verify all tests pass
- [x] 13.2 Verify `dotnet build` succeeds for the entire solution
- [x] 13.3 Git commit: `test: finalize Playwright E2E test suite`
