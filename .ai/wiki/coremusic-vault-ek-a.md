---
title: CoreMusic Vault — Ek A Çelişki Kaydı
type: kavram
created: 2026-10-06
updated: 2026-10-07
sources:
  - raw/coremusic-vault-template.md
  - raw/coremusic-db-sql/
tags: [coremusic, celiski, ek-a, denetim]
---

# CoreMusic Vault — Ek A Çelişki Kaydı

**Ne:** `raw/coremusic-vault-template.md` dosyasının "Ek A — Tespit Edilen Çelişkiler" bölümünün (satır 781–793) izleme defteri; [[coremusic-vault]]'tan ayrıldı (kelime sınırı).

**Durum tablosu (kanıt taraması 2026-10-07 00:12, salt-okunur):**

| Madde | Çelişki | Durum |
|---|---|---|
| A-1 | Agent 11 vs 13 → config'te 13 anahtar, registry 11; eksik 2 = `plan`, `vault-updater` | ✓ ÇÖZÜLDÜ |
| A-2 | `.workflows/` vs `.opencode/.workflows/`: `session-init.md` (9.353 B vs 2.566 B), `vault-sync.md` (8.209 B vs 1.242 B), `security-audit.md` (7.747 B vs 1.541 B) → 3 kopya farklı sürümde | ⚠ AÇIK — `.workflows/` kopyaları büyük/güncel (09-24/29), `.opencode/` küçük; geçerli sürüm upstream kararı |
| A-3 | Tablo 156 vs 112 → disk DDL kanıtı **164 benzersiz / 165 CREATE TABLE** (`raw/coremusic-db-sql/`) | ✓ ÇÖZÜLDÜ (2026-10-06 22:13) |
| A-4 | Yeni ADR numarası üçlü iddia: `≥090` (`.workflows/adr-creation.md`) · `088+` (kök CLAUDE.md + vault-updater) · defter `ADR-001..089` (senior-mode §2) | ⚠ AÇIK — kanıt: diskte **73 ADR dosyası, aralık 001–093, 20 boşluk** (051–088 arası) → "001..089" iddiası geçersiz, "≥090" uyumlu (90–93 verilmiş) |
| A-5 | Kırık wiki-link: `.workflows/CLAUDE.md` §3 `[[../AGENTS.md]]` → kök `AGENTS.md` yok (Test-Path=false) | ✓ ÇÖZÜLDÜ (2026-10-06 23:03) — kök `AGENTS.md` **artık VAR** (Test-Path=true); eski kanıt eskimiş, link geçerli |
| A-6 | Eksik script'ler: `session-save.mjs`, `vault-post-update.mjs`, `vault-cmd.mjs`, `project-state.md` zorunlu, `.ai/scripts/`'te yok | ⚠ DOĞRULANDI AÇIK — repo genelinde **4/4 dosya yok**; upstream ekleme gerektirir |
| A-7 | Boot listesi boyut farkı: `AGENTS.md §24.2` = 14 dosya · `MEMORY §5` = 20 adım · FULL boot = 17 öğe | ✓ ÇÖZÜLDÜ (2026-10-07 00:12) — **birim farkı**: 14 = kök liste (= §5 satır 1–14) · 20 = 14 + 6 genişletilmiş okuma (tablo 20/20 ✓) · 17 = 3 root + 14 kök dosya (§7.1 uyumlu) · 15 = glob (14 + `broken-links-report`); `MEMORY:600` "+3 dizin" ifadesi belirsiz (upstream notu) |
| A-8 | ADR banka sayımı: 37 frozen + 31 active + 12 rejected = 80 dolu / 89 numara / **9 boşluk** `DOĞRULAMA GEREKLI` | ⚠ GÜNCEL KANIT — disk sayımı: **73 dolu / max 093 / 20 boşluk** → "80/89/9" üçlüsü tutmuyor; upstream sayımı düzeltmeli |
| A-9 | "%100 test kapsamı" (25/25 class, 451/451 method) iddiası `.ai/CLAUDE.md` §17'de YOK; §17 ≥%80/≥%90 hedefidir | ✓ ÇÖZÜLDÜ (2026-10-06 23:03) — grep: CLAUDE.md'de `%100\|25/25\|451` = **0 eşleşme**; iddianın kaynağı `.ai/.templates/CLAUDE.md` üretim sayacı (test kapsamı değil) |

**Ek tespit (2026-10-06):** `api_keys` tablosu `coremusic_api.sql` + `coremusic_auth.sql` içinde çift tanımlı, `IF NOT EXISTS` yok → yükleme çakışması (bkz. [[coremusic-db-sql]]).

**Özet:** 9 maddenin **5'i kapandı** (A-1, A-3, A-5, A-7, A-9), **4'ü kanıtla daraldı** ve upstream onayı bekliyor (A-2, A-4, A-6, A-8).

**Kanal:** Açık maddeler CoreMusic vault'unun kendi denetimine aittir; bu wiki'de kanıt toplanır, upstream'e düzeltme uygulanmaz (bkz. [[ssot]]).

## İlgili Sayfalar
- [[coremusic-vault]] — şablonun ana sayfası (bu dosyanın kaynağı)
- [[coremusic-db-sql]] — A-3'ü kapatan disk DDL kanıtı
- [[adr]] — A-4 ve A-8'in konusu (ADR numaralandırma)
- [[vault-mimarisi]] — A-6, A-7 boot/script düzeninin bağlamı
- [[ssot]] — çelişki kaydının dayanağı: kanıtsız sayı yazılmaz
