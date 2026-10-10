---
title: "adr-index — ADR Durum Defteri"
type: index
category: ".decisions"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "ADR-001..032 durum defteri: her ADR'nin status, tarih, supersedes ve kaynak satırı — .decisions/ klasörünün tek indeksi"
tags: [adr, index, decisions, ledger]
source: "brain.md §1 özeti + 2026-10-07 yeniden inşa (Faz 1, D39)"
supersedes: ""
---

# adr-index — ADR Durum Defteri

> **Tek cümlelik sorumluluk:** `.decisions/` altındaki tüm ADR'lerin durumunu,
> supersede zincirini ve kaynak satırını tek tabloda tutar; ADR dosyaları bu
> defteri **tamamlar**, bu defter ADR dosyalarını **özetlemez** (G12/SRP).

## §0 Özet (Answer First)

- **ADR-001..030 + ADR-032 = `accepted/` altında** (30 dosya; ADR-020 status:
  superseded). **ADR-031 = `draft/` altında** (visual-production-provider,
  status: proposed — mevcut dosya korunmuştur).
- **Yeniden inşa notu (G9):** ADR-001..030 orijinal tam metinleri diskte yoktu;
  2026-10-07'de `brain.md` §1/§1.1/§1.2 özetlerinden + vault atıflarından
  yeniden yazıldı. Her dosyanın `source` alanında provenance taşır.
- **ADR-032 = `.ai` Enterprise Control Plane** — bu vault'un hedef mimarisinin
  tek kaydı (D54). Status: `proposed` (kullanıcı onayı bekliyor).

## §1 Durum Tablosu

| ADR | Dosya | Karar (tek satır) | Status | Supersedes / Superseded by |
|-----|-------|-------------------|--------|------------------------------|
| 001 | [[.decisions/accepted/adr-001-dual-target-net48-net10|ADR-001-dual-target-net48-net10]] | Çift TFM net48 + net10.0-windows; #if yalnız platform farkı | accepted | — |
| 002 | [[.decisions/accepted/adr-002-vault-ownership-no-copy|ADR-002-vault-ownership-no-copy]] | Vault içeriği elle sıfırdan yazılır; kopya yasak | accepted | — |
| 003 | [[.decisions/accepted/adr-003-vault-structure-500md-frontmatter|ADR-003-vault-structure-500md-frontmatter]] | ≥500 MD · 8 alan frontmatter · bozuk link = 0 | accepted | — |
| 004 | [[.decisions/accepted/adr-004-devexpress-universal-v261-office2010|ADR-004-devexpress-universal-v261-office2010]] | DevExpress v26.1 + ribbon Office2010 | accepted | — |
| 005 | [[.decisions/accepted/adr-005-ado-net-dapper-no-ef-core|ADR-005-ado-net-dapper-no-ef-core]] | ADO.NET + Dapper; EF Core yok; parametreli SQL | accepted | — |
| 006 | [[.decisions/accepted/adr-006-opencode-tool-contract|ADR-006-opencode-tool-contract]] | 7 araç = VersaCoder.Protocol sözleşmesi | accepted | — |
| 007 | [[.decisions/accepted/adr-007-agent-hierarchy-session|ADR-007-agent-hierarchy-session]] | 1 ana oturum + alt oturum; derinlik 2; yetki devretmez | accepted | — |
| 008 | [[.decisions/accepted/adr-008-transcript-compaction|ADR-008-transcript-compaction]] | 8 MB aşım → compaction; özgün metin arşive silinmez | accepted | — |
| 009 | [[.decisions/accepted/adr-009-tool-execution-sandbox|ADR-009-tool-execution-sandbox]] | Timeout 60 sn (bash 300) + iptal token + sabit çalışma kökü | accepted | — |
| 010 | [[.decisions/accepted/adr-010-cli-desktop-shared-core|ADR-010-cli-desktop-shared-core]] | Desktop + CLI l0..l6 çekirdeği paylaşır; ayrışma l7 | accepted | — |
| 011 | [[.decisions/accepted/adr-011-llm-provider-order|ADR-011-llm-provider-order]] | openrouter-free → ollama → openai-compatible; SSE zorunlu | accepted | — |
| 012 | [[.decisions/accepted/adr-012-dual-tfm-data-provider|ADR-012-dual-tfm-data-provider]] | SqlClient iki bacakta aynı major (pin 7.1.1 web-verified) | accepted | — |
| 013 | [[.decisions/accepted/adr-013-layered-architecture-52-nodes|ADR-013-layered-architecture-52-nodes]] | 8 makro + 44 alt = 52 düğüm; bileşen kaydı ≥5000 | accepted | — |
| 014 | [[.decisions/accepted/adr-014-slnx-and-cpm|ADR-014-slnx-and-cpm]] | VersaCoder.slnx + CPM; .csproj'da Version yok | accepted | — |
| 015 | [[.decisions/accepted/adr-015-winforms-form-triple-designer|ADR-015-winforms-form-triple-designer]] | .Form.cs + .Designer.cs + .resx üçlüsü; tasarımcı son söz | accepted | — |
| 016 | [[.decisions/accepted/adr-016-dbconnection-only-l4|ADR-016-dbconnection-only-l4]] | DbConnection yalnız l4 ado-dapper | accepted | — |
| 017 | [[.decisions/accepted/adr-017-parameterized-sql-only|ADR-017-parameterized-sql-only]] | @p1..@pN zorunlu; concat SQL = otomatik RED | accepted | — |
| 018 | [[.decisions/accepted/adr-018-secret-dpapi-keys-map|ADR-018-secret-dpapi-keys-map]] | Secret DPAPI + keys.md yalnız harita | accepted | — |
| 019 | [[.decisions/accepted/adr-019-dual-leg-build-test|ADR-019-dual-leg-build-test]] | Her değişiklik iki bacakta build + test; TDD %90+ | accepted | — |
| 020 | [[.decisions/accepted/adr-020-no-mermaid-only-box-drawing|ADR-020-no-mermaid-only-box-drawing]] | D1 UTF-8 box + D3 mermaid düz metin | **superseded** (D1) | superseded_by: ADR-029 (D1) |
| 021 | [[.decisions/accepted/adr-021-single-index-navigation|ADR-021-single-index-navigation]] | Navigasyon tek index.md; her dizinde en fazla bir liste | accepted | — |
| 022 | [[.decisions/accepted/adr-022-first-commit-gate-004|ADR-022-first-commit-gate-004]] | İlk commit yalnız GATE-004'te | accepted | — |
| 023 | [[.decisions/accepted/adr-023-reference-tree-readonly|ADR-023-reference-tree-readonly]] | %TEMP%\opencode-fresh salt okunur referans | accepted | — |
| 024 | [[.decisions/accepted/adr-024-coremusic-vault-immutable|ADR-024-coremusic-vault-immutable]] | CoreMusic vault dokunulmaz; yalnız yapı referansı | accepted | — |
| 025 | [[.decisions/accepted/adr-025-gate003-ui-hard-stop|ADR-025-gate003-ui-hard-stop]] | 7 ekran wireframe onayı = HARD STOP | accepted | — |
| 026 | [[.decisions/accepted/adr-026-structured-log-5-levels|ADR-026-structured-log-5-levels]] | Structured log, 5 seviye, tek şablon | accepted | — |
| 027 | [[.decisions/accepted/adr-027-count-components-script|ADR-027-count-components-script]] | Elle sayılmaz; tek kanıt count-components.ps1 | accepted | — |
| 028 | [[.decisions/accepted/adr-028-layered-baseline-9-projects|ADR-028-layered-baseline-9-projects]] | 8 makro / 52 düğüm / 9+1-2 proje baz | accepted | — |
| 029 | [[.decisions/accepted/adr-029-ascii-box-drawing-only|ADR-029-ascii-box-drawing-only]] | Diyagramlar saf ASCII (D22 ile politika revize → ADR-032) | accepted | supersedes: ADR-020 (D1) |
| 030 | [[.decisions/accepted/adr-030-5000-layers-class-level|ADR-030-5000-layers-class-level]] | 5000+ = sınıf/namespace kaydı; proje sabit 9+2 | accepted | — |
| 031 | [[.decisions/draft/adr-031-visual-production-provider|ADR-031-visual-production-provider]] | Görsel üretim sağlayıcısı (kapsam A+B+C) | proposed | — (draft'ta korunuyor) |
| 032 | [[.decisions/accepted/adr-032-ai-control-plane|ADR-032-ai-control-plane]] | **`.ai` Enterprise Control Plane — hedef mimari (D1..D56)** | **proposed** | — |

## §2 Supersede Zinciri

```text
ADR-020 (D1: UTF-8 box)  --superseded_by-->  ADR-029 (ASCII)  --revize-->  ADR-032 §2.16 (D22: Unicode kazanır)
```

## §3 Kaynak ve Provenance Kuralları

1. **ADR-001..030:** `source: brain.md §1/§1.1/§1.2 — yeniden inşa` (orijinal metin kayıp).
2. **ADR-030:** ayrıca `spec/layering-5000-spec.md v2.1.0` kaynağı kullanıldı.
3. **ADR-032:** onaylı mimari planı + D1..D56 karar defteri.
4. **ADR-031:** mevcut draft dosyası korunur; bu defter yalnız durumunu gösterir.
5. Yeni ADR: `draft/` → `accepted/` (onay sonrası); lifecycle
   `draft → proposed → accepted → implemented → superseded → deprecated` (D14).
6. `implemented` olan ADR = drift reconcile tetikleyicisi (D25).

## §4 İlgili

- [[.decisions/brain|brain]] — ADR özetleri (derived)
- [[index|index]] — vault navigasyonu
- [[claude|CLAUDE]] — anayasa (D5 zinciri)
- [[governance/audit|governance/audit]] — onay kayıtları
