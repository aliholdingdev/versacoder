---
title: "N26 SQLite Provider"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N26 - SQLite cevrimici depolama saglayicisi (System.Data.SQLite, tek dosya DB, yazilim lisanslari ve yerel durum)
---

# N26 SQLite Provider

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N25-ado-dapper.md](N25-ado-dapper.md)
- [N27-sqlserver-provider.md](N27-sqlserver-provider.md)
- [N28-migrations.md](N28-migrations.md)
- [../../index.md](../../index.md)
- [../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N26, tek dosyali SQLite veritabanini System.Data.SQLite uzerinden N25'in soyutlamasina takilarak saglar; yazilim lisansi, makro ve yerel durum kayitlarini cevrimici olarak barindirir, sunucu kurulumu gerektirmez.

## §0 Ozet (Answer First)

- N26, `Microsoft.Data` **degil** `System.Data.SQLite` kullanir; tek dosya `.db` ile `C:\versacoder\src` icinde cevrimici depolama saglar.
- N25'in `ISqlConnection`/`ISqlCommand` sozlesmelerine bagli bir **saglayici uyarlamasidir**; kodoglu sorgulari N25 gecirir, migration'lari N28 uygular.
- **Sahip oldugu nuans:** yerel-ilk calisma; Vana (V103, `[Architecture: SQLite]`) tarafindan dogrulanmis tek dosya semasi.
- Ilerleme kosulu: `dotnet build` sifir hata + tek dosya DB olusturma testi gecmeli (yukaridaki §9'da komutlar).

## §1 Sorumluluk

| Tablo A - N26 yapar | Tablo B - N26 yapmaz |
|---|---|
| `System.Data.SQLite` uzerinden tek dosya `.db` acip kapama | Sunucu uzerinde `master`/`model` yonetimi (N27'ye ait) |
| N25 `ISqlConnection`/`ISqlCommand` uyarlamasi | SQL migration yazmak (migration sahibi N28'dir) |
| Pragma (WAL, foreign_keys) ve yerel acilis ayarlari | N35 istemci protokolu (l5 - l4 yonu **yasak**) |
| Yazilim lisansi / makro / yerel durum tablolarinin CRUD'u | Kimlik dogrulama ilkesi belirlemek (l2 politikasi) |
| Tek dosya yedekleme ve kopyalama destegi | Operasyonel telemetri aki (l0'a gecilir) |

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| `ISqlConnection` baglanti fabrikasi | N25 (kodoglu) | SQLite cevrimicisi bu fabrikaya uyarlanir |
| Migration paketleri | N28 (`[Architecture: SQLite]` etiketli) | Ilk acilista sirayla uygulanir |
| DB dosya yolu ve mod | Config (l6'dan enjeksiyon) | Tek dosya `.db` konumu ve salt-okunur mod |
| PRAGMA tercihleri | Config | WAL, `foreign_keys=ON` acilista uygulanir |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `ISqlConnection` uretimi | N25 sorgu katmani | Kodoglu sorgular aynen calisir |
| Dosya tabanli `.db` | Uygulama (makro, lisans, durum) | Sunucu gerektirmez; kopyalanabilir |
| Migration uygulama kaydi | `__MigrationsHistory` | N28 ile ayni sema |
| Acilis hatasi durumu | Result/Either (l4 ic hatasi) | Fail-closed: dosya kirliyse acilmaz |

## §4 Yasak Yonler

| Yon | ADR-028 | N26 notu |
|---|---|---|
| l4 -> l2 | X | N26, N06 (Oidc) ve N07 (Policy) gormez |
| l4 -> l5 | YOK | N35/N36 protokolleri tersine baglanamaz |
| l5 -> l4 | X | l5'in N26'ya erisimi **DOĞRULAMA GEREKLİ** (l5, `System.Data`'yi kendi paketiyle baglar) |
| l3 -> l4 | YOK | `IMigrationHost` siniri kirilamaz |
| l4 -> l7 | YOK | TFM `#if` sahibi yalnizca `ITfmBridge` |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N26 | SQLite cevrimici saglayici | Dosya tabanli yerel depo |
| N25 | ADO.NET + Dapper | Soyutlama ve kodoglu sorgu |
| N28 | Migration orkestrasyonu | `[Architecture: SQLite]` paketleri |
| N27 | SQL Server (mirasci) | Ayri fiziksel dosya; ayni N25 arayuzu |

## §6 Projeler

| Proje | N26 katkisi |
|---|---|
| VersaCoder.Ado | Uzerine takilan cevrimici saglayici |
| VersaCoder.Config | DB modu ve dosya yolu yapilandirmasi |
| VersaCoder.Core | Kullanilan `Result` tipleri (bagimlilik gider) |
| VersaCoder.Sdk | Makro ve yerel durum tablolarinin tuketicisi |

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l4 -> l1 | Dapper + `System.Data.SQLite` (NuGet) | Kod seviyesi (NOT: `Microsoft.Data` degil) |
| l4 -> l0 | Dosya yazma uzerinden olaylari | Bayt seviyesi |
| l4 -> l2 | YOK | Yasak |
| l4 -> l3 | YOK | `IMigrationHost` gecilmez |
| l4 -> l7 | YOK | TFM `#if` yalnizca `ITfmBridge` |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Tek dosya DB olustur + sorgu | `dotnet test` | N26 cevrimicisi ve N25 gecidi |
| Migration ilk acilis | `dotnet test` | N28 paketlerinin SQLite sirasi |
| Dosya kirli / salt-okunur mod | `dotnet test` | Fail-closed acilis davranisi |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N26 sinirlari |
| Migration CLI cagrisi | `powershell -NoProfile -ExecutionPolicy Bypass -File ...\run-layer-migration.ps1 -Node N26` | Kanit (§9) |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P26-1 | `dotnet build src/VersaCoder.Ado/VersaCoder.Ado.csproj` | 0 hata |
| P26-2 | `dotnet test` (tek dosya DB uretim testi) | GECTI - **DOĞRULAMA GEREKLİ** |
| P26-3 | DB acilis testi (kirli dosya) | Fail-closed - **DOĞRULAMA GEREKLİ** |
| P26-4 | `Get-Content src\VersaCoder.Ado\*.cs \| Select-String "Microsoft.Data"` | 0 eslesme (yalnizca `System.Data.SQLite`) |
| P26-5 | `powershell -NoProfile -ExecutionPolicy Bypass -File ...\run-layer-migration.ps1 -Node N26` | Migration paketi dogrulandi |
