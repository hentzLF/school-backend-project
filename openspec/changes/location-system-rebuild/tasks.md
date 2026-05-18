## 1. Domain Entity'd

- [ ] 1.1 Loo `County` entity (`Id`, `Name`, `EhakCode`) faili `AgriMarket.Domain/Entities/County.cs`
- [ ] 1.2 Loo `Municipality` entity (`Id`, `Name`, `EhakCode`, `CountyId` FK) faili `AgriMarket.Domain/Entities/Municipality.cs`
- [ ] 1.3 Refaktoreeri `Location` entity: eemalda `City`, `Country`, `PostalCode`; lisa `MunicipalityId` FK; tee `Address`, `Latitude`, `Longitude` nullable'iks
- [ ] 1.4 Git commit: `feat: add County and Municipality entities, restructure Location`

## 2. DbContext ja seeding

- [ ] 2.1 Lisa `DbSet<County>` ja `DbSet<Municipality>` AppDbContext'i
- [ ] 2.2 Konfigureeri County → Municipality seos (`DeleteBehavior.Restrict`) ja unikaalne indeks `EhakCode`-le
- [ ] 2.3 Konfigureeri Municipality → Location seos (`DeleteBehavior.Restrict`)
- [ ] 2.4 Muuda ServiceListing → Location seos `DeleteBehavior.SetNull` → `DeleteBehavior.Cascade`
- [ ] 2.5 Lisa `HasData()` seeding 15 maakonna jaoks staatiliste Guid-idega
- [ ] 2.6 Lisa `HasData()` seeding 79 omavalitsuse jaoks staatiliste Guid-idega, viidates County Guid-idele
- [ ] 2.7 Loo EF Core migratsioon ja kontrolli, et `dotnet ef database update` töötab
- [ ] 2.8 Git commit: `feat: add County/Municipality seeding and Location migration`

## 3. DTO-d

- [ ] 3.1 Loo `CountyDto` record (`Id`, `Name`, `EhakCode`) faili `AgriMarket.BLL/Dtos/Locations/CountyDto.cs`
- [ ] 3.2 Loo `MunicipalityDto` record (`Id`, `Name`, `EhakCode`, `CountyId`) faili `AgriMarket.BLL/Dtos/Locations/MunicipalityDto.cs`
- [ ] 3.3 Loo `LocationDto` record (`Id`, `MunicipalityId`, `MunicipalityName`, `CountyId`, `CountyName`, `Address`, `Latitude`, `Longitude`) faili `AgriMarket.BLL/Dtos/Locations/LocationDto.cs`
- [ ] 3.4 Loo `CreateLocationDto` record (`MunicipalityId` required, `Address?`, `Latitude?`, `Longitude?`) koos valideerimisega
- [ ] 3.5 Loo `UpdateLocationDto` record (sama struktuur kui CreateLocationDto)
- [ ] 3.6 Muuda `CreateListingDto`: asenda `LocationId` → `CreateLocationDto? Location`
- [ ] 3.7 Muuda `UpdateListingDto`: asenda `LocationId` → `UpdateLocationDto? Location`
- [ ] 3.8 Muuda `ListingDto`: asenda `LocationId` → `LocationDto? Location`
- [ ] 3.9 Git commit: `feat: add location DTOs and update listing DTOs`

## 4. Teenuskiht (BLL)

- [ ] 4.1 Loo `ILocationLookupService` interface (GetAllCountiesAsync, GetMunicipalitiesByCountyAsync)
- [ ] 4.2 Loo `LocationLookupService` implementatsioon geneerilise repository abil
- [ ] 4.3 Muuda `ListingService.CreateAsync`: loo Location inline CreateLocationDto põhjal
- [ ] 4.4 Muuda `ListingService.UpdateAsync`: uuenda/loo/kustuta Location inline UpdateLocationDto põhjal
- [ ] 4.5 Muuda `ListingService` query'd: Include Location.Municipality.County listing päringutes
- [ ] 4.6 Muuda `ListingService` DTO mapping: ehita nested LocationDto response'idesse
- [ ] 4.7 Lisa MunicipalityId valideerimine (kas eksisteerib andmebaasis)
- [ ] 4.8 Lisa koordinaatide valideerimine (lat -90..90, lon -180..180, mõlemad või kumbki)
- [ ] 4.9 Git commit: `feat: add LocationLookupService and inline location management in ListingService`

## 5. API kiht

- [ ] 5.1 Loo `CountiesController` endpoint'iga `GET /api/v1/counties`
- [ ] 5.2 Lisa endpoint `GET /api/v1/counties/{countyId}/municipalities` samasse controllerisse
- [ ] 5.3 Registreeri `ILocationLookupService` DI konteineris `Program.cs`-s
- [ ] 5.4 Git commit: `feat: add CountiesController with municipalities endpoint`

## 6. Testid

- [ ] 6.1 Unit testid: County ja Municipality entity valideerimine
- [ ] 6.2 Unit testid: CreateLocationDto ja UpdateLocationDto valideerimine (koordinaadid, lat+lon paarsus)
- [ ] 6.3 Unit testid: LocationLookupService (counties, municipalities by county)
- [ ] 6.4 Unit testid: ListingService inline Location loomine/uuendamine/kustutamine
- [ ] 6.5 Integration testid: Counties ja Municipalities endpoint'id
- [ ] 6.6 Integration testid: Listing CRUD uue Location struktuuriga
- [ ] 6.7 Git commit: `test: add location system unit and integration tests`

## 7. Lõppkontroll

- [ ] 7.1 `dotnet build` kompileerub vigadeta
- [ ] 7.2 `dotnet test` kõik testid lähevad läbi
- [ ] 7.3 Swagger UI-s testida counties, municipalities ja listing endpoint'e
