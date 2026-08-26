---
title: "ADR-002: MVVM Pattern with CommunityToolkit.Mvvm"
type: adr
category: architecture
date: 2026-08-25
status: accepted
version: 1.0.0
---

# ADR-002: MVVM Pattern with CommunityToolkit.Mvvm

## Durum

Kabul Edildi (Accepted)

## Bağlam

WinForms uygulamalarında kod bakımsızlığını önlemek için MVVM pattern uygulanacaktır.

## Karar

**CommunityToolkit.Mvvm** kullanılarak MVVM pattern uygulanacaktır. WinForms Code-Behind yasaktır.

## Gerekçeler

1. **MVVM:** Separation of concerns
2. **CommunityToolkit.Mvvm:** Modern, performanslı, .NET 8 uyumlu
3. **BindableBase:** INotifyPropertyChanged otomatik implementasyonu
4. **RelayCommand:** Komut yönetimi

## Sonuçlar

### Olumlu
- Bakım kolaylığı
- Test edilebilirlik
- Kod tekrarının önlenmesi

### Olumsuz
- Öğrenme eğrisi
- WinForms'ta MVVM sınırlamaları

## Yasak Örüntüleri

| ❌ Yasak | ✅ Doğru |
|----------|----------|
| WinForms Code-Behind | MVVM + CommunityToolkit.Mvvm |
| Doğrudan DOM erişimi | BindableBase + INotifyPropertyChanged |
| `public` alanlar | `{ get; set; }` encapsulation |
