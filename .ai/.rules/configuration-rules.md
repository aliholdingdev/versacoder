---
title: "Yapılandırma Kuralları (G8)"
type: rule
category: ".rules"
status: active
version: 1.0.0
updated: 2026-10-07
authority: primary
scope: "CPM kilidi, appsettings l6, çift TFM ayarları"
tags: [['rules', 'configuration', 'cpx', 'g8', 'packages']]
source: "Birleştirme: .ai/CLAUDE.md G8 + ADR-014 + brain.md §1/014"
supersedes: ""
---


**Zorunlu Bağlantılar:** [rules-index](index.md) · [CLAUDE](../claude.md) · [ADR defteri](../.decisions/adr-index.md) · [brain](../.decisions/brain.md) · [WORKFLOW](../governance/workflow.md) · [VAULT-GATE](../governance/vault-gate.md)

# Yapılandırma Kuralları (G8)

## G8 — Teknoloji yığını kilidi

Yığın: C# · WinForms · DevExpress v26.1 · Dapper · xUnit + NSubstitute +
NetArchTest · **CPM**. Alternatif ORM / JS framework / "geçici paket" = RED →
`DECISION BLOCKED` (G10) + kullanıcı onayı.

## CPM (ADR-014)

- Paket sürümleri **yalnız** `Directory.Packages.props`.
- `.csproj`'larda `Version` satırı **= 0**.
- Çözüm: `VersaCoder.slnx`.

## Uygulama ayarları

- `appsettings*.json` **yalnız l6 Host** katmanında okunur; alt katmanlara
  `IOptions`/port ile taşınır (ADR-028).
- Ayar anahtarları sabittir; magic string `l6`'da bir kez tanımlanır.

## İhlalde

RED → BLOCKED; paket onaysız girmez (G7 paraleli: sürüm değişimi = onay).

## Kanıt

- CPM okuma raporu: onaysız satır = 0
- `.csproj` içinde `Version` = 0 araması

## İlgili

- [ADR-014](../.decisions/accepted/adr-014-slnx-and-cpm.md)
- [layer-rules](layer-rules.md)
