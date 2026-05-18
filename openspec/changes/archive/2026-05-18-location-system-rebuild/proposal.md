## Why

Location entity kasutab praegu free-text välju (City, Country, PostalCode), mis tekitab andmekvaliteedi probleeme — sama linn võib olla "Tartu", "tartu", "Tartu linn". Piirkondlik filtreerimine ("näita Tartumaa teenuseid") on free-textiga ebausaldusväärne. Lisaks puudub API-st igasugune võimalus Location andmeid lugeda või luua — ListingDto tagastab ainult `LocationId` GUID-i, mis on frontendile kasutu.

## What Changes

- **BREAKING**: Location entity kaotab väljad `City`, `Country`, `PostalCode`. Asemele tuleb `MunicipalityId` FK
- Uued lookup entity'd `County` (15 rida) ja `Municipality` (79 rida), seedistatakse EHAK andmetest
- Location luuakse inline Listing loomisel (CreateListingDto saab nested location objekti `LocationId` asemel)
- ListingDto tagastab nested location info (maakond, vald, aadress) `LocationId` GUID-i asemel
- Koordinaatide valideerimine (latitude -90..90, longitude -180..180)
- Eraldi Location CRUD endpoint'e **ei tule** — Location elab Listing'u elutsüklis

## Capabilities

### New Capabilities
- `location-lookup-tables`: County ja Municipality lookup entity'd, EHAK seeding, ja read-only API endpoint'id nimekirjade kättesaamiseks
- `location-inline-management`: Location loomine/uuendamine/kustutamine Listing elutsükli osana, valideerimine, nested DTO-d

### Modified Capabilities
- `service-listings-api`: CreateListingDto ja ListingDto muutuvad — `LocationId` asendub nested location objektiga, response sisaldab täielikku asukohainfot

## Impact

- **Domain**: Uued entity'd `County`, `Municipality`. Location entity muutub (väljad eemaldatakse, FK lisandub)
- **DAL**: Uus migratsioon (tabelite loomine, Location tabeli muutmine, seed data). DbContext konfiguratsiooni muudatused
- **BLL**: Listing DTO-de muudatused, ListingService peab looma/uuendama Location'it inline
- **API**: Uus endpoint Counties/Municipalities nimekirjade jaoks. Listing endpoint'ide request/response muutub
- **Breaking**: Olemasolevad Location read ilma migratsioonita ei tööta. Vanad LocationId-põhised päringud lakkavad töötamast
