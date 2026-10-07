---
title: "ULTRA-THINKING - Uzun Dusunme Protokolu"
type: knowledge
category: ".ai"
status: active
version: 2.0.0
updated: 2026-10-07
authority: primary
scope: "Karmasik karar oncesi hazirlik ve derin dusunme: 5 kriter, 5 adim, karar formati (ADR cevirme), kanit hiyerarsisi, tuzaklar, ADR-028 uretim kaydi ve prompt kaynakli akis"
tags: [identity]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# ULTRA-THINKING — Uzun Düşünme Protokolü

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE](../claude.md) · [WORKFLOW](../governance/workflow.md) · [AGENTS](agents.md) · [ROLE](role.md) · [engine](engine.md) · [brain](../.decisions/brain.md) · [MEMORY](../memory/memory.md) · [log](../ops/log.md) · [.decisions/adr-index](../.decisions/adr-index.md)

> **Tek cümlelik sorumluluk:** *Bu karar gerçekten zor mu, hangi kanıt hangi ağırlıkta, ne zaman durulur, karar yanlışsa ne kırılır ve sonuç nereye yazılır* sorusunun tek cevabıdır.
> Bu dosya **süreç değil, düşünme yöntemidir**: gate tanımı [WORKFLOW.md](../governance/workflow.md)'tedir, hard kural metni [CLAUDE.md](../claude.md)'dedir, ajan/kutu kuralları [AGENTS.md](agents.md)'tedir (SRP — tekrar yok).

---

## §0 Özet (Answer First)

Uzun düşünme **5 adımlık kapalı bir protokoldür**: **Kaynak oku -> Alternatif üret -> Kanıt karşılaştır -> Karar -> Risk.** Protokol **tek başına karar vermez**; çıktısı her zaman **karar + gerekçe + kanıt + etkilenen dosyalar + doğrulama adımı + ADR göstergesi**dır ve gate'i **kapatamaz** — yalnız kanıt hazırlar. Kanıt yoksa sonuç `NEEDS_VALIDATION` kalır, "bitti" denemez (G5). Gerçek kanıt örneği: **ADR-028, 3 tur / 21 ajan oyu ve arbitrajla üretildi** (§6).

| # | Adım | Tek cümle iş | Çıktı | Durma noktası |
|---|---|---|---|---|
| 1 | **Kaynak oku** | Vault + kaynak hiyerarşisi + çelişki taraması | Okuma notu + kısıt listesi | `CONFLICT DETECTED` çözülmüyorsa |
| 2 | **Alternatif üret** | En az 3 yol, her biri +/− ve maliyet | A/B/C tablosu | 2 seçenekten çıkamıyorsa |
| 3 | **Kanıt karşılaştır** | Seçenekleri kanıt hiyerarşisiyle eleyip sıralama | Eleme gerekçesi | Kanıt üretilemiyorsa |
| 4 | **Karar** | Tek karar + neden bu + neden diğerleri değil | Karar kutusu (§3) | Geri alma maliyeti gün sınıfındaysa |
| 5 | **Risk** | İkinci derece dalgalar + red-team `KALAN` | Risk satırı + doğrulama adımı | `KALAN` alanı boş kalamaz |

> **KURAL:** Adımlar **atlanamaz**; 1'siz 2 (kaynaksız alternatif) ve 3'süz 4 (kanıtsız karar) protokol ihlalidir. Doğrulama çalıştırılmadan sonuç `NEEDS_VALIDATION`'dır ve [engine.md](engine.md) §1.5 checkpoint'inden geçemez.

### §0.1 Prompt kaynağındaki ön-sıra (ai.08 §71 — MUTLAK KURAL)

Kod yazmadan önceki sıralı sıra; bu protokolün (§2) **girişi** sayılır. Adım
atlanmaz: anla → analiz et → modelle → ayır → doğrula → planla → onay al →
uygula.

```text
┌───────────┐   ┌──────────┐   ┌────────┐   ┌────────────┐
│ UNDERSTAND│──>│ ANALYZE  │──>│ MODEL  │──>│  SEPARATE  │
└───────────┘   └──────────┘   └────────┘   └─────┬──────┘
                                                   │
┌───────────┐   ┌──────────┐   ┌────────────┐      │
│ IMPLEMENT │<──│  APPROVE │<──│   PLAN     │<─────┤
└───────────┘   └──────────┘   └────────────┘      │
                                                   
  ┌──────────────────────────────────────────────┐
  │ DOGRULA: adim 5 (validate) — kanit yoksa     │
  │ asagidaki adimlara GECILMEZ                  │
  └──────────────────────────────────────────────┘
```

**Mutlak yasaklar (ai.08 §71):** kod yazma · class üretme · interface
implementation üretme · database oluşturma · CLI command uydurma · provider API
uydurma · framework/version uydurma. Hepsi ancak **APPROVE** (kullanıcı onayı,
GATE-003/004) sonrasındaki `IMPLEMENT` adımında başlar.

---

**İlgili:** [AGENTS](agents.md) §3.2 (12 mutlak kural) · [ROLE](role.md) §4 (tek soru · A/B/C) · [CLAUDE](../claude.md) G12 (gate disiplini) · [brain](../.decisions/brain.md) §3 (kod kuralları).
## §1 Ne Zaman Kullanılır

### §1.1 Beş tetik kriteri

Protokol **yalnız** aşağıdaki 5 durumdan biri görüldüğünde ateşlenir; tetiklenmediyse iş normal akışta ([engine.md](engine.md) §1) yürür.

| # | Kriter | Belirti | Etiket | İlk hareket |
|---|---|---|---|---|
| 1 | **Çelişen kaynak** | İki kaynak aynı konuda farklı diyor | `CONFLICT DETECTED` | Hangi basamak kazanır — onu işaretle, sonra adım 1 |
| 2 | **3. başarısız düzeltme** | Aynı hata 3 düzeltmede de kırmızı | `3-STRIKE` | Kod yazmayı bırak, **varsayımı adlandır** |
| 3 | **Geri alınması pahalı karar** | Katman/proje sayısı, teknoloji, gate kapsamı, proje sırası | — (sonuçta ADR) | Adım 1'den başla; karar [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) sınıfındaysa ADR'ye çevir |
| 4 | **Kanıtsız iddia / belirsizlik** | "Bitti" diyecek çıktı yok, sayı kaynağı yok | `UNKNOWN` · `NEEDS_VALIDATION` | Maddeyi yazma; komutu çalıştır ya da işi durdur |
| 5 | **Yıkıcı / iki bacak riski** | Silme, migration, git reset; `net48` + `net10.0-windows` uyumluluk sorusu | `G7 ONAYI` | Tek soruluk onay hazırla; protokol kararı hazırlar, onayı **kullanıcı** verir |

**Kriter → kanıt eşlemesi:**

| Kriter | Protokol adımında aranan kanıt | Yetersizse |
|---|---|---|
| 1 Çelişen kaynak | Kaynak hiyerarşisinde üst basamağın yazılı olması | `DECISION BLOCKED` + tek soru |
| 2 3. düzeltme | Yeni hipotez + farklı yöntem (aynı yöntem tekrarı yasak) | Bloke raporu ([AGENTS.md](agents.md) §4) |
| 3 Pahalı karar | En az 3 alternatif + geri alma maliyeti | Karar `NEEDS_VALIDATION` |
| 4 Kanıtsız iddia | Çalıştırılmış komutun çıktısı | Madde silinir (G5) |
| 5 Yıkıcı işlem | Onay istemi + kurtarma yolu | Komut çalışmaz (G7) |

### §1.2 Ne zaman kullanılmaz

| Durum | Yapılacak | Neden protokol yok |
|---|---|---|
| Açık kural, tek doğru cevap ([CLAUDE.md](../claude.md) G3: tek ribbon stili `RibbonControlStyle.Office2010`) | Uygula | Seçenek üretmek kuralı sorgulamak olur |
| Biçim düzeltmesi (frontmatter, link, UTF-8) | Düzelt + tara | Ölmek isteyen karar değil |
| ADR'de çoktan kararlaştırılmış konu | ADR'yi uygula | Karar zaten kayıtlı ([brain.md](../.decisions/brain.md)) |
| Mekanik refactor (rename, format) | `dotnet build` + `dotnet test` kanıtı yeter | Karar yok, işlem var |

> **UYARI:** Protokol **her işe** uygulanırsa karar yorgunluğu doğar ve G5'ten kaçış aracına döner ([AGENTS.md](agents.md) §6 bütçesiyle çakışır).

### §1.3 Giriş bütçesi (zaman kutusu)

| # | Kalem | Sınır | Aşım belirtisi | Aşımda |
|---|---|---|---|---|
| 1 | Protokol başlangıcı | tetikten **60 sn** içinde | "Sonra bakarım" | Kaynak çelişkisine dönüşür |
| 2 | Toplam protokol süresi | **≤ 65 dk** | Adım şişmesi | Aşımı `log.md`'ye yaz, adımı küçült |
| 3 | Adım 1 kaynak okuma | **≤ 15 dk** | Okuma notu hâlâ yok | Kısıt listesine geç, notu kısalt |
| 4 | Adım 2 alternatif | **≤ 15 dk** | 2 seçenekten çıkamıyor | "Yapma/ertele" seçeneğini ekle |
| 5 | Adım 3 kanıt | **≤ 20 dk** | Kanıt üretilemiyor | `NEEDS_VALIDATION` etiketiyle 4'e geç |

### §1.4 Tetik → ilk 60 saniye

Protokolün girişine "sonra bakarım" diye beklenen iş, kaynak çelişkisine dönüşür; her tetik için ilk hamle **aşağıdaki**dır.

| # | Tetik | İlk 60 sn içinde yapılacak | Yapılmayacak |
|---|---|---|---|
| 1 | İki kaynak çelişiyor | `CONFLICT DETECTED` yaz + hangi basamak kazanır onu işaretle | İkisini birden tutmak |
| 2 | Aynı hata 2. kez | Hipotezi **değiştir**, kapsamı genişlet | Aynı düzeltmeyi birebir tekrar |
| 3 | Aynı hata 3. kez | **DUR** → `3-STRIKE` bloke raporu | 4. deneme |
| 4 | `net48` derleme hatası | Kategoriyi bul (API / dil / paket) | `#if` ile hatalı kodu gizlemek |
| 5 | DevExpress sürüm/stil hatası | G3'e dön: tek sürüm (`v26.1`), tek stil (`Office2010`) | Alternatif stil denemek |
| 6 | EF Core / `DbContext` hatası | G2: paket ve referansı tara, kaldır | "Neden çalışmıyor" diye debug etmek |
| 7 | Test kırmızı, neden belirsiz | Testi izole et → hangi katman? | Toplu test çalıştırmaya devam |
| 8 | Gate kanıtı eksik | Boşluğu adlandır: `NEEDS_VALIDATION` | "Yeterli" demek |
| 9 | Yıkıcı işlem ihtiyacı | `G7 ONAYI` → tek soruluk onay iste | Sessiz `Remove-Item` / `git reset` |
| 10 | "Muhtemelen geçer" hissi | G5: çıktıyı üret | His ile devam etmek |

---

## §2 Adımlar

### §2.1 Akış (saf ASCII)

```text
 TETIK (§1.1 kriter)
       |
       v
 +------------------+
 | 1 KAYNAK OKU     |  vault + kaynak hiyerarsisi + celişki taramasi
 | cikti: kisit     |---- celişki cozulmediyor? --> DECISION BLOCKED
 +------------------+                              + kullaniciya tek soru
       |
       v
 +------------------+
 | 2 ALTERNATIF URET|  en az 3 yol: her biri + / - / maliyet
 | cikti: A/B/C     |
 +------------------+
       |
       v
 +------------------+
 | 3 KANIT KARSILASTIR | kanit hiyerarsisi (§4) + stres sorulari
 | cikti: eleme       |-- kanit yok? --> NEEDS_VALIDATION
 +------------------+         |
       |                      v
       |               (etiketle devam, "bitti" DE)
       v
 +------------------+
 | 4 KARAR          |  tek karar + gerekce + neden digerleri degil
 | cikti: kutu (§3) |--> gun sinifi geri alim? --> kullanici onayi
 +------------------+
       |
       v
 +------------------+
 | 5 RISK           |  6 dalga + red-team KALAN + dogrulama adimi
 | cikti: risk satiri|--> KALAN bos? --> karar tamamlanmamis sayilir
 +------------------+
       |
       v
  ADR'ye CEVIR (§3)  ->  .decisions/  veya  "ADR GEREKMEZ" gerekcesi
       |
       v
  GATE'E KANIT OLARAK SUN  ->  KAPATAN KULLANICI ([WORKFLOW.md](../governance/workflow.md))
```

### §2.2 Adım kartları

| Adım | Girdi | İşlem | Çıktı | Hata modu |
|---|---|---|---|---|
| **1 Kaynak oku** | Dağınık problem, hata çıktısı, çelişen kaynaklar | Vault dosyalarını oku, geçerli kısıtları (G maddesi, ADR, gate) listele, çelişkiyi işaretle | `KISIT: [liste]` + kaynak etiketi | Çözümü baştan seçip kısıtı ona göre yazmak |
| **2 Alternatif üret** | Kısıt listesi | En az 3 seçenek; her birine +/−, yaklaşık süre, katman etkisi | A/B/C tablosu | 2 seçenek = gizli "evet/hayır" varsayımı |
| **3 Kanıt karşılaştır** | Seçenekler + eldeki kanıt | Kanıt hiyerarşisine (§4) göre sırala; çürüten soruları sor; seçenek eler | Eleme gerekçesi + kalanlar | Tahmini kanıtı gerçek kanıt gibi değerlendirmek |
| **4 Karar** | Elemeden kalan(lar) | Tek karar seç; neden bu ve neden diğerleri değilini yaz | Karar kutusu (§3) | İki seçeneği birden tutmak |
| **5 Risk** | Karar | 6 dalga (katman/test/doküman/gate/risk/geri alma) + red-team | Risk + `KALAN` + doğrulama adımı | Dalga hesaplanmadan süre tahmini ("≈45 dk" uydurması) |

### §2.3 Adım 1 — Kaynak oku

| # | Okunan | Ne verir | Etiketi |
|---|---|---|---|
| 1 | [CLAUDE.md](../claude.md) | G1..G12, kanıt formatı, dur/bekle | `REPOSITORY_VERIFIED` |
| 2 | [WORKFLOW.md](../governance/workflow.md) | Gate kriteri + sırası + kanıt kutusu | `REPOSITORY_VERIFIED` |
| 3 | [AGENTS.md](agents.md) · [ROLE.md](role.md) | Kim yapar, kimin son sözü, bütçe | `REPOSITORY_VERIFIED` |
| 4 | İlgili ADR ([.decisions/adr-index.md](../.decisions/adr-index.md)) | Bağlayıcı karar + şartlar | `SOURCE_VERIFIED — ADR-NNN §X` |
| 5 | [engine.md](engine.md) | Yürütme/katman gerçeği (l0..l7, 9 üretim + 1-2 test proje) | `REPOSITORY_VERIFIED` |
| 6 | [reports/index.md](../ops/log.md) · [reports/k3-618-ham-envanter.md](../architecture/layer-5000/index.md) | Ham veri, `needs-validation` işaretli | `SOURCE_VERIFIED` ya da `NEEDS_VALIDATION` |

**Çıktı şablonu:**

```text
KAYNAK  : <dosya> : <satir>  ->  <ne ogrendim>   [<ETIKET>]
KISIT   : G5 (kanit zorunlu) · G6 (iki bacak) · ADR-028 s2.3 (6 sart)
CELIŞKI : <varsa hangi ikisi, hangi basamak kazaniyor>
```

> **UYARI:** Etiketsiz teknik iddia **uydurmadır** ve madde silinir ([CLAUDE.md](../claude.md) §3.1). "Kaynakta öyle yazıyordu" değil; `SOURCE_VERIFIED — ADR-028 §3`.

### §2.4 Adım 2 — Alternatif üret

| # | Kural | Detay |
|---|---|---|
| 1 | **En az 3** seçenek | 2 seçenek çoğunlukla gizli "evet/hayır" saklar |
| 2 | Her seçenekte **+ ve −** | Artı tek başına satış cümlesidir |
| 3 | **"Yapma / ertele"** seçeneği dâhil | Bazen doğru cevap budur |
| 4 | Her seçenekte **maliyet** | Somut: "≈45 dk" · "biraz sürer" yasak |
| 5 | Katman etkisi | Hangi makro katman (`l0..l7`) değişir, hangi test kırılır |

**Örnek alternatif tablosu (biçim standardı):**

| Seçenek | + | − | Maliyet |
|---|---|---|---|
| **A. Mevcut kararı olduğu gibi uygula** | Sıfır yeni borç, kanıt hazır | Bilinen eksik kalır | ≈45 dk |
| B. Alternatif çözümü dene | Uzun vadede daha temiz | Yeni paket = G8, iki bacak testi tekrar | ≈3 saat |
| C. Şimdi yapma, gate sonuna ertele | Bugünkü kapı kapanır | Ertelenmiş iş kapıda birikir | ≈0 dk + borç |

### §2.5 Adım 3 — Kanıt karşılaştır

Karşılaştırma iki ölçütle yapılır: **kaynak ağırlığı** (§4 hiyerarşi) ve **çürüklük soruları** (aşağıda). Bir seçenek "en iyi görünen" değil, **en ağır kanıtı taşıyan** olandır.

| # | Çürüklük sorusu | "Hayır" ise |
|---|---|---|
| 1 | Bu seçenek için **çalıştırılmış bir çıktı** var mı? | Tahminle devam edilmez — `NEEDS_VALIDATION` |
| 2 | **net48** ve **net10.0-windows** ikisinde de geçerli mi? | G6 ihlali — seçenek elenir |
| 3 | EF Core / string-concat SQL / v26.1 dışı sürüm gerektiriyor mu? | G2/G3 ihlali — **elenir** |
| 4 | Gate kriterini bu seçenek mi karşılıyor? | Geçersiz kanıt — elenir |
| 5 | **Kimse neden yanlış olabileceğini söylemedi mi?** | Red-team eksik (§5) — karar `NEEDS_VALIDATION` |

**Kategori bazlı derinleştirilmiş set (12 soru):**

| # | Kategori | Soru | Çıkarsa |
|---|---|---|---|
| 6 | İki bacak | Bu seçenek **net48**'de de geçerli mi? | G6 — elenir |
| 7 | İki bacak | `#if` yalnız platform farkı mı, iş mantığını bölüyor mu? | G6 — elenir |
| 8 | İki bacak | Yeni paket **her iki TFM'de** restore ediliyor mu? | `NU1202` riski |
| 9 | Veri erişimi | Kodda `Microsoft.EntityFrameworkCore*` / `DbContext` geçiyor mu? | G2 CRITICAL — elenir |
| 10 | Veri erişimi | Her sorgu `@parametre` ile mi (`$"...{x}"` = 0)? | Injection — elenir |
| 11 | Mimari | Seçenek `l4 -> l2` gibi **yasaklı ok** üretiyor mu? | [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) 8x8 matris — elenir |
| 12 | Mimari | [layer-boundary-check](../checklists/layer-boundary-check.md) 13 sorusundan geçer mi? | Mimari RED |
| 13 | UI | Yıkıcı kontrol/ekran değişikliği **GATE-003** öncesi mi? | G12 — ertele |
| 14 | UI | Sürüm yalnız `v26.1`, stil yalnız `Office2010` mu? | G3 — elenir |
| 15 | Gate | İlgili gate kriteri bu kanıtla karşılanıyor mu? | Geçersiz kanıt |
| 16 | Test | İlgili test **iki bacakta** da yeşil mi? | G5 — kanıt eksik |
| 17 | Kaynak | Her iddia bir `SOURCE_VERIFIED` / `REPOSITORY_VERIFIED` taşıyor mu? | Etiketsiz iddia = uydurma |

**Eleme karar tablosu:**

| Sonuç | Aksiyon |
|---|---|
| 3/3 "evet" ve kanıt ekli | 4'e geç |
| 1–2 madde `NEEDS_VALIDATION` | Etiketle 4'e geç; doğrulama adımı 5'te yazılı olmalı |
| Herhangi bir G2/G3/G6 ihlali | Seçenek **elenir**, 2'ye dön |
| Kanıt üretilemiyor | **Dur** — `NEEDS_VALIDATION`, işi [engine.md](engine.md) §4.2'ye göre durdur |
| 3. başarısız deneme | **Dur** — `3-STRIKE` bloke raporu |

### §2.6 Adım 4 — Karar

| Alan | Zorunlu içerik | Boş olabilir mi? |
|---|---|---|
| Seçilen seçenek | Tek, açık (A/B/C) | HAYIR |
| **Neden bu?** | 1–3 ölçülebilir gerekçe | HAYIR |
| **Neden diğerleri değil?** | Her elenen için tek cümle | HAYIR |
| Geri alma maliyeti | dakika / saat / gün + nasıl geri alınır | HAYIR |
| Onay sahibi | gate ise **kullanıcı**, kod içi ise ajan | HAYIR |

> **KURAL:** "En iyi seçenek bu" **karar değildir**. Karar; ad, gerekçe, eleneni ve geri alma yolunu birlikte içerir ([§3](#3-karar-formatı-adr-ye-çevirme)).

### §2.7 Adım 5 — Risk

Karar **yalnız** dokunduğu dosyayı değiştirmez; 6 dalga hesaplanır. Dalga hesaplanmadan verilen süre tahmini **taahhüt değil, hayaldir**.

| # | Dalga | Sorulacak | Örnek |
|---|---|---|---|
| D1 | **Katman** | Hangi makro katmanlar değişir (`l0..l7`)? | Yeni sağlayıcı = `l4` + `l6`; `l2` değişmez |
| D2 | **Test** | Hangi test kırılır, hangisi eklenmeli? | NetArchTest + kontrat testi |
| D3 | **Doküman** | Hangi vault dosyası güncellenmeli? | [engine.md](engine.md), [.rules/index.md](../.rules/index.md) altındaki kural |
| D4 | **Gate** | Hangi gate kanıtı tazelenmeli? | GATE-004 kanıt kutusu yeniden üretilir |
| D5 | **Risk** | Hangi risk kaydı güncellenmeli? | [reports/index.md](../ops/log.md) altındaki kayıt |
| D6 | **Geri alma** | Nasıl geri alınır? | ADR `superseded` + yeni ADR |

**Red-team zorunlu üçlüsü:**

```text
KARSI  : kararin EN GUCLU karsi argumani (adilce, siddetli)
CEVAP  : neden bu karari yikmiyor  (kanit + kaynak hiyerarsisindeki basamak)
KALAN  : cevabin zayif kismi -> NEEDS_VALIDATION + dogrulama adimi
```

> **UYARI:** `KALAN` alanı **boş bırakılamaz**. "Tamamen ikna oldum" cümlesi red-team'i işlevsiz kılar ve gizli varsayım bırakır.

### §2.8 Ne zaman durulur

| # | Durma nedeni | Etiket | Yapılacak |
|---|---|---|---|
| 1 | Çelişki çözülmedi | `DECISION BLOCKED` | Kullanıcıya **tek** soru ([ROLE.md](role.md) §4) |
| 2 | Kanıt üretilemiyor | `NEEDS_VALIDATION` | Madde yazılmaz; komutu çalıştırma planı yazılır |
| 3 | Bilgi hiç yok | `UNKNOWN` | İş durur, tahmin yazılmaz |
| 4 | Aynı hata 3. kez | `3-STRIKE` | Bloke raporu + `dur/bekle` ([engine.md](engine.md) §4) |
| 5 | Yıkıcı işlem gerekiyor | `G7 ONAYI` | Onay gelmeden komut çalışmaz |
| 6 | Gate kriteri konuşuluyor | `BEKLEMEDE` | Kanıt hazırlanır, **kapatma kullanıcıya** aittir |

---

## §3 Karar Formatı (ADR'ye çevirme)

### §3.1 Zorunlu 6 alan

| # | Alan | İçerik | Boş bırakılabilir mi? |
|---|---|---|---|
| 1 | **Karar** | Tek cümle, tek seçenek | HAYIR |
| 2 | **Gerekçe** | Neden bu, neden diğerleri değil (ölçülebilir) | HAYIR |
| 3 | **Kanıt** | Komut çıktısı **ya da** `NEEDS_VALIDATION` + neden | HAYIR (etiket zorunlu) |
| 4 | **Etkilenen dosyalar** | Yol listesi (`.ai/**` ve/veya `src/**`) | HAYIR |
| 5 | **Doğrulama adımı** | Tekrarlanabilir komut | HAYIR |
| 6 | **ADR göstergesi** | `ADR-NNN-<konu>` **veya** "ADR gerekmez + nedeni" | HAYIR |

**Karar kutusu (ASCII, standart biçim):**

```text
+--------------------------------------------------+
| KARAR    : <tek cumle, tek secenek>               |
| GEREKCE  : 1. <olculbilir>  2. <olculbilir>       |
| KANIT    : <komut -> sayilar> | NEEDS_VALIDATION  |
| ETKILENEN: <dosya yollari>                        |
| DOGRULAMA: <tekrarlanabilir komut>                |
| ADR      : ADR-NNN-<konu> | ADR GEREKMEZ: <neden> |
| RED-TEAM : KARSI <ozet> -> CEVAP <ozet> -> KALAN  |
| GATE     : <hangi gate> | DURUM: BEKLEMEDE        |
+--------------------------------------------------+
```

> **KURAL:** Alanlardan biri dahi `—` ise sonuç `DECISION BLOCKED` sayılır ve gate kanıtına giremez.

### §3.2 Hangi karar ADR açar

| Karar sınıfı | ADR? | Örnek | Dosya |
|---|---|---|---|
| Katman / proje sayısı, bağımlılık yönü | **EVET** | 8 makro katman, 9 üretim proje | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
| Teknoloji/sürüm kilidi | **EVET** | DevExpress tek sürüm, EF Core yok | `.decisions/accepted/` altındaki ADR |
| Geri alma maliyeti **gün** sınıfı | **EVET** | Migration sırası, sağlayıcı sırası | yeni `ADR-NNN-<konu>.md` |
| Kapsam kararı (ekran sayısı vb.) | Hayır + **kullanıcı onayı** | 7 ekran vs 16 wireframe | rapor + gate paketi |
| Kod içi, testle kanıtlanan | Hayır | Bir handler'ın davranışı | diff + `dotnet test` |
| Biçim/UTF-8/link düzeltmesi | Hayır | Frontmatter eksikliği | doğrudan düzelt |

**ADR dosya adı ve durum akışı:**

| Adım | Kural |
|---|---|
| Dosya adı | `ADR-NNN-<kebab-case-konu>.md` (ASCII, Türkçe karakter yok) |
| Numara | [brain.md](../.decisions/brain.md) + [.decisions/adr-index.md](../.decisions/adr-index.md) ile eş, tekrarsız |
| Durum | `proposed` -> `accepted` (yalnız kullanıcı onayıyla) -> `superseded` |
| Minimum içerik | Bağlam · Karar · Alternatifler (oy kaydıyla) · Sonuçlar |
| Kayıt yeri | Karar `accepted` ise tek yaşam yeri ADR'dir; başka dosyaya **kopyalanmaz** |

### §3.3 Dolu karar kaydı örneği

```text
KARAR    : Paralel ajan sayisi 3 ile sinirlanir.
GEREKCE  : 1. 2026-09-29'da 8 paralel ajan "Rate limit exceeded" verdi.
           2. 3 sayisi AGENTS s5 + WORKFLOW s3.2 ile ayni kaynaktadir.
           3. Sınır asilirsa is seriyi de alir (kayip yok, yalniz sure artar).
KANIT    : log.md 2026-09-29 kaydi + AGENTS.md s5 (SOURCE_VERIFIED)
ETKILENEN: .ai/AGENTS.md, .ai/WORKFLOW.md, .ai/engine.md
DOGRULAMA: AGENTS.md s5 madde sayimi = 1; engine.md s2.1 olay tablosu = 1
ADR      : ADR GEREKMEZ - surec kurali, mimari karar degil
RED-TEAM : KARSI: 3, olcusuz bir kestirim olabilir; CEVAP: 8->3 olayi olcum,
           limit nedeni agir; KALAN: model/saglayici adi DOĞRULAMA GEREKLI.
GATE     : GATE-004a (vault) kaniti | DURUM: BEKLEMEDE
```

---

## §4 Kanıt Hiyerarşisi

### §4.1 Dört seviye

| Seviye | Tanım | Ağırlık | Örnek | Kararda kullanımı |
|---|---|---|---|---|
| **S1 — Build / test çıktısı** | Çalıştırılmış komutun metni | **En ağır** — tek başına yeter | `dotnet build -> 0 hata (net48, net10.0-windows)` | "Bitti" ancak bu seviyeyle söylenir (G5) |
| **S2 — Dosya satırı / script çıktısı** | Diskte okunan gerçek ya da salt-okunur tarama | Ağır | `ADR-028 §3` · `AGENTS.md §5` · `count-components -> PASS` | Kısıt ve karar gerekçesi |
| **S3 — Kayıtlı karar (ADR) / gate kriteri** | Bağlayıcı, değişmesi onay isteyen | Bağlayıcı | [ADR-028](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) 6 şart | Seçenek **eler** (kural ihlali) |
| **S4 — Ajan sözü / tahmin / his** | Çıktısı olmayan ifade | **En hafif — tek başına asla yetmez** | "Muhtemelen geçer", "testler geçer" | Yalnız S1-S3'e giden **yol gösterir**, karar yapamaz |

**Ağırlık kuralı:**

```text
 S1 cikti  >  S2 dosya satiri  >  S3 kayitli karar  >  S4 ajan sozu
     |               |                  |                  |
     +---------------+------------------+                  |
             tersine karar olmaz                    tek basina KARAR degil
             (S4, S1'i uretmek icin kullanilir)
```

**Seviye çakışması tablosu:**

| Durum | Doğru hareket |
|---|---|
| S4 ile S2 çelişiyor | **S2 kazanır**; S4 işe yaramaz |
| S2 ile S1 çelişiyor | **S1 kazanır** (yazılı olan değil, çalışan kazanır) |
| S1 ile S3 çelişiyor | **S3 kazanır + çelişki `CONFLICT DETECTED`** — kural ihlali testle "geçilemez" |
| İkisi de S1 | Daha **yenisini** al, tarih/saat yaz |
| Hiçbiri yok | `UNKNOWN` + dur ([CLAUDE.md](../claude.md) §7 S2) |

### §4.2 Kaynak ve doğrulama etiketleri (7)

| Durum | Doğru yazım | Yanlış yazım |
|---|---|---|
| Diskte okuyup gördüm | `REPOSITORY_VERIFIED — .ai/index.md:12 (2026-09-29)` | "index'de yazıyordu" |
| Belgede okudum | `SOURCE_VERIFIED — ADR-028 §2.1` | "planda öyle" |
| Üçüncü taraf kaynak kodunda | `REFERENCE_VERIFIED — dosya + satır` | "kaynak böyle yapıyor" |
| Web'de doğruladım | `WEB_VERIFIED — <URL> (tarih)` | "internette öyle" |
| Kendi çıkarımım | `INFERRED — doğrulanmadı` | Doğrulanmış gibi sunmak |
| Doğrulanmalı | `NEEDS_VALIDATION — komut: <komut>` | "Sorun olmaz" |
| Bilmiyorum | `UNKNOWN` + işi durdur | Tahmin yazmak |

### §4.3 Kanıt üretilemezse

| Senaryo | Yazılacak | Yapılmayacak |
|---|---|---|
| Komut çalıştırılamıyor (sağlayıcı yok) | `NEEDS_VALIDATION — komut: <komut>` + dur | "Muhtemelen geçer" |
| Sayı kaynağı yok | `DOĞRULAMA GEREKLI` | Yaklaşık bir sayı uydurmak |
| Kaynak çelişiyor | `CONFLICT DETECTED` → tek soru | İkisini birden tutmak |
| Gate kriteri tartışılıyor | `BEKLEMEDE` + kanıt paketi | Gate'i kendisi kapatmak |
| Tahminle iddia edilmiş | `INFERRED` etiketi | Etiketsiz iddia (uydurmadır, madde silinir) |

### §4.4 Kanıt üretim komutları (S1'i üreten araçlar)

| Amaç | Komut / tarama | Beklenen çıktı |
|---|---|---|
| Derleme kanıtı | `dotnet build -c Release` | **0 hata — her iki TFM** (`net48`, `net10.0-windows`) |
| Tek bacak izolasyonu | `dotnet build -f net48` · `-f net10.0-windows` | 0 hata |
| Test kanıtı | `dotnet test --framework net48` · `--framework net10.0-windows` | `X gecti / 0 basarisiz / Y sn` |
| Mimari kanıt | NetArchTest (`LayerRulesTests`) | 0 ihlal |
| Vault link kanıtı | link tarama scripti | `BROKEN = 0` |
| Frontmatter / mojibake | `.ai/**/*.md` ilk satır + ilk 200 bayt sayımı | `FM = 0 eksik`, `MOJIBAKE = 0` |
| Unicode kutu çizim | `U+2500-U+257F` taraması | **0 satır** |
| Yasaklı referans | EF Core / v26.1 dışı / string-concat taraması | **0 eşleşme** |
| Katalog sayımı | `count-components.ps1` | `52 dosya / 5440 kayit / PASS` (ADR-028 C4) |

> **UYARI:** Bu komutların **hiçbiri çalıştırılmadan** yazılan satır S4'tür ve karar yapamaz; yalnız S1 üretme talimatıdır.

---

## §5 Tuzaklar

### §5.1 Yasaklı davranışlar

| # | Tuzak | Belirti | Sonuç | Doğrusu |
|---|---|---|---|---|
| 1 | **Uydurma sayı** | Kaynağı olmayan yüzde/süre/sayı | G9 ihlali, gate RED | `DOĞRULAMA GEREKLI` ya da kaynak etiketi |
| 2 | **Teyitsiz "bitti"** | Çıktı yoksa "tamamlandı" | G5 ihlali — iş bitmedi sayılır | `dotnet build/test -> ...` çıktısı |
| 3 | **Kaynaksız iddia** | "Kaynakta öyle yazıyordu" | Etiketsiz iddia = uydurma | `SOURCE_VERIFIED — dosya §bölüm` |
| 4 | **Adım atlama** | 2'ye girmeden 4'e geçmek | Seçeneksiz/kaynaksız karar | §2.1 sırası |
| 5 | **Seçeneksiz karar** | 2 alternatifle karar | Gizli "evet/hayır" varsayımı | ≥ 3 seçenek (§2.4) |
| 6 | **Adil olmayan red-team** | Zayıf karşı argüman | Sahte güvenlik | §2.7 sual seti + `KALAN` zorunlu |
| 7 | **"Tamamen ikna oldum"** | `KALAN` boş | Gizli varsayım kalır | `KALAN: NEEDS_VALIDATION` yaz |
| 8 | **Protokolün her işe uygulanması** | Biçim düzeltmesine bile protokol | Karar yorgunluğu | §1.2 tablosu |
| 9 | **Dalga hesaplamama** | "≈45 dk" tahmini | 10 kat sürebilir sapma | §2.7 6 dalga |
| 10 | **Kendi kararını onaylama** | Ajan "geçti" der | Gate yetkisi ihlali | Kapatan yalnız kullanıcı |
| 11 | **Kopyala-yapıştır** | Başka vault'tan metin | ADR-002 ihlali | Yeniden harmanla, bilgiden faydalan |
| 12 | **Sonucu ikinci dosyaya kopyalama** | Aynı karar iki yerde | SSOT ihlali | Tek yaşam yeri: ADR |

### §5.2 Erken uyarı sinyalleri

| # | Sinyal | Ne anlama gelir | Hemen |
|---|---|---|---|
| 1 | Cümle "bence" ile başlıyor | S4 seviyesine düştü | Kanıt ara (S1/S2) |
| 2 | "Biraz sürdü / yaklaşık" | Ölçüm yok | Süreyi/komutu üret |
| 3 | 3. aynı düzeltme denemesi | Varsayım yanlış | §2.8 madde 4 — **dur** |
| 4 | Alternatif listesi 2 satır | Seçenek eksik | 2'ye dön, 3. seçenek üret |
| 5 | `KALAN` boş | Red-team tiyatrolaştı | Karşı argümanı yeniden yaz |
| 6 | Kararda dosya yolu yok | Dalga hesaplanmadı | D1–D6 doldur |

### §5.3 Hata raporu biçimi (karar değil, arıza için)

```text
KONUM     : <dosya yolu> : <satir>  ya da  <gate adi>
NEDEN     : <kok neden, 1 cumle>            [ETIKET]
DUZELTME  : <yapilacak is, ≈ sure>
KANIT     : <komut -> sayilar>  |  NEEDS_VALIDATION
```

> **UYARI:** Bu kutu [engine.md](engine.md) §1.5'teki **gate kanıt kutusuyla** ve §3.1'deki **karar kutusuyla** aynı şey değildir — karıştırma SSOT ihlalidir ([AGENTS.md](agents.md) §2 "Karıştırma yasağı").

### §5.4 Red-team sual seti (en güçlü karşı argümanı kurmak)

Karşı argüman **zayıf kurulursa** test işe yaramaz. Her karar için 8 sua sorulur ve **en zayıf cevap** metne yazılır.

| # | Sual | Ne arıyor |
|---|---|---|
| 1 | Bu kararı 6 ay sonra geri almak ne kadara mal olur? | Geri alma maliyeti (§2.6) |
| 2 | Karar hangi varsayımı besliyor; o varsayım yanlışsa ne olur? | Kör nokta |
| 3 | Alternatif gerçekten denendi mi, yoksa konfor mu? | Seçim önyargısı |
| 4 | Beklenmeyen **ikinci derece** etki hangisi (§2.7 dalgaları)? | Sessiz borç |
| 5 | Kim bundan rahatsız olur — `reviewer` mı, `tester` mı, kullanıcı mı? | Paydaş çatışması |
| 6 | Bu karar hangi **gate kanıtını** değiştiriyor? | Süreç etkisi |
| 7 | "Bilmiyorum" dediğimiz kısım neresi? | `NEEDS_VALIDATION` alanı |
| 8 | Karar tek başına doğru, ama **sırası** yanlış olabilir mi? | Zamanlama hatası |

**Red-team sıklığı:**

| Karar tipi | Red-team zorunlu mu? |
|---|---|
| Mimari / katman değişikliği | **Evet**, her zaman |
| Teknoloji / paket / sürüm seçimi | **Evet** |
| Risk ile ilgili karar (iki bacak, UI kapsamı) | **Evet** |
| Kapsam / gate kararı | **Evet** + kullanıcı onayı |
| Küçük, testle kanıtlanan kod içi değişiklik | Hayır (build + test yeter) |

---

## §6 Örnek: ADR-028 Nasıl Üretildi (gerçek kayıt)

> Bu bölüm **gerçek bir olaydır**: 3 tur, 21 ajan oyu ve bir arbitraj sonucu ADR-028 kabul edildi. Kanıt dosyası: [.decisions/accepted/ADR-028-layered-baseline-9-projects.md](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) §3.

### §6.1 Bağlam (adım 1 çıktısı)

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | İki rakip mimari taslak vardı: vault ADR-013 (8 makro / 52 düğüm / 9 proje) vs K3 `versacoder.mimari-02.md` (9 Tier / 500+ DLL / 5000+ bileşen) | ADR-028 §1 |
| C2 | K3'ün elle sayım envanteri **618 giriş / 616 benzersiz**; 2 birebir tekrar, 9 başlıktan 2'si yanlış, 4 DLL dosyada yok | [reports/k3-618-ham-envanter.md](../architecture/layer-5000/index.md) |
| C3 | `count-components.ps1` **52 dosya / 5440 kayıt / 0 bozuk format / PASS** | ADR-028 C4 |
| C4 | 3 tur, **21 ajan** (10 senior → 5 senior → 3 junior + 3 ajans) | ADR-028 §3 |
| C5 | Yön çelişkisi: bir belge `l4 -> l2` derken dependency-map `l4 -> l2 = X` der | ADR-028 C6 |

**Kısıt listesi (adım 1 sonucu):** `KISIT-1` G1 (kopyalama yok) · `KISIT-2` G6 (iki bacak: `net48` + `net10.0-windows`) · `KISIT-3` G2 (EF Core yok, ADO.NET + Dapper) · `KISIT-4` 500 MD / 0 bozuk link gate eşiği · `KISIT-5` G9 (sayısız iddia yasak).

### §6.2 Alternatifler (adım 2) ve oylar (adım 3)

| Etiket | Alternatif | Tur 1 (10 senior) | Tur 2 (5 senior) | Tur 3 (3 junior + 3 ajans) | Sonuç |
|---|---|---|---|---|---|
| **A** | ADR-013 birebir + 6 şart | **4/10** | **4/5** | **6/6** (5 şartlı, 1 koşulsuz) | **KABUL** |
| **C** | Uzlaşma: 8 katman + tier envanteri katalogda (`Tier-DLL` türetilmiş sütun) | **6/10** | 1/5 | 0 | RED |
| **B** | K3: 9 Tier / 500+ DLL = ayrı proje | **0/10** | 0 | 0 | **UNIVERSAL RED** |

**Tur 1 detayı:** senior grubunda uzlaşma (C) ilk turda öndeydi (6/10) — katalogda tüm envanterin görünmesi cazip görünüyordu. **Arbitraj 1:** ikinci sayım birimi (`Tier-DLL` sütunu) tespit edildi; `count-components` iki birimde RED verir, `Durum` sütununda staleness iki katına çıkar → C 1/5'e düştü. **Arbitraj 2:** K3'ün (B) 575 `.csproj` × 2 TFM = **~1150 hedef** ürettiği ve CPM/VS/restore'u çökerteceği, üstelik kapsam dışı DLL'ler (EntityFramework, Host.Linux, Protocol.Kafka) taşıdığı görüldü → B'a **evrensel red**. **Tur 3'te** junior ve ajans itirazlarının tamamı A'ya **şart** olarak yazıldı, hiçbiri alternatif üretmedi → A 6/6.

### §6.3 Protokol adımlarıyla eşleme

| Adım | Olan | Kanıt |
|---|---|---|
| 1 Kaynak oku | ADR-013, K3 envanteri, dependency-map, `count-components` çıktısı okundu; çelişki `CONFLICT DETECTED` ile işaretlendi | ADR-028 §1 C1–C6 |
| 2 Alternatif üret | **3** alternatif (A/B/C), her biri +/− ve maliyetle yazıldı | ADR-028 §3 |
| 3 Kanıt karşılaştır | S2 (dosya satırı: 618/616, 5440, 52 dosya) ve S3 (bağlayıcı yön kuralı) ile elendi: C ikinci sayım birimi, B ~1150 hedef | ADR-028 §3 "Sonuç" sütunu |
| 4 Karar | **A** — ADR-013 birebir + 6 şart; neden C ve B olmadığı tek tek yazıldı | ADR-028 §2 |
| 5 Risk | 6 şart koşullu kabul edildi (D1–D6 dalgaları şartlara dönüştü) | ADR-028 §2.3 |

### §6.4 Sonuç: 6 şart (A'ya bağlanan koşullar)

| # | Şart (ADR-028 §2.3) | Kanıtı |
|---|---|---|
| 1 | `Directory.Packages.props`'a test paketleri (`NetArchTest.Rules`, `xunit`, `Microsoft.NET.Test.Sdk`, `NSubstitute`) | `dotnet restore` = 0 NU1008 |
| 2 | 8x8 matris tek kaynak: `l4->l2 X`, `l5->l4 X`, `l3` yalnız `l0` | matris = NetArchTest `LayerRulesTests` |
| 3 | "Yedi katman" → "Sekiz katman (l0..l7)" + yön satırı matrisle aynı | tarama 0 yanlış negatif |
| 4 | `architecture/catalog/index.md`'e **N ↔ klasör** eşleme tablosu | yeni tip yeri belirsiz kalmaz |
| 5 | Kanıt komutlarında tarama yolu gerçek proje adları | `src/**/Domain/**` sahte PASS üretiyor |
| 6 | Desktop designer baciği + testlerde çift TFM + `l0` portları (`IHttpGateway`, `IAgentStreamSink` …) | `net48` kolu NetArchTest'te kör kalmaz |

**Sonuç tablosu:**

| Alan | Değer |
|---|---|
| Kabul edilen | **A = ADR-013 birebir + 6 şart** |
| Kayıt dosyası | [.decisions/accepted/ADR-028-layered-baseline-9-projects.md](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) |
| Toplam oy | **3 tur · 21 ajan** (10 + 5 + 6) |
| Red edilen | C (ikinci sayım birimi) · B (evrensel red, ~1150 hedef) |
| Kapanış | 6 şart GATE-004a kapanışından önce; takip eden kapı [WORKFLOW.md](../governance/workflow.md) GATE-004a → 003 → 004 |

### §6.5 Tur 3 itirazlarının 6 şarta dönüşümü (arbitraj kanıtı)

Tur 3'te **hiçbir** katılımcı A'ya karşı çıkmadı; 6 itirazın tamamı **şarta** yazıldı. Bu, "oy birliği"nin nasıl **koşullu kabule** dönüştüğünü gösterir.

| # | Katılımcı | İtaz | Nereye yazıldı (§6.4 şartı) |
|---|---|---|---|
| 1 | junior #1 | K ↔ l eşlemesi yok · designer çift TFM · `TreatWarningsAsErrors` × `.Designer.cs` | Şart 4 · Şart 6 |
| 2 | junior #2 | "Yeni tip nereye yazılır" belirsiz · sahte PASS · test/Desktop TFM uyuşmazlığı (`NU1201`) | Şart 4 · Şart 5 · Şart 6 |
| 3 | junior #3 | `l4` şişmesi · `l6`'ya yığılma · Desktop/Cli ortak sözleşmesi | Şart 6 (+ ADR-028 §4 risk satırı) |
| 4 | ajans #1 | Termin (3 paralel akış) + GATE-004a/003/004 takılmaları | [WORKFLOW.md](../governance/workflow.md) §2 süre bütçesi + §2.7 |
| 5 | ajans #2 | Ekranlar 2 sözleşme olmadan açılmaz | GATE-003 paketi (ekran + kontrol envanteri) |
| 6 | ajans #3 | Gate takılmaları + **5440 eşiği 0 marj** | Şart 5 (semantik denetim) + ADR-028 §4 "Eksi" |

> **DERİN DÜŞÜNME:** Bu kaydın gücü oy çokluğunda değil, **her turda kanıtın değişmesindedir** (S2 dosya satırı: 618/616, 5440, ~1150). Oy S4'tür; S2 olmadan tur 2 ve tur 3 hiç olmazdı.

---

## §7 Doğrulama

| # | Kontrol | Beklenen | Kanıt |
|---|---|---|---|
| 1 | Frontmatter 11 alan | `authority: primary` · `updated: 2026-10-07` · `category: ".ai"` · `type: knowledge` | İlk satır `---` |
| 2 | 5 tetik kriteri tanımlı | Çelişki · 3. düzeltme · pahalı karar · kanıtsızlık · yıkıcı | §1.1 |
| 3 | Protokol dışı durumlar yazılı | 4 satır | §1.2 |
| 4 | 5 adım sıralı ve atlanamaz | Kaynak → Alternatif → Kanıt → Karar → Risk | §2.1 + §2.2 |
| 5 | Durma noktaları | 6 satır (`DECISION BLOCKED` … `BEKLEMEDE`) | §2.8 |
| 6 | Karar kutusu 6 alan | Hiçbiri `—` değil | §3.1 |
| 7 | ADR çevirme kuralı | Ne zaman ADR / ne zaman değil | §3.2 |
| 8 | Kanıt hiyerarşisi | S1 > S2 > S3 > S4 + çakışma tablosu | §4.1 |
| 9 | 7 doğrulama etiketi | `REPOSITORY_VERIFIED` … `UNKNOWN` | §4.2 |
| 10 | Tuzak listesi | ≥ 12 madde, hepsi yasak | §5.1 |
| 11 | Erken uyarı sinyalleri | ≥ 6 madde | §5.2 |
| 12 | Örnek gerçek | ADR-028 · 3 tur · 21 ajan · gerçek sayılar | §6 + ADR-028 §3 |
| 13 | Örnek sayıları kaynaklı | Tur 1: C=6/A=4/B=0 · Tur 2: A=4/C=1 · Tur 3: 6/6 A | §6.2 = ADR-028 §3 |
| 14 | Uydurma sayı yok | Bilinmeyen her yerde `DOĞRULAMA GEREKLI` | §4.3 |
| 15 | Linkler izinli hedefe | Bozuk link **0** | link taraması |
| 16 | Saf ASCII | `\| - +` dışında kutu çizim **0** · emoji **0** | `U+2500-U+257F` taraması |
| 17 | Boyut bandı | **30–88 KB** | dosya ölçümü |
| 18 | §0.1 prompt kaynağı akışı (ai.08 §71) eklendi | UNDERSTAND→ANALYZE→MODEL→SEPARATE→VALIDATE→PLAN→APPROVE→IMPLEMENT + 7 yasak | §0.1 |

---

## §8 Referanslar

| # | Kaynak | Ne alındı | Yol |
|---|---|---|---|
| 1 | G1..G12 · kanıt formatı · 7 etiket · dur/bekle | §1 · §4 | [CLAUDE.md](../claude.md) |
| 2 | Gate zinciri · kanıt kutusu · kapatma yetkisi | §2.8 · §3.3 | [WORKFLOW.md](../governance/workflow.md) |
| 3 | Ajan matrisi · kutu · 3-STRIKE · bütçe | §3.1 · §5.3 | [AGENTS.md](agents.md) |
| 4 | Karar hiyerarşisi · tek soru kuralı | §2.8 · §3.1 | [ROLE.md](role.md) |
| 5 | ADR-028 (bağlayıcı karar, 6 şart, oy kaydı) | §6 | [.decisions/accepted/ADR-028-layered-baseline-9-projects.md](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) |
| 6 | ADR navigasyonu | §3.2 | [.decisions/adr-index.md](../.decisions/adr-index.md) |
| 7 | Orkestrasyon · checkpoint · hata modları | §2.1 · §5.3 | [engine.md](engine.md) |
| 8 | ADR özetleri · mühendislik kısıtları | §3.2 | [brain.md](../.decisions/brain.md) |
| 9 | Ham envanter (618/616) | §6.1 | [reports/k3-618-ham-envanter.md](../architecture/layer-5000/index.md) · [reports/index.md](../ops/log.md) |
| 10 | Yeni dosya sözleşmesi | §7 | [.templates/new-doc-contract.md](../.templates/new-doc-contract.md) · [.templates/index.md](../.templates/index.md) |
| 11 | Oturumlar arası durum · tarihli kayıt | §2.8 · §3.3 | [MEMORY.md](../memory/memory.md) · [log.md](../ops/log.md) |
| 12 | Navigasyon · terimler | §0 | [index.md](../index.md) · [glossary.md](glossary.md) · [keys.md](../governance/keys.md) |

**Doğrulama etiketleri (bu dosya için):** ADR-028 olay kaydı ve oy sayıları → `SOURCE_VERIFIED` ([.decisions/accepted/ADR-028-layered-baseline-9-projects.md](../.decisions/accepted/adr-028-layered-baseline-9-projects.md) §1, §3) · kanıt etiketleri → `SOURCE_VERIFIED` ([CLAUDE.md](../claude.md) §3.1) · adım bütçeleri (≤ 65 dk dahil) → `INFERRED` (tasarım eşiği, ölçülmüş süre değil) · protokol adımları ve soru seti → `INFERRED` (ilk gerçek kullanımda ADR ile sabitlenecek).
| VISION | [VISION](vision.md) | urun vizyonu — 7 ekran, Studio/IDE kimligi, 4 farklilastirici |
| PROJECTS | [PROJECTS](projects.md) | proje envanteri — 9+1-2 proje, dis bagimliliklar, risk defteri |
| ADR-031 | [ADR-031](../.decisions/draft/adr-031-visual-production-provider.md) | gorsel uretim saglayicisi ADR (status: proposed, kapsam A+B+C) |
| kok README | [kok README](../../readme.md) | depo README — hizli baslangic, Studio/IDE kimligi |
| kok CLAUDE | [kok CLAUDE](../../claude.md) | kok kural dosyasi (golge — kural metni degil, baglanti) |
| kok AGENTS | [kok AGENTS](../../agents.md) | kok ajan golgesi (Chat!=Agent!=LLM baglantisi) |
| kok WORKFLOW | [kok WORKFLOW](../../workflow.md) | kok kapi ozeti (gate sirasi tek satir) |
