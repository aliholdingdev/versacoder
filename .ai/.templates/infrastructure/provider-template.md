---
title: "VersaCoder — Harici Sağlayıcı (LLM) Adaptörü Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "LLM saglayici adaptoru sablonu: cagri sozlesmesi, hata sekli ve fallback"
---

# VersaCoder — Harici Sağlayıcı (LLM) Adaptörü Şablonu

**Zorunlu Bağlantılar:** [[.templates/index]] · [[.templates/CLAUDE]] · [[../architecture/ascii-diagram-template]] · [[../adr/adr-architecture-template]] · [[../session-log-template]] · [[./configuration-template]]

> **Tek cümlelik sorumluluk:** Bu dosya, VersaCoder'ın harici bir LLM/sağlayıcıya bağlandığı **her adaptörün** (`src/VersaCoder.Infrastructure/Providers/*.cs`) arayüz, secret, hata eşleme, yeniden deneme ve doğrulama sözleşmesinin tek kaynağıdır (SSOT).

---

## §1 Amaç

### §1.1 Neden bu şablon zorunlu

Harici sağlayıcı, kod tabanının **tek dış bağımlılığıdır**: saldırı yüzeyi, maliyet kaynağı ve kırılma noktası oradadır. Sağlayıcı adı, endpoint veya API key satır satır kodun içine gömülürse üç şey aynı anda kaybolur: sağlayıcı değiştirilebilirliği, secret denetimi ve test edilebilirlik. Bu şablon Adapter + Factory desenini, secret saklama kuralını ve **durum kodu → sonuç** eşleme tablosunu tek yerde dayatır.

| # | Bu şablon neyi zorunlu kılar | Kırılırsa ne olur |
|---|---|---|
| 1 | Tek arayüz (`ILlmProvider`) + adaptör başına tek sınıf | Her sağlayıcı için ayrı kod → 2x bakım |
| 2 | Secret yalnız environment / user-secrets / DPAPI | Repo'da açık anahtar → sızıntı, hesap hırsızlığı |
| 3 | Zaman aşımı + sınırlı yeniden deneme + üstel backoff | Sağlayıcı yavaşsa uygulama kilitlenir (UI donar) |
| 4 | HTTP durum kodu → içerikli sonuç eşlemesi | Kullanıcıya "hata 500" görünür, neden bilinmez |
| 5 | Model/sürüm sabitleme | Sağlayıcı sürüm değişince sessiz davranış farkı |
| 6 | Logda secret maskesi | Log = ikinci bir secret deposu olur |
| 7 | Sahte provider ile test | Testler ağa bağımlı → CI'da kırılır |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

Kanıt `2026-09-25` tarihinde `C:\versacoder` üzerinde okunarak alındı. Kanıt yoksa **uydurulmaz**.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| E1 | Depo `C:\versacoder` var, git'li | ✅ DOĞRULANDI | `git status` → `?? .ai/`, `.git` klasörü mevcut |
| E2 | Depo içeriği = `.ai/` + `.git/` (kod henüz yok) | ✅ DOĞRULANDI | `Get-ChildItem -Force` → 2 girdi |
| E3 | `.ai/.templates/index.md` envanterde `#23 provider-template` (Part B, `T-VER`) | ✅ OKUNDU | envanter 38.816 bayt, §3.3 satır #23 |
| E4 | `ascii-diagram-template.md` (altın standart) | ✅ OKUNDU | 38.121 bayt / 644 satır |
| E5 | `adr/adr-template.md` (altın standart) | ❌ YOK | `adr/` altında `adr-template.md` yok; varyantlar var (7 dosya) |
| E6 | `adr/adr-architecture-template.md` (yapı referansı) | ✅ OKUNDU | 31.247 bayt — yalnız yapı için kullanıldı |
| E7 | `.ai/.templates/infrastructure/` klasörü | ❌ YOK | `Test-Path = False`; bu iş kapsamında açılır |
| E8 | `.ai/architecture/index.md`, `.ai/.rules/`, `.ai/keys.md`, `.ai/log.md` | ❌ YOK | `.ai/` altında tek girdi: `.templates/`, kök MD = 0 |
| E9 | `Directory.Build.props` / `Directory.Packages.props` / `global.json` / `.gitignore` | ❌ YOK | `Test-Path = False` (4/4) |
| E10 | `*.csproj`, `appsettings*.json`, `src/**` | ❌ YOK | Recursive sayım = 0 / 0 / 0 |
| E11 | .NET SDK **10.0.401** kurulu | ✅ DOĞRULANDI | `dotnet --list-sdks` → tek satır `10.0.401` |
| E12 | DevExpress **25.2.8** NuGet önbellekte | ❌ YOK | `~/.nuget/packages/devexpress*` = 0 girdi |
| E13 | Bu dosya yazıldı | ✅ BU DOSYA | `.ai/.templates/infrastructure/provider-template.md` |

**Yorum yasağı:** Var olmayan dosya için "şurada tanımlı" denmez; satır `⚠️ VERIFICATION REQUIRED` olarak kalır ve §5.3 komutlarıyla ancak dosya diske geldiğinde kalkar. Kod katmanı (`Providers/*.cs`) henüz yoktur → tüm kod iddiaları bu şablonun **sözleşmesidir**, gözlemlenmiş gerçek değil.

### §1.3 Sağlayıcı Türü → Şablon Eşlemesi

| Tür | Örnek adaptör | Bu şablonda? | Ek Zorunluluk |
|-----|---------------|--------------|----------------|
| Bulut LLM (chat/completion) | `OpenAiProvider`, `AnthropicProvider` | ✅ Tam | Hata eşleme + sabit model |
| Yerel/özel endpoint (self-host) | `OllamaProvider`, `InternalLlmProvider` | ✅ Tam | TLS + host allowlist |
| Gömülü/embedded model | `LocalEmbeddingProvider` | ✅ Tam | Bellek/tahmin bütçesi |
| Code-completion (stream) | `StreamingCompletionProvider` | ✅ Tam | İptal (`CancellationToken`) + parça maskeleme |
| Arama/doküman getirici | `SearchProvider` | Kısmen | Yalnız §3.5–§3.7 (secret/hata) |
| Git/SaaS sağlayıcı (LLM dışı) | `GitHubProvider` | Kısmen | §3.5 secret + §4.5 G1 |
| Veritabanı/ORM sağlayıcısı | — | ❌ | ADO.NET + Dapper (ADR-004), ayrı şablon |
| Dosya/sistem sağlayıcısı | — | ❌ | `CrossCutting` yardımcıları |

### §1.4 Neden Adapter + Factory (ve neden SRP)

Tek arayüz (`ILlmProvider`), sağlayıcı başına tek adaptör sınıfı, seçim tek yerde (`ProviderFactory`). Adaptör **yalnız** dönüşüm yapar: VersaCoder istek modeli → sağlayıcı protokolü → VersaCoder sonuç modeli. Zaman aşımı, backoff, rate limit ve maskeleme adaptörün içinde **değil**, §3.6'da sözleşmesi verilen ortak bileşenlerdedir; aksi hâlde üç adaptörde üç ayrı yeniden deneme davranışı doğar.

| Bileşen | Sorumlu | SRP İhlali Belirtisi |
|---------|---------|----------------------|
| `ILlmProvider` | sözleşme (istek/sonuç tipleri) | Sınıf hem sözleşme hem HTTP ise SRP ihlali |
| `*Provider.cs` | protokol dönüşümü | İçinde `retry` döngüsü varsa ihlal |
| `ProviderFactory` | sağlayıcı **seçimi** | İçinde HTTP çağrısı varsa ihlal |
| `ProviderRetryPolicy` | backoff + sınır | Her adaptörde tekrarlandıysa ihlal |
| `SecretMasker` | log maskeleme | Bir log satırı maskesizse ihlal (G8) |
| `ProviderErrorMapper` | durum kodu → sonuç | `throw new Exception("hata")` varsa ihlal |

---

## §2 Kapsam

**İçinde:** harici LLM/sağlayıcı adaptörünün arayüzü, factory seçimi, secret saklama, timeout/backoff/rate limit, hata eşleme, model sabitleme, maskeleme, sahte provider testi, secret taraması.
**Dışında:** yapılandırma katmanları (→ [[./configuration-template]]), veri erişimi (ADO.NET + Dapper), ekran/tel kafes (GATE-003), paket sürüm politikası (→ CPM, configuration §3.3).

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya / Konu | Sorumlu Rol | Bu Şablondan mı? |
|---|--------------|-------------|-------------------|
| 1 | `src/VersaCoder.Infrastructure/Providers/*.cs` | developer | ✅ Evet |
| 2 | `src/VersaCoder.Infrastructure/Providers/ProviderFactory.cs` | developer | ✅ Evet |
| 3 | Secret kaynağı (env / user-secrets / DPAPI) | devops-engineer | ✅ Evet (§3.5) |
| 4 | `.ai/keys.md` (vault anahtar sözlüğü) | security | ⚠️ Dosya yok (§1.2 E8) |
| 5 | `appsettings*.json` / MSBuild özellikleri | devops-engineer | ❌ → configuration-template |
| 6 | `Query-Template` / Dapper sorgusu | database-optimizer | ❌ |
| 7 | Provider birim/entegrasyon testi | testing | ✅ Kısmen (§3.8 test stratejisi) |
| 8 | `.ai/adr/ADR-NNN-*.md` (sağlayıcı seçimi kararı) | architect | ❌ ADR şablonu |

### §2.2 Kapsam Sınırı Testi

Üretilecek dosyaya sırayla sor; **ilk `EVET`** doğru şablonu verir:

1. **Dosya harici bir HTTP/sağlayıcı protokolünü VersaCoder modeline çeviriyor mu?** → `EVET` = bu şablon.
2. **Dosya API key/endpoint/secret okuyor veya taşıyor mu?** → `EVET` = bu şablon (+ G1).
3. **Sorun "hangi değer okunur, hangi sırada?" mı?** → configuration-template.
4. **Sorun sağlayıcının yanıtını kaydetmek mi (tablo/şema)?** → migration / query şablonu.
5. **Dosya yalnız UI gösteriyorsa (yanıt metnini çizmek)?** → frontend şablonları; yalnız §4.5 G1/G8 uygulanır.

---

## §3 Mimari

### §3.1 Bileşen Envanteri

| # | Bileşen | Ad (öneri) | Katman | Görev |
|---|---------|-----------|--------|-------|
| 1 | Arayüz | `ILlmProvider` | Abstractions | Tek sözleşme: istek → sonuç |
| 2 | Adaptör | `OpenAiProvider.cs` vb. | Infrastructure | Protokol dönüşümü |
| 3 | Factory | `ProviderFactory.cs` | Infrastructure | Ayar adı → adaptör örneği |
| 4 | Retry | `ProviderRetryPolicy` | Infrastructure | Sınır + üstel backoff |
| 5 | Rate limit | `ProviderRateLimiter` | Infrastructure | Dakikalık istek/token tavanı |
| 6 | Eşleyici | `ProviderErrorMapper` | Infrastructure | HTTP kodu → `ProviderResult` |
| 7 | Maskeleyici | `SecretMasker` | CrossCutting | Logda anahtar/PII maskesi |
| 8 | Test sahtesi | `FakeLlmProvider` | Tests | Ağ olmadan sözleşme testi |
| 9 | Ayar | `ProviderOptions` | Abstractions | Ad, endpoint, model, timeout |
| 10 | Sonuç | `ProviderResult<T>` | Abstractions | `Ok | Hata | YenidenDene` |

### §3.2 Arayüz Sözleşmesi (`ILlmProvider`)

```csharp
// Katman: VersaCoder.Abstractions  (Infrastructure bunu uygular, ters yön YASAK)
public interface ILlmProvider
{
    string ProviderName { get; }            // "openai" | "anthropic" | "ollama"
    string ModelId { get; }                 // sabitlenmiş sürüm (§3.8)
    Task<ProviderResult<ChatResponse>> CompleteAsync(
        ChatRequest request,
        CancellationToken cancellationToken);   // iptal her çağrıda zorunlu
}
```

| Sözleşme Kuralı | Değer | İhlal Belirtisi |
|-----------------|-------|-----------------|
| Geri dönüş tipi | `ProviderResult<T>` (Exception değil) | `throw` ile hata taşıma |
| İptal | `CancellationToken` parametresi zorunlu | UI kapatılınca çağrı sürer |
| Zaman aşımı | caller değil, ortak politika belirler (§3.6) | adaptörde kendi `Timeout` değeri |
| Model | `ModelId` somut, sabit | `"latest"`, `":v1"` gibi yüzen etiket |
| Sözleşme sahibi | yalnız `Abstractions` | adaptör kendi arayüzünü tanımlarsa ikiye bölünür |

### §3.3 Adaptör Akışı (T4 — uçtan uca)

| Aşama | Nerede | Girdi | Çıktı | Bağlantı |
|---|---|---|---|---|
| 1 Çağrı | Application / use case | `ChatRequest` | — | `ILlmProvider` |
| 2 Seçim | `ProviderFactory` | ayar adı (§3.5) | `ILlmProvider` | — |
| 3 Limit | `ProviderRateLimiter` | istek sayacı | izin / bekle | §3.6 |
| 4 Deneme | `ProviderRetryPolicy` | deneme #1..N | backoff | §3.6 |
| 5 Protokol | adaptör sınıfı | `ChatRequest` | HTTP gövdesi | §3.2 |
| 6 Çağrı | `HttpClient` (tekli, TLS zorunlu) | HTTPS endpoint | HTTP kodu | G2 |
| 7 Eşleme | `ProviderErrorMapper` | durum kodu | `ProviderResult` | §3.7 |
| 8 Maske | `SecretMasker` | log metni | maskeli log | G8 |
| 9 Sonuç | Application | `ProviderResult<ChatResponse>` | UI/Cli | — |

### §3.4 Seçici Factory (`ProviderFactory.cs`)

```csharp
// Katman: VersaCoder.Infrastructure — tek seçim noktası; HTTP burada YOK (SRP)
public sealed class ProviderFactory : IProviderFactory
{
    private readonly IReadOnlyDictionary<string, Func<ILlmProvider>> _map;
    public ILlmProvider Create(string providerName) =>
        _map.TryGetValue(providerName.ToLowerInvariant(), out var f)
            ? f()
            : ProviderResult.Fail($"Bilinmeyen sağlayıcı: {providerName}");   // fail-fast, §6.2
}
```

| Kural | Ayrıntı |
|---|---|
| Anahtar | küçük harf sağlayıcı adı (`openai`, `anthropic`, `ollama`) |
| Bilinmeyen anahtar | sessiz fallback **yok** → açık hata sonucu |
| Örnekleme | adaptörler singleton/tek `HttpClient` ile (socket tükenmesine karşı) |
| Test | aynı harita `FakeLlmProvider` ile doldurulabilir |

### §3.5 Endpoint + API Key Saklama Kuralı

| # | Kaynak | Öncelik | Kabul | Yasak mı |
|---|--------|---------|-------|----------|
| 1 | Ortam değişkeni (`PROVIDER_API_KEY`) | 1 (en yüksek) | ✅ | — |
| 2 | `dotnet user-secrets` (dev makinesi) | 2 | ✅ | — |
| 3 | DPAPI ile korunan yerel dosya (Windows, `CurrentUser` kapsamı) | 3 | ✅ | — |
| 4 | Ortam-a özgü gizli config (`appsettings.Production.json`, repo dışı) | 4 | ✅ (deploy) | — |
| 5 | `appsettings.json` → yalnız **şablon** anahtar adı, boş değer | 5 | ⚠️ | Dolu değer **YASAK** |
| 6 | `.cs` kaynağı içi literal (`"sk-..."`) | — | ❌ | **KESİN YASAK (G1)** |
| 7 | Repo'ya commit edilmiş `.env` / `secrets.json` | — | ❌ | **KESİN YASAK (G1)** |
| 8 | `C:\versacoder\.ai\keys.md` içine tam anahtar yazmak | — | ❌ | YASAK — yalnız **ad + uç nokta** |

| Placeholder | Kalıp |
|---|---|
| `{{SAGLAYICI_ADI}}` | küçük harf, ör. `openai` |
| `{{PROVIDER_SECRET_ENV}}` | `PROVIDER_{{SAGLAYICI_ADI}}_API_KEY` biçiminde |
| `{{MODEL_SABIT}}` | sürümlü, ör. `gpt-4o-mini-2024-07-18` (yüzen etiket YASAK) |
| `{{ZAMAN_ASIMI_SN}}` | tamsayı saniye, örn. `60` |
| `{{MAKSA_DENEME}}` | 1–5 arası, örn. `3` |

### §3.6 Zaman Aşımı · Yeniden Deneme · Rate Limit

| Parametre | Önerilen | Aşımında | Neden |
|---|---|---|---|
| Zaman aşımı (tek çağrı) | 30–60 sn | çağrı iptal + `YenidenDene=false` | UI donması |
| Toplam bütçe (retry dahil) | ≤ 2× timeout | üst sınır | kuyruk büyümesi |
| Deneme sayısı (`MAKSA_DENEME`) | 3 | `ProviderResult.Hata` | sağ dayanıklılığı |
| Backoff | üstel: 1s → 2s → 4s (+rastgele jitter) | sabit gecikme **yasak** | senkron çarpışma |
| Yeniden denenen kodlar | 408, 429, 500, 502, 503, 504 | 4xx (400/401/403/404) **denenmez** | anlamsız tekrar |
| 429 özel kuralı | `Retry-After` varsa onu kullan | sabit bekle | sağlayıcı kuralı |
| Rate limit (dakika) | sağlayıcı tavanının ≤ %80'i | bekleyerek değil, **erişim reddi** | UI'ın kendi kuyruğu |
| Paralellik | adaptör başına sabit (ör. 2) | sınırsız paralel **yasak** | IP yasağı |

### §3.7 Hata Eşleme Tablosu (durum kodu → sonuç)

| HTTP | Anlam | `ProviderResult` | Tekrar? | Kullanıcıya |
|---|---|---|---|---|
| 200 | Tamam | `Ok(yanıt)` | Hayır | yanıt metni |
| 400 | İstek bozuk | `Hata(BadRequest)` | Hayır | "İstek biçimi hatalı" |
| 401 | Anahtar geçersiz | `Hata(Auth)` | Hayır | "API key reddedildi (§3.5)" |
| 403 | Yetki/allowlist | `Hata(Forbidden)` | Hayır | "Bu model için yetkiniz yok" |
| 404 | Model/endpoint yok | `Hata(NotFound)` | Hayır | "Model adını kontrol edin (§3.8)" |
| 408 | Sunucu zaman aşımı | `YenidenDene` | Evet | bekleme göstergesi |
| 429 | Hız sınırı | `YenidenDene(Retry-After)` | Evet | "Hız sınırı — saniyeler içinde tekrar" |
| 500/502/503/504 | Sağlayıcı arızası | `YenidenDene` (3 deneme) | Evet | "Sağlayıcı geçici olarak yok" |
| Ağ hatası (DNS/TLS/timeout) | Bağlantı yok | `YenidenDene` → sonra `Hata(Connectivity)` | Evet → Hayır | "Bağlantı kurulamadı" |
| İptal (`OperationCanceled`) | kullanıcı iptal | `İptal` | Hayır | sessiz kapanır |

> Kural: **ham durum kodu / sağlayıcı gövdesi asla kullanıcıya gitmez** (G8); eşlemeyi `ProviderErrorMapper` yapar, adaptör değil.

### §3.8 Model Sabitleme · Loglama · Test Stratejisi

| Konu | Kural | Denetim |
|---|---|---|
| Model sabitleme | Sürümlü/tarihli ad zorunlu (`{{MODEL_SABIT}}`) | `Select-String 'latest|:v[0-9]+$'` = 0 |
| Yükseltme | Sürüm değişikliği = karar → `ADR` + §6.3 | `log.md` satırı |
| Log seviyesi | Başarı = `Debug`; hata = `Warning`; secret = **hiç** | `SecretMasker` zorunlu |
| Maskelenecek | `api[_-]?key`, `Authorization: Bearer`, uzun hex/token, e-posta (kısmi) | G8 |
| Maske biçimi | yalnız ilk 4 karakter + `…***` | `sk-abc…***` |
| Sahte provider | `FakeLlmProvider` sabit yanıt/durum kodu üretir | ağ çağrısı 0 |
| Sözleşme testi | gerçek adaptör `HttpMessageHandler` sahtesiyle test edilir | CI'da internet yok |
| Kapsam hedefi | provider katmanı ≥ %90 (vault kuralı G11) | coverage raporu |

### §3.9 Dolu Örnek + Tam Kalıp

Dolu örnek — `OpenAiProvider` için hata eşlemesi kararı (üretim standardı):

```text
ProviderResult eşlemesi (ProviderErrorMapper)
  200        → Ok(ChatResponse)
  401        → Hata(Auth)         tekrar: hayır  → "API key reddedildi"
  429        → YenidenDene        tekrar: evet   → Retry-After öncelikli
  5xx        → YenidenDene(3)     sonra Hata    → "Sağlayıcı geçici arıza"
  ağ/hata    → YenidenDene(3)     sonra Hata    → "Bağlantı kurulamadı"
  iptal      → İptal              tekrar: hayır  → sessiz
```

Yeni sağlayıcı üretirken kopyalanacak kalıp (yalnız `{{…}}` doldurulur):

````markdown
### {{SAGLAYICI_ADI}} Adaptörü — Üretim Kaydı

| Alan | Değer |
|---|---|
| Sağlayıcı adı | `{{SAGLAYICI_ADI}}` |
| Dosya | `src/VersaCoder.Infrastructure/Providers/{{SINIF_ADI}}.cs` |
| Arayüz | `ILlmProvider` (Abstractions) |
| Endpoint | `{{HTTPS_ENDPOINT}}` (TLS zorunlu, §4.5 G2) |
| Secret | env `{{PROVIDER_SECRET_ENV}}` — açık metin YASAK (§3.5) |
| Model | `{{MODEL_SABIT}}` (yüzen etiket YASAK) |
| Timeout | `{{ZAMAN_ASIMI_SN}}` sn · Deneme `{{MAKSA_DENEME}}` · backoff üstel+jitter |
| Eşleme | §3.7 tablosu birebir uygulanır |
| Log | `SecretMasker` → maskesiz satır 0 (G8) |
| Test | `FakeLlmProvider` + sahte `HttpMessageHandler` (ağ 0) |
| Doğrulama | §5.3 P1–P6 → hepsi 0/OK |
| Onay | developer → reviewer → security |
````

---

## §4 Kurallar

### §4.1 Uygulama Sırası

| Sıra | İş | Neden önce |
|---|---|---|
| 1 | `ILlmProvider` + `ProviderResult` (Abstractions) | Sözleşme olmadan adaptör yazılamaz |
| 2 | `ProviderOptions` + secret kaynağı (§3.5) | Kod, anahtarsız derlenemez/test edilemez |
| 3 | `SecretMasker` | İlk log satırından önce hazır olmalı |
| 4 | `ProviderErrorMapper` (§3.7) | Deneme mantığı eşlemeye bağlı |
| 5 | Retry + rate limit (§3.6) | Adaptör bunları **çağırır**, içinde tutmaz |
| 6 | Adaptör sınıfı (protokol dönüşümü) | 1–5 bittikten sonra |
| 7 | `ProviderFactory` | Sonra seçim devreye girer |
| 8 | Testler (sahte provider + sahte handler) | Sözleşmeyle birlikte yazılır, sonra değil |
| 9 | §5.3 taramaları + §6.1 checklist | Kapı öncesi son durak |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Süre |
|---|---|---|---|
| 1 | developer | adaptör + factory + eşleme | 40 dk |
| 2 | reviewer | §6.1 checklist (17 madde) | 15 dk |
| 3 | security | §4.5 G1, G2, G8 + §5.3 P1 sonucu | 10 dk |
| 4 | testing | sahte provider sözleşmesi + coverage | 20 dk |
| 5 | master-orchestrator | Quality Report (§5.4) → `log.md` | 5 dk |

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|---|---|---|
| Zaman aşımı | 30–60 sn | Aşım → yalnız gerekçeli ADR ile |
| Deneme | 3 | `1` (kırılgan) veya `>5` (storm) RED |
| Backoff | üstel + jitter | sabit/aritmetik RED (G4) |
| Adaptör sınıfı | 1 dosya / sağlayıcı | İçinde HTTP retry döngüsü = SRP ihlali |
| Satır uzunluğu | ≤ 120 karakter | daha uzun → kısalt |
| Kapsam (test) | ≥ %90 | G11 ihlali |
| Dosya boyutu (şablon) | 480–660 satır / 15–32 KB | §6.1 K13–K14 |

### §4.4 Değişiklik Kontrolü

| Değişiklik | Sürüm Etkisi | Zorunlu Kayıt |
|---|---|---|
| Yeni adaptör (aynı arayüz) | `patch` | §3.9 kayıt satırı |
| Arayüz değişikliği (`ILlmProvider`) | `major` | ADR + tüm adaptörler + testler |
| Eşleme tablosu değişimi | `minor` | §3.7 tablosu + `log.md` |
| Secret kaynağı değişimi | `minor` | security onayı (§4.2/3) |
| Yükseltme | `minor` | §6.3 tablosu |

---

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu | Tespit |
|---|-----------|--------------|--------|
| G1 | API key/secret açık metin (`.cs`, `appsettings.json`, repo, `.ai/`) | **DUR** — anahtar iptal + temizlenir | §5.3 P1 |
| G2 | Sahte/ atlanmış SSL doğrulama (`ServerCertificateCustomValidationCallback = true`, `http://` canlı endpoint) | **DUR** | §5.3 P3 |
| G3 | Prompt/yanıt metni maskesiz loga yazıldı | **DUR** | §5.3 P4 |
| G4 | Yeniden deneme yok ya da sabit gecikmeli (backoff'suz) | **DUR** | §6.1 K9 |
| G5 | Zaman aşımı yok (sınırsız bekleyen çağrı) | **DUR** | §6.1 K8 |
| G6 | Yüzen model etiketi (`latest`, `:v1`) | **DUR** — sabit sürüme çekilir | §5.3 P5 |
| G7 | Durum kodu eşlemesiz genel `Exception` fırlatma | **DUR** | §3.7 denetimi |
| G8 | Sağlayıcı ham gövdesi / anahtar log'a sızdı | **DUR** | §5.3 P1, P4 |
| G9 | Sync-over-async (`.Result`, `.Wait`) | **DUR** — UI donması | gözden geçirme |
| G10 | Sağlayıcı başına ayrı arayüz (`ILlmOpenAi`, `ILlmAnthropic` …) | **DUR** — tek `ILlmProvider` | §6.1 K2 |
| G11 | Testte gerçek ağ çağrısı / sahte provider yok | **DUR** | §6.1 K14 |
| G12 | EF Core referansı (ADR-004) veya `coremusic`/versacoder.ai içeriği kopyası | **DUR** | §7.2 R10 |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|---|---|---|---|---|
| 1 | Sağlayıcı tespiti (§1.3) | ihtiyaç | tür + adaptör adı | developer | 5 dk |
| 2 | Arayüz + sonuç tipi | §3.2 | `ILlmProvider.cs` | developer | 15 dk |
| 3 | Secret kaynağı seçimi | §3.5 | env/user-secrets kaydı | devops-engineer | 10 dk |
| 4 | `SecretMasker` | §3.8 | maskeleme testi | developer | 15 dk |
| 5 | Eşleme tablosu | §3.7 | `ProviderErrorMapper.cs` | developer | 20 dk |
| 6 | Timeout/backoff/limit | §3.6 | politika sınıfı | developer | 25 dk |
| 7 | Adaptör + factory | §3.4 | 2 dosya | developer | 40 dk |
| 8 | Testler (sahte) | §3.8 | ≥ %90 kapsam | testing | 45 dk |
| 9 | Tarama + checklist | §5.3, §6.1 | 0 ihlal | reviewer | 15 dk |
| 10 | Quality Report | §5.4 | `log.md` satırı | orchestrator | 5 dk |
| | **Toplam** | | | | **≈ 195 dk (3,3 sa)** |

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Anahtar repoda | P1 > 0 satır | Anahtar iptal → §3.5 kaynağına taşı |
| 2 | Retry storm | 429'da 10+ çağrı/sn | Üstel backoff + rate limit (§3.6) |
| 3 | UI donması | `.Result` satırı | `await` + `CancellationToken` |
| 4 | Belirsiz hata | kullanıcıda "hata 500" | §3.7 eşlemesine dön |
| 5 | Sessiz model değişimi | `"latest"` | `{{MODEL_SABIT}}` + ADR |
| 6 | Testler ağ bekliyor | CI timeout | `FakeLlmProvider` |
| 7 | Socket tükenmesi | Yeni `HttpClient` her çağrıda | Tekli/`IHttpClientFactory` |
| 8 | Log sızıntısı | maskesiz token | `SecretMasker` zorunlu (G8) |
| 9 | Bilinmeyen sağlayıcı adı | sessiz null dönüş | fail-fast sonucu (§3.4) |

### §5.3 Salt Okunur Doğrulama Komutları

> Aşağıdaki 6 komut **hiçbir dosyayı değiştirmez**. P1–P5 hedefleri (kod/uygulama katmanı) henüz diskte yok → beklenen çıktı **0 satır**'dır; dosyalar geldikçe aynı komutlar kanıt üretir.

```powershell
# P1) SECRET SIZINTISI — çıktı 0 satır OLMALI (G1, G8)
Select-String -Path 'C:\versacoder\src\**\*.cs','C:\versacoder\**\appsettings*.json' `
  -Pattern 'sk-[A-Za-z0-9]{16,}|(?i)(api[_-]?key|password|secret|token)"?\s*[:=]\s*"[^"$]{8,}' `
  -ErrorAction SilentlyContinue | Select-Object Path, LineNumber, Line
```

```powershell
# P2) ARAYÜZ SÖZLEŞMESİ — her adaptör 'ILlmProvider' implement etmeli
Select-String -Path 'C:\versacoder\src\VersaCoder.Infrastructure\Providers\*.cs' `
  -Pattern 'class\s+\w+Provider\s*:\s*ILlmProvider' -ErrorAction SilentlyContinue |
  Select-Object Filename, LineNumber
```

```powershell
# P3) TLS / SAHTE SSL — 'http://' canlı endpoint veya sertifika kapatma 0 olmalı (G2)
Select-String -Path 'C:\versacoder\src\**\*.cs','C:\versacoder\**\appsettings*.json' `
  -Pattern 'http://(?!localhost|127\.0\.0\.1)|ServerCertificateCustomValidationCallback\s*=\s*.*true' `
  -ErrorAction SilentlyContinue | Select-Object Path, LineNumber, Line
```

```powershell
# P4) MASKELEME — loglayan dosyalarda maskesiz anahtar deseni 0 olmalı (G8)
Select-String -Path 'C:\versacoder\src\**\*.cs' -Pattern 'Log|Logger|Trace' `
  -ErrorAction SilentlyContinue | Select-String -Pattern '"sk-[A-Za-z0-9]{8,}' | Measure-Object
```

```powershell
# P5) MODEL SABİTLEME — yüzen etiket 0 olmalı (G6)
Select-String -Path 'C:\versacoder\src\**\*.cs','C:\versacoder\**\appsettings*.json' `
  -Pattern '(?i)model["\s:=]+["'']?(latest|v\d+$/|:v\d+["''])' -ErrorAction SilentlyContinue |
  Select-Object Path, LineNumber, Line
```

```powershell
# P6) ÖLÇÜM — bu dosya: satır 480-660, KB 15-32, mojibake 0
$f = 'C:\versacoder\.ai\.templates\infrastructure\provider-template.md'
"satu: {0} (hedef 480-660)" -f (Get-Content -LiteralPath $f).Count
"KB:   {0} (hedef 15-32)"    -f [math]::Round((Get-Item -LiteralPath $f).Length / 1KB, 1)
#    Gerçek mojibake 0 olmalı; bu dosyadaki kasıtlı tarama satırları (P6'nın kendi
#    deseni ve §6.2 red satırı) hariç tutulur — ikisi de bilinçli örnek metindir.
"mojibake: {0} (hedef 0)"    -f (Select-String -LiteralPath $f -Pattern 'Ã|Â|\ufffd' |
  Where-Object { $_.Line -notmatch 'Select-String -LiteralPath \$f|Mojibake \(' } |
  Measure-Object).Count
```

**Geçme kriteri:** P1, P3, P4, P5 = 0 satır · P2 ≥ 1 satır (kod varsa) · P6 üçü de aralıkta.

### §5.4 Quality Report

```markdown
## Provider Adaptörü Kalite Raporu — YYYY-MM-DD

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| Adaptör dosyası (`Providers/*.cs`) | N | ≥1 | 🟢/🔴 |
| P1 secret sızıntısı | N | 0 | 🟢/🔴 |
| P2 `ILlmProvider` uygulayan | N | ≥ N adaptör | 🟢/🔴 |
| P3 sahte SSL / http endpoint | N | 0 | 🟢/🔴 |
| P5 yüzen model etiketi | N | 0 | 🟢/🔴 |
| Kapsam (provider katmanı) | %N | ≥ %90 | 🟢/🔴 |
| §4.5 G1..G12 ihlali | N | 0 | 🟢/🔴 |

**Karar:** 🟢 `log.md`'ye düşüldü / 🔴 düzeltmeye gönderildi
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi

- [ ] K1 Frontmatter 7 alan (`title, type, category, status, version, updated, authority: SSOT`)
- [ ] K2 H1 + `**Zorunlu Bağlantılar:**` 6 bağlantı
- [ ] K3 §1.1–§1.4 dolu; §1.2 disk kanıtlı + `⚠️` işaretli
- [ ] K4 §2.1–§2.2 dolu
- [ ] K5 §3.1–§3.9 arası eksiksiz; §3.9 `{{…}}` kalıbı ````markdown ```` içinde
- [ ] K6 Her §'de en az bir tablo; toplam ≥ 25 tablo satırı
- [ ] K7 §3.5 secret yasak listesi mevcut (6–8 satır)
- [ ] K8 §3.6 timeout/backoff/rate limit değerleri yazılı
- [ ] K9 §3.7 eşleme tablosu ≥ 9 satır
- [ ] K10 §4.5 = 12 guardrail (≥10)
- [ ] K11 §5.3 ≥ 5 salt-okunur PowerShell (6 komut)
- [ ] K12 §5.4 Quality Report bloğu
- [ ] K13 Satır 480–660
- [ ] K14 KB 15–32
- [ ] K15 §6.2 red sinyalleri + §6.5 envanter kontrolleri
- [ ] K16 §7.1 referans + §7.2 Red Team ≥ 10 satır
- [ ] K17 Mojibake = 0 (§5.3 P6); `{{…}}` yalnız §3.5/§3.9 kalıp bloklarında

### §6.2 Red Sinyalleri (Otomatik DUR)

| Sinyal | Aksiyon |
|---|---|
| P1 çıktısı > 0 satır | **DUR** — anahtar iptal edilir, geçmiş loglar temizlenir |
| `sk-` benzeri desen `.cs` içinde | **DUR** — §3.5 kaynağına taşınır |
| `ILlmProvider`'sız adaptör | **DUR** — §3.2'ye çekilir |
| `http://` canlı endpoint / sertifika kapatma | **DUR** — TLS (G2) |
| `throw new Exception(` adaptörde | **DUR** — §3.7 eşlemesi |
| `.Result` / `.Wait()` | **DUR** — `await` (G9) |
| `{{…}}` kalıp bloğu dışında | **DUR** — doldur |
| Satır < 480 veya > 660 | **DUR** — §4.3 ölçeği |
| Mojibake (`Ã`, `Â`, `?`) | **DUR** — UTF-8 BOM'suz yeniden yaz |

### §6.3 Sürüm / Yükseltme Geçiş Kuralları

| Değişiklik | Etki | Zorunlu |
|---|---|---|
| Yeni adaptör, aynı arayüz | `patch` | §3.9 + test |
| Model sürümü yükseltme | `minor` | §6.5 K3 + `log.md` |
| Arayüz (`ILlmProvider`) değişimi | `major` | ADR + tüm adaptörler |
| Secret kaynağı değişimi | `minor` | security onayı |
| Deneme/backoff değişimi | `minor` | §3.6 tablosu + yeniden test |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|---|---|
| `.ai/.templates/infrastructure/provider-template.md` | **bu dosya** (adaptör sözleşmesi SSOT) |
| `.ai/.templates/infrastructure/configuration-template.md` | secret'ın okunduğu yapılandırma katmanı |
| `.ai/.templates/index.md` | envanter `#23` (Part B, `T-VER`) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | görsel anlatım standardı |
| `.ai/.templates/adr/adr-architecture-template.md` | katman/bağımlılık kuralı (yapı referansı) |
| `.ai/.templates/query/Query-Template.md` | veri erişimi ayrı (ADR-004) |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Komut |
|---|---|---|---|
| K1 | `infrastructure/` klasöründe 2 şablon | 2 | `Get-ChildItem …\infrastructure -Filter *.md` |
| K2 | Envanter satırı `#23` hedefi | `Providers/*.cs` | index.md §3.3 |
| K3 | `{{MODEL_SABIT}}` yükselebilir mi (sürüm kilidi yazıldı mı) | evet/ADRsiz hayır | §6.3 |
| K4 | EF Core geçen satır | 0 | `Select-String -Pattern 'EntityFramework'` |
| K5 | `coremusic` geçen satır | 0 | `Select-String -Pattern 'coremusic'` |
| K6 | `net48`/`net10.0-windows` yazımı tutarlı | 2/2 | gözden geçirme |
| K7 | `{{…}}` kalıp bloğu dışında | 0 | §5.3 P6 |
| K8 | Guardrail adedi | 12 | §4.5 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| # | Kaynak | Yol | Ne İçin | Durum |
|---|---|---|---|---|
| 1 | Şablon envanteri | `.ai/.templates/index.md` | `#23` kaydı, Part B | ✅ OKUNDU |
| 2 | Dizin kuralı | `.ai/.templates/CLAUDE.md` | erişim sırası | ✅ OKUNDU |
| 3 | ASCII/Unicode standardı | `.ai/.templates/architecture/ascii-diagram-template.md` | görsel/kod bloğu biçimi | ✅ OKUNDU |
| 4 | ADR yapı referansı | `.ai/.templates/adr/adr-architecture-template.md` | §1–§7 iskeleti (yalnız yapı) | ✅ OKUNDU |
| 5 | Bu şablonun ikizi | `.ai/.templates/infrastructure/configuration-template.md` | secret okuma önceliği | ✅ BU İŞ |
| 6 | Altın standart ADR | `.ai/.templates/adr/adr-template.md` | derinlik referansı | ❌ YOK (§1.2 E5) |
| 7 | Kurallar / keys / log | `.ai/.rules/`, `.ai/keys.md`, `.ai/log.md` | kural + kayıt | ❌ YOK (§1.2 E8) |
| 8 | Kod hedefi | `src/VersaCoder.Infrastructure/Providers/*.cs` | adaptörler | ❌ YOK (§1.2 E10) |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Saldırı/Kırılma | Yakalama | Doğrusu |
|---|---|---|---|---|
| 1 | API key `.cs` içine yazmak | Repo sızıntısı → hesap hırsızlığı | §5.3 P1 | env / user-secrets / DPAPI (§3.5) |
| 2 | `keys.md`'ye tam anahtar koymak | Vault = ikinci sızıntı noktası | P1 + gözden geçirme | yalnız ad + uç nokta |
| 3 | SSL doğrulamasını kapatmak | MITM → prompt + yanıt çalınır | §5.3 P3 | TLS zorunlu (G2) |
| 4 | Prompt enjeksiyonuna karşı tek savunma = model | Dış kaynaklı talimatla araç çağrısı | test senaryosu | yanıt doğrulama + rol kısıtı (test §3.8) |
| 5 | Ham sağlayıcı hatasını kullanıcıya göstermek | İstek/anahtar bilgisi sızar | G8 | §3.7 eşlemesi |
| 6 | Sonsuz/sabit yeniden deneme | 429 storm → kalıcı IP yasağı | §6.1 K9 | 3 deneme + üstel backoff |
| 7 | `"latest"` model etiketi | Sessiz davranış değişimi | §5.3 P5 | sürümlü `{{MODEL_SABIT}}` |
| 8 | `.Result` / `.Wait()` | UI deadlock | gözden geçirme | `await` + `CancellationToken` |
| 9 | Her çağrıda yeni `HttpClient` | Socket exhaustion | §5.2/7 | tekli/factory |
| 10 | Sağlayıcı başına ayrı arayüz | Bakım 2–3 katı | §6.1 K2 | tek `ILlmProvider` |
| 11 | Testte gerçek API çağrısı | CI maliyeti + kırılganlık | §6.1 K14 | `FakeLlmProvider` + sahte handler |
| 12 | EF Core ile adaptör konfigürasyonu | ADR-004 ihlali | §6.5 K4 | ADO.NET + Dapper, config ayrı şablon |

**SSUT beyanı:** Bu dosya harici sağlayıcı adaptörünün **tek** kaynağıdır; kopyalayan her dosya yalnız bu dosyaya bağlanır.
