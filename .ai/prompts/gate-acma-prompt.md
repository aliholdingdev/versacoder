---
title: "Gate Acma Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "GATE-004a/003/004/005/006 kapanis paketi icin ajana verilen 5 adimli is emri: kanit kutusu, sure butcesi, red sinyalleri"
---

# Gate Acma Promptu

**Zorunlu Baglantilar:** [index](index.md) · [AGENTS](../AGENTS.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Bu gate hangi kanıtla kapanır* sorusunu, ajanın
> tekrar uygulayabileceği 5 adımlı iş emrine çeviren prompttur.

---

## §0 Ozet (Answer First)

Zincir tektir: `GATE-004a -> GATE-003 -> GATE-004 -> GATE-005 -> GATE-006`
([WORKFLOW](../WORKFLOW.md) §1). Bu prompt **kapıyı kapatmaz** — kapatan
kullanıcıdır; ajana düşen iş, kanıt kutusunu **sayılarla** doldurup onay
istemektir. Kullanım anı: gate kabul kriterleri karşılanmış görünüyor ve
kapanış paketi hazırlanacaksa.

| # | Gate | Ne ölçer | Bütçe |
|---|---|---|---|
| 1 | GATE-004a | vault 500 MD · 0 bozuk link · 0 mojibake | ≈45 dk |
| 2 | GATE-003 | 7 wireframe + kullanıcı onayı (HARD STOP) | ≈60 dk + onay |
| 3 | GATE-004 | iskelet + 2 bacak build + ilk commit | ≈90 dk |
| 4 | GATE-005 | test yeşil + NetArchTest 0 ihlal | ≈120 dk |
| 5 | GATE-006 | teslim: 4 kanıt satırı dosyada | ≈40 dk |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [WORKFLOW](../WORKFLOW.md) | gate sahibi, kabul kriteri, 4 satırlık kanıt kutusu | §2.1-§2.5 |
| 2 | [AGENTS](../AGENTS.md) | ajan kutusu (5 alan) ile gate kutusu (4 satır) ayrımı | §2 |
| 3 | [CLAUDE](../CLAUDE.md) | G5 kanıtsız "bitti" yasağı, G12 kod yazım yasağı | tam |
| 4 | [gate-004a-vault-kapanis](../checklists/gate-004a-vault-kapanis.md) | vault kapanışının adım adım soruları | tam |
| 5 | [keys](../keys.md) | anahtar kelime -> doğru dosya eşlemesi | §2 |

> **SRP notu:** Gate tanımı ve sıra bu promptta **yeniden tanımlanmaz**;
> tek kaynak [WORKFLOW](../WORKFLOW.md) §1-§2'dir. Burada yalnız *nasıl
> doldurulacağı* yazılır (SSUT).

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Kanıt denetçisi - gate kapanış paketi hazırlayan ajan.
> **KONU:** <GATE-AD> · <tarih>
>
> **GOREV:** Asagidaki §2.1'deki 5 adimi sirayla uygula; her adimdan sonra
> `(n/5)` ilerlemeyi yaz. Kapagi **kapatma** - kapatan kullanici. Once cevap,
> sonra kanit.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - Kanitsiz "bitti / kapandi" yasak (G5); sayisi olmayan kutu islenmez.
> - Gate kanit kutusu (4 satir) ile ajan cikti kutusu (5 alan) karistirilamaz.
> - Gate sirasi atlanamaz; atlanan gate geri gelir (WORKFLOW §1.2).
> - GATE-003 kapaliyken tek satir `.cs` bile yazilamaz (G12).
> - Butce asiminda "devam ederiz" yasak; is durur ve raporlanir.
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `KAPATILABILIR: EVET | HAYIR` + tek satir gerekce.
> 2. Sonra 4 satirlik kanit kutusu: `KANIT / DOSYA / EKSIK / SONRAKI`
>    - sayilar konsol ciktisindan, tahmin yasak.
> 3. Kontrol listesi: ilgili gate maddeleri `[ ] -> [x]` isaretli.
> 4. Onay istemi: secenekli + sureli (asagidaki bicim).
> 5. Kayit: `log.md`'ye tarih + gate adi + sayilar.

**Onay istemi biçimi (zorunlu):**

```text
GATE-<no> - kapanis onayi bekleniyor
  KANIT  : <olcum adi> -> <sayi> (konsol ciktisi)
  EKSIK  : <YOK ya da DOĞRULAMA GEREKLI>
  SECENEK: A) onayla   B) <eksik> icin duzeltme iste   C) kapsami genislet
  SURE   : onay ~5 dk -> SONRAKI adim baslar
```

### §2.1 Adimlar (n/5)

| # | Adim | Kanıt |
|---|---|---|
| 1 | Ölçümü koş (vault / build / test / tarama) | konsol çıktısı, sayı var |
| 2 | 4 satırlık kanıt kutusunu sayılarla doldur | `KANIT` satırında rakam |
| 3 | Gate kontrol listesini `[ ] -> [x]` işaretle | işaretli madde sayısı |
| 4 | Seçenekli + süreli onay istemini gönder | kullanıcının yanıtı |
| 5 | `log.md`'ye tarih + gate + sayılarla kaydı düş | tarihli satır |

---

## §3 Cikti Kutusu

Gate kapanışında kullanılan kutu **4 satırdır**; ajanın kendi kutusu
([AGENTS](../AGENTS.md) §2) 5 alandır ve bu işte ayrıca sona eklenir.

```text
KANIT   : measure-vault -> MD=500, FM=0 eksik, BROKEN=0, MOJIBAKE=0
DOSYA   : .ai/prompts/gate-acma-prompt.md
EKSIK   : YOK
SONRAKI : ajan dagitma promptu ile paralel blok baslat (~10 dk)
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Kanıt kutusu | 4 satır dolu, her satırda sayı ya da `YOK` |
| Onay istemi | A/B/C seçenekleri + süre yazıyor |
| Kontrol listesi | `[x]` işareti olan madde sayısı raporlanmış |
| Cevap biçimi | Önce `KAPATILABILIR:` · sonra kutu · sonra `(n/5)` |
| Kayıt | `log.md` satırı tarihli ve sayı içeriyor |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| "testler geçti, kod temiz" | sayı yok (G5) | kutuyu konsol çıktısıyla yeniden doldur |
| 4 satır yerine 5 alanlı ajan kutusu | kutu karıştırıldı (AGENTS §2) | doğru kutuyu kullan |
| Ajan "gate kapandı" dedi | yetki aşımı | kullanıcıdan onay iste |
| Süre 20 dk aşıldı | bütçe ihlali | işi durdur + raporla |
| Sıra atlandı | zincir kırıldı | gate'i geri aç |
| `EKSIK` satırı boş | eksik gizleniyor | `YOK` ya da madde yaz |

---

## §6 Yasaklar

1. **Kapıyı kapatmak yasak** — ajan kanıt sunar, kapatan kullanıcıdır.
2. **Kanıtsız sayı yasak** — tahmin, "yaklaşık", "birkaç" satırı sayılmaz.
3. **Kutu karıştırma yasak** — 4 satır gate, 5 alan ajan (SSOT).
4. **Sıra atlama yasak** — atlanan gate geri gelir, sessiz geri sarım yok.
5. **GATE-003 öncesi kod yazma yasak** — onaysız her `.cs` geri alınır.

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Ölçüm + kutu doldurma | ≈15 dk | iş durur (20 dk eşiği) |
| Kontrol listesi işaretleme | ≈10 dk | özetle, maddeyi atma |
| Onay istemi yazımı | ≈5 dk | tek soruya indir (ROLE §3) |
| `log.md` kaydı | ≈3 dk | kayıtsız kapanış geçersiz |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [ajan-dagitma-prompt](ajan-dagitma-prompt.md) | kapı açıldıktan **sonra** işin kime gideceği |
| [risk-degerlendirme-prompt](risk-degerlendirme-prompt.md) | `EKSIK` satırı doluysa risk sıralaması |
| [session-kapanis-prompt](session-kapanis-prompt.md) | oturum sonunda gate durumunun devri |
| [butce-kontrol-prompt](butce-kontrol-prompt.md) | 20 dk aşım şüphesinde bütçe denetimi |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Bölüm | §0…§9 arası, son bölüm `§9 Kanit` | başlık taraması |
| Link | bozuk link = 0 | `check-links.ps1` |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| İçerik | 5 madde sınırı + en az 1 tablo | gözden geçirme |
