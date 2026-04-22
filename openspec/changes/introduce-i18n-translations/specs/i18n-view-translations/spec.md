## ADDED Requirements

### Requirement: All hardcoded UI strings replaced
All hardcoded user-facing strings in Razor views across Admin and Client areas SHALL be replaced with `@Localizer["KeyName"]` calls.

#### Scenario: English rendering
- **WHEN** a view renders with English culture
- **THEN** all UI text displays in English, matching current hardcoded values

#### Scenario: Estonian rendering
- **WHEN** a view renders with Estonian culture
- **THEN** all UI text displays in Estonian

### Requirement: Resource keys in English .resx
Every localization key used in views SHALL have a corresponding entry in `SharedResource.resx` with the English text value.

#### Scenario: English resource completeness
- **WHEN** a view references `@Localizer["KeyName"]`
- **THEN** `SharedResource.resx` contains an entry for `KeyName` with the English translation

### Requirement: Resource keys in Estonian .resx
Every localization key used in views SHALL have a corresponding entry in `SharedResource.et.resx` with the Estonian text value.

#### Scenario: Estonian resource completeness
- **WHEN** a view references `@Localizer["KeyName"]`
- **THEN** `SharedResource.et.resx` contains an entry for `KeyName` with the Estonian translation

### Requirement: HTML lang attribute reflects culture
The `<html lang="...">` attribute in all layouts SHALL reflect the current culture.

#### Scenario: English lang attribute
- **WHEN** the culture is English
- **THEN** the HTML element has `lang="en"`

#### Scenario: Estonian lang attribute
- **WHEN** the culture is Estonian
- **THEN** the HTML element has `lang="et"`
