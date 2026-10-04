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
