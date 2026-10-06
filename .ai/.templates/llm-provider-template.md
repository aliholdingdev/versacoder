---
title: "LLM Provider Sablonu (OpenRouter / NVIDIA NIM - OpenAI uyumlu)"
type: template
category: templates
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "OpenAI uyumlu harici LLM saglayici (OpenRouter, NVIDIA NIM) tanimlama sablonu: base URL, auth, model slug, ortam degiskeni kurallari"
---

# LLM Provider Sablonu

**Zorunlu Baglantilar:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [platform-arastirma](../reports/12-platform-arastirmasi.md) · [new-doc-contract](new-doc-contract.md)

> **Tek cumlelik sorumluluk:** *OpenAI uyumlu bir LLM saglayicisi (OpenRouter /
> NIM) nasil tanimlanir, hangi alanlar zorunludur, sirlar nereye konur* sorusunun
> web kanitli tek cevabidir.

---

## §0 Ozet (Answer First)

- **Tek degisen 3 alan:** `base_url` + `api_key` (ortam degiskeni) + `model` slug.
- **OpenRouter:** `https://openrouter.ai/api/v1` · anahtar `sk-or-` ile baslar ·
  slug `saglayici/model` formunda (`anthropic/claude-sonnet-4`).
- **NVIDIA NIM:** `https://integrate.api.nvidia.com/v1` · `Authorization: Bearer $NVIDIA_API_KEY`
  · tüm uçlar OpenAI Chat Completions uyumlu (ek olarak `/v1/messages` Anthropic-uyumlu).
- **Anahtar asla MD'ye, src/**'ye, commit'e yazilmaz** (G12 / guvenlik); yalniz
  ortam degiskeni veya `opencode.json` disi gizli depo.

---

## §1 Saglayici tablosu

| # | Saglayici | Base URL | Auth | Model slug ornegi |
|---|---|---|---|---|
| 1 | OpenRouter | `https://openrouter.ai/api/v1` | `Bearer $OPENROUTER_API_KEY` (`sk-or-...`) | `anthropic/claude-sonnet-4` |
| 2 | NVIDIA NIM | `https://integrate.api.nvidia.com/v1` | `Bearer $NVIDIA_API_KEY` | `nvidia/llama-3.1-70b-instruct` |

> `WEB_VERIFIED — openrouter.ai/docs/api/reference/authentication + build.nvidia.com/llms.txt (2026-10-04)`

## §2 YAML iskeleti (provider tanimi)

```yaml
provider:
  <ad>:
    npm: <istek-paketi>          # openai uyumlu istemci
    options:
      base_url: <§1-tablosundaki-url>
      api_key: <ORTAM_DEGISKENI_ADI>   # degeri DEGIL, degisken adi
    models:
      <ad>:
        <slug>: true             # §1 model slug formu
```

| # | Kural | Neden |
|---|---|---|
| 1 | `api_key` = ortam degiskeni **adi** | sızıntı riski yok (G12) |
| 2 | `base_url` birebir §1'deki | yanlış uç = 404/timeout |
| 3 | slug `saglayici/model` | OpenRouter kataloğu (`openrouter.ai/models`) |
| 4 | 300+ model / 70+ sağlayıcı tek uçta | OpenRouter unified schema |

## §3 Bileşen testi (kanit)

```text
POST <base_url>/chat/completions
Authorization: Bearer $<ANAHTAR>
{"model":"<slug>","messages":[{"role":"user","content":"ping"}]}
→ HTTP 200 + choices[0].message icermelidir
```

## §4 RED sinyalleri

| # | Sinyal | Düzeltme |
|---|---|---|
| 1 | Anahtar MD/commit içinde | sil + anahtarı yenile (G7: onayla) |
| 2 | `sk-...` (OpenAI) anahtarı OpenRouter'da | `sk-or-` ile değiştir |
| 3 | §1'deki base URL'den farklı uç | §1 tablosundaki birebir değeri kullan |
| 4 | Model slug'sız / sadece marka adı | `saglayici/model` slug'ı |
| 5 | Sağlayıcı provasısız "çalışıyor" | §3 HTTP 200 kanıtı yoksa `DOĞRULAMA GEREKLI` |

---

## §5 Sağlayıcı Sözleşmesi (4 Sınıf + §1 Tablosunun Açılımı)

§1'deki iki satır **değiştirilmez**; bu tablo sözleşme sınıfını genişletir. "Doğrulama"
sütunu, o uç için eldeki kanıtın durumunu söyler — `VARSAYILAN` satırı ilk kullanımda
§9 ile **sınanmak zorundadır**, yazıyla "çalışıyor" denmez (G9).

| # | Sınıf | Sağlayıcı | Base URL | Auth | Slug ornegi | Doğrulama |
|---|---|---|---|---|---|---|
| 1 | Bulut, uyumlu | OpenRouter | `https://openrouter.ai/api/v1` | `Bearer $OPENROUTER_API_KEY` | `anthropic/claude-sonnet-4` | WEB_VERIFIED (§1) |
| 2 | Bulut, uyumlu | NVIDIA NIM | `https://integrate.api.nvidia.com/v1` | `Bearer $NVIDIA_API_KEY` | `nvidia/llama-3.1-70b-instruct` | WEB_VERIFIED (§1) |
| 3 | Bulut, ana sağlayıcı | OpenAI | `https://api.openai.com/v1` | `Bearer $OPENAI_API_KEY` | `gpt-4.1-mini` (kataloğdan) | VARSAYILAN — §9 ile doğrula |
| 4 | Bulut, kiracıya özel | Azure OpenAI | `https://<kaynak>.openai.azure.com/` | `api-key: $AZURE_OPENAI_KEY` + `api-version` sorgusu | deployment adı | VARSAYILAN — §9 + sürüm sorgusu |
| 5 | Yerel, açık kapı | Ollama | `http://localhost:11434/v1` | başlık yok ya da sahte Bearer | yerel model etiketi | VARSAYILAN — yerelde §9 |
| 6 | Yerel, açık kapı | LM Studio / yerel sunucu | `http://localhost:1234/v1` | başlık yok ya da sahte Bearer | yerel etiket | VARSAYILAN — yerelde §9 |

### §5.1 Sınıf Bazlı Farklar

| # | Fark | OpenAI-uyumlu (1–3) | Azure (4) | Yerel (5–6) |
|---|---|---|---|---|
| 1 | Uç yolu | tek `base_url`, `/chat/completions` eklenir | `deployment` + `api-version` **zorunlu** | `/v1` açıksa OpenAI-uyumlu |
| 2 | Kimlik | `Authorization: Bearer` | `api-key` başlığı ya da Azure AD token | çoğu zaman **gerekmez** |
| 3 | TLS | zorunlu (https) | zorunlu | http yeterli (yalnız loopback) |
| 4 | Slug | `saglayici/model` (§1) | deployment adı = slug | `ollama list` çıktısındaki etiket |
| 5 | Sınama | §9 T1–T6 | T1 + `api-version` hatası kontrolü | T1 + T4 (streaming) |

### §5.2 YAML Örnekleri (§2 İskeletinin Doldurulmuş Hâli)

```yaml
# sınıf 3 — OpenAI
provider:
  openai:
    npm: "@ai-sdk/openai"
    options:
      base_url: https://api.openai.com/v1
      api_key: OPENAI_API_KEY        # degeri DEGIL, ortam degiskeni adi
    models:
      gpt: <katalogdaki-slug>
```

```yaml
# sınıf 5 — Ollama (yerel)
provider:
  ollama:
    npm: "@ai-sdk/openai-compatible"
    options:
      base_url: http://localhost:11434/v1
      api_key: OLLAMA_API_KEY        # deger bos birakilabilir; ad yazar
    models:
      yerel: <ollama-list-etiketi>
```

```yaml
# sınıf 4 — Azure (fark: api-version sorgusu ve deployment slug)
provider:
  azure:
    npm: "@ai-sdk/azure"
    options:
      base_url: https://<kaynak>.openai.azure.com/
      api_key: AZURE_OPENAI_KEY
      api-version: <desteklenen-surum>
    models:
      <deployment-adi>: true
```

### §5.3 Ortam Değişkeni Kuralları

| # | Kural | Doğru | Yanlış |
|---|---|---|---|
| 1 | Dosyada **ad** yazar, değer **asla** | `api_key: OPENAI_API_KEY` | `api_key: sk-tgizli...` |
| 2 | Değer yalnız ortam değişkeni ya da git-dışı depoda | `setx` / CI secret | `opencode.json` içi, commit içi |
| 3 | Her sağlayıcı kendi adı | `OPENROUTER_API_KEY` · `NVIDIA_API_KEY` | tek anahtar her yerde |
| 4 | Prefix doğru sağlayıcıda | OpenRouter → `sk-or-` | OpenAI `sk-` anahtarı OpenRouter'da (§4/2) |
| 5 | Sızarsa: **sil + yenile** | anahtar rotasyonu + `log.md` notu | yalnız dosyayı silmek ( geçmişte kalmıştır ) |

---

## §6 `IChatClient` ve Streaming Sözleşmesi

Üretim kodu (`.NET`) sağlayıcıya **doğrudan** değil, arayüz üzerinden bağlanır: bu da bu
şablonun kod tarafıdır. Arayüz `Microsoft.Extensions.AI.Abstractions` paketindedir
(`netstandard2.0` → `net48` ve `net10.0-windows` ikili bacakta da derlenir).

### §6.1 Zorunlu İmzalar

```csharp
using Microsoft.Extensions.AI;

public interface IChatClient : IDisposable
{
    Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);

    TService? GetService<TService>(object? key = null) where TService : class;
}
```

| # | Üye | Sözleşme | İhlali |
|---|---|---|---|
| 1 | `GetResponseAsync` | tek `ChatResponse` döner; `CancellationToken` taşır | iptal edilemeyen çağrı = G11 riski |
| 2 | `GetStreamingResponseAsync` | `IAsyncEnumerable<ChatResponseUpdate>` — parça parça akar | parçalar `await` ile tek seferde beklenmez (P6) |
| 3 | `ChatOptions.ModelId` | §1/§5 slug'ı | marka adı slug değildir (§4/4) |
| 4 | `GetService<T>` | taşıyıcı özellik erişimi (ör. örnekleme ayarları) | yoksa sessizce varsayılara düşer |
| 5 | `Dispose` | istemci tek example'dan yönetilir | her istekte yeni istemci = bağlantı şişmesi |

### §6.2 Kullanım Kalıbı (senkron + akış)

```csharp
using Microsoft.Extensions.AI;

var client = sp.GetRequiredService<IChatClient>();
var msgs = new[] { new ChatMessage(ChatRole.User, "ping") };
var opts = new ChatOptions { ModelId = "nvidia/llama-3.1-70b-instruct" };

// 1) tek seferlik
ChatResponse yanit = await client.GetResponseAsync(msgs, opts, ct);
string metin = yanit.Text;

// 2) akisli — her guncelleme gelir gelmez islenir
await foreach (ChatResponseUpdate u in client.GetStreamingResponseAsync(msgs, opts, ct))
    Console.Write(u.Text);

// 3) kullanim sayaclari (kesin token sayisi buradan gelir)
var usage = yanit.Usage;   // girdi / cikti / toplam token alanlari
```

| # | Kural | Neden |
|---|---|---|
| 1 | Akış döngüsü `ct` ile biter | kullanıcı iptali 5 sn gecikmez |
| 2 | Kısmi akış **sayılır**, fakat "tamam" sayılmaz | kesik akış = `❌`, yeniden denenir (§7) |
| 3 | `usage` okunmadan bütçe iddiası yazılmaz | G9 — kanıtsız sayı |
| 4 | `net48` akışında `Microsoft.Bcl.AsyncInterfaces` gerekir | `IAsyncEnumerable` eski TFM'de paket ister |
| 5 | İstemci DI'da tek örnek (singleton/scoped) | her çağrıda `new` = socket sızıntısı |

> **Alan adı notu:** `usage` sayaçlarının tam adları paket sürümüne göre değişebilir; derleme
> hatası verirse paket dokümantasyonundan doğrulanır — tahminle **yazılmaz** (G9).

---

## §7 Hata Sınıflandırma ve Retry Politikası

### §7.1 HTTP Durum → Sınıf → Aksiyon

| # | Durum | Sınıf | Retry | Aksiyon |
|---|---|---|---|---|
| 1 | `400` istek biçimi | kalıcı | hayır | isteği/şemayı düzelt |
| 2 | `401` geçersiz anahtar | kalıcı | hayır | ortam değişkenini kontrol et; sızdıysa anahtar **yenile** (P1) |
| 3 | `403` erişim/plan | kalıcı | hayır | kaynak/limit kontrolü |
| 4 | `404` model/deployment yok | kalıcı | hayır | slug'ı §1/§5'ten al |
| 5 | `408` zaman aşımı | geçici | evet | §7.2 backoff, en fazla 3 |
| 6 | `429` hız limiti | geçici | evet | `Retry-After` başlığına uy, sonra backoff |
| 7 | `500/502/503/504` sunucu | geçici | evet | en fazla 3 deneme, sonra `❌` |
| 8 | akış ortasında kopma | geçici | evet | yeniden dene; kısmi çıktıyı **tamam sayma** |

### §7.2 Backoff ve Zaman Aşımı

| # | Parametre | Önerilen | Neden |
|---|---|---|---|
| 1 | Formül | `min(taban * 2^deneme, tavan) + rastgele` | üstel büyüme + jitter = aynı anda taşmayı kırar |
| 2 | Taban / tavan | 1 sn / 30 sn | 429'da daha uzun, 5xx'te daha kısa |
| 3 | En fazla deneme | 3 (toplam 4 istek) | sonsuz döngü bütçeyi bitirir |
| 4 | Bağlantı zaman aşımı | ~10 sn | ölü uçta kilitlenmeyi önler |
| 5 | İstek zaman aşımı | ~60 sn (akışta: akış bitene kadar) | akış çağrıları normalden uzun sürer |

- 429'da `Retry-After` **varsa** o saniye beklenir; backoff ondan **büyükse** bile zincire
  uyulmazsa sağlayıcı hesabı geçici askıya alabilir (P7).
- Retry yalnız **geçici** sınıf için yapılır; kalıcı sınıfın tekrarı isteği bozar (400'ü 4 kez
  göndermek sorunu çözmez, kota yakar).

---

## §8 Token Sayma

| # | Kaynak | Ne verir | Güven |
|---|---|---|---|
| 1 | Yanıttaki `usage` sayaçları | kesin girdi / çıktı / toplam token | **kanıt** — tek geçerli sayı |
| 2 | Akış yanıtları | sayaç genelde **son** güncellemede gelir | sona kadar okunmadan sayı yazılmaz |
| 3 | Karakterden tahmin | kaba üst sınır (karakter ÷ 4 dâhil) | **tahmin** — Türkçe için güvenilmez, `NEEDS_VALIDATION` |
| 4 | Yerel modeller | sayaç sunucuya göre değişir | §9 T6 ile sınanır |
| 5 | Oturum bütçesi | anahtar başına harcama takibi | toplam = Σ(usage toplam) |

| # | Kural | Uygulama |
|---|---|---|
| 1 | Bütçe iddiası yalnız `usage` ile | "≈ X token" yazımı yasak, kesin sayı yazılır |
| 2 | Her çağrı tek satır kaydedilir | `model · girdi · cikti · toplam` — toplanabilir |
| 3 | Retry'lar **sayılmaz** | aynı isteğin 4 denemesi 1 kez sayılır |
| 4 | Kısmi akış sayılmaz | `❌` çağrıdan dönen token harcama değildir |
| 5 | Log dosyası sır içermez | yalnız sayılar; anahtar **asla** (P1) |

---

## §9 Test Kimliği (Kimlik Kanıtları)

§3 tek bir HTTP 200 kanıtı verir; **kimlik** bunun *hangi* sağlayıcıdan geldiğini de kanıtlar.
Altı test birlikte çalışır — tek başına 200 yetmez (§4/5).

| # | Test | İşlem | Beklenen | Ne kanıtlar |
|---|---|---|---|---|
| T1 | Uç + anahtar | §3 `ping` | HTTP 200 + `choices[0].message` | uç canlı |
| T2 | Anahtar reddi | bilinçli **yanlış** anahtarla §3 | HTTP 401/403 | 200'ün anahtarla geldiği (sahiplik) |
| T3 | Slug bağlaması | bilinmeyen model adı | HTTP 400/404 | slug'ın gerçekten sağlayıcıya gittiği |
| T4 | Akış | §6.2 blok 2 | en az 1 delta + kapanış güncellemesi | streaming hattı |
| T5 | İptal | 2. saniyede `ct` iptali | `OperationCanceledException` | iptal yolu (P6/§6.2) |
| T6 | Sayaç | T1 yanıtındaki `usage` | toplam ≈ girdi + çıktı | §8 kanıt zinciri |

```powershell
# T1 kaniti — yaniti dosyaya yaz, HTTP durumu + model adini goruntule
$hub = 'https://openrouter.ai/api/v1/chat/completions'
$govde = @{ model = 'anthropic/claude-sonnet-4'
            messages = @(@{ role = 'user'; content = 'ping' }) } | ConvertTo-Json -Depth 5
$yanit = Invoke-RestMethod -Uri $hub -Method Post -TimeoutSec 30 `
  -Headers @{ Authorization = "Bearer $env:OPENROUTER_API_KEY" } -Body $govde -ContentType 'application/json'
'durum OK · model {0} · yanit {1}' -f $yanit.model, $yanit.choices[0].message.content
# beklenti: model slug'i birebir, yanit bos DEGIL
```

> Kimlik testi **canlı uçta** ve **anahtarla** çalışır; sonuç komut çıktısı olarak
> Quality Report'a yapıştırılır. Çıktı yoksa §4/5 gereği `DOĞRULAMA GEREKLI` kalır.

---

## §10 Hata Modları (P1–P11)

| # | Hata modu | Belirti | Kök neden | Düzeltme |
|---|---|---|---|---|
| P1 | Anahtar MD/commit içinde | `sk-` geçen metin satırı | G12 ihlali | sil + anahtarı **yenile** + log |
| P2 | Prefix yanlış sağlayıcıda | OpenRouter'da OpenAI anahtarı | kopya-yapıştır | `sk-or-` anahtarıyla değiştir |
| P3 | Uç yanlış | 404/timeout, yol iki kez birleşmiş | `base_url`'e `/chat/completions` eklenmiş | §5 tablosundaki **kök** URL |
| P4 | Slug yok / marka adı | `404 model not found` | slug kuralı (§1/4) | `saglayici/model` ya da deployment adı |
| P5 | Dosyada anahtar değeri | YAML'de gerçek anahtar | ortam değişkeni **adı** yazılmamış | §5.3 kural 1 |
| P6 | Akışta kilit | UI donar, çağrı bitmez | akış `await` ile tek parça beklenmiş | §6.2 blok 2 |
| P7 | Retry'da 429 ihlali | üst üste 429 → hesap askı | `Retry-After` yok sayıldı | §7.2 kural |
| P8 | Kanıtsız token bütçesi | "≈ 50.000" komut çıktısız | G9 | `usage` kaydı ekle |
| P9 | `net48` akış derlenmiyor | `IAsyncEnumerable` tipi bulunamadı | Bcl paketi eksik | §6.2 kural 4 |
| P10 | Azure 404/400 | `api-version` hatalı | sürüm sorgusu eksik | §5.1 satır 1 |
| P11 | Provasız "çalışıyor" | §9 tablosu boş | kimlik testi koşulmamış | T1–T6, yoksa `DOĞRULAMA GEREKLI` |

---

## §11 G1..G12 Eşlemesi (Sağlayıcı → Guardrail)

| ID | Guardrail (tek satır) | Bu şablondaki karşılığı | Tespit |
|---|---|---|---|
| G1 | Harici vault'tan kopya yasak | §1/§5 satırları kaynak dokümden (WEB_VERIFIED notu) | kopya cümle yok |
| G2 | EF Core yasak; veri ADO.NET + Dapper | LLM çıktısı **hiçbir** veri katmanına bağlanmaz | `EntityFramework` = 0 |
| G3 | Frontmatter'sız Markdown yasak | bu dosyanın 8 alanlı başlığı | §6/§9'daki 8 alan kontrolü |
| G4 | Üretilen dosyada yer tutucu 0 | §5.2 YAML kesikli alanları **üretim çıktısında** doldurulur | üretilen JSON'da `<…>` = 0 |
| G5 | Türkçe zorunlu, mojibake yasak | kod blokları dâhil UTF-8 | mojibake sayacı = 0 |
| G6 | < 480 satır şablon red | bu dosya ≥ 480 satır sınıfı | §10/2 benzeri satır sayımı |
| G7 | Bozuk bağlantı yasak | yeni bağlantı yalnız diskte var olan hedef | kırık = 0 |
| G8 | GATE-003 onaysız ekran/kod | bu şablon kod **üretmez** (yalnız sözleşmeyi tarif eder) | kapsam dışı — kapı dokunulmaz |
| G9 | Kanıtsız sayı yazılmaz | token/latency/suçlama iddiası yalnız komut çıktısıyla | §9 T1–T6 · §8 kural 1 |
| G10 | Concat SQL yasak | LLM çıktısındaki SQL string birleştirilerek **çalıştırılmaz** | `@p1..@pN` kalıbı |
| G11 | `TreatWarningsAsErrors` + %90 | §6 kod kalıpları test projesinde sınanır | `dotnet test` çıktısı |
| G12 | SRP + sır tekrarı yok | anahtar yalnız ortam değişkeni (§5.3) | P1 taraması: `sk-` = 0 |

> **Okunuşu:** bu dosya sağlayıcı **sözleşmesini** verir; sağlayıcı **kodu**
> `src/VersaCoder.Infrastructure/` içinde, kanıtı §9'da yaşar — ikisi tek dosyada birleştirilirse
> G12 ihlali doğar (ayrıntı [new-doc-contract](new-doc-contract.md) §7.6).

---

## §12 Entegrasyon Akışı (Sözleşmeden Koda)

| # | Adım | İş | Çıktı | Kanıt |
|---|---|---|---|---|
| 1 | Sınıf seç | §5 satırından sağlayıcı sınıfı (1–6) | base URL + auth biçimi | §5.1 fark tablosu |
| 2 | Sırla | yalnız ortam değişkeninin **adı** yazılr | YAML bloğu (§5.2) | dosyada gerçek anahtar = 0 |
| 3 | Ucu sınıyor | §9 T1–T6 | HTTP durumları + model adı | komut çıktısı |
| 4 | Kodu bağla | `IChatClient` DI kaydı (§6.1) | tek istemci örneği | derleme + §9 T4/T5 |
| 5 | Direnci kur | §7 retry + timeout | zamanlanmış yeniden deneme | 429/5xx simülasyonu (sahte uç) |
| 6 | Sayacı aç | §8 `usage` kaydı | tek satır/kullanım | toplam = Σ usage |
| 7 | Raporla | §13 Quality Report | ONAY / RED | sayacın tamamı |

### §12.1 Ortam Değişkeni Denetimi (değer görünmez)

```powershell
@('OPENROUTER_API_KEY','NVIDIA_API_KEY','OPENAI_API_KEY','AZURE_OPENAI_KEY') |
  ForEach-Object {
    $v = [Environment]::GetEnvironmentVariable($_)
    '{0,-22} {1}' -f $_, $(if ($v) { 'TANIMLI (deger goruntulenmez)' } else { 'YOK' })
  }
# beklenti: secilen saglayicinin degiskeni TANIMLI; deger rapora ASLA yazilmaz (G12)
```

| # | Sıra kuralı | Neden |
|---|---|---|
| 1 | 3 (uç sınaması) olmadan 4 (kod) başlamaz | canlı kanıtsız kod = P11 |
| 2 | 2 adımında değer dosyaya girmez | G12 — sır metni üretilmez |
| 3 | 5, 4'ten sonra kurulur | önce doğru uç, sonra direnç |
| 4 | 6 olmadan 7 yazılmaz | kanıtsız bütçe = P8 |
| 5 | Her adımın çıktısı Quality Report'a | G9 — sayı komut çıktılı |

---

## §13 Doğrulama Checklist'i (K1–K16)

| # | Kontrol | Beklenen | İlgili |
|---|---|---|---|
| K1 | Frontmatter 8 alan + ilk satır `---` | 8/8 · True | G3 · contract §9 |
| K2 | §1 tablosunun 2 WEB_VERIFIED satırı **korunmuş** | 2/2 | §1 |
| K3 | §5 sınıf tablosu = 2 korunan + 4 yeni satır | 6 | §5 |
| K4 | Her YAML'de `api_key` = ortam değişkeni **adı** | değişken adı, değer yok | §5.3 |
| K5 | Dosyada gerçek anahtar metni (`sk-` benzeri) | 0 | G12 · P1 |
| K6 | `base_url` §5 tablosundaki kök değerle birebir | eşit | P3 |
| K7 | Slug formu sınıf kuralına uyar (`saglayici/model` / deployment / yerel etiket) | uygun | §1 · §5.1 |
| K8 | `IChatClient` 3 üye imzası kodda mevcut | 3/3 | §6.1 |
| K9 | Streaming döngüsü `CancellationToken` taşır | evet | §6.2 · T5 |
| K10 | Retry yalnız geçici sınıf (408/429/5xx) | evet | §7.1 |
| K11 | 429'da `Retry-After` gözetilir | evet | §7.2 |
| K12 | Token sayısı yalnız `usage` | evet | §8 kural 1 |
| K13 | §9 kimlik testleri koşulmuş | T1–T6 | §9 |
| K14 | Mojibake · BOM · yer tutucu | 0 · 0 · 0 | G4 · G5 |
| K15 | Satır ≥ 480 · KB ≥ 26 | hedefte | G6 · §15 |
| K16 | Yeni eklenen bağlantı hedefleri diskte | 0 kırık | G7 |

```markdown
## Quality Report — llm-provider-template.md (YYYY-MM-DD)
Ölçüm: <satır> satır / <KB> KB (≥480 / ≥26) · Checklist K1–K16 → <n>/16
Kimlik T1–T6 → <n>/6 · Anahtar metni 0 · Mojibake 0 · BOM yok · Kırık link (yeni) 0
Karar: ONAY | RED (<gerekçe>) · Kanıt: §9 komut çıktısı ekte
```

---

## §14 Referanslar

| # | Kaynak | Ne verir | Kullanım |
|---|---|---|---|
| 1 | `openrouter.ai/docs/api/reference/authentication` | anahtar biçimi (`sk-or-`) · başlık | §1 · WEB_VERIFIED notu |
| 2 | `build.nvidia.com/llms.txt` | NIM uç listesi | §1 · WEB_VERIFIED notu |
| 3 | `learn.microsoft.com` → `Microsoft.Extensions.AI` | `IChatClient` imzaları | §6 — sürümle doğrulanır |
| 4 | [new-doc-contract](new-doc-contract.md) | frontmatter · link · 3-8 KB · red nedenleri | tüm §'ler |
| 5 | [index](index.md) | şablon envanteri · G1..G12 · kalite sözleşmesi §3.7 | §11 · K15 |
| 6 | [WORKFLOW](../../WORKFLOW.md) | kapı zinciri · kanıt biçimi | §11 G8 satırı |

> **Kaynak doğrulama kuralı:** §1'deki iki satır `WEB_VERIFIED` (2026-10-04) ile işaretlidir;
> §5 sınıf 3–6 satırları **VARSAYILAN**'dır ve ilk kullanımda §9 ile sınanmadan "doğru"
> sayılmaz — kanıtsız doğruluk iddiası G9 ihlalidir.

### §14.1 Red Team — Sık Yapılan Hatalar

| # | Hata | Sinyal | Doğrusu |
|---|---|---|---|
| 1 | Anahtarın YAML'a değer olarak yazılması | K4 · K5 | yalnız değişken **adı** (§5.3) |
| 2 | `base_url`'e uç yolu eklemek | P3 · K6 | kök URL; `/chat/completions` istemci ekler |
| 3 | 200'ü tek kanıt saymak | §9 (T2–T6 boş) | kimlik 6 testle kanıtlanır |
| 4 | Akışı tek yanıt gibi beklemek | P6 | §6.2 blok 2 |
| 5 | 429'da hızlanmak | P7 · K11 | `Retry-After` + §7.2 backoff |
| 6 | Karakterden token tahminini kesin saymak | §8 satır 3 | yalnız `usage` (G9) |
| 7 | `net48`'de akış derlememesi | P9 · K8 | `Microsoft.Bcl.AsyncInterfaces` (§6.2) |
| 8 | Bu dosyaya sağlayıcı kodu gömmek | §11 notu | kod `src/`, kanıt §9 (G12) |

---

## §15 Ölçüm (Bu Dosyanın Kendi Sınıfı)

```powershell
$d = 'C:\versacoder\.ai\.templates\llm-provider-template.md'
$b = [System.IO.File]::ReadAllBytes($d)
$r = Get-Content $d -Raw -Encoding UTF8
'{0} satir · {1:N1} KB · BOM={2} · mojibake={3} · yer-tutucu={4}' -f `
  (Get-Content $d -Encoding UTF8).Count, ((Get-Item $d).Length / 1KB), `
  ($b[0] -eq 239 -and $b[1] -eq 187 -and $b[2] -eq 191), `
  [regex]::Matches($r, '\u00C3.|\u00E2\u20AC').Count,
  [regex]::Matches($r, '\x7B\x7B[^\x7D]+\x7D\x7D').Count
# hedef (G6 + K15): satir >= 480 · KB >= 26 · BOM False · mojibake 0 · yer-tutucu 0
```

| # | Sınıf | Eşik | Kaynak |
|---|---|---|---|
| 1 | Bu dosya (`.templates/` kök şablonu) | ≥ 480 satır · ≥ 26 KB | G6 · envanter §3.7 |
| 2 | Üretilen yapılandırma (provider bloğu) | yer tutucu **0** | G4 |
| 3 | Aktarılan token sayısı | kesin `usage` | §8 · G9 |
| 4 | Kimlik testi | T1–T6 | §9 · P11 |

---

## §16 Karar Matrisi ve Değişiklik Kuralları

### §16.1 Hangi Sınıf, Hangi Durumda

| # | Durum | Seçilen sınıf | Kazanım | Bedel |
|---|---|---|---|---|
| 1 | Tek uçta çoklu model denemesi | 1 OpenRouter | tek anahtar, geniş katalog (§1) | dış ağ bağımlılığı |
| 2 | NVIDIA donanımında yerel sunum | 2 NIM | sağlayıcı uçları doğrulanmış | dış ağ bağımlılığı |
| 3 | Üretim, tek sağlayıcı, basit entegrasyon | 3 OpenAI | en kısa yol, geniş dokümantasyon | tek sağlayıcı riski |
| 4 | Kurumsal veri bölgesi / kota denetimi | 4 Azure | kiracı içi denetim, `api-version` kilidi | ek `api-version` karmaşası (P10) |
| 5 | Kanıt/prototip; veri makineden çıkmasın | 5/6 Yerel | ağ yok, anahtar yok, maliyet 0 | model gücü makineye bağlı |

### §16.2 Değişiklik Kuralları (Ne Değişir / Ne Sınanır)

| # | Değişiklik | Değişmez | Yeniden sınanan |
|---|---|---|---|
| 1 | `base_url` (sınıf 1 → 3 vb.) | slug kuralları, §6 kodu | §9 T1–T3 |
| 2 | Model slug | auth başlığı biçimi | §9 T1 + T3 |
| 3 | Anahtar rotasyonu | YAML (yalnız **ad** yazılıdır) | §9 T2 — reddin gerçekliği |
| 4 | Sağlayıcı sınıfı | `IChatClient` sözleşmesi | §13 K8–K13 bütünü |
| 5 | Retry/timeout değeri | sınıflandırma (§7.1) | 429/5xx simülasyonu |

### §16.3 Dağıtım Öncesi Sekiz Madde

| # | Madde | Kanıt |
|---|---|---|
| 1 | Ortam değişkeni tanımlı, değer dosyada yok | §12.1 çıktısı · K4/K5 |
| 2 | Uç canlı, anahtar reddi gerçek | §9 T1 + T2 çıktıları |
| 3 | Slug sağlayıcı kataloğundan | §9 T3 |
| 4 | Akış ve iptal yolu çalışıyor | §9 T4 + T5 |
| 5 | `usage` sayaçları okunuyor | §9 T6 |
| 6 | Retry yalnız geçici sınıf | §13 K10/K11 |
| 7 | Kodda tek istemci örneği | §6.2 kural 5 |
| 8 | Rapor ONAY, komut çıktıları ekte | §13 Quality Report |

---

## §17 Sık Sorulanlar (SSS)

| # | Soru | Cevap | İlgili |
|---|---|---|---|
| 1 | `base_url`'e `/chat/completions` ekler miyim? | **Hayır** — §5 tablosundaki kök yazılır; uç yolunu istemci ekler | P3 · K6 |
| 2 | Anahtarı YAML'a değer olarak yapıştırabilir miyim? | **Hayır** — yalnız ortam değişkeninin **adı** yazılır | §5.3 · P1 · P5 |
| 3 | OpenAI anahtarımı OpenRouter'da kullanabilir miyim? | **Hayır** — her sağlayıcı kendi prefix'i (`sk-or-` vb.) | §4/2 · P2 |
| 4 | Akış ile senkron çağrı farkı ne? | senkron tek yanıt (§6.2 blok 1), akış parçalı UI günceller (blok 2); ikisi de `ct` taşır | §6.1 · T4 |
| 5 | Model adını nereden bulurum? | bulut: sağlayıcı kataloğu; yerel: `ollama list` gibi komutun çıktısı | §5.1 |
| 6 | `429` alırsam ne yaparım? | `Retry-After` kadar bekle, sonra §7.2 backoff; en fazla 3 deneme | §7.1 · P7 |
| 7 | Azure `400/404` veriyor? | `api-version` sorgusu ve deployment adı kontrol edilir | §5.1 · P10 |
| 8 | Token sayısını nasıl kanıtlarım? | yalnız yanıt içindeki `usage` ile; karakter tahmini kesin sayı sayılmaz | §8 · G9 · P8 |
| 9 | `net48` üzerinde akış çalışır mı? | evet — arayüz `netstandard2.0`; akış için `Microsoft.Bcl.AsyncInterfaces` gerekir | §6.2 · P9 |
| 10 | Sağlayıcı kodu bu dosyaya yazılır mı? | **Hayır** — kod `src/VersaCoder.Infrastructure/`, bu dosya yalnız sözleşme | §11 · G12 |
| 11 | Tek HTTP 200 yeterli mi? | **Hayır** — kimlik T1–T6 ile kanıtlanır | §9 · P11 |
| 12 | Henüz doğrulanmadıysa ne yazarım? | `DOĞRULAMA GEREKLI` — "çalışıyor" iddiası komut çıktısız yazılmaz | §4/5 · G9 |

> **Tek cümlelik kapanış:** üç değişken (`base_url` · `api_key` **adı** · `model` slug) doğru,
> kimlik T1–T6 kanıtlı, `usage` kayıtlı — geri kalan her şey §5'ten §13'e kadar bu belgede
> denetlenebilir; kanıtsız kalan her satır `DOĞRULAMA GEREKLI` olarak kalır.
