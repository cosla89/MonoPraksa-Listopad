# Zoo API

ASP.NET Core API za upravljanje životinjama, s PostgreSQL bazom,
registracijom korisnika i JWT autentifikacijom.

## Preduvjeti

- .NET 8 SDK
- PostgreSQL
- Visual Studio 2022 ili drugi editor koji podržava .NET
- DBeaver ili drugi PostgreSQL klijent
- Postman za testiranje API-ja

## 1. Priprema baze

U PostgreSQL-u napravi bazu naziva `zoo`.

Poveži se na bazu `zoo` i izvrši cijelu skriptu `database/init.sql`.
Skripta stvara tablice `animals` i `users`.

Skripta priprema novu bazu. Ne mijenja strukturu već postojećih tablica.

## 2. Lokalna konfiguracija

Otvori `WebApplication1.sln`.

U Visual Studiju desnim klikom na API projekt `WebApplication1`
odaberi **Manage User Secrets**.

Postavi konfiguraciju prema ovom primjeru, zamjenjujući označene vrijednosti:

```json
{
  "ConnectionStrings": {
    "ZooDatabase": "Host=localhost;Port=PORT_POSTGRESQLA;Database=zoo;Username=postgres;Password=TVOJA_LOZINKA"
  },
  "Jwt": {
    "Issuer": "ZooApi",
    "Audience": "ZooApiClients",
    "Key": "NOVI_BASE64_KLJUC",
    "ExpirationMinutes": 30
  }
}
```

Port mora odgovarati tvojoj PostgreSQL instalaciji.
U izvornom razvojnom okruženju korišten je port 7197.

JWT ključ mora biti Base64 zapis najmanje 32 nasumična bajta.
Novi ključ možeš generirati u PowerShellu:

```powershell
$jwtBytes = New-Object byte[] 32
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($jwtBytes)
[Convert]::ToBase64String($jwtBytes)
$rng.Dispose()
```

Dobivenu vrijednost kopiraj u `Jwt:Key` u User Secrets.

Stvarnu lozinku baze i JWT ključ nemoj spremati u Git.

## 3. Pokretanje

Postavi `WebApplication1` kao Startup Project.
Pokreni projekt odabranim HTTPS profilom.

Koristi adresu koju aplikacija prikaže pri pokretanju.
Port API-ja i port PostgreSQL-a imaju različite namjene.

## 4. Prijava i testiranje

Registriraj korisnika preko `POST /api/auth/register`,
zatim se prijavi preko `POST /api/auth/login`.

Dobiveni token u Postmanu postavi pod:
Authorization → Bearer Token.

Novi korisnik dobiva ulogu `User`.
Za administratorske operacije potrebna je uloga `Admin`.
Nakon promjene uloge u bazi potrebna je nova prijava i novi token.

Očekivani odgovori za zaštićene rute:
- 401: nedostaje valjan token
- 403: korisnik nema potrebnu ulogu
- Uspješan odgovor: valjan token i odgovarajuća prava