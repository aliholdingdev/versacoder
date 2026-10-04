---
title: "Kontrol Listeleri Dizini - VersaCoder"
type: index
category: checklists
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "checklists/ altindaki tum denetim listelerinin kayit ve navigasyon noktasi - gate oncesi ne kontrol edilir"
---

# Kontrol Listeleri Dizini (`checklists/`)

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [WORKFLOW.md](../WORKFLOW.md) · [test-strategy](../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** *Hangi gate oncesi hangi kontrol listesi zorunladir* sorusunun tek cevabidir.
> Checklist = **evet/hayir** sorulari listesi; aciklama `spec/`'te yasar (SRP).

---

## §0 Ozet (Answer First)

`checklists/` = kapidan gecmeden once tiklanan kutular. Her liste en fazla 30 soru, her soru **kanit bekler** (cikti/dosya/test). "Bos evet" = checklist gecersiz.

---

## §1 Kayit Tablosu

| Dosya | Hangi gate | Kanit |
|---|---|---|
| [gate-004a-vault-kapanis.md](gate-004a-vault-kapanis.md) | GATE-004a | 3 script ciktisi (500 MD / 0 link / 5000 kayit) |
| [gate-003-ui-onay.md](gate-003-ui-onay.md) | GATE-003 | 7 wireframe + kullanici onayi |
| [gate-004-ilk-commit.md](gate-004-ilk-commit.md) | GATE-004 | git log + build ciktisi |
| [gate-005-test-kapsami.md](gate-005-test-kapsami.md) | GATE-005 | `dotnet test` coverage %90 |
| [layer-5000-check.md](layer-5000-check.md) | GATE-005 | sayac PASS + 2 build + NetArchTest 0 ihlal (ADR-030) |

---

## §2 Kural

1. Checklist'i dolduran **farkli** bir rol olabilir (reviewer) - ureten degil.
2. Her sorunun `[ ]` + kanit sutunu vardir; kanitsiz soru **RED**.
3. Yeni checklist acilirken bu tabloya satir eklenir.

---

## §3 Kardesler

[playbooks/](../playbooks/index.md) · [prompts/](../prompts/index.md) · [knowledge/](../knowledge/index.md)
