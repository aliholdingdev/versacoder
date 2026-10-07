---
title: "AI Electronics Engineer & Circuit Analyst & BOM Shopping Agent — Datasheet + Web Research Edition"
raw_path: "raw/electronic-gpt-ozle-promt.md"
ingested: 2026-10-06 18:59
tags: [prompt-arsivi, elektronik, devre-tasarimi, bom, satis-alma, zero-hallucination]
---

# AI Electronics Engineer & Circuit Analyst & BOM Shopping Agent

## Genel Özet
`electronic-gpt-ozle-promt.md` dosyası, Bayram Ali Akşit tarafından tasarlanmış 49 bölümlük bir ChatGPT sistem promptudur (Version 1.0.0). Prompt, kullanıcıya bir elektronik proje fikri, şema, PCB fotoğrafı veya arıza verildiğinde onu 50+ yıllık deneyime sahip Senior Electronics Engineer kimliğiyle analiz edip devre tasarımı, BOM ve satın alma listesi üreten bir ajan tanımlar (electronic-gpt-ozle-promt.md). Temel disiplin "Zero-Hallucination"tir: MPN, URL, fiyat, stok ve datasheet değeri uydurulamaz, doğrulanamayan bilgi `DOĞRULANAMADI` olarak işaretlenir (electronic-gpt-ozle-promt.md). Akış, eksik kritik bilgi varsa tasarıma geçmeyen interaktif soru döngüsü, zorunlu PDF/datasheet okuma ve web araştırmasıyla beslenir (electronic-gpt-ozle-promt.md). Satın alma tarafında Türkiye öncelikli tedarik ve doğrudan ürün linki verme kuralı vardır (electronic-gpt-ozle-promt.md).

## Ana Fikirler
- Rol: Senior Electronics Engineer / Circuit Designer / PCB Engineer / BOM Engineer / Technical Procurement Assistant, 50+ yıllık deneyim (electronic-gpt-ozle-promt.md).
- Zorunlu pipeline: `INPUT → PROJECT ANALYSIS → REQUIREMENTS → CONSTRAINTS → ELECTRICAL ANALYSIS → COMPONENT SELECTION → RISK ANALYSIS → CIRCUIT DESIGN → BOM → SHOPPING LIST → VALIDATION → FINAL OUTPUT` (electronic-gpt-ozle-promt.md).
- Zero-Hallucination kuralı: komponent, model, datasheet, fiyat, stok, URL, MPN asla uydurulmaz; doğrulanamıyorsa `DOĞRULANAMADI` yazılır (electronic-gpt-ozle-promt.md).
- Safety/Risk Gate: `⚠ LOW`, `⚠⚠ MEDIUM`, `⚠⚠⚠ CRITICAL` seviyeleri; 220/230 VAC, yüksek enerji batarya, Li-ion pack gibi durumlarda HARD STOP uygulanır (electronic-gpt-ozle-promt.md).
- Interactive Question Engine: soru sayısı dinamiktir (`N = yalnızca gerçekten gerekli soru sayısı`), soru önceliği SAFETY → POWER → ELECTRICAL → LOAD şeklinde sıralıdır (electronic-gpt-ozle-promt.md).
- Zorunlu PDF/Datasheet okuma ve kaynak önceliği: Manufacturer Datasheet birincil kaynaktır, çelişki `⚠ SOURCE CONFLICT` olarak belirtilir (electronic-gpt-ozle-promt.md).
- Türkiye öncelikli satın alma sırası: Türkiye satıcısı → Türkiye distribütörü → Türkiye'ye gönderim yapan uluslararası distribütör → yurtdışı alternatif (electronic-gpt-ozle-promt.md).
- Final durum etiketleri: `✅ DESIGN READY`, `⚠ DESIGN REQUIRES INFORMATION`, `⚠⚠ DESIGN REQUIRES ENGINEERING REVIEW`, `🛑 HARD STOP`; test edilmemiş sistem `VERIFIED` yapılamaz (electronic-gpt-ozle-promt.md).

## Önemli Alıntılar/Veriler
- **Rol:** 50+ yıllık Senior Electronics Engineer; görevler devre analizi, mimari, teşhis, komponent seçimi, BOM, PCB gereksinimleri (electronic-gpt-ozle-promt.md).
- **Bağlam girişi:** fotoğraf, şema, PCB görüntüsü, komponent listesi veya teknik problem — önce "PROJEYİ ANLA" (electronic-gpt-ozle-promt.md).
- **Kısıtlar:** tahmin yok, sahte link/MPN/fiyat/stok yok, gereksiz complexity ve gereksiz component eklenmez (electronic-gpt-ozle-promt.md).
- **Çıktı formatı (18 bölüm):** PROJECT SUMMARY, REQUIREMENTS, MISSING INFORMATION, RISK ANALYSIS, BLOCK DIAGRAM, CIRCUIT DESIGN, ASCII SCHEMATIC, CONNECTION TABLE, COMPONENT SELECTION, BOM, SHOPPING LIST, TURKEY PURCHASE OPTIONS, INTERNATIONAL ALTERNATIVES, ENGINEERING CALCULATIONS, PCB NOTES, TEST/MEASUREMENT PROCEDURE, VALIDATION, FINAL STATUS (electronic-gpt-ozle-promt.md).
- **BOM tablosu:** `| # | Component | Manufacturer | MPN | Value/Rating | Package | Qty | Alternative |` — doğrulanamayan parça `[MPN DOĞRULANMALI]` (electronic-gpt-ozle-promt.md).
- **Örnek komut bloğu (soru formatı):**
  ```text
  ## GEREKLİ SORULAR
  Toplam soru: N
  1. [SORU]
     Beklenen cevap: [FORMAT]
  ```
  ve numaralı cevap: `1: 12V DC` / `2: Maksimum 3A` (electronic-gpt-ozle-promt.md).
- **Ölçüm formatı:** `TEST POINT → MEASUREMENT MODE → EXPECTED VALUE → ACCEPTABLE RANGE → OBSERVED VALUE → RESULT`; örnek `TP1 / DC Voltage / Expected: 5.0V / Range: 4.8V–5.2V` (electronic-gpt-ozle-promt.md).
- **Öncelik sırası:** User Intent → Safety → Electrical Correctness → Component Compatibility → Availability → Maintainability → Cost → Simplicity (electronic-gpt-ozle-promt.md).
- **Final prensip:** "Doğruluk > Eksiksizlik > Hız", "Tahmin yok. Uydurma yok." (electronic-gpt-ozle-promt.md).

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları
- [[elektronik-prompt]] — promptun rolü, 49 bölümlük yapısı, zero-hallucination, BOM/alışveriş ve Türkiye-öncelikli tedarik kuralları.
- [[bayram-ali-prompt-arsivi]] — dosyanın Bayram Ali Akşit imzalı Prompt Arşivi üyesi olduğu (Version 1.0.0).
- [[prompt-engineering]] — interaktif soru motoru, placeholder (`[MPN DOĞRULANMALI]`), final status gate ve validation checklist örnekleri.
