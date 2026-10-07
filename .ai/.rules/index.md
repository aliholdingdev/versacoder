---
title: "rules-index — Tek Küresel Kural Seti (G1..G16) Defteri"
type: index
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Tüm kural dosyalarının indeksi; G-seti tek burada; ihlalde ne + kanıt"
tags: [['rules', 'index', 'g-set', 'governance']]
source: "Birleştirme: .ai/CLAUDE.md §1 + kök CLAUDE.md özetleri + brain.md §3 + AGENTS 12 kural (2026-10-07)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# rules-index — Tek Küresel Kural Seti

> **Tek cümlelik sorumluluk:** VersaCoder vault'u ve kodu için **tek** kural
> setinin (G1..G16) hangi dosyada yaşadığını ve her kuralın kanıtını söyler.
> **Çakışmada bu dosyadaki G-seti kazanır** (D20); başka yerdeki yerel G
> numaralandırması geçersizdir.

## §0 Özet (Answer First)

- **Tek G-seti: G1..G16** — tam metin bu klasördeki dosyalarda; kök CLAUDE.md
  ve `.ai/CLAUDE.md` yalnız **özet + bağlantı** verir (D20).
- **Authority zinciri:** ADR > Kural (bu klasör) > Gate > Türev (D5).
- Kural = davranış kuralı; ADR = mimari karar; bilgi ≠ kural (D9 ayrımı).

## §1 G-Seti Defteri

| G | Kural | Yaşadığı dosya | İhlalde | Kanıt |
|---|-------|----------------|----------|-------|
| **G1** | Vault içeriği sıfırdan yazılır; kopya yasak | [coding-standards](coding-standards.md) | RED — dosya yeniden yazılır | benzerlik taraması |
| **G2** | ADO.NET + Dapper; EF Core yok; parametreli SQL | [data-access-rules](data-access-rules.md) | CRITICAL — build RED | EF taraması = 0 |
| **G3** | DevExpress v26.1 + ribbon Office2010 | [devexpress-rules](devexpress-rules.md) | CRITICAL — RED | CPM tek DevExpress satırı |
| **G4** | Vault biçimi: 8→11 alan FM, UTF-8, 0 bozuk link, 0 mojibake | [coding-standards](coding-standards.md) | RED — biçim düzeltilir | FM taraması = 0 eksik |
| **G5** | Kanıtsız "bitti" yasak; etiket + kaynak zorunlu | [coding-standards](coding-standards.md) | RED — iş bitmedi | komut çıktısı |
| **G6** | Çift TFM: net48 + net10.0-windows; #if yalnız platform | [testing-rules](testing-rules.md) | HIGH — mimari RED | iki derleme, 0 hata |
| **G7** | Yıkıcı işlem öncesi kullanıcı onayı | [security-rules](security-rules.md) | CRITICAL — durdurulur | log onay satırı |
| **G8** | Sürüm/teknoloji sapması yasak (CPM kilidi) | [configuration-rules](configuration-rules.md) | RED → BLOCKED | CPM okuma raporu |
| **G9** | Uydurma yasak (zero-hallucination) | [coding-standards](coding-standards.md) | RED — madde silinir | etiket + kaynak satırı |
| **G10** | Çelişki: BLOCKED + tek soru; sessiz çözüm yasak | [coding-standards](coding-standards.md) | RED — karar geçersiz | BLOCKED raporu |
| **G11** | Gizli işlem yok; uzun/sessiz iş önceden raporlanır | [coding-standards](coding-standards.md) | CRITICAL | görev listesi + onay |
| **G12** | Yetki + gate disiplini: gate'i yalnız kullanıcı kapatır | [layer-rules](layer-rules.md) | RED — dosyalar geri alınır | gate kapanış kaydı |
| **G13** | Türkçe zorunlu; mojibake yasak | [coding-standards](coding-standards.md) | RED — çevrilir | mojibake taraması |
| **G14** | Yer tutucu değişken/metin kalmaz (0 adet) | [coding-standards](coding-standards.md) | RED — doldurulur | placeholder taraması |
| **G15** | Vault envanterinde SRP: tek dosya tek konu; tekrar yok | [coding-standards](coding-standards.md) | RED — bölünür/düzeltir | duplicate taraması |
| **G16** | Test: TDD, TreatWarningsAsErrors, %90 coverage hedefi | [testing-rules](testing-rules.md) | HIGH — tamamlanmadı | coverage raporu |

## §2 Dosya Haritası

| Dosya | Kapsam |
|---|---|
| [coding-standards](coding-standards.md) | G1 G4 G5 G9 G10 G11 G13 G14 G15 + genel kod stili |
| [data-access-rules](data-access-rules.md) | G2 + ADR-005/016/017 |
| [devexpress-rules](devexpress-rules.md) | G3 + ADR-004 |
| [ui-rules](ui-rules.md) | UI/UX: ribbon, ekran üçlüsü, çizim politikası (D46) |
| [testing-rules](testing-rules.md) | G6 G16 + ADR-019 |
| [error-handling-rules](error-handling-rules.md) | Result<T> + exception sınırı (brain §3/1) |
| [async-rules](async-rules.md) | async/await + iptal (DOĞRULAMA GEREKLİ bölümü var) |
| [security-rules](security-rules.md) | G7 + ADR-017/018 |
| [solid-rules](solid-rules.md) | SOLID 5+1 (brain §2) |
| [layer-rules](layer-rules.md) | G12 + ADR-028 katman yasakları |
| [logging-rules](logging-rules.md) | ADR-026 structured log |
| [performance-guidelines](performance-guidelines.md) | timeout, bütçe, ölçüm (ADR-009/027) |
| [configuration-rules](configuration-rules.md) | G8 + CPM + l6 host (ADR-014) |
| [naming-conventions](naming-conventions.md) | adlandırma (DOĞRULAMA GEREKLİ bölümü var) |
| [accessibility-rules](accessibility-rules.md) | erişilebilirlik (DOĞRULAMA GEREKLİ — brain §4.3) |
| [caching-rules](caching-rules.md) | önbellek kuralı (minyal — vault kaynağı sınırlı) |
| [deployment-guide](deployment-guide.md) | dağıtım (spec/deployment-spec.md + packaging-spec.md) |

## §3 Okuma Sırası (Context Paketleri — D11/D16)

- **Herkes (global):** bu index + [coding-standards](coding-standards.md).
- **Rol bazlı ek:** architect → [layer-rules](layer-rules.md) + [solid-rules](solid-rules.md);
  developer → [data-access](data-access-rules.md) + [error-handling](error-handling-rules.md) +
  [async](async-rules.md); tester → [testing-rules](testing-rules.md);
  ui-designer → [ui-rules](ui-rules.md) + [accessibility](accessibility-rules.md).
- **Üstünlük:** global > rol (D16); rol dosyası global kuralı **override edemez**.

## §4 İlgili

- [ADR-032](../.decisions/accepted/adr-032-ai-control-plane.md) — hedef mimari
- [adr-index](../.decisions/adr-index.md) · [brain](../.decisions/brain.md)
- [audit](../governance/audit.md) — kural değişikliği onay kayıtları
