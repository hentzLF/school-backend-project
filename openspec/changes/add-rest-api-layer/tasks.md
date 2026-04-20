## 1. Project Scaffold

- [x] 1.1 Create `AgriMarket.Api` ASP.NET Core Web API project (`dotnet new webapi`)
- [x] 1.2 Add `AgriMarket.Api` to `AgriMarket.slnx`
- [x] 1.3 Add project references to `AgriMarket.DAL` and `AgriMarket.Domain`
- [x] 1.4 Configure `Program.cs`: register `AppDbContext` with Npgsql, add camelCase JSON serialization, enable ProblemDetails
- [x] 1.5 Copy `DefaultConnection` connection string into `appsettings.json` and `appsettings.Development.json`
- [x] 1.6 Verify `dotnet build` succeeds for the full solution

## 2. Service Listings — DTOs and Controller

- [ ] 2.1 Create `Dtos/ServiceListings/ServiceListingResponse.cs` with fields: `Id`, `Title`, `Description`, `PricePerHectare`, `IsActive`, `UserProfileId`, `ServiceCategoryId`, `LocationId`
- [ ] 2.2 Create `Dtos/ServiceListings/CreateListingRequest.cs` with required fields and data annotations for validation
- [ ] 2.3 Create `Dtos/ServiceListings/UpdateListingRequest.cs`
- [ ] 2.4 Create `Controllers/ListingsController.cs` with `GET /api/listings` (paginated), `GET /api/listings/{id}`, `POST /api/listings`, `PUT /api/listings/{id}`, `DELETE /api/listings/{id}`
- [ ] 2.5 Verify all listing endpoints return correct status codes and ProblemDetails on errors

## 3. Bookings — DTOs and Controller

- [ ] 3.1 Create `Dtos/Bookings/BookingResponse.cs` with fields: `Id`, `Status`, `TotalPrice`, `AreaInHectares`, `CreatedAt`, `Notes`, `ServiceListingId`, `ClientProfileId`, `AvailabilityId`
- [ ] 3.2 Create `Dtos/Bookings/CreateBookingRequest.cs` with required fields and validation
- [ ] 3.3 Create `Dtos/Bookings/UpdateBookingStatusRequest.cs` with a `Status` field
- [ ] 3.4 Create `Controllers/BookingsController.cs` with `GET /api/bookings` (paginated), `GET /api/bookings/{id}`, `POST /api/bookings`, `PATCH /api/bookings/{id}/status`
- [ ] 3.5 Verify booking endpoints return correct status codes and ProblemDetails on errors

## 4. Users — DTOs and Controller

- [ ] 4.1 Create `Dtos/Users/UserProfileResponse.cs` with fields: `Id`, `FirstName`, `LastName`, `Bio`, `AvatarUrl`, `AppUserId`, `Email`
- [ ] 4.2 Create `Controllers/UsersController.cs` with `GET /api/users` (paginated), `GET /api/users/{id}` (eager-loads `AppUser` for email)
- [ ] 4.3 Verify user endpoints return correct status codes and ProblemDetails on errors

## 5. Reviews — DTOs and Controller

- [ ] 5.1 Create `Dtos/Reviews/ReviewResponse.cs` with fields: `Id`, `Rating`, `Comment`, `CreatedAt`, `BookingId`, `ReviewerProfileId`
- [ ] 5.2 Create `Dtos/Reviews/CreateReviewRequest.cs` with required fields and `[Range(1, 5)]` on `Rating`
- [ ] 5.3 Create `Controllers/ReviewsController.cs` with `GET /api/reviews` (paginated), `GET /api/reviews/{id}`, `POST /api/reviews`
- [ ] 5.4 Verify review endpoints return correct status codes and ProblemDetails on errors

## 6. Swagger

- [ ] 6.1 Add `Swashbuckle.AspNetCore` NuGet package to `AgriMarket.Api`
- [ ] 6.2 Register `AddSwaggerGen()` and `AddEndpointsApiExplorer()` in `Program.cs`
- [ ] 6.3 Add `UseSwagger()` and `UseSwaggerUI()` inside `if (app.Environment.IsDevelopment())` block
- [ ] 6.4 Verify Swagger UI loads at `/swagger` in Development and lists all four controllers
