## Context

AgriMarket is an ASP.NET Core MVC application (.NET 10) with 51 Razor views across three standalone layouts: root `_Layout.cshtml`, `_AdminLayout.cshtml`, and `_ClientLayout.cshtml`. All UI strings are currently hardcoded in English. The area layouts are standalone (not nested into the root layout).

## Goals / Non-Goals

**Goals:**
- Enable UI localization for English (default) and Estonian across all views
- Provide a simple, cookie-based language switching mechanism
- Centralize all translation strings in a dedicated project for maintainability
- Preserve existing behavior — English remains the default experience

**Non-Goals:**
- Localizing data content (product names, descriptions, categories stored in the database)
- Localizing validation messages or model data annotations (future work)
- URL/route-based culture switching
- Right-to-left language support
- Localizing the API project responses

## Decisions

### 1. Dedicated AgriMarket.Resources project

**Decision**: Create a separate `AgriMarket.Resources` class library to host all .resx files and the marker class.

**Alternatives considered**:
- Embedding resources in `AgriMarket.Web/Resources/` — simpler setup but couples translations to the web project, preventing reuse from BLL or API if needed later.
- Per-view resource files — would create 100+ .resx files with heavy duplication of common strings like "Save", "Cancel", "Delete".

**Rationale**: A dedicated project provides clean separation, a single location for translators, and future extensibility to other layers.

### 2. IStringLocalizer\<SharedResource\> over IViewLocalizer

**Decision**: Use `IStringLocalizer<SharedResource>` injected via `_ViewImports.cshtml` instead of the built-in `IViewLocalizer`.

**Alternatives considered**:
- `IViewLocalizer` — auto-resolves .resx files based on view path, but this couples resource file layout to view paths and doesn't work well with an external resources project.

**Rationale**: A single shared localizer with one set of keys is simpler to manage, avoids namespace/path coupling, and works identically across views, controllers, and services.

### 3. Cookie-based culture provider

**Decision**: Use `CookieRequestCultureProvider` to persist the user's language choice.

**Alternatives considered**:
- Query string (`?culture=et`) — doesn't persist across navigation.
- Route-based (`/{culture}/...`) — SEO-friendly but requires modifying all route definitions and link generation.

**Rationale**: Cookie-based switching is the least invasive approach. A single cookie persists the choice across requests without touching routes or URLs.

### 4. Language switcher in all three layouts

**Decision**: Add a language switcher dropdown to `_Layout.cshtml`, `_AdminLayout.cshtml`, and `_ClientLayout.cshtml` since they are standalone layouts (not nested).

**Rationale**: Each layout renders its own full HTML document. A partial view (`_LanguageSwitcher.cshtml`) keeps the component DRY across all three layouts.

### 5. Resource key naming convention

**Decision**: Use PascalCase descriptive keys (e.g., `SignIn`, `Email`, `BackToMainPage`, `NoAccount`). Shared keys used across views use generic names; view-specific keys use a `Section_Key` prefix only when disambiguation is needed.

**Rationale**: Flat PascalCase keys are simple to reference in views (`@Localizer["SignIn"]`) and easy to search. Prefixing is reserved for when the same English word needs different translations in different contexts.

### 6. Parameterized / interpolated strings

**Decision**: Strings containing dynamic values use `string.Format`-style placeholders in .resx entries (e.g., `Welcome, {0}`) and are invoked via `@Localizer["Welcome", user.Name]`. Translators can reorder placeholders as needed by the target language's grammar.

**Rationale**: This is the built-in mechanism supported by `IStringLocalizer`. It keeps formatting logic out of views while giving translators flexibility over word order.

### 7. Culture cookie lifetime

**Decision**: Set an explicit cookie expiry (e.g., 365 days) so the language preference persists across browser sessions, rather than relying on the default session cookie.

**Rationale**: A session cookie would reset the user's choice every time they close the browser, which is a poor UX for a language preference. A long-lived cookie matches user expectations.

## Risks / Trade-offs

- **Large view diff** — All 51 views will be modified to replace hardcoded strings. Risk of introducing typos or missing strings. → Mitigation: Systematic view-by-view pass with resource key verification.
- **Missing translations** — If a key exists in the English .resx but is missing from the Estonian .resx, ASP.NET Core falls back to the default culture (English). If both are missing, the raw key name is displayed. → Mitigation: Keep both .resx files in sync during development; add a unit test that compares key sets across .resx files to catch drift.
- **Three layouts to maintain** — The language switcher partial must be included in all three standalone layouts. → Mitigation: Use a shared partial view (`_LanguageSwitcher.cshtml`) to keep the component in one place.
