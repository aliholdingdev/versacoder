---
title: "prompt - Scaffold N4 (5000 .csproj, K3800-K5499)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "TODOS/06 Faz D / N4 - 10 paket + 2000 scaffold = 5000 .csproj + 5000² matris; 3 kanit zorunlu, GATE-006 (teslim)"
---

# prompt — Scaffold N4 (5000 .csproj, K3800-K5499)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [TODOS/06](../TODOS/06-5000-katman-kurulumu.md) · [k5000-dll-list](../architecture/layer-5000/k5000-dll-list.md) · [dependency-map](../architecture/00-overview/dependency-map.md) · [layer-5000-check](../checklists/layer-5000-check.md) · [ADR-030](../.decisions/accepted/ADR-030-5000-layers-class-level.md)

> **Tek cumlelik sorumluluk:** Faz D / **N4** — 10 mikro paket + 2000 scaffold
> (`K3800-K5499`) = **5000 `.csproj`** + **5000² matris** üreten, **3 kanıtla** kapanan
> **GATE-006 (teslim)** is emridir — **hedefe ulaşılır**.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Faz | **D / N4** (TODOS/06 §1 Blok D, Adım 24-29) |
| Hedef | `count-dll.ps1` = **5000** uretim `.csproj` (marj **0**, L-08) |
| K bandı | `K3800-K4299` IDE (500) + `K4300-K4599` Plugin (300) + `K4600-K4999` Enterprise (400) + `K5000-K5499` Future (500) = 1700 scaffold (+ 300 eksik → K0850-K0999, §3.5) |
| TFM | `net48;net10.0-windows` |
| Gereksinim | ADR-030 §2.1 (tek eşik `>= 5000`) + §2.5 (6 ölçüm) |
| Ajan | `developer` → `tester` → `reviewer` → `architect` (kapanış) |
| Kapı | **GATE-006 (teslim)** |
| Kanıt | 3 tane: build x2 + NetArchTest 5000² + `count-dll` = 5000 |

**YASAK:** iş mantığı · `git add -A` · `.sln` dışı `.csproj` · eşik düşürme (G5/L-08).

---

## §1 Baglam

1. `TODOS/06 §1` Blok D: Adım 24 (10 `.csproj`) → 25 (**N4 scaffold**) → 26 (**matris 5000²**) → 27 (eşleme kapa) → 28 (düğüm 5000) → 29 (sınıf taşı + 3 kanıt).
2. `k5000-dll-list §3.4-§3.5`: N4 = 1700 + 300 (Foundation kalan) = 2000 → **kontrol `count-dll` = 5000**.
3. `dependency-map §6.1`: `l_i → l_j` izinli ⇔ `j ≤ i` (5000² = 25.000.000 hücre, elle yazılmaz).
4. `ADR-030 §2.5`: 6 ölçüm — `count-components` · `gen-layer-5000` · dosya sayımı · NetArchTest · build x2 · **restore 5000 csproj**.

---

## §2 Görev — Adım adım (TODOS/06 §1 Blok D)

| # | Adım | İş | Kanıt |
|---|---|---|---|
| 24 | 10 `.csproj` aç | CodeAnalysis · ProjectAnalysis · Refactoring · Testing · Templating · Documentation · Reporting · Diagram · Deployment · Learning | toplam **74** |
| 25 | **N4 scaffold** | `tools\gen-5000-csproj.ps1 -Phase N4` (2000 klasör) | `count-dll.ps1` = **5000** |
| 26 | **Matris 5000²** | `tools\gen-matrix-5000.ps1` son üretim (5000 satır) | NetArchTest 0 ihlal (25M hücre üretilmiş) |
| 27 | Eşleme kapa | 29 paket → 7 bant (K0000-K0849) | §1.2 toplamı = **29** |
| 28 | Düğüm 5000 | 44 → 5000 düğüm (`N53..`) — **katalog işi, kod değil** | `Kayit dosyasi: 5000` |
| 29 | Sınıf taşı + kanıt | D grubu sınıfları | 3 kanıt |

**Kabul kriterleri (ADR-030 §2.5 + layer-5000-check):**

- [ ] `count-dll.ps1` = **5000** uretim + 2 test (marj **0**, hatalı format 0) → **PASS**
- [ ] `count-components.ps1` = tip kaydı `>= 5000` (bugün 5440 PASS)
- [ ] `gen-layer-5000.ps1` = namespace parça `>= 5000` (bugün 5834 PASS)
- [ ] `dotnet restore` 5000 `.csproj` = **0 hata** (ADR-030 K6)
- [ ] NetArchTest 5000² = 0 ihlal
- [ ] 8x8 ölçüm korunur (20 OK / 40 X / 4 -)
- [ ] `.sln` ↔ `.slnx` diff = 0 (VC-LAY-013)

---

## §3 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> NetArchTest 5000² 0 ihlal
KANIT 3: tools\count-dll.ps1 -> 5000 uretim (marj 0, hatali 0) = PASS
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : N4 scaffold 5000 csproj - HEDEF ULASTI  |
| KAPSAM : src/Infrastructure.{...} (10) +         |
|          K3800-K5499 (1700) + K0850 (300)        |
| KARAR  : gen-5000-csproj -Phase N4 + matris 5000²|
| RISK   : restore 5000 csproj hic olculmedi (K6)  |
| KANIT  : build x2=0 + 5000² 0 ihlal + count=5000 |
+--------------------------------------------------+
```

---

## §4 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 10 paket + 1700 scaffold |
| 2 | `tester` | build + NetArchTest 5000² |
| 3 | `reviewer` | Eşleme + 8x8 korunumu |
| 4 | `architect` | ADR-030 §2.5 6 ölçüm + GATE-006 |

---

**Olculen kanit:** `.ai/prompts/scaffold-n4.md` kaydedildi; `prompts/` = 46 dosya.
