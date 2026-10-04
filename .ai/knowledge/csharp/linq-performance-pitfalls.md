---
title: LINQ Performans Tuzaklari
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-10-04
authority: SSOT
scope: csharp
---

# LINQ Performans Tuzaklari

**Zorunlu Baglantilar:** [Knowledge Index](../index.md) | [CLAUDE](../../CLAUDE.md) | [Anahtarlar](../../keys.md) | [Test Kurallari](../../.rules/testing-rules.md) | [Hata Kurallari](../../.rules/error-handling-rules.md) | [C Sharp Index](index.md)

> **Tek cumlelik sorumluluk:** LINQ satirlarini olcum yapmadan "hizli" varsayma; ToList komutlari ve sorgu govdesi dort ana tuzakta toplanir.

## §0 Ozet (Answer First)

LINQ'nun dort tuzagi: **kotu ToList** (sorgu disinda sarmalayip iki kez calistirir), **gorunmez kod** (Where ilk nesnede calisir), **Where-in-Where** (satirda fatura), **raydaki ters sorgu** (comparer'i yanlis kullanmak). Kural basit: liste islemlerinde LINQ kullan, veriye esit sarmalayici olarak algila, olcum etmeden elle optimizasyon yazma.

## §1 Karar Tablosu

| Tuzak | Belirti | Sonuc |
| --- | --- | --- |
| KOTU ToList | `ToList().Any()` / `ToList().Count()` | Tam koleksiyon kopyalanir |
| Gorunmez kod | `if (x.Any()) { foreach (x) }` | Sorgu iki kez calisir |
| Where-in-Where | Karmasik Where ifadeleri | Satirda fatura |
| Raydaki ters sorgu | Yanlis comparer | Yanlis siralama / bulamama |
| Elle optimizasyon | Karmasik dongu | Hata riski |

## §2 Iki Bacak

**Bacak 1 - Sunucu tarafinda filtrele:** Veritabani sorgusunda yapilabilen islemi istemciye getirme. `SELECT *` + bellekte `Where` yerine `WHERE` yaz. Ayni kural dosya okumada da gecerli.

**Bacak 2 - Tek degerde sarmalayici yok:** `Any()`, `Count()`, `First()` gibi tek deger donduruculer tek basina sorun degildir; tuzak onlari `ToList()` ile sarmalamaktir. Sarmalayici gerekliyse tek sefer cekip sonraki adimlarda bellekten calistir.

## §3 Ornek

```csharp
// KOTU: iki kez calisir, tam kopya
if (items.ToList().Any(x => x.IsActive))
{
    foreach (var it in items.ToList())
        Use(it);
}

// IYI: tek sorgu, tek kopya
var active = items.Where(x => x.IsActive).ToList();
if (active.Count > 0)
{
    foreach (var it in active)
        Use(it);
}

// IYI: tek deger, sarmalayici yok
var hasAny = items.Any(x => x.IsActive);
```

## §4 Hata Modlari

1. Sarmalayici her dongude yeniden hesaplanir; bellek ve sure katlanir.
2. Raydaki ters sirket comparer'i (a-b) - (b-a) sarmalayarak yanlis siralar; Top-1 secimi yanlis kaydi verir.
3. Gorunmez kod nedeniyle profil cikmasi beklenmedik yerden cikar; hata "LINQ yavas" diye raporlanir.
4. Karmasik Where ifadeleri yanlis yorumlanir; filtre disari tasi emniyetli olur.

## §5 Kurallar

- Liste islemlerinde LINQ kullan; el yazmi dongu yalnizca kritik yol olculeyince yazilir.
- Sorguyu veriye esit sarmalayici olarak algila; gorunmez kod yazma.
- `ToList()` ihtiyacini tek adimda gider; ard arda sarmalayici biriktirme.
- Profil vermeden elle optimizasyon yazma; once olc, sonra karar ver.
- Sorgu govdesinde karmasiklasan Where'yi disari tasi.

## §6 Yasaklar

- `ToList().Any()` / `ToList().Count()` / `ToList().First()` **YASAK**; dogrudan `Any()` / `Count()` / `First()` kullan.
- Dongu icinde tekrarlanan sorgu **YASAK**; tek seferde sarmala.
- Olcmeden "LINQ yavas" karari **YASAK**; DOGRULAMA GEREKLİ.
- Raydaki comparer'de tersi ters **YASAK**; tek sarmalayici kullan.

## §7 Test Baglantisi

- Ayni girdi iki yolda ayni dondurme testi tek ifade.
- Filtre sonucu `Assert.AreEqual(1, sayi)` ile kilitlenir.
- Hata durumu try-catch icinde emniyetle test edilir. Detay: [Test Kurallari](../../.rules/testing-rules.md).

## §8 İlgili Kaynaklar

- [Hata Kurallari](../../.rules/error-handling-rules.md) · [C Sharp Index](index.md) · [Knowledge Index](../index.md) · [ADR Index](../../.decisions/adr-index.md)

## §9 Kanit

- Karar: sarmalayici yerine dogrudan tek deger; gorunmez kod yasak. Kaynak: kaynak vault C# seti, 2026-09-25; Buyuk degisiklik 2026-09-29.
- Tuzak listesi dort ana baslikta toplandi. Kaynak: ayni set; olcum ONCESI elle optimizasyon yasagi dahil.
