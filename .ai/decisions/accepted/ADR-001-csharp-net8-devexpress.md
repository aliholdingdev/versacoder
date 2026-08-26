---
title: "ADR-001: C# .NET 8 + DevExpress WinForms"
type: adr
category: architecture
date: 2026-08-25
status: accepted
version: 1.0.0
---

# ADR-001: C# .NET 8 + DevExpress WinForms

## Durum

Kabul Edildi (Accepted)

## Bağlam

Versa Coder, profesyonel yazılım geliştiriciler için AI destekli bir IDE platformu olarak tasarlanmaktadır. UI teknolojisi seçimi, platformun başarısı için kritik öneme sahiptir.

## Karar

Versa Coder, **C# .NET 8** ve **DevExpress 2026 Universal WinForms** kullanılarak geliştirilecektir.

## Gerekçeler

1. **DevExpress Ribbon UI:** Office tarzı profesyonel arayüz
2. **WinForms:** Olgun ekosistem, geniş topluluk desteği
3. **.NET 8:** Modern C#, performans, LTS desteği
4. **DevExpress:** Zengin kontrol seti (grid, editor, docking, ribbon)

## Sonuçlar

### Olumlu
- Profesyonel görünüm
- Hızlı geliştirme
- Geniş kontrol seti

### Olumsuz
- DevExpress lisans maliyeti
- Sadece Windows desteği

## İlgili ADR'ler

- ADR-002: MVVM Pattern
- ADR-003: SQLite Database
