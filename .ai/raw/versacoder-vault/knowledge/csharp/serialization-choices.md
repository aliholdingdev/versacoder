---
title: "serialization-choices - VersaCoder"
type: knowledge
category: csharp
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "JSON / XML / veri transferi secimi: System.Text.Json, XmlSerializer, BinaryFormatter yasagi, surum uyumlulugu ve net48 + net10.0-windows bacaklarindaki paket farki"
---

# serialization-choices — Serileştirme Seçimi

**Zorunlu Baglantilar:** [vault index](../../index.md) · [knowledge index](../index.md) · [CLAUDE.md](../../CLAUDE.md) · [coding-standards](../../.rules/coding-standards.md) · [security-rules](../../.rules/security-rules.md) · [records-and-immutability](records-and-immutability.md) · [pattern-matching](pattern-matching.md)

> **Tek cümlelik sorumluluk:** *Bir nesne disariya nasil cikarilir (JSON mu, XML mi), hangi kutuphane secilir ve surum degisince ne kirilir* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Bu vault'ta varsayılan **JSON + `System.Text.Json`**tur: tip güvenli, kaynak üreteci (source generator) ile yansıma (reflection) olmadan çalışır, XML gereksinim yoksa tercih edilir. `BinaryFormatter` **yasaktır** (güvenlik: deserializasyon saldırı yüzeyi) — bkz. [security-rules](../../.rules/security-rules.md). Şema **ileriye dönük uyumlu** kurulur: yeni alan **opsiyonel + varsayılanlı** eklenir, eski alan silinmez, enum'a değer **sondan** eklenir. Dosyaya/`net48`'e yazılan biçim **sürüm alanı** (`schemaVersion`) taşır.

| # | Biçim | Ne zaman | Kütüphane | Yasak mı |
|---|---|---|---|---|
| 1 | JSON | ayarlar, API gövdesi, dosya paylaşımı | `System.Text.Json` | — |
| 2 | XML | dış sistem zorunlu kılıyorsa | `XmlSerializer` | — |
| 3 | Nesne → XML (veri sözleşmesi) | WCF benzeri katı şema | `DataContractSerializer` | — |
| 4 | İkili (ikili nesne grafiği) | hiçbir yerde | `BinaryFormatter` · `NetDataContractSerializer` | **EVET** |
| 5 | CSV (tablo) | kullanıcı dışa aktarımı | elle yazılır (Dapper sonuçları) | — |

---

## §1 Karar akışı

```
Nesne disari mi cikiyor?
   |
   +-- evet -> dis sistem JSON istiyor mu? -- EVET -> System.Text.Json
   |                    |
   |                    +-- HAYIR -> dis sistem XML zorunlu mu? -- EVET -> XmlSerializer
   |                                                    |
   |                                                    +-- HAYIR -> JSON (varsayilan)
   +-- hayir -> dosya ya da bellek ici gecis mi? -> yine JSON (imzali/gizli alan yok)
```

| Soru | Cevap | Sonuç |
|---|---|---|
| Tip güvenli mi olmalı | evet | generic `Deserialize<T>` — `object` yerine somut tip |
| Kaynak üretilsin mi | evet | source generator; yansıma yasağı olan ortamda şart |
| Hassas alan var mı (`password`, `token`) | evet | `JsonIgnore` + asla dosyaya yazma |
| Sürüm geçmişi var mı | evet | `schemaVersion` alanı + okuma tarafında `switch` |
| `DateTime` / `decimal` / enum | dikkat | ISO 8601 tarih, invariant kültür, enum **sayısal** yazılır |

---

## §2 Kod (JSON, iki biçim de aynı dosyada)

```csharp
// --- paylasilan dosya: C# 7.3 uyumlu yazim (net48 bacakinda da derlenir) ---
private static readonly JsonSerializerOptions Opt = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = false,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
};

public static string Serialize<T>(T value)
{
    return JsonSerializer.Serialize(value, Opt);
}

public static T Deserialize<T>(string json)
{
    T result = JsonSerializer.Deserialize<T>(json, Opt);
    if (result == null)
        throw new InvalidDataException("Bos govde: " + typeof(T).Name);  // fail-fast
    return result;
}
```

| Sık hata | Belirti | Düzeltme |
|---|---|---|
| `object` hedefi ile okuma | çalışma zamanı sürprizi, tip kaybı | `Deserialize<T>` |
| Kültüre bağlı sayı/tarih | TR'de `1,5`, EN'de `1.5` farkı | invariant CultureInfo + ISO 8601 |
| Private alan / readonly nesne | alanlar boş döner | `init`/`set` property ya da kaynak üreteci |
| Enum'u metin yazıp sayı beklemek | `JsonException` | sözleşmeyi tek tarafta sabitle (sayısal) |
| Hassas alanın dosyaya yazılması | sızıntı | `JsonIgnore` + dosya izinleri (`security-rules`) |

---

## §3 Sürüm uyumlulugu (şema kuralı)

| Değişiklik | Yapılır mı | Neden |
|---|---|---|
| Yeni alan ekleme (`= varsayılan`) | **evet** | eski okuyucu yok sayar |
| Alanı silme | hayır | eski istemciler kırılır |
| Alan adını değiştirme | hayır | aynı etki (yeniden adlandırma = silme) |
| Enum'a değer ekleme | evet, **sondan** | sıra korunur |
| Enum'dan değer çıkarma | hayır | okuyucu bilinmeyen değeri düşer |
| Alan tipini değiştirme (`int` → `long`) | hayır | `JsonException` / veri kaybı |

---

## §4 net48 / net10.0-windows farki

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `System.Text.Json` | **NuGet paketi** gerekir | runtime ile gelir, paket eklenmez |
| Kaynak üreteci (`JsonSerializerContext`) | paket sürümü destekliyorsa çalışır; **C# 9+** dil sürümü şart (net48 varsayılanı 7.3 → `LangVersion` gerekir) | doğrudan |
| `BinaryFormatter` | teknik olarak var — **kullanmak yasak** | kaldırılmış/ağır kısıtlı (yasağın adı geçerli) |
| `XmlSerializer` | var | var (aynı imza) |
| Dil | **C# 7.3**: `using var` yok, target-typed `new` yok | C# 14 |

> Paylaşılan dosyada source generator **kullanılmaz**; kaynak üretilen türler bacak başına ayrı `.cs`'te kalır, `#if` yalnız API varlığı içindir (G6).

---

## §5 Kardesler

| Konu | Dosya |
|---|---|
| Değiştirilemez veri modeli | [records-and-immutability](records-and-immutability.md) |
| Okuma dalı / tip eşleme | [pattern-matching](pattern-matching.md) |
| Dil sürümü farkları | [language-versions-net48-net10](language-versions-net48-net10.md) |
| Hassas veri kuralları | [security-rules](../../.rules/security-rules.md) |

---

## §6 Yasaklar

1. `BinaryFormatter` · `NetDataContractSerializer` · deserializasyonda `TypeNameHandling.Auto` benzeri tip-yönlendirme (G2/CRITICAL).
2. Serileştirilmiş metne **çözülmemiş** ham SQL/dosya yolu yazma; veri erişimi **ADO.NET + Dapper**, sorgu `@p1` parametreli.
3. EF Core ile model serileştirme (`DbContext` yasak).
4. Şemada **alan silme / yeniden adlandırma** (ileriye dönük kırılma).
5. Kanıtsız "biçim uyumlu" iddiası (G5): kanıt = eski örnek dosyanın yeni kodla okunduğu test çıktısı.
