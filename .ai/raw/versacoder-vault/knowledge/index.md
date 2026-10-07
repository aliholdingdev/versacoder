---
title: "Bilgi Bankasi Dizini - VersaCoder"
type: index
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "knowledge/ altindaki 4 alt konu (devexpress, winforms, data, csharp) icin kayit ve navigasyon noktasi"
---

# Bilgi Bankasi Dizini (`knowledge/`)

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [devexpress-rules](../.rules/devexpress-rules.md) · [data-access-rules](../.rules/data-access-rules.md) · [ADR-004](../.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md)

> **Tek cumlelik sorumluluk:** *Kod yazmadan once hangi teknik bilgi nerede okunur* sorusunun tek cevabidir.
> Burada **karar yok**, bilgi var; karar `.decisions/`'te, kural `.rules/`'tadır (SRP).

---

## §0 Ozet (Answer First)

`knowledge/` = uygulama yazilirken bakilan **teknik bilgi bankasi**. 4 alt klasor: DevExpress v26.1 rehberi, WinForms kalıplari, veri erisimi (ADO.NET + Dapper), C# dili/dil ozellikleri. Her dosya tek bir konuyu 3-8 KB'da anlatir.

---

## §1 Alt Klasorler

| Klasor | Kapsam | Surum/TFM notu |
|---|---|---|
| [devexpress/](devexpress/index.md) | Ribbon, Grid, TreeList, Docking, Editors, Wizard - **v26.1** | `net48` + `net10.0-windows` |
| [winforms/](winforms/index.md) | Form yasam dongusu, designer + resx, DI, async, layout | her iki bacak |
| [data/](data/index.md) | ADO.NET + Dapper, parameterized SQL, migration, provider | SqlClient 5.2.x (ADR-012) |
| [csharp/](csharp/index.md) | Dil ozellikleri, nullable, async, generics, hata yonetimi | C# 7.3 (net48) vs C# 14 |

---

## §2 Kullanim SIRASI

1. [index.md](../index.md) → 2. ilgili alt klasor index'i → 3. tek dosya.
2. Bilgi bankasi **karar degistirmez**: surum/dil kararlari [.decisions/](../.decisions/index.md)'te.
3. Her dosya `new-doc-contract.md` kurallarina uyar.

---

## §3 Kardesler

[checklists/](../checklists/index.md) · [prompts/](../prompts/index.md) · [playbooks/](../playbooks/index.md) · [spec/](../spec/index.md) · [.rules/](../.rules/index.md)
