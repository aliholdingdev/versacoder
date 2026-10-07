---
title: 'VERSACODER - CONTRIBUTING: Katkı, Roller, Stil, Review, Commit Kurallari'
source: 'USER-BOOTSTRAP §9 (Root Documentation); versacoder proje planaması ai.md'
date: '2026-09-24'
status: 'ACTIVE'
confidence: 'SOURCE_VERIFIED'
related_decisions: ['ADR-001', 'ADR-003', 'ADR-004', 'ADR-005']
related_requirements: ['REQ-DOC-001']
---

# 1. Amaç ve Kapsam

Bu dosya, VERSA CODER'a katkı kurallarını tek yerde toplar: kabul koşulları, roller ve görev formatı, kodlama stili, review ve commit kuralları. Kapsam: katkı akışı, stil, review, commit. Kapsam dışı: kapı kriterleri (`WORKFLOW.md`), gate listesi ve oturum kaydı (`AGENTS.md`), güvenlik derinliği (`SECURITY.md`), mimari katman (`ARCHITECTURE.md`).

Neden bu dosya var: katkıcı kuralları bilmeden girerse P0 kapı koşulu oluşur (`REQ-DOC-001`: kök belgeler tek giriş kapısı). Köken: `USER-BOOTSTRAP §9 (Root Documentation)` + `.ai/.rules/*.md` + 6 ADR + disk (`REPOSITORY_VERIFIED`, 2026-09-24).

İçindekiler: 1 Amaç / 2 Başlamadan / 3 Roller / 4 Stil / 5 Review / 6 Commit / 7 Sorumluluk / 8 Kabul / 9 Kapsam Dışı / 10 Çelişkiler.

# 2. Başlamadan Önce (P0)

| # | Adım | Kanıt |
|---|---|---|
| 1 | Kök belgeleri oku (`AGENTS/CLAUDE/README/WORKFLOW/SECURITY/ARCHITECTURE`) | okuma kaydı |
| 2 | İşe vurulan `.ai/.rules/*.md` + gerektiren ADR | `.ai/.rules/`, `.ai/.decisions/` |
| 3 | Open question kapat, elde `.ai/sessions/<id>` oluştur | oturum kaydı 6 alan |
| 4 | İlgili gate `PASSED` + `KANIT` | `.ai/gates/` (yoksa `BLOCKED`) |
| 5 | `.ai/archives/snapshots/<id>` snapshot (`BACKUP_OK`) | kopya = özgün sayı |

P0 açıkken kod yazmak yasak (`AGENTS.md` §6.1); kapı kanıtsız `PASSED` = sahtecilik (`AGENTS.md` §4.2). 2026-09-24: `.git` 0 commit olduğu için git'e güvenilmez, snapshot zorunlu; `.ai/gates/` klasörü yok → `BLOCKED` (§10.4).

# 3. Roller ve Görev Formatı

`.ai/.agents/AGENTS.md` ve `.ai/.templates/` yok (`EKSIK_OKUMA`) → buradaki liste `NEEDS_VALIDATION`. Görev kimliği `T-NNN`, 8 alanlı: `ID / BASLIK / KAPSAM / CIKTI / KANIT / BUDGET / DURUM / BAGIMLILIK`. Taslak: `T-000` yer tutucu; kapalı: `DONE` + `KANIT`; bekleyen bağımlılık: `BLOCKED` + neden (`AGENTS.md` Bölüm 6).

| Rol | Ne yapar | Sınır |
|---|---|---|
| master-orchestrator | plan, gate, görev atama | P0 üretmez |
| context-analyst | okuma, kural/ADR bulma | kod yazmaz |
| planner | kırılım, bağımlılık, bütçe | kod yazmaz |
| architect | mimari/AUI, ADR hazırlığı | kod yazmaz |
| ui-designer | wireframe, WPF/WinForms XAML | kod yazmaz |
| dev | GATE-006 sonrası kod + test | gate'siz kod yasak |
| reviewer | P0/P1 review | onay = kendinin üretmediği |
| test-runner | GATE-007, coverage | kanıtsız PASSED yok |
| debugger | log/dump kök-neden | doğrudan patch yok |
| security-engineer | SECURITY.md §4-§6 | politika gevşetemez |
| document-writer | kök + `.ai` belgeleri | `.decisions/.requirements/.gates` yasak |

Sub-agent dördü (`AGENTS.md` Bölüm 3.2): yalnız parent için dosya değiştirmek; `.decisions/.requirements/.gates` altına yazmamak; sınırı genişletmemek; çıktıyı parent doğrulamadan sisteme sokmamak.

# 4. Kodlama Stili

| Alan | Kural | Kaynak |
|---|---|---|
| Dil / SDK | C# 13 (net48) + C# 14 (net10.0-windows); `net10.0-windows` NOTLIB | ADR-003 |
| UI | DevExpress Universal 2026, RibbonControl Office2010 teması | ADR-001 |
| Veri | ADO.NET + Dapper; EF Core/EF6 yasak | ADR-004 |
| Tasarım | her form elle `.Designer.cs`; kod üreticisi yasak | ADR-005 |
| Kapsam | yalnız sahip olunan dosya; dead code üretme | `.ai/.rules/coding.md` |
| Test | test projesi `net48`, MSTest xUnit/NUnit/TUnit kopyası | `.ai/.rules/testing.md` |
| Yardımcı | NLog loglama, FluentValidation kural, Polly tekrar | ADR-004 |
| Katman | katmanlar arası DTO ile geçiş, tip eşlemesi tek yerde | ADR-004 |

Sürüm seçimi otomatik değil: ADR-003 (`net10.0-windows`) ile aday `C-ARCH-002` ("yerel DevExpress feed'de `net10` varyantı yok") birlikte ele alınmalı. `.ai/.rules/dependencies.md` kapı gerekçesi ister.

# 5. Kod İnceleme (Review)

| Kural | Ayrıntı |
|---|---|
| Amaç | mimari, kod, güvenlik, test kalitesi (`AGENTS.md` Bölüm 3.2 #4) |
| Yetki | `P0` → durdurucu; `P1` → bölüm; `P2` → tek görev; `P3` → opsiyonel |
| Çıktı | ilgili `.ai/sessions/<id>` altına, `KANIT` olmadan `DONE` yok |
| Yasak | geçiş notu = belge; kısa yol = bulgu; yuvarlak sayı; kapsam dışı rapor |
| Roller | üretici review isteyemez (`AGENTS.md` Bölüm 4.4), üreten de onaylayan da olsa 4 göz; üretim + onay `ROLLBACK` |
| Sonuç | sahtecilik tespitinde oturum `ROLLBACK`, kapı `FAILED` |

`.ai/.rules/*.md` içindeki review ayrıntısı `EKSIK_OKUMA` (üretici `.ai/.rules/coding.md` sahibidir; Bölüm 10.4).

# 6. Commit ve Kapsam

Adım 1: kendine ait dosyaları listele. Adım 2: `.decisions/.requirements/.gates` altındaysa vazgeç, `SINIR: EVET` ile bildir (`AGENTS.md` Bölüm 3.1 #2, 7.2). Adım 3: `.ai/archives/snapshots/<id>` snapshot, kopya = özgün sayı (`BACKUP_OK`).

Adım 4: `AGENTS.md` Bölüm 4.3 yedi maddesi; eksik varsa `BLOCKED_AT_GATE`. Adım 5: görev `KANIT` + bütçe güncellemesi ile kapanır. Adım 6: oturum kaydı (`AGENT/GOREV/NEDEN/SONUC/KANIT/SINIR`) güncellenir; `SINIR: EVET` ise 7 adım da işlenir.

2026-09-24 depo gerçekleri: `.gitignore` **yok**, git `main` üzerinde **0 commit** → commit mesajı şablonu, dallanma ve PR akışı depoda yok (`NEEDS_VALIDATION`). İlk commit adayları: `.gitignore`, kök 7 MD, `.ai/.decisions/`, `.ai/.rules/`, `.ai/archives/snapshots/`.

# 7. Kod Sorumluluğu (Ownership)

Dosya tek sahibe aittir; çakışmada `.ai/.agents/AGENTS.md` (yoksa `AGENTS.md` Bölüm 3.1) kazanır, çakışma `CONFLICT` satırı olarak kayda geçer. Sahiplik dosya yolu ile kanıtlanır; kanıtsız iddia `NEEDS_VALIDATION`, üretime girmez (`AGENTS.md` Bölüm 3.4). `.ai/` içinde yazma yetkisi `.decisions/.requirements/.gates` altı yasak; `.ai/requirements/` ve `.ai/gates/` klasörleri yok → yeni dosya oluşturma, yalnız `.decisions/` altına ADR adayı yaz (`AGENTS.md` Bölüm 3.1 #2, 2.2).

# 8. Kabul Koşulları (Doğrulama)

- [ ] Okuma sırası tam, eksikler `EKSIK_OKUMA`; P0 açık değil (`AGENTS.md` §2, §6.1)
- [ ] İlgili gate `PASSED` + 7 alanlı `KANIT`; üretim + onay ikiliği yok (`AGENTS.md` §4.2, §4.4)
- [ ] BACKUP_OK + sahiplik + dead code yok + bağımlılık kapısı + test planı (`AGENTS.md` §4.3)
- [ ] Bütçe 45000 eşiği altında, kalan iş yazıldı (`AGENTS.md` §5.2)
- [ ] Oturum 6 alanlı ve `KANIT` dolu; durum geçerli (`AGENTS.md` §8.1)
- [ ] ADR durumu diskteki 6 dosya ile aynı (`REPOSITORY_VERIFIED`, 2026-09-24)
- [ ] `.rules` Bölüm eşlemesi `NEEDS_VALIDATION` işareti duruyor (`AGENTS.md` §9.2)

# 9. Kapsam Dışı

UI fikri ve pazarlama metni (`README.md`), deploy ve rollback adımları (`WORKFLOW.md` GATE-008), exploit/ payload (`SECURITY.md` §7), UI kabul kriterleri (`ui-designer`) ve üçüncü taraf lisans listesi (`dependencies.md`) — bu konular kök belgelerde ele alınır; katkı öncesi oradan okunur.

# 10. Çelişkiler ve Kanıt

## 10.1 C-CONTRIB-001 (technologies.md)

`.ai/.rules/technologies.md` dosyası diskte **yok** (`.ai/.rules/` içindeki 10 dosya: agents/architecture/coding/data/dependencies/documentation/security/testing/ui/workflow). §4'deki teknoloji satırları ADR-001/003/004/005 ve `README.md` §5.4 ile uyumlu; `NEEDS_VALIDATION`.

## 10.2 C-CONTRIB-002 (review kuralı ayrıntısı)

Review kuralları `.ai/.rules/*.md` içinde ayrıntılıdır; bu dosyada yalnız özet verildi. İlgili kural `EKSIK_OKUMA` listesine eklenmelidir.

## 10.3 C-CONTRIB-003 (commit şablonu yok)

Commit/dallanma/PR kuralı hiçbir kök belgede yok (`.gitignore` ve commit yok) → `NEEDS_VALIDATION`; üretim durmaz.

## 10.4 Kanıt listesi

`REPOSITORY_VERIFIED` (2026-09-25): `.ai/.rules/` 10 dosya, `.ai/.decisions/` 7 ADR, `.ai/.agents/` VAR (21 dosya), `.ai/.templates/` VAR (33 dosya), `.ai/gates/` YOK, `.gitignore` YOK, git 0 commit. Kaynaklar: `USER-BOOTSTRAP §9`, `AGENTS.md` §3-§8, `SECURITY.md` §6-§7, ADR-001/003/004/005.
