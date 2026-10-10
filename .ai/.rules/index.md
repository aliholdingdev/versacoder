---
title: "rules-index — Tek Küresel Kural Seti (G1..G16) Defteri"
type: index
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Tüm kural dosyalarının indeksi; G-seti tek burada; ihlalde ne + kanıt"
tags: [rules, index, g-set, governance]
source: "Birleştirme: .ai/CLAUDE.md §1 + kök CLAUDE.md özetleri + brain.md §3 + AGENTS 12 kural (2026-10-07)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [[.rules/index|rules-index]] · [[claude|CLAUDE]] · [[.decisions/adr-index|ADR defteri]] · [[.decisions/brain|brain]] · [[governance/workflow|WORKFLOW]] · [[governance/vault-gate|VAULT-GATE]]

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
| **G1** | Vault içeriği sıfırdan yazılır; kopya yasak | [[.rules/coding-standards|coding-standards]] | RED — dosya yeniden yazılır | benzerlik taraması |
| **G2** | ADO.NET + Dapper; EF Core yok; parametreli SQL | [[.rules/data-access-rules|data-access-rules]] | CRITICAL — build RED | EF taraması = 0 |
| **G3** | DevExpress v26.1 + ribbon Office2010 | [[.rules/devexpress-rules|devexpress-rules]] | CRITICAL — RED | CPM tek DevExpress satırı |
| **G4** | Vault biçimi: 8→11 alan FM, UTF-8, 0 bozuk link, 0 mojibake | [[.rules/coding-standards|coding-standards]] | RED — biçim düzeltilir | FM taraması = 0 eksik |
| **G5** | Kanıtsız "bitti" yasak; etiket + kaynak zorunlu | [[.rules/coding-standards|coding-standards]] | RED — iş bitmedi | komut çıktısı |
| **G6** | Çift TFM: net48 + net10.0-windows; #if yalnız platform | [[.rules/testing-rules|testing-rules]] | HIGH — mimari RED | iki derleme, 0 hata |
| **G7** | Yıkıcı işlem öncesi kullanıcı onayı | [[.rules/security-rules|security-rules]] | CRITICAL — durdurulur | log onay satırı |
| **G8** | Sürüm/teknoloji sapması yasak (CPM kilidi) | [[.rules/configuration-rules|configuration-rules]] | RED → BLOCKED | CPM okuma raporu |
| **G9** | Uydurma yasak (zero-hallucination) | [[.rules/coding-standards|coding-standards]] | RED — madde silinir | etiket + kaynak satırı |
| **G10** | Çelişki: BLOCKED + tek soru; sessiz çözüm yasak | [[.rules/coding-standards|coding-standards]] | RED — karar geçersiz | BLOCKED raporu |
| **G11** | Gizli işlem yok; uzun/sessiz iş önceden raporlanır | [[.rules/coding-standards|coding-standards]] | CRITICAL | görev listesi + onay |
| **G12** | Yetki + gate disiplini: gate'i yalnız kullanıcı kapatır | [[.rules/layer-rules|layer-rules]] | RED — dosyalar geri alınır | gate kapanış kaydı |
| **G13** | Türkçe zorunlu; mojibake yasak | [[.rules/coding-standards|coding-standards]] | RED — çevrilir | mojibake taraması |
| **G14** | Yer tutucu değişken/metin kalmaz (0 adet) | [[.rules/coding-standards|coding-standards]] | RED — doldurulur | placeholder taraması |
| **G15** | Vault envanterinde SRP: tek dosya tek konu; tekrar yok | [[.rules/coding-standards|coding-standards]] | RED — bölünür/düzeltir | duplicate taraması |
| **G16** | Test: TDD, TreatWarningsAsErrors, %90 coverage hedefi | [[.rules/testing-rules|testing-rules]] | HIGH — tamamlanmadı | coverage raporu |

## §2 Dosya Haritası

| Dosya | Kapsam |
|---|---|
| [[.rules/coding-standards|coding-standards]] | G1 G4 G5 G9 G10 G11 G13 G14 G15 + genel kod stili |
| [[.rules/data-access-rules|data-access-rules]] | G2 + ADR-005/016/017 |
| [[.rules/devexpress-rules|devexpress-rules]] | G3 + ADR-004 |
| [[.rules/ui-rules|ui-rules]] | UI/UX: ribbon, ekran üçlüsü, çizim politikası (D46) |
| [[.rules/testing-rules|testing-rules]] | G6 G16 + ADR-019 |
| [[.rules/error-handling-rules|error-handling-rules]] | Result<T> + exception sınırı (brain §3/1) |
| [[.rules/async-rules|async-rules]] | async/await + iptal (DOĞRULAMA GEREKLİ bölümü var) |
| [[.rules/security-rules|security-rules]] | G7 + ADR-017/018 |
| [[.rules/solid-rules|solid-rules]] | SOLID 5+1 (brain §2) |
| [[.rules/layer-rules|layer-rules]] | G12 + ADR-028 katman yasakları |
| [[.rules/logging-rules|logging-rules]] | ADR-026 structured log |
| [[.rules/performance-guidelines|performance-guidelines]] | timeout, bütçe, ölçüm (ADR-009/027) |
| [[.rules/configuration-rules|configuration-rules]] | G8 + CPM + l6 host (ADR-014) |
| [[.rules/naming-conventions|naming-conventions]] | adlandırma (DOĞRULAMA GEREKLİ bölümü var) |
| [[.rules/accessibility-rules|accessibility-rules]] | erişilebilirlik (DOĞRULAMA GEREKLİ — brain §4.3) |
| [[.rules/caching-rules|caching-rules]] | önbellek kuralı (minyal — vault kaynağı sınırlı) |
| [[.rules/deployment-guide|deployment-guide]] | dağıtım (spec/deployment-spec.md + packaging-spec.md) |

## §3 Okuma Sırası (Context Paketleri — D11/D16)

- **Herkes (global):** bu index + [[.rules/coding-standards|coding-standards]].
- **Rol bazlı ek:** architect → [[.rules/layer-rules|layer-rules]] + [[.rules/solid-rules|solid-rules]];
  developer → [[.rules/data-access-rules|data-access]] + [[.rules/error-handling-rules|error-handling]] +
  [[.rules/async-rules|async]]; tester → [[.rules/testing-rules|testing-rules]];
  ui-designer → [[.rules/ui-rules|ui-rules]] + [[.rules/accessibility-rules|accessibility]].
- **Üstünlük:** global > rol (D16); rol dosyası global kuralı **override edemez**.

## §4 İlgili

- [[.decisions/accepted/adr-032-ai-control-plane|ADR-032]] — hedef mimari
- [[.decisions/adr-index|adr-index]] · [[.decisions/brain|brain]]
- [[governance/audit|audit]] — kural değişikliği onay kayıtları
