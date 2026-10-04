---
title: "VersaCoder — ADR (Karar Kaydı) Genel Şablonu"
type: template
category: adr
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "genel ADR sablonu: baslik, durum, baglam, karar, sonuc ve kanit baglantilari"
---

# VersaCoder — ADR (Karar Kaydı) Genel Şablonu

**Zorunlu Bağlantılar:** [index](../../index.md) · [CLAUDE.md](../../CLAUDE.md) · [ascii-diagram-template](../architecture/ascii-diagram-template.md) · [adr-index](adr-index.md) · [adr-nygard-template](adr-nygard-template.md) · [AGENTS.md](../../../AGENTS.md)

---

## §1 Amaç

Bu şablon, VersaCoder `.ai/` vault'unda **her kararın tek biçimde** yazılması için zorunludur: tek cümlelik karar, en az iki alternatif, ödenmiş sonuç, sahipli takip. `T-ADR`'nın **tek genel kaynağıdır**; `.templates/adr/` altındaki altı varyant buradan türetilir ve §1–§7 iskeletini **silemez**.

| Alan | Değer |
|------|-------|
| Template Name | `adr-template.md` |
| Template Path | `.ai/.templates/adr/adr-template.md` |
| Hedef Dosya | `.ai/adr/ADR-NNN-<ingilizce-slug>.md` |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Üretilen Uzunluk | 250–400 satır (dolu kayıt; iskelet değil) |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — karar slug'ı İngilizce |
| Versiyon | 1.0.0 |
| Authority | SSOT (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.1 Bu Şablonun Sorumluluğu ve ADR Ailesi

ADR ailesi yedi dosyadır; altısı bu şablonun **daraltılmış varyantıdır**, yalnız soruyu değiştirir. Ortak: §1–§7 iskeleti, zorunlu alanlar, guardrails, doğrulama komutları. **Varyant kuralı:** varyant yalnız `§3.x`'i doldurur; §1, §2, §4–§7 aynen kalır, bir bölüm eksik varyant **reddedilir** (§6.2 R4).

| Dosya | Rol | Daralttığı Soru | Bu Şablondan mı |
|-------|-----|-----------------|------------------|
| `adr-template.md` | **tek genel kaynak (bu dosya)** | her karar nasıl yazılır | — |
| `adr-nygard-template.md` | kısa, geri alınabilir karar | 15 dakikada kayıt | ✅ türetilmiş |
| `adr-architecture-template.md` | katman sınırı, bağımlılık, TFM | referans yönü doğru mu | ✅ türetilmiş |
| `adr-data-template.md` | şema, migration, sorgu | veri yığını nedir | ✅ türetilmiş |
| `adr-security-template.md` | secret, auth, oturum, threat model | tehdit azaltıldı mı | ✅ türetilmiş |
| `adr-devexpress-template.md` | bileşen, sürüm, stil | masaüstü UI kararı | ✅ türetilmiş |
| `adr-index.md` | karar defteri kaydı | hangi ADR numarası açık | ✅ türetilmiş |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Kanıt yoksa uydurulmaz, `⚠️ VERIFICATION REQUIRED` yazılır. Ölçüm: `C:\versacoder`, 2026-09-25.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/adr/` klasörü (karar defteri) | ❌ YOK | `Test-Path = False` |
| 2 | `.ai/adr/` içinde `ADR-NNN-*.md` ve `index.md` | ❌ 0 adet | dizin yok |
| 3 | `.ai/.templates/adr/` altındaki 6 varyant | ✅ DOĞRULANDI | 6 varyant · 20.260–31.873 bayt (§7.1) |
| 4 | `adr-template.md` = envantere göre `MEVCUT · 34,0 KB` | ✅ BU DOSYA | ölçüm §6.5 K2 |
| 5 | `architecture/ascii-diagram-template.md` (biçim altın örneği) | ✅ DOĞRULANDI | 38.121 bayt / 644 satır |
| 6 | `.ai/.rules/` (7 MD kural dosyası) | ❌ DİZİN YOK | envanter beyanı, diske işlenmemiş |
| 7 | `.ai/architecture/index.md` · `WORKFLOW.md` · `keys.md` · `log.md` | ❌ YOK | `.ai/` altında yalnız `.templates/` |
| 8 | `src/` (VersaCoder.Desktop · Cli · Application) | ❌ YOK | — |
| 9 | `global.json` + `Directory.Packages.props` (CPM) | ❌ YOK | — |
| 10 | `versacoder.db` (SQLite) | ❌ YOK | — |
| 11 | Harici referans vault'ları (envanter G1) | 🔒 ERİŞİM YASAK | yalnız yapı referansı; tek kelime kopyalanmadı |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır, satır olduğu yerde durur. §1.2'deki ❌ satırları §3.9'daki gerçeklerin kanıtsızlığını hatırlatır; kanıtsız satır ADR'ye süs olarak girmez.

### §1.3 ADR Nedir / Ne Değildir

Nygard niyeti üç cümledir: **bağlam sabitlenir, karar değişebilir, sonuç ödenir.** ADR karar anında yazılır; gerekçesi o gün diske düşmezse karar savunulamaz, ekip aynı tartışmayı yeniden açar.

| ADR'dir | ADR değildir |
|---------|--------------|
| Kararın **kayıdı**: neyi, neden seçtiğimiz | Tasarım dokümanı / mimari savunma yazımı |
| Tek cümlelik karar + eleme gerekçesi | Uygulama kılavuzu (adım adım "nasıl") |
| Tarihli ve **karar verenleri yazılı** belge | Gereksinim listesi (ne istendiği) |
| Alternatiflerin hafızası (elenen seçenek de yaşar) | Kalıcı kural (→ `.ai/.rules/`) |
| Geri alınabilirlik bilgisi taşıyan kayıt | Süreç/oturum günlüğü (→ `log.md`) |
| Geçmişi koruyan, **silinmeyen** belge | Kod yorumu / PR açıklaması |

### §1.4 ADR Yaşam Döngüsü

| Durum | Anlamı | Kim Onaylar | Dosyaya Etki |
|-------|--------|-------------|--------------|
| `proposed` | yazıldı, henüz onaylanmadı | yazar (architect) | dosya var, defterde `draft` |
| `accepted` | onaylandı, uygulanıyor | master-orchestrator | defterde satır açılır |
| `deprecated` | hâlâ doğru ama önerilmiyor | architect | dosya kalır, üstüne uyarı |
| `superseded` | yeni ADR-NNN ile değişti | architect + orchestrator | eski dosya işaretlenir, **silinmez** |

```text
+----------------------------------------------------------------+
|[1] proposed    hâlâ onaylanmadı — taslak durur                 |
|    v                                                           |
|[2] accepted    onaylandı, uygulanıyor (§5.1 adım 10)           |
|    v                                                           |
|[3] deprecated  artık önerilmiyor — dosya durur                 |
|    v                                                           |
|[4] superseded  yeni ADR-NNN gelince eski dosya işaretlenir     |
+----------------------------------------------------------------+
```

Geçişler **tek yönlüdür**: `proposed → accepted → deprecated | superseded`. Geri dönüş yoktur; yanlış karar yeni bir ADR ile düzeltilir (§6.3).

---

## §2 Kapsam

**İçinde:** kararın yazımı, numaralandırma, durum yaşam döngüsü, karar kalitesi kuralları, salt-okunur doğrulama.
**Dışında:** kararın uygulanması (kod), kuralların metni, gate geçişleri, diyagram çizimi.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| Bağlam · sorun · alternatif · karar · sonuç · takip | Kararın **uygulanması** (→ `developer`) |
| `ADR-NNN` numaralandırma + defter senkronu | `.templates/` envanteri (→ `[[index]]`) |
| Durum yaşam döngüsü + supersede zinciri | Gate geçişleri (`GATE-004a..006` → `WORKFLOW.md`) |
| Karar kalitesi kuralları + hard guardrails | Kuralların metni (→ `.ai/.rules/`) |
| 7 salt-okunur doğrulama komutu | Diyagram çizimi (→ `[[architecture/ascii-diagram-template]]`) |
| Varyant seçimi eşiği (§2.2) | 600+ satırlık mimari tez |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Sorumlu | Bu Şablondan mı |
|------------|---------|------------------|
| `.ai/adr/ADR-NNN-*.md` (genel karar) | architect | ✅ |
| `.ai/adr/ADR-NNN-{arch,data,sec,devx}-*.md` | ilgili uzman rol | ✅ (varyant §1.1) |
| `.ai/adr/index.md` (karar defteri) | docs-writer | ✅ (`[[adr-index]]`) |
| `.ai/.rules/*.md` (tekrarlayan kural) | security · developer | ❌ kural ≠ karar |
| `.ai/architecture/index.md` | architect | ❌ ADR'den türetilir |
| `.ai/keys.md` (secret sözlüğü) | security | ❌ değer asla ADR'ye girmez |

### §2.2 Eşik Kuralları — Hangi Karar ADR İster?

| Karar | ADR? | Neden | Varyant |
|-------|------|-------|---------|
| Katman sınırı, bağımlılık yönü, TFM, solution yapısı | ✅ EVET | geri alınması pahalı, herkesi bağlar | architecture |
| Veri yığını, şema, migration, sorgu biçimi | ✅ EVET | veri kaybı riski, kalıcı iz | data |
| Secret, auth, oturum, kripto, threat model | ✅ EVET | güvenlik etkisi + denetim izi | security |
| DevExpress sürümü, ribbon stili, bileşen kilidi | ✅ EVET | 26.1 donduruldu (ADR-001) | devexpress |
| Ürün/genel süreç kararı (dizin adı, ajan kataloğu, defter biçimi) | ✅ EVET | vault'un hafızası | **bu şablon** |
| Küçük teknik tercih: yardımcı metot adı, dosya içi refactor, tek seferlik script, geçici bayrak | ❌ HAYIR | 1 günde/1 haftada tersine çevrilebilir | — (kod yorumu) |
| Tekrarlayan, her dosyayı bağlayan kural | ❌ HAYIR | kural ≠ karar | — (`.ai/.rules/`) |

**4 soruluk eşik testi** — ilk `EVET` ADR ister: (1) ay sonra hâlâ geçerli mi, kod/veri göçü mü? (2) ≥2 dosya/aşamayı bağlıyor mu? (3) yanlışsının düzeltme maliyeti yüksek mi? (4) altı ay sonra "neden böyle yaptık?" cevabı yok mu? Hepsi `HAYIR` → **ADR yazılmaz**; karar dosya yorumunda veya PR açıklamasında kalır.

---

## §3 Mimari

Bu bölüm hem **şablonun kendi yapısını** hem **üretilecek ADR'nin yapısını** tanımlar.

### §3.1 Zorunlu Alanlar

Dosyanın ilk 16 satırı frontmatter'dır; dolu örnek §3.10 kalıbındadır (ADR-001 değerleriyle):

| # | Alan | Zorunlu | Örnek | Eksikse |
|---|------|---------|-------|---------|
| 1 | `title` | Evet | `ADR-001 — devexpress-version-lock` | RED |
| 2 | `type` | Evet | `adr` | RED |
| 3 | `category` | Evet | `adr` | RED |
| 4 | `status` | Evet | `accepted` (enum §1.4) | RED |
| 5 | `version` | Evet | `1.0.0` (üç bölüm) | RED |
| 6 | `updated` | Evet | `2026-09-25` (ISO) | RED |
| 7 | `authority` | Evet | `SSOT` — birebir | RED (G4) |
| 8 | `karar` (ADR'e özgü) | Evet | tek cümle (§3.4) | RED (G1) |
| 9 | `karar_tarihi` (ADR'e özgü) | Evet | ISO tarih | RED |
| 10 | `karar_verenler` (ADR'e özgü) | Evet | rol listesi | RED (G13) |
| 11 | `supersedes` / `superseded_by` (ADR'e özgü) | Koşullu | yalnız supersede zincirinde | ZİNCİR RED (G6) |

### §3.2 Numaralandırma ve Dosya Adı

1. Sıradaki numara = defterdeki en yüksek `ADR-NNN` + 1; **açık numara atlanmaz**.
2. Dosya adı `.ai/adr/ADR-<NNN>-<ingilizce-slug>.md` — slug `kebab-case`, 2–4 sözcük: `ADR-004-data-stack.md`.
3. Numara **üç hane**: `ADR-001`…`ADR-999`. `ADR-1`, `ADR-01` yasak.
4. Dosya yazıldıktan sonra `.ai/adr/index.md` defterine **tek satır** eklenir; bilgi iki yerde ayrı tutulmaz (SSUT).
5. Numara asla yeniden kullanılmaz: karar vazgeçilirse dosya `deprecated` kalır, numara yanar.

| Kural | Doğru | Yanlış |
|-------|-------|--------|
| Dosya adı | `ADR-004-data-stack.md` | `adr4.md`, `ADR-4-data.md` |
| Sıralama | defterdeki en yüksek + 1 | açık numara atlayarak |
| Defter | `.ai/adr/index.md` tek satır | ayrı ikinci liste |
| Hedef dizin | `.ai/adr/` | `.ai/.rules/`, `.ai/documentation/` |
| Slug dili | İngilizce, tireli | Türkçe / boşluklu başlık |

> **§ ayrımı:** şablonun §1–§7'si (`Amaç … Referanslar`) şablonun iskeletidir; üretilen ADR'nin §1–§7'si (`Bağlam … Referanslar`) §3.10 kalıbında yaşar. §5.3/4 ikincisini tarar.

### §3.3 Zorunlu 7 Bölüm İskeleti (üretilen ADR)

| Bölüm | Ne Yazılır | Min Satır | Sorumlu |
|-------|-----------|-----------|---------|
| `§1 Bağlam` | kararın masada olma nedeni, tetikleyici, etkilenen katman | 12 | architect |
| `§2 Sorun` | çözülmesi gereken gerilim + bağlayıcı kısıtlar | 10 | architect |
| `§3 Karar` | §3.1 tek cümle · §3.2 alternatifler · §3.3 gerekçe | 35 | architect |
| `§4 Sonuç / Kabul` | olumlu · olumsuz · nötr — **üç ayrı tablo** | 20 | architect |
| `§5 Uygulama` | dosya yolu, komut, sürüm/sayı, geri alma | 15 | developer |
| `§6 Takip` | madde · sahip · son tarih · durum | 8 | master-orchestrator |
| `§7 Referanslar` | bağlantı + URL + erişim tarihi | 8 | docs-writer |
| **Toplam** | iskelet değil, **dolu** kayıt | **≥ 110 · hedef 250–400** | — |

### §3.4 §3 Karar — Tek Cümle, Alternatifler, Gerekçe

**ADR §3.1 — karar cümlesi.** Tam **bir** cümle, en fazla 30 sözcük, `… karar verildi.` ile biter. İki seçenek bırakmak (`veya`), soru cümlesi ve "gerekirse değerlendirilir" ifadesi yasaktır (G1).

> Örnek: "Veri erişimi EF Core yerine ADO.NET + Dapper ile yapılır; her sorgu `@p1..@pN` parametresi taşır. Karar verildi."

**ADR §3.2 — alternatifler tablosu.** Dört sütun zorunlu; seçilen seçenek dahil **her satır dolu** yazılır:

| Seçenek | Artı | Eksi | Neden Elendi |
|---------|------|------|--------------|
| A — ADO.NET + Dapper (**SEÇİLDİ**) | SQL tam görünür, `@p1` enjeksiyonu kapatır, iki TFM'de aynı davranış | eşleme elle yazılır | **elenmedi** |
| B — EF Core | migration ve ekip deneyimi hazır | `net48` üzerinde sürüm riski, üretilen SQL görünmez | denetlenebilir SQL şart → elendi |
| C — saf `SqlCommand` | maksimum kontrol | her sorgu için tekrarlayan kod | aynı kontrol daha az kodla → elendi |
| D — ek mini-ORM paketi | hızlı başlangıç | ikinci TFM'de uyum + CPM'te ek sürüm yükü | CPM tek sürüm kaynağı → elendi |

**ADR §3.3 — gerekçe.** 3–5 cümle; sıra **kısıt → ölçülebilir etki → elenen seçeneğin bedeli**. Masada tartışılmamış seçenek uydurulmaz; kanıtsız satır `⚠️ VERIFICATION REQUIRED` ile işaretlenir ve §7.1'den kaynak istenir.

### §3.5 §4 Sonuç / Kabul — Üç Ayrı Tablo

Üç tablo da zorunludur; hiçbiri boş bırakılamaz (G3). "Avantajlar" başlığıyla olumsuzlar örtbas edilemez.

| Olumlu Sonuç | Ölçüt |
|--------------|-------|
| Karar altı ay sonra tek dosyadan okunur | §7.1'de kaynak var |
| Elenen seçenek hafızada kalır | §3.4 ≥ 2 dolu satır |
| Yeni karar eskiyi gömmüyor | `superseded` zinciri işliyor |

| Olumsuz Sonuç (ödenen bedel) | Ölçüt |
|------------------------------|-------|
| Eşleme kodu elle yazılır | repository başına ek kod |
| İki TFM ayrı test ister | `net48` + `net10.0-windows` derlemesi |
| Devir yükü | yeni gelen SQL bilmek zorunda |

| Nötr / Bilgi | Not |
|--------------|-----|
| Geri alınabilir mi | evet — migration scripti geri alınır |
| Kimleri bağlar | tüm repository katmanı |
| Neyi kapsamıyor | ekran/UX kararları (ADR-002) |

### §3.6 §5 Uygulama (Implementation)

| Öğe | Ne Yazılır | Doğru Örnek |
|------|-----------|-------------|
| Dosya yolu | değişen dosyalar, kökten itibaren | `src/VersaCoder.Infrastructure/**` (⚠️ diskte YOK — §1.2/8) |
| Sürüm/sayı | sabitlenen değer | `26.1` · `net48;net10.0-windows` |
| Geri alma | nasıl tersine çevrileceği + tahmini süre | migration `V0xx` geri alınır, ~5 dk |
| Kabul kriteri | ölçülebilir, sayısal | sahipsiz `@pN` = 0 · ters referans = 0 |

### §3.7 §6 Takip (Follow-up)

| # | Takip Maddesi | Sahip | Son Tarih | Durum |
|---|---------------|-------|-----------|-------|
| 1 | Repository'lerde `@pN` denetimi | database-optimizer | 2026-10-10 | open |
| 2 | `schema_version` seed verisi | database-optimizer | 2026-10-15 | open |
| 3 | Eski paket referanslarının temizliği | developer | 2026-10-05 | done |

Kurallar: **en az 1 madde zorunlu** (G13) · sahip envanterdeki rollerden biri, boş olamaz · tarih ISO `YYYY-MM-DD` · durum `open|moved|done|blocked`.

### §3.8 §7 Referanslar (üretilen ADR'nin)

| Kaynak Türü | Format | Zorunlu | Örnek |
|-------------|--------|---------|-------|
| İç vault dosyası | `[[bağlantı]]` | Evet | `[[adr-index]]` |
| Harici sürüm notu | `URL — erişim: YYYY-MM-DD` | Evet (sürüm iddiası) | resmi sürüm notu |
| İlgili ADR | `ADR-NNN` + ilişki | Evet (supersede) | `ADR-001` |
| İkinci el kaynak (blog, forum) | — | **YASAK** | kanıtsız sayılmaz |

### §3.9 VersaCoder Gerçekleri (uydurulmaz sabitler)

Bu şablonla yazılan her ADR'de aşağıdaki değerler aynen kullanılır; disk kanıtı yoksa `⚠️ VERIFICATION REQUIRED` kalır (ölçüm 2026-09-25, §1.2).

| # | Gerçek | Değer | İlgili ADR | Disk Kanıtı |
|---|--------|-------|------------|-------------|
| 1 | DevExpress sürümü sabit, yükseltme YASAK | `26.1` | ADR-001 | ❌ `src/` YOK |
| 2 | Veri erişimi EF Core → ADO.NET + Dapper | Dapper + `@p1,@p2` | ADR-004 | ❌ YOK |
| 3 | İkili hedef (TFM) | `net48;net10.0-windows` | ADR-003 | ❌ YOK |
| 4 | Yerel veritabanı | `versacoder.db` (SQLite) + `schema_version` | ADR-004 | ❌ YOK |
| 5 | Parametreli SQL zorunlu, string concat yasak | `@p1..@pN` | ADR-004 · G10 | ❌ YOK |
| 6 | Sürümlü SDK + merkezî paket yönetimi | SDK `10.0.401` · `global.json` `10.0.301`/`latestFeature` · `Directory.Packages.props` | — | ❌ YOK |
| 7 | Karar defteri yeri | `.ai/adr/` + `index.md` | — | ❌ YOK |
| 8 | Ürün adı | `VERSACODER` / VersaCoder — harici vault adı **ASLA** | G12 | 🔒 yasaklı vault |
| 9 | Karar veren roller | architect · reviewer · master-orchestrator | §4.2 | ⚠️ `.ai/.agents/` YOK |

### §3.10 Tam Kalıp (yer tutuculu ADR iskeleti)

Yer tutucular çift süslü parantez biçimindedir ve **yalnız bu blokta** yaşar; üretilen ADR'de adedi 0 olmalıdır (G9).

```markdown
---
title: "ADR-{{NO}} — {{INGILIZCE_SLUG}}"
type: adr
category: adr
status: {{PROPOSED_ACCEPTED_DEPRECATED_SUPERSEDED}}
version: 1.0.0
updated: {{YYYY-MM-DD}}
authority: SSOT
karar: "{{TEK_CUMLE}}"
karar_tarihi: {{YYYY-MM-DD}}
karar_verenler: [{{ROL_1}}, {{ROL_2}}]
supersedes: {{ADR_NNN_Veya_NULL}}
superseded_by: {{ADR_NNN_Veya_NULL}}
---
# ADR-{{NO}}: {{INGILIZCE_KARAR_BASLIGI}}
**Zorunlu Bağlantılar:** [adr-index](.templates/adr/adr-index.md) · [index](index.md) · [{{ILGILI_BAGLANTI}}]({{ILGILI_BAGLANTI}})
## §1 Bağlam (Context)
{{KARARIN_MASADA_OLMA_NEDENI — tetikleyici, tarih, etkilenen katman}}
## §2 Sorun (Problem)
{{COZULMESI_GEREKEN_GERILIM — her kısıt tek satır}}
## §3 Karar (Decision)
### §3.1 Karar cümlesi
{{TEK_CUMLE}} — karar verildi.
### §3.2 Alternatifler
| Seçenek | Artı | Eksi | Neden Elendi |
|---|---|---|---|
| A — {{SECENEK_A}} (SEÇİLDİ) | {{ARTI}} | {{EKSI}} | **elenmedi** |
| B — {{SECENEK_B}} | {{ARTI}} | {{EKSI}} | {{ELENME_NEDENI}} |
| C — {{SECENEK_C}} | {{ARTI}} | {{EKSI}} | {{ELENME_NEDENI}} |
### §3.3 Gerekçe
{{3–5 CÜMLE: kısıt → ölçülebilir etki → elenen seçeneğin bedeli}}
## §4 Sonuç / Kabul (Consequences)
| Olumlu Sonuç | Ölçüt |
|---|---|
| {{KAZANIM}} | {{OLCUT}} |
| Olumsuz Sonuç (bedel) | Ölçüt |
|---|---|
| {{BEDEL}} | {{OLCUT}} |
| Nötr / Bilgi | Not |
|---|---|
| {{BILGI}} | {{NOT}} |
## §5 Uygulama (Implementation)
| Öğe | Değer |
|---|---|
| Dosya yolu | {{KOKTEN_YOL}} |
| Sürüm/sayı | {{SABIT_DEGER}} |
| Geri alma | {{GERI_ALMA_KOSULU + SURE}} |
| Kabul kriteri | {{OLCULEBILIR_KRITER}} |
## §6 Takip (Follow-up)
| # | Takip Maddesi | Sahip | Son Tarih | Durum |
|---|---|---|---|---|
| 1 | {{TAKIP_MADDESI}} | {{ROL}} | {{YYYY-MM-DD}} | open |
## §7 Referanslar
| Kaynak | Bağlantı / URL | Erişim |
|---|---|---|
| Vault gerçeği | [{{BAGLANTI}}]({{BAGLANTI}}) | {{YYYY-MM-DD}} |
| İlgili ADR | {{ADR_NNN}} | {{YYYY-MM-DD}} |
```

---

## §4 Kurallar

### §4.1 Karar Kalitesi Kuralları

| # | Kural | Test | İhlal |
|---|-------|------|-------|
| 1 | Karar tek cümlede biter (`… karar verildi.`) | §3.4 = 1 cümle | RED (G1) |
| 2 | En az 2 alternatif, her satırın 4 sütunu dolu | §3.4 ≥ 2 satır | RED (G2) |
| 3 | Elenecek seçenek masada tartışılmış olmalı | uydurma seçenek yok | `⚠️` → RED |
| 4 | Sonuç üç ayrı tabloda | §3.5 = 3 dolu tablo | RED (G3) |
| 5 | Takip ≥ 1 madde + sahip + tarih | §3.7 dolu | RED (G13) |
| 6 | Geri alınabilirlik §5'te yazılı | "geri alma" satırı | DUR |
| 7 | Karar `.ai/adr/` dışında belgelenmez | dosya yolu | RED (G7) |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Çıktı | Süre |
|-------|-----|-----------|-------|------|
| 1 | architect | §1–§3 (bağlam, sorun, karar) | tek cümle + alternatifler | 15 dk |
| 2 | reviewer | §6.1 kontrol listesi | ✅ / RED | 10 dk |
| 3 | security · database-optimizer | ilgili varyant (§1.1) | uzman onayı | 8 dk |
| 4 | master-orchestrator | §4.5 + §5.3 çıktıları | `status: accepted` + defter satırı | 7 dk |
| | **Toplam** | | | **≈ 40 dk/ADR** |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir. Red → düzelt → aşama 1'e dön.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Alternatif sayısı | ≥ 2 | 1 satır = RED |
| Karar cümlesi | 1 cümle, ≤ 30 sözcük | 2 cümle = RED |
| Sonuç tablosu | 3 (olumlu / olumsuz / nötr) | boş tablo = RED |
| Takip maddesi | ≥ 1 | 0 = RED (G13) |
| Üretilen ADR | 250–400 satır | < 150 iskelet · > 600 doküman |
| Numaralandırma | `ADR-NNN`, 3 hane, artan | delik/tekrar = §5.3/5 |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik ve Supersede Kontrolü

| Olay | Aksiyon | Durum | Kayıt |
|------|---------|-------|-------|
| Yazım/ımle düzeltmesi | dosya içinde | değişmez | `updated` + `version` patch |
| Gerekçenin güçlendirilmesi | §3.4/§3.5 | `accepted` kalır | `log.md` tek satır |
| Kararın değişmesi | **yeni ADR** açılır, eski **silinemez** | eski → `superseded` | defter + her iki dosya |
| Karar artık önerilmiyor | dosya üstüne uyarı | `deprecated` | defter satırı |
| Defter ↔ dosya uyuşmazlığı | — | **DUR** | §4.5 G6 |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | Tek karar cümlesi yok (yok / iki / paragraf) | **DUR** — ADR kabul edilmez |
| G2 | Alternatifsiz (§3.4 tablosu < 2 satır veya boş hücre) | **DUR** — alternatif yazılır |
| G3 | Sonuçsuz (§3.5 tablolarından biri boş) | **DUR** — sonuç ödenmeden kabul yok |
| G4 | `authority: SSOT` yok veya yazım farklı | **DUR** — §5.3/3 |
| G5 | Numara deliği, tekrar veya `ADR-NNN` formatı bozuk | **DUR** — §5.3/5 |
| G6 | Defter `superseded` diyor ama eski dosya işaretlenmemiş (ya da tersi) | **DUR** — defter ↔ dosya uyuşmazlığı |
| G7 | `.ai/adr/` dışında karar belgesi | **DUR** — dosya taşınır |
| G8 | `status` enum dışı (`approved`, `done`, `WIP`) | **DUR** — §5.3/6 |
| G9 | Üretilen ADR'de yer tutucusu kalmış | **DUR** — §5.3/2 |
| G10 | String concat SQL veya EF Core paket referansı | **DUR** — ADR-004 · envanter G2/G10 |
| G11 | DevExpress sürümü `26.1` dışında iddia | **DUR** — ADR-001 |
| G12 | Yasaklı harici vault'tan cümle | **DUR** — envanter G1 |
| G13 | Takip maddesi sahipsiz veya tarihsiz | **DUR** — §3.7 |
| G14 | Mojibake (çiftlenmiş Latin-1 kodlaması) veya karışık dil | **DUR** — UTF-8 BOM'suz yeniden yaz |

---

## §5 Workflow

### §5.1 Adım Bazlı ADR Yazım Akışı

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Bağlam ve sorunu topla | tetikleyici + ilgili dosyalar | §1 + §2 taslağı | architect | 12 dk |
| 2 | Varyant seç | §1.1 + §2.2 | şablon adı | architect | 3 dk |
| 3 | Numara ata | defterdeki en yüksek NNN | `ADR-NNN-slug.md` | architect | 2 dk |
| 4 | Alternatifleri yaz | ≥ 2 seçenek, 4 sütun | §3.4 tablosu | architect | 10 dk |
| 5 | Tek karar cümlesi | en iyi seçenek | §3.4 tek cümle | architect | 3 dk |
| 6 | Gerekçe + sonuç | seçimin bedeli | §3.4/§3.5 | architect | 10 dk |
| 7 | Uygulama + takip | dosya · komut · sürüm | §3.6 + §3.7 | developer | 8 dk |
| 8 | Senkron | `.ai/adr/index.md`, `.ai/log.md` | defter satırı | docs-writer | 4 dk |
| 9 | İnceleme + kabul | §6.1 + §5.3 | `accepted` | reviewer → orchestrator | 15 dk |
| | **Toplam/ADR** | | | | **≈ 67 dk** |

**Zaman bütçesi:** 1–3 ≈ 17 dk, 4–7 ≈ 31 dk, 8–9 ≈ 19 dk. Karar **1 saatten uzun** yazılıyorsa §1.3 sapmıştır: dokümana dönüşmüş, §3.4'e dönülür.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Dokümana dönüşme | 600+ satır, karar cümlesi kaybolmuş | savunma yazıldı | §4.3: 250–400 satıra indir |
| 2 | Uydurma alternatif | seçenek hiçbir tartışmada yok | gerekçe zayıf | masada olmayanı sil, `⚠️` yaz |
| 3 | Karar cümlesiz kabul | §3.4 paragraf | G1 atlanmış | tek cümleye indir |
| 4 | Durum çiftliği | defter `accepted`, dosya `proposed` | senkron unutulmuş | §5.3/6 + G6 |
| 5 | Supersede'de silme | eski dosya yok | geçmiş silinmiş | dosya geri konur, `superseded` işaretlenir |
| 6 | Kanıtsız sürüm | "en son sürüm" yazılmış | araştırma atlanmış | §3.8 URL + tarih, yoksa `⚠️` |
| 7 | Sahipsiz takip | §3.7 boş | iş kapatıldı sayıldı | rol + tarih ata (G13) |
| 8 | Yer tutucu sızıntısı | üretilen dosyada yer tutucu görünür | §3.10 doldurulmadı | temizle, §5.3/2 |

### §5.3 Salt Okunur Doğrulama Komutları

Yedi komut da **hiçbir dosyayı değiştirmez**.

```powershell
# 1) ADR envanteri — dizin yoksa dürüstçe 0 ADR
$adrDir = 'C:\versacoder\.ai\adr'
if (-not (Test-Path $adrDir)) { 'DIZIN YOK -> 0 ADR (defter henuz acilmadi)' }
else { Get-ChildItem $adrDir -Filter 'ADR-*.md' | Sort-Object Name |
         ForEach-Object { '{0}  {1} KB' -f $_.Name, [math]::Round($_.Length / 1KB, 1) } }
# 2) Frontmatter — eksik alanlı dosya 0 (şablon + ADR)
$need = 'title','type','category','status','version','updated','authority'
Get-ChildItem 'C:\versacoder\.ai\.templates\adr' -Filter *.md -ErrorAction SilentlyContinue |
  ForEach-Object {
    $head = (Get-Content $_.FullName -TotalCount 16 -ErrorAction SilentlyContinue) -join "`n"
    $miss = $need | Where-Object { $head -notmatch ('(?m)^' + $_ + ':') }
    if ($miss) { '{0} -> eksik: {1}' -f $_.Name, ($miss -join ', ') } }
# 3) authority: SSOT — sapma 0
Get-ChildItem 'C:\versacoder\.ai\.templates\adr\*.md','C:\versacoder\.ai\adr\*.md' -ErrorAction SilentlyContinue |
  ForEach-Object { if (-not ((Get-Content $_.FullName -Raw) -match '(?m)^authority:\s*SSOT\s*$')) {
      'SSOT YOK -> ' + $_.Name } }
# 4) Eksik § başlığı — §1 Bağlam … §7 Referanslar, 7/7 zorunlu
$bolum = '§1 Bağlam','§2 Sorun','§3 Karar','§4 Sonuç','§5 Uygulama','§6 Takip','§7 Referanslar'
if (-not (Test-Path 'C:\versacoder\.ai\adr')) { 'DIZIN YOK -> 0 ADR, kontrol atlandi' } else {
  Get-ChildItem 'C:\versacoder\.ai\adr' -Filter 'ADR-*.md' | ForEach-Object {
    $raw = Get-Content $_.FullName -Raw
    $ex = $bolum | Where-Object { $raw -notmatch ('(?m)^## ' + [regex]::Escape($_)) }
    if ($ex) { '{0} -> eksik bolum: {1}' -f $_.Name, ($ex -join ', ') } } }
# 5) Numara tekilligi + delik — yanlis ad/tekrar 0, delik listelenir
$adrFiles = Get-ChildItem 'C:\versacoder\.ai\adr' -Filter 'ADR-*.md' -ErrorAction SilentlyContinue
if (-not $adrFiles) { 'DIZIN YOK -> 0 ADR' } else {
  $nos = foreach ($f in $adrFiles) {
    if ($f.Name -match '^ADR-(\d{3})-.+\.md$') { [int]$Matches[1] } else { 'YANLIS AD -> ' + $f.Name } }
  $nos | Where-Object { $_ -isnot [int] }
  $n = @($nos | Where-Object { $_ -is [int] })
  $n | Group-Object | Where-Object Count -gt 1 | ForEach-Object { 'TEKRAR -> ADR-' + $_.Name }
  $r = $n | Measure-Object -Minimum -Maximum
  ($r.Minimum..$r.Maximum | Where-Object { $_ -notin $n }) | ForEach-Object { 'DELIK -> ADR-{0:D3}' -f $_ } }
# 6) Status — enum disi deger 0 (proposed|accepted|deprecated|superseded)
$okDurum = 'proposed','accepted','deprecated','superseded'
Get-ChildItem 'C:\versacoder\.ai\adr' -Filter 'ADR-*.md' -ErrorAction SilentlyContinue | ForEach-Object {
  $m = [regex]::Match((Get-Content $_.FullName -Raw), '(?m)^status:\s*(\S+)')
  if (-not $m.Success) { 'STATUS YOK -> ' + $_.Name }
  elseif ($m.Groups[1].Value -notin $okDurum) { 'GECERSIZ STATUS -> {0}: {1}' -f $_.Name, $m.Groups[1].Value } }
# 7) Mojibake — gercek satir 0 (UTF-8, BOM'suz)
Get-ChildItem 'C:\versacoder\.ai' -Recurse -Filter *.md -ErrorAction SilentlyContinue |
  Select-String -Pattern '\xC3[\x80-\xBF]|\uFFFD' |
  ForEach-Object { '{0}:{1}' -f $_.Path, $_.LineNumber }
```

**Geçme:** #1 dizin yoksa dürüst `0 ADR` · #2–#7 **0 satır** (kalıp dizeleri hariç).

### §5.4 Quality Report

```markdown
## Quality Report — ADR (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|---|---|---|---|
| Ölçüm (şablon) | <satır> satır / <KB> KB | 560–660 · 33–35 | 🟢/🔴 |
| Checklist §6.1 | n/16 | 16/16 | 🟢/🔴 |
| Komut §5.3 (#1–#7) | n satır çıktı | 0 | 🟢/🔴 |
| Yer tutucu / mojibake / yasaklı vault adı | n/n/n | 0/0/0 | 🟢/🔴 |
| Guardrail G1..G14 | ihlal n | 0 | 🟢/🔴 |
| Onay | 4 aşama (§4.2) | 4/4 | 🟢/🔴 |
**Karar:** 🟢 ONAYLANDI (deftere işlenir) / 🔴 RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (16 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 7 alan (`title…authority`) | 7/7 | §3.1 |
| K2 | `authority: SSOT` birebir yazıyor | `SSOT` | §5.3/3 |
| K3 | H1 + `**Zorunlu Bağlantılar:**` (6 bağlantı) | 1 satır | üst kısım |
| K4 | ADR'e özgü alanlar: `karar`, `karar_tarihi`, `karar_verenler` | 3/3 | §3.1 |
| K5 | Üretilen ADR'nin §1–§7 başlıkları | 7/7 | §5.3/4 |
| K6 | Alternatif tablosu ≥ 2 satır, 4 sütun dolu | ≥ 2 | G2 |
| K7 | Tek karar cümlesi (`… karar verildi.`) | 1 | G1 |
| K8 | Üç sonuç tablosu dolu | 3/3 | G3 |
| K9 | Takip ≥ 1 + sahip + ISO tarih | ≥ 1 | G13 |
| K10 | Sürüm iddiasında URL + erişim tarihi | var | §3.8 |
| K11 | Numara formatı + tekillik + delik | 0 sorun | §5.3/5 |
| K12 | `status` değeri enum içinde | 4 değerden biri | §5.3/6 |
| K13 | Defter ↔ dosya durum eşleşmesi | 0 uyuşmazlık | G6 |
| K14 | Yer tutucu (yalnız §3.10) | 0 | G9 |
| K15 | EF Core paket referansı / concat SQL / 26.1 dışı iddia | 0 / 0 / 0 | G10 · G11 |
| K16 | Mojibake + yasaklı vault adı | 0 / 0 | G12 · G14 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3 komutlarından biri > 0 satır çıktısı | **DUR** — komutun gösterdiği dosya düzeltilir |
| R2 | §3.4 tek cümle değil | **DUR** — karar cümlesine indirgenir |
| R3 | §3.4 tablosu < 2 satır veya boş hücre | **DUR** — alternatif yazılır ya da `⚠️` |
| R4 | §3.5 tablolarından biri boş | **DUR** — sonuç ödenmeden kabul yok |
| R5 | `authority` yok / farklı yazım | **DUR** — `SSOT` yazılır |
| R6 | Numara deliği veya tekrar | **DUR** — defter yeniden sayılır |
| R7 | Defter ↔ dosya durum uyuşmazlığı | **DUR** — iki taraf birlikte güncellenir |
| R8 | `.ai/adr/` dışında karar belgesi | **DUR** — dosya taşınır |
| R9 | Yasaklı vault adı veya mojibake | **DUR** — silinir / UTF-8 BOM'suz yeniden yaz (G12 · G14) |

### §6.3 Durum / Supersede Geçiş Kuralları

| Geçiş | Koşul (hepsi `EVET`) | Onaylayan | Etki |
|-------|----------------------|------------|------|
| `proposed → accepted` | §6.1 16/16 · §5.3 temiz · §4.2 4. aşama | master-orchestrator | defter satırı `accepted` |
| `accepted → deprecated` | karar hâlâ doğru ama önerilmiyor | architect | dosya üstüne uyarı, numara yanar |
| `accepted → superseded` | yeni ADR-NNN `accepted` | architect + orchestrator | eski dosya `superseded_by` alır |
| `superseded → silme` | **imkânsız** | — | geçmiş kanıtdır, dosya kalır |
| `→ proposed` geri dönüş | yalnız `accepted` öncesi | yazar | — |

Geri dönüş yoktur: yanlış karar **yeni ADR** ile düzeltilir, eski dosya silinmez yalnız işaretlenir (G6, §4.4).

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/adr/adr-template.md` | **bu dosya** — ADR ailesinin tek kaynağı (T-ADR) |
| `.ai/.templates/adr/adr-nygard-template.md` + 4 uzman varyantı | varyant ailesi (§1.1) |
| `.ai/.templates/adr/adr-index.md` | karar defteri şablonu |
| `.ai/.templates/index.md` | envanter + kalite sözleşmesi §3.7 (satır 175) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | biçim, yoğunluk ve ölçü altın örneği |
| `.ai/adr/index.md` | **üretilen** defter — diskte YOK (§1.2/2) |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablonun satır aralığı | 560–660 | `Get-Content` + `Measure-Object -Line` |
| K2 | Bu şablonun KB aralığı (envanter satır 175: `34,0`) | 33–35 | `(Get-Item …).Length / 1KB` |
| K3 | §1.4 kutu satırları 66 karakter | 9/9 = 66 | ` ```text ` bloğu ölçümü |
| K4 | 6 zorunlu bağlantı adı brief ile birebir | 6/6 | H1 altı satır |
| K5 | §1.2'deki ❌ beyanları hâlâ doğru | `Test-Path = False` | §5.3/1 |
| K6 | §1.1 varyant sayısı = diskteki dosya | 6 | `Get-ChildItem .ai\.templates\adr` |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-09-25) |
|--------|-----|---------|---------------------|
| Şablon envanteri | `.ai/.templates/index.md` | §3.7 sözleşmesi · satır 175 | ✅ 38.816 bayt |
| Biçim altın örneği | `.ai/.templates/architecture/ascii-diagram-template.md` | §1–§7 yoğunluğu | ✅ 38.121 bayt / 644 satır |
| ADR ailesi (6 varyant) | `.ai/.templates/adr/*.md` | §1.1 tablosu | ✅ okundu |
| Karar defteri | `.ai/adr/index.md` | §3.2 senkronu | ❌ YOK |
| Vault kökü · kural · kod kanıtı | `.ai/*.md`, `.ai/.rules/`, `src/`, `global.json`, `Directory.Packages.props` | §1.2/6–9 · §3.9 | ❌ YOK → `⚠️` |
| Yasaklı vault'lar | `C:\www\…` (envanter G1) | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | ADR'yi tasarım dokümanına çevirmek | 600 satır savunma okunmaz, kararı gizler | 250–400 satır, §3.4 tek cümle |
| 2 | Alternatifsiz karar yazmak | elemek için seçenek yoksa gerekçe kanıtsız | ≥ 2 alternatif, 4 sütun dolu |
| 3 | Sonuca yalnız "avantajlar" yazmak | bedel örtbas edilir, sonradan sürpriz olur | üç ayrı tablo: olumlu/olumsuz/nötr |
| 4 | Numarayı elle atlayarak seçmek | defterde delik → kayıt güvensiz | defterdeki en yüksek + 1 (§5.3/5) |
| 5 | `status: approved` / `done` yazmak | enum dışı, denetim komutu okuyamaz | `proposed·accepted·deprecated·superseded` |
| 6 | Kararı `.ai/.rules/` içine koymak | kural ≠ karar (G7); geçmiş kaybolur | `.ai/adr/ADR-NNN-<slug>.md` |
| 7 | Supersede edince eski dosyayı silmek | denetim izi ve gerekçe kaybolur | eski dosya kalır, `superseded` işaretlenir |
| 8 | Sürüm/URL uydurmak (farklı DevExpress sürümü) | ADR-001 ihlali, kanıtsız iddia | `26.1` sabit; kanıtsız satır `⚠️` |
| 9 | EF Core'a geri dönmek / concat SQL | ADR-004 ve envanter G2/G10 ihlali | ADO.NET + Dapper, `@p1..@pN` |
| 10 | Yer tutucuyla yayımlamak | iskelet yayınlanıyor, içerik yok | §3.10 doldurulur, §5.3/2 = 0 |
| 11 | Şablon §'si ile ADR §'si karıştırılır | denetim yanlış dosyayı tarar | §3.2 notu · §5.3/4 üretileni tarar |

**SSUT beyanı:** Bu dosya ADR ailesinin **tek** kaynağıdır; her varyant ve `.ai/adr/index.md` yalnız buna bağlanır. Kopyalanan her satır ikinci gerçek kaynağı doğurur (envanter G12).
