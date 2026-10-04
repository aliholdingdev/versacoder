---
title: "ADR-028 — Katmanlı Mimari Baz Kararı: 8 Makro Katman / 52 Düğüm / 9 Proje"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "VersaCoder'in katman sayisini, proje (csproj) sayisini, tier/618 DLL envanterinin vault'taki yerini ve ilk build oncesi 6 zorunlu duzeltmeyi kilitler. ADR-013'u dogrular, K3 (mimari-02) 9 Tier / 500+ DLL iddiasini reddeder."
---

# ADR-028 — Katmanli Mimari Baz Karari: 8 Makro Katman / 52 Dugum / 9 Proje

**Zorunlu Baglantilar:** [index](../../index.md) | [architecture/index](../../architecture/index.md) | [ADR-013](ADR-013-layered-architecture-52-nodes.md) | [CLAUDE.md](../../CLAUDE.md) | [WORKFLOW.md](../../WORKFLOW.md)

> **Tek cumlelik karar:** VersaCoder **8 makro katman (l0..l7) + 44 alt katman = 52 dugum** olarak modellenir; uretim **9 proje + 1-2 test projesi** (cift TFM `net48;net10.0-windows`); K3'un 9 Tier / 618 DLL'lik envanteri vault kataloguna **girmez**, yalniz `reports/` altinda `DOĞRULAMA GEREKLI` kanit olarak yasar; katalogda **tek sayim birimi `N###` kayittir.**

---

## §1 Bağlam (Context)

| # | Gercek | Kanit |
|---|---|---|
| C1 | Iki rakip mimari taslak vardi: vault ADR-013 (8 makro / 52 dugum / 9 proje) vs prompt K3 `versacoder.mimari-02.md` (9 Tier / 500+ DLL / 5000+ bilesen) | bu ADR §3 |
| C2 | K3'un elle sayilan envanteri **618 giris / 616 benzersiz**; 2 birebir tekrar (Performance 642/662, Security 643/663), 9 basliktan 2'si yanlis, 4 DLL dosyada yok, §15 "nihai degil" | `reports/k3-618-ham-envanter.md` |
| C3 | 3 tur, 21 ajanlik mimari tartismasi yapildi (10 senior -> 5 senior -> 3 junior + 3 ajans) | bu ADR §3 |
| C4 | `scripts/count-components.ps1` bugun calistirildi: **52 dosya / 5440 kayit / 0 bozuk format / PASS** | gate-004a Soru 5 |
| C5 | `Directory.Packages.props` su an yalniz 5 paket; NetArchTest + xunit + Test.Sdk yok | `Directory.Packages.props` satir 6-15 |
| C6 | Yon celligi var: ADR-013 §2.3 `l4 -> l2` derken dependency-map `l4 -> l2 = X` der | `architecture/00-overview/dependency-map.md` §2 |

---

## §2 Karar (Decision)

### 2.1 Katman ve proje (degismez)

| # | Makro | Alt katman (N kodu) | Proje (.csproj) | TFM |
|---|---|---|---|---|
| 1 | l0 Abstractions | N01-N05 | `VersaCoder.Abstractions` | iki bacak |
| 2 | l1 Domain | N06-N11 | `VersaCoder.Domain` | iki bacak |
| 3 | l2 Application | N12-N18 | `VersaCoder.Application` | iki bacak |
| 4 | l3 Crosscutting | N19-N24 | `VersaCoder.Crosscutting` | iki bacak |
| 5 | l4 Infrastructure | N25-N32 | `VersaCoder.Infrastructure` | iki bacak |
| 6 | l5 Protocol | N33-N37 | `VersaCoder.Protocol` | iki bacak |
| 7 | l6 Host | N38-N40 | `VersaCoder.Host` | iki bacak |
| 8 | l7 UI + CLI | N41-N44 | `VersaCoder.Desktop` + `VersaCoder.Cli` | iki bacak |
| — | test | — | `VersaCoder.Tests` (+ gerekirse `VersaCoder.ArchitectureTests`) | iki bacak |

- **9 uretim + 1-2 test = en fazla 11 proje.** Yeni `.csproj` yalniz 3 kapidan gecer: (1) tek cumle sorumluluk, (2) bagimlilik yonu, (3) ayri TFM/paket seti.
- Yon: `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0`; `l3` yalniz `l0`'a bakar; `l2` ile `l4` arasinda **dogrudan ok YOK** (repository portu `l0`'da, uygulamasi `l4`'te).
- Alt katmanlar (N01-N44) **klasor/namespace** duzeyindedir; `architecture/catalog/` altinda birer dosyadir, birer proje degil.

### 2.2 Tier / 618 DLL envanteri (kapsam disi)

- K3'un 618 DLL listesi **kataloga, N-ID'lere ve `count-components.ps1` sayimina girmez.**
- Tek iz: `reports/k3-618-ham-envanter.md` (`status: needs-validation`, `authority: derived`) + bu ADR §3.
- Sayim birimi tektir: `| N##-### |` ile baslayan katalog tablo satiri. Turetilmis/tier verisi **sutun degil, etiketli rapor satiridir.**

### 2.3 Ilk build oncesi 6 zorunlu duzeltme (hepsi GATE-004a kapanisindan once)

| # | Dosya | Duzeltme | Kanit |
|---|---|---|---|
| 1 | `Directory.Packages.props` | `NetArchTest.Rules`, `xunit`, `Microsoft.NET.Test.Sdk`, `NSubstitute` `PackageVersion` satirlari | `dotnet restore` = 0 NU1008 |
| 2 | `architecture/00-overview/dependency-map.md` + ADR-013 ek | 8x8 matris tek kaynak: `l4->l2 X`, `l5->l4 X`, `l3` yalniz `l0` | matris = NetArchTest `LayerRulesTests` |
| 3 | `checklists/layer-boundary-check.md` | "Yedi katman" -> "Sekiz katman (l0..l7)" + yon satiri matrisle ayni | tarama 0 yanlis negatif |
| 4 | `architecture/catalog/index.md` | **N <-> klasor esleme tablosu**: `N33 -> src/VersaCoder.Protocol/ToolSchema/`; dosya basina `node: N##` | yeni tip yeri belirsiz kalmaz |
| 5 | kanit komutlari | tarama yolu gercek proje adlari (`src/VersaCoder.Domain/**`) | `src/**/Domain/**` sahte PASS uretiyor |
| 6 | TFM + l0 portlari | Desktop designer baci belirlenir; test projeleri cift TFM; `IHttpGateway`, `IPermissionPrompt`, `IAgentStreamSink`, `IWorkbenchContract` **l0**'a yazilir | net48 kolu NetArchTest'te kor kalmaz; l4 sismez |

Ek yazi kurallari: proje adi tek yazim `VersaCoder.Crosscutting` (NC-02'deki `CrossCutting` yazimi iptal); semantik stale denetimi ("katalog = kod") gate metnine eklenir — `Durum` degil, **kodla eslesme** kontrolu.

---

## §3 Alternatifler (tartisma kaydi)

| # | Alternatif | Tur 1 | Tur 2 | Tur 3 | Sonuc |
|---|---|---|---|---|---|
| A1 | **ADR-013 birebir + 6 sart (BU ADR)** | 4/10 | **4/5** | **6/6** (5 sartli, 1 kosulsuz) | **KABUL** |
| A2 | Uzlastirma: 8 katman + tier envanteri katalogda (`Tier-DLL` turetilmis sutun) | **6/10** | 1/5 | 0 | RED — ikinci sayim birimi, `count-components` iki birimde RED, staleness 2 kat |
| A3 | K3: 9 Tier / 500+ DLL = ayri proje | 0/10 | 0 | 0 | **UNIVERSAL RED** — 575 csproj x 2 TFM = ~1150 hedef; CPM/VS/restore catlar; K3 kapsim disi DLL'ler (EntityFramework, Host.Linux, Protocol.Kafka) |

**Tur 3 itirazlari (sart olarak §2.3'e alindi):** junior #1 K<->l eslemesi yok / designer cift TFM / `TreatWarningsAsErrors` x `.Designer.cs`; junior #2 "yeni tip nereye yazilir" + sahte PASS + test/Desktop TFM uyusmazligi (NU1201); junior #3 l4 sisersi + Host'a yiginma + Desktop/Cli ortak sozlesme; ajans #1 termin (3 paralel akis) + GATE-004a/003/004 takilmalari; ajans #2 ekranlar 2 sozlesme olmadan acilmaz; ajans #3 gate takilmalari + 5440 esigi **0 marj**.

---

## §4 Sonuclar (Consequences)

**Arti (+)**
- Derleme/restore/IDE yuku sinirli: 9+2 proje x 2 bacak = 22 hedef (K3'te 1150+).
- NetArchTest 8x8 matris ile yon denetimi ilk gunden calisir (sart 1-3 kapaliysa).
- Tek sayim birimi: 5440 kayit PASS, K3 hatalari vault'a sizmaz.
- 3 paralel gelistirme akisi: (l0->l1->l2 seri) -> (l3 || l4 || l5) -> (l6 -> l7 Desktop || Cli).

**Eksi (-) / risk**
- `count-components` gate Q5 esigi **5440 = olculen 5440, marj 0**; tek satir silinirse RED.
- Katalog `Durum` sutunu stale olabilir; semantik denetim gate metnine henuz yazilmadi (ADR §2.3 sart 6).
- l4 tek projede tool-runner + fs + git + db + ag tasir; ic kod disiplini ile denetlenir (assembly siniri yok).

**Takip eden kapi:** [WORKFLOW](../../WORKFLOW.md) GATE-004a -> GATE-003 (kullanici onayi, HARD STOP) -> GATE-004 (ilk commit).
