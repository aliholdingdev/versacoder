---
title: INGEST Prosedürü
type: kavram
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-planlamasi-ai-00.md
  - raw/coremusic-vault-template.md
tags: [ingest, surec, index, log]
---

# INGEST Prosedürü

**Tam metin:** [[CLAUDE]] §9 (anayasa). Özet:

1. `raw/` içinde özeti olmayan dosyaları tespit et
2. Dosyayı **baştan sona** oku
3. Kullanıcıya 3-5 çıkarım söyle
4. `sources/<kaynak>.md` özetini oluştur
5. Kavram/kisi/arac/proje sayfaları üret veya güncelle
6. Çelişki varsa **`⚠ Çelişki`** notu + iki kaynak
7. `[[çapraz link]]` ekle
8. `index.md`'yi güncelle
9. `log.md`'ye tarihli ingest kaydı ekle

**Hedef:** Kaynak başına **5-15 sayfa** etkilenmesi normaldir.

**Zorunlu tablo:** `sources/özet.md`'ye satır + üstteki kaynak sayısı ve son güncelleme tarihi güncellenir; **bunu yapmadan ingest tamamlanmış sayılmaz** (CLAUDE.md §9.1).

**Tarih kuralı:** Tarih asla tahmin edilmez; `date "+%Y-%m-%d %H:%M"` (PowerShell: `Get-Date -Format "yyyy-MM-dd HH:mm"`) ile gerçek değer alınır.

**İlk uygulama:** 2026-10-06 — 20 kaynak, 32 wiki sayfası ile başladı; LINT düzeltmeleri sonrası **36 sayfa** (bkz. [[log]]).

## İlgili Sayfalar
- [[llm-wiki]] — prosedürün çalıştığı sistem
- [[ssot]] — index/log tutarlılığının dayanağı
- [[versacoder-analiz-seti]] — benzer doğrulama/batch disiplini
- [[bayram-ali-prompt-arsivi]] — işlenen ilk kaynak kümesi
