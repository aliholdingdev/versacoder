---
title: "Hata Yönetimi Kuralları — Result<T>"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Her dönüş Result<T>; exception yalnız beklenmeyen çökme; mesaj kodları"
tags: [['rules', 'error-handling', 'result', 'hata']]
source: "Birleştirme: brain.md §3 kural 1 + §3.7/1 + ADR-019"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# Hata Yönetimi — Result<T>

## Kural

Her işlem sonucu **`Result<T>`** ile döner; exception fırlatmak yerine sonuç
nesnesi taşınır. Exception **yalnız beklenmeyen çökme** içindir (null
referans, IO çökmesi vb.) ve yakalanıp log'a düşülür (ADR-026 seviyesi: Error).

```text
IYI  : public Result<User> GetUser(int id)
       {
           var u = _db.QuerySingleOrDefault<User>(ById, new { p1 = id });
           return u is null
               ? Result<User>.Fail($"user.not_found:{id}")
               : Result<User>.Ok(u);
       }
```

## Mesaj kodu kuralları

- Kod `alan.olay[:ayrıntı]` biçimindedir (`user.not_found:99`).
- Kod **uydurulmaz**; CLI çıkışı aynı kodu gösterir (ADR-010 ortak çekirdek).

## Çökme yönetimi

- Beklenmeyen exception: yakala → structured log (Error seviyesi) → kullanıcıya
  genel hata → stack trace yalnız logda (UI'a sızmaz).

## İhlalde

RED — kod incelemesinde istisna gerekçelendirilmezse reddedilir (brain §3.9/6).

## İlgili

- [ADR-019](../.decisions/accepted/adr-019-dual-leg-build-test.md)
- [logging-rules](logging-rules.md) · [rules-index](index.md)
