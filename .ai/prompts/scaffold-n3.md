---
title: "prompt - Scaffold N3 (3000 .csproj, K2300-K3799)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "TODOS/06 Faz C / N3 - 7 paket (Git, MCP, Plugins, Services, Integration, Backup, Versioning) + 1500 scaffold = 3000 .csproj; 3 kanit zorunlu, GATE-005"
---

# prompt — Scaffold N3 (3000 .csproj, K2300-K3799)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [TODOS/06](../TODOS/06-5000-katman-kurulumu.md) · [k5000-dll-list](../architecture/layer-5000/k5000-dll-list.md) · [yon-gruplari](../architecture/layer-5000/yon-gruplari.md) · [layer-5000-check](../checklists/layer-5000-check.md)

> **Tek cumlelik sorumluluk:** Faz C / **N3** — 7 mikro paket (Git, MCP, Plugins, Services,
> Integration, Backup, Versioning) + 1500 scaffold (`K2300-K3799`) = **3000 `.csproj`**
> üreten, **3 kanıtla** kapanan **GATE-005** is emridir.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Faz | **C / N3** (TODOS/06 §1 Blok C, Adım 20-23) |
| Hedef | `count-dll.ps1` = **3000** uretim `.csproj` |
| K bandı | `K2300-K2799` CodeIntel (500) + `K2800-K3299` IDE (500) + `K3300-K3599` Plugin (300) + `K3600-K3799` Enterprise (200) = 1500 scaffold |
| TFM | `net48;net10.0-windows` |
| Ajan | `developer` → `tester` → `reviewer` |
| Kapı | GATE-005 |
| Kanıt | 3 tane: build x2 + NetArchTest + `count-dll` = 3000 |

**YASAK:** iş mantığı · `git add -A` · `.sln` dışı `.csproj` (LAY-06).

---

## §1 Baglam

1. `TODOS/06 §1` Blok C: Adım 20 (7 `.csproj`) → 21 (**N3 scaffold**) → 22 (eşleme güncelle) → 23 (sınıf taşı + 3 kanıt).
2. `k5000-dll-list §3.3`: 1500 scaffold (`K2300-K3799`).

---

## §2 Görev — Adım adım (TODOS/06 §1 Blok C)

| # | Adım | İş | Kanıt |
|---|---|---|---|
| 20 | 7 `.csproj` aç | Git · MCP · Plugins · Services · Integration · Backup · Versioning | toplam **64** |
| 21 | **N3 scaffold** | `tools\gen-5000-csproj.ps1 -Phase N3` (1500 klasör) | `count-dll.ps1` = **3000** |
| 22 | Eşleme güncelle | `l11` +Git · `l16` Integration (+MCP·Plugins·Services) · `l8` +Backup · `l19` +Versioning | §1.2 eşlemesi 29 içinde 19 dağıldı |
| 23 | Sınıf taşı + kanıt | C grubu sınıfları | 3 kanıt |

**Kabul kriterleri:**

- [ ] 7 mikro paket + 1500 scaffold = **3000** uretim
- [ ] `count-dll.ps1` = 3000 (marj −2000, hatalı format 0)
- [ ] `git status` temiz

---

## §3 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 2: dotnet test -> NetArchTest LayerRulesTests 0 ihlal
KANIT 3: tools\count-dll.ps1 -> 3000 uretim (marj -2000, hatali 0)
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : N3 scaffold 3000 csproj                 |
| KAPSAM : src/Infrastructure.{Git,MCP,...} (7)    |
|          + K2300-K3799 (1500)                    |
| KARAR  : gen-5000-csproj -Phase N3               |
| RISK   : marj -2000 (FAIL) - N4'te kapanir       |
| KANIT  : build x2=0 + NetArchTest=0 + count=3000 |
+--------------------------------------------------+
```

---

## §4 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `developer` | 7 paket + 1500 scaffold |
| 2 | `tester` | build + NetArchTest |
| 3 | `reviewer` | Eşleme (VC-LAY-011) |

---

**Olculen kanit:** `.ai/prompts/scaffold-n3.md` kaydedildi; `prompts/` = 45 dosya.
