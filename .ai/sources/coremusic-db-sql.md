---
title: "CoreMusic DB Şeması (19 SQL Dosyası)"
raw_path: "raw/coremusic-db-sql/"
ingested: 2026-10-06 22:13
tags: [mysql, db-sema, coremusic, celiski, kanit]
---

# coremusic-db-sql/ (19 dosya)

## Genel Özet

CoreMusic şemasının diskteki DDL kaynağı: `C:\www\coremusic.net\.ai\sources\.sql\mysql\` altında 19 SQL dosyası, aynen `raw/coremusic-db-sql/` altına kopyalandı (ham, değiştirilmez). Amaç: `wiki/coremusic-vault.md` Ek A-3 "tablo 156 vs 112" çelişkisini kanıtla kapatmak.

## Ana Fikirler

- **Toplam 165 `CREATE TABLE` / 164 benzersiz tablo** (19 dosya); `CREATE TABLE IF NOT EXISTS` = 0.
- **İki iddia de tutmuyor:** README "156 tablo" → eksik 8; `.ai.OLD/CLAUDE.md` §5 K5 "112 tablo" → eksik 52. Düzeltilmesi gereken değer: **164**.
- **Yeni çelişki:** `api_keys` tablosu hem `coremusic_api.sql` hem `coremusic_auth.sql` içinde tanımlı; `IF NOT EXISTS` yok → ikinci dosya canlı yüklemede hata verir.
- `coremusic_system.sql` = 17 tablo (13 system + 4 i18n) → ADR-079 sayımıyla birebir tutuyor.
- En büyük dosyalar: `coremusic_logs.sql` 22 · `coremusic_musics.sql` 22 · `coremusic_system.sql` 17 · `coremusic_auth.sql` 13.

## Önemli Alıntılar/Veriler

- Sayım yöntemi: dosya başına `CREATE\s+TABLE` regex sayımı + benzersiz tablo adı kontrolü (2026-10-06 22:13).
- Dağılım: ai 6 · albums 5 · api 4 · auth 13 · catalog 8 · cms 8 · download 4 · logs 22 · media 8 · musics 22 · neva 4 · patch 3 · playlist 5 · social 9 · studio 6 · system 17 · user 7 · wireless 5 · media_catalog 9 (= 165).
- Tablo tekrarı: 1 (`api_keys`); diğer 163 benzersiz.

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları

- [[coremusic-vault]] — Ek A-3 (156 vs 112) disk DDL kanıtıyla kapatıldı; gerçek sayı 164 + `api_keys` çakışması notu eklendi
