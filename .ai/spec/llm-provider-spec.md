---
title: "LLM Provider Spec - OpenRouter + NVIDIA NIM"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "OpenRouter ve NVIDIA NIM LLM sağlayıcılarının port sözleşmesi (IAgentProvider), endpoint, anahtar yönetimi, hata kodları (VC-E-06xx) ve AI.Engine entegrasyonu - VC-LLM-001..014 gereksinimleri"
---

# LLM Provider Spec — OpenRouter + NVIDIA NIM

**Zorunlu Baglantilar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [tool-spec](tool-spec.md) · [protocol-spec](protocol-spec.md) · [cli-spec](cli-spec.md) · [session-spec](session-spec.md) · [security-rules](../.rules/security-rules.md) · [data-access-rules](../.rules/data-access-rules.md)

> **Tek cümlelik sorumluluk:** *OpenRouter ve NVIDIA NIM sağlayıcıları nasıl bağlanır, port (l0) nasıl tanımlanır, anahtarlar nerede tutulur, hangi hata kodu ne anlama gelir* sorusunun tek cevabı.
> Gereksinim kimliği: **`VC-LLM-NNN`** · Hata kodları **`VC-E-0601..0615`** · Port: `IAgentProvider` (l0) · Uygulama: `VersaCoder.AI.Engine` (K0400-K0449).

---

## §0 Özet (Answer First)

2 sağlayıcı, **tek port** (`IAgentProvider`), **tek orkestratör** (`AI.Engine`). Sağlayıcı seçimi config'den okunur; anahtarlar `keys.md`'de (vault) + `~/.versacoder/secrets.json` (runtime). **Hiçbir `.cs` yazılmadan önce** bu spec `log.md`'ye kaydedilir (GATE-003 açık).

| Sağlayıcı | Endpoint | Anahtar | Hedef | K bandı |
|---|---|---|---|---|
| **OpenRouter** | `https://openrouter.ai/api/v1` | `sk-or-v1-...` | Çoklu model routing (Claude, GPT, Llama, DeepSeek) | K0400-K0449 |
| **NVIDIA NIM** | `https://integrate.api.nvidia.com/v1` | `nvapi-...` | Yerel/uzak NIM (Llama 3.1, Mistral, DeepSeek-R1) | K0400-K0449 |

> **Security (SEC-06):** Anahtar **hiçbir `.cs`'e gömülmez**, log'a yazılmaz, konsola basılmaz. Yalnız config (vault) + environment variable. `.gitignore`'da `secrets.json`.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | `IAgentProvider` port sözleşmesi (l0) | Provider **uygulaması** (l4, AI.Engine) | [versacoder-spec](versacoder-spec.md) |
| 2 | OpenRouter + NIM endpoint, model listesi, hata kodları | Diğer sağlayıcılar (OpenAI, Anthropic doğrudan) — **eklenebilir** | bu spec §7 |
| 3 | Anahtar yönetimi (config + env) | Anahtar **üretimi** / rotation | [security-rules](../.rules/security-rules.md) |
| 4 | Retry, rate limit, timeout | Rate limit **sayacı** (telemetri) | [performance-guidelines](../.rules/performance-guidelines.md) |
| 5 | Streaming (SSE) + tool çağrısı | Agent **planlama** / görev kuyruğu | [session-spec](session-spec.md) |

> **YASAK:** HTTP isteği **doğrudan UI'dan** atılmaz (l7 izolasyon); EF Core **yasak** (G2, ADO.NET+Dapper); anahtar log'a yazılmaz (SEC-05).

---

## §2 Port (`IAgentProvider`) — l0

```csharp
// src/VersaCoder.Abstractions/Ports/IAgentProvider.cs  (K0000 bandı)
namespace VersaCoder.Abstractions.Ports;

public interface IAgentProvider
{
    string Name { get; }                      // "openrouter" | "nvidia-nim"
    bool IsAvailable { get; }                 // ping / key check
    Task<ProviderResult> CompleteAsync(ProviderRequest request, CancellationToken ct);
    IAsyncEnumerable<string> StreamAsync(ProviderRequest request, CancellationToken ct);
    Task<IReadOnlyList<ModelInfo>> ListModelsAsync(CancellationToken ct);
}
```

**Sözleşme alanları:**

| Alan | Tip | Not |
|---|---|---|
| `ProviderRequest` | record | `Model`, `Messages[]`, `Temperature`, `MaxTokens`, `Tools[]`, `Stream` |
| `ProviderResult` | record | `Text`, `Usage` (tokens), `FinishReason`, `Model` |
| `ModelInfo` | record | `Id`, `ContextLength`, `Pricing`, `Capabilities[]` |
| `ProviderError` | enum | `RateLimit`, `Auth`, `InvalidRequest`, `Unavailable`, `Timeout` |

**Bağımlılık oku (ADR-028 8x8):**
- `Abstractions` (l0) → hiçbir şey (port saflığı)
- `AI.Engine` (l4) → `Abstractions` (portu uygular) ✓
- `Protocol` (l5) → `Abstractions` (tool çağrısı port üzerinden) ✓
- `Desktop` (l7) → `Application` (l2) → **asla** `AI.Engine` doğrudan ✗

---

## §3 Sağlayıcı Detayları

### §3.1 OpenRouter (`VC-LLM-001..005`)

| ID | Konu | Değer | Kanıt |
|---|---|---|---|
| `VC-LLM-001` | Endpoint | `https://openrouter.ai/api/v1/chat/completions` | POST 200 |
| `VC-LLM-002` | Auth | `Authorization: Bearer sk-or-v1-...` | 401 = `VC-E-0602` |
| `VC-LLM-003` | Varsayılan model | `anthropic/claude-sonnet-4.5` (veya config) | config'den okunur |
| `VC-LLM-004` | Model listesi | `GET /models` → `ModelInfo[]` | ≥ 1 model |
| `VC-LLM-005` | Ek header | `HTTP-Referer` + `X-Title` (OpenRouter kuralı) | 200 |

**Örnek istek:**

```json
POST https://openrouter.ai/api/v1/chat/completions
Authorization: Bearer sk-or-v1-...
Content-Type: application/json
{
  "model": "anthropic/claude-sonnet-4.5",
  "messages": [
    {"role": "system", "content": "VersaCoder agent"},
    {"role": "user", "content": "K2105 layer'ını açıkla"}
  ],
  "temperature": 0.2,
  "max_tokens": 4096,
  "stream": true
}
```

### §3.2 NVIDIA NIM (`VC-LLM-006..010`)

| ID | Konu | Değer | Kanıt |
|---|---|---|---|
| `VC-LLM-006` | Endpoint | `https://integrate.api.nvidia.com/v1/chat/completions` | POST 200 |
| `VC-LLM-007` | Auth | `Authorization: Bearer nvapi-...` | 401 = `VC-E-0602` |
| `VC-LLM-008` | Varsayılan model | `meta/llama-3.1-70b-instruct` (veya config) | config'den okunur |
| `VC-LLM-009` | Model listesi | `GET /models` → `ModelInfo[]` | ≥ 1 model |
| `VC-LLM-010` | Yerel NIM | `http://localhost:8000/v1` (Docker NIM) | `IsAvailable` = ping |

**Örnek istek:**

```json
POST https://integrate.api.nvidia.com/v1/chat/completions
Authorization: Bearer nvapi-...
Content-Type: application/json
{
  "model": "meta/llama-3.1-70b-instruct",
  "messages": [
    {"role": "user", "content": "Merhaba, VersaCoder mısın?"}
  ],
  "temperature": 0.3,
  "max_tokens": 1024
}
```

> **`DOĞRULAMA GEREKLİ`:** NIM endpoint URL ve model adları **canlı test edilmedi** (2026-10-02). İlk bağlantıda `GET /models` ile doğrulanır.

---

## §4 Ortak Davranış (`VC-LLM-011..014`)

| ID | Konu | Kural | Kanıt |
|---|---|---|---|
| `VC-LLM-011` | **Retry** | 429/5xx → exponential backoff: 1s, 2s, 4s (max 3) | 3 retry sonrası fail |
| `VC-LLM-012` | **Timeout** | `CancellationToken` + `--timeout` (CLI) varsayılan 300 sn | timeout = `VC-E-0611` |
| `VC-LLM-013` | **Streaming** | SSE `data:` satırları → `StreamAsync` yield | ilk token < 30 sn (`protocol-spec` §5) |
| `VC-LLM-014` | **Fallback** | OpenRouter 4xx → NIM'e düş (config'de `fallback: true`) | ikinci sağlayıcı success |

**Tool çağrısı (protocol-spec §4):**
- Provider `tool_calls` dönerse → `Protocol` (l5) parse eder → `Tool` (tool-spec) çalıştırılır → sonuç tekrar provider'a gönderilir.
- **`IAgentProvider` tool çağrısını bilmez** — tool orkestrasyonu `AI.Engine`'dedir (SRP).

---

## §5 Hata Kodları (`VC-E-0601..0615`)

| Kod | Anlam | HTTP | Exit |
|---|---|---|---|
| `VC-E-0601` | Sağlayıcıya ulaşılamadı (DNS/timeout) | — | 2 |
| `VC-E-0602` | Yetkisiz (anahtar geçersiz) | 401/403 | 4 |
| `VC-E-0603` | Rate limit (429) | 429 | 2 (retry) |
| `VC-E-0604` | Geçersiz istek (400) | 400 | 1 |
| `VC-E-0605` | Model bulunamadı (404) | 404 | 3 |
| `VC-E-0606` | İç sunucu hatası (5xx) | 500+ | 2 (retry) |
| `VC-E-0607` | Kota/deneme bitti | 402 | 4 |
| `VC-E-0608` | Context length aşıldı | 400 | 1 |
| `VC-E-0609` | Streaming kesildi (bağlantı) | — | 2 |
| `VC-E-0610` | Tool çağrısı geçersiz şema | 400 | 1 |
| `VC-E-0611` | Timeout (CLI `--timeout`) | — | 5 |
| `VC-E-0612` | Kullanıcı iptali (Ctrl+C) | — | 5 |
| `VC-E-0613` | Anahtar config'de yok (`secrets.json`) | — | 4 |
| `VC-E-0614` | Sağlayıcı config'de tanımsız | — | 2 |
| `VC-E-0615` | Yanıt JSON parse hatası | — | 2 |

---

## §6 Konfigürasyon

**`~/.versacoder/secrets.json`** (`.gitignore`'da, sadece okunur):

```json
{
  "openrouter": { "apiKey": "sk-or-v1-..." },
  "nvidia-nim": { "apiKey": "nvapi-..." }
}
```

**Proje `.versacoder/config.json`:**

```json
{
  "llm": {
    "provider": "openrouter",
    "model": "anthropic/claude-sonnet-4.5",
    "temperature": 0.2,
    "maxTokens": 4096,
    "fallback": { "provider": "nvidia-nim", "model": "meta/llama-3.1-70b-instruct" },
    "timeout": 300
  }
}
```

**OKUMA SIRASI (config-spec §3):** env (`OPENROUTER_API_KEY`) → `secrets.json` → default. **Eksikse** `VC-E-0613` (exit 4), crash yok.

---

## §7 Genişletilebilirlik (Open/Closed)

| Sağlayıcı | Durum | Port'a nasıl eklenir |
|---|---|---|
| OpenRouter | **bu spec** | `OpenRouterProvider : IAgentProvider` |
| NVIDIA NIM | **bu spec** | `NvidiaNimProvider : IAgentProvider` |
| OpenAI doğrudan | gelecek | `OpenAiProvider` — aynı port, yeni adapter |
| Anthropic doğrudan | gelecek | `AnthropicProvider` — aynı port |
| Yerel (Ollama, llama.cpp) | gelecek | `LocalProvider` — `http://localhost:11434/v1` |

**Kayıt:** her yeni sağlayıcı `k5000-layer-map.csv`'ye satır + `layer.json` `apiContract` alanına eklenir (LAY-06).

---

## §8 Kabul Kriterleri

- [ ] 14 gereksinim (`VC-LLM-001..014`) tek tek test edilebilir
- [ ] `IAgentProvider` portu l0'da, bağımlılığı **0** (NetArchTest `LayerRulesTests`)
- [ ] 2 sağlayıcı adapter'ı portu uygular, **asla** `DbConnection`/DevExpress görmez
- [ ] 15 hata kodu (`VC-E-0601..0615`) her biri en az 1 testte tetiklenir
- [ ] Retry 429 → 3 deneme (1s/2s/4s) → `VC-E-0603`
- [ ] Streaming ilk token < 30 sn (`protocol-spec` §5)
- [ ] Anahtar `.cs`'te **0** eşleşme (grep `sk-or-v1|nvapi-` = 0)
- [ ] Fallback: OpenRouter 4xx → NIM success
- [ ] `check-links.ps1` = 0 bozuk link

---

## §9 Açık Konular (`DOĞRULAMA GEREKLİ`)

| # | Açık | Neden | Kapanış |
|---|---|---|---|
| 1 | NIM endpoint URL + model adları canlı test edilmedi | 2026-10-02 spec yazımı | ilk bağlantıda `GET /models` |
| 2 | OpenRouter model listesi (Claude, GPT, Llama, DeepSeek) | `GET /models` çağrılmadı | canlı test |
| 3 | Tool çağrısı şeması (her sağlayıcı farklı) | `protocol-spec` §4 henüz detaylı değil | protocol-spec revizyonu |
| 4 | Fallback sırası (OpenRouter → NIM → yerel mi?) | 3'lü zincir netleşmedi | config şeması onayı |
| 5 | Fiyat/kota takibi (OpenRouter kredi) | NIM ücretsiz, OpenRouter kredi | telemetry |
| 6 | `keys.md`'ye `openrouter`/`nvidia` anahtar satırı eklenmedi | bu spec yeni | `keys.md`'ye 2 satır |

---

**Sahip:** architect · **Güncelleyen:** architect · **Tarih:** 2026-10-02 · **Ölçüm:** bu spec yazılmadan önce `keys.md` + tüm spec'lerde `openrouter|nvidia|nim` = **0 eşleşme** (grep). Şimdi 1 dosya.
