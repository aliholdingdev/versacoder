---
title: "Docs Prompt"
type: prompt
category: prompts
status: active
version: 1.0
updated: 2026-10-04
authority: ".ai/AGENTS.md"
scope: "Dosya belgelemesi icin yeni-belge-sozlesmesi ve sablon promptu"
---

# Docs Prompt

**Zorunlu Baglantilar:** [index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [new-doc-contract](../.templates/new-doc-contract.md) | [docs-md-template](../.templates/documentation/docs-md-template.md)

> **Tek cumlelik sorumluluk:** Yeni bir .md dosyasini sozlesmeye uygun yaz: frontmatter 9 alan, H1, Zorunlu Baglantilar, Tek cumlelik sorumluluk, §0 Ozet, §9 Kanit.

## §0 Ozet (Answer First)

Docs promptu her yeni Markdown dosyasini **[new-doc-contract](../.templates/new-doc-contract.md)** sozlesmesine gore uretir: ilk satir `---`, **9 alanli YAML frontmatter**, H1, `**Zorunlu Baglantilar:**`, `> **Tek cumlelik sorumluluk:**`, `## §0 Ozet (Answer First)` ve **son bolum `## §9 Kanit`**. Boyut 3-8 KB, ASCII kutu, emoji yok.

## §1 Girdi

- Dosya amaci (tek cumle)
- [new-doc-contract](../.templates/new-doc-contract.md) — sozlesme
- [docs-md-template](../.templates/documentation/docs-md-template.md) — sablon

## §2 Prompt

```text
Docs uretimi basliyor: <dosya amaci>.
1. new-doc-contract.md'yi oku (9 alan frontmatter + link beyaz listesi).
2. Ilk satir ---; title, type, category, status, version, updated, authority, owner, scope.
3. H1 + Zorunlu Baglantilar + Tek cumlelik sorumluluk + §0 Ozet.
4. Son bolum §9 Kanit; boyut 3-8 KB; ASCII kutu; emoji yok.
Cikti kutusunu da yaz.
```

## §3 Cikti Kutusu

```text
------------------------------------------------------------
KARAR     : DOSYA URETDI | REVIZE
KAPSAM    : <dosya yolu> (tek satir)
SIZES     : <KB> KB (3-8 KB araliginda)
KANIT     : frontmatter 9 alan + §0 + §9 tamam
------------------------------------------------------------
```

## §4 Kabul

- Frontmatter 9 alan eksiksiz; ilk satir `---`
- §0 Ozet var; son bolum §9 Kanit
- Boyut 3-8 KB; linkler beyaz listede

## §5 RED Kosulu

Beyaz listede olmayan link eklendi — SEBEP: kirik link riski; [new-doc-contract](../.templates/new-doc-contract.md) link tablosu disina cikma.

## §6 Yasaklar

Emoji ve mojibake yok. Unicode kutu karakteri yok. Beyaz liste disi link yok. `as`/`bang`/`any` yok. Madde listesi 5 maddeyi asma.

## §7 Sure

Docs: 15 dk/dosya. Cikti kutusu: 1 dk.

## §8 Kardesler

[genel-ozet](genel-ozet-prompt.md) | [sezon-sonu](sezon-sonu-prompt.md) | [gate-004a](../checklists/gate-004a-vault-kapanis.md) | [gate-kapanis](gate-kapanis-prompt.md)

## §9 Kanit

Kanit: 4 satir docs kutusu. Referans: [new-doc-contract](../.templates/new-doc-contract.md), [docs-md-template](../.templates/documentation/docs-md-template.md), [AGENTS.md §2](../AGENTS.md).
