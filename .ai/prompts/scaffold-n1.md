---
title: "prompt - Scaffold N1 (500 .csproj, K0850-K1299)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "TODOS/06 Faz A / N1 - 5 paket (Data, Config, Logging, Security, FileSystem) + 455 scaffold = 500 .csproj; 3 kanit zorunlu, GATE-005"
---

# prompt — Scaffold N1 (500 .csproj, K0850-K1299)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [TODOS/06](../TODOS/06-5000-katman-kurulumu.md) · [k5000-dll-list](../architecture/layer-5000/k5000-dll-list.md) · [yon-gruplari](../architecture/layer-5000/yon-gruplari.md) · [layer-5000-check](../checklists/layer-5000-check.md)

> **Tek cumlelik sorumluluk:** Faz A / **N1** — 5 mikro paket (Data, Config, Logging,
> Security, FileSystem) + 455 scaffold (`K0850-K1299`) = **500 `.csproj`** üreten,
> **3 kanıtla** kapanan **GATE-005** is emridir.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Faz | **A / N1** (TODOS/06 §1 Blok A, Adım 11-15) |
| Hedef | `count-dll.ps1` = **500** uretim `.csproj` |
| K bandı | `K0850-K1299` (455 scaffold) + `K0200-K0204` (5 mikro) |
| TFM | `net48;net10.0-windows` |
| Gereksinim | `VC-LAY-011` (29 paket → yön grubu eşlemesi) |
| Ajan | `developer` (5 paket) → `tester` (build) → `reviewer` |
| Kapı | GATE-005 |
| Kanıt | 3 tane: build x2 + NetArchTest + `count-dll` = 500 |

**YASAK:** iş mantığı (scaffold) · `git add -A` · `.sln` dışı `.csproj` (LAY-06).

---

## §1 Baglam

1. `TODOS/06 §1` Blok A: Adım 11 (sınıf etiketle) → 12 (5 `.csproj` aç) → 13 (**N1 scaffold**) → 14 (matris 500²) → 15 (sınıf taşı + 3 kanıt).
2. `k5000-dll-list §3.1`: `K0850-K0999` Foundation (150) + `K1000-K1149` AI Agent (150) + `K1150-K1199` CodeIntel (50) + `K1200-K1299` IDE (100) = 455.
3. `yon-gruplari §1`: yeni yön grubu = 6'dan fazla modül → bölme + eski ağaç adı.

---

## §2 Görev — Adım adım (TODOS/06 §1 Blok A)

| # | Adım | İş | Kanıt |
|---|---|---|---|
| 11 | Sınıf etiketle | `Infrastructure` içindeki her sınıf → hangi 29 pakete | sınıf→paket tablosu |
| 12 | 5 `.csproj` aç | `Infrastructure.Data` · `.Config` · `.Logging` · `.Security` · `.FileSystem` | toplam **50** + GATE-004 kaydı |
| 13 | **N1 scaffold** | `tools\gen-5000-csproj.ps1 -Phase N1` (455 klasör) | `count-dll.ps1` = **500** |
| 14 | Matris 500² | `tools\gen-matrix-5000.ps1` yeniden üret | kural seti = 500 satır |
| 15 | Sınıf taşı | sınıfları ilgili pakete taşı (namespace güncelle) | build x2 + NetArchTest 0 + sayac PASS |

**Kabul kriterleri:**

- [ ] 5 mikro paket `.csproj` + 455 scaffold = **500** uretim
- [ ] `count-dll.ps1` = 500 (marj −4500, hatalı format 0)
- [ ] 29 paket → K0000-K0849 eşlemesi (VC-LAY-011)
- [ ] `git status` temiz

---

## §3 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> NetArchTest LayerRulesTests 0 ihlal
KANIT 3: tools\count-dll.ps1 -> 500 uretim (marj -4500, hatali 0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : N1 scaffold 500 csproj                  |
| KAPSAM : src/Infrastructure.{Data,Config,...}    |
|          + K0850-K1299 (455)                     |
| KARAR  : gen-5000-csproj -Phase N1               |
| RISK   : marj -4500 (FAIL) - N4'te kapanir       |
| KANIT  : build x2=0 + NetArchTest=0 + count=500  |
+--------------------------------------------------+
```

---

## §4 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 5 paket + 455 scaffold |
| 2 | `tester` | build + NetArchTest |
| 3 | `reviewer` | VC-LAY-011 eşlemesi |

---

**Olculen kanit:** `.ai/prompts/scaffold-n1.md` kaydedildi; `prompts/` = 43 dosya.
