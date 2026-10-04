---
title: "Sistem Genel Bakisi - Moduller, Akislar, Teknoloji Kararlari"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "VersaCoder'in 5 modulunu, uc cekirdek calisma akisini, 12 teknoloji kararini ve olculebilir NFR hedeflerini tek yerde toplar"
---

# Sistem Genel Bakisi - Moduller, Akislar, Teknoloji Kararlari

**Zorunlu Baglantilar:** [mimari index](../index.md) · [dependency-map](dependency-map.md) · [catalog index](../catalog/index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)

> **Tek cumlelik sorumluluk:** *VersaCoder ne modullerden olusur, acilista/gorevde/sorguda ne olur, hangi teknoloji neden secildi* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**VersaCoder, WinForms + DevExpress v26.1 kabuklu bir AI coding platformudur; 5 modul, 8 makro katman (l0..l7) ve iki TFM (`net48` + `net10.0-windows`) uzerine kuruludur.** M1 (Desktop) ve M2 (CLI) iki kabuktur, ayni `M3 Application+Domain · M4 Infrastructure · M5 Protocol` cekirdegin paylasir; kabuklar SQL yazmaz, entity yaratmaz - hepsi `l0` portlarina bakar. Veri erisimi **ADO.NET + Dapper**, EF Core yasaktir. Katman **sinniri** [mimari index](../index.md) §2'de, bagimlilik **matrisi** [dependency-map](dependency-map.md)'dedir (SRP).

| Alan | Deger |
|---|---|
| Urun | VersaCoder - AI Coding Platform / Agentic IDE |
| Kabuk | `VersaCoder.Desktop` (7 ekran) + `VersaCoder.Cli` (headless) |
| Protokol | opencode v1.18.32 uyumlu adaptör (PART 7) |
| Veri | ADO.NET + Dapper · SQLite / SQL Server · EF Core = 0 |
| Gate | GATE-004a (vault) -> GATE-003 (onay) -> GATE-004 (ilk commit) |

---

## §1 Moduller (5 modul)

| # | Modul | Katman / Proje | Sorumluluk | Girdi | Cikti |
|---|---|---|---|---|---|
| M1 | Desktop WinForms host | l7 `VersaCoder.Desktop` + l6 `VersaCoder.Host` | 7 ekran, ribbon kabugu, DI'dan servis cekme, `Program.Main` | kullanici olayi, DTO | ekran durumu, `Result<T>` |
| M2 | CLI host | l7 `VersaCoder.Cli` + l6 `VersaCoder.Host` | arguman ayristirma, satir ciktisi, headless gorev | komut satiri argumani | stdout / stderr, cikis kodu |
| M3 | Application + Domain core | l2 `VersaCoder.Application` + l1 `VersaCoder.Domain` | use case, handler, DTO, entity, invariant, dogrulama | komut/sorgu nesnesi | `Result<T>` + DTO |
| M4 | Infrastructure | l4 `VersaCoder.Infrastructure` | ADO.NET + Dapper, provider, migration, dosya sistemi | port cagrisi | satir/set, `Result.Fail` |
| M5 | Protocol adapter | l5 `VersaCoder.Protocol` | tool/session/transcript/mesaj, agent dongusu siniri | mesaj akisi (JSON) | tool cagrisi, transcript |

```text
  KULLANICI
     |
     +--> M1 Desktop (l7)      M2 Cli (l7)
     |          \               /
     |           v             v
     |        +---------------------+
     |        |  l6 HOST (DI + cfg) |
     |        +---------------------+
     |           |       |       |
     |           v       v       v
     |        [M3 App] [M4 Infra] [M5 Protocol]
     |           |       |       |
     +-----------+-------+-------+
                     v
            [ l0 ABSTRACTIONS ] --> dis dunya (SQLite / SQL / LLM API / dosya)
```

**Okuma:** oklar asagi referanstir; yukari ok varsa [dependency-map](dependency-map.md) §3 ihlalidir.

---

## §2 Calisma Akislari

| Akis | # | Katman | Ne olur | Hata yolu |
|---|---|---|---|---|
| Acilis | 1 | l7 | `Program.Main`, tek ornek kilidi (`Mutex`) | ikinci ornek -> mesaj + cikis |
| Acilis | 2 | l6 | Host builder + config (varsayilan -> dosya -> ortam) | okunamazsa varsayilan + log |
| Acilis | 3 | l3 | logger + sifreleme anahtari hazirlanir | anahtar yoksa DB adimi **durur** |
| Acilis | 4 | l4 | provider secimi (`sqlite`/`sqlserver`) + migration | bilinmeyen deger -> `Result.Fail` |
| Acilis | 5 | l6 | DI kayitlari (port <-> adaptor) + sicak baglanti sinama | cozumlenemezse kayit hatasi |
| Acilis | 6 | l7 | Desktop: `MainForm` · Cli: arguman islenir | - |
| Gorev | 1 | l7 -> l2 | komut dogrulama (varlik, yetki, butce) | `Result.Invalid` |
| Gorev | 2 | l5 | session + transcript acilir, dongu baslar | - |
| Gorev | 3 | l5 -> l0 | model -> yanit -> tool cagrisi -> port yonlendirme | `Result<ToolOutput>` |
| Gorev | 4 | l4 | transcript + task durumu parametreli SQL ile yazilir | concat = RED |
| Gorev | 5 | l7 -> l2 | akis olaylari ekran'a duser, durum `Completed/Failed` | exception disari sizmaz |
| Sorgu | 1 | l7 -> l2 | `IQueryHandler<TQuery,TResult>` + giris dogrulama | `Result.Invalid` |
| Sorgu | 2 | l0 | repository/port cagrisi - **SQL burada gorunmez** | arayuz sozlesmesi |
| Sorgu | 3 | l4 | baglanti acma + sabit metin SQL + `@p1..@pN` | concat = RED |
| Sorgu | 4 | l4 | her komutta acik `CommandTimeout` (varsayilan 30 sn) | sure asimi -> `Result.Fail` |
| Sorgu | 5 | l4 -> l2 -> l7 | Dapper satiri DTO'ya eslenir, olculmus sure doner | bos sonuc != null |

**Sinirlar:** l5 donguyu **kurar ve durdurur**, SQL **yazmaz**; l4 SQL yazar, donguyu **bilmz**; l7 yalniz gosterir ve iptal sinyali gonderir. Hedef butce: l7->l2 <= 5 ms · l2 dogrulama <= 2 ms · l4 sorgu <= 50 ms · toplam ekran yanit <= 150 ms.

---

## §3 Teknoloji Kararlari (degismez)

| # | Alan | Secilen | Reddedilen | Gerekce |
|---|---|---|---|---|
| T1 | TFM | `net48` + `net10.0-windows` | tek net10, .NET 8 | legacy kanit + modern yuzey |
| T2 | Veri erisimi | ADO.NET + Dapper | **EF Core**, ORM sihri | sorgu gorunur, iki TFM'de ayni |
| T3 | Sorgu guvenligi | parameterized `@p1..@pN` | string concat / interpolasyon | enjeksiyon yuzeyi sifir |
| T4 | Saglayici | `Microsoft.Data.Sqlite` + `Microsoft.Data.SqlClient` | tek saglayiciya kilit | provider abstraction |
| T5 | Migration | saf SQL + `__Migrations` versiyon tablosu | EF migrate | checksum + geri alinamazlik acik |
| T6 | UI kutuphanesi | DevExpress Universal **v26.1** | 24.2 / 23.x | surum kilidi (G3) |
| T7 | Ribbon stili | `RibbonControlStyle.Office2010` | diger stiller | tek gorsel dil, GATE-003 |
| T8 | Form kalibi | `.cs` + `.Designer.cs` + `.resx` | Designer'siz form | U5 sahipligi `ui-designer` |
| T9 | Paket | CPM (`Directory.Packages.props`) | proje basina surum | tek surum kaynagi (G8) |
| T10 | Test | xUnit + NSubstitute + NetArchTest | kanitsiz coverage | birim + sahte + mimari test |
| T11 | Protokol | opencode v1.18.32 adaptörü | kendi protokolunu icat etmek | uyumluluk (PART 7) |
| T12 | Uygulama | 2 (`Desktop`, `Cli`) | tek uygulama | masaustu + headless ayni cekirdek |

> **KURAL:** T1-T12 degismez; degisiklik `DECISION BLOCKED` + ADR gerektirir ([CLAUDE.md](../../CLAUDE.md) G8/G10).

---

## §4 Veri, Kalicilik ve NFR

| # | Konu | Karar |
|---|---|---|
| D1 | Yerel / sunucu DB | SQLite dosyasi · SQL Server (`SqlClientProvider`), config ile degisir |
| D2 | Sehma hareketi | ileri yönlü saf `.sql`; down script yok -> geri alinamazsa **raporlanir** |
| D3 | Versiyon tablosu | `__Migrations` (`ScriptName`, `Checksum`, `AppliedUtc`); uyusmazlik = **dur** |
| D4 | Baglanti dizesi | **sifreli** saklanir, bellekte cozulur; diskte duz metin = 0 |
| D5 | Yazma butcegunlugu | tek transaction + rollback; dosya yazimi atomik (gecici -> degistir) |

| # | NFR | Hedef | Olcum |
|---|---|---|---|
| N1 | Acilis | Desktop <= 3 sn (sicak DB) | stopwatch log |
| N2 | Ekran yanit | <= 150 ms | log suresi |
| N3 | Iki bacak | iki TFM'de build 0 hata + test yesil | `dotnet build` / `dotnet test` |
| N4 | Enjeksiyon + EF | concat SQL = 0 · EF referansi = 0 | kod + paket taramasi |
| N5 | Test | birim hedefi %90; mimari test 0 ihlal | `dotnet test` + NetArchTest |
| N6 | Bellek | surekli calisma <= 256 MB | `dotnet-counters` |

> **CONFLICT DETECTED:** versiyon tablosu icin iki ad var: kanonik ad **`__Migrations`**, [CLAUDE.md](../../CLAUDE.md) G2 ise `schema_version` yaziyor. Ayni kavramdir; **tek ad ADR ile sabitlenmelidir** - cozulene kadar bu vault `__Migrations` kullanir (G10).

---

## §5 Sinir ve Dogrulama

| Bu dosya NE YAPMAZ | Nerede |
|---|---|
| Katman izin/yasak matrisi | [dependency-map](dependency-map.md) §2 |
| Katman bazli YAPAR/YAPMAZ + N01-N44 | [mimari index](../index.md) §2 · [catalog index](../catalog/index.md) |
| Bilesen kayit listesi | [catalog index](../catalog/index.md) §1 |
| Ekran wireframe'i (7 ekran) | ui-design (PART 4, GATE-003) |

- [ ] Frontmatter 8 alan · `updated: 2026-09-29`
- [ ] §1 = 5 modul · §2 = 3 akis · §3 = 12 karar · §4 = 5 veri + 6 NFR
- [ ] Tum diyagramlar yalniz `| + - > [ ]` (Unicode kutu cizim = 0)
- [ ] EF Core / v26.1 disi surum / 3. TFM onerisi yok
- [ ] `check-links.ps1` -> bozuk link 0

**Kaynaklar:** [mimari index](../index.md) (l0..l7, N01-N44, 9 proje) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) (SSOT) · [00-kapsam-analizi](../../reports/00-kapsam-analizi.md) (5 karar, R1-R6) · [keys.md](../../keys.md) (teknoloji -> dosya).
