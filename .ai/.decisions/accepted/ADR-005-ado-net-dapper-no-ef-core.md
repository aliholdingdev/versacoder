---
title: "ADR-005 — Veri erişimi ADO.NET + Dapper; EF Core yok"
type: adr
category: ".decisions"
status: accepted
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "EF Core yasak; parametreli SQL zorunlu; migration = saf SQL + schema_version"
tags: [adr, data, dapper, ef-core-ban, g2]
source: "brain.md §1/§1.1/§1.2 — 2026-10-07 yeniden inşa (orijinal tam metin kayıp; G9: kaynaksız detay yok)"
supersedes: ""
---

## §1 Bağlam

EF Core, `net48` bacağında sürüm kilidi oluşturur; SQL'i görünmez kılar;
migration'ı denetim dışına çıkarır.

## §2 Karar

- Veri erişimi **ADO.NET + Dapper**; ORM yok.
- Her sorgu **parametreli** (`@p1..@pN`); string-concat SQL = otomatik RED (ADR-017).
- Migration = **saf SQL** dosyaları + `schema_version` tablosu; ORM migration yok.
- Her komutta `CommandTimeout` açık set edilir.

## §3 Alternatifler

- **EF Core:** hızlı geliştirme ama sürüm kilidi + görünmeyen SQL. ❌
- **Dapper/ADO.NET (seçilen):** SQL elle yazılır ama denetlenebilir. ✅
- **Diğer ORM'ler (NHibernate vb.):** aynı riskler. ❌

## §4 Sonuçlar

**Ters-senaryo:** EF Core girseydi net48 bacağında sürüm kilidi oluşur, SQL
görünmez olurdu; migration denetlenebilir olmazdı.

**Yakalama:** `data-access-check`, `migration-check` + EF `using` taraması = 0 satır.

## §5 Ölçülebilir çıktı

| Dosya | Çıktı |
|---|---|
| Infrastructure veri kodu | `using Microsoft.EntityFrameworkCore` = 0 |

## §6 Kaynak

- brain.md §1 satır 005 · §1.1 · §1.2
- .ai/CLAUDE.md G2 (EF Core + concat yasağı)

## §7 İlgili

- [ADR-016-dbconnection-only-l4.md](adr-016-dbconnection-only-l4.md) — kapsam genişletmesi
- [ADR-017-parameterized-sql-only.md](adr-017-parameterized-sql-only.md)
- [adr-index](../adr-index.md) · [brain](../brain.md)
