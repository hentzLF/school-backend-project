## 1. Project Setup

- [ ] 1.1 Create `AgriMarket.Resources` class library project and add it to the solution
- [ ] 1.2 Add `SharedResource.cs` marker class to `AgriMarket.Resources`
- [ ] 1.3 Create `SharedResource.resx` (English) and `SharedResource.et.resx` (Estonian) with initial empty structure
- [ ] 1.4 Add project reference from `AgriMarket.Web` to `AgriMarket.Resources`
- [ ] 1.5 Add `Microsoft.Extensions.Localization` NuGet package to `AgriMarket.Web`

## 2. Localization Service Registration

- [ ] 2.1 Register `AddLocalization` and `AddViewLocalization` in `Program.cs`
- [ ] 2.2 Configure `RequestLocalizationOptions` with `en` (default) and `et` supported cultures, using `CookieRequestCultureProvider`
- [ ] 2.3 Add `app.UseRequestLocalization()` middleware in the correct position

## 3. View Infrastructure

- [ ] 3.1 Add `@inject IStringLocalizer<SharedResource> Localizer` to all `_ViewImports.cshtml` files (root, Admin, Client)
- [ ] 3.2 Add required `@using` directives for `Microsoft.Extensions.Localization` and `AgriMarket.Resources` to `_ViewImports.cshtml` files

## 4. Language Switcher

- [ ] 4.1 Create `CultureController` with a `SetCulture` action that sets the cookie and redirects back
- [ ] 4.2 Create `_LanguageSwitcher.cshtml` partial view with EN/ET dropdown
- [ ] 4.3 Include `_LanguageSwitcher.cshtml` in `_Layout.cshtml`
- [ ] 4.4 Include `_LanguageSwitcher.cshtml` in `_AdminLayout.cshtml`
- [ ] 4.5 Include `_LanguageSwitcher.cshtml` in `_ClientLayout.cshtml`
- [ ] 4.6 Update `<html lang="en">` to use dynamic culture value in all three layouts

## 5. Translate Admin Area Views

- [ ] 5.1 Extract strings from Admin Account views (Login, Register, AccessDenied) and replace with `@Localizer` calls
- [ ] 5.2 Extract strings from Admin Bookings views (Index, Details, Edit, Delete) and replace with `@Localizer` calls
- [ ] 5.3 Extract strings from Admin Categories views (Index, Create, Edit, Delete) and replace with `@Localizer` calls
- [ ] 5.4 Extract strings from Admin Dashboard views (Index) and replace with `@Localizer` calls
- [ ] 5.5 Extract strings from Admin Listings views (Index, Details, Edit, Delete) and replace with `@Localizer` calls
- [ ] 5.6 Extract strings from Admin Payments views (Index, Details) and replace with `@Localizer` calls
- [ ] 5.7 Extract strings from Admin Users views (Index, Details, Edit, Delete) and replace with `@Localizer` calls
- [ ] 5.8 Extract strings from Admin shared/layout views and replace with `@Localizer` calls

## 6. Translate Client Area Views

- [ ] 6.1 Extract strings from Client Account views (Login, Register, AccessDenied) and replace with `@Localizer` calls
- [ ] 6.2 Extract strings from Client Bookings views and replace with `@Localizer` calls
- [ ] 6.3 Extract strings from Client Listings views and replace with `@Localizer` calls
- [ ] 6.4 Extract strings from Client shared/layout views and replace with `@Localizer` calls

## 7. Translate Root Views

- [ ] 7.1 Extract strings from root-level views (Home, Error, etc.) and replace with `@Localizer` calls
- [ ] 7.2 Extract strings from root `_Layout.cshtml` and replace with `@Localizer` calls

## 8. Populate Resource Files

- [ ] 8.1 Add all extracted English strings to `SharedResource.resx`
- [ ] 8.2 Add all Estonian translations to `SharedResource.et.resx`

## 9. Verification

- [ ] 9.1 Build the solution and verify no compilation errors
- [ ] 9.2 Run the application and verify English rendering matches original
- [ ] 9.3 Switch to Estonian and verify all strings display in Estonian
- [ ] 9.4 Verify language switcher persists preference across page navigation
