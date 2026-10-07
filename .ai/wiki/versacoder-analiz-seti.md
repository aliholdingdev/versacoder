---
title: VersaCoder Analiz Seti
type: proje
created: 2026-10-06
updated: 2026-10-06
sources:
  - raw/versacoder-proje-ai-00.md
tags: [analiz-seti, 100-part, okuma-yolu, batch]
---

# VersaCoder Analiz Seti

**Ne:** 100 partlık VersaCoder analiz setinin **ana kapağı** (versacoder-proje-ai-00).

**Harita:** 13 blok (0–12) / 100 part.
- Blok 2 — Ana Prompt §1–§36 (15 part)
- Blok 4 — Kural Seti 0→40 (14 part)
- Blok 6 — Mimari 9 Tier / 618 DLL (10 part)
- Blok 9 — Kararlar: Autofac, EF Core, xUnit, DevExpress, SSE, SQLite+PostgreSQL, AES-256-GCM
- Blok 10 — coremusic-vault-template (part 92–94)

**Part şeması (9 bölüm):** başlık → öğren → ELI10 → adım adım → derin dal → örnek → dikkat → gelecek → sonraki. Her part **≥500 satır**.

**3 okuma yolu:** HIZLI ~45 dk · DERİN ~20 saat · UYGULAMA ~6 saat.

**7 batch üretim sırası:** 00-17, 18-32, 33-41, 42-55, 56-74, 75-91, 92-99.

**7 doğrulama kontrolü:** 100 dosya · 00–99 kesintisiz · ≥500 satır · frontmatter 7 alan · UTF-8 · 9 blok sırası · `Sonraki part` linki.

**Yazım disiplini:** "önce niye, sonra ne, sonra nasıl"; her derin dalda `kaynak: dosya satır X–Y` atfı; kaynakta olmayan bilgi = `YOK / DOĞRULAMA GEREKLI`.

**✓ Çelişki ÇÖZÜLDÜ (2026-10-06) — 5 vs 6 dürüstlük etiketi:**
- `raw/versacoder-proje-planlamasi-ai-00.md` satır 226–230 → tam olarak **5 etiket**: `DOĞRULANAMADI` · `BİLİNMİYOR` · `ÇIKARIM` · `WEB DOĞRULANDI` · `REFERENCE VERIFIED`.
- `raw/versacoder-proje-ai-00.md` satır 153 (part-18) → "§1 SYSTEM ROLE ve **5 dürüstlük etiketi (kaynakta 6. yok)**".
- Sonuç: **5 doğrulandı**, "6" iddiası kaynakta karşılığı olmayan bir sayım; 6. etiket üretilmeyecek.

## İlgili Sayfalar
- [[versacoder-projesi]] — setin anlattığı proje
- [[prompt-engineering]] — setin yazım disiplini
- [[referans-proje-analizi]] — sette kullanılan doğrulama yöntemi
- [[coremusic-vault]] — Blok 10 konusu
- [[vault-mimarisi]] — Part 94 `.ai` kurulum sırası
