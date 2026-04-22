## 1. ViewModels

- [x] 1.1 Create `AvailabilityItemViewModel` (Id, StartTime, EndTime, IsBooked) in `Areas/Client/ViewModels/MyListings/`
- [x] 1.2 Create `ManageAvailabilitiesViewModel` (ListingId, ListingTitle, Availabilities list, AddStartTime, AddEndTime) in `Areas/Client/ViewModels/MyListings/`

## 2. Controller Actions

- [x] 2.1 Implement `MyListingsController.Availabilities(Guid id)` GET — loads owned listing (404 if not owned), projects slots to `AvailabilityItemViewModel`, returns `ManageAvailabilitiesViewModel`
- [x] 2.2 Implement `MyListingsController.AddAvailability(Guid listingId, ManageAvailabilitiesViewModel model)` POST — validates ownership (404), validates StartTime < EndTime (model error), creates `Availability` with `IsBooked = false`, redirects to `Availabilities`
- [x] 2.3 Implement `MyListingsController.DeleteAvailability(Guid id)` POST — loads slot, validates listing ownership (404), rejects if `IsBooked = true` (redirect with error), deletes and redirects to `Availabilities`

## 3. View

- [x] 3.1 Create `Areas/Client/Views/MyListings/Availabilities.cshtml` — shows listing title, table of existing slots (StartTime, EndTime, Booked badge, Delete button hidden/disabled for booked slots), inline add form with StartTime and EndTime inputs and validation summary

## 4. Navigation

- [x] 4.1 Add "Manage Slots" link to `Areas/Client/Views/MyListings/Details.cshtml` pointing to `Availabilities` action

## 5. Verification

- [ ] 5.1 Verify `ProviderOnly` policy blocks Farmer-role users from accessing `/Client/MyListings/Availabilities/{id}`
- [ ] 5.2 Verify a Provider cannot view, add, or delete slots on another Provider's listing (all return 404)
- [ ] 5.3 Verify deletion of a booked slot is rejected
- [ ] 5.4 Verify AddAvailability rejects StartTime >= EndTime with a validation error
- [ ] 5.5 Run manual flow: Provider login → create listing → Manage Slots → add slot → Farmer login → view listing details → confirm booking form appears → submit booking → verify slot shows as Booked in provider's Availabilities page
