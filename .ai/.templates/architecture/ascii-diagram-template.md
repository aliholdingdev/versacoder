---
title: "VersaCoder — ASCII/Unicode Box Diagram Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "ASCII kutu diyagram sablonu: kenar karakterleri, hizalama ve olcu kontrolu"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — ASCII/Unicode Box Diagram Şablonu

**Zorunlu Bağlantılar:** [[.templates/index]] · [[../CLAUDE.md]] · [[../index]] · [[../../AGENTS.md]] · [[../../WORKFLOW.md]] · [[../../architecture/index.md]]

---

## §1 Amaç

Bu şablon, `.ai/` vault'undaki **her görsel anlatımın** yalnız `|`, `-`, `+` ASCII karakterleriyle, **garantili hizada** üretilmesi için zorunludur (CLAUDE G4: Unicode kutu çizim `U+2500-U+257F` yasaktır). Mermaid yalnızca *ek* olarak bulunabilir; **asıl kaynak bu şablondaki ASCII kutudur** (master prompt PART 4.2: "mermaid varsa ASCII karşılığını da yaz").

Düz metin oklar (`-->`, `|`, `+`) tek başına yeterli değildir: katman sınırı, dallanma, kapı (gate) ve geri akış gibi ilişkiler **kutu** ile gösterilmezse vault'ta görsel tutarsızlık oluşur. Bu şablon tek bir çizim alfabesi, tek bir hizalama kuralı ve tek bir doğrulama komutu dayatır.

| Alan | Değer |
|------|-------|
| Template Name | `ascii-diagram-template.md` |
| Template Path | `.ai/.templates/architecture/ascii-diagram-template.md` |
| Hedef Dosya Tipi | `.ai/` altındaki her MD'deki görsel blok |
| Hedef Dizin | `.ai/architecture/`, `.ai/ui-design/`, `.ai/reports/`, kök MD'ler |
| Karakter Seti | ASCII `|` `-` `+` (kutu) + oklar `→ ← ↑ ↓ v` — Unicode kutu çizim `U+2500-U+257F` yasak |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Standart Genişlik | **64 sütun** (iç dolgu); dar yerde 48, tablo içinde 40 |
| Zorunlu Bölüm | §3.10 dolu örnek (D1–D6) + §5.3 hizalama ölçer |
| Bilinçli İstisna | Tablo içindeki tek hücre diyagramlar (genişlik hücreye uysun) |
| Dil | Türkçe (ç ğ ı İ ö ş ü doğru) — kod adları/slug İngilizce |
| Versiyon | 1.0.0 |
| Authority | SSOT (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.1 Neden Box ASCII, Düz Metin Değil?

Düz metin akış (`A --> B`) yalnız iki düğümü taşır. Vault'ta anlatılması gereken 4 şey var ve **hepsi kutu ister**:

| Anlatım İhtiyacı | Düz metin | Kutu | Neden |
|------------------|-----------|------|-------|
| Katman sınırı (içeride/dışarıda) | ✗ | ✅ | Sınır fiziksel olarak görünmeli |
| Aynı anda birden çok şey (UI + Cli) | zayıf | ✅ | Paralellik yan yana olur |
| Süreç/kapı sırası (GATE-000..005) | kısmen | ✅ | Kapı nerede "dur" diyor görülmeli |
| Bağımlılık yönü + ihlal tespiti | ✗ | ✅ | Yukarı bakan ok = ihlal |
| Tablo içi özet | fazla | ⚠️ | Hücre genişliğine sığmaz → istisna |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

| İddia | Durum | Kanıt |
|-------|-------|-------|
| `.ai/architecture/` klasörü var | ✅ DOĞRULANDI (2026-09-25) | `architecture/layers.md` (4,6 KB) |
| `.ai/architecture/diagrams.md` | ❌ YOK | bu şablonla üretilecek |
| `.ai/ui-design/` klasörü | ❌ YOK | PART 4'te açılacak |
| `.ai/.templates/architecture/` klasörü | ✅ DOĞRULANDI | Part B kapsamında |
| `.ai/.templates/ascii-diagram-template.md` | ✅ BU DOSYA | — |
| Vault'ta mevcut mermaid blokları | ⚠️ VERIFICATION REQUIRED | tarama komutu §5.3/4 |
| Mevcut diyagramlarda hizalama | ⚠️ VERIFICATION REQUIRED | tarama komutu §5.3/1 |

**Yorum yasağı:** Var olmayan dosya için "şurada" denmez; `⚠️ VERIFICATION REQUIRED` yazılır.

### §1.3 Diyagram Türleri

| Tür | Ne anlatır | Zorunlu mu | Örnek | Tipik Yer |
|-----|-----------|------------|-------|-----------|
| **T1 Yığın** | Katman/sıra, alttan üste bağımlılık | Evet | D1 | `architecture/` |
| **T2 Dallanma** | Bir kaynaktan iki+ yol | Evet | D2 | ADR, `spec/` |
| **T3 Akış** | Sıralı süreç + kapı | Evet | D3 | `gates/`, `WORKFLOW.md` |
| **T4 Yol/zincir** | Uçtan uca veri akışı | Evet | D4 | `architecture/` |
| **T5 Hiyerarşi** | Ağaç/dağıtım (parent→child) | Evet | D5 | `.agents/`, `subagent-template` |
| **T6 Döngü** | Tekrarlayan yaşam döngüsü | Evet | D6 | `session-log-template` |
| **T7 Karşılaştırma** | İki sütun yan yana | İsteğe | D2 | ADR §3 Alternatifler |
| **T8 Kapsama/örtüşme** | Kümeler arası ilişki | İsteğe | — | `spec/` |

### §1.4 Neden Hizalama Zorunlu?

Kutudaki her satır **aynı görünür uzunlukta** olmalıdır. Bir karakter sapması, sağ kenarın kırılmasına ve tüm şablonun "el yazması" görünmesine yol açar. Bu yüzden hizalama **elle tahmin edilmez**; §5.3/1 ölçüm komutu ile **kanıtlanır**. Ölçüm geçmeden diyagram vault'a **kabul edilmez**.

---

## §2 Kapsam

Box diyagram üretimi ve denetimi. Kapsam dışı: içerik kararları, kod üretimi, renk/emoji.

| Kapsam | Kapsam Dışı |
|--------|-------------|
| ASCII kutu alfabesi (`|` `-` `+`) + genişlik standardı (64) | Diyagramın **ne** anlatacağı (→ ilgili şablon/ADR) |
| Hizalama kuralı + otomatik ölçüm komutu | İçerik doğruluğu (→ `[[../adr/adr-architecture-template]]`) |
| 8 diyagram türü (T1–T8) | Renk/emoji/GIF/mermaid-only (→ §4.5 G5) |
| D1–D6 dolu VersaCoder örnekleri | PNG/şematik resim (→ `ui-design/`) |
| Tam kalıp (`{{PLACEHOLDER}}`) | Kod dosyaları içindeki diyagram |
| Kodlama (UTF-8 BOM'suz) + mojibake denetimi | Tablo içi tek hücre ifadeler (istisna §4.5 G7) |
| Vault senkronunda bozuk diyagram onarımı | — |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Sorumlu | Bu Şablondan mı? |
|------------|---------|-------------------|
| `.ai/architecture/*.md` | architect | ✅ |
| `.ai/adr/*` §3 diyagramları | architect | ✅ (yalnız çizim bloğu) |
| `.ai/ui-design/*.md` | ui-designer | ✅ (tel kafesi ayrı → `[[../ui-design/wireframe-template]]`) |
| `.ai/gates/*.md` akışları | master-orchestrator | ✅ |
| `.ai/.agents/*.md` | docs-writer | ✅ |
| `.ai/reports/*.md` | yazan rol | ✅ |
| `.ai/*.md` kök | master-orchestrator | ✅ |
| `src/**/*.cs` içindeki şema | developer | ❌ kod yorumu |
| `.ai/.templates/*` | docs-writer | ✅ (bu şablon) |

### §2.2 Kapsam Sınırı Testi

1. Bu blok **karakter tabanlı ve hizalanabilir** mi? (evet → bu şablon)
2. Bu blok **süreç/katman/iliski** mi anlatıyor? (evet → bu şablon)
3. Bu blok **renk/emoji/PNG** içeriyor mu? (evet → **hayır**, bu şablon dışı)
4. Bu blok **yalnız iki düğüm** mi gösteriyor? (evet → tek satır `A → B` yeterli, kutu opsiyonel)

---

## §3 Mimari

### §3.1 Çizim Alfabesi (kullanılacak tek karakter seti)

| Amaç | Karakter | Not |
|------|----------|-----|
| Yatay çizgi | `-` | tek yatay çizgi; Unicode `U+2500` yasak |
| Dikey çizgi | `|` | tek dikey çizgi; Unicode `U+2502` yasak |
| Köşe ↖ ↗ | `+` `+` | köşe daima `+` |
| Köşe ↙ ↘ | `+` `+` | köşe daima `+` |
| Dallanma + + | `+` `+` | SEP satırı |
| Üst dallanma | `+` | — |
| Alt dallanma | `+` | — |
| Kesişim | `+` | — |
| Vurgu / kapı | `<== KAPI:` | kapı etiketi; ASCII'de çift çizgi ayrı yok |
| Dolu/boş kutu | `█` `░` | dolgu = kutu çizimi değil; dolu = kapalı, boş = açık |
| İleri ok | `→` | G4 yalnız `U+2500-U+257F`'i yasaklar; ok 1 birim, 66 sütunu bozmaz |
| Geri ok | `←` | geri dönüş planı |
| Aşağı ok | `↓` `v` | dikey akışta `v` (satır tasarrufu) |
| Koşullu ok | `?` | `evet ↓ / hayır →` ayrımı |
| Ayraç | `·` `|` | **kutu DIŞINDA** metin ayracı |

**Yasaklar:** Unicode kutu çizim (`U+2500-U+257F`), emoji (`✅ ⚡`), ASCII art yüz/figür, Latin-1 `Ä±`.

### §3.2 Hizalama Kuralı (tek kural)

```
Satır = "| " + içerik + dolgu + "|"
Görünür uzunluk = 64 karakter   (içerik + dolgu = 63)
dolgu = 63 - uzunluk(içerik)
```

- Ölçüm **karakter** üzerinden yapılır (UTF-16 code unit), byte üzerinden değil → Türkçe harf 1 sayılır.
- İçerik 63 karakteri aşarsa **satır kısaltılır**, kutu genişletilmez (genişlik sabittir).
- Karışık genişlik: T7 karşılaştırmada iki sütun `+` ile ayrılır, her sütun 31 karakter.
- ASCII diyagram satırları ` ```text ` kod bloğu **içinde** durur (MD dönüştürücüsü bozmasın).

### §3.3 T1 — Yığın (katman) iskeleti

```text
+----------------------------------------------------------------+
| {{K1_AD}}            {{K1_AÇIKLAMA}}                           |
+----------------------------------------------------------------+
| {{K2_AD}}            {{K2_AÇIKLAMA}}                           |
+----------------------------------------------------------------+
| {{K3_AD}}            {{K3_AÇIKLAMA}}                           |
+----------------------------------------------------------------+
```

**Kural:** En üst = en çok bağımlı olan, en alt = en az bağımlı. Her katman sınırında `+-+-+` SEP satırı zorunlu.

### §3.4 T2 — Dallanma iskeleti

```text
+----------------------------------------------------------------+
| {{KAYNAK}}                                                     |
+----------------------------------------------------------------+
|     +-→ {{YOL_1}}                                              |
|     +-→ {{YOL_2}}                                              |
|     +-→ {{YOL_3}}                                              |
+----------------------------------------------------------------+
```

### §3.5 T3 — Akış + kapı iskeleti

```text
+----------------------------------------------------------------+
| {{ADIM_1}}                                                     |
|    v                                                           |
| {{GATE}}   <== KAPI: {{KOŞUL}}                                 |
|    v                                                           |
| {{ADIM_N}}                                                     |
+----------------------------------------------------------------+
```

**Kural:** Kapı satırında `<== KAPI:` etiketi zorunlu; "nerede duruyor" görünür olmalı.

### §3.6 T4 — Yol/zincir iskeleti

```text
+----------------------------------------------------------------+
| {{BAŞLANGIÇ}}                                                  |
+----------------------------------------------------------------+
| {{ARA_1}}                                                      |
+----------------------------------------------------------------+
| {{ARA_N}}                                                      |
+----------------------------------------------------------------+
| {{BİTİŞ}}                                                      |
+----------------------------------------------------------------+
```

### §3.7 T5 — Hiyerarşi iskeleti

```text
+----------------------------------------------------------------+
| {{ROOT}}                                                       |
| +-- {{ÇOCUK_1}}                                                |
| |   +-- {{TORUN_1}}                                            |
| |   +-- {{TORUN_2}}                                            |
| +-- {{ÇOCUK_2}}                                                |
| +-- {{ÇOCUK_3}}                                                |
+----------------------------------------------------------------+
```

### §3.8 T6 — Döngü iskeleti

```text
+----------------------------------------------------------------+
| [1] {{ADIM_1}}                                                 |
|    v                                                           |
| [N] {{ADIM_N}}                                                 |
|    v                                                           |
| [1] ← geri dönüş ({{TETİK}})                                   |
+----------------------------------------------------------------+
```

### §3.9 T7 — Karşılaştırma iskeleti (tek kutu, iki sütun)

```text
+----------------------------+-----------------------------      +
| {{SOL_BASLIK}}             | {{SAĞ_BASLIK}}                    |
+----------------------------+-----------------------------      +
| {{SOL_1}}                  | {{SAĞ_1}}                         |
+----------------------------+-----------------------------      +
| {{SOL_2}}                  | {{SAĞ_2}}                         |
+----------------------------+-----------------------------      +
```

### §3.10 Dolu VersaCoder Örnekleri (D1–D6 — üretim standardı)

> Bu altı örnek **gerçek VersaCoder** içeriğidir. Üretirken hizalama, kolon genişliği ve karakter seti olarak bunları takip edin.

#### D1 — Katman Yığını (T1)

```text
+----------------------------------------------------------------+
| K7  Host             composition root + DI                     |
+----------------------------------------------------------------+
| K6  UI               VersaCoder.Desktop (RibbonForm)           |
| K6  UI               VersaCoder.Cli (parser)                   |
+----------------------------------------------------------------+
| K5  Protocol         mesaj şeması + tool sözleşmeleri          |
+----------------------------------------------------------------+
| K4  Infrastructure   ADO.NET + Dapper, dosya, git, LLM         |
+----------------------------------------------------------------+
| K3  Application      use case + CQRS                           |
+----------------------------------------------------------------+
| K2  Domain           entity + invariant                        |
+----------------------------------------------------------------+
| K1  Abstractions     contract, result, event                   |
+----------------------------------------------------------------+
| K0  CrossCutting     logging, zaman, yardımcılar               |
+----------------------------------------------------------------+
```

#### D2 — İkili Hedef / TFM Dallanması (T2 + T7)

```text
+----------------------------------------------------------------+
| Directory.Build.props                                          |
| TargetFrameworks = net48 ; net10.0-windows                     |
+----------------------------------------------------------------+
|    net48 (legacy kanıt)      net10.0-windows (modern)          |
|    DevExpress 25.2.8         DevExpress 25.2.8                 |
|    #if NET48 platform farkı  UseWindowsForms=true              |
|    Designer.cs elle          Designer.cs elle                  |
+----------------------------------------------------------------+
```

#### D3 — Gate Akışı (T3)

```text
+----------------------------------------------------------------+
| GATE-000  discovery      okuma + envanter                      |
|    v                                                           |
| GATE-001  reference      opencode + coremusic (salt okunur)    |
|    v                                                           |
| GATE-002  requirements   spec + ADR taslağı                    |
|    v                                                           |
| GATE-003  ui  TEL KAFES ONAYI <== KAPI: onaysiz ekran YASAK    |
|    v                                                           |
| GATE-004  architecture   katman + bağımlılık                   |
|    v                                                           |
| GATE-005  impl-plan      görev bölme                           |
|    v                                                           |
| PART 6 KODLAMA   →   PART 8 DOĞRULAMA (build ×2 + test)        |
+----------------------------------------------------------------+
```

#### D4 — Veri Erişim Yolu (T4)

```text
+----------------------------------------------------------------+
| VersaCoder.Desktop  /  VersaCoder.Cli                          |
+----------------------------------------------------------------+
| Application    IUseCase.HandleAsync()                          |
+----------------------------------------------------------------+
| Infrastructure I*Repository   (arayüz: Abstractions)           |
+----------------------------------------------------------------+
| Infrastructure *Repository → Dapper sorgusu (@p1, @p2)         |
+----------------------------------------------------------------+
| AdoConnectionFactory → IDbConnection (Sqlite | SqlClient)      |
+----------------------------------------------------------------+
| versacoder.db  +  schema_version  +  saf .sql script           |
+----------------------------------------------------------------+
```

#### D5 — Agent Dağıtımı (T5)

```text
+----------------------------------------------------------------+
| Kullanıcı promptu                                              |
|    v                                                           |
| master-orchestrator   bütçe 50.000 token / oturum              |
| +-- architect     → ADR + katman                               |
| +-- developer     → kod (katman sırası)                        |
| +-- reviewer      → SOLID + bağımlılık denetimi                |
| +-- testing       → dotnet test, %90 coverage                  |
| +-- security      → parameterized SQL + secret denetimi        |
| +-- docs-writer   → vault senkronu + rapor                     |
|                                                                |
| checkpoint: her aşamada (n/8) raporu zorunlu                   |
+----------------------------------------------------------------+
```

#### D6 — Oturum Yaşam Döngüsü (T6)

```text
+----------------------------------------------------------------+
| [1]  vault oku            .ai/ loader                          |
|    v                                                           |
| [2]  exploration gate     5-10 explore agent                   |
|    v                                                           |
| [3]  context sakla        .ai/SESSIONS/                        |
|    v                                                           |
| [4]  prompt maker         50-100 soru                          |
|    v                                                           |
| [5]  intent router        confidence + vault filtresi          |
|    v                                                           |
| [6]  instruction          vault dosyaları, 50K bütçe           |
|    v                                                           |
| [7]  todo oluştur         görev böl + neden-sonuç              |
|    v                                                           |
| [8]  agent dağıt          uzman rollere yönlendir              |
|    v                                                           |
| [9→12]  kod → review → test → merge                            |
|    v                                                           |
| [13] session kaydet       .ai/SESSIONS/ + .ai/MEMORY/          |
|    ← geri dönüş: yeni promptta [1]'den başla                   |
+----------------------------------------------------------------+
```

### §3.11 Tam Kalıp İskeleti (kopyala-yapıştır)

```text
+----------------------------------------------------------------+
| {{BASLIK_1}}                                                   |
+----------------------------------------------------------------+
| {{SATIR_1}}                                                    |
|    v                                                           |
| {{KAPI_ETIKET}}   <== KAPI: {{KOŞUL}}                          |
|    v                                                           |
| {{SATIR_N}}                                                    |
+----------------------------------------------------------------+
```

Yerine kullanıcak MD bloğu:

````markdown
### {{DİYAGRAM_ADI}} (T{{TIp}})

```text
{{BOX}}
```

**Ne anlatır:** {{AÇIKLAMA}}
**Kaynak:** [[{{BAĞLANTI}}]] · **Doğrulama:** §5.3/1 — {{TARİH}}
````

---

## §4 Kurallar

### §4.1 Diyagram Türü → Kullanım Sırası

| Öncelik | Şart | Kullanılacak tür |
|---------|------|------------------|
| 1 | Katman/sıra anlatımı | T1 yığın |
| 2 | Sıralı süreç + kapı | T3 akış |
| 3 | Uçtan uca veri yolu | T4 zincir |
| 4 | Birden çok yol/çatal | T2 dallanma |
| 5 | Ağaç dağılımı | T5 hiyerarşi |
| 6 | Tekrarlayan süreç | T6 döngü |
| 7 | İki seçenek karşılaştırma | T7 |

**Tür seçimi rastgele değildir:** anlatım önce listede yukarıda olan türle denenir; ancak uymuyorsa aşağı inilir ve gerekçe §7.1'e yazılır.

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Çıktı |
|-------|-----|-------|
| 1 | Çizen rol (ilgili şablonun sahibi) | diyagram + açıklama |
| 2 | Ölçüm | §5.3/1 çıktısı = 0 uyuşmayan satır |
| 3 | Denetim | reviewer — doğru tür mü, doğru içerik mi |
| 4 | Vault senkronu | docs-writer — link + `log.md` |

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Standart genişlik | 64 | dar alan: 48 · tablo içi: 40 |
| Maksimum satır | 16 | aşımda diyagram 2'ye bölünür |
| Maksimum düğüm | 12 | aşımda alt diyagrama inilir |
| Kod bloğu | ` ```text ` | ` ``` ` boş bırakmak yasak |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

Diyagram içerik olarak değişirse: açıklama satırı (`**Ne anlatır:**`) da güncellenir, `updated` tarihi artar, `.ai/log.md`'ye tek satır düşülür. Sadece dolgu değişiyorsa (hizalama düzeltmesi) tarih değişmez, yalnız §5.3/1 çıktısı yeniden alınır.

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | `-` yerine `-`, `|` yerine `|` | diyagram reddedilir |
| G2 | Satır genişlikleri eşit değil (§5.3/1 > 0) | **DUR** — ölçüm geçene kadar |
| G3 | Kod bloğu `text` değil | MD dönüştürme bozulur → RED |
| G4 | Emoji / renk / GIF | RED |
| G5 | Mermaid **tek** kaynak (ASCII karşılığı yok) | RED — master prompt PART 4.2 |
| G6 | Latin-1/CP1254 kodlama | dosya UTF-8 ile yeniden yazılır |
| G7 | Tablo içi hücrede kutu (istisna) | serbest — ama hücreden taşma RED |
| G8 | Genişlik 64'ün dışında (standart alanda) | RED |
| G9 | İçerik 63 karakteri aşıp kırpılmamış | RED |
| G10 | Türkçe karakter (ç ğ ı İ ö ş ü) bozuk | RED |
| G11 | Kapı (gate) etiketi `<== KAPI:` yok | T3 diyagram RED |
| G12 | Kanıtsız "hizalı/doğru" iddiası | §5.3/1 çıktısı istenir |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|------|
| 1 | Tür seç | anlatım ihtiyacı | T1–T8 | çizen rol | 1 dk |
| 2 | Kalıbı kopyala | §3.3–§3.9 | iskelet | çizen rol | 1 dk |
| 3 | İçeriği doldur | gerçek dosya/sayı | dolu kutu | çizen rol | 5 dk |
| 4 | Ölç | §5.3/1 | 0 uyuşmayan | çizen rol | 1 dk |
| 5 | Düzelt | uyuşmayan satırlar | hizalı kutu | çizen rol | 3 dk |
| 6 | Açıklama yaz | `**Ne anlatır:**` | 1 satır | çizen rol | 1 dk |
| 7 | Kapı etiketi (T3 ise) | gate no | `<== KAPI:` | çizen rol | 1 dk |
| 8 | Denetim | diyagram | ✅/RED | reviewer | 3 dk |
| 9 | Senkron | `log.md`, link | kayıt | docs-writer | 2 dk |

**Zaman bütçesi:** adım 1-7 ≈12 dk, 8-9 ≈5 dk → **≈17 dk/diyagram**. Üç diyagramlık set ≈45 dk.

### §5.2 Hata Modları

| Hata | Belirti | Düzeltme |
|------|---------|----------|
| Sağ kenar kırık | satır uzunlukları farklı | §5.3/1 → taşan satırı kısalt |
| Karışan çizgi | Unicode kutu (`U+2500`/`U+2502`) kalmış → hizalama bozulur | §3.1 alfabesi (`\|` `-` `+`) ile yeniden çiz |
| Emoji sızması | `✅` kutu içinde | `█`/`░` veya metinle değiştir |
| Yanlış tür | süreç T1 ile çizilmiş | §4.1 sırasına dön |
| Mermaid tek başına | ASCII yok | ASCII karşılığını yaz |
| Bozuk kodlama | `Ä±`, `?` | UTF-8 BOM'suz kaydet |
| Aşırı karmaşık | >16 satır / >12 düğüm | alt diyagramlara böl |
| Kapı görünmüyor | T3'te etiket yok | `<== KAPI:` ekle |

### §5.3 Salt Okunur Doğrulama Komutları

```powershell
# 1) HIZALAMA ÖLÇER — çıkan satır sayısı 0 OLMALI
Get-ChildItem -Path 'C:\versacoder\.ai' -Recurse -Filter *.md | ForEach-Object {
  $f = $_.FullName
  $box = $false
  $lines = Get-Content -LiteralPath $f -Encoding UTF8
  for ($i = 0; $i -lt $lines.Count; $i++) {
    $l = $lines[$i]
    if ($l -match '^```text')  { $box = $true; continue }
    if ($l -match '^```' -and $box) { $box = $false; continue }
    if ($box -and $l -match '^[+|]') {
      if ($l.Length -ne 66) { "{0}:{1}  uzunluk={2} (66 olmali)" -f $f, ($i+1), $l.Length }
    }
  }
}

# 2) YASAK KARAKTER — 0 olmalı (kutu içinde tire/boru)
Select-String -Path 'C:\versacoder\.ai\*.md','C:\versacoder\.ai\*\*.md' `
  -Pattern '^\s*\|.*[-]{3,}\s*$' | Measure-Object

# 3) KOD BLOĞU TÜRÜ — '```text' dışındaki kutu bloğu 0 olmalı
Select-String -Path 'C:\versacoder\.ai\*\*.md' -Pattern '^```$' | Measure-Object

# 4) MERMİD KARŞILIĞI — 'mermaid' bloğu varsa yanındaki text bloğu da olmalı
Select-String -Path 'C:\versacoder\.ai\*\*.md' -Pattern 'mermaid'

# 5) KODLAMA — UTF-8 BOM'suz, mojibake 0 olmalı
Select-String -Path 'C:\versacoder\.ai\*\*.md' -Pattern 'Ã|Â|\ufffd' | Measure-Object

# 6) KAPI ETİKETİ — T3 diyagramlarında '<== KAPI:' sayısı = T3 sayısı olmalı
Select-String -Path 'C:\versacoder\.ai\*\*.md' -Pattern '<== KAPI:'
```

**Geçme kriteri:** #1 = 0 satır, #2 = 0, #3 = 0, #5 = 0 (gerçek mojibake; yasak-listesi örnekleri hariç).

### §5.4 Quality Report

```markdown
## Box Diagram Kalite Raporu — YYYY-MM-DD

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Diyagram bloğu (` ```text `) | N | — | ℹ️ |
| Hizasız satır (§5.3/1) | N | 0 | 🟢/🔴 |
| Yasak karakter (`-`/`\|` çizim) | N | 0 | 🟢/🔴 |
| Mermaid = ASCII karşılıksız | N | 0 | 🟢/🔴 |
| Emoji kutu içinde | N | 0 | 🟢/🔴 |
| `<== KAPI:` eksik T3 | N | 0 | 🟢/🔴 |

**Karar:** 🟢 vault senkronu serbest / 🔴 düzeltmeye gönderildi
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi

- [ ] Frontmatter tam (`title, type, category, status, version, updated, authority: primary`)
- [ ] H1 + `**Zorunlu Bağlantılar:**` (6 bağlantı)
- [ ] §1.1–§1.4 dolu
- [ ] §2.1–§2.2 dolu
- [ ] §3.1 alfabe tablosu (≥16 karakter)
- [ ] §3.2 hizalama kuralı (64/63 hesabı yazılı)
- [ ] §3.3–§3.9 sekiz tür iskeleti
- [ ] §3.10 **D1–D6 dolu örnekler** (hepsi ` ```text ` içinde)
- [ ] §3.11 tam kalıp (`{{PLACEHOLDER}}`)
- [ ] §4.1 tür öncelik tablosu
- [ ] §4.3 ölçek tablosu (64/16/12)
- [ ] §4.5 G1..G12 dolu
- [ ] §5.1 9 adımlı tablo + zaman bütçesi
- [ ] §5.2 ≥8 hata modu
- [ ] §5.3 **6 salt-okunur komut**
- [ ] §5.4 Quality Report bloğu
- [ ] §6.2 Red Sinyalleri
- [ ] §7.2 Red Team ≥10 satır
- [ ] `{{PLACEHOLDER}}` = 0 (kalıp bloğu hariç)
- [ ] Kutu satırları 64 sütuna hizalı (§5.3/1 = 0)
- [ ] Türkçe/Mojibake temiz

### §6.2 Red Sinyalleri (Otomatik DUR)

| Sinyal | Aksiyon |
|--------|---------|
| §5.3/1 çıktısı > 0 satır | **DUR** — hizalama düzeltilir |
| Kutu içinde `-` ile çizgi | **DUR** — `-` ile yeniden çiz |
| ` ```text ` yok | **DUR** — blok türü düzeltilir |
| Mermaid var, ASCII yok | **DUR** — ASCII karşılığı yazılır |
| Emoji kutu içinde | **DUR** — kaldırılır |
| İçerik `{{...}}` kalmış | **DUR** — doldurulur |
| 64 sütun dışında | **DUR** — standart genişliğe çekilir |
| Kodlama bozuk (`Ä±`) | **DUR** — UTF-8 BOM'suz kaydet |

### §6.3 Tür/Sürüm Geçiş Kuralları

| Değişiklik | Etki |
|-----------|------|
| Sadece dolgu/hizalama | tarih değişmez, §5.3/1 yeniden alınır |
| Diyagram içeriği değişti | açıklama + `updated` + `log.md` satırı |
| Tür değişti (T1→T3) | yeni bölüm + gerekçe §7.1'e |
| Genişlik değişti (64→48) | yalnız dar alanda, gerekçe zorunlu |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/architecture/architecture-template.md` | diyagramı üreten şablon |
| `.ai/.templates/architecture/ascii-diagram-template.md` | **bu dosya** |
| `.ai/.templates/adr/adr-architecture-template.md` | ADR içindeki diyagram blokları |
| `.ai/.templates/ui-design/wireframe-template.md` | tel kafesi (kutu değil, piksel ızgara) |
| `.ai/architecture/index.md` | katman gerçeği |
| `.ai/WORKFLOW.md` | gate gerçeği |

### §6.5 Envantere Özgü Kontroller

- [ ] D1'deki katman adları `architecture/index.md` ile birebir aynı
- [ ] D3'teki gate listesi `WORKFLOW.md` ile birebir aynı
- [ ] D4'te `EF Core` geçmiyor (ADR-004 ihlali olur)
- [ ] D4'te `@p1, @p2` görünür (parameterized kanıtı)
- [ ] D5'teki roller `.ai/.agents/` kataloğu ile aynı
- [ ] D2'de sürüm `25.2.8` (ADR-001 ihlali olmasın)

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne için |
|--------|-----|---------|
| Katman gerçeği | `.ai/architecture/index.md` | D1 doğruluğu |
| Akış/kapı gerçeği | `.ai/WORKFLOW.md` | D3 doğruluğu |
| Şablon envanteri | `.ai/.templates/index.md` | SRP, bu dosyada tekrarlanmaz |
| Tel kafesi | `.ai/.templates/ui-design/wireframe-template.md` | piksel ızgara ayrı standart |
| ADR diyagramı | `.ai/.templates/adr/adr-architecture-template.md` | ADR içi blok |
| Anahtar sözlük | `.ai/keys.md` | terim tutarlılığı |
| Tür seçim gerekçesi | bu dosya §7.1 | T1–T8 öncelik sırası |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu |
|---|------|--------------|---------|
| 1 | Unicode kutu çizim (`U+2500`-ailesi) ile çizmek | G4 taraması `U+2500-U+257F` RED verir | §3.1 alfabesi (`\|` `-` `+`) ile yeniden çiz |
| 2 | Satırları elle hizalamak | mutlaka kayar | §5.3/1 ile ölç, sonra onayla |
| 3 | ` ``` ` boş bırakmak | dönüştürücü bozar | ` ```text ` yaz |
| 4 | Mermaid'i tek kaynak yapmak | vault'ta okunmaz | ASCII karşılığı zorunlu |
| 5 | Emoji koymak (`✅`) | monospace'de 2 sütun | `█`/`░` veya düz metin |
| 6 | Kutuyu 80 sütuna açmak | standart kırılır | 64 sabit; uzun satırı kısalt |
| 7 | Kapıyı gizlemek | süreç nerede duruyor bilinmez | `<== KAPI:` etiketi zorunlu |
| 8 | 16+ satırlık dev kutu | okunmaz | alt diyagramlara böl |
| 9 | Uydurma katman adı | vault'la çelişir | `architecture/index.md`'den al |
| 10 | Hizasız olduğu hâlde "tamam" demek | kanıtsız iddia (master prompt §6/5) | §5.3/1 çıktısını göster |
