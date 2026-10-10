---
title: "VersaCoder — API Dokümantasyon Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "API dokumantasyon sablonu: uclar, parametreler, ornekler ve hata kodlari"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — API Dokümantasyon Şablonu

**Zorunlu Bağlantılar:** [[.templates/index]] · [[.templates/index]] · [[.templates/query/Query-Template]] · [[.templates/backend/service-template]] · [[.templates/adr/adr-template]] · [[katman-readme-template]]

---

## §1 Amaç

`C:\versacoder\.ai\documentation\api\<servis>-api.md` dosyasının **tek üretim kaynağıdır**. API dokümanı kodun kopyası değil, kodun **yüzey**idir: hangi yol var, ne gönderiliyor, ne dönüyor, hangi durumda 4xx/5xx gelir. Kod değiştiğinde doküman değil **sözleşme** değişir; bu şablon sözleşmeyi kilitler.

SRP gereği bu şablon **kod yazmaz, SQL yazmaz, ADR üretmez**: imza `src/`'te, sorgu `Query-Template`, karar `adr-template` şeklindedir — burada yalnız **arayüz belgesi** üretilir.

| Alan | Değer |
|------|-------|
| Template Name | `api-doc-template.md` |
| Template Path | `.ai/.templates/documentation/api-doc-template.md` |
| Hedef Dosya | `.ai/documentation/api/<servis>-api.md` |
| Hedef Dizin | `.ai/documentation/api/` — yoksa üretilir |
| Üretilen Uzunluk | `120–300 satır` · `8–16 KB` |
| Kodlama | **UTF-8 (BOM'suz)** — Latin-1/CP1254 YASAK |
| Dil | Türkçe — HTTP metodu, yol, JSON anahtarı, başlık adı İngilizce |
| Zorunlu Blok | 9 blok `A1..A9` (§3.1) |
| Bağlam Bütçesi | 50.000 token/oturum — doküman payı ≤ %2 (§1.4) |
| Versiyon · Authority | `1.0.0` · `SSOT` (bu dosya) |
| Bootstrap | 2026-09-25 |

### §1.1 Okuma Sırası — Sözleşme Nereden Gelir

Doküman beş kaynaktan beslenir; **hiçbiri tahmin değildir**. Sıra ters çevrilirse doküman koddan değil hayalden yazılır.

| Sıra | Okunan | Ne verir | Tahmin Olur mu |
|------|--------|----------|----------------|
| 1 | `src/**` imza (Controller / handler / `IRequestHandler`) | gerçek yol + girdi | Hayır — kod gerçeği |
| 2 | `../query/Query-Template` | parametreli SQL (`@p1..@pN`) | Hayır |
| 3 | `../adr/adr-template` | sürüm ve karar kilidi | Hayır |
| 4 | `../backend/service-template` | use case sınırı | Hayır |
| 5 | `.ai/documentation/api/<servis>-api.md` (hedef) | bu şablonun çıktısı | — |
| 6 | `.ai/reports/*` | kanıt (test/ölçüm) | Hayır — sayı varsa |

**Boşluk riski:** `src/` diskte YOK (§1.2/4). Bu durumda doküman **uydurulmaz**: yol ve imza `⚠️ VERIFICATION REQUIRED` ile işaretlenir, doküman `DRAFT` kalır.

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

İddia → kanıt. Ölçüm: `C:\versacoder`, 2026-09-25, `Test-Path` + `Get-ChildItem`.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| 1 | `.ai/documentation/api/` (hedef dizin) | ❌ YOK | `Test-Path = False` — üretilir |
| 2 | `openapi.yaml` / `swagger.json` | ❌ YOK | `Test-Path = False` → §3.3 `⚠️` |
| 3 | `.ai/documentation/` (üst dizin) | ❌ YOK | `Test-Path = False` |
| 4 | `src/` (9 VersaCoder projesi) | ❌ YOK | `Test-Path = False` → imza doğrulanamaz |
| 5 | `tests/` + `.sln` | ❌ YOK | `Test-Path = False` |
| 6 | `C:\versacoder\CLAUDE.md` (kök) | ❌ YOK | `Test-Path = False` |
| 7 | `.ai/.rules/` (6 MD) | ✅ DOĞRULANDI | `coding-standards` · `data-access-rules` · `security-rules` … |
| 8 | `.ai/reports/00-kapsam-analizi.md` | ✅ DOĞRULANDI | 9.711 bayt |
| 9 | `.ai/.decisions/adr-index.md` | ✅ DOĞRULANDI | tek ADR kaydı |
| 10 | `.ai/scripts/check-links.md` · `measure-vault.md` | ✅ DOĞRULANDI | 6.291 · 5.845 bayt |
| 11 | `.ai/.templates/query/Query-Template.md` | ✅ DOĞRULANDI | 29.905 bayt |
| 12 | `.ai/.templates/backend/service-template.md` | ✅ DOĞRULANDI | 32.712 bayt |
| 13 | `.ai/.templates/index.md` (envanter) | ✅ DOĞRULANDI | 40.440 bayt |
| 14 | Kopyalama yasağı kapsamındaki harici vault'lar | 🔒 ERİŞİM YASAK | yalnız **yapı** referansı (G1) |
| 15 | `.ai/keys.md` (secret sözlüğü) | ⚠️ ERİŞİM DIŞI | değer asla dokümana girmez |

**Yorum yasağı:** Olmayan `src/` imzası için "Controller'da tanımlı" denmez; `⚠️ VERIFICATION REQUIRED` yazılır. §1.2/1–6 satırları **üretilen** dokümanda da görünür: bağlantı verilir, varlık iddia edilmez.

### §1.3 Doküman Bloğu ↔ Kod Gerçeği Farkı

Dört şey dokümanda **tekrar edilmez**, yalnız tanımlanır:

| Öğe | Dokümanda | Tekrar Nerede | Neden Tekrar Edilmez |
|-----|-----------|---------------|----------------------|
| SQL gövdesi | `@p1..@pN` adları | `Query-Template` | iki gerçek kaynağı (G12) |
| DTO sınıf kodu | alan + tip tablosu | `src/**/Dtos/` | kod bayatlar, tablo bayatlamaz |
| ADR kararı | tek satır + bağlantı | `.ai/.decisions/` | karar oradadır (G9) |
| Auth akışı | şema + başlık adı | `security-rules` | kural `.ai/.rules/`'tedir |
| Test senaryosu | kabul kriteri satırı | `tests/**` | kanıt testte |

### §1.4 Bağlam Bütçesi ve Uzunluk

| Öğe | Değer | Aşım Belirtisi |
|-----|-------|----------------|
| Oturum bütçesi | 50.000 token | — |
| Doküman payı | ≤ 1.000 token (%2) | > 1.500 → §4.3 RED |
| Üretilen satır | 120–300 | < 100 iskelet · > 360 RED |
| Endpoint satırı | 4–20 | > 24 → servisi böl |
| Örnek HTTP bloğu | 3–6 | her uç için tam kod yapıştırmak yasak |
| Güncelleme tetiği | imza / durum kodu / auth değişimi | `version` + `log.md` satırı |

```text
+----------------------------------------------------------------+
| [1] api-doc oku      amac + kapsam + zorunlu A1..A9 bloklari   |
|    v                                                           |
| [2] kaynagi oku      src imzasi + Query-Template + ADR         |
|    v                                                           |
| [3] sozlesme yaz     yol · metod · girdi · cikti · hata kodu   |
|    v                                                           |
| [4] HTTP ornegi <== KAPI: imza dogrulanmadan yayin YASAK       |
|    v                                                           |
| [5] olcum + rapor    120-300 satir · 8-16 KB · 0 yer tutucu    |
+----------------------------------------------------------------+
```

**Ne anlatır:** doküman üretim sırası; `[4]` imza kapısı. **Durum:** `[2]` diskte YOK (§1.2/4) → sıra planlanmış, `⚠️ VERIFICATION REQUIRED`.

---

## §2 Kapsam

**İçinde:** `<servis>-api.md` iskeleti, endpoint/DTO/hata tabloları, HTTP örnek biçimi, auth özeti, dokümana özgü ölçüm.
**Dışında:** kod (`src/`), SQL gövdesi (`Query-Template`), ADR metni (`adr-template`), kural (`.ai/.rules/`), wireframe (`ui-design/`).

| Kapsam | Kapsam Dışı |
|--------|-------------|
| A1..A9 blok sırası + zorunlu tablolar | Uç noktanın **implementasyonu** |
| `4xx/5xx` tablosu + tek örnek HTTP | SQL metni (yalnız `@p1..@pN` adları) |
| Auth şeması + başlık adı | Token üretme / anahtar saklama (G1) |
| Sürümleme + deprecate kuralı | ADR karar gerekçesi |
| 6 salt-okunur PowerShell komutu | OpenAPI üretimi (araç işi, bu şablon değil) |
| Doküman uzunluk/bütçe eşiği | Katman içi tasarım (`layer-spec-template`) |

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| Dosya Tipi | Sorumlu | Bu Şablondan mı |
|------------|---------|------------------|
| `.ai/documentation/api/<servis>-api.md` | docs-writer | ✅ |
| `src/**/README.md` (katman) | docs-writer | ❌ → `katman-readme-template` |
| `wiki/<konu>.md` | docs-writer | ❌ → `WikiPage-Template` |
| `.ai/reports/security-audit-*` | security | ❌ → `security-audit-template` |
| `.ai/documentation/**` (genel anlatım) | docs-writer | ❌ → `docs-md-template` |
| `src/**` kod dosyası | developer | ❌ → `backend/service-template` |
| `.ai/.rules/*.md` | security · architect | ❌ kural ≠ doküman |

### §2.2 Kapsam Sınırı Testi

**İlk `EVET`** şablonu belirler:

1. Dosya **bir HTTP arayüzünün** yol/metod/girdi/çıktısını mı anlatıyor? → **EVET** (bu şablon)
2. Bir `src/` katmanının genel yapısı mı? → `katman-readme-template`
3. Depo genelinde anlatım yazısı mı? → `docs-md-template`
4. `wiki/` altında bilgi sayfası mı? → `WikiPage-Template`
5. `.ai/reports/security-audit-*` mı? → `security-audit-template`
6. Hiçbiri değilse bu şablon **kullanılmaz**; talep §5.1 Adım 1'den açılır.

`HAYIR` soruları: SQL gövdesi var mı (→ `Query-Template`)? · `{{…}}` dışarıda mı (G4)? · `src/` kodu kopyalanmış mı (G12)?

---

## §3 Mimari

### §3.1 Zorunlu Blok Sırası (A1..A9)

Üretilen `<servis>-api.md` dokuz bloktur; sıra **değiştirilemez**. Eksik blok RED (§6.1 K5).

| # | Blok | İçerik | Min Satır |
|---|------|--------|-----------|
| A1 | `# H1` + **Zorunlu Bağlantılar** | 6 bağlantı, `·` ayracılı | 2 |
| A2 | Tek cümlelik sorumluluk | `>` ile 1–2 cümle | 2 |
| A3 | Genel bakış | sürüm · auth · taban URL · içerik tipi | 6 |
| A4 | Endpoint tablosu | `metod · yol · amaç · auth · durum` | 6 |
| A5 | DTO / şema tablosu | alan · tip · zorunlu · not | 6 |
| A6 | Hata kodları | `4xx/5xx` + kod + çözüm | 8 |
| A7 | HTTP örnekleri | 3–6 istek/yanıt bloğu | 14 |
| A8 | Güvenlik | şema · başlık · kapsam · limit | 6 |
| A9 | Doğrulama | ölçüm özeti + Quality Report | 8 |

### §3.2 Frontmatter, H1 ve Bağlantı Satırı

| Öğe | Kural | Doğru | Yanlış |
|-----|-------|-------|--------|
| Frontmatter | 7 alan, ilk 9 satır | `authority: primary` | alan eksik / yazım farkı |
| H1 | tek tane, `# ` | `# VersaCoder — <Servis> API` | iki H1 · `##` |
| Bağlantı | `**Zorunlu Bağlantılar:**` + 6 `[[…]]` | `·` ayracılı | 5 altı · markdown `](…)` |
| Dil | Türkçe; yol/anahtar İngilizce | `GET` · `Authorization` | karışık dil |
| Ürün adı | `VersaCoder` | — | **CoreMusic ASLA** (G13) |
| Sürüm | `apiVersion: 1` + tarih | `2026-09-25` | "en güncel" kanıtsız |

Doküman `.ai/documentation/api/` altında olduğu için bağlantı yolu `.ai/.templates/...` üst sıçramasıyla yazılır (`/../`), `\` yazılmaz.

### §3.3 A4 — Endpoint Tablosu Kuralları

Tek satır = tek uç nokta. Sıra: `GET` → `POST` → `PUT/PATCH` → `DELETE`, sonra yol uzunluğuna göre.

| Sütun | Kural | Yanlış |
|-------|-------|--------|
| `Metod` | tamamı büyük harf HTTP verb'ü | `get` · `Get` |
| `Yol` | kökten itibaren, parametre `{id}` | gömülü query string |
| `Amaç` | tek Türkçe cümle, fiille başlar | "çeşitli işlemler" |
| `Auth` | `anon` · `user` · `admin` · `service` | boş hücre |
| `Durum` | beklenen `2xx` (ör. `200` · `201`) | — |
| `Kaynak` | `src/.../Dosya.cs` ya da `⚠️` | uydurma yol |

```markdown
| Metod | Yol | Amaç | Auth | Durum | Kaynak |
|-------|-----|------|------|-------|--------|
| GET | `/api/v1/sessions/{id}` | Oturumu kimlikle getirir | user | 200 | ⚠️ VERIFICATION REQUIRED |
| POST | `/api/v1/sessions` | Yeni oturum açar | user | 201 | ⚠️ VERIFICATION REQUIRED |
```

### §3.4 A5 — DTO / Şema Tablosu

| Kural | Değer |
|-------|-------|
| Her DTO için **tek** tablo; alan adı kodla birebir | `sessionId` ≠ `session_id` |
| Tip sütunu C# tipi + JSON tipi | `Guid` / `string` |
| `Zorunlu` sütunu `evet`/`hayır` | belirsiz `?` yasak |
| Tarih/saat `ISO 8601` (`2026-09-25T13:30:00Z`) | serbest format yasak |
| Para/ölçü birimi yazılır | yalnız sayı |
| Gizli alan (parola, token) tabloda **yok** | varsa RED (§4.5 G6) |

### §3.5 A6 — Hata Kodları ve Durum Tablosu

| HTTP | Uygulama Kodu | Ne Zaman | Müşteri Ne Yapar |
|------|---------------|----------|------------------|
| `400` | `VALIDATION_FAILED` | alan eksik / tip hatalı | alan adlarını düzeltir |
| `401` | `UNAUTHENTICATED` | başlık yok / süresi dolmuş | token yenilenir |
| `403` | `FORBIDDEN` | yetki kapsamı dışı | rol talep eder |
| `404` | `NOT_FOUND` | kimlik yok | istek tekrarlanmaz |
| `409` | `CONFLICT` | çakışan durum | yeniden dener |
| `429` | `RATE_LIMITED` | limit aşıldı | tekrar dener |
| `500` | `INTERNAL` | beklenmeyen hata | `log.md`'ye düşer, destek |
| `503` | `UNAVAILABLE` | servis kapalı | tekrar dener |

**Kural:** gövde `{ "code": "...", "message": "...", "traceId": "..." }` biçimindedir; `message` Türkçe ve **teknik ayrıntı sızdırmaz** (stack trace yasak — §4.5 G7).

### §3.6 A7 — HTTP Örnek Biçimi

Her örnek **blok içinde**, `http` diliyle, tek satırlık yorumla başlar. Örnek gerçek veri kullanır, `foo`/`bar` kullanmaz.

```http
GET /api/v1/sessions/0f1c9d2e-8a44-4f6b-9e2a-1c7b5d3a0e11 HTTP/1.1
Host: localhost:5080
Authorization: Bearer <token>
Accept: application/json
```

```json
{
  "sessionId": "0f1c9d2e-8a44-4f6b-9e2a-1c7b5d3a0e11",
  "createdAt": "2026-09-25T13:30:00Z",
  "state": "active"
}
```

**Yasak:** gerçek anahtar, gerçek kişisel veri, `…` ile kesilmiş yarım JSON (RED).

### §3.7 A8 — Güvenlik Bloğu

| Öğe | Zorunlu İçerik | Yasak |
|-----|----------------|-------|
| Şema | `Bearer` · `ApiKey` · `anon` (hangisi) | "çeşitli yöntemler" |
| Başlık | tam ad: `Authorization: Bearer <token>` | `auth` kısaltması tek başına |
| Kapsam | uç başına rol (`user`/`admin`) | "ilgili roller" |
| Limit | istek/dakika sayısı ya da `bilinmiyor → ⚠️` | uydurma sayı |
| Veri | PII taşıyıp taşımadığı (`evet`/`hayır`) | sessiz geçme |
| Kayıt | log'a ne düşer (token **düşmez**) | token log'u (§4.5 G8) |

### §3.8 A9 — Doğrulama Kalıbı (ölçüm + bağlantı)

**Sözleşme tutarlı mı, boyut uygun mu?** Komutlar salt-okunurdur; özet dokümana gömülür, tam metin §5.3'tedir (SRP).

```powershell
$doc = 'C:\versacoder\.ai\documentation\api\ornek-api.md'
if (-not (Test-Path -LiteralPath $doc)) { 'API DOC YOK -> bu sablonla uretilecek'; return }
$n = (Get-Content -LiteralPath $doc).Count
$rows = (Select-String -LiteralPath $doc -Pattern '^\| (GET|POST|PUT|PATCH|DELETE) \|' -Encoding UTF8).Count
$par = (Select-String -LiteralPath $doc -Pattern '@p1\.\.@pN' -Encoding UTF8).Count
'giris {0} (120-300) · endpoint satiri {1} (4-20) · @p1..@pN gecen satir {2} (>=1)' -f $n, $rows, $par
```

### §3.9 Dolu VersaCoder Örneği ve Tam Kalıp

> Üst blok üretim standardı, alt blok yer tutucu taşıyan iskelettir. Yer tutucu **yalnız** alt blokta yaşar.

````markdown
# VersaCoder — Session API
**Zorunlu Bağlantılar:** [[.templates/index]] · [[.templates/index]] · [[.templates/query/Query-Template]] · [[.templates/backend/service-template]] · [[.templates/adr/adr-template]] · [[.templates/architecture/architecture-template]]

> Oturum servisinin HTTP sözleşmesini tanımlar; kod `src/`'tedir, burada yalnız yol/imza/durum yaşar.

## A4 Endpoint'ler
| Metod | Yol | Amaç | Auth | Durum |
|---|---|---|---|---|
| GET | `/api/v1/sessions/{id}` | Oturumu getirir | user | 200 |

## A6 Hata Kodları
| HTTP | Kod | Ne Zaman |
|---|---|---|
| 404 | `NOT_FOUND` | kimlik yok |

**apiVersion:** 1 · **Güncelleme:** 2026-09-25 · **Kaynak:** `⚠️ VERIFICATION REQUIRED`
````

**Tam kalıp (üretilecek dokümanın iskeleti):**

````markdown
---
title: "VersaCoder — {{SERVIS}} API"
type: doc
category: api
status: draft
version: 1.0.0
updated: {{YYYY-MM-DD}}
authority: primary
---
# VersaCoder — {{SERVIS}} API
**Zorunlu Bağlantılar:** [[{{BAG_1}}]] · [[{{BAG_2}}]] · [[{{BAG_3}}]] · [[{{BAG_4}}]] · [[{{BAG_5}}]] · [[{{BAG_6}}]]

> {{TEK_PARAGRAF — bu API ne yapar, kim kullanir}}

## A3 Genel Bakış
{{taban URL · apiVersion · auth · icerik tipi}}
## A4 Endpoint Tablosu
| Metod | Yol | Amaç | Auth | Durum | Kaynak |
|---|---|---|---|---|---|
| {{METOD}} | {{YOL}} | {{AMAC}} | {{AUTH}} | {{2XX}} | {{KAYNAK}} |
## A5 DTO Tablosu
{{alan · tip · zorunlu · not}}
## A6 Hata Kodları
{{4xx/5xx + kod + cozum}}
## A7 HTTP Örnekleri
{{3-6 ornek: istek + yanit}}
## A8 Güvenlik
{{sema · baslik · kapsam · limit · PII}}
## A9 Doğrulama
{{olcum ozeti + Quality Report}}
````

---

## §4 Kurallar

### §4.1 Yazım Sırası

| # | Adım | Kural | İhlal |
|---|------|-------|-------|
| 1 | Önce kaynak oku | `src/` yoksa §1.2/4 → `⚠️` ile devam | uydurma imza RED |
| 2 | Blok sırası A1..A9 | blok yer değiştirmez (§3.1) | RED |
| 3 | Tablolar sonra | DTO alan adları kodla birebir | RED |
| 4 | Örnek en son | imza doğrulanmadan örnek yayinlanmaz | RED (§1.1) |
| 5 | Ölçüm | §5.3/1–§5.3/6 | RED |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Süre |
|-------|-----|-----------|------|
| 1 | docs-writer | iskelet + tablolar (A1..A9) | 25 dk |
| 2 | reviewer | §6.1 20 madde + örnek gerçekliği | 10 dk |
| 3 | security | A8 güvenlik bloğu + `security-rules` | 6 dk |
| 4 | master-orchestrator | §5.3 çıktıları + §6.4 Quality Report | 6 dk |
| | **Toplam** | | **≈ 47 dk** |

> **3 RED** sonrası iş durur; gerekçe `master-orchestrator`'a yükseltilir.

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|-----|-------|-------|
| Üretilen doküman | 120–300 satır | < 100 iskelet · > 360 RED |
| Endpoint satırı | 4–20 | > 24 RED — servis bölünür |
| DTO tablosu | ≥ 2 (istek + yanıt) | 1 altı RED |
| Hata kodu satırı | ≥ 6 | 5 altı RED |
| HTTP örneği | 3–6 blok | 2 altı / 7 üstü RED |
| Bağlantı | tam 6 | 5 altı / 8 üstü RED |
| Token payı | ≤ 1.000 (%2) | > 1.500 RED (§1.4) |
| Bu şablon | 480–660 satır · 25–34 KB | dışı §5.3/1 |
| Kodlama | UTF-8 BOM'suz | Latin-1/CP1254 otomatik RED |

### §4.4 Değişiklik Kontrolü

| Olay | Aksiyon | Etkilenen | Kayıt |
|------|---------|-----------|-------|
| Uç nokta eklendi/çıkarıldı | A4 satırı + A6 hata satırı | `<servis>-api.md` | `version` minor + `log.md` |
| Alan adı değişti (DTO) | A5 tablosu + A7 örnek | iki dosya | `version` minor |
| Auth şeması değişti | A8 bloğu | tek dosya | `version` minor + security |
| Bağlanan dosya taşındı | bağlantı hedefi yeniden yazılır | tek dosya | §5.3/6 = 0 |

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | Harici vault'tan (`C:\www\…`) cümle/dosya kopyalanması | **DUR** — silinir, özgün yazılır |
| G2 | `EntityFramework` / EF Core referansı | **DUR** — ADR-004, ADO.NET + Dapper |
| G3 | Frontmatter 7 alan eksik / `authority: primary` bozuk | **DUR** — §5.3/4 |
| G4 | Üretilen dokümanda `{{…}}` (kalıp bloğu hariç) | **DUR** — §5.3/4 |
| G5 | Kanıtsız iddia: "test edildi", "üretimde çalışıyor" | **DUR** — sayı/kanıt yoksa `⚠️` |
| G6 | Gizli alan (token/parola/anahtar) tabloda veya örnekte | **DUR** — §5.3/5 |
| G7 | 5xx gövdesinde stack trace / iç yol sızdırmak | **DUR** — `traceId` yeterli |
| G8 | `Authorization` başlık değeri gerçek token içerir | **DUR** — `<token>` yer tutucu |
| G9 | ADR vermeden sürüm/karar kilidi iddia etmek | **DUR** — `.ai/.decisions/` |
| G10 | SQL gövdesi dokümana girmek (`SELECT …`) | **DUR** — yalnız `@p1..@pN` |
| G11 | `TreatWarningsAsErrors` / %90 coverage özetinin kaybı | **DUR** — A9'da kalır |
| G12 | Kod/DTO sınıfının dokümana kopyalanması | **DUR** — tabloya indirgenir |
| G13 | `CoreMusic` adının geçmesi | **DUR** — `VersaCoder` |
| G14 | Örnek `foo`/`bar`/`xxx` verisi | **DUR** — gerçekçi, sahte veri |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|------|-------|-------|-------|-------|
| 1 | Disk ölçümü | `Test-Path` listesi (§1.2) | güncel kanıt tablosu | docs-writer | 5 dk |
| 2 | İskelet seç | §3.1 (A1..A9) | blok listesi | docs-writer | 3 dk |
| 3 | Bağlantıyı yaz | 6 zorunlu hedef | H1 altı satır | docs-writer | 4 dk |
| 4 | Endpoint tablosu | kod imzası (`⚠️` dahil) | A4 | docs-writer | 8 dk |
| 5 | DTO + hata tablosu | §3.4 · §3.5 | A5 + A6 | docs-writer | 8 dk |
| 6 | HTTP örnekleri | §3.6 | 3–6 blok | docs-writer | 6 dk |
| 7 | Güvenlik bloğu | `security-rules` | A8 | security | 6 dk |
| 8 | Ölç | §5.3/1–§5.3/6 | 6 çıktı | reviewer | 6 dk |
| 9 | Onay + kayıt | §6.1 20/20 | Quality Report | orchestrator | 5 dk |
| | **Toplam** | | | | **≈ 51 dk** |

**Zaman bütçesi:** 1–3 ≈ 12 dk · 4–7 ≈ 28 dk · 8–9 ≈ 11 dk. **1 saatten uzun** sürüyorsa örnekler şişirilmiş demektir → A7'de her uç için tam kod yerine tek temsili örnek yazılır.

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Kök Neden | Düzeltme |
|---|-----------|---------|-----------|----------|
| 1 | Uydurma imza | diskte olmayan `src/` yolu "mevcut" | §1.2 ölçülmemiş | `⚠️ VERIFICATION REQUIRED` |
| 2 | SQL sızıntısı | dokümanda `SELECT` gövdesi | `Query-Template` unutulmuş | yalnız `@p1..@pN` (G10) |
| 3 | Eksik hata kodu | yalnız `200` anlatılmış | hata düşünülmemiş | §3.5 tablosu |
| 4 | Eksik bağlantı | 5 veya daha az `[[…]]` | hedef unutulmuş | §3.2 altı hedefi tamamla |
| 5 | Örnek tutarsızlığı | tabloda `sessionId`, örnekte `id` | DTO kopyalanmış | §3.4 birebir kuralı |
| 6 | Uzunluk sapması | > 360 / < 100 satır | tam API mi, iskelet mi | §4.3 aralığına çek |
| 7 | Gizli veri | örnekte gerçek token/PII | örnek üretilirken kopyalanmış | `<token>` + sahte UUID |
| 8 | Mojibake + ürün adı | bozuk Türkçe · `CoreMusic` | ANSI kaydetme | UTF-8 BOM'suz · G13 |

### §5.3 Salt Okunur Doğrulama Komutları

Altı komut da **hiçbir dosyayı değiştirmez**; hedef yoksa dürüstçe söyler.

```powershell
# 1) YAPI — 7 ust bolum · 480-660 satir · 25-34 KB (bu sablon)
$doc = 'C:\versacoder\.ai\.templates\documentation\api-doc-template.md'
$n = (Get-Content -LiteralPath $doc).Count
$kb = [math]::Round((Get-Item -LiteralPath $doc).Length / 1KB, 1)
$bol = (Select-String -LiteralPath $doc -Pattern '^## §\d ' -Encoding UTF8).Count
'boluH2 {0} (7 olmali) · satir {1} (480-660) · KB {2} (25-34) · {3}' -f $bol, $n, $kb,
  $(if ($bol -eq 7 -and $n -ge 480 -and $n -le 660 -and $kb -ge 25 -and $kb -le 34) { 'GECTI' } else { 'KALDI' })

# 2) YER TUTUCU — [{}] kalibinda, yalniz kalip blogundan sonrasi
#    -CaseSensitive zorunlu: tr-TR + IgnoreCase, [A-Z] sinifinda SERVIS gibi
#    I iceren kelimeleri yakalamaz (kultur duyarli karsilikli donusum)
$kalip = (Select-String -LiteralPath $doc -Pattern '^### §3\.9' -Encoding UTF8).LineNumber
$sizan = @(Select-String -LiteralPath $doc -Pattern '[{}]{2}[A-Z0-9_]+[}]{2}' -Encoding UTF8 -CaseSensitive |
           Where-Object { $_.LineNumber -lt $kalip })
$kalipYazi = @(Select-String -LiteralPath $doc -Pattern '[{}]{2}[A-Z0-9_]+[}]{2}' -Encoding UTF8 -CaseSensitive)
'kalip satiri {0} · oncesinde {1} (0 olmali) · toplam {2}' -f $kalip, $sizan.Count, $kalipYazi.Count

# 3) SÖZLESME — endpoint satiri · hata kodu · @p1..@pN · EF Core 0 · concat SQL 0
$doc = 'C:\versacoder\.ai\.templates\documentation\api-doc-template.md'
$ep  = (Select-String -LiteralPath $doc -Pattern '^\| (GET|POST|PUT|PATCH|DELETE) \|' -Encoding UTF8).Count
$err = (Select-String -LiteralPath $doc -Pattern '^\| `4\d\d` \|' -Encoding UTF8).Count
$p   = (Select-String -LiteralPath $doc -Pattern '@p1\.\.@pN' -Encoding UTF8).Count
$ef  = (Select-String -LiteralPath $doc -Pattern 'EntityFrameworkCore' -Encoding UTF8).Count
'endpoint {0} (>=4) · hata kodu {1} (>=6) · @p1..@pN satiri {2} (>=1) · EF referansi {3} (0 olmali)' -f $ep, $err, $p, $ef

# 4) FRONTMATTER — 7 alan tam; kisa satir sayimi
$need = 'title','type','category','status','version','updated','authority'
$head = (Get-Content -LiteralPath $doc -TotalCount 12) -join "`n"
$eksik = @($need | Where-Object { $head -notmatch ('(?m)^' + [regex]::Escape($_) + ':') })
'eksik alan {0} ({1}) · ilk satir {2}' -f $eksik.Count, ($eksik -join ','), ((Get-Content -LiteralPath $doc -TotalCount 1)[0])

# 5) GIZLI VERI — token/parola/anahtar degeri dokumana girmis mi (kalip blogu haric)
$desen = '(?i)(api[_-]?key|password|passwd|secret|bearer)\s*[:=]\s*[A-Za-z0-9+/=]{16,}'
$sizan2 = @(Select-String -LiteralPath $doc -Pattern $desen -Encoding UTF8)
'supheli sifre satiri {0} (0 olmali)' -f $sizan2.Count
$sizan2 | ForEach-Object { '  BULGU -> satir ' + $_.LineNumber }

# 6) KODLAMA + BAGLANTI — BOM yok, mojibake 0, [[.templates/documentation/]] hedefleri diskte
$bytes = [System.IO.File]::ReadAllBytes($doc)
$bom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
$text = [System.IO.File]::ReadAllText($doc, [System.Text.UTF8Encoding]::new($false))
$moj = ([regex]::Matches($text, "\uFFFD|Ã[\u0080-\u00BF]|Â[\u0080-\u00BF]")).Count
$bag = [regex]::Matches($text, '\[\[([^\]]+)\]\]') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
$base = 'C:\versacoder\.ai\.templates\documentation'
$kirik = @($bag | Where-Object { -not (Test-Path -LiteralPath (Join-Path $base ($_.Replace('/','\') + '.md'))) })
'BOM={0} · mojibake={1} (0 olmali) · baglanti {2} · kirik {3} (0 olmali)' -f $bom, $moj, $bag.Count, $kirik.Count
$kirik | ForEach-Object { '  KIRIK -> ' + $_ }
```

**Geçme kriteri:** #1 `GECTI` · #2 `0 sızan` · #3 `endpoint ≥ 4 · hata ≥ 6 · EF 0` · #4 `0 eksik` · #5 `0 bulgu` · #6 `BOM=False · mojibake=0 · kirik=0`.

### §5.4 Quality Report

```markdown
## Quality Report — <servis>-api.md (YYYY-MM-DD HH:MM)

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Üretilen doküman | <n> satır / <KB> KB | 120–300 · 8–16 | 🟢/🔴 |
| A1..A9 blok | <n>/9 | 9 | 🟢/🔴 |
| Endpoint · hata kodu · örnek | <n>/<n>/<n> | ≥4 · ≥6 · 3–6 | 🟢/🔴 |
| Bağlantı / kırık | 6 / 0 | tam 6 · 0 | 🟢/🔴 |
| Yer tutucu / gizli veri / mojibake | n/n/n | 0/0/0 | 🟢/🔴 |
| Guardrail G1..G14 | ihlal n | 0 | 🟢/🔴 |
| Şablon ölçümü (§5.3/1) | <satır>/<KB> | 480–660 · 25–34 | 🟢/🔴 |

**Karar:** 🟢 ONAYLANDI (doküman `.ai/documentation/api/`'ye yazılır, `log.md`'ye 1 satır) / 🔴 RED (<gerekçe>)
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi (20 madde)

| # | Kontrol | Beklenen | İlgili |
|---|---------|----------|--------|
| K1 | Frontmatter 7 alan + `authority: primary` birebir | 7/7 | §3.2 · §5.3/4 |
| K2 | H1 tek + `**Zorunlu Bağlantılar:**` satırı | 1 satır | §3.2 |
| K3 | Bağlantı sayısı | 6 | §4.3 |
| K4 | Blok haritası A1..A9 | 9/9 | §3.1 |
| K5 | §1.1–§1.4 dolu; §1.2 ≥ 12 kanıt satırı | 4/4 · ≥ 12 | §1 |
| K6 | §1.3 tekrar yasağı tablosu | 5 satır | §1.3 |
| K7 | §1.4 bütçe 1.000 token + kutu 66 | var · 0 uyuşmayan | §1.4 |
| K8 | §2.1 + §2.2 dolu | 2/2 | §2 |
| K9 | §3.1 A1..A9 tablosu | 9 satır | §3.1 |
| K10 | §3.3 endpoint kural tablosu + örnek | var | §3.3 |
| K11 | §3.5 hata kodu ≥ 8 satır | ≥ 8 | §3.5 |
| K12 | §3.6 iki HTTP örneği (istek + yanıt) | 2 blok | §3.6 |
| K13 | §3.9 dolu örnek + tam kalıp (`{{…}}` yalnız kalıpta) | 2 blok | §3.9 |
| K14 | §4.5 G1..G14 | 14 madde | §4.5 |
| K15 | §5.1 9 adım · §5.2 ≥ 8 hata modu | 9 · 8 | §5 |
| K16 | §5.3 salt-okunur komut ≥ 5 | 6 | §5.3 |
| K17 | §5.4 Quality Report | var | §5.4 |
| K18 | §6.2–§6.5 dolu | 4/4 | §6 |
| K19 | §7.1 kaynak tablosu + §7.2 Red Team ≥ 10 satır | ≥ 10 | §7 |
| K20 | Ölçüm: 480–660 satır · 25–34 KB · `## §` = 7 · mojibake 0 | 4/4 | §5.3/1·6 |

### §6.2 Red Sinyalleri (Otomatik DUR)

| # | Sinyal | Aksiyon |
|---|--------|---------|
| R1 | §5.3/6'da kırık bağlantı | **DUR** — hedef üretilir ya da bağlantı silinir |
| R2 | Üretilen dokümanda `{{…}}` | **DUR** — doldurulur (G4) |
| R3 | Dokümanda `SELECT`/`INSERT` gövdesi | **DUR** — `Query-Template` (G10) |
| R4 | `EntityFramework` geçen satır | **DUR** — G2 |
| R5 | Örnekte gerçek token/PII/gizli alan | **DUR** — G6 · G8 |
| R6 | Olmayan `src/` yolu "mevcut" diye anılmış | **DUR** — `⚠️ VERIFICATION REQUIRED` |
| R7 | 5xx örneğinde stack trace | **DUR** — G7 |
| R8 | `CoreMusic` / mojibake / BOM | **DUR** — G13 · §5.3/6 |
| R9 | Ölçüm dışı uzunluk | **DUR** — §4.3 |
| R10 | §5.3 çıktısı olmadan "tamam" beyanı | **DUR** — çıktı istenir |

### §6.3 Sürüm / Değişiklik Geçiş Kuralları

| Değişiklik | `version` | `updated` | Kayıt |
|------------|-----------|-----------|-------|
| Uç nokta ekleme/çıkarma | `minor` | evet | `log.md` + §6.4 |
| Yalnız düzeltme (ımle/bağlantı) | `patch` | evet | `log.md` tek satır |
| Auth şeması / durum kodu değişimi | `minor` | evet | security onayı |
| Bölüm adı değişimi (`A4` gibi) | `major` | evet | bağlantı veren tüm dosyalar |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|-------|--------|
| `.ai/.templates/documentation/api-doc-template.md` | **bu dosya** — API dokümanının tek kaynağı |
| `.ai/.templates/query/Query-Template.md` | SQL sözleşmesi (`@p1..@pN`) — gövde burada |
| `.ai/.templates/backend/service-template.md` | use case/imza kaynağı |
| `.ai/.templates/adr/adr-template.md` | sürüm/karar kilidi (G9) |
| `.ai/.templates/architecture/architecture-template.md` | katman yönü — API hangi katmanda |
| `.ai/.templates/documentation/katman-readme-template.md` | katman README'si bu dokümana bağlanır |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Kanıt |
|---|---------|----------|-------|
| K1 | Bu şablonun satır aralığı | 480–660 | §5.3/1 |
| K2 | Bu şablonun KB aralığı | 25–34 | §5.3/1 |
| K3 | Hedef dosya yolu envantere birebir | `.ai/documentation/api/<servis>-api.md` | `index.md` |
| K4 | §1.2 disk tablosu üretim anında yeniden ölçülmüş | tarihli | §5.3/1 |
| K5 | Kutu satırları 66 karakter | 0 uyuşmayan | §5.3/4 · §6.1 K7 |
| K6 | Yer tutucu yalnız §3.9 kalıp bloğunda | 0 sızan | §5.3/2 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| Kaynak | Yol | Ne İçin | Durum (2026-09-25) |
|--------|-----|---------|---------------------|
| Şablon envanteri + kalite sözleşmesi | `.ai/.templates/index.md` | kayıt + §3.7 | ✅ 40.440 bayt |
| SQL sözleşmesi | `.ai/.templates/query/Query-Template.md` | A4/A7 `@p1..@pN` | ✅ 29.905 bayt |
| Uç/imza kaynağı | `.ai/.templates/backend/service-template.md` | §1.1 adım 4 | ✅ 32.712 bayt |
| Karar defteri | `.ai/.templates/adr/adr-template.md` | G9 · sürüm kilidi | ✅ 35.702 bayt |
| Katman yönü | `.ai/.templates/architecture/architecture-template.md` | API hangi katman | ✅ 34.059 bayt |
| Güvenlik kuralı | `.ai/.rules/security-rules.md` | A8 bloğu | ✅ 15.322 bayt |
| Kod + imza gerçeği | `src/**` | §1.1 adım 1 | ❌ YOK → `⚠️` |
| Yasaklı vault'lar | `C:\www\coremusic.net\…` · `C:\www\versacoder\…` | yalnız **yapı** referansı | 🔒 kopyalama YASAK |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Neden Yanlış | Doğrusu | Yakalama |
|---|------|--------------|---------|----------|
| 1 | Kodu dokümana kopyalamak | iki gerçek kaynağı, biri çürür (G12) | alan tablosu + kod bağlantısı | gözden geçirme |
| 2 | `src/` yokken imza uydurmak | ajan yanlış yere gider | `⚠️ VERIFICATION REQUIRED` (§1.2/4) | §1.2 tablosu |
| 3 | SQL gövdesini örneğe koymak | G10 + Query-Template ihlali | yalnız `@p1..@pN` | §5.3/3 |
| 4 | Yalnız `200` yazmak | hata yolu anlatılmamış | §3.5 hata tablosu | K11 |
| 5 | Örnekte gerçek token/PII | sızıntı, denetim bulgusu | `<token>` + sahte UUID | §5.3/5 |
| 6 | `foo`/`bar` örnekleri | sözleşmeyi anlamayı zorlaştırır | gerçekçi sahte veri | G14 |
| 7 | DTO alan adını değiştirmek | istemci kırılır, doküman bayat | birebir kural (§3.4) | §5.3/3 |
| 8 | 6 bağlantıyı 12'ye çıkarmak | okuma bütçesi şişer | tam 6, `·` ayracılı | §5.3/6 |
| 9 | EF Core / `DbSet` geçirmek | ADR-004 + G2 ihlali | ADO.NET + Dapper | §5.3/3 |
| 10 | `CoreMusic` adını geçirmek | yasaklı vault izi (G1) | `VersaCoder` | §5.3/6 |
| 11 | ANSI/CP1254 kaydetmek | bozuk Türkçe, okunmaz | UTF-8 **BOM'suz** | §5.3/6 |
| 12 | Ölçümden "tamam" demek | kanıtsız beyan (G5) | 6 komutun çıktısı | §5.3/1–6 |

**SSUT beyanı:** `<servis>-api.md` üretiminin **tek** kaynağı bu dosyadır; kod `src/`'te, SQL `Query-Template`'tedir — ikisi de bu dokümana **bağlanır**, kopyalanmaz. `⚠️ VERIFICATION REQUIRED` yazan hiçbir satır "mevcut" diye anılmaz.
