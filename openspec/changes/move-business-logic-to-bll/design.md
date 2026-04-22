## Context

The AgriMarket solution has three presentation/API entry points (Web Admin, Web Client, API) that all need to perform the same business operations (create listings, book services, submit reviews, etc.). A BLL layer exists with service interfaces, but services currently accept and return domain entities directly. Controllers construct entities, set IDs, calculate derived values, and validate business rules — duplicating logic across Web and API.

The Auth services already follow the correct pattern: they accept DTOs (`LoginRequest`, `RegisterRequest`) and return DTOs (`LoginResult`, `TokenResponse`). The rest of the BLL needs to follow the same approach.

## Goals / Non-Goals

**Goals:**
- Define input/output DTOs for all BLL service methods that currently accept/return entities
- Move entity construction, ID generation, default value assignment, and business-rule validation into BLL services
- Consolidate all DTOs under `AgriMarket.BLL/Dtos/` (move existing API DTOs there)
- Slim controllers down to: receive request → map to DTO → call service → map result → return response
- Keep controller mapping logic in explicit manual mapper classes/extensions per project (Web and API), not inline blocks
- Maintain identical external behavior (same HTTP endpoints, same MVC routes, same response shapes)

**Non-Goals:**
- Changing API endpoint routes, HTTP verbs, or response JSON shapes
- Changing MVC view rendering or ViewModel structures
- Introducing AutoMapper or any mapping library (manual mapping is sufficient at this scale)
- Adding new features or endpoints
- Changing the DAL or domain entities
- Refactoring Auth services (they already use DTOs correctly)
- Adding a repository pattern or unit-of-work abstraction

## Decisions

### Decision 1: DTOs live in `AgriMarket.BLL/Dtos/`, organized by domain area

All service-layer DTOs go under `AgriMarket.BLL/Dtos/` in subfolders: `Listings/`, `Bookings/`, `Reviews/`, `Users/`, `Categories/`, `Dashboard/`. The existing `Auth/` folder stays as-is.

**Alternative considered**: A separate `AgriMarket.Dto` class library project. Rejected — both Web and API already reference BLL, so a new project adds a dependency without benefit at this solution size.

### Decision 2: Services accept input DTOs and return output DTOs — never entities

Every service method that currently takes or returns an entity will be changed. For example:
- `CreateAsync(ServiceListing entity)` → `CreateAsync(Guid providerUserId, CreateListingDto dto)` returning `ListingDto`
- `CreateAsync(Booking entity)` → `CreateAsync(Guid clientUserId, CreateBookingDto dto)` returning `BookingDto`

Services internally handle: resolving user profiles from the userId, constructing entities, generating IDs, setting timestamps, calculating derived values (e.g. `TotalPrice`), and enforcing business rules.

The userId parameter is passed explicitly by controllers from the authenticated claims — services never read `HttpContext`.

**Alternative considered**: Embedding the userId inside the DTO. Rejected — the userId comes from auth claims and is a controller concern, not a caller-supplied field.

### Decision 3: API DTOs are removed; API controllers use BLL DTOs directly

The existing `AgriMarket.Api/Dtos/` folder is deleted. API controllers consume and return BLL DTOs directly. If an API shape must differ in the future, that will be handled in a separate change.

### Decision 4: Web ViewModels remain in the Web project; controllers map ViewModel ↔ DTO

ViewModels contain UI concerns (`SelectListItem`, display strings, pagination state) that don't belong in BLL. Web controllers will:
- **Inbound**: Extract relevant fields from the ViewModel and construct the BLL input DTO
- **Outbound**: Receive the BLL output DTO and map it into a ViewModel, adding UI-specific data (dropdown lists, computed display strings)
Mappings are implemented in project-local manual mapper classes/extensions and invoked by controllers, rather than inlined repeatedly inside action methods.

### Decision 5: Business rules move into services with result types for expected failures

Service methods that can fail for business reasons (e.g., self-booking, reviewing a non-completed booking, deleting a listing with active bookings) will throw a `BusinessRuleException` (a custom exception in the BLL) with a descriptive message. Controllers catch this and return appropriate HTTP status codes (400/422 for API, ModelState errors or redirects for MVC).

**Alternative considered**: A `Result<T>` return type. Rejected — adds complexity across all methods when only a few have business-rule failures. Exceptions are the simpler pattern for this scale.

## Risks / Trade-offs

- **Breaking change risk to API response shapes** → Mitigated by designing BLL output DTOs to match existing API DTO shapes. The `fix-api-bll-integration` change should be applied first so API controllers already use BLL services before this change modifies service signatures.
- **Large surface area** → 6 service interfaces change signatures. Mitigated by working service-by-service with compilation checks after each.
- **EF Core Include chains** → Services must ensure the correct `Include` calls to populate data needed by DTOs. Existing includes in the DAL context should be verified per service method.
- **Parallel change conflict** → The `fix-api-bll-integration` change modifies API controllers to use BLL services. This change modifies BLL service signatures. Apply `fix-api-bll-integration` first, then this change.

## Migration Plan

1. Apply `fix-api-bll-integration` first (wires API controllers to existing BLL services)
2. Create all BLL DTOs under `AgriMarket.BLL/Dtos/`
3. Add `BusinessRuleException` to BLL
4. Refactor services one at a time: ListingService → BookingService → ReviewService → UserService → CategoryService → DashboardService
5. For each service: update interface → update implementation → update Web controllers → update API controllers → run tests
6. Delete `AgriMarket.Api/Dtos/` folder after all API controllers are migrated
7. Final full build and test pass
