## ADDED Requirements

### Requirement: Resources project exists
The solution SHALL contain an `AgriMarket.Resources` class library project with a `SharedResource` marker class and .resx resource files for English and Estonian.

#### Scenario: Project structure
- **WHEN** the solution is built
- **THEN** `AgriMarket.Resources` project exists with `SharedResource.cs`, `SharedResource.resx` (English), and `SharedResource.et.resx` (Estonian)

### Requirement: Localization services registered
The application SHALL register ASP.NET Core localization services in `Program.cs` with `AddLocalization` and `AddViewLocalization` pointing to the `AgriMarket.Resources` assembly.

#### Scenario: Service registration
- **WHEN** the application starts
- **THEN** `IStringLocalizer<SharedResource>` is resolvable from the DI container

### Requirement: Supported cultures configured
The application SHALL configure request localization with English (`en`) as the default culture and Estonian (`et`) as a supported culture.

#### Scenario: Default culture
- **WHEN** a request is made without any culture preference
- **THEN** the application uses English (`en`) as the culture

#### Scenario: Estonian culture supported
- **WHEN** a request includes a culture preference for Estonian (`et`)
- **THEN** the application uses Estonian as the culture

### Requirement: Cookie-based culture provider
The application SHALL use `CookieRequestCultureProvider` as the mechanism for persisting the user's language choice.

#### Scenario: Culture persisted in cookie
- **WHEN** a user sets their language preference to Estonian
- **THEN** the `.AspNetCore.Culture` cookie is set with value `c=et|uic=et`
- **AND** subsequent requests use Estonian as the culture

### Requirement: Fallback to default culture
When a localization key is missing from a non-default culture's .resx file, the application SHALL fall back to the default culture (English) value rather than displaying the raw key name.

#### Scenario: Missing Estonian translation falls back to English
- **WHEN** a view renders with Estonian culture
- **AND** a key exists in `SharedResource.resx` but not in `SharedResource.et.resx`
- **THEN** the English value is displayed

### Requirement: Culture cookie lifetime
The culture cookie SHALL be set with an explicit expiry (365 days) so the user's language preference persists across browser sessions.

#### Scenario: Cookie persists after browser restart
- **WHEN** a user sets their language preference
- **AND** closes and reopens the browser
- **THEN** the language preference is still active

### Requirement: IStringLocalizer available in views
All Razor views SHALL have access to `IStringLocalizer<SharedResource>` via `@inject` in `_ViewImports.cshtml`.

#### Scenario: Localizer injected in views
- **WHEN** a Razor view renders
- **THEN** `@Localizer["KeyName"]` resolves to the correct localized string for the current culture

### Requirement: HTML lang attribute reflects culture
The `<html lang="...">` attribute in all layouts SHALL dynamically reflect the current request culture using `CultureInfo.CurrentUICulture.TwoLetterISOLanguageName`.

#### Scenario: English lang attribute
- **WHEN** the culture is English
- **THEN** the HTML element has `lang="en"`

#### Scenario: Estonian lang attribute
- **WHEN** the culture is Estonian
- **THEN** the HTML element has `lang="et"`
