---
title: ".decisions — ADR Defteri (Karar Kaydı)"
type: index
category: decisions
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Tüm ADR'lerin tek kayıt defteri: durum, konu, hedef bacak, ilgili kural ve gate"
---

# `.decisions/` — ADR Defteri

**Zorunlu bağlantılar:** [[../index.md]] · [[../CLAUDE.md]] · [[../WORKFLOW.md]] · [[../reports/00-kapsam-analizi.md]] · [[../reports/01-web-kaynaklari.md]]

> **Tek cümlelik sorumluluk:** *Hangi karar ne zaman, hangi gerekçeyle, hangi alternatifler arasından seçildi* sorusunun tek cevabıdır.
> Çelişirse bu defter kazanır; kurala dönüşen karar ilgili `.rules/` dosyasında **zorunlu** hale getirilir.

---

## §0 Özet (Answer First)

**7 ADR kabul edildi (ADR-001..005, ADR-012, ADR-013), 6 ADR taslak/planli (ADR-006..011).** Kritik dortlu: iki bacak (`net48`+`net10.0-windows`), elle yazim (COPY=NO), DevExpress v26.1 + Office2010, ADO.NET+Dapper. ADR-013 = 52 dugum/5000+ bilesen katalogu. `accepted` altindaki her dosya `status: accepted` + kanit + >=2 alternatif icerir.

---

## §1 Kabul edilmiş ADR'ler (`accepted/`)

| ADR | Konu | Karar özeti | Durum | İlgili |
|---|---|---|---|---|
| [ADR-001](accepted/ADR-001-dual-target-net48-net10.md) | dual-target | Tek projede **iki bacak**: `net48` (legacy kanıt) + `net10.0-windows` (modern); ortak kaynak, `#if` yalnız platform farkı | accepted | GATE-004 · GATE-005 |
| [ADR-002](accepted/ADR-002-handwritten-content-no-copypaste.md) | handwritten-content | Yedek içerik **elle sıfırdan** yeniden yazılır; kopyala-yapıştır YOK (CoreMusic = yapi referansı) | accepted | G1 · GATE-004a |
| [ADR-003](accepted/ADR-003-vault-structure-500md-frontmatter.md) | vault-structure | ≥500 MD · 8 alanlı frontmatter · kök 30–88 KB · **bozuk link = 0** · noktalı dizinler | accepted | GATE-004a |
| [ADR-004](accepted/ADR-004-devexpress-universal-v261-office2010.md) | devexpress-v261 | **v26.1 tek sürüm** + `RibbonControlStyle.Office2010` tek stil; iki bacak web ile doğrulandı | accepted | G3 · GATE-004 |
| [ADR-005](accepted/ADR-005-ado-net-dapper-no-ef-core.md) | ado-net-dapper | **EF Core YOK**; ADO.NET + Dapper; parameterized + timeout; SqlClient **5.x** (iki bacak); migration = saf SQL + `schema_version` | accepted | G2 · GATE-005 |
| [ADR-012](accepted/ADR-012-dual-tfm-data-provider.md) | dual-tfm-data-provider | SqlClient **5.x iki bacakta ayni**; Sqlite yalniz `net10.0-windows` test (D4) + `NEEDS_VALIDATION` (PART 5 teyidi) | accepted | ADR-001 · ADR-005 · GATE-004 |
| [ADR-013](accepted/ADR-013-layered-architecture-52-nodes.md) | layered-architecture-52-nodes | **8 makro (l0..l7) + 44 alt = 52 dugum** · **>=5000 bilesen kaydi** · tek yon bagimlilik; `7 makro` formulu sapmasi §2 C3'te gerekceli | accepted | ADR-001 · ADR-005 · GATE-004a |

---

## §2 Taslak / planlı ADR'ler

| ADR | Konu | Beklenen karar | Aşama | Sahip |
|---|---|---|---|---|
| ADR-006 | tool-system | opencode tool seti → C# sözleşmesi (read/write/edit/bash/grep/glob/list) | PART 7 | architect |
| ADR-007 | session-model | ana ajan + alt ajan oturum mimarisi, context bütçesi | PART 7 | architect |
| ADR-008 | context-assembly | bağlam toplama stratejisi (≤8 KB subagent paketi) | PART 7 | architect |
| ADR-009 | mcp-integration | MCP server/istemci kapsamı (spec seviyesi) | PART 7 | architect |
| ADR-010 | skill-memory | skill + memory depolama modeli | PART 7 | architect |
| ADR-011 | llm-provider | `ILLMProvider` tek contract + 2 sağlayıcı; ilk doğrulama **OpenRouter `/free`**, sonra Ollama | PART 7 | architect |

> **NOT:** Taslak ADR'ler `accepted/` altına taşınmadan `status: draft` ile tutulur; her biri ≥2 alternatif + sonuç (+/−) ister (AGENTS.md §9.1).

---

## §3 Yazım şablonu

- Şablonlar: [[../.templates/adr/adr-nygard-template.md]] · [[../.templates/adr/adr-architecture-template.md]] · [[../.templates/adr/adr-data-template.md]] · [[../.templates/adr/adr-security-template.md]] · [[../.templates/adr/adr-devexpress-template.md]]
- Dosya adı kalıbı: `ADR-NNN-<slug-kucuk-harf-tire>.md`
- Zorunlu alanlar: Context · Decision · Alternatif (≥2, +/− ile) · Consequences (+/−) · Kanıt (web URL + tarih ya da build/test çıktısı) · Kabul kriterleri

---

## §4 Bağlantı haritası

```
karar ortaya cikar
      |
      v
.architect  ->  ADR-NNN draft  ->  web/yerel kanit topla
      |                                  |
      v                                  v
alternatif >= 2  ----->  karar +/-  ----->  status: accepted
      |                                          |
      v                                          v
.rules/ dosyasinda zorunlu kural        kod + test (developer/tester)
      |
      v
GATE-004 / GATE-005 kaniti  ->  log.md kaydi  ->  reports/
```

---

## §5 Doğrulama

- [x] Frontmatter 8 alan, `authority: SSOT`
- [x] `accepted/` altındaki her ADR: durum + alternatif ≥2 + sonuç (+/−) + kriter
- [x] ADR-004/005 web kanıtlı (URL + erişim tarihi)
- [x] ADR-012 kabul edildi (PART 5'te yalniz D4 / `NEEDS_VALIDATION` teyidi kaldi)
- [x] ADR-013 kabul edildi; katalog uretimi suruyor (PART 3)
