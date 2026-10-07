---
title: "Oyun Kitaplari Dizini - VersaCoder"
type: index
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "playbooks/ altindaki tekrar eden is akisi rehberlerinin kayit ve navigasyon noktasi - adim adim nasil yapilir"
---

# Oyun Kitaplari Dizini (`playbooks/`)

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [WORKFLOW.md](../WORKFLOW.md) · [coding-standards](../.rules/coding-standards.md)

> **Tek cumlelik sorumluluk:** *Bu is daha once yapildiysa hangi adimlarin sirasi* sorusunun tek cevabidir.
> Playbook = **nasil yapilir**; neden oyle oldugunu [spec/](../spec/index.md) aciklar (SRP).

---

## §0 Ozet (Answer First)

`playbooks/` = tekrar eden islerin adim adim rehberi. Her playbook: on-kosul → adimlar (maks 5) → kanit → geri alma. Yeni katman, yeni ekran, yeni migration icin ayri kitap.

---

## §1 Kayit Tablosu

| Dosya | Is | Kanit |
|---|---|---|
| [yeni-katman-ekleme.md](yeni-katman-ekleme.md) | l0..l7'ye katman/dugum ekleme | build + catalog script |
| [yeni-form-tasarlama.md](yeni-form-tasarlama.md) | Designer.cs + .resx ile form | designer acilir, build yesil |
| [migration-ekleme.md](migration-ekleme.md) | Saf SQL migration + version | script + rollback testi |
| [adr-acma.md](adr-acma.md) | Yeni ADR acma + defter guncelleme | adr-index satiri |

---

## §2 Kural

1. Adimlar **sirali** ve **olculur**; "biraz yap" yasak, "≈15 dk" gibi somut sure.
2. Geri alma adimi her playbookta zorunlu.
3. Yeni playbook bu tabloya satir + `index.md`'ye kayit.

---

## §3 Kardesler

[checklists/](../checklists/index.md) · [prompts/](../prompts/index.md) · [knowledge/](../knowledge/index.md)
