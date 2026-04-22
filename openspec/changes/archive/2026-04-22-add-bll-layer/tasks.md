## 1. Project Setup

- [x] 1.1 Create `AgriMarket.BLL` class library project (`dotnet new classlib`)
- [x] 1.2 Add `AgriMarket.BLL` to the solution file (`dotnet sln add`)
- [x] 1.3 Add project reference BLL → DAL (`AgriMarket.DAL`)
- [x] 1.4 Add project reference BLL → Domain (`AgriMarket.Domain`)
- [x] 1.5 Add project reference Api → BLL (`AgriMarket.BLL`)
- [x] 1.6 Add project reference Web → BLL (`AgriMarket.BLL`)

## 2. Migrate Existing Auth Services

- [x] 2.1 Copy `IAuthService.cs` and `AuthService.cs` from `AgriMarket.Api/Services/` into `AgriMarket.BLL/Services/` and update namespaces to `AgriMarket.BLL.Services`
- [x] 2.2 Copy `ITokenService.cs` and `TokenService.cs` from `AgriMarket.Api/Services/` into `AgriMarket.BLL/Services/` and update namespaces
- [x] 2.3 Delete the original files from `AgriMarket.Api/Services/`
- [x] 2.4 Update all `using` statements in `AgriMarket.Api` that referenced the old namespace

## 3. Implement New BLL Services

- [x] 3.1 Create `AgriMarket.BLL/Services/IBookingService.cs` with methods: `GetAllAsync`, `GetByIdAsync`, `GetByClientAsync`, `GetByProviderAsync`, `CreateAsync`, `UpdateStatusAsync`, `DeleteAsync`
- [x] 3.2 Create `AgriMarket.BLL/Services/BookingService.cs` implementing `IBookingService`, injecting `AppDbContext`, enforcing provider self-booking rule
- [x] 3.3 Create `AgriMarket.BLL/Services/IListingService.cs` with methods: `GetAllAsync`, `GetByIdAsync`, `GetByProviderAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`
- [x] 3.4 Create `AgriMarket.BLL/Services/ListingService.cs` implementing `IListingService`
- [x] 3.5 Create `AgriMarket.BLL/Services/IReviewService.cs` with methods: `GetByBookingAsync`, `CreateAsync`
- [x] 3.6 Create `AgriMarket.BLL/Services/ReviewService.cs` implementing `IReviewService`, rejecting reviews for non-Completed bookings
- [x] 3.7 Create `AgriMarket.BLL/Services/IUserService.cs` with methods: `GetByIdAsync`, `GetAllAsync`, `UpdateAsync`
- [x] 3.8 Create `AgriMarket.BLL/Services/UserService.cs` implementing `IUserService`

## 4. DI Registration

- [x] 4.1 Create `AgriMarket.BLL/BllServiceExtensions.cs` with `AddBll(this IServiceCollection services)` registering all six service interfaces as scoped
- [x] 4.2 Replace individual service registrations in `AgriMarket.Api/Program.cs` with `builder.Services.AddBll()`
- [x] 4.3 Add `builder.Services.AddBll()` to `AgriMarket.Web/Program.cs`

## 5. Refactor Web Controllers

- [x] 5.1 Refactor `AgriMarket.Web/Areas/Admin/Controllers/BookingsController.cs` to inject `IBookingService` instead of `AppDbContext`
- [x] 5.2 Refactor `AgriMarket.Web/Areas/Admin/Controllers/ListingsController.cs` to inject `IListingService`
- [x] 5.3 Refactor `AgriMarket.Web/Areas/Admin/Controllers/UsersController.cs` to inject `IUserService`
- [x] 5.4 Refactor `AgriMarket.Web/Areas/Admin/Controllers/CategoriesController.cs` (inject appropriate service or keep DAL if no BLL service covers it)
- [x] 5.5 Refactor `AgriMarket.Web/Areas/Admin/Controllers/DashboardController.cs`
- [x] 5.6 Refactor `AgriMarket.Web/Areas/Admin/Controllers/PaymentsController.cs`
- [x] 5.7 Refactor `AgriMarket.Web/Areas/Admin/Controllers/AccountController.cs` to inject `IAuthService`
- [x] 5.8 Refactor all Client-area controllers in `AgriMarket.Web/Areas/Client/Controllers/` to use BLL interfaces

## 6. Verification

- [x] 6.1 Confirm solution builds with zero errors (`dotnet build`)
- [x] 6.2 Confirm no `AppDbContext` direct injection remains in any Web controller (grep check)
- [x] 6.3 Run existing test suite and fix any broken tests (`dotnet test`)
