---
title: "Hata Yönetimi Kuralları — Result<T>"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "Her dönüş Result<T>; exception yalnız beklenmeyen çökme; mesaj kodları"
tags: [rules, error-handling, result, hata]
source: "Birleştirme: brain.md §3 kural 1 + §3.7/1 + ADR-019"
supersedes: ""
---


**Zorunlu Bağlantılar:** [[.rules/index|rules-index]] · [[claude|CLAUDE]] · [[.decisions/adr-index|ADR defteri]] · [[.decisions/brain|brain]] · [[governance/workflow|WORKFLOW]] · [[governance/vault-gate|VAULT-GATE]]

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

- [[.decisions/accepted/adr-019-dual-leg-build-test|ADR-019]]
- [[.rules/logging-rules|logging-rules]] · [[.rules/index|rules-index]]
