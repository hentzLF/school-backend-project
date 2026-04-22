## 1. Project Setup

- [ ] 1.1 Create `AgriMarket.Resources` class library project and add it to the solution
- [ ] 1.2 Add `SharedResource.cs` marker class to `AgriMarket.Resources`
- [ ] 1.3 Create `SharedResource.resx` (English) and `SharedResource.et.resx` (Estonian) with initial empty structure
- [ ] 1.4 Add project reference from `AgriMarket.Web` to `AgriMarket.Resources`
- [ ] 1.5 Add `Microsoft.Extensions.Localization` NuGet package to `AgriMarket.Web`

## 2. Localization Service Registration

- [ ] 2.1 Register `AddLocalization` and `AddViewLocalization` in `Program.cs`
- [ ] 2.2 Configure `RequestLocalizationOptions` with `en` (default) and `et` supported cultures, using `CookieRequestCultureProvider` with a 365-day cookie expiry
- [ ] 2.3 Add `app.UseRequestLocalization()` middleware in the correct position

## 3. View Infrastructure

- [ ] 3.1 Add `@inject IStringLocalizer<SharedResource> Localizer` to all `_ViewImports.cshtml` files (root, Admin, Client)
- [ ] 3.2 Add required `@using` directives for `Microsoft.Extensions.Localization` and `AgriMarket.Resources` to `_ViewImports.cshtml` files
- [ ] 3.3 Update `<html lang="en">` to use `CultureInfo.CurrentUICulture.TwoLetterISOLanguageName` in all three layouts

## 4. Language Switcher

- [ ] 4.1 Create `CultureController` with a `SetCulture` action that sets the cookie (with 365-day expiry) and redirects back
- [ ] 4.2 Create `_LanguageSwitcher.cshtml` partial view with EN/ET dropdown
- [ ] 4.3 Include `_LanguageSwitcher.cshtml` in `_Layout.cshtml`
- [ ] 4.4 Include `_LanguageSwitcher.cshtml` in `_AdminLayout.cshtml`
- [ ] 4.5 Include `_LanguageSwitcher.cshtml` in `_ClientLayout.cshtml`

## 5. Translate Admin Area Views

- [ ] 5.1 Extract strings from Admin Account views (Login, Register, AccessDenied), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 5.2 Extract strings from Admin Bookings views (Index, Details, Edit, Delete), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 5.3 Extract strings from Admin Categories views (Index, Create, Edit, Delete), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 5.4 Extract strings from Admin Dashboard views (Index), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 5.5 Extract strings from Admin Listings views (Index, Details, Edit, Delete), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 5.6 Extract strings from Admin Payments views (Index, Details), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 5.7 Extract strings from Admin Users views (Index, Details, Edit, Delete), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 5.8 Extract strings from Admin shared/layout views, replace with `@Localizer` calls, and add entries to both .resx files

## 6. Translate Client Area Views

- [ ] 6.1 Extract strings from Client Account views (Login, Register, AccessDenied), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 6.2 Extract strings from Client Bookings views, replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 6.3 Extract strings from Client Listings views, replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 6.4 Extract strings from Client shared/layout views, replace with `@Localizer` calls, and add entries to both .resx files

## 7. Translate Root Views

- [ ] 7.1 Extract strings from root-level views (Home, Error, etc.), replace with `@Localizer` calls, and add entries to both .resx files
- [ ] 7.2 Extract strings from root `_Layout.cshtml`, replace with `@Localizer` calls, and add entries to both .resx files

## 8. Verification

- [ ] 8.1 Add a unit test that verifies both .resx files contain the same set of keys (no missing translations)
- [ ] 8.2 Build the solution and verify no compilation errors
- [ ] 8.3 Run the application and verify English rendering matches original
- [ ] 8.4 Switch to Estonian and verify all strings display in Estonian
- [ ] 8.5 Verify language switcher persists preference across page navigation and browser restart
