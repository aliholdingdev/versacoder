---
title: "connection-string-secrets - VersaCoder"
type: knowledge
category: data
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "Baglanti metni sirrlarinin DPAPI ile sifreli saklanmasi, duz metin yasagi, rotasyon ve denetim listesi"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# connection-string-secrets - Baglanti Metni ve Sirlar

**Zorunlu Baglantilar:** [[knowledge/index|vault indeks]] · [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005]] · [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001]] · [[governance/keys|keys]] · [[.rules/coding-standards|coding-standards]] · [[knowledge/data/ado-net-connection|ado-net-connection]]
**Küme + Hub:** [[knowledge/data/index|data index]] · [[index|index]] · [[claude|CLAUDE]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]]
**Kardeşler (data · 6/25):** [[knowledge/data/audit-columns-convention|audit-columns-convention]] · [[knowledge/data/command-timeout-policy|command-timeout-policy]] · [[knowledge/data/concurrency-optimistic|concurrency-optimistic]] · [[knowledge/data/dapper-async-patterns|dapper-async-patterns]] · [[knowledge/data/dapper-multi-map|dapper-multi-map]] · [[knowledge/data/dapper-procedures-stored|dapper-procedures-stored]] · [[knowledge/data/dapper-query-mapping|dapper-query-mapping]] · [[knowledge/data/migration-rollback|migration-rollback]] · [[knowledge/data/migration-script-writing|migration-script-writing]] · [[knowledge/data/migration-version-table|migration-version-table]] · [[knowledge/data/null-handling-db-mapping|null-handling-db-mapping]] · [[knowledge/data/pagination-offset-keyset|pagination-offset-keyset]] · [[knowledge/data/parameterized-queries|parameterized-queries]] · [[knowledge/data/repository-pattern-thin|repository-pattern-thin]] · [[knowledge/data/sql-injection-defense|sql-injection-defense]] · [[knowledge/data/sqlite-provider-setup|sqlite-provider-setup]] · [[knowledge/data/sqlserver-provider-setup|sqlserver-provider-setup]] · [[knowledge/data/transaction-scope-local|transaction-scope-local]] · [[knowledge/data/unit-of-work-pattern|unit-of-work-pattern]]

> **Tek cumlelik sorumluluk:** *Baglanti metni - ozellikle parola - nerede, nasil, kim icin sifrelenerek saklanir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Baglanti metni **duz metin durmaz**: `ProtectedData` (DPAPI) ile `DataProtectionScope.CurrentUser` kapsaminda sifrelenir, base64 olarak ayar dosyasinda ya da kullanici profilinde saklanir, cozumleme yalniz calistirma aninda yapilir. Parola **koda, ayar dosyasina, `.sql` dosyasina ve repo'ya** yazilmaz; `keys.md` yalniz anahtar kelime haritasidir, sir deposu degildir. Sifreleme **metni** korur; erisim ayriciligi ayri bir katmandir (en az yetki).

```csharp
var clear = "Server=.;Database=VersaCoder;Integrated Security=true";
var entropy = Encoding.UTF8.GetBytes(Vault.Entropy);       // uygulamaya ozel

var protectedBytes = ProtectedData.Protect(
    Encoding.UTF8.GetBytes(clear), entropy, DataProtectionScope.CurrentUser);

File.WriteAllText(secretPath, Convert.ToBase64String(protectedBytes));  // duz metin yok
```

---

## §1 Saklama kurali tablosu

| Yer | Durum | Aciklama |
|---|---|---|
| Kaynak kod (`.cs`) | **yasak** | string literal = repo sizintisi |
| `.sql` / migration | **yasak** | sema dosyalari paylasilir |
| `keys.md` | yalniz anahtar kelime haritasi | sir icermez |
| Ayar dosyasi (sifreli blob) | **izinli** | base64 + DPAPI |
| Kullanici ortam degiskeni | **izinli** | CI / ortam stratejisi |
| Git | `.gitignore` + tarama | sir = CRITICAL seviye |

---

## §2 Denetim listesi

| # | Kontrol | Kanit |
|---|---|---|
| 1 | Duz metin parola aramasi (`Password=`, `Pwd=`) repoda = **0** | kod taramasi |
| 2 | DPAPI kapsami bilincli: `CurrentUser` (tek kullanici) / `Machine` (servis) | kod incelemesi |
| 3 | Entropy sabiti sir sayilmaz ama **degistirilemez** | surum kontrolu |
| 4 | Rotasyon: parola degistiginde yalniz sifreli blob yenilenir | calistirma kilavuzu |
| 5 | `Encrypt=true` + guven sertifikasi bilincli | baglanti testi |

---

## §3 Guvenlik katmanlari

| Katman | Ne korur | Kapsam |
|---|---|---|
| DPAPI sifreleme | metni okunamaz yapar | CurrentUser ya da Machine |
| Entropy | standart cozumlemeyi zorlastirir | uygulamaya ozel sabit |
| `.gitignore` | repoya girmesini engeller | depolama |
| Erisim ayriciligi | kimin okuyabilecegini belirler | isletim sistemi |
| `Encrypt=true` | hat uzerini korur | ag |

> Sifreleme tek basina yetmez; hicbiri digerinin yerine degildir. Sir sizzintisi CRITICAL olarak siniflandirilir.

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `ProtectedData` | `System.Security.dll` yerlesik | NuGet `System.Security.Cryptography.ProtectedData` (**yalniz Windows**) |
| Kapsam | `CurrentUser` / `Machine` | ayni (Windows DPAPI) |
| Parola kaynagi | yerel ayar / profil | ayni - ortam degiskeni tercih edilir |
| Erisim | `#if` **degil**, ayri `ISecretStore` cozumu | ayni arayuz |

> **NOT:** Kasa (secret store) soyutlamasi `IDbConnectionFactory` gibi **tek arayuz** ile iki bacağa ortaktir; `#if` yalniz platform API farki icin kalir (ADR-001).

---

## §5 Kardesler

[[knowledge/data/ado-net-connection|ado-net-connection]] · [[knowledge/data/connection-pooling-notes|connection-pooling-notes]] · [[knowledge/data/dual-tfm-provider-choice|dual-tfm-provider-choice]] · [[knowledge/data/ado-net-command|ado-net-command]] · [[knowledge/data/dapper-parameters-dynamic|dapper-parameters-dynamic]]

---

## §6 Kisa karar

Parola hicbir metin dosyasina duz yazilmaz; DPAPI + base64 blob, cozumleme yalniz calistirma aninda. `keys.md` sir deposu degildir. Rotasyonda yalniz blob yenilenir, kod degismez.

---

## §7 Soru - cevap

| Soru | Cevap |
|---|---|
| Baglanti metni kodda durabilir mi? | Hayir - string literal repo sizintisidir |
| `keys.md` sir saklar mi? | Hayir, yalniz anahtar kelime haritasidir |
| Kapsam neden onemli? | `CurrentUser` tek kullanici, `Machine` servis icindir |
| Entropy sir midir? | Hayir, ama degistirilemez - degisirse blob okunamaz |
| `net48`'de fark nedir? | `ProtectedData` yerlesik; `net10`'da ayri NuGet paketi |

---

## §8 Doğrulama

| Kontrol | Kanit |
|---|---|
| `Password=` / `Pwd=` aramasi repoda = **0** | kod taramasi |
| Sifreli blob dosyasi okunabilir durumda | calistirma testi |
| DPAPI kapsami kodda acikca yaziliyor | kod incelemesi |
| `.gitignore` sirli dosyayi kapsiyor | depo incelemesi |
| `net10` bacakinda paket referansi mevcut | derleme ciktisi |

> `Machine` kapsaminin servis hesabi icin dogru secilip secilmedigi ortama baglidir: **DOĞRULAMA GEREKLI**.

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `connection-string-secrets.md` | DPAPI ornegi, saklama kurali ve denetim tablolari, iki bacak farki |
| `ado-net-connection.md` | baglanti metninin factory icinde kullanilmasi |
| `connection-pooling-notes.md` | `Connect Timeout` ve `Encrypt` ayar anahtarlari |
| `dual-tfm-provider-choice.md` | `#if` disiplini ve ortak arayuz yaklasimi |
| `ado-net-command.md` | parametreli sorgu ile parolanin metne girmemesi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
