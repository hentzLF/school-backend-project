# AgriMarket MVC - Manuaalse testimise juhend

## Eeltingimused

- Andmebaas on migreeritud ja seed data laetud
- Web projekt (`AgriMarket.Web`) jookseb
- Vaikimisi kasutajad:
  - **Admin:** `admin@agrimarket.ee` / `Admin123!`
  - **Provider:** `provider@agrimarket.ee` / `Provider123!`
  - **Farmer:** `farmer@agrimarket.ee` / `Farmer123!`

---

## 1. Avalikud lehed (ilma sisselogimiseta)

### 1.1 Avalehe laadimine
- [ ] Mine `/` — avalehe peaks kuvama
- [ ] Kontrolli, et navigatsiooniriba on nähtav
- [ ] Kontrolli, et sisselogimata kasutajale ei kuvata kaitstud linke

### 1.2 Privaatsuspoliitika
- [ ] Mine `/Home/Privacy` — leht peaks laadima

### 1.3 Keelevahetaja
- [ ] Vaheta keel eesti keelele — UI tekstid peaksid muutuma eestikeelseks
- [ ] Vaheta keel inglise keelele — UI tekstid peaksid muutuma ingliskeelseks
- [ ] Kontrolli, et keelevalik säilib lehekülgede vahel navigeerimisel

---

## 2. Kliendi autentimine (`/Client/Account`)

### 2.1 Registreerimine
- [ ] Mine `/Client/Account/Register`
- [ ] Täida vorm: email, parool, eesnimi, perekonnanimi
- [ ] Kinnita, et edukas registreerimine suunab sisselogimislehele
- [ ] Proovi registreerida juba olemasoleva e-mailiga — veateade peaks ilmuma
- [ ] Proovi registreerida tühja e-mailiga — valideerimise veateade
- [ ] Proovi registreerida liiga lühikese parooliga — valideerimise veateade
- [ ] Proovi registreerida tühja eesnime/perekonnanimega — valideerimise veateade

### 2.2 Sisselogimine
- [ ] Mine `/Client/Account/Login`
- [ ] Logi sisse kehtivate andmetega (`provider@agrimarket.ee` / `Provider123!`)
- [ ] Kontrolli, et suunatakse `/Client/Listings`
- [ ] Proovi sisselogimist vale parooliga — veateade peaks ilmuma
- [ ] Proovi sisselogimist olematu e-mailiga — veateade peaks ilmuma
- [ ] Proovi sisselogimist tühjade väljadega — valideerimise veateade

### 2.3 Väljalogimine
- [ ] Vajuta väljalogimisnuppu
- [ ] Kontrolli, et suunatakse sisselogimislehele
- [ ] Proovi pärast väljalogimist minna kaitstud lehele (nt `/Client/Bookings`) — peaks suunama sisselogimislehele

### 2.4 Ligipääsu keelamine
- [ ] Logi sisse tavalise kasutajana ja proovi minna admin alale (`/Admin/Dashboard`) — peaks kuvama Access Denied lehte

---

## 3. Kuulutuste sirvimine (`/Client/Listings`)

### 3.1 Kuulutuste nimekiri
- [ ] Mine `/Client/Listings` — kõik aktiivsed kuulutused peaksid olema nähtavad
- [ ] Kontrolli, et iga kuulutuse kaardil on: pealkiri, hind, kategooria, hinnang
- [ ] Kontrolli, et mitteaktiivsed kuulutused EI ole nähtavad

### 3.2 Kuulutuse detailvaade
- [ ] Klõpsa mõnel kuulutusel
- [ ] Kontrolli, et kuvatakse: pealkiri, kirjeldus, hind hektari kohta, kategooria
- [ ] Kontrolli, et näidatakse seadmete nimekirja (kui on)
- [ ] Kontrolli, et näidatakse arvustusi ja keskmist hinnangut (kui on)
- [ ] Kontrolli, et näidatakse saadavuse aegu (kui on)
- [ ] Kontrolli, et broneerimise vorm on nähtav (sisse logitud kasutajale)

---

## 4. Teenusepakkuja kuulutused (`/Client/MyListings`) — Provider roll

> Logi sisse: `provider@agrimarket.ee` / `Provider123!`

### 4.1 Minu kuulutuste nimekiri
- [ ] Mine `/Client/MyListings` — ainult selle pakkuja kuulutused peaks olema nähtavad
- [ ] Kontrolli, et iga kuulutusel on aktiivsuse staatus

### 4.2 Uue kuulutuse loomine
- [ ] Mine `/Client/MyListings/Create`
- [ ] Täida kõik väljad: pealkiri, kirjeldus, hind hektari kohta, kategooria, maakond, vald
- [ ] Kinnita, et kuulutus ilmub nimekirja
- [ ] Proovi luua kuulutus tühja pealkirjaga — valideerimise veateade
- [ ] Proovi luua kuulutus negatiivse hinnaga — valideerimise veateade
- [ ] Proovi luua kuulutus ilma kategooriata — valideerimise veateade

### 4.3 Kuulutuse muutmine
- [ ] Vali kuulutus ja mine muutmise lehele
- [ ] Muuda pealkirja — salvestamisel peaks uuendus nähtav olema
- [ ] Muuda hinda — salvestamisel peaks uuendus nähtav olema
- [ ] Muuda kirjeldust — salvestamisel peaks uuendus nähtav olema

### 4.4 Kuulutuse kustutamine
- [ ] Vali kuulutus ilma aktiivsete broneeringuteta
- [ ] Kinnita kustutamine — kuulutus peaks nimekirjast kaduma
- [ ] Proovi kustutada kuulutust, millel on aktiivsed broneeringud — veateade peaks ilmuma

### 4.5 Kuulutuse aktiivsuse lülitamine
- [ ] Lülita aktiivne kuulutus mitteaktiivseks — staatus peaks muutuma
- [ ] Lülita mitteaktiivne kuulutus aktiivseks — staatus peaks muutuma
- [ ] Kontrolli, et mitteaktiivne kuulutus EI ilmu avalikul kuulutuste lehel (`/Client/Listings`)

### 4.6 Saadavuse haldamine
- [ ] Mine kuulutuse saadavuste haldamise lehele
- [ ] Lisa uus saadavuse aeg: alguskuupäev ja lõppkuupäev
- [ ] Kontrolli, et uus saadavus ilmub nimekirja
- [ ] Kustuta saadavus — peaks nimekirjast kaduma
- [ ] Proovi lisada saadavust, kus lõppkuupäev on enne alguskuupäeva — veateade peaks ilmuma

### 4.7 Kuulutuse broneeringute vaatamine
- [ ] Mine kuulutuse broneeringute lehele
- [ ] Kontrolli, et kuvatakse selle kuulutuse broneeringud
- [ ] Muuda broneeringu staatust (kinnita/keeldu/lõpeta) — staatus peaks uuenema

---

## 5. Seadmed (`/Client/Equipment`) — Provider roll

> Logi sisse: `provider@agrimarket.ee` / `Provider123!`

### 5.1 Seadmete nimekiri
- [ ] Mine `/Client/Equipment` — pakkuja seadmed peaks olema nähtavad

### 5.2 Uue seadme lisamine
- [ ] Loo uus seade: nimi, mark, mudel, tootmisaasta, hobujõud, seisukord
- [ ] Kontrolli, et uus seade ilmub nimekirja
- [ ] Proovi luua seade tühja nimega — valideerimise veateade
- [ ] Proovi luua seade ebareaalse tootmisaastaga — valideerimise veateade

### 5.3 Seadme muutmine
- [ ] Vali seade ja muuda andmeid
- [ ] Kontrolli, et muudatused salvestuvad

### 5.4 Seadme kustutamine
- [ ] Vali seade, mis pole kuulutusega seotud
- [ ] Kinnita kustutamine — peaks nimekirjast kaduma
- [ ] Proovi kustutada seadet, mis on kuulutusega seotud — kas tekib veateade?

### 5.5 Seadme staatuse muutmine
- [ ] Muuda seadme staatust (nt aktiivne/hoolduses/mitteaktiivne)
- [ ] Kontrolli, et staatus uueneb

### 5.6 Seadme kuulutusele määramine
- [ ] Mine seadme assign lehele
- [ ] Vali kuulutus(ed) ja määra seade
- [ ] Kontrolli, et kuulutuse detailvaates ilmub seade
- [ ] Eemalda seade kuulutuselt — seade peaks kaduma kuulutuse detailvaatest

---

## 6. Broneeringud (`/Client/Bookings`) — Farmer roll

> Logi sisse: `farmer@agrimarket.ee` / `Farmer123!`

### 6.1 Broneeringu loomine
- [ ] Mine kuulutuse detailvaatesse
- [ ] Vali saadavuse aeg
- [ ] Sisesta pindala hektarites
- [ ] Kinnita broneering — peaks suunama broneeringu detailvaatesse
- [ ] Kontrolli, et broneeringu staatus on "Pending"
- [ ] Kontrolli, et broneeringu koguhind on arvutatud (hind/ha * pindala)
- [ ] Proovi broneerida juba broneeritud aega — veateade peaks ilmuma
- [ ] Proovi broneerida 0 hektarit — valideerimise veateade
- [ ] Proovi broneerida negatiivset pindala — valideerimise veateade

### 6.2 Broneeringute nimekiri
- [ ] Mine `/Client/Bookings` — kasutaja broneeringud peaks olema nähtavad
- [ ] Kontrolli, et iga broneeringul on: staatus, kuulutuse pealkiri, kuupäevad, hind

### 6.3 Broneeringu detailvaade
- [ ] Klõpsa broneeringul
- [ ] Kontrolli, et kuvatakse: teenusepakkuja info, kuulutuse pealkiri, pindala, hind, staatus

### 6.4 Broneeringu staatuse voog (terve elutsükkel)
> See nõuab kahe kasutaja vaheldumisi tegutsemist

1. **Farmer loob broneeringu** → Staatus: `Pending`
2. **Provider kinnitab** (MyListings → Bookings → kinnita) → Staatus: `Confirmed`
3. **Provider alustab tööd** → Staatus: `InProgress`
4. **Provider märgib lõpetatuks** → Staatus: `ProviderCompleted`
5. **Farmer kinnitab lõpetamise** → Staatus: `ClientConfirmed`
6. **Makse sooritatakse** → Staatus: `AwaitingPayment` → `Archived` (pärast makset)

### 6.5 Broneeringu tühistamine
- [ ] Loo uus broneering
- [ ] Tühista broneering — staatus peaks muutuma `Cancelled`-iks
- [ ] Kontrolli, et tühistatud broneeringu saadavuse aeg vabaneb

### 6.6 Broneeringu vaidlustamine
- [ ] Proovi vaidlustada broneeringut — staatus peaks muutuma `Disputed`-iks

---

## 7. Maksed (`/Client/Payments`) — Farmer roll

> Logi sisse: `farmer@agrimarket.ee` / `Farmer123!`

### 7.1 Makse sooritamine
- [ ] Mine lõpetatud broneeringu checkout lehele (`/Client/Payments/Checkout/{bookingId}`)
- [ ] Vali makseviis (pangaülekanne või kaart)
- [ ] Kinnita makse — peaks suunama kviitungi lehele
- [ ] Kontrolli kviitungil: summa, platvormi tasu, makseviis
- [ ] Proovi maksta broneeringu eest, mille staatus pole maksmiseks valmis — veateade

### 7.2 Maksete ajalugu
- [ ] Mine `/Client/Payments/History`
- [ ] Kontrolli, et kõik sooritatud maksed on nimekirjas
- [ ] Kontrolli, et iga maksel on: summa, kuupäev, staatus, seotud broneering

---

## 8. Arvustused (`/Client/Reviews`) — Farmer roll

> Logi sisse: `farmer@agrimarket.ee` / `Farmer123!`

### 8.1 Arvustuse loomine
- [ ] Mine lõpetatud broneeringu arvustuse loomise lehele
- [ ] Sisesta hinnang (1-5) ja kommentaar
- [ ] Kinnita — arvustus peaks ilmuma kuulutuse detailvaates
- [ ] Proovi luua arvustust broneeringule, mis pole lõpetatud — veateade
- [ ] Proovi luua arvustust ilma hinnanguta — valideerimise veateade
- [ ] Proovi luua arvustust hinnanguga 0 — valideerimise veateade
- [ ] Proovi luua arvustust hinnanguga 6 — valideerimise veateade

### 8.2 Arvustuse muutmine
- [ ] Vali olemasolev arvustus ja mine muutmise lehele
- [ ] Muuda hinnangut ja kommentaari
- [ ] Kontrolli, et muudatused salvestuvad ja on nähtavad kuulutuse detailvaates

### 8.3 Arvustuse kustutamine
- [ ] Vali arvustus ja kinnita kustutamine
- [ ] Kontrolli, et arvustus kadub kuulutuse detailvaatest
- [ ] Kontrolli, et keskmine hinnang uueneb

---

## 9. Sõnumid (`/Client/Messaging`) — Mõlemad rollid

> Testi kahe kasutaja vahel (provider ja farmer)

### 9.1 Vestluse alustamine
- [ ] Ava teise kasutaja profiil või broneeringu detailvaade
- [ ] Alusta vestlust — peaks looma uue vestluse või avama olemasoleva
- [ ] Kontrolli, et vestlus ilmub vestluste nimekirja

### 9.2 Sõnumi saatmine
- [ ] Ava vestlus
- [ ] Kirjuta sõnum ja saada
- [ ] Kontrolli, et sõnum ilmub vestluse ajalukku
- [ ] Proovi saata tühja sõnumit — kas midagi juhtub?

### 9.3 Sõnumite lugemine
- [ ] Logi sisse teise kasutajana
- [ ] Kontrolli, et lugemata sõnumite arv on nähtav
- [ ] Ava vestlus — sõnum peaks olema nähtav
- [ ] Märgi sõnum loetuks
- [ ] Kontrolli, et lugemata arv väheneb

### 9.4 Vestluste nimekiri
- [ ] Mine `/Client/Messaging`
- [ ] Kontrolli, et kõik vestlused on nimekirjas
- [ ] Kontrolli, et iga vestlusel näidatakse viimast sõnumit
- [ ] Kontrolli, et lugemata vestlused on visuaalselt eristatavad

---

## 10. Profiil (`/Client/Profile`)

### 10.1 Profiili vaatamine
- [ ] Mine `/Client/Profile` — peaks kuvama oma profiili
- [ ] Kontrolli, et kuvatakse: eesnimi, perekonnanimi, bio, avatar, rollid

### 10.2 Profiili muutmine
- [ ] Mine `/Client/Profile/Edit`
- [ ] Muuda eesnime — peaks salvestuma
- [ ] Muuda perekonnanime — peaks salvestuma
- [ ] Muuda bio — peaks salvestuma
- [ ] Muuda avatari URL-i — peaks salvestuma
- [ ] Proovi salvestada tühja eesnime — valideerimise veateade
- [ ] Proovi salvestada tühja perekonnanime — valideerimise veateade

---

## 11. Admin ala (`/Admin`)

### 11.1 Admin sisselogimine
- [ ] Mine `/Admin/Account/Login`
- [ ] Logi sisse: `admin@agrimarket.ee` / `Admin123!`
- [ ] Kontrolli, et suunatakse admin dashboardile
- [ ] Proovi sisselogimist tavalise kasutaja andmetega — veateade peaks ilmuma

### 11.2 Admin kasutaja registreerimine
- [ ] Mine `/Admin/Account/Register` (nõuab admin sisselogimist)
- [ ] Loo uus admin kasutaja
- [ ] Kontrolli, et uus kasutaja saab Admin rolli

### 11.3 Dashboard (`/Admin/Dashboard`)
- [ ] Kontrolli, et kuvatakse: kasutajate koguarv, uued kasutajad sel kuul/nädalal
- [ ] Kontrolli, et kuvatakse: kuulutuste arv, aktiivsed kuulutused
- [ ] Kontrolli, et kuvatakse: broneeringute arv, tulu, platvormi tasud
- [ ] Kontrolli, et kuvatakse: vaidluste arv, viimased broneeringud

### 11.4 Kasutajate haldamine (`/Admin/Users`)
- [ ] Vaata kasutajate nimekirja — kõik kasutajad peaks olema nähtavad
- [ ] Ava kasutaja detailvaade — peaks kuvama profiili, kuulutuste arvu, broneeringute arvu
- [ ] Muuda kasutaja andmeid — peaks salvestuma
- [ ] Lukusta kasutaja konto — kasutaja ei peaks saama sisse logida
- [ ] Ava kasutaja konto — kasutaja peaks saama uuesti sisse logida
- [ ] Kustuta kasutaja — kasutaja peaks kaduma nimekirjast

### 11.5 Kuulutuste haldamine (`/Admin/Listings`)
- [ ] Vaata kõiki kuulutusi — filter aktiivsete/mitteaktiivsete vahel
- [ ] Ava kuulutuse detailvaade
- [ ] Muuda kuulutuse andmeid
- [ ] Kustuta kuulutus

### 11.6 Broneeringute haldamine (`/Admin/Bookings`)
- [ ] Vaata kõiki broneeringuid — filter staatuse järgi
- [ ] Ava broneeringu detailvaade
- [ ] Muuda broneeringu staatust

### 11.7 Maksete haldamine (`/Admin/Payments`)
- [ ] Vaata kõiki makseid
- [ ] Ava makse detailvaade
- [ ] Vabasta raha (release) — makse staatus peaks muutuma `Released`
- [ ] Teosta tagasimakse (refund) — makse staatus peaks muutuma `Refunded`
- [ ] Lahenda vaidlus (resolve dispute) — vaidlus peaks saama lahendatud

### 11.8 Kategooriate haldamine (`/Admin/Categories`)
- [ ] Vaata kategooriate nimekirja — 7 vaikekategooriat peaks olema
- [ ] Loo uus kategooria — peaks ilmuma nimekirja
- [ ] Muuda kategooria nime — peaks salvestuma
- [ ] Kustuta kategooria — peaks kaduma nimekirjast
- [ ] Proovi luua kategooriat olemasoleva nimega — veateade (unikaalsuse piirang)
- [ ] Proovi luua kategooriat tühja nimega — valideerimise veateade

---

## 12. Autoriseerimise ja ligipääsukontrolli testid

### 12.1 Roll-põhine ligipääs
- [ ] **Farmer kasutajana:** proovi minna `/Client/MyListings/Create` — peaks olema keelatud (ainult Provider)
- [ ] **Farmer kasutajana:** proovi minna `/Client/Equipment` — peaks olema keelatud (ainult Provider)
- [ ] **Sisselogimata:** proovi minna `/Client/Bookings` — peaks suunama sisselogimislehele
- [ ] **Sisselogimata:** proovi minna `/Client/Profile` — peaks suunama sisselogimislehele
- [ ] **Tavaline kasutaja:** proovi minna `/Admin/Dashboard` — peaks olema keelatud
- [ ] **Tavaline kasutaja:** proovi minna `/Admin/Users` — peaks olema keelatud

### 12.2 Andmete ligipääs
- [ ] **Kasutaja A:** proovi vaadata kasutaja B broneeringu detaile, milles A ei osale — peaks olema keelatud (403)
- [ ] **Kasutaja A:** proovi muuta kasutaja B kuulutust — peaks olema keelatud
- [ ] **Kasutaja A:** proovi kustutada kasutaja B arvustust — peaks olema keelatud
- [ ] **Kasutaja A:** proovi vaadata kasutaja B vestlust — peaks olema keelatud

---

## 13. Äriloogika äärejuhtumid

### 13.1 Topeltbroneering
- [ ] Broneeri saadavuse aeg kasutaja A-ga
- [ ] Proovi broneerida sama aega kasutaja B-ga — veateade peaks ilmuma

### 13.2 Kuulutuse kustutamine aktiivse broneeringuga
- [ ] Loo kuulutus ja broneering
- [ ] Proovi kuulutust kustutada — peaks olema blokeeritud

### 13.3 Iseendale teenuse broneerimine
- [ ] Logi sisse providerina
- [ ] Proovi broneerida oma kuulutust — kas süsteem lubab seda?

### 13.4 Arvustuse piiramine
- [ ] Proovi luua teist arvustust samale broneeringule — kas süsteem lubab?
- [ ] Proovi luua arvustust ilma broneeringuta — veateade peaks ilmuma

### 13.5 Makse piiramine
- [ ] Proovi maksta broneeringu eest, mille eest on juba makstud — veateade peaks ilmuma
- [ ] Proovi maksta tühistatud broneeringu eest — veateade peaks ilmuma

### 13.6 Optimistic concurrency (saadavus)
- [ ] Ava sama saadavuse broneerimise vorm kahes brauseris
- [ ] Broneeri mõlemas korraga — ainult üks peaks õnnestuma

### 13.7 Staatuse üleminekud
- [ ] Proovi kinnitada juba tühistatud broneeringut — peaks olema keelatud
- [ ] Proovi märkida lõpetatuks broneeringut, mis pole `InProgress` — peaks olema keelatud
- [ ] Proovi tühistada juba lõpetatud broneeringut — peaks olema keelatud

---

## 14. Kasutajaliidese kontrollid

### 14.1 Navigeerimine
- [ ] Kontrolli, et kõik menüülingid töötavad
- [ ] Kontrolli, et "tagasi" nupud suunavad õigele lehele
- [ ] Kontrolli, et lehekülje pealkiri on igal lehel korrektne

### 14.2 Vormide käitumine
- [ ] Kontrolli, et vormide valideerimise veateatded on nähtavad punases tekstis
- [ ] Kontrolli, et pärast edukat saatmist näidatakse õnnestumise teadet
- [ ] Kontrolli, et pärast ebaõnnestunud saatmist jäävad sisestatud andmed alles

### 14.3 Tühjad olekud
- [ ] Vaata broneeringute lehte ilma broneeringuteta — kas kuvatakse sõnum "broneeringuid pole"?
- [ ] Vaata seadmete lehte ilma seadmeteta — kas kuvatakse sõnum "seadmeid pole"?
- [ ] Vaata vestluste lehte ilma vestlusteta — kas kuvatakse sõnum "vestlusi pole"?
- [ ] Vaata maksete ajalugu ilma makseteta — kas kuvatakse tühi olek?

### 14.4 Lehekülgede pagineerimine
- [ ] Kuulutuste nimekirjas: kontrolli pagineerimist (nt 10+ kuulutusega)
- [ ] Broneeringute nimekirjas: kontrolli pagineerimist
- [ ] Vestluse sõnumites: kontrolli pagineerimist (nt 20+ sõnumiga)

---

## 15. Vigade käsitlemine

### 15.1 Olematu ressurss
- [ ] Mine `/Client/Listings/99999` (olematu kuulutus) — peaks kuvama 404 lehe
- [ ] Mine `/Client/Bookings/99999` (olematu broneering) — peaks kuvama veateate
- [ ] Mine `/olematu-leht` — peaks kuvama 404 lehe

### 15.2 Vigased sisendid URL-is
- [ ] Mine `/Client/Listings/abc` (vale ID formaat) — peaks käsitlema graatsiliselt
- [ ] Mine `/Client/Payments/Checkout/abc` — peaks käsitlema graatsiliselt

---

## 16. Terviklikud stsenaariumid (end-to-end)

### 16.1 Teenuse pakkumise ja broneerimise täielik voog

1. [ ] Registreeri uus kasutaja (saab Farmer + Provider rollid)
2. [ ] Logi sisse uue kasutajana
3. [ ] Muuda profiili (lisa bio ja nimi)
4. [ ] Loo uus seade (nt traktor)
5. [ ] Loo uus teenuse kuulutus (vali kategooria, määra hind)
6. [ ] Lisa kuulutusele saadavuse ajad
7. [ ] Määra seade kuulutusele
8. [ ] Logi välja
9. [ ] Logi sisse teise kasutajana (farmer)
10. [ ] Sirvi kuulutusi ja leia äsja loodud kuulutus
11. [ ] Vaata kuulutuse detaile (kontrolli seadmeid, saadavust)
12. [ ] Broneeri teenus (vali aeg, sisesta pindala)
13. [ ] Logi välja
14. [ ] Logi sisse providerina
15. [ ] Vaata broneeringut MyListings → Bookings
16. [ ] Kinnita broneering (Pending → Confirmed)
17. [ ] Alusta tööd (Confirmed → InProgress)
18. [ ] Märgi töö lõpetatuks (InProgress → ProviderCompleted)
19. [ ] Logi välja
20. [ ] Logi sisse farmerina
21. [ ] Kinnita töö lõpetamine (ProviderCompleted → ClientConfirmed)
22. [ ] Soorita makse (vali makseviis, kontrolli kviitungit)
23. [ ] Jäta arvustus (hinnang + kommentaar)
24. [ ] Kontrolli, et arvustus on kuulutuse detailvaates nähtav
25. [ ] Kontrolli, et keskmine hinnang uuenes

### 16.2 Vestluse voog broneeringu kontekstis

1. [ ] Farmer loob broneeringu
2. [ ] Farmer alustab vestlust provideriga (seotud broneeringuga)
3. [ ] Farmer saadab sõnumi
4. [ ] Provider logib sisse ja näeb lugemata sõnumite arvu
5. [ ] Provider avab vestluse ja loeb sõnumit
6. [ ] Provider vastab sõnumiga
7. [ ] Farmer logib sisse ja näeb vastust

### 16.3 Admin vaidluse lahendamise voog

1. [ ] Farmer loob broneeringu ja sooritab makse
2. [ ] Farmer vaidlustab broneeringu (Disputed)
3. [ ] Admin logib sisse ja näeb vaidlust dashboardil
4. [ ] Admin vaatab makse detaile
5. [ ] Admin lahendab vaidluse (resolve dispute)
6. [ ] Kontrolli, et makse staatus uueneb

### 16.4 Kasutaja lukustamise voog

1. [ ] Admin lukustab kasutaja konto
2. [ ] Lukustatud kasutaja proovib sisse logida — peaks ebaõnnestuma
3. [ ] Admin avab konto
4. [ ] Kasutaja logib edukalt sisse

---

## 17. Andmebaasi seisundi kontrollid

### 17.1 Cascade delete kontrollid
- [ ] Kustuta kasutaja, kellel on seadmeid — seadmed peaksid samuti kustuma
- [ ] Kustuta broneering — seotud makse ja arvustus peaksid kustuma
- [ ] Kustuta vestlus — seotud sõnumid peaksid kustuma

### 17.2 Restrict delete kontrollid
- [ ] Proovi kustutada kuulutust, millel on broneeringud — peaks olema blokeeritud
- [ ] Proovi kustutada saadavust, millel on broneering — peaks olema blokeeritud

---

## Märkused

- Iga test peaks olema sõltumatu: alusta puhtast seisundist
- Testi nii Chrome kui Firefox brauserites
- Kontrolli brauseri konsooli vigade osas (F12 → Console)
- Pööra tähelepanu CSRF tokenile vormides (anti-forgery)
- Kontrolli, et kõik ümbersuunamised toimivad HTTPS-iga (kui seadistatud)
