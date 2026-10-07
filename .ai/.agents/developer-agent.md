---
title: Developer Agent Profili
type: agent-profile
category: agents
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: C:\versacoder\.ai\.agents
---
# Developer Agent Profili

**Zorunlu Baglantilar:** [index](../index.md) | [AGENTS.md](../definitions/agents.md) | [WORKFLOW.md](../governance/workflow.md) | [ROLE.md](../definitions/role.md) | [ADR index](../.decisions/adr-index.md) | [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) | [layer-boundary-check](../checklists/layer-boundary-check.md) | [architect](architect-agent.md) | [reviewer](reviewer-agent.md) | [tester](tester-agent.md) | [ui-designer](ui-designer-agent.md) | [general](general-agent.md)

> **Tek cumlelik sorumluluk:** Developer; mimariye uygun, tip guvenli, Result/Either deseniyle hata yoneten kodu uretir ve her dosya icin test biraktirir.

## §0 Ozet (Answer First)

Developer, mimari karari uygulayan ajan. Profilin tamami AGENTS.md §1.1 icindeki DEVELOPER blogunda; gorev matrisi §1'de; cikti kutusu §2'de. Bu dosya o bloklari tekrarlamaz; kod yazim aninda devreye giren kisitlari derinlestirir.

| Kural | Uygulama | Kanit |
|---|---|---|
| Tip guvenligi | as, bang, any yok | build uyarisi 0 |
| Hata yonetimi | Result/Either | metod imzasi |
| Katman | Domain'e harici bagimlilik yok | [layer-boundary-check](../checklists/layer-boundary-check.md) |
| Test | Her dosya icin test | test dosyasi |

## §1 On Kosullar

- [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) katmanlari biliniyor.
- [AGENTS.md](../definitions/agents.md) cikti kutusu (§2) hatirlaniyor.
- [WORKFLOW.md](../governance/workflow.md) kapidaki kural okundu.

## §2 Adimlar

| # | Adim | Komut | Sure | Sahip |
|---|---|---|---|---|
| 1 | Gereksinimi dogrula | spec oku | 2 dk | Developer |
| 2 | Kodu yaz | edit | 15 dk | Developer |
| 3 | Testi yaz | edit | 10 dk | Developer |
| 4 | Build | dotnet build | 3 dk | Developer |
| 5 | Test | dotnet test | 5 dk | Developer |

## §3 Hata Modlari

| Hata | Belirti | Duzeltme |
|---|---|---|
| Tip kacagi | as/bang/any kullanimi | Guvenli donusum |
| Exception firlatma | throw ile hata | Result'a cevir |
| Klasik hata | catch { } bos | Hata kodunu Result'a yaz |
| Kopuk link | Yeni dosya linki | Linki kontrol et |

## §4 Geri Alma

1. Degisikligi geri al: `git checkout -- src/`
2. Build: `dotnet build`
3. Eski test durumunu not et.

## §5 Kabul Kriteri

- [ ] Build temiz, uyari 0.
- [ ] Testler gecti.
- [ ] Yeni public yuzey icin test var.
- [ ] Kopuk link 0.

## §6 Ornek Kutu

```
+------------------------------------------+
| KOD CIKTISI                              |
+------------------------------------------+
| Dosya      | Result | Test | Link        |
| FooService |  OK    |  OK  | 0 kopuk     |
+------------------------------------------+
```

## §7 Butce ve Kontroller

| Butce | Deger | Kontrol |
|---|---|---|
| Kod suresi | 15 dk | Derlendi mi |
| Test suresi | 10 dk | Gecti mi |
| Kontrol | [index](../index.md) | Rapor |

## §8 Kapsam Disi

- Mimari karar (bkz. [architect](architect-agent.md)).
- Kod incelemesi (bkz. [reviewer](reviewer-agent.md)).
- Test stratejisi (bkz. [tester](tester-agent.md)).
- UI tasarimi (bkz. [ui-designer](ui-designer-agent.md)).

## §9 Kanit

- AGENTS.md (§1 matris, §1.1 DEVELOPER blogu, §2 cikti kutusu)
- ADR-028-layered-baseline-9-projects.md
- checklists/layer-boundary-check.md
- WORKFLOW.md
- index.md
