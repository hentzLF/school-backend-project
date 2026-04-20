## Context

`AgriMarket.Web` is an ASP.NET Core MVC/Razor app backed by `AgriMarket.DAL` (EF Core + PostgreSQL via Npgsql) and `AgriMarket.Domain` (entities). There is currently no REST API surface. The new `AgriMarket.Api` project sits alongside `AgriMarket.Web` in the same solution and shares the same database — it references `AgriMarket.DAL` and `AgriMarket.Domain` directly.

## Goals / Non-Goals

**Goals:**
- Expose core resources (`ServiceListing`, `Booking`, `UserProfile`, `Review`) as JSON REST endpoints
- Keep API concerns (routing, serialization, error format, Swagger) isolated from the MVC app
- Follow a consistent DTO pattern that can grow as more resources are added later
- Keep individual commits small (one controller + its DTOs per commit)

**Non-Goals:**
- Authentication/authorization — all endpoints are public for this phase
- `Equipment`, `ServiceCategory`, `Location`, `Availability`, `Conversation`, `Message`, `Notification`, `Payment` endpoints — deferred
- Service/repository layer abstraction — controllers query `AppDbContext` directly for now
- API versioning — out of scope for this phase

## Decisions

### 1. Separate `AgriMarket.Api` project, not a folder inside `AgriMarket.Web`

`AgriMarket.Web` serves Razor views and has MVC-specific middleware. Mixing JSON API controllers into it would couple two different delivery models. A separate project gets its own `Program.cs`, its own Swagger config, and its own middleware pipeline.

*Alternative considered*: add an `Api/` folder inside `AgriMarket.Web` — rejected because it shares the MVC pipeline and makes it harder to split later.

### 2. DTOs in `AgriMarket.Api` — separate request and response types

Each resource has distinct request and response shapes:
- `Create<Resource>Request` — input validation only, no navigation properties
- `<Resource>Response` — flattened output safe to serialize, no EF navigation cycles

*Alternative considered*: single `<Resource>Dto` for both — rejected because it either leaks internal fields or forces nullable properties on required inputs.

### 3. Controllers query `AppDbContext` directly

No repository or service layer is introduced in this phase. Controllers inject `AppDbContext` and use LINQ queries. This keeps the scope small and matches the current pattern in the solution.

*Alternative considered*: add a service layer first — rejected as premature for this phase; the learning goal is controllers + DTOs, not layering.

### 4. ProblemDetails error format (RFC 7807)

`AddProblemDetails()` + `UseExceptionHandler` configured in `Program.cs`. All error responses return `{ type, title, status, detail }` JSON. This is the ASP.NET Core built-in and requires no extra packages.

### 5. Pagination via `page` + `pageSize` query params

List endpoints accept `?page=1&pageSize=20`. Default page size is 20, max is 100. Response wraps items in `{ items, page, pageSize, totalCount }`.

### 6. Swashbuckle for Swagger

`Swashbuckle.AspNetCore` is the standard choice for ASP.NET Core. Swagger UI enabled in Development only, mirroring the pattern in `AgriMarket.Web`.

## Risks / Trade-offs

- **Direct DbContext in controllers** → harder to unit test. Acceptable for this learning phase; a service layer can be extracted later without changing the API contract.
- **No auth** → all data is publicly readable/writable. Acceptable because this is a development/learning environment; auth is a future change.
- **Shared database with `AgriMarket.Web`** → both apps must use the same connection string. No risk now since they don't conflict, but worth noting.

## Migration Plan

1. Scaffold `AgriMarket.Api` project and add to `AgriMarket.slnx`
2. Copy connection string from `AgriMarket.Web/appsettings.json`
3. Add controllers one at a time, each with its DTOs, each as its own commit
4. Add Swagger last (after at least one controller exists to document)

No database migrations required — this phase adds no new tables.

## Open Questions

- Should `UserProfile` endpoints return the linked `AppUser.Email`, or keep that internal? (Defaulting to: include email in response for now since there's no auth to protect it.)

