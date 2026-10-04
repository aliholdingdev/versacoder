---
title: "K5000 DLL Listesi - 5000 Fiziksel .dll Katalogu (K bandi eslemesi)"
type: architecture
category: architecture
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "5000 uretim .dll'in (1 yon grubu = 1 fiziksel .dll = 1 .csproj) K bandi karsiligi: K0000..K5499 arasi her bandin hangi cekirdek/mikro/yeni nesil .dll ile doldugu, 5 faz (K0 + N1..N4) ve 12 alanli layer.json kayit semasi"
---

# K5000 DLL Listesi - 5000 Fiziksel `.dll`

**Zorunlu Baglantilar:** [layer-5000 index](index.md) · [yon-gruplari](yon-gruplari.md) · [ADR-030](../../.decisions/accepted/ADR-030-5000-layers-class-level.md) · [layer-rules](../../.rules/layer-rules.md) · [layering-5000-spec](../../spec/layering-5000-spec.md) · [layer-5000-check](../../checklists/layer-5000-check.md) · [k5000-layer-map](../catalog/k5000-layer-map.csv)

> **Tek cumlelik sorumluluk:** *5000 uretim `.dll`'in hangi K bandinda durdugu, hangi
> fazda acildigi ve her birinin `layer.json`'da nasil kayitli oldugu* sorusunun tek cevabi.

---

## §0 Ozet (Answer First)

**Hedef: 5000 uretim `.dll` = 5000 `.csproj` = 5000 K bandi kaydi.** Cekirdek
hedefi **45 cekirdek** (`8+29+4+4`, §2); diskte **9 uretim + 2 test `.csproj`** var
(2026-10-03 olcumu); planlanan `45 cekirdek + 5000 scaffold = 5045 uretim .csproj`
+ `2 test = 5047 toplam .csproj`.

| Olcut | Deger | Kaynak |
|---|---|---|
| Hedef uretim `.dll` | **>= 5000** | ADR-030 §2.1 (v2.1.0) |
| K bandi araligi | **K0000..K5499** (5500 slot; 5000 dolu + 500 bos) | bu dosya §1 |
| Cekirdek DLL (K0000-K0849) | **45** | §2 (proje + mikro + yeni nesil + IDE + plugin + CLI; metadata N1'e ertelendi) |
| Scaffold DLL (K0850-K5499) | **5000** | §3 (fazli; 500 klasor bossuz) |
| Toplam uretim `.csproj` | **5045** | 45 + 5000 |
| Test `.csproj` | **2** (matris satiri degil) | `tests/` |
| TFM | `net48;net10.0-windows` (G6 cift bacak) | ADR-001 |
| Layer bilgisi | **DLL'e gomulmez** -> `layer.json` | bu dosya §4 |

**Kisa okuma:** K bandi **plandir**, `.csproj` **kapidir** — banda yazmadan `.csproj`
acilmaz (LAY-06); `.csproj` acilmadan bandin `planli` durumu `yazildi`'ya donmez
(spec VC-LAY-009).

---

## §1 K bandi semasi (7 bant)

| Band | Aralik | Kapsam | Slot | Faz |
|---|---|---|---|---|
| **Foundation** | `K0000-K0999` | Cekirdek l0..l7 + mikro 29 + yeni nesil 4 + IDE/plugin/CLI 4 + metadata 1 + genisleme | 1000 | K0 (44) + N1 (150) |
| **AI Agent** | `K1000-K1999` | AI.Engine, Agent.Runtime, Memory, Vault + genisleme | 1000 | N1 (150) + N2 (500) + N3 (350) |
| **Code Intelligence** | `K2000-K2999` | Editor, Infrastructure.CodeAnalysis, Infrastructure.Refactoring + genisleme | 1000 | N1 (50) + N2 (300) + N3 (650) |
| **IDE Experience** | `K3000-K3999` | ProjectSystem, Infrastructure.MCP, Infrastructure.Git + genisleme | 1000 | N1 (100) + N2 (200) + N3 (500) + N4 (200) |
| **Plugin Extension** | `K4000-K4499` | PluginSystem, Infrastructure.Plugins + genisleme | 500 | N3 (300) + N4 (200) |
| **Enterprise** | `K4500-K4999` | Protocol, Host, CLI, Infrastructure.Deployment + genisleme | 500 | N3 (200) + N4 (300) |
| **Future Expansion** | `K5000-K5499` | K5000.Metadata + genisleme (K5000+ kurali) | 500 | N4 (500) |
| **TOPLAM** | `K0000-K5499` | — | **5500** | 5000 dolu + 500 bos |

> **Bos slot (500):** `K5500+` acildiktan sonra banta eklenir; ADR-030 §2.1 tek
> esiktir (`>= 5000`), bant siniri degildir.

---

## §2 Cekirdek 44 DLL (K0000-K0849) — proje + mikro + yeni nesil + IDE + plugin + CLI + metadata

### §2.1 Cekirdek 8 (l0..l7, ADR-028)

| K bandi | DLL (klasor) | Katman | Rol | TFM |
|---|---|---|---|---|
| `K0000-K0049` | `VersaCoder.Abstractions` | l0 | portlar/arayuzler (IAgentProvider, ILLMProvider, IVaultReader, IEditorService) | cift |
| `K0050-K0099` | `VersaCoder.Domain` | l1 | varliklar, DDD | cift |
| `K0100-K0149` | `VersaCoder.Application` | l2 | use-case, handler, DTO | cift |
| `K0150-K0199` | `VersaCoder.Crosscutting` | l3 | log/cache/clock/config (notr) | cift |
| `K0200-K0249` | `VersaCoder.Infrastructure` | l4 | semsiye -> 29 pakete bolunur | cift |
| `K0250-K0299` | `VersaCoder.Protocol` | l5 | tool/agent sozlesmesi, JSON-RPC | cift |
| `K0300-K0349` | `VersaCoder.Host` | l6 | DI, bootstrap, composition root | cift |
| `K0350-K0399` | `VersaCoder.Desktop` | l7 | WinForms Exe (DevExpress, Ribbon 2010) | cift |

### §2.2 Infrastructure mikro 29 (l4 icinde, K0200-K0249 bandinda doldurulur)

`K0200` = Data · `K0201` = Config · `K0202` = Logging · `K0203` = Security ·
`K0204` = FileSystem · `K0205` = AI · `K0206` = Auth · `K0207` = Caching ·
`K0208` = Context · `K0209` = Messaging · `K0210` = Network · `K0211` = Observability ·
`K0212` = Git · `K0213` = MCP · `K0214` = Plugins · `K0215` = Services ·
`K0216` = Integration · `K0217` = Backup · `K0218` = Versioning ·
`K0219` = CodeAnalysis · `K0220` = ProjectAnalysis · `K0221` = Refactoring ·
`K0222` = Testing · `K0223` = Templating · `K0224` = Documentation ·
`K0225` = Reporting · `K0226` = Diagram · `K0227` = Deployment · `K0228` = Learning

> **29 paket** = `versacoder.old.1` (36) + `versacoder.old.2` (34) +
> `versacoder.old.old old` (36) union'inin cekirdek-disi kismi (ek-1 §0).

### §2.3 Yeni Nesil AI/Agent 4 (K0400-K0599)

| K bandi | DLL | Yeni nesil rol | K5000 bandi |
|---|---|---|---|
| `K0400-K0449` | `VersaCoder.AI.Engine` | Master Orchestrator, IAgentProvider adapter (OpenAI/Claude/Ollama) | K1000-K1999 |
| `K0450-K0499` | `VersaCoder.Agent.Runtime` | 8 agent rolu (Architect/Developer/UI/Security/QA/Doc/DevOps/Research) | K1000-K1999 |
| `K0500-K0549` | `VersaCoder.Memory` | Episode, vector, retention | K1000-K1999 |
| `K0550-K0599` | `VersaCoder.Vault` | Reader/Writer/Search/Loader | K0000-K0999 |

### §2.4 IDE + Plugin + CLI + Metadata 4 (K0600-K0849)

| K bandi | DLL | Rol | K5000 bandi |
|---|---|---|---|
| `K0600-K0649` | `VersaCoder.Editor` | Roslyn syntax, completion, refactoring, diagnostics | K2000-K2999 |
| `K0650-K0699` | `VersaCoder.PluginSystem` | manifest, loader, isolation | K4000-K4499 |
| `K0700-K0749` | `VersaCoder.ProjectSystem` | .sln/.slnx, MSBuild, references | K3000-K3999 |
| `K0750-K0799` | `VersaCoder.Cli` | Exe, commands, parsing, output | K4500-K4999 |
| `K0800-K0849` | `VersaCoder.K5000.Metadata` | layer.json okuma/yazma, 12 alan schema - **N1'e ertelendi (K0 disi)** | K5000-K5499 |

**Cekirdek toplam:** 8 + 29 + 4 + 4 = **45** (29'luk K0200-K0249 bandinda doldurulur;
`K0000..K0849` = 850 slot, 45 dolu + 805 bos -> N1'de doldurulur).

> **Duzeltme (2026-10-02):** onceki taslakta "cekirdek 44" yazilmisti; olcum 45
> (`8 + 29 + 4 + 4`). `K0000..K0849` 850 slot icinde 45 dolu, **805 bos** -> N1 fazinda
> doldurulur. Bu dosya 45'i esas alir.

---

## §3 Scaffold 5000 DLL (K0850-K5499) — 5 faz

| Faz | Yeni | Toplam | K bandi araligi | Kapı | Kanıt |
|---|---|---|---|---|---|
| **K0** | 45 (cekirdek) | **45** | `K0000-K0849` | GATE-004 | restore 0 hata · build 0 hata |
| **N1** | +455 | **500** | `K0850-K1299` | GATE-005 | build 0 · NetArchTest RED=0 |
| **N2** | +1000 | **1500** | `K1300-K2299` | GATE-005 | build 0 |
| **N3** | +1500 | **3000** | `K2300-K3799` | GATE-005 | build 0 · matris 3000² |
| **N4** | +2000 | **5000** | `K3800-K5499` | GATE-006 | build 0 · matris 5000² |

### §3.1 N1 (+455 = 500)

```text
K0850-K0999  Foundation genisleme        (150)  bos scaffold
K1000-K1149  AI Agent genisleme          (150)  bos scaffold
K1150-K1199  Code Intelligence           (50)   bos scaffold
K1200-K1299  IDE Experience              (100)  bos scaffold
```

### §3.2 N2 (+1000 = 1500)

```text
K1300-K1799  AI Agent devam              (500)
K1800-K2099  Code Intelligence devam     (300)
K2100-K2299  IDE Experience devam        (200)
```

### §3.3 N3 (+1500 = 3000)

```text
K2300-K2799  Code Intelligence devam     (500)
K2800-K3299  IDE Experience devam        (500)
K3300-K3599  Plugin Extension            (300)
K3600-K3799  Enterprise                  (200)
```

### §3.4 N4 (+2000 = 5000)

```text
K3800-K4299  IDE Experience devam        (500)
K4300-K4599  Plugin Extension devam      (300)
K4600-K4999  Enterprise devam            (400)
K5000-K5499  Future Expansion            (500)
```

**N4 toplam:** 500 + 300 + 400 + 500 = **1700** (+ 44 onceki = ... ) -> asagidaki
§3.5 kontrolu ile dogrulanir.

### §3.5 Sayi kontrolu (G5 - olcusuz iddia yok)

| Faz | Yeni | Toplam | Dogrulama |
|---|---|---|---|
| K0 | 45 | 45 | `src/**/*.csproj` = 45 + `tests/` 2 = 47 |
| N1 | +455 | 500 | 500 uretim `.csproj` |
| N2 | +1000 | 1500 | 1500 |
| N3 | +1500 | 3000 | 3000 |
| N4 | +2000 | **5000** | **5000 uretim `.csproj`** = K bandi dolu 5000 |

**N4 ic dagilimi (2000):** K3800-K4299 (500) + K4300-K4599 (300) + K4600-K4999 (400)
+ K5000-K5499 (500) = **1700** -> **+300 eksik (K0850-K0999'ya yazildi).** K0 sonrasi
`K0000..K5499` = 5500 slot; dolu 5000, **bos 500** (`K5500+` icin acik).

> **DOGRULAMA GEREKLI:** N4 ic dagilimi 1700 + 300 (Foundation kalan) = 2000; 300'lük
> kisim K0850-K0999 bandinda zaten N1'de sayildi -> **iki kez sayim riski (G9)**.
> `tools/count-dll.ps1` calistirilip N4 sonu `5000` olmali; olmazsa bu satir duzeltilir.

---

## §4 `layer.json` semasi (12 alan - Layer Creation Rule)

Her `<Band>.K<NNNN>/` klasorunde 1 `layer.json` (1 kayit). **DLL'e gomulmez.**

```json
{
  "layerId": "K0000",
  "name": "Foundation Layer 0",
  "purpose": "Bos scaffold - genisleme icin ayrildi",
  "responsibility": "Tanimlanacak",
  "ownerAgent": "architect",
  "input": "Tanimlanacak",
  "output": "Tanimlanacak",
  "dependencies": ["VersaCoder.Abstractions"],
  "apiContract": "Yok",
  "securityRules": ["G2: ADO.NET + Dapper", "EF Core YASAK"],
  "testingRules": ["GATE-005"],
  "documentation": ".ai/architecture/layer-5000/k5000-dll-list.md",
  "status": "planli",
  "version": "1.0.0"
}
```

**12 alan zorunlu** (Layer Creation Rule): `layerId` · `name` · `purpose` ·
`responsibility` · `ownerAgent` · `input` · `output` · `dependencies` ·
`apiContract` · `securityRules` · `testingRules` · `documentation`.
Ek alanlar: `status` (`planli|yazildi|test-edildi`) · `version`.

**Denetim:** `tools/gen-layer-json.ps1` uretir; `layer.json` eksik alan = LAY-06 ihlali.

---

## §5 K5000 klasor yapisi (kod YOK, plan)

```text
C:\versacoder
+-- src/
|   +-- VersaCoder.Abstractions/            (cekirdek, K0000-K0049)
|   +-- VersaCoder.Infrastructure/
|   |   +-- Data/  Config/  ...  Learning/  (29 mikro, K0200-K0228)
|   +-- VersaCoder.AI.Engine/               (yeni nesil, K0400-K0449)
|   +-- Foundation.K0850/ ... K5499/         (scaffold, 5000 klasor)
|   +-- ...
+-- tests/
|   +-- VersaCoder.Tests.Unit/
|   +-- VersaCoder.Tests.Architecture/
+-- layer.json                              (5000 kayit - TEK KAYNAK)
+-- VersaCoder.sln  +  VersaCoder.slnx      (senkron, VC-LAY-013)
+-- tools/
    +-- gen-5000-csproj.ps1                 (scaffold uretici)
    +-- gen-layer-json.ps1                  (layer.json uretici)
    +-- count-dll.ps1                       (.csproj sayaci)
```

---

## §6 Kabul kriterleri (her madde kanitli)

- [ ] K bandi 7 bant, aralik `K0000..K5499`, toplam slot 5500 (§1).
- [ ] Cekirdek 45 DLL (8 + 29 + 4 + 4), `K0000..K0849` icinde 45 dolu (§2).
- [ ] Scaffold 5000 DLL, 5 faz (K0 + N1..N4), toplam 5000 uretim `.csproj` (§3).
- [ ] N4 sonu `tools/count-dll.ps1` = 5045 uretim + 2 test = 5047 (§3.5, **olculmeli**).
- [ ] Her scaffold klasorunde 1 `layer.json` = 12 alan dolu (§4).
- [ ] `layer.json` sayisi = `K0000..K5499` dolu slot sayisi (§4).
- [ ] `check-links.ps1` = 0 bozuk link.
- [ ] `count-components.ps1` = tip kaydi `>= 5000` PASS (bugun 5440).
- [ ] `gen-layer-5000.ps1` = namespace parca `>= 5000` PASS (bugun 5834).

---

## §7 Acik konular (DOGRULAMA GEREKLI - gizlenmez)

| # | Acik konu | Neden acik | Kapanis kosulu |
|---|---|---|---|
| 1 | N4 ic dagilimi 1700 + 300 = 2000 | 300'lük kisim N1'de sayildi (§3.5) | `count-dll.ps1` = 5000 |
| 2 | `dotnet restore` 5000 `.csproj` sagligi | hic olculmedi (ADR-030 K6) | restore 0 hata |
| 3 | Matris 5000x5000 = 25.000.000 hucre | kural uretim yolu yok (ADR-030 §2.3) | JSON kural seti + NetArchTest |
| 4 | `VersaCoder.Infrastructure` semsiyesi 29'a bolunecek mi | K1'de karar verilir | bosaltma + G7 onayi |
| 5 | K0'da diskte `src/` = **9 uretim + 2 test `.csproj`** (2026-10-03 olcumu), hedef 45 | aaaaa.md "9 proje var" iddiasi bayat kaydilmisti (MEMORY L-06); 2026-10-03 olcumuyla karsilasti | K0'da 45 uretim `.csproj` + 2 test acilir |

---

**Olcum tarihi:** 2026-10-02 · **Sahip:** architect · **Guncelleyen:** architect
(bu dosya `yon-gruplari.md` + ADR-030 §2.1 ile senkrondur; celisirse bu dosya kazanir,
sonra ADR guncellenir - SSOT).
