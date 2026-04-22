## Why

Web MVC and API controllers currently construct domain entities directly, perform business-rule validation (price calculation, ownership checks, availability guards), and pass pre-built entities to BLL services that act as thin CRUD wrappers. This means business logic is duplicated across two presentation layers and cannot be tested without spinning up controllers. Consolidating logic and DTOs in the BLL makes both surfaces thinner, testable, and consistent.

## What Changes

- Move all API-project DTOs (`AgriMarket.Api/Dtos/`) into `AgriMarket.BLL/Dtos/` so both Web and API share one set of service-layer DTOs
- Add input/output DTOs for every BLL service method that currently accepts or returns a domain entity (e.g. `CreateListingDto`, `ListingDto`, `CreateBookingDto`, `BookingDto`, `CreateAvailabilityDto`, `CreateReviewDto`, `ReviewDto`)
- Rewrite BLL service interfaces and implementations to accept DTOs instead of entities — services own entity construction, ID generation, default values, and business-rule validation
- Refactor Web MVC controllers to map ViewModel → DTO before calling services and map returned DTOs → ViewModel
- Refactor API controllers to use the shared BLL DTOs directly as request/response shapes (or map where the API shape differs)
- Remove entity references (`using AgriMarket.Domain.Entities`) from all controller files

## Capabilities

### New Capabilities

- `bll-dto-contracts`: Defines the shared DTO types that form the BLL service contract, replacing direct entity usage at service boundaries

### Modified Capabilities

- `bll-core`: Service interfaces change from entity-based to DTO-based signatures
- `service-listings-api`: API controllers delegate to BLL instead of constructing entities
- `bookings-api`: API controllers delegate to BLL instead of constructing entities
- `reviews-api`: API controllers delegate to BLL instead of constructing entities
- `users-api`: API controllers delegate to BLL instead of constructing entities
- `provider-listing-management`: Web MVC controllers map ViewModel↔DTO instead of building entities
- `client-booking-management-mvc`: Web MVC controllers map ViewModel↔DTO instead of building entities
- `client-listing-browse-mvc`: Booking action delegates entity construction to BLL

## Impact

- **AgriMarket.BLL** — new `Dtos/` folder with shared DTOs; all service interfaces and implementations change signatures
- **AgriMarket.Api** — `Dtos/` folder removed; controllers import DTOs from BLL; entity references removed
- **AgriMarket.Web** — controllers gain ViewModel↔DTO mapping; entity references removed from controllers
- **AgriMarket.Tests** — test helpers and assertions updated to match new service signatures
- No database or migration changes — domain entities and DAL are untouched
