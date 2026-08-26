---
title: "ADR-003: SQLite Database with WAL Mode"
type: adr
category: database
date: 2026-08-25
status: accepted
version: 1.0.0
---

# ADR-003: SQLite Database with WAL Mode

## Durum

Kabul Edildi (Accepted)

## Bağlam

Versa Coder, yerel veritabanı gerektiren bir IDE platformudur. Veritabanı teknolojisi seçimi, performans ve taşınabilirlik açısından kritiktir.

## Karar

**SQLite** veritabanı **WAL (Write-Ahead Logging)** modunda kullanılacaktır. ORM olarak **EF Core (DbContext ONLY)** kullanılacaktır.

## Gerekçeler

1. **SQLite:** Sıfır kurulum, dosya tabanlı, taşınabilir
2. **WAL Mode:** Eşzamanlı okuma/yazma desteği, performans artışı
3. **EF Core:** Modern ORM, LINQ desteği, migration sistemi
4. **DbContext ONLY:** Safe pattern, Bağımlılık enjeksiyonu

## Sonuçlar

### Olumlu
- Sıfır kurulum maliyeti
- Yüksek performans (WAL)
- Taşınabilirlik

### Olumsuz
- Eşzamanlı yazma sınırlamaları
- Büyük veri setlerinde performans düşüklüğü

## Yasak Örüntüleri

| ❌ Yasak | ✅ Doğru |
|----------|----------|
| Manuel Connection String | IConfiguration |
| MySQL/PostgreSQL | SQLite (WAL) |
| Manuel SQL sorgusu | EF Core LINQ |
