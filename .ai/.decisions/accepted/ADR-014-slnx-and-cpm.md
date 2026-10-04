---
title: "ADR-014 — VersaCoder.slnx + Central Package Management"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "çözüm dosyası biçimi (.slnx), paket sürüm tekeli (Directory.Packages.props) ve sürüm kilidi"
---

# ADR-014 — VersaCoder.slnx + Central Package Management

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-012](ADR-012-dual-tfm-data-provider.md) · [ADR-004](ADR-004-devexpress-universal-v261-office2010.md)

> **Tek cümlelik karar:** Çözüm **`VersaCoder.slnx`** olarak, paket sürümleri **tek noktada `Directory.Packages.props` (CPM)** içinde tutulur; `.csproj` dosyalarında `Version` yazmaz, sürümü **yalnız** kilit dosyası verir — karar verildi.

---

## §0 Özet (Answer First)

Sürüm bilgisi dağınıksa her `.csproj` kendi gerçeğini söyler ve iki bacak arasında sessiz fark doğar. Bu karar sürümün **tek** olduğu yeri kilitler: `Directory.Packages.props` (CPM, `ManagePackageVersionsCentrally=true`) — DevExpress **26.1**, Dapper **2.x**, SqlClient **5.x** gibi değerler orada tek satırdır. Çözüm dosyası **`.slnx`** (XML, diff'lenebilir) ile sekiz çekirdek + iki host tek yerde listelenir. Yeni paket/versiyon eklemek **her koşulda G8 (kullanıcı onayı)** ister.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Paket kilidi denetimi "tek kilit listesi" ister; ikinci framework satırı ihlaldir | `CLAUDE.md` §denetim G8 |
| C2 | DevExpress sürümü **tek** değer: 26.1; ikinci sürüm satırı = ihlal | [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) |
| C3 | SqlClient 5.x / Dapper 2.x iki bacakta aynı sürüm olmalı | [ADR-012](ADR-012-dual-tfm-data-provider.md) D1–D2 |
| C4 | 8 çekirdek + 2 host projesi tek çözümde toplanır | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2 · [ADR-010](ADR-010-cli-desktop-shared-core.md) |
| C5 | İki bacak (`net48` + `net10.0-windows`) koşullu paket davranışını tolere etmez | [ADR-001](ADR-001-dual-target-net48-net10.md) |
| C6 | `global.json` ile SDK sürümü sabitlenmemişse CPM davranışı makineye göre değişir | `.templates/adr/adr-template.md` §3.9/6 |

---

## §2 Karar

**Karar:** Paket sürümleri **tek dosyada** (`Directory.Packages.props`) yaşar, proje dosyaları yalnız `PackageReference` yazar; çözüm biçimi **`.slnx`**'tir ve SDK `global.json` ile sabitlenir — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Kilit dosyası | `Directory.Packages.props` → `ManagePackageVersionsCentrally=true` | C1 |
| D2 | `.csproj` kuralı | `<PackageReference Include="X" />` — **`Version` atribütü yasak** | C1 · C3 |
| D3 | Çözüm | `VersaCoder.slnx` (metin XML) — 8 çekirdek + `VersaCoder.Desktop` + `VersaCoder.Cli` | C4 |
| D4 | SDK | `global.json` ile sürüm + `latestFeature` rollForward (şablon §3.9/6) | C6 |
| D5 | Koşullu sürüm | CPM'te **koşullu `VersionOverride` yok** → iki bacak aynı sürümü görür | C5 |
| D6 | Değişiklik yetkisi | yeni paket / sürüm yükseltmesi = **G8 onayı** (kullanıcı) | `CLAUDE.md` G8 |
| D7 | DevExpress | `DevExpress.*` paketleri tek satır, değer **yalnız 26.1** | C2 |

**Neden:** CPM, "hangi sürüm gerçekten kullanılıyor" sorusunun **tek** cevabını üretir; bu olmadan iki bacak test matrisi ([ADR-019](ADR-019-dual-tfm-test-matrix.md)) anlamsızdır çünkü aynı test farklı paketlerle koşulabilir. `.slnx` seçimi, klasik `.sln`'in binary/gürültülü yapısını yerine okunabilir metin koyar; proje eklemek tek satır işlemidir. Koşullu sürümün yasaklanması (D5), `#if` bolluğunu ve "yalnız net10'da kırık" sürprizlerini kaynağında engeller.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Sürüm tek noktada → denetim tek komut | `Select-String -Path Directory.Packages.props` |
| İki bacak aynı paketleri görür | koşulsuz CPM + restore logu |
| DevExpress tek sürüm | `26.1` tek satır (G8 denetimi) |
| Çözüm diff'lenebilir | `.slnx` metin dosyası, `git diff` okunur |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| `.slnx` desteği yeni SDK ister; eski SDK'da açılmaz | **NEEDS_VALIDATION** (aşağıda) |
| CPM, tek sürüm istisnası gerektiren pakette sıkıntı yaratır | `VersionOverride` yalnız G8 onayıyla |
| `global.json` yanlışsa tüm ekip restore hatası alır | SDK sürümü rapora yazılır, PART 5'te teyit |
| `.slnx` araç desteği (eski VS) sınırlı olabilir | CLI (`dotnet build VersaCoder.slnx`) birincil yol |

**NEEDS_VALIDATION:** `global.json`'da sabitlenecek SDK sürümünün `.slnx`'i desteklemesi **PART 5'te** `dotnet sln list` + `dotnet build VersaCoder.slnx` ile doğrulanır. Desteklenmiyorsa D3 geçici olarak klasik `.sln`'e döner ve **bu ADR yeni ADR ile superseded edilir** (kayıt silinmez).

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-012](ADR-012-dual-tfm-data-provider.md) | related | CPM'e yazılacak paket/sürüm değerleri |
| [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) | related | DevExpress 26.1 kilidi bu dosyada yaşar |
| [ADR-001](ADR-001-dual-target-net48-net10.md) | related | koşulsuz sürüm = iki bacak şartının parçası |
| [ADR-019](ADR-019-dual-tfm-test-matrix.md) | related | matris, aynı paketlerle koşulur |
| [ADR-010](ADR-010-cli-desktop-shared-core.md) | related | çözümün içindeki proje listesi |

---

## §5 Kabul Kriterleri

- [ ] `Directory.Packages.props` var ve `ManagePackageVersionsCentrally=true`
- [ ] Hiçbir `.csproj`'da `Version` atribütü **yok** (kod taraması = 0 satır)
- [ ] `DevExpress.*` sürüm satırı **tek** = `26.1`
- [ ] `dotnet restore` + `dotnet build VersaCoder.slnx` = 0 hata (iki bacak)
- [ ] G8 onayı alınmadan yeni paket eklenmemiş

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: master prompt (CPM) + `CLAUDE.md` G8.
