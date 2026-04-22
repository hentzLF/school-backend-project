## Context

The solution has three existing layers: `AgriMarket.Domain` (entities/enums), `AgriMarket.DAL` (EF Core + `AppDbContext`), and two consumer projects — `AgriMarket.Api` (REST API) and `AgriMarket.Web` (MVC frontend). Both consumers currently inject `AppDbContext` directly into controllers, meaning domain rules (e.g. booking validation, pricing calculation) either don't exist or are duplicated. `AgriMarket.Api` has an `AuthService` and `TokenService` but they are local to the Api project and not reusable by Web.

## Goals / Non-Goals

**Goals:**
- Introduce `AgriMarket.BLL` as a class library project between the DAL and the two consumers
- Define service interfaces (`IBookingService`, `IListingService`, `IReviewService`, `IUserService`, `IAuthService`, `ITokenService`) in BLL
- Implement all services in BLL, with `AppDbContext` injected via constructor
- Migrate existing `AuthService` / `TokenService` from `AgriMarket.Api` to BLL
- Refactor Web controllers to use BLL interfaces instead of `AppDbContext`
- Expose a `BllServiceExtensions.AddBll()` DI helper so both Api and Web register services with one call

**Non-Goals:**
- Changing the DAL schema or adding migrations
- Introducing CQRS, MediatR, or any additional architectural patterns
- Adding caching or background jobs
- Changing the REST API contract or MVC routes

## Decisions

**D1 — One BLL project, not per-feature assemblies**
Rationale: the codebase is small and university-scoped. A single `AgriMarket.BLL` project avoids over-engineering while still enforcing the layering rule. If the project grows, services can be split later.

Alternative considered: separate `AgriMarket.BLL.Bookings`, `AgriMarket.BLL.Auth` assemblies — rejected as premature complexity.

**D2 — Interfaces live in BLL, not in Domain**
Rationale: Domain should stay persistence-ignorant and framework-free. Interfaces that reference service-level operations (CRUD, business rules) belong in BLL next to their implementations.

Alternative considered: putting interfaces in Domain — rejected because Domain should not depend on service concepts.

**D3 — Services inject `AppDbContext` directly (no repository pattern)**
Rationale: repository pattern adds indirection without benefit at this scale. EF Core's `DbSet<T>` already acts as a repository. Keeping it simple is the right call for a uni project.

Alternative considered: `IRepository<T>` generic — rejected as unnecessary abstraction.

**D4 — Web controllers map BLL results to ViewModels themselves**
Rationale: ViewModel mapping is presentation-layer concern. BLL services return domain entities or lightweight result objects; controllers handle the ViewModel projection. This avoids BLL taking a dependency on Web-specific types.

## Risks / Trade-offs

- [Breaking namespace change for AuthService/TokenService] → Api DI registrations must be updated; straightforward but easy to miss — covered by tasks checklist
- [Web controllers currently do EF Include chains in-line] → services must replicate necessary eager loading; risk of N+1 if missed → mitigated by reviewing each controller during refactor
- [Tests currently test controllers directly against a real DB] → after refactor, tests can target BLL services directly which is cleaner; no test logic is lost

## Migration Plan

1. Create `AgriMarket.BLL` project and add to solution
2. Add project reference: BLL → DAL, BLL → Domain
3. Move AuthService/TokenService into BLL; update namespaces
4. Add remaining service implementations (Booking, Listing, Review, User)
5. Add `AddBll()` DI extension; register in Api and Web `Program.cs`
6. Add project references: Api → BLL, Web → BLL
7. Refactor Web controllers one-by-one to use service interfaces
8. Remove direct `AppDbContext` injections from Web controllers
9. Build and fix any compilation errors
10. Run existing tests; update as needed

Rollback: revert project references and restore direct `AppDbContext` injection in controllers — no DB migration involved so rollback is safe.

## Open Questions

- Should `ITokenService` remain internal to `IAuthService` or be exposed as a standalone interface? (Current: standalone — keep for now)
- Do Web client-area controllers also need the same refactor? (Assumed yes — include in scope)
