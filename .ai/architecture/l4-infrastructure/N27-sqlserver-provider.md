---
title: "N27 SQL Server Provider"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N27 - kurumsal SQL Server saglayicisi (Microsoft.Data.SqlClient, sunucu-ustu durum, mirasci modu)
---

# N27 SQL Server Provider

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N25-ado-dapper.md](N25-ado-dapper.md)
- [N26-sqlite-provider.md](N26-sqlite-provider.md)
- [N28-migrations.md](N28-migrations.md)
- [../../index.md](../../index.md)
- [../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N27, kurumsal ortamlar icin `Microsoft.Data.SqlClient` ile N25'in soyutlamasi uzerinden SQL Server saglar; kimlik dogrulamayi entegrasyon moduna birakir ve SQLite (N26) ile ayni migration paketlerini paylasir.

## §0 Ozet (Answer First)

- N27 **mirasci** bir saglayicidir: Vana (V103) `[Architecture: SQLite]` dogrulamasindan sonra hedeflenmis, su an **default degil**.
- N25 `ISqlConnection`/`ISqlCommand` sozlesmesine uyar; connection string'i disaridan (l6 config) alir, **sirf sifre okumaz**.
- **Sahip oldugu nuans:** sunucu-ustu durum (havuz, zaman asimi, telemetri) bu node'dadir; bagimlilik matrisinde `l0 -> l7` ve `l1 -> l0` kesisir - N27 gozlem yazimini l0 kanalina verir.
- Ilerleme kosulu: `dotnet build` 0 hata + SQLite ayni migration paketinden gecme kaniti (§9).

## §1 Sorumluluk

| Tablo A - N27 yapar | Tablo B - N27 yapmaz |
|---|---|
| `Microsoft.Data.SqlClient` baglanti uretimi ve havuzlama | SQL migration yazmak (N28 sahibidir) |
| N25 `ISqlConnection` uyarlamasi (kurumsal mod) | Yerel tek dosya depolama (N26'ya ait) |
| Zaman asimi, telemetri ve guvenli kapatma | Ilk acilisa default SQLite secimi (l6 karari) |
| `[Architecture: SQLite]` disi paketleri tanimak | N24 SPT cagrisi (programlar ic gecittir) |
| Entegrasyon modu: kimlikdogrulamayi disarida birakmak | l2/l7 ile dogrudan iletisim |

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Connection string | l6 config (harici sifre) | Baglanti havuzuna verilir; kod icine **gömülmez** |
| `ISqlConnection` fabrikasi | N25 | Kurumsal uyarlama bu fabrikaya takilir |
| Migration paketleri | N28 | Ayni paket; farkli `[Architecture]` etiketli surum |
| Guvenlik modu | Config | Entegrasyon / SQL kimlik secimi |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| `ISqlConnection` uretimi | N25 kodoglu sorgular | Ayni sozlesme, farkli saglayici |
| Sunucu-ustu durum olaylari | l0 telemetri kanali | Bayt seviyesi akis |
| Migration kaydi | `__MigrationsHistory` | N28 ile ayni sema ailesi |
| Hata sonucu (Result) | ust katman | Fail-closed; exception yukseltilmez |

## §4 Yasak Yonler

| Yon | ADR-028 | N27 notu |
|---|---|---|
| l4 -> l2 | X | Oidc/Policy gorulmez |
| l4 -> l5 | YOK | N35 istemcisi baglanamaz |
| l5 -> l4 | X | Ters yon **DOĞRULAMA GEREKLİ** |
| l3 -> l4 | YOK | `IMigrationHost` kirilamaz |
| l4 -> l7 | YOK | `#if` sahibi yalnizca `ITfmBridge` |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N27 | SQL Server saglayici | Kurumsal / mirasci mod |
| N25 | ADO.NET + Dapper | Ortak soyutlama |
| N28 | Migration | Ayni paket ailesi, farkli etiket |
| N26 | SQLite | Ilk acilis default'u (rakip degil, alternatif) |

## §6 Projeler

| Proje | N27 katkisi |
|---|---|
| VersaCoder.Ado | Kurumsal saglayici uyarlamasi |
| VersaCoder.Config | Connection string ve guvenlik modu |
| VersaCoder.Core | `Result` ve ortak tipler |
| VersaCoder.Web | Sunucu-ustu durum raporlamasinin olasi tuketicisi |

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l4 -> l1 | `Microsoft.Data.SqlClient` + Dapper | Kod seviyesi |
| l4 -> l0 | Telemetri ve gozlem yazimi | Bayt seviyesi |
| l4 -> l2 | YOK | Yasak |
| l4 -> l3 | YOK | `IMigrationHost` gecilmez |
| l4 -> l7 | YOK | Iletisim geri donmez |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Baglanti uretimi + basit sorgu | `dotnet test` | N27 uyarlamasi |
| N25 ile ortak sozlesme | `dotnet test` | Ayni test takimi N26'yla paylasilir |
| Migration etiket uyumu | `dotnet test` | `[Architecture: SQLite]` disi paket |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N27 sinirlari |
| Migration CLI | `powershell -NoProfile -ExecutionPolicy Bypass -File ...\run-layer-migration.ps1 -Node N27` | Kanit (§9) |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P27-1 | `dotnet build src/VersaCoder.Ado/VersaCoder.Ado.csproj` | 0 hata |
| P27-2 | `dotnet test` (baglanti + sorgu testleri) | GECTI - **DOĞRULAMA GEREKLİ** |
| P27-3 | `Get-Content ...\*.cs \| Select-String "sifre=|=password"` | 0 eslesme (sifre kodda degil) |
| P27-4 | N25 test paketinin N27 uzerinden calismasi | GECTI - **DOĞRULAMA GEREKLİ** |
| P27-5 | `powershell -NoProfile -ExecutionPolicy Bypass -File ...\run-layer-migration.ps1 -Node N27` | Migration dogrulandi |
