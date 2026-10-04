---
title: "ADR-017 — Parametreli SQL Zorunlu, String-Concat Kod İncelemesinde RED"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "her sorguda parametre zorunluluğu, string-concat SQL'in inceleme sonucu ve otomatik denetim kalıbı"
---

# ADR-017 — Parametreli SQL Zorunlu, String-Concat Kod İncelemesinde RED

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) · [ADR-016](ADR-016-no-ef-core-restated.md)

> **Tek cümlelik karar:** Her SQL sorgusu **parametreli** yazılır (`@p1..@pN`), SQL dizesine değer **birleştirme (string concat) yapılamaz**; bu kalıp kod incelemesinde **`reviewer` için otomatik RED** sebebidir — karar verildi.

---

## §0 Özet (Answer First)

SQL enjeksiyonu, en pahalı güvenlik açığıdır çünkü tek satırlık bir gevşeklikten doğar. Bu ADR üç şeyi bağlar: **(1)** tüm sorgularda parametre zorunlu, **(2)** string-concat tespiti = RED (düzeltmesi `developer`'a gider, pazarlık yok), **(3)** denetim **elle okuma değil**, `CLAUDE.md` G2'deki kod tarama komutuyla **sayısal**dır: `0 satır` beklenir. Ek olarak `IN (...)` grupları ve dinamik sıralama gibi "legit" birleştirme durumları için de tek şablon (`@p1, @p2, ...` üretimi) tanımlanır; elle dize birleştirme **her koşulda** yasaktır.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Veri erişimi ADO.NET + Dapper; parametre `@ad` biçiminde; her komutta `CommandTimeout` | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §karar 2 |
| C2 | Denetim komutu hazır: `'"\s*\+\s*\w*(Id\|Name)'` = **0 satır** | `CLAUDE.md` §denetim G2 |
| C3 | EF Core / ORM referansı da G2 ihlali → 0 satır | `CLAUDE.md` §denetim G2 · [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) |
| C4 | Erişim yalnız `l4` içinde → tarama yüzeyi küçük ve net | [ADR-016](ADR-016-no-ef-core-restated.md) D1 |
| C5 | `reviewer` kanıtsız PASS veremez; bulgu biçimi sabittir | `AGENTS.md` §2.3 |
| C6 | Her iki bacakta da test zorunlu | [ADR-019](ADR-019-dual-tfm-test-matrix.md) |

---

## §2 Karar

**Karar:** Parametreli SQL **zorunlu**, string-concat SQL **kalıcı yasak** ve tespiti `reviewer`'ın **otomatik RED** gerekçesidir; denetim G2 kod taramasıyla **0 satır** ölçülür — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Parametre biçimi | `@p1..@pN` her `DbCommand`/Dapper çağrısında | C1 |
| D2 | Yasak | SQL dizesine değer **+** ile eklenemez; sabit sorgu + parametre | C2 |
| D3 | Denetim | G2 taraması = **0 satır**; `reviewer` bunu **RED** olarak işler | C2 · C5 |
| D4 | Dinamik `IN` / sıralama | parametre listesi **koddan** üretilir (`@p1,@p2,..`); değerler **hiç** birleştirilmez | C2 |
| D5 | `CommandTimeout` | her komutta **açıkça** set edilir | C1 |
| D6 | Hata | SQL istisnası hata **değerine** çevrilir; ham SQL metni log'a **yazılmaz** (parametre değerleri maskeli) | C5 |
| D7 | Kapsam | yalnız `l4` — başka katmanda SQL = [ADR-016](ADR-016-no-ef-core-restated.md) ihlali | C4 |

**Neden:** `+` ile birleştirme her zaman "masum" görünür (id sayıysa güvenli sanılır) ve tam da bu yüzden yıllarca üretimde kalır. **Otomatik RED** kuralı, güvenliği insan yargısından çıkarıp **sayıya** bağlar: tarama `0 satır` döndürmezse inceleme biter, tartışma başlamaz. D4, kaçak kapıyı kapatır: `IN` listesi yasak değildir, **değer birleştirmesi** yasaktır.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Enjeksiyon yüzeyi kaynağında kapanır | G2 taraması = 0 satır |
| Denetim insan bağımsız, sayısaldır | komut çıktısı rapora yapıştırılır |
| Dinamik sorgu ihtiyacı karşılanıyor | D4 şablonu ile `IN`/sıralama serbest |
| İki bacakta aynı güvenlik davranışı | [ADR-019](ADR-019-dual-tfm-test-matrix.md) |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Elle sorgu yazmak yavaş hissettirir | repository şablonu + Dapper çağrı kalıbı |
| D4 üretimi tek yerde değilse tekrarlar | `l4` içinde tek yardımcı (query builder yok, **parametre listesi üretici**) |
| Tarama kalıbı dar olabilir (yalnız `Id\|Name`) | kalıp `reviewer` checklist'inde genişletilir; **RED eşiği değişmez** |
| Maskelenen log, arızayı gizleyebilir | maskeleme **değer** içindir; parametre **adları** logda kalır |

**NEEDS_VALIDATION:** G2 kod tarama kalıbının pratikte yeterli kapsamı sağlayıp sağlamadığı (yalnız `Id|Name` ile sınırlı olması) **PART 5'te** genişletilmiş kalıpla ölçülür; sonuç `reports/`'a yazılır. Kalıp genişleyebilir, **RED kararı (D3) değişmez**.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) | extends | parametre kuralının kökeni ve inceleme karşılığı |
| [ADR-016](ADR-016-no-ef-core-restated.md) | related | SQL'in hangi katmanda yazıldığı sınırı |
| [ADR-012](ADR-012-dual-tfm-data-provider.md) | related | sağlayıcı + `CommandTimeout` davranışı |
| [ADR-019](ADR-019-dual-tfm-test-matrix.md) | related | parametreli sorgu testlerinin koştuğu matris |
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | related | SQL'in yazıldığı `l4 ado-dapper` düğümü |

---

## §5 Kabul Kriterleri

- [ ] `Select-String` G2 kalıbı `src/` üzerinde **0 satır** döndürüyor
- [ ] `EF Core|DbContext|FromSql` taraması = **0 satır**
- [ ] Her `DbCommand` için `CommandTimeout` testi var
- [ ] `IN (...)` listesi parametreleme testi (değerler birleştirilmiyor)
- [ ] `reviewer` checklist'inde "G2 tarama çıktısı: 0" maddesi zorunlu

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: ADR-005 + `CLAUDE.md` G2.
