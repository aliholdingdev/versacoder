---
title: "Versa Coder Proje Planlaması — Master Prompt (ai-00)"
raw_path: "raw/versacoder-proje-planlamasi-ai-00.md"
ingested: 2026-10-06 19:01
tags: [versacoder, proje-taniti, mimari, referans-analizi, second-brain, gereksinim]
---

# versacoder-proje-planlamasi-ai-00.md

## Genel Özet

Bu dosya, Versa Coder için yazılmış **master analiz ve mühendislik promptu**'dur (VERSION 1.0.0, "Designed & Coding by Bayram Ali Akşit"). Amaç kodlamaya başlamak değil; projeyi ve referans projeyi analiz etmek, teknik gerçekleri doğrulamak, gereksinim çıkarmak, kullanıcıya soru sormak ve `.ai` ikinci beyni kurup onay almaktır (`versacoder-proje-planlamasi-ai-00.md`). Belge; rol/tanım, çalışma akış diyagramaları, referans proje erişim kuralları, web araştırma ve kaynak doğrulama, gereksinim formatı, soru sorma motoru, `.ai` kalite kuralı ve kod tabanı indeksi gibi bölümlerden oluşur (`versacoder-proje-planlamasi-ai-00.md`). Teknik yığın C#, net48 + net10.0-windows, DevExpress 2026 Universal, katmanlı mimari, SOLID/Clean Architecture olarak tanımlanır (`versacoder-proje-planlamasi-ai-00.md`).

## Ana Fikirler

- Ana amaç "Versa Coder'ı doğrudan kodlamak" değil; 11 adımlık sıra: projeyi anlamak → referans projeyi analiz etmek → teknik gerçekleri doğrulamak → gereksinim çıkarmak → eksikleri tespit etmek → soru sormak → `.ai` bilgi tabanını kurmak → 5000+ katmanlı mimari dosyalarını oluşturmak → onay almak → implementation planı → kodlama (`versacoder-proje-planlamasi-ai-00.md`).
- LLM sistemin kendisi değildir; asıl sistem agent runtime, orchestration, policy engine, session management, tool gateway, memory, workflow engine, persistence ve observability katmanlarından oluşur (`versacoder-proje-planlamasi-ai-00.md`).
- Referans projeler: `C:\www\versacoder\versacoder\versacoder.ai\.ai`, `https://github.com/anomalyco/opencode.git` (branches: v2), `C:\opencode`, `C:\www\versacoder\opencode`, `C:\www\versacoder\versacoder`; referans projeden kod/mimari/isim kopyalamak yasaktır (`versacoder-proje-planlamasi-ai-00.md`).
- Teknik karar hiyerarşisi: "Doğruluk > Eksiksizlik > Hız", "Gerçek veri > Varsayım", "Kaynak > Tahmin", "Analiz > Kodlama", "Kullanıcı onayı > Agent varsayımı" (`versacoder-proje-planlamasi-ai-00.md`).
- `.ai` Second Brain kodlamadan önce kurulur; minimum 500 satır hedeflenir ancak "satır sayısı kalite ölçütü değildir"; anlamsız tekrar, yapay dolgu, doğrulanmamış bilgi ve placeholder yasaktır (`versacoder-proje-planlamasi-ai-00.md`).
- Gereksinimler `REQ-ID / TITLE / DESCRIPTION / SOURCE / PRIORITY / DEPENDENCIES / STATUS / OPEN QUESTIONS` formatında; öncelikler P0 Blocking, P1 Critical, P2 Important, P3 Optional (`versacoder-proje-planlamasi-ai-00.md`).
- NEVER ASSUME kuralı: database, ORM, authentication, AI provider, model, hosting, deployment, vector database vb. kullanıcı adına varsayılmaz (`versacoder-proje-planlamasi-ai-00.md`).
- Soru sorma öncelikleri P0/P1/P2; her cevaptan sonra zincir: Answer → Requirement Update → Conflict Check → Dependency Check → Requirement Re-validation (`versacoder-proje-planlamasi-ai-00.md`).

## Önemli Alıntılar/Veriler

- "Amaç, Versa Coder projesini doğrudan kodlamaya başlamak değildir." (`versacoder-proje-planlamasi-ai-00.md`)
- Dürüstlük etiketleri: «DOĞRULANAMADI», «BİLİNMİYOR», «ÇIKARIM», «WEB DOĞRULANDI», «REFERENCE VERIFIED» (`versacoder-proje-planlamasi-ai-00.md`)
- Çelişki durumu: "CONFLICT DETECTED"; kaynak yoksa: "VERIFICATION REQUIRED" (`versacoder-proje-planlamasi-ai-00.md`)
- "Doğrulama yapmadan kabul etme source ekle" — `.ai` kalite kuralı (`versacoder-proje-planlamasi-ai-00.md`)
- Web araştırma önceliği: 1. Official Documentation, 2. Official GitHub/Repository, 3. RFC/Specification, 4. Primary Technical Source, 5. Güvenilir Secondary Source (`versacoder-proje-planlamasi-ai-00.md`)
- Teknoloji listesi: C#, net48 + net10.0-windows, DevExpress 25.2.8 Universal, ASP.NET Core, Class Library/DLL, SOLID, DDD/CQRS "gerektiği yerde", MCP, Plugin Architecture, Session/Workflow Management (`versacoder-proje-planlamasi-ai-00.md`)
- `.ai` yapıyı oluşturan klasörler: `.agents/`, `.decisions/`, `.diagram/`, `.rules/`, `.templates/`, `.plans/`, `.spec/`, `.todo/`, `checklists/`, `knowledge/`, `prompts/`, `reports/`, `scripts/`, `ui-design/`, `architecture/`, `ecosystem/`, `servers/` + kök `brain.md`, `engine.md`, `glossary.md`, `keys.md`, `MEMORY.md`, `ROLE.md`, `VISION.md` vb. (`versacoder-proje-planlamasi-ai-00.md`)
- Çıktılar: "AI VALIDATION REPORT" ve "CODEBASE INDEX" (`versacoder-proje-planlamasi-ai-00.md`)

## Bu Kaynaktan Oluşan/Güncellenen Wiki Sayfaları

- [[versacoder-projesi]] — proje tanımı, teknoloji yığını, referans proje listesi ve yasaklar
- [[agent-workflow]] — DISCOVERY→ANALYSIS→VALIDATION→KNOWLEDGE MODEL→.AI BRAIN akışı, Multi-Agent Swarm ve TARGET SYSTEM diyagramları
- [[prompt-engineering]] — Asking Questions Engine, NEVER ASSUME, P0-P3 öncelik ve soru-cevap zinciri
- [[ai-agent-hafizasi]] — `.ai` Second Brain yapısı, 500 satır kalite kuralı ve doğrulama zorunluluğu
- [[referans-proje-analizi]] — referans projeden ne alınır/ne alınmaz, 9 analiz alanı ve kaynak doğrulama kademeleri
