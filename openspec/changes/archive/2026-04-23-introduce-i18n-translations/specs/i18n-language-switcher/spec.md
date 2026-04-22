## ADDED Requirements

### Requirement: Language switcher dropdown
The application SHALL display a language switcher dropdown in the navigation area of all layouts (`_Layout.cshtml`, `_AdminLayout.cshtml`, `_ClientLayout.cshtml`).

#### Scenario: Dropdown displays available languages
- **WHEN** any page is rendered
- **THEN** a dropdown is visible with options "EN" and "ET"
- **AND** the currently active language is indicated

#### Scenario: Switching to Estonian
- **WHEN** a user selects "ET" from the dropdown
- **THEN** the culture cookie is set to Estonian
- **AND** the page reloads in Estonian

#### Scenario: Switching to English
- **WHEN** a user selects "EN" from the dropdown
- **THEN** the culture cookie is set to English
- **AND** the page reloads in English

### Requirement: Language switcher partial view
The language switcher SHALL be implemented as a shared partial view (`_LanguageSwitcher.cshtml`) to avoid duplication across the three standalone layouts.

#### Scenario: Partial view reuse
- **WHEN** any of the three layouts renders
- **THEN** the language switcher is rendered from the same `_LanguageSwitcher.cshtml` partial

### Requirement: Culture controller action
A controller action SHALL exist to receive the language switch request, set the culture cookie, and redirect back to the originating page.

#### Scenario: Set culture and redirect
- **WHEN** the language switcher posts a culture change to the controller
- **THEN** the controller sets the `.AspNetCore.Culture` cookie
- **AND** redirects the user back to the page they were on
