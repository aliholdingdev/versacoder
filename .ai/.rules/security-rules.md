---
title: "Güvenlik Kuralları (G7)"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Onay zinciri, secret yok (ADR-018), parametreli SQL köprüsü, DPAPI"
tags: [['rules', 'security', 'secrets', 'g7', 'approval']]
source: "Birleştirme: .ai/CLAUDE.md G7 + ADR-017/018 + D17 (vault salt referans)"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# Güvenlik Kuralları (G7)

## G7 — Yıkıcı işlem onayı

Silme / overwrite / `reset --hard` / `push --force` **öncesi** kullanıcı onayı
zorunlu. Sessiz `Remove-Item -Recurse` yasak. Onay `log.md`'ye tarihli satır
olarak düşer (D12 audit).

## Secret (ADR-018 · D17)

- API anahtarı / token / parola / connection string **vault'ta yaşamaz**;
  vault **salt referans**tır.
- `keys.md` yalnız tür + saklama yeri haritasıdır; **değer içermez**.
- Saklama: **DPAPI** şifreli yerel depo. Repoda/koda anahtar = otomatik RED.
- Tarama: `secret-leak-check` + `security-review-checklist`.

## SQL enjeksiyon köprüsü

Parametreli SQL zorunlu (G2/ADR-017); bu dosyada tekrarlanmaz —
[data-access-rules](data-access-rules.md) tek kaynaktır (SRP).

## İhlalde

G7: CRITICAL — işlem durdurulur. Secret: RED — anahtar rotasyonu önerilir.

## Kanıt

- `keys.md` içinde değer = 0 · repoda anahtar = 0
- onay satırı: `log.md` tarihli G7 kaydı

## İlgili

- [ADR-018](../.decisions/accepted/adr-018-secret-dpapi-keys-map.md)
- [data-access-rules](data-access-rules.md)
