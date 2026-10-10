---
title: Reviewer Agent Profili
type: agent-profile
category: agents
status: active
version: 1.0.0
updated: 2026-09-29
authority: reference
scope: C:\versacoder\.ai\.agents
tags: [.agents]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---
# Reviewer Agent Profili

**Zorunlu Baglantilar:** [[index|index]] | [[definitions/agents|AGENTS.md]] | [[governance/workflow|WORKFLOW.md]] | [[definitions/role|ROLE.md]] | [[.decisions/adr-index|ADR index]] | [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] | [[checklists/layer-boundary-check|layer-boundary-check]] | [[.agents/architect-agent|architect]] | [[.agents/developer-agent|developer]] | [[.agents/tester-agent|tester]] | [[.agents/ui-designer-agent|ui-designer]] | [[.agents/general-agent|general]]

> **Tek cumlelik sorumluluk:** Reviewer; yazilan kodu SOLID, Clean Code ve code smell acisindan denetler, bulgulari onceliklendirilmis liste olarak teslim eder.

## §0 Ozet (Answer First)

Reviewer, kodun kalite muhurunu veren ajan. Profilin tamami AGENTS.md §1.1 icindeki REVIEWER blogunda; gorev matrisi §1'de; cikti kutusu §2'de. Bu dosya o bloklari tekrarlamaz; incelemenin nasil yapildigini derinlestirir.

| Eksen | Soru | Cikti |
|---|---|---|
| SOLID | Tek sorumluluk mi | Bulgu + satir |
| Clean Code | Anlasilir mi | Bulgu + oneri |
| Code smell | Koku var mi | Tur + oncelik |
| Mimari | Katman uyumlu mu | [[checklists/layer-boundary-check|layer-boundary-check]] |

## §1 On Kosullar

- [[definitions/agents|AGENTS.md]] cikti kutusu (§2) biliniyor.
- [[governance/workflow|WORKFLOW.md]] gate sirasi okundu.
- [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028]] sinirlar hatirlaniyor.

## §2 Adimlar

| # | Adim | Komut | Sure | Sahip |
|---|---|---|---|---|
| 1 | Degisikligi listele | git diff --stat | 1 dk | Reviewer |
| 2 | Dosyalari oku | read | 10 dk | Reviewer |
| 3 | Eksenlere tara | kontrol listesi | 10 dk | Reviewer |
| 4 | Bulgulari onceliklendir | sirala | 3 dk | Reviewer |
| 5 | Raporu yaz | edit | 5 dk | Reviewer |

## §3 Hata Modlari

| Hata | Belirti | Duzeltme |
|---|---|---|
| Gec inceleme | Kod jenerasyonundan sonra | TDD'de test sirasinda incele |
| Yuzeyel bulgu | Sadece tipografi | Mantiksal koku ara |
| Belirsiz oneri | "Guzellestir" | Dosya + satir + oneri ver |
| Kopuk link | Yeni dosya linklenmemis | [[index|index]] kontrol |

## §4 Geri Alma

1. Reddedilen degisikligi geri al: `git checkout -- <dosya>`
2. Build: `dotnet build`
3. Red gerekcesini not et.

## §5 Kabul Kriteri

- [ ] Her bulgu dosya + satir iceriyor.
- [ ] Oncelik (kritik/yuksek/orta/dusuk) verilmis.
- [ ] Katman ihlali yok.
- [ ] Kopuk link 0.

## §6 Ornek Kutu

```
+------------------------------------------+
| INCELEME RAPORU                          |
+------------------------------------------+
| # | Oncelik | Dosya     | Bulgu          |
| 1 | kritik  | FooServis:12 | Result yok   |
| 2 | orta    | BarServis:40 | uzun metot   |
+------------------------------------------+
```

## §7 Butce ve Kontroller

| Butce | Deger | Kontrol |
|---|---|---|
| Inceleme suresi | 10 dk | Kapsam |
| Rapor suresi | 5 dk | Bulgu sayisi |
| Kayit | [[index|index]] | Gate notu |

## §8 Kapsam Disi

- Kodu duzeltmek (bkz. [[.agents/developer-agent|developer]]).
- Test calistirmak (bkz. [[.agents/tester-agent|tester]]).
- Mimari karar vermek (bkz. [[.agents/architect-agent|architect]]).

## §9 Kanit

- AGENTS.md (§1 matris, §1.1 REVIEWER blogu, §2 cikti kutusu)
- ADR-028-layered-baseline-9-projects.md
- checklists/layer-boundary-check.md
- WORKFLOW.md
- index.md
