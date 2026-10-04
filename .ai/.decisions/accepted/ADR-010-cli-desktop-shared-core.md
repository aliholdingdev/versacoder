---
title: "ADR-010 — Desktop ve CLI Aynı Çekirdeği Paylaşır (l0..l6)"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "iki uygulama (WinForms Desktop + konsol CLI) arasındaki ortak çekirdek sınırı, proje ayrımı ve bağımlılık kuralı"
---

# ADR-010 — Desktop ve CLI Aynı Çekirdeği Paylaşır (l0..l6)

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-013](ADR-013-layered-architecture-52-nodes.md) · [ADR-001](ADR-001-dual-target-net48-net10.md)

> **Tek cümlelik karar:** `VersaCoder.Desktop` ile `VersaCoder.Cli`, **l0..l6 katmanlarının tamamını (çekirdek) birebir paylaşır**; ayrışma yalnız `l7`'de (UI/CLI host) olur ve çekirdeğe `DevExpress.*` / `System.Windows.Forms` referansı **girmez** — karar verildi.

---

## §0 Özet (Answer First)

Aynı iş mantığı iki kez yazılırsa iki gerçek doğar ve biri er ya da geç yanlış olur. Bu karar tek bir `VersaCoder.Core` yerine **var olan 8 projeyi (l0..l6 + l7)** korur: `l0..l6` iki hostun da referans verdiği ortak çekirdek, `l7` ise `VersaCoder.Desktop` (WinForms + DevExpress) ve `VersaCoder.Cli` (konsol) olarak ayrışır. Çekirdek, UI teknolojisinden **bilinçli olarak habersizdir**; NetArchTest bunu sürekli denetler. Böylece CLI, headless ortamda (CI, sunucu) aynı davranışı gösterir.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Mimari 8 makro katman (l0..l7) + 44 alt katman = 52 düğüm olarak tanımlı | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2 |
| C2 | `l7 UI + CLI` altında `shell-ribbon` (Desktop) ve `settings-cli` (CLI) ayrı düğümler | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2 N41–N44 |
| C3 | Bağımlılık yönü tek yönlü; geri dönüş yalnız `interface` (l0) ile | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2.3 · NetArchTest |
| C4 | DevExpress **yalnız v26.1** ve ribbon **yalnız Office2010**; CLI'da bu paketlere gerek yok | [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) |
| C5 | Her kod parçası iki bacakta değerlendirilir; `#if` yalnız platform/API farkı | [ADR-001](ADR-001-dual-target-net48-net10.md) |
| C6 | CLI, build/test zincirinin başında çalışır (headless) | `CLAUDE.md` §denetim G5 |

---

## §2 Karar

**Karar:** Uygulama ikiye bölünür: **ortak çekirdek l0..l6** + **iki ince host (Desktop/Cli) l7**; çekirdeğe UI paketi referansı verilmez, ortak davranış tek yerde yaşar — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Paylaşılan | `VersaCoder.Abstractions` … `VersaCoder.Host` (l0..l6) — her iki host da referanslar | C1 |
| D2 | Ayrışan | `VersaCoder.Desktop` (DevExpress + WinForms) · `VersaCoder.Cli` (konsol + `System.CommandLine`-benzeri basit argüman ayrıştırıcı) | C2 |
| D3 | Yasak referans | çekirdek projelerinde `DevExpress.*` ve `System.Windows.Forms` = **0 satır** | C4 |
| D4 | Denetim | NetArchTest: çekirdek → UI yönünde bağımlılık **0 ihlal**; ters referans yalnız `interface` | C3 |
| D5 | Yapılandırma | iki host **aynı** konfigürasyon dosyasını ve `l6 config-bridges`'ini kullanır | C1 |
| D6 | TFM | ortak çekirdek **iki bacak**; hostlar kendi TFM'lerinde derlenir | C5 |

**Neden:** İki uygulama, aynı iş mantığını iki kez yazarsa davranış farkı kaçınılmazdır ve bu fark ancak üretimde ortaya çıkar (tek gerçek ihlali). `l7`'de ayrışmak, **en düşük maliyetli ayrışma** noktasıdır: UI teknolojisi zaten doğal olarak ayrılır. Çekirdeğe UI paketi sokmamak, CLI'ın DevExpress lisansına/paketine bağımlı olmasını engeller ve headless ortamda derlemeyi mümkün kılar.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| İş mantığı tek yerde → iki host aynı davranışı gösterir | ortak çekirdek testleri tek sefer yazılır, iki host koşar |
| CLI headless çalışır (CI'da DevExpress gerekmez) | çekirdekte `DevExpress.*` = 0 satır |
| Bağımlılık kuralı sürekli denetlenir | NetArchTest `LayerRulesTests` = 0 ihlal |
| İki bacak tek noktada korunur | ortak çekirdek `net48` + `net10.0-windows` |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Çekirdek, UI'a özel davranış taşırsa kirletilir | D3 + NetArchTest; ihlal `reviewer` için RED |
| CLI'nın kendi ekranda gösterdiği veri Desktop'tan farklılaşabilir | ortak DTO (l2 `dto-mappers`) tek kaynak |
| İki host = iki derleme/CI işi | [ADR-019](ADR-019-dual-tfm-test-matrix.md) matrisi tek komutta koşar |
| `.slnx` içinde proje sayısı artar | [ADR-014](ADR-014-slnx-and-cpm.md) tek çözüm + CPM |

**NEEDS_VALIDATION:** `VersaCoder.Cli` argüman ayrıştırıcısı için harici paket G8 onayı ister; onay gelene kadar **elle yazılmış** küçük bir ayrıştırıcı (l7 `settings-cli` içinde) kullanılır, paket eklenmez.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | extends | 52 düğüm ağacının "ne paylaşılır/ne ayrılır" kararını netleştirir |
| [ADR-001](ADR-001-dual-target-net48-net10.md) | related | ortak çekirdeğin iki bacakta derlenmesi |
| [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) | related | DevExpress'in yalnız Desktop hostuna hapsi |
| [ADR-019](ADR-019-dual-tfm-test-matrix.md) | related | iki host × iki bacak test matrisi |
| [ADR-014](ADR-014-slnx-and-cpm.md) | related | çözüm dosyası ve paket kilidi |

---

## §5 Kabul Kriterleri

- [ ] Çekirdek projelerde `DevExpress.*` / `System.Windows.Forms` = **0** (kod taraması)
- [ ] NetArchTest: çekirdek → UI bağımlılığı **0** ihlal
- [ ] Aynı senaryo testi hem Desktop hem CLI hostunda koşuyor
- [ ] `dotnet build` her iki host + her iki bacak = 0 hata
- [ ] `.slnx` içinde iki host + 7 çekirdek proje listeleniyor

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: master prompt PART 7 + ADR-013 l0..l7 yapısı.
