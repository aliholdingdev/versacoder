---
title: "prompt - Scaffold K0 (Iskelet + 45 .csproj + 5000 kural)"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "TODOS/06 Blok K0 - .sln + 45 cekirdek .csproj + 2 test + 5000 NetArchTest kurali + CPM; 3 kanit zorunlu, GATE-004"
---

# prompt — Scaffold K0 (İskelet + 45 .csproj + 5000 kural)

**Zorunlu Baglantilar:** [prompts/index](index.md) · [AGENTS](../AGENTS.md) · [CLAUDE](../CLAUDE.md) · [TODOS/06](../TODOS/06-5000-katman-kurulumu.md) · [k5000-dll-list](../architecture/layer-5000/k5000-dll-list.md) · [k5000-layer-map](../architecture/catalog/k5000-layer-map.csv) · [dependency-map](../architecture/00-overview/dependency-map.md) · [layer-5000-check](../checklists/layer-5000-check.md) · [layer-rules](../.rules/layer-rules.md)

> **Tek cumlelik sorumluluk:** `.sln` + 45 çekirdek `.csproj` + 2 test projesi + **5000
> NetArchTest kuralı** üreten, **3 kanıtla** (build + restore + NetArchTest) kapanan
> **GATE-004** is emridir — **ilk commit burada**.

---

## §0 Ozet (Answer First)

| Alan | Deger |
|---|---|
| Faz | **K0** (TODOS/06 §1 Blok K0, Adım 4-10) |
| K bandı | `K0000-K0849` (45 çekirdek dolu) |
| TFM | `net48;net10.0-windows` (G6) |
| Gereksinim | `VC-LAY-013` (sln↔slnx senkron) + `VC-LAY-014` (CPM) |
| Ajan | `architect` (karar) → `developer` (iskelet) → `tester` (test) |
| Kapı | **GATE-004** (iskelet + ilk commit) |
| Kanıt | 3 tane: `dotnet restore` 0 · `dotnet build` x2 = 0 · NetArchTest 0 ihlal |

**YASAK:** iş mantığı (tek satır bile) · `git add -A` · `.cs` yazmak (yalnız iskelet) · EF Core.

---

## §1 Baglam

1. `TODOS/06 §1` Blok K0: Adım 4 (`.sln`) → 5 (test) → 6 (NetArchTest) → 7 (CPM) → 8 (scaffold 45) → 9 (matrix) → 10 (**ilk commit**).
2. `k5000-dll-list §2`: 45 çekirdek = 8 + 29 + 4 + 4 (`K0000-K0849`).
3. `k5000-layer-map.csv`: `K#### → DLL → Owner Agent` eşlemesi (60 satır).
4. `dependency-map §6.1`: `l_i → l_j` izinli ⇔ `j ≤ i` (5000² elle yazılmaz, üretilir).
5. `tools/gen-5000-csproj.ps1 -Phase K0` + `tools/gen-matrix-5000.ps1` + `tools/count-dll.ps1` zaten yazıldı.

---

## §2 Görev — Adım adım (TODOS/06 §1 Blok K0)

| # | Adım | Komut / İş | Kanıt |
|---|---|---|---|
| 4 | `.sln` üret | `dotnet new sln -n VersaCoder` + `dotnet sln add` (45 `.csproj`) | `.sln` diskte + `.slnx` diff = 0 |
| 5 | Test projeleri | `tests/VersaCoder.Tests.Unit` + `tests/VersaCoder.Tests.Architecture` | 2 `.Tests.csproj` |
| 6 | NetArchTest bağla | `LayerRulesTests` = **5000 kural** (dependency-map §6) + 8x8 ölçüm | `dotnet test` 0 ihlal |
| 7 | CPM doğrula | `Directory.Packages.props` iki çözümde geçerli | `dotnet restore` 0 hata |
| 8 | **Scaffold 45** | `tools\gen-5000-csproj.ps1 -Phase K0` | `tools\count-dll.ps1` = **45 + 2** |
| 9 | **Matrix üret** | `tools\gen-matrix-5000.ps1` (5000 satır JSON) + 8x8 doğrulama | ilk 8 satır §2 ile birebir |
| 10 | **İlk commit** | dosya bazlı `git add` (**`git add -A` YASAK**) | `git log` 1 satır |

**Kabul kriterleri (TODOS/06 §2):**

- [ ] `.sln` + `.slnx` senkron (0 fark) · 45 çekirdek `.csproj` + 2 test · 5000 kural üretilmiş · NetArchTest 0 ihlal · restore 0 hata · 1 commit
- [ ] `count-dll.ps1` = 45 uretim + 2 test = 47 (marj −4955, **FAIL beklenir** — K0 sadece 45)
- [ ] `gen-matrix-5000.ps1` = 5000 satır JSON
- [ ] `git status` temiz (onaysız `.cs` yok)

---

## §3 Kanıt (3 tane zorunlu)

```text
KANIT 1: dotnet restore -> 0 hata (VC-LAY-014)
KANIT 2: dotnet build -f net48 + -f net10.0-windows -> 0 hata
KANIT 3: dotnet test -> NetArchTest LayerRulesTests 0 ihlal + count-dll = 47
```

**Çıktı kutusu:**

```text
+--------------------------------------------------+
| BASLIK : K0 iskelet + 45 csproj + 5000 kural     |
| KAPSAM : VersaCoder.sln, src/**, tests/**, tools/|
| KARAR  : dosya bazli git add (git add -A YASAK)  |
| RISK   : count-dll marj -4955 (FAIL) - N1'de      |
|          kapanir                                 |
| KANIT  : restore=0 + build x2=0 + NetArchTest=0  |
+--------------------------------------------------+
```

---

## §4 Ajan Ataması

| # | Ajan | İş |
|---|---|---|
| 1 | `architect` | `.sln` kararı + katman 13 soru |
| 2 | `developer` | İskelet + scaffold 45 |
| 3 | `tester` | Test + NetArchTest |
| 4 | `general` | `git add` dosya bazlı (matris dışı) |

---

**Olculen kanit:** `.ai/prompts/scaffold-k0.md` kaydedildi; `prompts/` = 42 dosya.
