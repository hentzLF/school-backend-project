## Why

AgriMarket currently has all UI strings hardcoded in English across 51 Razor views spanning both Admin and Client areas. To serve Estonian-speaking users and meet localization requirements, the application needs internationalization (i18n) support with Estonian as a second language.

## What Changes

- Create a new `AgriMarket.Resources` class library project dedicated to hosting all .resx resource files
- Add `SharedResource.resx` (English, default) and `SharedResource.et.resx` (Estonian) resource files
- Register ASP.NET Core localization services with cookie-based culture switching
- Replace all hardcoded UI strings in Admin and Client area views with `IStringLocalizer<SharedResource>` calls
- Add a language switcher dropdown (EN / ET) to shared layout(s)
- Add a controller action to set the `.AspNetCore.Culture` cookie when the user switches language

## Capabilities

### New Capabilities
- `i18n-infrastructure`: Localization service registration, culture providers, cookie-based language switching, and the AgriMarket.Resources project setup
- `i18n-language-switcher`: Language switcher dropdown UI component in shared layouts
- `i18n-view-translations`: Extraction and localization of all hardcoded UI strings in Admin and Client area Razor views

### Modified Capabilities

None — this change adds a new cross-cutting concern without altering existing spec-level behavior.

## Impact

- **New project**: `AgriMarket.Resources` added to the solution
- **AgriMarket.Web**: New project reference to `AgriMarket.Resources`, service registration changes in `Program.cs`, `_ViewImports.cshtml` updates, all 51 `.cshtml` views modified, layout changes for language switcher
- **Dependencies**: `Microsoft.Extensions.Localization` NuGet package
- **No breaking changes**: Default culture remains English; existing behavior is preserved for users who don't switch language
