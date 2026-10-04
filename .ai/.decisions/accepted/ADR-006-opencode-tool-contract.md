---
title: "ADR-006 — opencode Tool Seti → VersaCoder.Protocol Sözleşmeleri"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "opencode araç kümesinin C# tarafındaki tek sözleşme karşılığı: tipler, JSON şeması, sonuç ve hata biçimi"
---

# ADR-006 — opencode Tool Seti → VersaCoder.Protocol Sözleşmeleri

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-013](ADR-013-layered-architecture-52-nodes.md) · [ADR-001](ADR-001-dual-target-net48-net10.md)

> **Tek cümlelik karar:** opencode'un araç kümesi (`read` · `write` · `edit` · `bash` · `grep` · `glob` · `list`) VersaCoder'da **`VersaCoder.Protocol` içindeki tek C# sözleşme ailesi** (`ITool` / `ToolCall` / `ToolResult`) ile temsil edilir, bu sözleşme iki bacakta da aynıdır — karar verildi.

---

## §0 Özet (Answer First)

Bu ADR, opencode'da karşılığını gördüğümüz **7 aracın** C# sözleşme karşılığını kilitler: her araç `ITool` uygular, çağrısı `ToolCall` ile gelir, çıktısı `ToolResult` olarak döner. Araç **hata fırlatmaz**; başarısızlık `ToolResult.Success = false` + `ToolError` olarak **değerdir**, böylece ajan döngüsü bir araç yüzünden çökmez. Şema JSON'dur ve `tool_schema_version` ile sürümü vardır. Çalışma zamanı davranışı (timeout, iptal, izin) bu ADR'nin **kapsamı dışındadır** → [ADR-009](ADR-009-tool-execution-sandbox.md).

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Master prompt PART 7, opencode tool setinin VersaCoder karşılığını istiyor; kayıt satırı defterde mevcut | `adr-index.md` §2 · ADR-006 |
| C2 | Runtime ajan döngüsü `l5 Protocol` altında; `tool-schema` ve `loop-policy` düğümleri tanımlı | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2 (N33, N36) |
| C3 | Aracın girdisi **LLM çıktısıdır**, yani doğrulanmamış sınırdır → sözleşme hatayı değer olarak taşımalı | `AGENTS.md` §8.1 (3-STRIKE) |
| C4 | `net48` + `net10.0-windows` iki bacakta da aynı imza çalışmalı | [ADR-001](ADR-001-dual-target-net48-net10.md) |
| C5 | Vault'ta `spec/tool-spec.md` henüz yok; karar bu ADR'de sabitlenmezse PART 7 iki ayrı sözleşme üretir | `AGENTS.md` §1.3/2 |

---

## §2 Karar

**Karar:** opencode tool seti, `VersaCoder.Protocol` içinde **tek bir sözleşme ailesi** (`ITool` · `ToolCall` · `ToolResult` · `ToolError`) ile modellenir; yedi araç bu sözleşmenin **yalnız ad ve şema** tarafında ayrışır, çağrı ve sonuç tarafı ortaktır — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Sözleşme evi | `VersaCoder.Protocol` (l5) — implementasyon `l4 tool-runner` içinde | C2 |
| D2 | Araç kümesi v1 | `read` · `write` · `edit` · `bash` · `grep` · `glob` · `list` (7 adet) | C1 |
| D3 | Çağrısı | `Task<ToolResult> ExecuteAsync(ToolCall call, CancellationToken ct)` | C3 · C4 |
| D4 | Sonuç biçimi | `ToolResult { Success, Stdout, Stderr, ExitCode, DurationMs, Error }` — hata **değerdir** | C3 |
| D5 | Şema | JSON Schema + `tool_schema_version: 1`; şema uyuşmazlığı = araç **çağrılmaz**, döngüye `ToolError` döner | C1 |
| D6 | Uyumluluk | Kontrakt testleri **her iki bacakta** aynı beklentiyi doğrular | C4 |

**Neden:** Araç sözleşmesi en kolay dağılan şeydir — her katman kendi `ToolResult`'unu tanımlarsa `l5` ile `l4` arasında sürekli çeviri katmanı doğar. Tek sözleşme, `reviewer`'ın denetimini de tek noktaya indirir: "hangi araç hangi sonucu döndürür" sorusunun cevabı tek dosyadır. Hata değer olarak taşınması, `AGENTS.md` §8.1'deki 3-STRIKE döngüsünün **araç arızası ile ajan arızasını ayırmasını** sağlar.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Tek sözleşme → `l5`/`l4` arasında çeviri katmanı yok | `ITool` tek tanım, 7 implementasyon |
| Hata = değer → döngü araç arızasında ölmez | `ToolResult.Success=false` ile test edilir |
| İki bacak aynı kontraktı paylaşıyor | kontrakt testleri `net48` + `net10.0-windows` |
| Şema sürümü ile araç ekleme geriye dönük | `tool_schema_version` alanı |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Şema + kontrakt testleri ek iş (~½ gün/araç) | v1'de 7 araçla sınırlı tutulur (D2) |
| `bash` sözleşmesi güvenliği bu ADR'de çözülmedi | güvenlik ayrı ADR → [ADR-009](ADR-009-tool-execution-sandbox.md) |
| JSON Schema doğrulama için harici paket gerekebilir | **NEEDS_VALIDATION** (aşağıda) |

**NEEDS_VALIDATION:** JSON Schema doğrulama kütüphanesinin seçimi ve iki bacakta restore edilebilirliği **G8 (paket/sürüm onayı)** kapsamındadır; onay alınana kadar şema **yalnız belge seviyesinde** tutulur, paket referansı eklenmez.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | related | l5 `tool-schema` düğümünün içeriğini bu sözleşmeden alır |
| [ADR-009](ADR-009-tool-execution-sandbox.md) | related | çalışma zamanı güvenliği (timeout · iptal · izin) orada |
| [ADR-007](ADR-007-agent-hierarchy-session.md) | related | araç çağrısının hangi oturumda yapıldığı orada |
| [ADR-010](ADR-010-cli-desktop-shared-core.md) | related | sözleşme her iki host'un da paylaştığı çekirdekte |
| [ADR-001](ADR-001-dual-target-net48-net10.md) | related | iki bacak şartı kontrakt testlerini bağlar |

---

## §5 Kabul Kriterleri

- [ ] `VersaCoder.Protocol` altında `ITool` · `ToolCall` · `ToolResult` · `ToolError` tanımlı
- [ ] 7 araç (`read` `write` `edit` `bash` `grep` `glob` `list`) `ITool` uyguluyor
- [ ] Kontrakt testleri **her iki bacakta** yeşil (`dotnet test` çıktısı raporda)
- [ ] Araç kaynaklı hata yakalama testi: `Success=false` → döngü devam eder
- [ ] G8 onayı alınmadan JSON Schema paketi eklenmemiş

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: master prompt PART 7 + `adr-index.md` §2.
