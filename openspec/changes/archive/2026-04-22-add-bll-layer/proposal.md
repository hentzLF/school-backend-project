## Why

Both `AgriMarket.Api` and `AgriMarket.Web` currently inject `AppDbContext` directly in their controllers, meaning there is no shared business logic layer — query logic, validation, and domain rules are duplicated or missing entirely. Introducing a dedicated `AgriMarket.BLL` project centralises all service logic so both consumers enforce the same rules.

## What Changes

- **New project** `AgriMarket.BLL` added to the solution, referencing `AgriMarket.DAL` and `AgriMarket.Domain`
- **BREAKING**: `AuthService` and `TokenService` move from `AgriMarket.Api/Services/` to `AgriMarket.BLL/Services/` with updated namespaces
- New service classes added to BLL: `BookingService`, `ListingService`, `ReviewService`, `UserService`
- `AgriMarket.Api` updated to reference `AgriMarket.BLL` instead of calling `AppDbContext` directly in controllers (where applicable)
- `AgriMarket.Web` controllers refactored to inject BLL service interfaces instead of `AppDbContext`
- Service interfaces defined alongside implementations in BLL (`IBookingService`, `IListingService`, etc.)
- DI registrations updated in both `AgriMarket.Api` and `AgriMarket.Web` `Program.cs`

## Capabilities

### New Capabilities

- `bll-core`: The `AgriMarket.BLL` project itself — project file, namespace structure, DI extension method, service interfaces and implementations for Booking, Listing, Review, User, Auth, and Token

### Modified Capabilities

_(none — no existing spec files exist yet)_

## Impact

- **AgriMarket.Api**: removes direct `AppDbContext` usage from controllers; depends on BLL interfaces; `AuthService`/`TokenService` namespaces change
- **AgriMarket.Web**: all admin and client controllers stop injecting `AppDbContext` directly; depend on BLL interfaces instead
- **AgriMarket.DAL**: no changes — still owns `AppDbContext` and migrations
- **AgriMarket.Tests**: test targets shift to BLL services rather than controllers hitting the DB directly
- **Solution file**: new project reference added
