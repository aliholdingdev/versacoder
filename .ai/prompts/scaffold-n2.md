---
title: "prompt - Scaffold N2 (1500 .csproj, K1300-K2299)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "TODOS/06 Faz B / N2 - 7 paket (AI, Auth, Caching, Context, Messaging, Network, Observability) + 1000 scaffold = 1500 .csproj; 3 kanit zorunlu, GATE-005"
---

# prompt — Scaffold N2 (1500 .csproj, K1300-K2299)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [TODOS/06](../TODOS/06-5000-katman-kurulumu.md) · [k5000-dll-list](../architecture/layer-5000/k5000-dll-list.md) · [yon-gruplari](../architecture/layer-5000/yon-gruplari.md) · [layer-5000-check](../checklists/layer-5000-check.md)

> **Tek cumlelik sorumluluk:** Faz B / **N2** — 7 mikro paket (AI, Auth, Caching, Context,
> Messaging, Network, Observability) + 1000 scaffold (`K1300-K2299`) = **1500 `.csproj`**
> üreten, **3 kanıtla** kapanan **GATE-005** is emridir.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Faz | **B / N2** (TODOS/06 §1 Blok B, Adım 16-19) |
| Hedef | `count-dll.ps1` = **1500** uretim `.csproj` |
| K bandı | `K1300-K1799` AI Agent (500) + `K1800-K2099` CodeIntel (300) + `K2100-K2299` IDE (200) = 1000 scaffold |
| TFM | `net48;net10.0-windows` |
| Ajan | `developer` → `tester` → `reviewer` |
| Kapı | GATE-005 |
| Kanıt | 3 tane: build x2 + NetArchTest + `count-dll` = 1500 |

**YASAK:** iş mantığı · `git add -A` · `.sln` dışı `.csproj` (LAY-06).

---

## §1 Baglam

1. `TODOS/06 §1` Blok B: Adım 16 (7 `.csproj`) → 17 (**N2 scaffold**) → 18 (Logging taşı) → 19 (sınıf taşı + 3 kanıt).
2. `k5000-dll-list §3.2`: 1000 scaffold (`K1300-K2299`).

---

## §2 Görev — Adım adım (TODOS/06 §1 Blok B)

| # | Adım | İş | Kanıt |
|---|---|---|---|
| 16 | 7 `.csproj` aç | AI · Auth · Caching · Context · Messaging · Network · Observability | toplam **57** + GATE-004 kaydı |
| 17 | **N2 scaffold** | `tools\gen-5000-csproj.ps1 -Phase N2` (1000 klasör) | `count-dll.ps1` = **1500** |
| 18 | Logging taşı | `Logging` `l11 Platform` → `l14 Observability` | namespace + katalog güncel, 0 hata |
| 19 | Sınıf taşı + kanıt | B grubu sınıfları | 3 kanıt |

**Kabul kriterleri:**

- [ ] 7 mikro paket + 1000 scaffold = **1500** uretim
- [ ] `count-dll.ps1` = 1500 (marj −3500, hatalı format 0)
- [ ] `git status` temiz

---

## §3 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> NetArchTest LayerRulesTests 0 ihlal
KANIT 3: tools\count-dll.ps1 -> 1500 uretim (marj -3500, hatali 0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : N2 scaffold 1500 csproj                 |
| KAPSAM : src/Infrastructure.{AI,Auth,...} (7)    |
|          + K1300-K2299 (1000)                    |
| KARAR  : gen-5000-csproj -Phase N2               |
| RISK   : marj -3500 (FAIL) - N4'te kapanir       |
| KANIT  : build x2=0 + NetArchTest=0 + count=1500 |
+--------------------------------------------------+
```

---

## §4 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 7 paket + 1000 scaffold |
| 2 | `tester` | build + NetArchTest |
| 3 | `reviewer` | Logging taşıma |

---

**Olculen kanit:** `.ai/prompts/scaffold-n2.md` kaydedildi; `prompts/` = 44 dosya.
