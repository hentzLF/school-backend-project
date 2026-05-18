## Context

AgriMarket Location entity kasutab free-text välju (City, Country, PostalCode), mis ei võimalda usaldusväärset piirkondlikku filtreerimist. API ei paljasta Location andmeid — ListingDto tagastab ainult `LocationId` GUID-i. Eesti haldusjaotus on stabiilne: 15 maakonda, 79 omavalitsust (viimane reform 2017). EHAK klassifikaator pakub ametlikke koode ja nimesid.

Praegune seis:
- `Location` entity: Id, Latitude, Longitude, Address, City, PostalCode, Country
- `ServiceListing.LocationId` — nullable FK
- Puudub: LocationDto, LocationService, LocationController, valideerimine
- Location'it ei saa API kaudu luua ega lugeda

## Goals / Non-Goals

**Goals:**
- County ja Municipality lookup-tabelid EHAK andmetega (seeded)
- Location entity refaktoreerimine: City/Country/PostalCode → MunicipalityId FK
- Location inline loomine/uuendamine Listing elutsükli osana
- Nested location info Listing DTO response'ides
- Read-only endpoint'id County ja Municipality nimekirjade jaoks
- Koordinaatide ja aadressi valideerimine

**Non-Goals:**
- Geograafiline raadiusotsing (Haversine, spatial indeksid)
- Settlement (asula) tase (~4692 rida) — lisatakse hiljem vajadusel
- Eraldi Location CRUD endpoint'id (Location elab Listing'u sees)
- Geocoding (aadress → koordinaadid automaatselt)
- Mitmekeelne nimede tugi (ainult eestikeelsed nimed)

## Decisions

### Decision 1: Lookup-tabelid vs free-text

**Valik:** Eraldi County ja Municipality entity'd lookup-tabelitena.

**Alternatiivid:**
- Free-text (praegune) — halb andmekvaliteet, filtreerimine ei tööta
- Enum — ei skaleeru, koodis hardcoded
- Üks lookup-tabel hierarhiaga — keerulisem querying

**Põhjus:** 15 + 79 rida on triviaalse suurusega andmestik. Lookup-tabelid tagavad andmekvaliteedi, võimaldavad kaskaaddropdown'e frontendis ja annavad efektiivse filtreerimise ilma fuzzy matchinguta.

### Decision 2: Location elutsükkel — inline vs eraldi CRUD

**Valik:** Location luuakse, uuendatakse ja kustutatakse Listing'u operatsioonide osana.

**Alternatiivid:**
- Eraldi LocationsController + CRUD — lisab keerukust, nõuab orbaanide puhastamist
- Location kui owned type (EF value object) — ei saa jagada Location'eid Listing'ute vahel

**Põhjus:** Kasutajad ei mõtle asukohtadest eraldi — nad loovad teenuse kindlas kohas. Inline lähenemine hoiab API lihtsa ja väldib orbaanide Location'ite teket.

### Decision 3: Location jagamine Listing'ute vahel

**Valik:** Iga Listing saab oma Location instance'i. Jagamist ei ole.

**Alternatiivid:**
- Jagatud Location'id (mitu Listing'ut → sama Location) — nõuab keerulist upsert loogikat ja referentsi loendamist

**Põhjus:** Inline loomine tähendab, et iga Listing'u asukoht on iseseisev. Kui teenusepakkuja muudab ühe Listing'u aadressi, ei mõjuta see teist. Lihtsam mudel, vähem edge-case'e.

### Decision 4: Seeding strateegia

**Valik:** EF Core `HasData()` ModelBuilder konfiguratsioonis. Staatilised Guid-id County ja Municipality jaoks.

**Alternatiivid:**
- SQL skript — platvormisspetsiifiline, raskesti versioonitav
- Runtime seeding (startup code) — aeglustab käivitust, idempotentsuse haldus

**Põhjus:** `HasData()` genereerib migratsiooni automaatselt, on versioonikontrollis, ja töötab igal andmebaasi reset'il. 15 + 79 rida on piisavalt väike andmestik.

### Decision 5: DTO struktuur

**Valik:** Nested `LocationDto` Listing response'ides, nested `CreateLocationDto` / `UpdateLocationDto` Listing request'ides.

```
CreateListingDto:
  location: { municipalityId, address?, latitude?, longitude? }

ListingDto:
  location: { id, municipalityName, countyName, address?, latitude?, longitude? }
```

**Põhjus:** Frontend saab kogu asukohainfo ühe päringuga. Eraldi Location ID lookup pole vajalik.

### Decision 6: DeleteBehavior muudatus

**Valik:** Location'i kustutamisel kasutame `Cascade` (Location kustutatakse koos Listing'uga), mitte praegust `SetNull`. Kuna Location elab Listing'u sees, ei ole mõtet seda eraldi alles hoida.

**Alternatiiv:** Jätta `SetNull` — aga siis tekivad orbaanid Location'id.

**Põhjus:** Inline mudelis on Location Listing'u alamkomponent. Listing kustutamisel peab Location kaasa minema. Vastupidist suunda (Location kustutamine) API ei luba.

## Risks / Trade-offs

**[Breaking migration]** → Olemasolevad Location read (City, Country, PostalCode väljad) kaovad. Migratsioon peab olemasolevad andmed teisendama või kustutama. → Kuna tegemist on arenduskeskkonnaga, on andmekadu aktsepteeritav.

**[Staatilised EHAK andmed]** → Kui haldusjaotus muutub, tuleb andmeid uuendada käsitsi. → Viimane reform oli 2017, risk on madal. Uuendamine = uus migratsioon.

**[Koordinaadid valikulised]** → Mitte iga teenusepakkuja ei pruugi koordinaate teada. → Frontendile see on aktsepteeritav — kaardikuva on nice-to-have, mitte kohustuslik.

**[Üks Location per Listing]** → Kui teenusepakkuja pakub sama teenust 3 asukohas, peab ta iga asukoha kohta eraldi Listing'u tegema. → See on äriliselt mõistlik — erinevad asukohad = erinevad teenusepakkumised.
