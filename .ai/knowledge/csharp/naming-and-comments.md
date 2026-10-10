---
title: Isimlendirme ve Yorum Politikasi
type: knowledge
category: knowledge
status: active
version: 1.0
updated: 2026-09-29
authority: primary
scope: csharp
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Isimlendirme ve Yorum Politikasi

**Zorunlu Baglantilar:** [[knowledge/index|Knowledge Index]] | [[claude|CLAUDE]] | [[.rules/coding-standards|Kodlama Standartlari]] | [[knowledge/csharp/guard-clauses-and-method-shape|Guard Clause ve Metot Sekli]] | [[knowledge/csharp/index|C Sharp Index]]
**Küme + Hub:** [[index|index]] · [[definitions/agents|AGENTS]] · [[.decisions/brain|brain]] · [[definitions/glossary|glossary]] · [[governance/keys|keys]]
**Kardeşler (csharp · 3/28):** [[knowledge/csharp/async-await-basics|async-await-basics]] · [[knowledge/csharp/async-ui-state-flows|async-ui-state-flows]] · [[knowledge/csharp/cancellation-token-policy|cancellation-token-policy]] · [[knowledge/csharp/channel-queue-patterns|channel-queue-patterns]] · [[knowledge/csharp/collections-choice|collections-choice]] · [[knowledge/csharp/constants-and-magic-values|constants-and-magic-values]] · [[knowledge/csharp/exception-handling-patterns|exception-handling-patterns]] · [[knowledge/csharp/generics-constraints|generics-constraints]] · [[knowledge/csharp/interfaces-vs-abstract|interfaces-vs-abstract]] · [[knowledge/csharp/language-versions-net48-net10|language-versions-net48-net10]] · [[knowledge/csharp/layer-dependency-direction|layer-dependency-direction]] · [[knowledge/csharp/linq-performance-pitfalls|linq-performance-pitfalls]] · [[knowledge/csharp/multi-targeting-props|multi-targeting-props]] · [[knowledge/csharp/nullable-reference-types|nullable-reference-types]] · [[knowledge/csharp/pattern-matching|pattern-matching]] · [[knowledge/csharp/records-and-immutability|records-and-immutability]] · [[knowledge/csharp/resource-disposal-patterns|resource-disposal-patterns]] · [[knowledge/csharp/result-type-design|result-type-design]] · [[knowledge/csharp/retry-and-timeout-policy|retry-and-timeout-policy]] · [[knowledge/csharp/serialization-choices|serialization-choices]] · [[knowledge/csharp/solid-principles-applications|solid-principles-applications]] · [[knowledge/csharp/span-interop-net10|span-interop-net10]] · [[knowledge/csharp/task-vs-value-task|task-vs-value-task]] · [[knowledge/csharp/testing-determinism|testing-determinism]] · [[knowledge/csharp/threading-locking|threading-locking]]

> **Tek cumlelik sorumluluk:** Yorum ne degil neden anlatir; comment'lenmis kod silinir, public API `///` ile belgelenir, kod ici yorum Ingilizcedir.

## §0 Ozet (Answer First)

Yorum politikasi uc madde: **olu yorum yok** (commented-out kod yasak), **yorum neden yazar** (kodu tekrar eden yorum gereksiz), **dil tutarliligi** (kod ici yorum ve XML doc Ingilizce, vault prose Turkce). Test adlari dahil isimlendirme senaryoyu anlatir: `Method_Scenario_Expected`.

## §1 Karar Tablosu

| Durum | Yapilan | Yapilmayan |
| --- | --- | --- |
| Kod tekrar eden yorum | Sil | Dosyada birak |
| Neden gerekiyor | Tek satir neden yaz | Paragraf aciklama |
| Eski hal | Issue/ADR'a tasi | `// TODO eski hali` |
| Public API | `///` XML doc | Yalniz yorum satiri |
| Test adi | `FindAsync_Missing_ReturnsFailure` | `Test1` |

## §2 Iki Bacak

**Bacak 1 - Isim isin yerine gecmediginde:** Test adi senaryoyu anlatmiyorsa test belgedir, denetim degildir. Sınıf adi `<Sınıf>Tests` olur; kod dili Ingilizce (vault dili farkli).

**Bacak 2 - Yorum eskir, kod yasar:** Yorumlar kodla birlikte eskir; olu yorum yanlis yonlendirir. Neden lazimsa tek satir yaz, TODO biraktiysan issue/ADR ac - yorum degil, takip araci.

## §3 Ornek

```csharp
// YASAK: comment'lenmis eski kod
// public void OldLoad() { ... }

// IYI: neden tek satirda
// Bolum kilidi, ayni anda iki aktarimi engeller (bkz. ADR-012).
using var bolum = new SemaphoreSlim(1, 1);

/// <summary>Verilen oturumu bulur; bulunamazsa null dondurur.</summary>
public Task<Session?> FindAsync(int id, CancellationToken cancellationToken)
    => _repo.FindAsync(id, cancellationToken);
```

## §4 Hata Modlari

1. Yorumda kalan eski kod: okunamaz dosya, silinmeye korkulan blok.
2. Kodu tekrar eden yorum (`// artir` ile `count++`): gurultu, gercek neden kaybolur.
3. Karisik dil: kod ici Turkce / vault Ingilizce olunca iki arama sistemi cikar.
4. Isimsiz test (`Test1`): adim test dosyasiyla inceleme yapilamaz.

## §5 Kurallar

- Once isimle; isim yetmiyorsa tek satir neden yaz.
- Public API icin XML doc zorunlu; ic alanlar icin kisitli.
- Kod ici yorum ve doc Ingilizce; vault yazisi Turkce kalsin.
- Test adi `Yontem_Senaryo_Beklenen` biciminde olsun.
- TODO biraktiysan issue/ADR olustur; yorumu is takip araci yapma.

## §6 Yasaklar

- Commented-out kod **YASAK**; sil.
- Kodu tekrar eden yorum **YASAK**.
- `Test1` / `MyTest` gibi anlamsiz test adi **YASAK**.
- Yorumla kural savunmak (yerine kodu duzeltmek) **YASAK**.

## §7 Test Baglantisi

- Test adinin kendisi ilk assertion'dir: isim hataliysa test yanlis kanitlar.
- Yorumlanmis `[Fact]` bloklari silinir veya geri acilir; olu test = yanlis guven.
- Assertion mesaji okunur olsun; kisaltma yapma. Detay: [[.rules/testing-rules|Test Kurallari]].

## §8 İlgili Kaynaklar

- [[.rules/coding-standards|Kodlama Standartlari]] · [[knowledge/csharp/guard-clauses-and-method-shape|Guard Clause ve Metot Sekli]] · [[knowledge/csharp/assertion-quality|Assertion Kalitesi]] · [[knowledge/csharp/index|C Sharp Index]]

## §9 Kanit

- Yorum politikasi (neden/neden degil, yasak comment, dil) kodlama standartlarinin yorum maddesinde. Kaynak: [[.rules/coding-standards|Kodlama Standartlari]].
- Test adlandirma `Method_Scenario_Expected` ornegi test kural dosyasinda; tam isimlendirme listesi icin DOĞRULAMA GEREKLİ.
