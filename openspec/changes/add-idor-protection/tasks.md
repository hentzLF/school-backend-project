## 1. Break Request DTOs (BREAKING)

- [ ] 1.1 Remove `ClientProfileId` from `CreateBookingRequest`
- [ ] 1.2 Remove `UserProfileId` from `CreateListingRequest`
- [ ] 1.3 Remove `ReviewerProfileId` from `CreateReviewRequest`

## 2. Add [Authorize] to Mutation Endpoints

- [ ] 2.1 Add `[Authorize]` to `POST`, `PATCH` actions in `BookingsController`
- [ ] 2.2 Add `[Authorize]` to `POST`, `PUT`, `DELETE` actions in `ListingsController`
- [ ] 2.3 Add `[Authorize]` to `POST` action in `ReviewsController`

## 3. Derive Owner IDs from JWT in Create Endpoints

- [ ] 3.1 In `BookingsController.Create`: extract `profileId` claim and set `booking.ClientProfileId`
- [ ] 3.2 In `ListingsController.Create`: extract `profileId` claim and set `listing.UserProfileId`
- [ ] 3.3 In `ReviewsController.Create`: extract `profileId` claim and set `review.ReviewerProfileId`

## 4. Add Ownership Checks to Listing Mutations

- [ ] 4.1 In `ListingsController.Update`: load listing, compare `UserProfileId` to caller `profileId`, return 403 on mismatch
- [ ] 4.2 In `ListingsController.Delete`: same ownership check, return 403 on mismatch

## 5. Scope and Guard Booking Endpoints

- [ ] 5.1 In `BookingsController.GetAll`: add `[Authorize]`, filter query to bookings where `ClientProfileId == callerProfile OR ServiceListing.UserProfileId == callerProfile` (requires `.Include(b => b.ServiceListing)`)
- [ ] 5.2 In `BookingsController.GetById`: add `[Authorize]`, after fetch check caller is client or provider, return 403 if neither

## 6. Implement Role-Gated Booking Status Transitions

- [ ] 6.1 In `BookingsController.UpdateStatus`: load booking with `ServiceListing` included
- [ ] 6.2 Determine caller role (client / provider / neither); return 403 if neither
- [ ] 6.3 Define allowed transitions per role and validate requested transition; return 422 if disallowed

## 7. Protect User Profile Email

- [ ] 7.1 In `UsersController.GetById`: extract `sub` claim (AppUser.Id); only populate `Email` in response when `profile.AppUserId == callerUserId`
- [ ] 7.2 Ensure the endpoint remains publicly accessible (no `[Authorize]` required) — email is simply omitted for non-owners
