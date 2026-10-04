---
title: WORKFLOW - kapi zinciri ozeti (boot)
type: boot-instruction
category: root
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Repo koku; kapi adlari ve sirasi - kapi kriterleri .ai/WORKFLOW.md'de, durum kaniti .ai/log.md'dedir"
---

# WORKFLOW - Kapi zinciri (ozet)

> **Tek cumlelik sorumluluk:** kapi adlarini ve sirasini tek satirda verir; kapi
> kriterleri, kanit kutusu bicimi ve durum SSOT dosyalardadir - burada tekrarlanmaz.

## §0 Zincir (AGENTS.md §4)

```text
GATE-004a (vault) -> GATE-003 (UI onayi, HARD STOP) -> GATE-004 (iskelet + ilk commit) -> GATE-005 (test) -> GATE-006 (teslim)
```

| Kapi | Ne kapatir | Kanit nerede aranir |
|---|---|---|
| GATE-004a | vault yesil: olcum sayaclari sifirlarda (link/mojibake/frontmatter) | `.ai/scripts/*.ps1` ciktisi |
| GATE-003 | ekran onayi - onaysiz `src/**` kodu yasak (HARD STOP) | kullanici onayi (tarihli, `.ai/log.md`) |
| GATE-004 | iskelet + dosya bazli ilk commit | `dotnet build` + `dotnet test` ciktisi |
| GATE-005 | test + NetArchTest 0 ihlal | `dotnet test` ciktisi |
| GATE-006 | teslim | WORKFLOW §5 raporu |

## §1 Kurallar (tek satir)

- **Kapiyi yalniz kullanici acar**; RED veren kapi isi ureten ajana dondurur (`.ai/AGENTS.md` §1.3).
- **Gunluk durum** (hangi kapi acik/kapali, son onay ne zaman): [.ai/log.md](.ai/log.md) son kayit - burada tahmin yazilmaz.
- **Tam metin** (kapi kriterleri, kanit kutusu `KANIT/DOSYA/EKSIK/SONRAKI`, sureler): [.ai/WORKFLOW.md](.ai/WORKFLOW.md).
- **Kapali soru:** "kapi kriteri tam nedir?" -> `.ai/WORKFLOW.md`; "simdi durum ne?" -> `.ai/log.md`.
