---
title: "Part 00 — 100 Partlık Analiz Seti İçindekileri ve Haritası"
raw_path: "raw/versacoder-proje-ai-00.md"
ingested: 2026-10-06 19:01
tags: [analiz-seti, versacoder, indeks, icerik-haritasi, celiski-defteri, vault]
---

# versacoder-proje-ai-00.md

## Genel Özet

Bu dosya, VersaCoder hakkında yazılan **100 partlık analiz setinin ana kapağı/indisi**dir: 13 blok (0–12) halinde 100 partın tek satırlık açıklamaları, okuma sırası ve batch üretim durumu verilir (`versacoder-proje-ai-00.md`). Her partın 9 bloktan oluştuğu (başlık → ELI10 → adım adım → derin dal → …) ve ELI10 + Derin Dal katmanlı yazıldığı; derin dal bölümlerinde `kaynak: dosya satır X–Y` atfı bulunduğu belirtilir (`versacoder-proje-ai-00.md`). Dosya ayrıca 3 okuma yolu (hızlı ~45 dk, derin ~20 saat, uygulama ~6 saat), 7 maddelik batch doğrulama kontrolü, 100 part okuma çizelgesi ve 5 maddelik gelecek özellik listesi sunar (`versacoder-proje-ai-00.md`). Çelişki ve doğrulama notları (sayım farkları, eksik kaynaklar, YOK/DOĞRULAMA GEREKLI işaretleri) dosyanın "Dikkat / çelişki" bölümünde toplanmıştır (`versacoder-proje-ai-00.md`).

## Ana Fikirler

- Set 13 blok / 100 parttan oluşur; bloklar: 0 Başlangıç (00–09), 1 Kimlik&Roller (10–17), 2 Ana Prompt §1–§36 (18–32), 3 Ana Talimat 1→20 (33–41), 4 Kural Seti 0→40 (42–55), 5 Şablonlar (56–64), 6 Mimari 9 Tier/618 DLL (65–74), 7 Wizard&Gelecek (75–81), 8 Ücretsiz Modeller (82–87), 9 Kurallar&Referans (88–91), 10 Vault Köprüsü (92–94), 11 Çelişki Defteri (95–96), 12 Gelecek&Final (97–99) (`versacoder-proje-ai-00.md`).
- Set kuralı: "önce niye, sonra ne, sonra nasıl" — Blok 0–1 "niye", Blok 2–5 "ne", Blok 6–8 "nasıl inşa edilir", Blok 9–12 "kurallar, çelişkiler ve gelecek" (`versacoder-proje-ai-00.md`).
- Part 00/01/07/08/09 hariç her partın ayrı kaynak dosyası vardır; kaynakta olmayan şey yazılmaz, **YOK** veya **DOĞRULAMA GEREKLI** denir (`versacoder-proje-ai-00.md`).
- Part 03 sözlük (90 madde) sürekli açık sayfa, Part 95 çelişki defteridir (`versacoder-proje-ai-00.md`).
- Mimari blok: Tier 0→8, 618 giriş / **616 benzersiz**; "500+" kaynak alıntısıdır; 50/52/5000+ katman çelişkisi Part 74'tedir (`versacoder-proje-ai-00.md`).
- Vault köprüsü bloğu (92–94): coremusic-vault-template'in neden **YAPI referansı** olduğu (içerik değil), alınacak 14 kural ve `.ai` kurulum sırası (`versacoder-proje-ai-00.md`).
- Üretim 7 batch'tir (1: 00–17, 2: 18–32, 3: 33–41, 4: 42–55, 5: 56–74, 6: 75–91, 7: 92–99); hepsi ⏳ (`versacoder-proje-ai-00.md`).

## Önemli Alıntılar/Veriler

- "Bu dosya setin **ana kapağıdır**." — `kaynak: _PLAN.md satır 79–231` (`versacoder-proje-ai-00.md`)
- 7 doğrulama kontrolü: 100 dosya · numara 00–99 kesintisiz · her part ≥500 satır · frontmatter 7 alan · UTF-8 Türkçe · 9 blok sırası · `Sonraki part` linki (`versacoder-proje-ai-00.md`).
- Frontmatter 7 alan: `title, type, category, version, status, authority, updated` (`versacoder-proje-ai-00.md`).
- Sayım düzeltmeleri: §19'da kaynakta **9** bileşen var, plan "10" der; §6'da **9** analiz alanı var, plan "10" der; §1'de **5** dürüstlük etiketi var, plan "6" der (`versacoder-proje-ai-00.md`).
- ⚠️ "Plan **13 blok** der; blok numaraları 0–12'dir" · "Blok 3'te 9 part var, 10 değil" (`versacoder-proje-ai-00.md`).
- Okuma yolları: HIZLI ~45 dk (00,02,04,58,60,99) · DERİN ~20 saat · UYGULAMA ~6 saat (Blok 3+4+51+88–89+95) (`versacoder-proje-ai-00.md`).
- Blok 9 kararları: Autofac, EF Core, xUnit, DevExpress, SSE, SQLite+PostgreSQL, AES-256-GCM + "⚠️ EF Core ↔ Dapper çelişkisi" (`versacoder-proje-ai-00.md`).
- Dosya adı deseni: `part-<2 hane numara>-<kucuk-harf-turkce-ozet>.md` (`versacoder-proje-ai-00.md`)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları

- [[versacoder-analiz-seti]] — 13 blok / 100 part yapısı, 3 okuma yolu, 7 batch ve doğrulama kontrolleri
- [[versacoder-projesi]] — setin anlattığı ürün tanımı referansları (Part 02, 58–60)
- [[coremusic-vault]] — Part 92–94: vault şablonunun "yapı referansı" olarak kullanımı
- [[vault-mimarisi]] — Part 94 `.ai` kurulum sırası (templates → agents → kök MD → mimari + onay kuralı)
- [[prompt-engineering]] — setin ELI10 + Derin Dal yazım şeması ve kaynak atfı disiplini
