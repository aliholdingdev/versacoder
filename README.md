---
title: README - VersaCoder depo tanitimi
type: overview
category: root
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Repo koku; bu depoda ne var, teknoloji kararlari hangi kaynakta, ilk adim hangi dosya"
---

# VersaCoder

> **Tek cumlelik sorumluluk:** bu depoda **ne oldugunu**, teknoloji secimlerinin
> **kaynagini** ve ilk adim **okuma sirasini** tek yerde soyler.

## §0 Ozet (Answer First)

- **Ne:** VersaCoder - .NET masaustunde calisan **agentic IDE** (WinForms + DevExpress):
  gelistirici tek pencerede kod yazar, ajanin cagrisini canli gorur, onaysiz yikici
  islem yapilmaz. Urun tanimi: [.ai/VISION.md](.ai/VISION.md); kapsam: [.ai/PROJECTS.md](.ai/PROJECTS.md).
- **Bu depoda su an:** 4 kok dokuman (`CLAUDE`, `AGENTS`, `README`, `WORKFLOW`) +
  bilgi bankasi `.ai/` (vault: kural, ADR, sablon, spec, bilgi, rapor, prompt, olcum scriptleri).
- **Ilk adim:** [AGENTS.md](AGENTS.md) §1 okuma sirasi. Bu dosya tanitimdir, kural degildir
  (kural: [.ai/CLAUDE.md](.ai/CLAUDE.md)).

## §1 Depo icerigi (olculmus: 2026-10-04)

| Yol | Ne | Not |
|---|---|---|
| `CLAUDE.md` | kisa kural yonlendirmesi (5 kirmizi cizgi) | kural metni `.ai/CLAUDE.md`'de |
| `AGENTS.md` | oturum boot: okuma sirasi, ajan matrisi, cikti kutusu, kapi kurali | otomatik yuklenen tek talimat |
| `README.md` | bu dosya | tanitim |
| `WORKFLOW.md` | kapi zinciri ozeti | tam metin `.ai/WORKFLOW.md` |
| `.ai/` | vault (bilgi bankasi) | giris: [.ai/index.md](.ai/index.md); 16 klasor + 14 kok belge + olcum scriptleri |
| `.opencode/` | komut ve skill dosyalari | `/prompt-maker`, `/gate`, `rules-check` |
| `.gitignore` | git dislama kurallari | - |

## §2 Teknoloji anayasasi (secim + kaynak)

| Karar | Secim | Kaynak |
|---|---|---|
| TFM | `net48` + `net10.0-windows` (ikili bacak) | [ADR-001](.ai/.decisions/accepted/ADR-001-dual-target-net48-net10.md) |
| UI | DevExpress v26.1, `RibbonControlStyle.Office2010` | [ADR-004](.ai/.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md) + [.ai/.rules/devexpress-rules.md](.ai/.rules/devexpress-rules.md) |
| Veri | ADO.NET + Dapper, parametreli sorgu; `EF Core` yasak (G2) | [.ai/.rules/data-access-rules.md](.ai/.rules/data-access-rules.md) |
| Test | xUnit + NSubstitute + NetArchTest (CPM ile kilitli) | [.ai/ROLE.md](.ai/ROLE.md) |

## §3 Nereden baslanir

1. [AGENTS.md](AGENTS.md) - okuma sirasi + ajan dagitimi + cikti kutusu.
2. [WORKFLOW.md](WORKFLOW.md) - kapi zinciri (`GATE-004a -> 003 -> 004 -> 005 -> 006`).
3. [.ai/index.md](.ai/index.md) - vault haritasi + olcum sayaclari (§4).
4. [.ai/log.md](.ai/log.md) - son durum kaydi (hangi kapi, ne bitti).
