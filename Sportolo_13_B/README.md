# Sportoló – Eredmény nyilvántartó API

ASP.NET Core Web API (kontroller alapú), MySqlConnector-rel, nyers SQL-lel
(nincs Entity Framework). A `sportolo` tábla adott; ez a projekt az `eredmeny`
táblát és a hozzá tartozó végpontokat valósítja meg.

## 1. Előkészítés

1. Hozd létre / futtasd le a `sportolo13b` adatbázist a kiadott scripttel
   (sportolo tábla + mintaadatok), majd futtasd le rá az
   `sql/01_eredmeny_table_and_seed.sql` fájlt. (Vagy egyből a
   `sql/02_sportolo13b_full_export.sql`-t egy üres szerveren – ez mindkettőt
   létrehozza.)
2. Nyisd meg az `appsettings.json`-t, és írd át a `DefaultConnection`
   connection stringben a jelszót/portot a saját MySQL szerverednek megfelelően.
3. A projekt gyökerében:
   ```bash
   dotnet restore
   dotnet run
   ```
4. Fejlesztői módban a Scalar felület itt érhető el:
   `https://localhost:<port>/scalar/v1`

## 2. Végpontok

### CRUD (`/eredmeny`)

| Metódus | Route                | Leírás                                                        |
|---------|-----------------------|-----------------------------------------------------------------|
| GET     | `/eredmeny`            | összes eredmény listázása                                       |
| GET     | `/eredmeny/{id}`       | egy eredmény lekérdezése id alapján                              |
| POST    | `/eredmeny`            | új eredmény (ResultTime/UpdateTime automatikusan "most")         |
| PUT     | `/eredmeny/{id}`       | módosítás (UpdateTime automatikusan frissül)                     |
| DELETE  | `/eredmeny/{id}`       | törlés                                                           |

POST / PUT body példa:
```json
{
  "competition": "Téli Kupa",
  "description": "200 m gyorsúszás, 3. hely.",
  "sportoloId": 1
}
```

### Speciális lekérdezések

| Metódus | Route                                          | Leírás                                            |
|---------|--------------------------------------------------|----------------------------------------------------|
| GET     | `/eredmeny/sportolo/{sportoloId}/nev-email`       | egy sportoló name + email párosa                    |
| GET     | `/eredmeny/sportolo/{sportoloId}/eredmenyek`      | sportoló neve + eredményei (competition, description) |
| GET     | `/eredmeny/darabszam`                             | összes rögzített eredmény száma                     |
| GET     | `/eredmeny/sportolo/{sportoloId}/darabszam`       | egy adott sportoló eredményeinek száma               |

## 3. Tesztelés (curl példák Postman/Scalar helyett)

```bash
curl https://localhost:<port>/eredmeny
curl https://localhost:<port>/eredmeny/1
curl -X POST https://localhost:<port>/eredmeny \
  -H "Content-Type: application/json" \
  -d '{"competition":"Téli Kupa","description":"teszt","sportoloId":1}'
curl -X PUT https://localhost:<port>/eredmeny/1 \
  -H "Content-Type: application/json" \
  -d '{"competition":"Téli Kupa - javítva","description":"teszt2","sportoloId":1}'
curl -X DELETE https://localhost:<port>/eredmeny/1

curl https://localhost:<port>/eredmeny/sportolo/1/nev-email
curl https://localhost:<port>/eredmeny/sportolo/1/eredmenyek
curl https://localhost:<port>/eredmeny/darabszam
curl https://localhost:<port>/eredmeny/sportolo/1/darabszam
```

Ellenőrizd le mindegyiket Scalarban vagy Postmanben, és nézd meg, hogy a
válasz státuszkód (200/201/204/404) és a JSON tartalom is a várt.

## 4. Javasolt commit-sorrend

A feladat kéri, hogy minden elkészült lépés után külön commit legyen. Javasolt
bontás (a fájlok már ennek megfelelően különülnek el):

```bash
git init                       # ha még nincs
git add Models/Eredmeny.cs
git commit -m "Eredmeny model osztaly"

git add Controllers/EredmenyController.cs Data/DbConnectionFactory.cs
git commit -m "EredmenyController vazlat + DB kapcsolat"

git add Controllers/EredmenyController.cs
git commit -m "CRUD muveletek (GET, GET by id, POST, PUT, DELETE)"

git add Controllers/EredmenyController.cs Models/Dtos.cs
git commit -m "Specialis lekerdezo vegpontok (nev-email, eredmenyek, darabszam)"

git add sql/
git commit -m "eredmeny tabla + mintaadatok, teljes DB export"

git remote add origin <A_SAJAT_GITHUB_REPOD_URL-JE>
git push -u origin main
```

(A fenti add-ok csak példák – a valóságban akkor commitolj, amikor az adott
rész ténylegesen elkészült és működik nálad.)
