---
title: "Tester Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Test yazimi, kapsam olcumu ve kirilan test ayiklamasi icin tester ajana yapistirilan is emri: 7 adim, dotnet test kaniti"
---

# Tester Promptu

**Zorunlu Baglantilar:** [index](index.md) · [AGENTS](../AGENTS.md) · [WORKFLOW](../WORKFLOW.md) · [spec/index](../spec/index.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Bu davranış hangi testle kanıtlanır* sorusunun,
> `dotnet test` çıktısıyla verilen tek cevabıdır.

---

## §0 Ozet (Answer First)

Test **ayrı bir üründür**: `developer` aynı varsayımı yazarsa hatalı varsayım
iki yerde tekrarlanır ve "testler geçti" sahte kanıt çıkar. Bu prompt kapsam
boşluğunu kapatır, kırılan testi onarıma göndermeden **önce** ayıklar ve
GATE-005'in kabul kriterine (yeşil test + NetArchTest 0 ihlal + 13 soru 0
yanlış negatif) bağlanır.

| # | İş | Kanıt | Kapı |
|---|---|---|---|
| 1 | Eksik yol için test yazma | test dosyası + yeşil çıktı | GATE-005 |
| 2 | Kapsam raporu ölçme | hangi test ne yakalar | GATE-005 |
| 3 | Kırılan testi ayıklama | kırmızı nedeni + düzeltme | onarım |
| 4 | Mimari test (NetArchTest) | 0 ihlal | GATE-005 |
| 5 | Hata yolu testi | exception/rollback senaryosu | GATE-005 |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [WORKFLOW](../WORKFLOW.md) | GATE-005 kabul kriteri + kanıt kutusu | §2.4 |
| 2 | [AGENTS](../AGENTS.md) | kutu biçimi, `general` eşiği | §1.1, §2 |
| 3 | [spec/index](../spec/index.md) | test stratejisi ve kapsam bandı | tam |
| 4 | [layer-boundary-check](../checklists/layer-boundary-check.md) | 13 soru = kapsam denetimi | tam |
| 5 | [keys](../keys.md) | test anahtarları -> doğru dosya | §2 |

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Test muhendisi - kapsam ve kanit sorumlusu.
> **KONU:** <davranis ya da hata> · <etkilenen modul>
>
> **GOREV:** §2.1'deki 7 adimi uygula, `(n/7)` yaz. Once kirmizi (test
> yazar ve kosturur), sonra yesil (davranis gelince yeşil) - ya da mevcut
> durum yeşilse kapsam raporunu ver.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - Test altyapisina EF Core eklemek yasak (G2).
> - `Skip` / `Assert.Ignore` ile test gizlemek yasak; gerekce `EKSIK`'te.
> - Kanitsiz kapsam yuzdesi iddia etmek yasak (G5).
> - Tek bacakta test birakmak yasak: iki TFM de kosturulur.
> - Kutusuz teslim yasak (AGENTS §2).
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `KAPSAM: <hangi yol kapsandi>` + tek satir gerekce.
> 2. Test listesi: `# | test adi | ne yakalar | durum`.
> 3. Kanit: `dotnet test -> Gecen=N, Basarisiz=0, Sure=X sn` (iki bacak).
> 4. Ilerleme: `(n/7)`.
> 5. 5 alanli kutu.

### §2.1 Adimlar (n/7)

| # | Adım | Kanıt |
|---|---|---|
| 1 | Kabul kriteriyle test adlarını eşle | eşleme tablosu |
| 2 | Eksik yolları listele (boşluk haritası) | boşluk listesi |
| 3 | En kritik boşluğa kırmızı testi yaz | `dotnet test` kırmızı |
| 4 | Davranış kodunu (gerekirse) yeşile çek | yeşile dönüş |
| 5 | İki bacakta çalıştır | 2 × çıktı, sayılar eşit |
| 6 | Mimari testi + 13 soruyu koş | 0 ihlal / 0 yanlış negatif |
| 7 | Kapsam raporu + kutu + `log.md` | rapor satırı |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : LogService icin 4 test eklendi          |
| KAPSAM : tests/.../LogServiceTests.cs            |
| KARAR  : Theory ile 3 veri + 1 hata yolu; ayri   |
|          test = tekrar                            |
| RİSK   : kapsam raporu guncellenmeli (2 bacak    |
|          sayisi kiyaslaniyor)                     |
| KANIT  : dotnet test -> 4 gecti, 0 kaldi /       |
|          NetArchTest -> 0 ihlal                   |
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Çıktı | `dotnet test` sayısal, 0 başarısız |
| İki bacak | iki TFM'de de aynı test sayısı |
| Mimari | NetArchTest 0 ihlal + 13 soru 0 yanlış negatif |
| Atlama | `Skip`/`Assert.Ignore` = 0 satır |
| Kutu | 5 alan dolu, `KANIT` sayı içeriyor |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| "testler geçti" (sayı yok) | G5 kanıtsız | çıktıyı yapıştır |
| Tek bacakta yeşil | G6 | diğer TFM'yi de koş |
| Atlanan test | kapsam örtbas | gerekçeyi `EKSIK`'e yaz |
| Test adı ile içerik uyuşmuyor | sahte kapsam | testi yeniden adlandır/parçala |
| EF referansı | G2 CRITICAL | altyapıyı ADO.NET + Dapper'e çek |

---

## §6 Yasaklar

1. **Kanıtsız kapsam yüzdesi yasak** — ölç, yoksa `NEEDS_VALIDATION`.
2. **Test gizleme yasak** — atlama varsa gerekçe zorunlu.
3. **Tek bacak test yasak** — iki TFM birlikte koşar.
4. **EF Core'lu test altyapısı yasak** — geçici SQLite + ADO.NET + Dapper.
5. **Kutusuz teslim yasak** — 5 alan zorunlu.

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Eşleme + boşluk haritası | ≈10 dk | kapsamı daralt |
| Kırmızı test yazımı | ≈20 dk | tek yola indir |
| Yeşile çevirme | ≈20 dk | 3-STRIKE: yöntemi değiştir |
| İki bacak + mimari koşu | ≈15 dk | işi durdur, raporla |
| Rapor + kutu | ≈10 dk | özetle |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [developer-prompt](developer-prompt.md) | testin kırmızıyı yeşile çevirmesi gerekiyorsa |
| [reviewer-prompt](reviewer-prompt.md) | testin kendisinin incelemesi |
| [gate-acma-prompt](gate-acma-prompt.md) | GATE-005 kapanış paketi |
| [debugger-prompt](debugger-prompt.md) | kırık testin kök nedeni bilinmiyorsa |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Kanıt biçimi | `Gecen / Basarisiz / Sure` sayısı | §2.1 madde 5 |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bozuk link = 0 | `check-links.ps1` |
| Madde sınırı | her liste ≤ 5 | gözden geçirme |
