---
title: "VersaCoder — Akış (Flow) Şablonu"
type: template
category: templates/ui-design
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "akis sablonu: baslangic-bitis, adim DAG'i, girdi-cikti, karar noktalari, hata yollari, cancellation, timeout ve audit"
---

# VersaCoder — Akış (Flow) Şablonu

**Zorunlu Bağlantılar:** [index](../index.md) · [wireframe](./wireframe-template.md)

---

## §1 Amaç

Bu şablon `.ai/ui-design/flow/` altındaki **akış dosyalarını** tek biçimde yazdırır: bir işlemin **başlangıcından bitişine** kadar adım adım DAG'ı, girdi/çıktıları, karar noktalarını, hata yollarını, cancellation ve timeout kurallarını, rol ayrımını (kullanıcı / agent / sistem) ve audit noktalarını tek dosyada toplar. Tel kafes **yeri**, şartname **kontrolü**, flow ise **zamanı** anlatır.

| Alan | Değer |
|------|-------|
| Template Name / Path | `flow-template.md` · `.ai/.templates/ui-design/` |
| Hedef Dosya | `.ai/ui-design/flow/<akis-adi>.md` |
| Tür | `T-UX` (envanter Part C, satır #27) |
| Diyagram | ASCII DAG kutusu — ` ```text `, ≤ 66 sütun |
| Katman Oku | `l7→l6→l5→l4→l2→l1→l0` · `l3` yalnız `l0`'a bakar · `l2↔l4` doğrudan ok YOK |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Kapı | `GATE-003` — flow dosyaları **kapı muafidir** (§1.3) |
| Dil / Versiyon / Authority | Türkçe (ç ğ ı İ ö ş ü) · 1.0.0 · **SSOT** · 2026-10-05 |

### §1.1 Flow Ne İşe Yarar / Ne Değildir?

| YAPILIR | YAPMAZ |
|---------|--------|
| Başlangıç → adımlar → **son durum** zinciri | Ekranın görünümü (→ `wireframe`) |
| Karar noktaları (`<>` evet/hayır dalları) | Kontrol envanteri (→ `screen-spec`) |
| Hata yolu ve geri dönüş okları | Hex renk / token (→ `reference`) |
| Cancellation + timeout değerleri | Kod / `async` implementasyonu (`src/**` yasak) |
| Rol sütunu: kullanıcı / agent / sistem | Domain invariant tanımı (→ `l1`) |
| Audit noktaları (neyin, ne zaman kaydı) | Log altyapısı kodu (→ `l3` Crosscutting) |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt; kanıt yoksa uydurulmaz. Ölçüm: `C:\versacoder`, 2026-10-05.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/ui-design/flow/*.md` (akış dosyaları) | ❌ YOK | `Test-Path = False`, 0 dosya |
| 2 | `.ai/.templates/ui-design/flow-template.md` | ❌ YOK | envanter #27 `PLANLI · 12,0` |
| 3 | `.ai/.templates/ui-design/wireframe-template.md` | ✅ DOĞRULANDI | 642 satır / 34,5 KB |
| 4 | `.ai/.templates/index.md` §3.4 satır 217 | ✅ OKUNDU | hedef `.ai/ui-design/flow/*.md` |
| 5 | `.ai/WORKFLOW.md` (kapı zinciri kaynağı) | ❌ YOK | `.ai/` altında yalnız `.templates` var |
| 6 | `.ai/.rules/` · `.ai/.decisions/` · `.ai/log.md` | ❌ YOK | envanter beyanı → ⚠️ VERIFICATION REQUIRED |
| 7 | `GATE-003` kayıt satırı | ❌ YOK → ⚠️ | kapı henüz alınmadı |
| 8 | `src/**` (akışın kod karşılığı) | ❌ YOK | `src/` = `Test-Path False` |
| 9 | Katman tanımı `.ai/architecture/index.md` | ❌ YOK | ⚠️ — `l0..l7` burada tanım kaynağı |
| 10 | Harici vault `C:\www\…` (envanter G1) | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı, cümle kopyalanmaz |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır. `.ai/index.md`, `.ai/CLAUDE.md`, `.ai/WORKFLOW.md`, `.ai/.rules/`, `.ai/.decisions/`, `.ai/architecture/`, `.ai/keys.md`, `.ai/log.md` yolları **kırık sayılır**; metin olarak değinilir, bağlantı verilmez.

### §1.3 GATE-003 Kapsamı — Flow Muafiyeti

Kapı yasağı **yalnız gerçek ekran dosyasını ve `src/**` kodunu** kapsar. Bu şablon bir şablondur: kapı alınmadan da yazılır. Kök `CLAUDE.md` G8 "ekran/kod üretilmez" der — **akışı anlatan bu şablon ne ekran ne koddur.**

```text
+------------------------------------------------------------------+
| [1] bu dosya (akis Sablonu)   KAPI MUAF      -- yazilabilir      |
|     v                                                            |
| [2] .ai/ui-design/flow/*.md   KAPI MUAF      -- akis anlatimi    |
|     v                                                            |
| [3] .ai/ui-design/01..07 + screen/  KAPI GEREKIR -- onay         |
|     v                                                            |
| [4] src/** handler kodu       KAPI SONRASI   -- onaysiz yazilmaz |
+------------------------------------------------------------------+
```

| Öğe | GATE-003 kapsamı | Bu şablon |
|-----|------------------|-----------|
| `.ai/.templates/ui-design/flow-template.md` | ❌ kapsam dışı | ✅ serbest |
| `.ai/ui-design/flow/<akis>.md` (gerçek dosya) | ❌ kapsam dışı | ✅ serbest |
| `.ai/ui-design/screen/*.md`, `01..07` tel kafes | ✅ kapsar | ⛔ kapı öncesi yazılmaz |
| `src/**` handler / form kodu | ✅ kapsar | ⛔ kapı öncesi yazılmaz |

**Kural:** Flow dosyaları kapıdan bağımsız ilerler; ancak flow'da adı geçen **ekran** GATE-003'te onaylanmadan o ekranın kodu yazılmaz (§6.3).

### §1.4 Neden Ayrık Bir Akış Dosyası?

| Boşluk | Belirti | Bu Şablonun Kestiği Şey |
|--------|---------|--------------------------|
| Akış koddan okunuyor | "nerede dallanıyor?" 40 dk | §3.3 DAG — tek bakışta |
| Hata yolu yok | başarı yolu tek yol sanılır | §3.6 her adımda hata dalı |
| İptal tanımsız | ESC çalışmıyor, iş yarıda kalır | §3.7 cancellation + §3.8 timeout |
| Rol belirsiz | agent mı kullanıcı mı karar verir | §3.9 üç sütun |
| Kayıt yok | "neden sildin?" cevabı yok | §3.10 audit noktaları |

---

## §2 Kapsam

**İçinde:** akış dosyasının yazımı, başlangıç–bitiş sözleşmesi, adım DAG'ı, girdi/çıktı, karar noktaları, hata yolları, cancellation, timeout, rol ayrımı, audit noktaları, salt-okunur doğrulama.
**Dışında:** tel kafes (`./wireframe-template.md`), ekran şartnamesi, renk/token, prompt, `*.cs` kodu, SQL/kalıcılık.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| Adım DAG'ı + karar dalları | Kontrol ID'si / binding (→ `screen-spec`) |
| Girdi → çıktı sözleşmesi | Gerçek veri şeması (→ `../query/Query-Template.md`) |
| Hata yolu + geri dönüş oku | Hata metinlerinin kaynak kodu (→ `l2` handler) |
| Cancellation + timeout değerleri | `CancellationToken` kodu (→ `src/**`) |
| Rol sütunu (kullanıcı/agent/sistem) | Rol yetki tablosu (→ `l1` Domain) |
| Audit noktası + kaydı gereken olay | Log altyapısı (→ `l3` Crosscutting) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Hedef | Sorumlu | Bu Şablondan mı |
|------------|-------|---------|------------------|
| `.ai/ui-design/flow/<akis>.md` | akış diyagramı | ui-designer | ✅ **bu şablon** |
| `.ai/ui-design/01…07-*.md` | tel kafes | ui-designer | ❌ → `./wireframe-template.md` |
| `.ai/ui-design/screen/*.md` | ekran şartnamesi | ui-designer | ❌ → `screen-spec` |
| `.ai/ui-design/prompts/*.md` | üretim promptu | ui-designer | ❌ → `prompt` |
| `src/**/*Handler.cs` | akışın kodu | developer | ❌ GATE-003 sonrası |
| `src/**/XForm.cs` | olay bağları | developer | ❌ → [../frontend/form-template.md](../frontend/form-template.md) |
| `.ai/reports/*.md` | akış denetim raporu | docs-writer | ❌ `.ai/reports/` diskte YOK |
| `tests/VersaCoder.Tests/**` | akış testi | testing | ❌ → [../testing/unit-test-template.md](../testing/unit-test-template.md) |

### §2.2 Kapsam Sınırı Testi

Üretilecek dosyaya sırayla sor; **ilk `EVET`** şablonu belirler:

1. **Başlangıç → adımlar → son durum** mu? → **bu şablon**.
2. Ekranın **yerleşimi** mi? → `./wireframe-template.md`.
3. Ekranın **kontrol/durum dökümü** mı? → `screen-spec`.
4. **Renk / token / ikon** mu? → `reference`.
5. **Agent komutu / prompt metni** mi? → `prompt`.
6. **`.cs`** mi → ❌ kapı geçmeden üretilmez (§1.3).

`HAYIR` soruları: yasaklı vault cümlesi mi (G1)? · EF Core / yıldızlı sorgu / string-concat SQL var mı (G2)? · `{{…}}` yayımlanacak mı (G5)? · `l2↔l4` doğrudan ok çizildi mi (G8)?

---

## §3 Mimari

Bu bölüm hem **şablonun yapısını** hem **üretilecek akış dosyasının yapısını** tanımlar. DAG kutuları ` ```text ` içinde ve **≤ 66 sütun** yazılır.

### §3.1 Dosya ve Akış Kimliği

| Alan | Kural | Doğru | Yanlış |
|------|-------|-------|--------|
| Dosya adı | `.ai/ui-design/flow/<akis-adi>.md`, kebab-case | `flow/project-create.md` | `flow/Akis1.md` |
| Akış kodu | `FL-NN` | `FL-02` | `F2` |
| Başlık | "Ne → Ne" biçiminde tek satır | "Proje oluştur → kaydet" | "Akış" |
| Sahip | tek rol | `kullanıcı + l2 handler` | üç rol belirsiz |
| Girdi kaynağı | bölüm/ad alan adı | `{{Girdi_Adlari}}` | "form" |
| Son durum | iki değerden biri | `başarı / hata / iptal` | boş |
| İlgili ekran | `SP-NN` + `WF-NN` | `SP-02 · WF-02` | — |
| Kapı | muafiyet satırı | `GATE-003: şablon muaf (§1.3)` | satır yok |

### §3.2 Başlangıç–Bitiş Sözleşmesi

Her akış **tek bir başlangıç** ve **üç bitişten biri** ile biter; döngü açık uçlu bitemez.

| Öğe | Kural | Zorunlu Değer |
|-----|-------|---------------|
| Başlangıç | `<> BAŞLANGIÇ` — kullanıcı niyeti ya da sistem olayı | 1 adet |
| Bitiş A | `✓ BAŞARI` — çıktı üretilir | 0..1 |
| Bitiş B | `✗ HATA` — kod + mesaj + geri dönüş hedefi | 0..n |
| Bitiş C | `× İPTAL` — cancellation ile temiz çıkış | 0..1 |
| Zorunlu | her yol en az bir bitişe ulaşır | §5.3/2 = 0 |
| Yasak | `... devam eder` gibi bitmeyen dal | RED |

### §3.3 Adım DAG'ı (ASCII kutu)

```text
+------------------------------------------------------------------+
| FL-02 · Proje Olustur → Kaydet                    [1024x600] ref |
+------------------------------------------------------------------+
| <> BASLANGIC  kullanici "Yeni Proje" tiklar                       |
|    v                                                             |
| [1] girdi topla         (kullanici)   veri: ad, sablon, yol      |
|    v                                                             |
| <> KARAR  ad gecerli mi?                                         |
|    | EVET                          | HAYIR                       |
|    v                               +--> [E1] hata: gecersiz ad   |
| [2] sablonu hazirla     (agent)     -> bitis: HATA               |
|    v                                                             |
| <> KARAR  dosya yolu yazilabilir mi?                             |
|    | EVET                          | HAYIR                       |
|    v                               +--> [E2] hata: yetki yok     |
| [3] kaydet              (sistem)    -> bitis: HATA               |
|    v                                                             |
| <> BASARI  proje listesine eklenir  (+ AUDIT-1)                  |
|                                                                     |
| iptal: [1] veya [2] adiminda ESC -> bitis: IPTAL (temizlik zorunlu)|
+------------------------------------------------------------------+
```

| Kural | Test |
|-------|------|
| Tüm kutular ` ```text ` içinde | Markdown dönüştürücü bozmasın |
| Satır uzunluğu ≤ 66 karakter | §5.3/1 = 0 satır |
| Adımlar `[N]`, kararlar `<>`, bitişler `✓ ✗ ×` | §3.2 |
| Hata dalları sağa, ana dal aşağı | okuma yönü tek |
| Unicode kutu çizim (`U+2500-U+257F`) | **YASAK** — `-` `+` `|` |

### §3.4 Girdi / Çıktı Tablosu

| # | Adım | Girdi | Kaynak | Çıktı | Giden Yer |
|---|------|-------|--------|-------|-----------|
| 1 | `[1]` | `{{ad}}`, `{{sablon}}`, `{{yol}}` | kullanıcı | `{{ProjeTaslagi}}` | `[2]` |
| 2 | `[2]` | `{{ProjeTaslagi}}` | `l2` use case | `{{HazirPaket}}` | `[3]` |
| 3 | `[3]` | `{{HazirPaket}}` | `l4` repository | `{{ProjeId}}` | `✓ BAŞARI` |
| E1 | `[E1]` | `{{ad}}` | `l7` kontrol | `INVALID_NAME` | `✗ HATA` |
| E2 | `[E2]` | `{{yol}}` | `l4` dosya sistemi | `ACCESS_DENIED` | `✗ HATA` |
| C | `ESC` | — | `l7` klavye | `CANCELLED` | `× İPTAL` |

**Kurallar:** (a) Girdi sütununda **yalnız ad** yazılır — SQL, tablo, `DbContext`, sorgu metni yazılmaz (G2). (b) "Kaynak" sütunu yalnız katman adı ya da `lN` verir (`l2↔l4` doğrudan ok yok, G8). (c) Çıktı adı `UPPER_SNAKE` olur ve hata kodu listesiyle eşleşir. (d) Her satır bir sonraki adıma bağlanır; asılı satır yoktur.

### §3.5 Karar Noktaları

| # | Soru | Tip | Dal Sayısı | Veren | Varsayılan Dal |
|---|------|-----|-----------|-------|----------------|
| K1 | `{{girdi}}` geçerli mi? | anlık (saf) | 2 | `l7` | HAYIR → `[E1]` |
| K2 | `{{kaynak}}` var mı? | I/O | 2 | `l4` | HAYIR → `[E2]` |
| K3 | Kullanıcı onayladı mı? | modal | 2 | `l7` | HAYIR → `× İPTAL` |
| K4 | `{{rol}}` yetkili mi? | Domain | 3 | `l1` | İZİNLİ/DEĞİL + `?` |
| K5 | Daha önce yapılmış mı? | tekrar | 2 | `l2` | EVET → uyarı, HAYIR → devam |

**Kurallar:** (a) Her kararın **iki dalı** en az yazılır; tek dallı karar = RED (§6.2 R3). (b) Karar veren katman `l7` (arayüz), `l1` (kural) veya `l4` (kaynak) olabilir — `l2` iş akışını **taşıdır**, karar **vermez**. (c) Karar sorusu tek cümle ve **evet/hayır** ile cevaplanabilir olmalıdır. (d) Bilinmeyen dal (`?`) varsa §3.6'ya bağlanır.

### §3.6 Hata Yolları

| # | Hata | Tetikleyen | Kod | Geri Dönüş | Kullanıcıya Gösterim |
|---|------|-----------|-----|-----------|----------------------|
| E1 | Geçersiz girdi | `l7` anlık doğrulama | `INVALID_INPUT` | `[1]` (aynı alan odaklı) | alan yanındaki mesaj |
| E2 | Kaynak yok / yetki yok | `l4` | `NOT_FOUND` / `ACCESS_DENIED` | `[1]` | modal + açıklama |
| E3 | Çakışma (kopya anahtar) | `l2` handler `failure` | `DUPLICATE` | `[1]` | modal + mevcut kayıt |
| E4 | Beklenmeyen hata | `l3` yakalama | `UNEXPECTED` | `[2]` (yeniden dene) | kod + "Tekrar dene" |
| E5 | Zaman aşımı | §3.8 | `TIMEOUT` | `[2]` | "Zaman aşımı — tekrar dene" |
| E6 | İptal edildi | §3.7 | `CANCELLED` | temizlik → `× İPTAL` | sessiz (yalnız durum çubuğu) |

**Zorunluluklar:** (1) Her adımın **en az bir** hata dalı vardır; hattı olmayan adım §6.1 K10'da düşer. (2) Hata dalı asla `catch { }` gibi sessiz kapanmaz — kod + mesaj + geri dönüş hedefi **üçü de** yazılır. (3) Ekran **kapanmaz**; yalnız durum değişir. (4) Hata kodu listesi `l2`'de yaşar, metni `l7`'de.

### §3.7 Cancellation

| Kural | Değer / Davranış |
|-------|------------------|
| Tetikleyici | `Esc`, "İptal" butonu, pencere kapanışı |
| İzin verilen adımlar | `[1]`, `[2]` — `[3]` (kaydet) **yarıda kesilemez** |
| Davranış | iş durur, kısmi çıktı **geri alınır**, kaynak `Dispose` edilir |
| Temizlik | `finally` eşdeğeri zorunlu — masa belleği / geçici dosya bırakılmaz |
| Kullanıcıya dönüş | `× İPTAL` bitişi, mesaj **yok** (yalnız durum çubuğu) |
| Audit | iptal **kaydedilmez**; yalnız §3.10/A3 kuralı uygulanır |
| İptal edilemez adım | `[3]` — iptal isteği kuyruğa alınır, işlem **bitirilir** |

### §3.8 Timeout

| Adım | Eşik | Aşım Davranışı | Kod | Yeniden Dene |
|------|------|----------------|-----|--------------|
| `[1]` anlık girdi | 0 sn (eşik yok) | — | — | — |
| `[2]` uzun işlem | **30 sn** | iptal + `E5` | `TIMEOUT` | evet, 1 kez |
| `[3]` kayıt | **60 sn** | işlemi **bitir**, geri bildirim beklenmez | `TIMEOUT` | hayır (tekrar kayıt riski) |
| agent çağrısı | **90 sn** | agent'i durdur, `E5` | `TIMEOUT` | evet, 2 kez |
| Dosya okuma/yazma | **10 sn** | `E2` | `NOT_FOUND` | evet |

**Kural:** timeout değeri **sayı ile** yazılır (ör. `30 sn`); "biraz bekler" gibi belirsiz ifade RED (§6.2 R6). Zaman aşımında **kaynak serbest bırakılır** ve kullanıcıya tek bir geri bildirim gösterilir (çift toast yasak).

### §3.9 Rol Ayrımı — Kullanıcı / Agent / Sistem

Her adıma **tek bir** rol atanır; sütun asla boş kalmaz ve iki rol birden yazılmaz.

| Rol | Ne Yapar | Ne Yapmaz | Örnek Adım |
|-----|----------|-----------|-----------|
| **kullanıcı** | niyet bildirir, girer, onaylar | iş kuralını uygulamaz | `[1] girdi topla`, `K3` |
| **agent** | üretir, dönüştürür, önerir | kalıcı kayıt yapmaz | `[2] şablonu hazırla` |
| **sistem** | kaydeder, doğrular, kapatır | kullanıcıya soru sormaz | `[3] kaydet`, `AUDIT-1` |

| Kural | Test |
|-------|------|
| Her adımda tek rol | §5.3/4 |
| Agent adımı `l2` üzerinden çağrılır | G8 — `l7` agent'e **doğrudan** bakmaz |
| Sistem adımı kullanıcı onayı **istemez** | §3.5 K3 yalnız kullanıcı kararında |
| İki rol tek satırda | §6.2 R5 — satır ikiye bölünür |

### §3.10 Audit Noktaları

| # | Nokta | Ne Kaydedilir | Ne Kaydedilmez | Yazan Katman |
|---|-------|---------------|----------------|--------------|
| A1 | `[1]` girdi kabulü | — | parola / anahtar / token | — |
| A2 | `K4` yetki reddi | rol + hedef + zaman | parola özeti | `l3` |
| A3 | `× İPTAL` | yalnız **canlı** iptal (kullanıcı `[3]` öncesi iptal) | her ESC | `l3` |
| A4 | `✗ HATA` (E3, E4) | kod + adım + correlation id | ham istek gövdesi | `l3` |
| A5 | `✓ BAŞARI` + `[3]` | kim · ne · ne zaman · sonuç | dosya içeriği / BLOB | `l4` → `l3` |

**Kurallar:** (a) Audit kaydı **işlemle birlikte** düşünülür; kayıt başarısızsa iş **geri alınır**. (b) Hassas alan (parola, anahtar, token) **asla** audit'e girmez. (c) Her audit satırında correlation id vardır; §5.3/5 bunu arar. (d) Audit noktası olmayan **kayıt eden** adım yoktur (A5 zorunlu).

### §3.11 Dolu Örnek ve Tam Kalıp

**Dolu örnek — `FL-05 · Doküman dışa aktarma`:**

```text
+------------------------------------------------------------------+
| FL-05 · Disa Aktarma -> Dosya                        [1024x600] |
+------------------------------------------------------------------+
| <> BASLANGIC  kullanici "Disa Aktar" secer                        |
|    v                                                             |
| [1] kapsam sec          (kullanici)  girdi: tur, tarih araligi   |
|    v                                                             |
| <> KARAR secim gecerli mi?        EVET --> [2]                   |
|    | HAYIR                              +--> [E1] INVALID_INPUT  |
|    +----------------------------------------> [1] (alan odagi)    |
| [2] uret               (agent)   30 sn esik   -> E5 TIMEOUT     |
|    v                                                             |
| [3] dosyaya yaz         (sistem)  10 sn esik   -> E2 NOT_FOUND   |
|    v                                                             |
| <> BASARI  dosya yolu gosterilir  (+ AUDIT-5)                    |
| iptal: [2] adiminda ESC -> IPTAL (agent durdurulur, dosya silinir)|
+------------------------------------------------------------------+
```

**Tam kalıp (yalnız bu blokta `{{…}}` yaşar; üretilen akışta adedi 0 olmalıdır — G5):**

````markdown
# Akış — {{FL_NN}} · {{AKIS_ADI}}

| Alan | Değer |
|---|---|
| Akış / dosya | {{FL_NN}} · `.ai/ui-design/flow/{{akis-adi}}.md` |
| İlgili ekran | {{SP_NN}} · {{WF_NN}} |
| Girdi → Çıktı | {{Girdi_Adlari}} → {{Cikti_Adlari}} |
| Bitişler | ✓ {{Basari}} · ✗ {{Hata_Kodlari}} · × İPTAL |
| Timeout | [2] {{30}} sn · [3] {{60}} sn · agent {{90}} sn |
| GATE-003 | şablon muaf (§1.3) — ekran onayı kapıda |

## DAG
```text
{{ASCII — <= 66 sutun}}
```

## Girdi / Çıktı (§3.4)
| Adım | Girdi | Kaynak | Çıktı | Yer |

## Kararlar (§3.5)
| # | Soru | Dal | Veren | Varsayılan |

## Hata Yolları (§3.6)
| # | Kod | Geri Dönüş | Gösterim |

## Cancellation + Timeout (§3.7 · §3.8)
{{adım bazlı}}

## Roller (§3.9)
{{kullanıcı / agent / sistem — her adımda tek}}

## Audit (§3.10)
{{A1..A5}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası ve Öncelik

| Öncelik | Şart | Yapılacak |
|---------|------|-----------|
| 1 | İlgili ekran tel kafesi yoksa | not düşülür, akış **yazılabilir** (§1.3) |
| 2 | Kimlik satırı (§3.1) | 8 alan dolu |
| 3 | Başlangıç–bitiş (§3.2) | 1 başlangıç + 3 bitiş tipi |
| 4 | DAG (§3.3) | ` ```text `, ≤66, `[N]` `<>` `✓✗×` |
| 5 | Girdi/çıktı (§3.4) | her satır bağlı, SQL yok |
| 6 | Kararlar (§3.5) | her karar ≥ 2 dal |
| 7 | Hata yolları (§3.6) | her adımda ≥ 1 dal |
| 8 | Cancellation + timeout (§3.7–§3.8) | sayısal eşikler |
| 9 | Roller (§3.9) + audit (§3.10) | her adımda tek rol |
| 10 | Ölçüm + denetim | §5.3 = 0 · §6.1 tam |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | ui-designer | akış dosyaları + §5.3 çıktısı | `.ai/ui-design/flow/*.md` | 34 dk |
| 2 | reviewer | §6.1 (20 madde) | ✅ / RED | 10 dk |
| 3 | architect | katman oku `l7→…→l0` + `l2↔l4` yasağı | yön tutarlılığı | 8 dk |
| 4 | **kullanıcı (GATE-003)** | tel kafes + şartname (flow muaf) | kapı kaydı | 20 dk |
| 5 | master-orchestrator | §5.4 Quality Report | `log.md` satırı | 6 dk |
| | **Toplam** | | | **≈ 78 dk** |

**3 RED** sonrası iş durur, gerekçe `master-orchestrator`'a yükseltilir.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Satır sayısı | ≥ 480 | < 480 = **RED** (G6) |
| Dosya boyutu | ≥ 27,0 KB | < 27,0 KB = RED |
| Adım sayısı / akış | 3–12 | > 15 → iki akışa böl |
| Karar sayısı | ≥ 1 | 0 = RED |
| Hata dalı | her adımda ≥ 1 | eksik = RED |
| Kutu satırı | ≤ 66 karakter | > 66 = düzelt |
| Timeout | sayısal (`30 sn`) | belirsiz = RED |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | `updated` | Kayıt |
|------|---------|-----------|-------|
| Yalnız yazım/ok yönü düzeltmesi | dosya içinde | değişmez | §5.3 yeniden |
| Adım eklendi/çıkarıldı | DAG + §3.4 + §3.6 | artar | `log.md` tek satır |
| Karar/hata dalı değişti | §3.5 + §3.6 + §6.1 | minor sürüm | reviewer |
| Timeout değeri değişti | §3.8 + akış testi notu | minor sürüm | architect |
| İlgili ekran değişti | SP ↔ FL çapraz kontrol | artar | kapı kaydı güncellenir |
| Kapı sonrası handler kodu yazıldı | flow dondurulur | patch | talep kapı üzerinden |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| **G1** | Yasaklı vault (`C:\www\…`) cümlesi kopyalamak | **DUR** — sil, özgün yaz; yalnız yapı referansı |
| **G2** | EF Core / yıldızlı sorgu / string-concat SQL / girdi sütununda tablo adı | **DUR** — ADO.NET + Dapper, `@p1..@pN` |
| **G3** | Akışta hex renk, tema, font adı | **DUR** — `reference` şablonuna taşı |
| **G4** | Kapı öncesi `src/**` kodu ya da gerçek ekran dosyası | **DUR** — §1.3, iş geri alınır |
| **G5** | Üretilen akışta `{{…}}` | **DUR** — §3.11 kalbı doldurulur |
| **G6** | Satır < 480 ya da KB < 27,0 | **DUR** — §4.3 |
| **G7** | Kırık bağlantı (`.ai/index.md`, `.ai/CLAUDE.md`, `.ai/WORKFLOW.md`, `.ai/.rules/`, `.ai/.decisions/`, `.ai/architecture/`, `.ai/keys.md`, `.ai/log.md`, `.ai/ui-design/`, `.ai/reports/`) | **DUR** — bağlantı kaldırılır, metin yazılır |
| **G8** | `l2↔l4` doğrudan ok ya da `l3`'ün `l0` dışına bakması | **DUR** — §3.4 kaynak sütunu |
| **G9** | Tek dallı karar / bitmeyen dal / bitişsiz yol | **DUR** — §3.2, §3.5 |
| **G10** | Sayısız timeout (`biraz bekler`) | **DUR** — §3.8 |
| **G11** | Adımda rol sütunu boş ya da çift rol | **DUR** — §3.9 |
| **G12** | Kanıtsız "akış doğrulandı" iddiası | **DUR** — §5.3 çıktısı + §3.10 audit kanıtı |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış Yazımı

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Akış listesini belirle | ekranlar + use case listesi | 3–6 `FL-NN` | ui-designer | 5 dk |
| 2 | Kimlik + bitişler | §3.1 · §3.2 | başlık tablosu | ui-designer | 5 dk |
| 3 | DAG çizimi | §3.3 | ASCII ≤66 | ui-designer | 9 dk |
| 4 | Girdi/çıktı | §3.4 | 4–8 satır | ui-designer | 5 dk |
| 5 | Kararlar | §3.5 | ≥1 karar, 2 dal | ui-designer | 5 dk |
| 6 | Hata yolları | §3.6 | her adımda ≥1 | ui-designer | 6 dk |
| 7 | Cancellation + timeout | §3.7 · §3.8 | sayısal eşik | ui-designer | 4 dk |
| 8 | Roller + audit | §3.9 · §3.10 | tek rol/adım | ui-designer | 5 dk |
| 9 | Ölçüm | §5.3/1–6 | 0 satır | ui-designer | 4 dk |
| 10 | Denetim | §6.1 (20 madde) | ✅ / RED | reviewer | 10 dk |
| | **Toplam** | | | | **≈ 58 dk** |

**Bütçe:** 1–8 ≈ 44 dk, 9–10 ≈ 14 dk. Tek akış 25 dakikayı aşarsa §2.2'ye dön: ekran detayı ya da renk tartışması sızmıştır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Bitişsiz yol | ok hiçbir `✓✗×`'e ulaşmıyor | DAG eksik çizilmiş | §3.2 |
| 2 | Tek dallı karar | `<>` var, tek ok var | dal atlanmış | §3.5 ≥2 dal |
| 3 | Hattı olmayan adım | `[2]` hata yazmıyor | hata yolu sona bırakılmış | §3.6 |
| 4 | Çift rol | satırda "kullanıcı+sistem" | rol sütunu birleştirilmiş | §3.9 satırı böl |
| 5 | SQL sızması | girdi sütununda tablo/sorgu | katman karışmış | G2, `l7→…→l0` |
| 6 | Belirsiz timeout | "uygun süre beklenir" | eşik yazılmamış | §3.8 sayısal |
| 7 | `l2↔l4` doğrudan ok | handler doğrudan repo'ya | katman unutulmuş | G8, `l5`/`l4` ara |
| 8 | Yer tutucu sızıntısı | `{{ADIM}}` yayımlanmış | §3.11 doldurulmadı | G5 |
| 9 | Kutu > 66 | hizalama bozuk | metin taşmış | kısalt (§5.3/1) |
| 10 | Kapı karıştırılması | flow için kapı beklenmiş | §1.3 okunmamış | muafiyet tablosu |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; dizin yoksa dürüstçe `DIZIN YOK` yazar.

```powershell
# 1) YAPI — frontmatter 8 alan (scope) + 7 bolum + zorunlu baglanti
$need='title','type','category','status','version','updated','authority','scope'
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $r=[IO.File]::ReadAllText($_.FullName)
  $m=$need | Where-Object { $r -notmatch ('(?m)^'+$_+':') }
  $s=([regex]::Matches($r,'(?m)^## \u00A7')).Count
  $b=([regex]::Match($r,'(?m)^\*\*Zorunlu Ba\u011Flant\u0131lar:\*\*(.*)$')).Groups[1].Value
  if ($m) { 'EKSIK ALAN -> '+$_.Name+': '+($m -join ',') }
  if ($s -ne 7) { 'BOLUM -> '+$_.Name+': '+$s+'/7' }
  if ($b -notmatch '\]\(\.\.\/index\.md\)' -or $b -notmatch '\]\(\.\/wireframe-template\.md\)') {
    'BAGLANTI -> '+$_.Name } }

# 2) BAGLANTI HEDEFLERI — kirik link = 0 (G7)
$base='C:\versacoder\.ai\.templates\ui-design'
Get-ChildItem "$base\*.md" | Select-String -Pattern '\]\((\.\.?\/[^)#]+)\)' -AllMatches |
  ForEach-Object { $_.Matches } | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique |
  ForEach-Object { if (-not (Test-Path (Join-Path $base $_))) { 'KIRIK -> '+$_ } }

# 3) OLCU — satir >= 480, KB >= 27,0, kutu <= 66 (G6)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $n=(Get-Content $_.FullName).Count; $kb=[math]::Round($_.Length/1KB,1)
  if ($n -lt 480) { 'KISA -> {0}: {1} satir' -f $_.Name,$n }
  if ($kb -lt 27.0) { 'KUCUK -> {0}: {1} KB' -f $_.Name,$kb }
  $box=$false; $i=0
  foreach ($l in [IO.File]::ReadAllLines($_.FullName,[Text.Encoding]::UTF8)) { $i++
    if ($l -match '^```text') { $box=$true; continue }
    if ($l -match '^`{3,}' -and $box) { $box=$false; continue }
    if ($box -and $l.Length -gt 66) { '{0}:{1} kutu={2} (>66)' -f $_.Name,$i,$l.Length } } }

# 4) YASAK ICERIK — EF / yildizli sorgu / string-concat = 0 (G2)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' |
  Select-String -Pattern 'EntityFramework|DbConnection|SELECT\s+\+|CONCAT\s*\(' |
  ForEach-Object { 'YASAK SQL -> {0}:{1}' -f $_.Filename,$_.LineNumber }

# 5) YER TUTUCU + KODLAMA — BOM/mojibake 0 (G5)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' | ForEach-Object {
  $b=[IO.File]::ReadAllBytes($_.FullName)
  if ($b.Length -ge 3 -and $b[0] -eq 0xEF -and $b[1] -eq 0xBB -and $b[2] -eq 0xBF) { 'BOM -> '+$_.Name }
  $t=[Text.Encoding]::UTF8.GetString($b)
  if ($t -match '\u00C3[\u0080-\u00BF]\u00C3[\u0080-\u00BF]|\uFFFD') { 'MOJIBAKE -> '+$_.Name }
  $in=$false; $i=0
  foreach ($l in ($t -split "`n")) { $i++
    if ($l -match '^`{3,}md') { $in=$true; continue }
    if ($l -match '^`{3,}' -and $in) { $in=$false; continue }
    if (-not $in -and $l -match '\{\{[A-Z0-9_]+\}\}') { 'YER TUTUCU -> {0}:{1}' -f $_.Name,$i } } }

# 6) KATMAN OKU — yasak ifadeler (l2 ile l4 arasinda dogrudan ok, l3 disina bakis)
Get-ChildItem 'C:\versacoder\.ai\.templates\ui-design\*.md' |
  Select-String -Pattern 'l2\s*(-->|->|↔)\s*l4|l3\s*(-->|->)\s*l[12567]' |
  ForEach-Object { 'KATMAN OKU -> {0}:{1}' -f $_.Filename,$_.LineNumber }
```

**Geçme:** #1–#6 hepsi **0 satır uyarı**.

### §5.4 Quality Report

```markdown
## Quality Report — flow (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (bu şablon) | <satır> satır / <KB> KB | ≥ 480 · ≥ 27,0 | 🟢/🔴 |
| Üretilen akış dosyası | n | ≥ 3 | 🟢/🔴 |
| Frontmatter 8 alan (§5.3/1) | n/8 | 8 | 🟢/🔴 |
| Kırık bağlantı (§5.3/2) | n | 0 | 🟢/🔴 |
| Kutu > 66 satır (§5.3/3) | n | 0 | 🟢/🔴 |
| EF / yıldızlı-sorgu / concat (§5.3/4) | n | 0 | 🟢/🔴 |
| Yer tutucu / BOM / mojibake | n/n/n | 0/0/0 | 🟢/🔴 |
| Katman oku ihlali (§5.3/6) | n | 0 | 🟢/🔴 |
| Checklist §6.1 | n/20 | 20/20 | 🟢/🔴 |
| Guardrail G1..G12 | ihlal n | 0 | 🟢/🔴 |

**Karar:** 🟢 kapıya sunulur / 🔴 düzeltmeye gönderildi (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (20 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 8 alan (`title…scope`) | 8/8 | §5.3/1 |
| K2 | `authority: SSOT` birebir | `SSOT` | envanter §3.7 |
| K3 | H1 + `**Zorunlu Bağlantılar:**` (index · wireframe) | 1 satır | §5.3/1 |
| K4 | `## §1…§7` | 7/7 | §5.3/1 |
| K5 | `§1.1–§1.4` dolu | 4/4 | §1 |
| K6 | `§1.3` flow muafiyeti + ekran kapsamı ayrımı | var | G4 |
| K7 | `§2.1` eşleme + `§2.2` sınır testi | 2/2 | §2 |
| K8 | `§3.1–§3.11` tam (11 alt bölüm) | 11/11 | §3 |
| K9 | DAG ` ```text ` + ≤66 + `<>`/`[N]`/`✓✗×` | 0 sapma | §3.3 |
| K10 | Her adımda ≥ 1 hata dalı | 0 eksik adım | §3.6 |
| K11 | Kararlar ≥ 2 dal | 0 tek dallı | §3.5 |
| K12 | Cancellation kuralı + iptal edilemez adım yazılı | var | §3.7 |
| K13 | Timeout sayısal (≥ 3 değer) | ≥ 3 | §3.8 |
| K14 | Her adımda tek rol (kullanıcı/agent/sistem) | 0 çift rol | §3.9 |
| K15 | Audit A1..A5 + hassas alan yasağı | 5 | §3.10 |
| K16 | Girdi/çıktı sütununda SQL/EF yok | 0 | G2 |
| K17 | `§4.1–§4.5` + G1..G12 | 5 + 12 | §4.5 |
| K18 | `§5.1–§5.4` (10 adım · ≥8 hata modu · 6 komut · report) | 4/4 | §5 |
| K19 | `§6.1–§6.5` | 5/5 | §6 |
| K20 | `§7.1–§7.2` (Red Team ≥ 10 satır) + ≥ 480 satır / ≥ 27 KB | 2/2 · 2/2 | G6 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3 komutlarından biri > 0 satır | **DUR** — gösterilen dosya düzeltilir |
| R2 | Bitişsiz yol (`✓✗×`'e ulaşmayan ok) | **DUR** — §3.2 |
| R3 | Tek dallı karar | **DUR** — §3.5 |
| R4 | Hattı olmayan adım | **DUR** — §3.6 |
| R5 | Adımda boş/çift rol | **DUR** — §3.9 |
| R6 | Belirsiz timeout (`biraz`, `uygun süre`) | **DUR** — §3.8 |
| R7 | `l2↔l4` ok ya da `l3`'ün `l0` dışına bakışı | **DUR** — G8 |
| R8 | Hex/renk/font akışta | **DUR** — `reference` |
| R9 | Yer tutucu / kırık bağlantı | **DUR** — G5 · G7 |
| R10 | Kapı öncesi `src/**` kodu | **DUR** — G4, §1.3 |

### §6.3 Kapı ve Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan | Etki |
|-------|----------------------|------------|------|
| taslak → inceleme | §5.3 = 0 · §6.1 20/20 | ui-designer → reviewer | mimariye sunulur |
| inceleme → onay | §6.2 = 0 · §5.4 `log.md`'de | architect | akış dondurulur |
| akış → GATE-003 paketi | tel kafes + şartname 7/7 | **kullanıcı** | kapı kaydı |
| kapı → kod | `GATE-003` kaydı mevcut | orchestrator | `src/**` handler serbest |
| onay → revizyon | adım/dal değişikliği | ui-designer | §6.1 + §6.5 yeniden |

Flow dosyası tek başına kapı **açmaz**; kapı paketindedir (7 tel kafes + 7 şartname). Ancak kapı öncesi flow yazılmak zorundadır — akışsız şartname, akışsız kod demektir.

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| [./wireframe-template.md](./wireframe-template.md) | ekranın **yeri** — flow onun **zamanını** anlatır |
| [../index.md](../index.md) | envanter satır 27 · §3.7 · §4.5 G1..G12 |
| [../frontend/form-template.md](../frontend/form-template.md) | olay → handler zinciri kodda burada |
| [../backend/handler-template.md](../backend/handler-template.md) | `l2` handler akışın kod karşılığı |
| [../backend/service-template.md](../backend/service-template.md) | use case akışı |
| [../backend/repository-template.md](../backend/repository-template.md) | `[3] kaydet` adımı (`l4`) |
| [../query/Query-Template.md](../query/Query-Template.md) | sorgu sözleşmesi (`@p1..@pN`) |
| `./screen-spec-template.md` · `./reference-template.md` | kardeşler — bağlantı verilmez, metin |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablon satır aralığı | ≥ 480 | `Get-Content` + `Count` |
| K2 | Bu şablon KB (envanter: `12,0`) | ≥ 27,0 | `(Get-Item …).Length / 1KB` |
| K3 | Kutu satırları ≤ 66 karakter | 0 sapma | §5.3/3 |
| K4 | `l0..l7` katman sözlüğü tek yerde | tek tablo | §3.4 |
| K5 | `l3` yalnız `l0`'a bakar kuralı yazılı | var | §3.4 · §5.3/6 |
| K6 | Yasaklı vault adı geçen satır | 0 | G1 |
| K7 | `.ai/ui-design/flow/` dosya sayısı (üretim sonrası) | ≥ 3 | `Test-Path` |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol / Bağlantı | Ne İçin | Durum (2026-10-05) |
|--------|----------------|---------|---------------------|
| Şablon envanteri | [../index.md](../index.md) | §3.4 satır 27 · §3.7 · G1..G12 | ✅ okundu |
| Kardeş (altın örneği) | [./wireframe-template.md](./wireframe-template.md) | biçim referansı | ✅ 642 satır / 34,5 KB |
| Form karşılığı | [../frontend/form-template.md](../frontend/form-template.md) | olay akışı kodu | ✅ okundu |
| Handler | [../backend/handler-template.md](../backend/handler-template.md) | `l2` karşılığı | ✅ |
| Servis | [../backend/service-template.md](../backend/service-template.md) | use case | ✅ |
| Repository | [../backend/repository-template.md](../backend/repository-template.md) | `l4` kaydetme | ✅ |
| Veri ADR | [../adr/adr-data-template.md](../adr/adr-data-template.md) | ADO.NET + Dapper | ✅ G2 |
| Sorgu şablonu | [../query/Query-Template.md](../query/Query-Template.md) | `@p1..@pN` | ✅ |
| Kök belgeler | [../../../CLAUDE.md](../../../CLAUDE.md) · [../../../WORKFLOW.md](../../../WORKFLOW.md) | G8 · kapı zinciri | ✅ diskte var |
| `.ai/index.md` · `.ai/WORKFLOW.md` · `.ai/architecture/` | **metin** (bağlantı yok) | kapı/katman referansı | ❌ YOK → ⚠️ |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Flow için GATE-003 beklemek | flow kapı muafidir, Part C kilitlenir | §1.3 tablosu |
| 2 | Akışı koddan çıkarmak | 40 dk debug, tek gerçek kaynağı koddur | §3.3 DAG |
| 3 | Başarı yolunu tek yol yazmak | hata/iptal görünmez kalır | §3.6 her adımda dal |
| 4 | Karar dalını tek yazmak | ne zaman dallanıyor belirsiz | §3.5 ≥ 2 dal |
| 5 | `l2`'yi karar veren yapmak | iş akışı katmanı karar vermez | `l7`/`l1`/`l4` (§3.5) |
| 6 | `l2` → `l4` doğrudan ok | mimari yasağı çiğnenir | `l5`/`l4` ara, G8 |
| 7 | Timeout'u "uygun süre" yazmak | test edilemez, §6.2 R6 | sayısal eşik (§3.8) |
| 8 | `[3] kaydet`'i iptal edilebilir yapmak | yarım kayıt riski | §3.7 iptal edilemez adım |
| 9 | Audit'e parola/anahtar koymak | sızıntı riski | §3.10 hassas alan yasağı |
| 10 | `.ai/index.md`'ye bağlantı vermek | diskte yok, kırık link (G7) | metin olarak yaz |
| 11 | `{{ADIM}}` ile onaya sunmak | iskelet onaylanıyor | G5, §5.3/5 = 0 |
| 12 | Akışa hex renk/tema koymak | iki gerçek kaynağı | `reference` şablonu |

**SSUT beyanı:** Akış biçiminin **tek** kaynağı bu dosyadır; tel kafes, screen-spec, reference, prompt ve frontend şablonları yalnız buna bağlanır. Kopyalanan her satır ikinci gerçek kaynağı doğurur (envanter G12).
