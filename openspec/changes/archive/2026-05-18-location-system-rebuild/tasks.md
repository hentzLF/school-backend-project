## 1. Domain Entity'd

- [x] 1.1 Loo `County` entity (`Id`, `Name`, `EhakCode`) faili `AgriMarket.Domain/Entities/County.cs`
- [x] 1.2 Loo `Municipality` entity (`Id`, `Name`, `EhakCode`, `CountyId` FK) faili `AgriMarket.Domain/Entities/Municipality.cs`
- [x] 1.3 Refaktoreeri `Location` entity: eemalda `City`, `Country`, `PostalCode`; lisa `MunicipalityId` FK; tee `Address`, `Latitude`, `Longitude` nullable'iks
- [x] 1.4 Git commit: `feat: add County and Municipality entities, restructure Location`

## 2. DbContext ja seeding

- [x] 2.1 Lisa `DbSet<County>` ja `DbSet<Municipality>` AppDbContext'i
- [x] 2.2 Konfigureeri County → Municipality seos (`DeleteBehavior.Restrict`) ja unikaalne indeks `EhakCode`-le
- [x] 2.3 Konfigureeri Municipality → Location seos (`DeleteBehavior.Restrict`)
- [x] 2.4 Muuda ServiceListing → Location seos `DeleteBehavior.SetNull` → `DeleteBehavior.Cascade`
- [x] 2.5 Lisa `HasData()` seeding 15 maakonna jaoks staatiliste Guid-idega
- [x] 2.6 Lisa `HasData()` seeding 79 omavalitsuse jaoks staatiliste Guid-idega, viidates County Guid-idele
- [x] 2.7 Loo EF Core migratsioon ja kontrolli, et `dotnet ef database update` töötab
- [x] 2.8 Git commit: `feat: add County/Municipality seeding and Location migration`

## 3. DTO-d

- [x] 3.1 Loo `CountyDto` record (`Id`, `Name`, `EhakCode`) faili `AgriMarket.BLL/Dtos/Locations/CountyDto.cs`
- [x] 3.2 Loo `MunicipalityDto` record (`Id`, `Name`, `EhakCode`, `CountyId`) faili `AgriMarket.BLL/Dtos/Locations/MunicipalityDto.cs`
- [x] 3.3 Loo `LocationDto` record (`Id`, `MunicipalityId`, `MunicipalityName`, `CountyId`, `CountyName`, `Address`, `Latitude`, `Longitude`) faili `AgriMarket.BLL/Dtos/Locations/LocationDto.cs`
- [x] 3.4 Loo `CreateLocationDto` record (`MunicipalityId` required, `Address?`, `Latitude?`, `Longitude?`) koos valideerimisega
- [x] 3.5 Loo `UpdateLocationDto` record (sama struktuur kui CreateLocationDto)
- [x] 3.6 Muuda `CreateListingDto`: asenda `LocationId` → `CreateLocationDto? Location`
- [x] 3.7 Muuda `UpdateListingDto`: asenda `LocationId` → `UpdateLocationDto? Location`
- [x] 3.8 Muuda `ListingDto`: asenda `LocationId` → `LocationDto? Location`
- [x] 3.9 Git commit: `feat: add location DTOs and update listing DTOs`

## 4. Teenuskiht (BLL)

- [x] 4.1 Loo `ILocationLookupService` interface (GetAllCountiesAsync, GetMunicipalitiesByCountyAsync)
- [x] 4.2 Loo `LocationLookupService` implementatsioon geneerilise repository abil
- [x] 4.3 Muuda `ListingService.CreateAsync`: loo Location inline CreateLocationDto põhjal
- [x] 4.4 Muuda `ListingService.UpdateAsync`: uuenda/loo/kustuta Location inline UpdateLocationDto põhjal
- [x] 4.5 Muuda `ListingService` query'd: Include Location.Municipality.County listing päringutes
- [x] 4.6 Muuda `ListingService` DTO mapping: ehita nested LocationDto response'idesse
- [x] 4.7 Lisa MunicipalityId valideerimine (kas eksisteerib andmebaasis)
- [x] 4.8 Lisa koordinaatide valideerimine (lat -90..90, lon -180..180, mõlemad või kumbki)
- [x] 4.9 Git commit: `feat: add LocationLookupService and inline location management in ListingService`

## 5. API kiht

- [x] 5.1 Loo `CountiesController` endpoint'iga `GET /api/v1/counties`
- [x] 5.2 Lisa endpoint `GET /api/v1/counties/{countyId}/municipalities` samasse controllerisse
- [x] 5.3 Registreeri `ILocationLookupService` DI konteineris `Program.cs`-s
- [x] 5.4 Git commit: `feat: add CountiesController with municipalities endpoint`

## 6. Testid

- [x] 6.1 Unit testid: County ja Municipality entity valideerimine
- [x] 6.2 Unit testid: CreateLocationDto ja UpdateLocationDto valideerimine (koordinaadid, lat+lon paarsus)
- [x] 6.3 Unit testid: LocationLookupService (counties, municipalities by county)
- [x] 6.4 Unit testid: ListingService inline Location loomine/uuendamine/kustutamine
- [x] 6.5 Integration testid: Counties ja Municipalities endpoint'id
- [x] 6.6 Integration testid: Listing CRUD uue Location struktuuriga
- [x] 6.7 Git commit: `test: add location system unit and integration tests`

## 7. Lõppkontroll

- [x] 7.1 `dotnet build` kompileerub vigadeta
- [x] 7.2 `dotnet test` kõik testid lähevad läbi
- [ ] 7.3 Swagger UI-s testida counties, municipalities ja listing endpoint'e
