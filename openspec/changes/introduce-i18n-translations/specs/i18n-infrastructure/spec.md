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

### Requirement: IStringLocalizer available in views
All Razor views SHALL have access to `IStringLocalizer<SharedResource>` via `@inject` in `_ViewImports.cshtml`.

#### Scenario: Localizer injected in views
- **WHEN** a Razor view renders
- **THEN** `@Localizer["KeyName"]` resolves to the correct localized string for the current culture
