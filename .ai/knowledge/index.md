---
title: "Bilgi İndeksi — VersaCoder knowledge/"
type: index
category: knowledge
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Kanitli ogrenimlerin (DERS/LESSON) yazildigi yer: kabul kriteri, ornek basliklar ve kanit zorunlulugu - kanitsiz ders RED"
---

# Bilgi İndeksi (knowledge/)

**Zorunlu Bağlantılar:** [CLAUDE](../CLAUDE.md) · [keys](../keys.md) · [adr-index](../.decisions/adr-index.md) · [Rapor indeksi](../reports/index.md)
**Küme + Hub:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [brain](../brain.md) · [glossary](../glossary.md) · [keys](../keys.md)

> **Tek cümlelik sorumluluk:** "Bu vault'ta kanıtlanmış ders nerede yaşar, nasıl yazılır, kabul edilir?" sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`knowledge/` yalnız **kanıtlanmış öğrenimler** (DERS / LESSON) yazar: bir şey denendi, çıktısı ölçüldü, sonra tek satır ders olarak buraya konur. **Kanıt yoksa ders yazılmaz** — `DOĞRULAMA GEREKLİ` etiketiyle `reports/10-bilinen-eksikler.md`'ye düşer. Karar (ne yapılacağı) burada değil, [adr-index](../.decisions/adr-index.md) altındaki ADR'lerde yaşar; kural (ne yapılacağı zorunlu) `.rules/` içinde yaşar — bu dosya yalnız "ne öğrendik"i anlatır. Klasör durumu (2026-09-29): `knowledge/` = 0 dosya, bu indeks ilk kayıttır.

---

## §1 Ne Tur Bilgi Buraya Yazılır

| # | Yazılır | Yazılmaz | Nereye gider |
|---|---|---|---|
| 1 | Ölçülmüş ders: "X denendi, Y çıktısı alındı, Z dersi çıkarıldı" | denemeden çıkarılmış tahmin | `NEEDS_VALIDATION` → `reports/10-bilinen-eksikler.md` |
| 2 | Tekrar eden hata ve kesin düzeltmesi (belge + satır + komut çıktısı) | tek seferlik ufak typo | ilgili dosyada yerinde düzeltme |
| 3 | Araç/komut beklenen vs gerçek çıktı farkı | "genellikle çalışır" cümlesi | `scripts/*.md` + bu klasör |
| 4 | Mimari/teknoloji dersi (ADR'yi **gerekçelendiren** kanıt) | ADR'nin kendisi (tekrar olur) | karar → [adr-index](../.decisions/adr-index.md) |
| 5 | Ders özeti tek satır; detay kanıt tablosunda | kanıtsız "bitti/tamam" | G5 ihlali → RED |

**Sınır testi (ilk EVET yeter):** "Bu cümlede ölçüm/komut çıktısı/dosya+satır var mı?" → EVET = knowledge; "Karar vermem mi gerekiyor?" → EVET = ADR; "Bunu bundan sonra zorunlu yapmam mı gerekiyor?" → EVET = `.rules/`.

---

## §2 Örnek Başlıklar (REZERVE — dosyalar henüz yok)

| # | Örnek dosya adı | Ne öğretir | Kanıt türü |
|---|---|---|---|
| 1 | `dual-tfm-derleme-dersleri.md` | `net48` ile `net10.0-windows` arasında derleme/uyarlama farkları | derleme çıktısı (satır no + uyarı kodu) |
| 2 | `dapper-parametreli-sorgu-dersleri.md` | ADO.NET + Dapper'da `@pN` kullanımı ve kaçınma desenleri | sorgu + plan çıktısı |
| 3 | `devexpress-v261-office2010-dersleri.md` | DevExpress v26.1 + `RibbonControlStyle.Office2010` uygulama dersleri | ekran + kod satırı |
| 4 | `gate-kapanis-kanit-dersleri.md` | Gate kapanışında hangi kanıtın yettiği/yetmediği | gate raporu satırı |

> Bu dört ad **rezervedir**: dosya henuz uretilmedi; bu tablo link degil, yazi olarak durur (bozuk link = G7).

---

## §3 Kabul Kriteri (kanıt zorunlu)

| # | Kriter | Sağlanmazsa |
|---|---|---|
| 1 | Her dersin en az **1 kanıtı** var: komut çıktısı, dosya yolu + satır no veya ölçüm tablosu | dosya üretilmez |
| 2 | Kanıt ölçümlü gelir: sayı + tarih + ölçüm yeri ("9 dosya, 2026-09-29, dizin listesi") | `DOĞRULAMA GEREKLİ` |
| 3 | Ders tek satır özet + kanıt tablosu; tekrar eden alıntı yok (SSUT/G12) | ilgili ADR/.rules'a taşınır |
| 4 | Yararı ölçülebilir: aynı hata tekrarladığında bu dosya mı kullanılıyor? | dosya silinir, kanıt `log.md`'ye döner |
| 5 | Türkçe, UTF-8, emoji yok, frontmatter 8 alan | G3/G5 RED |

**Ders kalıbı (tek satır):** `Ne denendi -> ne ölçüldü -> ne öğrendik -> hangi dosya değişti`.

---

## §4 Yazım Akışı

| # | Adım | Çıktı | Süre |
|---|---|---|---|
| 1 | Kanıt topla (komutu çalıştır, çıktıyı al) | ham çıktı | 5 dk |
| 2 | Dosya adını §2 tablosundan/rezervden seç | `NN-konu-dersleri.md` | 1 dk |
| 3 | Frontmatter 8 alan + tek cümlelik sorumluluk + §0 | iskelet | 4 dk |
| 4 | Kanıt tablosunu yaz (iddia · durum · kanıt) | §1 tablosu | 6 dk |
| 5 | Bu indekse 1 satır ekle | §2/§6 kaydı | 2 dk |

- [ ] Kanıt satırı dolu (boş ders = RED)
- [ ] Bu indekste dosya kaydı var
- [ ] Karar/kural metni kopyalanmadı (link verildi)
- [ ] `check-links.ps1` → 0 kırık

---

## §5 Kayıtlı Boşluk (dürüstlük notu)

| # | Dosya | Durum |
|---|---|---|
| 1 | Kök `.ai/index.md` | 2026-09-29'da diskte YOK → link yazılmadı, düz metin |
| 2 | `.ai/MEMORY.md` | 2026-09-29'da diskte YOK → link yazılmadı, düz metin |
| 3 | `.ai/.decisions/adr-index.md` | VAR → link aktif |
| 4 | `knowledge/` alt dersleri | 0 dosya — §2'deki 4 ad rezerve |

---

## §6 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Karar indeksi | `.ai/.decisions/adr-index.md` | ders ADR'ye dönüşürse gidilecek yer |
| 2 | Kanıt/G5 kuralı | `.ai/CLAUDE.md` | kanıtsız "bitti" yasağı |
| 3 | Anahtar haritası | `.ai/keys.md` | ders anahtarları |
| 4 | Eksikler defteri | `.ai/reports/10-bilinen-eksikler.md` | doğrulanmamış maddeler |
| 5 | Rapor indeksi | `.ai/reports/index.md` | ham ölçümler burada |

---

## §7 Küme Haritasi (5 alt dizin · 131 dosya — olcum 2026-10-07)

> Bu tablo **hub**tur: her küme satiri o klasorun `index.md` dosyasina gider;
> oradan o klasordeki tum dosyalara ulasilir. Dosyadan diger kümeye gecis:
> dosya -> bu hub -> hedef küme index -> hedef dosya (**2 adim**).

| # | Küme | Sorumluluk | Dosya | Index |
|---|---|---|---|---|
| 1 | [`csharp/`](csharp/index.md) | C# / .NET dil kurallari, SOLID, async, nullable, performans | **29** | [index](csharp/index.md) |
| 2 | [`data/`](data/index.md) | ADO.NET + Dapper veri erisimi, migration, transaction, index | **26** | [index](data/index.md) |
| 3 | [`devexpress/`](devexpress/index.md) | DevExpress v26.1 kontrolleri: Grid, TreeList, Editor, Ribbon | **36** | [index](devexpress/index.md) |
| 4 | [`engine/`](engine/index.md) | Agentic orkestrasyon: router, SSE, checkpoint, diff onay | **9** | [index](engine/index.md) |
| 5 | [`winforms/`](winforms/index.md) | WinForms form uclusu, layout, async, erisilebilirlik, Designer | **33** | [index](winforms/index.md) |
| — | [`knowledge/index.md`](index.md) (bu dosya) | hub — kümeler arasi gecis | 1 | — |

**Toplam: 5 küme + 1 hub = 131 MD** — csharp 29 · data 26 · devexpress 35 · engine 8 · winforms 32 · hub 1

**Dogrulanmis (2026-10-07):** frontmatter 131/131 · mojibake 0 · boyut 2.4-7.7 KB · placeholder 0 (gercek) · 5/5 kümeyde `index.md` VAR

**Vault koku:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [WORKFLOW](../WORKFLOW.md) · [brain](../brain.md) · [VISION](../VISION.md) · [glossary](../glossary.md) · [keys](../keys.md)
