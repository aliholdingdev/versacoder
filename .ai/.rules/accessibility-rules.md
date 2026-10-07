---
title: "Erişilebilirlik Kuralları"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "WinForms/DevExpress erişilebilirliği; DOĞRULAMA GEREKLİ bölümü var"
tags: [['rules', 'accessibility', 'a11y', 'ui']]
source: "Türetme: brain §4.3/5 (kural metni vault'ta YOK -> DOĞRULAMA GEREKLİ) + ui-design kapsamı; tam metin beklemede"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# Erişilebilirlik Kuralları

> **DOĞRULAMA GEREKLİ:** brain.md §4.3 madde 5 — "accessibility-rules metni
> vault'ta YOK" olarak işaretlenmişti. Aşağıdaki maddeler vault'taki ekran
> tanımlarından (`ui-design/`) + genel WCAG pratiklerinden **türetilmiştir**
> (G9). Tam vault kaynağı (ör. eksik checklist orijinali) bulunursa bu dosya
> o kaynağa göre güncellenir.

## Temel kurallar

1. **Klavye erişimi:** tüm etkileşimli öğeler sekme sırasına girilebilir;
   `Tab`/`Shift+Tab` kırılmaz; odak görseli görünür.
2. **Kontrast:** metin/arka plan WCAG AA (4.5:1 normal metin); ribbon
   stilleri `Office2010` paleti içinde kalır (G3).
3. **Adlandırma:** her editör/label'ın `AccessibleName` değeri dolu olur;
   boş `AccessibleName` = RED.
4. **Hata durumu:** yalnız renkle değil metinle de bildirilir
   (error-handling kodları UI'a metin olarak yansır).
5. **Boyut/tarayıcı:** ekran elementleri `ui-design/` wireframe'lerindeki
   ölçüleri korur; DPI değişimi bozulma üretmez.

## İhlalde

RED — tasarım/tasarımcı turu; GATE-003'te yakalanır.

## İlgili

- [ui-rules](ui-rules.md) · [rules-index](index.md)
