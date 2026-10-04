---
title: "N28 Migrations"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: N28 - sema migration orkestrasyonu (etiketli paket sirasi, idempotent uygulama, geri alma politikasi)
---

# N28 Migrations

**Zorunlu Baglantilar:**
- [index.md](index.md)
- [N25-ado-dapper.md](N25-ado-dapper.md)
- [N26-sqlite-provider.md](N26-sqlite-provider.md)
- [N27-sqlserver-provider.md](N27-sqlserver-provider.md)
- [../../index.md](../../index.md)
- [../../spec/index.md](../../spec/index.md)
- [../../checklists/layer-boundary-check.md](../../checklists/layer-boundary-check.md)

> **Tek cumlelik sorumluluk:** N28, `[Architecture: SQLite]` etiketli migration paketlerini sirayla ve idempotent bicimde uygular; sema degisikliklerinin tek giris noktasi olur, `IMigrationHost` (l3) sinirini gecmez.

## §0 Ozet (Answer First)

- N28 **migration yazar**, sadece calistirilmaz: her paket etiketlidir ve dogru saglayiciya (N26 SQLite / N27 SQL Server) dusmek zorundadir.
- Tek giris noktadir; dogrudan `CREATE TABLE` kodoglu yazimi diger katmanlarda **yasak**.
- **Sahip oldugu nuans:** `l3 -> l4` engeli (ADR-028 `YOK`) ile `IMigrationHost` cagrisi arasindaki **DOĞRULAMA GEREKLİ** - l3 cagrisi mevcutsa `IMigrationHost` uzerinden gecmelidir.
- Ilerleme kosulu: tekrar calistirdiginda ayni sonucu veren idempotent test (§9 P28-2).

## §1 Sorumluluk

| Tablo A - N28 yapar | Tablo B - N28 yapmaz |
|---|---|
| Paket etiketini ve sirayi dogrulamak | Sema tasarimi politikasini belirlemek (l2 deil) |
| Idempotent uygulama ve `__MigrationsHistory` yazmak | Baglanti string yonetimi (N25/N26/N27) |
| Basari/basarisizlik kaydini tutmak (Result) | N24 SPT cagrisi (programlar ic gecittir) |
| Geri alma (down) paketi saglamak | l3 `IMigrationHost` sinirini gecmek |
| Ilk acilis ve guncelleme akisini yonetmek | l5 protokolu uygulamak |

## §2 GIRENLER

| Girdi | Kaynak | Nasil kullanilir |
|---|---|---|
| Migration paketleri | N28 ic dizini (numarali) | Etikete gore saglayici secilir |
| `ISqlConnection` | N25 / N26 / N27 | Uygulama icin ortak gecit |
| Hedef saglayici modu | l6 config | SQLite mi, SQL Server mi |
| Geri alma istegi | Operasyon komutu | `down` paketi sirayla ters uygulanir |

## §3 CIKANLAR

| Cikti | Alici | Not |
|---|---|---|
| Guncel sema | N26 / N27 ve tuketiciler | Dosya veya sunucu uzerinde |
| `__MigrationsHistory` | N28 kendi kaydi | Tekrar calistirma guvenligi |
| Uygulama sonucu (Result) | l6 host | Fail-closed: hata halinde yarim yazilmaz |
| Degisiklik gunlugu | Rapor / telemetri | l0 kanali, bayt seviyesi |

## §4 Yasak Yonler

| Yon | ADR-028 | N28 notu |
|---|---|---|
| l3 -> l4 | YOK | `IMigrationHost` cagrisi **DOĞRULAMA GEREKLİ** - varsa arayuz uzerinden |
| l4 -> l2 | X | Politika veya kimlik kodu okunmaz |
| l4 -> l5 | YOK | N35 istemcisiyle iliski yok |
| l4 -> l7 | YOK | TFM `#if` sahibi `ITfmBridge` |
| l4 -> l1 | OK | Dapper/ADO ornegi (bagimlilik matrisi satiri 3) |

## §5 N Kodlari

| Node | Rol | Ilişki |
|---|---|---|
| N28 | Migration orkestratoru | Etiket + sira + idempotent |
| N25 | ADO + Dapper | Ortak SQL gecidi |
| N26 | SQLite hedefi | `[Architecture: SQLite]` |
| N27 | SQL Server hedefi | Alternatif etiket |

## §6 Projeler

| Proje | N28 katkisi |
|---|---|
| VersaCoder.Ado | Migration motorunun barindigi proje |
| VersaCoder.Core | `Result` ile hata sozlesmesi |
| VersaCoder.Config | Hedef saglayici modu |
| VersaCoder.Sdk | Paket uretimi (migration dosyalari) |

## §7 Bagimlilik Listesi

| Yon | Hedef | Tur |
|---|---|---|
| l4 -> l1 | Dapper + saglayici paketleri | Kod seviyesi |
| l4 -> l0 | Migration olaylarinin gozlemi | Bayt seviyesi |
| l4 -> l2 | YOK | Yasak |
| l3 -> l4 | YOK (veya `IMigrationHost` - DOĞRULAMA GEREKLİ) | Tek gecisli istisna |
| l4 -> l7 | YOK | Iletisim geri donmez |

## §8 Test Stratejisi

| Test | Arac | Kapsam |
|---|---|---|
| Ilk acilista tam uygulama | `dotnet test` | Bosa migration |
| Ikinci calistirma (idempotent) | `dotnet test` | Ayni sonuc, hata yok |
| Etiketsiz paket reddi | `dotnet test` | Fail-closed dogrulama |
| Layer siniri (13 soru) | [layer-boundary-check.md](../../checklists/layer-boundary-check.md) | N28 + l3 gecisi |
| Migration CLI | `powershell -NoProfile -ExecutionPolicy Bypass -File ...\run-layer-migration.ps1 -Node N28` | Kanit (§9) |

## §9 Kanit

| Id | Kanit | Beklenen |
|---|---|---|
| P28-1 | `dotnet test` (migration test paketi) | GECTI - **DOĞRULAMA GEREKLİ** |
| P28-2 | Migration'i iki kez calistir | Ikincisi 0 hata, ayni sema |
| P28-3 | Etiket uygunsuz paket denemesi | Reddedildi - **DOĞRULAMA GEREKLİ** |
| P28-4 | `Get-Content ...\*.cs \| Select-String "CREATE TABLE"` (N28 disi) | 0 eslesme (migration disi yerde DDL yok) |
| P28-5 | `powershell -NoProfile -ExecutionPolicy Bypass -File ...\run-layer-migration.ps1 -Node N28` | Paket dogrulandi |
