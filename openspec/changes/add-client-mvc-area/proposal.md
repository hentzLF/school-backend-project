## Why

The web app currently has only an Admin MVC area and an admin-oriented account flow, so clients cannot browse services or manage bookings through a dedicated client-facing experience. We need a functional Client area now to support core booking journeys while keeping admin and client sign-in flows clearly separated.

## What Changes

- Add a new MVC `Client` area with its own controllers, view models, views, and area-level layout conventions, mirroring the established Admin area structure.
- Add client-facing pages for listing browse and listing details.
- Add client booking flow pages to create bookings from listings and manage bookings (view history/status and confirm completion).
- Add client account pages for registration, login, logout, and basic profile management.
- Split web login endpoints and UI by audience (`Admin` vs `Client`) while preserving role-based authorization boundaries.
- Update shared web cookie authentication behavior to support distinct login and access-denied routes for each audience.

## Capabilities

### New Capabilities
- `client-area-layout`: MVC area structure, routing, shared layout, and view conventions for the Client area.
- `client-listing-browse-mvc`: Client pages for browsing active listings and viewing listing details.
- `client-booking-management-mvc`: Client pages for creating bookings from listings and managing booking lifecycle actions available to clients (including completion confirmation).
- `client-account-management-mvc`: Client registration (with Farmer/Provider role selection), login, logout, and profile management pages with role-appropriate behavior.

### Modified Capabilities
- `web-cookie-auth`: Extend cookie auth to route unauthenticated/unauthorized requests to audience-specific login/access-denied endpoints via `OnRedirectToLogin`/`OnRedirectToAccessDenied` event handlers (single cookie scheme retained).
- `auth-login`: Align web login behavior with separated admin/client entry points while keeping credential validation and role checks explicit for each flow.

## Impact

- Affected web project: `AgriMarket.Web` (new `Areas/Client/*`, account/auth controllers/views, route registration, policies).
- Affected OpenSpec coverage: new client MVC capability specs and deltas for existing auth-related capabilities.
- No API contract break is expected for existing JSON API endpoints; impact is concentrated in MVC web surface and authentication UX/routing behavior.
