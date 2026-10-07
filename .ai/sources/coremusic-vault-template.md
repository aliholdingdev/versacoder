---
title: "CoreMusic Vault — Proje Şablonu (Yeniden Kullanılabilir Vault İskeleti)"
raw_path: "raw/coremusic-vault-template.md"
ingested: 2026-10-06 19:01
tags: [coremusic, vault, sablon, ssot, guardrail, agent-kayit-defteri, celiski]
---

# coremusic-vault-template.md

## Genel Özet

Bu dosya, CoreMusic `.ai/` vault'unun tamamını başka projede yeniden kullanmak için çıkarılmış, **salt doğrulanmış** vault içeriğinden üretilmiş şablondur (v1.1.0, authority: super-primary) (`coremusic-vault-template.md`). Proje künyesi, gerçek teknoloji yığını, K0–K20 / A0–A5 / 10 panel / 7 servis mimarisi, gerçek klasör ağacı, 16 Hard Guardrail, `.rules/senior-mode.md` (zero-hallucination, 16-adım ön-analiz), 9 workflow dosyası, 11/13 agent kayıt defteri, kalite-güvenlik standartları, ADR/commit kuralları ve yeni proje için 30 dakikalık kurulum adımları içerir (`coremusic-vault-template.md`). Her belirsiz yer `YOK`, `[ŞABLON NOTU]` veya `⚠️` ile işaretli; çözülmemiş 9 çelişki Ek A'da toplanmıştır (`coremusic-vault-template.md`). §10, neyin değiştirileceğini (16 madde) ve neyin olduğu gibi kalacağını (14 madde) ayırır (`coremusic-vault-template.md`).

## Ana Fikirler

- CoreMusic künyesi: `coremusic.net`, vizyon "Aynı Müzik Her Yerde Seninle", vault envanteri **24 klasör · 340 MD dosya**, 10 subdomain/panel, 7 backend servis (`coremusic-vault-template.md`).
- Mimari: **21 katman K0–K20 · 1.095 bileşen** + A0–A5 alt katman eşlemesi (`A0 = K0-K5, A1 = K6-K7, A2 = K8-K9, A3 = K10-K11, A4 = K12-K15, A5 = K16-K20`) (`coremusic-vault-template.md`).
- Kök `CLAUDE.md` bir **pointer**'dır ("All AI instructions ... live in `.ai/`"); SSOT = `.ai/` (ADR-042) (`coremusic-vault-template.md`).
- 16 Hard Guardrail (numaralar 1–14, 16, 17): Zero Code Before Plan, Vault First, Zero Hallucination, In-Place Refactoring, SSOT, CSRF `csrf_token`, Middleware Order Immutable, Port 81, No ORM, No Frameworks, Mockup Before Frontend, Contradiction Gate, Session Continuity, Human Approval Gate, Template Mandatory, Single Component Responsive (`coremusic-vault-template.md`).
- Agent sistemi: `.ai/AGENTS.md` §4 = **11 agent** (1 Master Orchestrator + 10 uzman), `.opencode/opencode.json` = **13** ⚠️ çelişki; keyword → agent routing 9 grup; öncelik CRITICAL/HIGH/MEDIUM/LOW; eskalasyon L1→L2→L3→İnsan (`coremusic-vault-template.md`).
- Onay gerektirenler: ADR freeze, dosya adı/yolu, production migration, silme, yeni harici bağımlılık (`coremusic-vault-template.md`).
- UTF-8 yazım protokolü: tüm vault yazımları yalnız `node .ai/scripts/vault-utf8-writer.mjs`; `Set-Content`, `Out-File`, `Add-Content`, `echo >` yasak; `log.md` sadece append (`coremusic-vault-template.md`).
- Yeni proje 30 dakika: dizin kur → pointer/boot dosyaları → rules+workflows → agent+config → şablon+script → doğrulama (`coremusic-vault-template.md`).

## Önemli Alıntılar/Veriler

- Test kapsamı: "min ≥%80 · hedef ≥%90 (modül bazlı)" — `.ai/CLAUDE.md` §17 (`coremusic-vault-template.md`).
- ADR serileri: `.ai/.decisions/` 001-089 (001-037 frozen, 038-089 active) · `.ai/architecture/adr/` 023-026 — "birleştirme REDDEDİLDİ (ADR-026 §3.4)" (`coremusic-vault-template.md`).
- ADR sayımları: "37 frozen · 31 active · 12 rejected · 0 draft (toplam 80 dolu; 89 numaradan 9 boşluk)" (`coremusic-vault-template.md`).
- Middleware sırası (immutable): `OriginCheck → Cors → RateLimiter → SecurityHeaders → SessionManager → Csrf → BypassAuth → Auth → Permission → Validation` (`coremusic-vault-template.md`).
- Güvenlik: Argon2id · AES-256-GCM (96-bit IV, 16-byte tag, ADR-022) · CSP nonce 256-bit (ADR-012) · rate limit 60 req/60s APCu (ADR-013) · CORS whitelist ONLY (`coremusic-vault-template.md`).
- Stack: PHP 8.4+ · Vanilla JS ES2022 · C++20 JUCE 9/ASIO/WASAPI/ALSA · MySQL 9 18 BCNF · Redis/APCu (`coremusic-vault-template.md`).
- Test edilmeden **"tamamlandı" denmez**; review zinciri: `IMPLEMENT → BUILD → TEST → ARCHITECTURE CHECK → SECURITY CHECK → REVIEW → REFACTOR` (`coremusic-vault-template.md`).
- Ek A çelişkileri (9): agent 11 vs 13 · workflows kopyaları · tablo 156 vs 112 · yeni ADR numarası üçlü iddia · kırık `[[../AGENTS.md]]` · eksik script'ler · boot listesi boyut farkı · 80/89 sayımda 9 boşluk · "%100 test kapsamı" iddiası §17'de YOK (`coremusic-vault-template.md`).
- Öncelik seviyeleri timeout'ları: CRITICAL 5s/max retry 1 · HIGH 15s/3 · MEDIUM 30s/3 · LOW 60s/2; Context Lock max 30s (`coremusic-vault-template.md`).

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları

- [[coremusic-vault]] — proje künyesi, envanter (24 klasör/340 MD), teknoloji yığını ve komutlar
- [[vault-mimarisi]] — K0–K20/A0–A5, klasör ağacı, 15 kök boot dosyası, 36 şablon, §9 30 dk kurulum
- [[ssot]] — kök CLAUDE.md pointer, `.ai/` SSOT (ADR-042), frontmatter 7 alan, wiki-link kuralı
- [[agent-workflow]] — 9 workflow dosyası, orchestrator-flow 11 aşama, agent routing/dispatch/handover/eskalasyon
- [[multi-llm-bagimsizlik]] — `.opencode/opencode.json` agent/komut/skill/MCP yapısı ve 11 vs 13 agent çelişkisi

**Çelişki çözüm notu (2026-10-06):** Ek A-1 (agent 11 vs 13) **kapandı** → config'te 13 anahtar sayıldı, registry 11; eksik 2 = `plan`, `vault-updater`. Ek A-3 (tablo 156 vs 112) **kapandı (22:13)** → disk DDL kanıtı: 19 dosyada 165 `CREATE TABLE` / **164 benzersiz** (`raw/coremusic-db-sql/`); README 156 eksik 8, K5 112 eksik 52. Ek A'nın kalan 7 maddesi (A-2 workflows kopyaları, A-4 ADR numarası, A-5 kırık `[[../AGENTS.md]]`, A-6 eksik script'ler, A-7 boot listesi, A-8 80/89 boşluk, A-9 test kapsamı) **wiki'ye taşındı** → bkz. [[coremusic-vault]].
