---
title: "Kaynak Kod Arastirmasi - opencode / Claude Code / AI editorler / provider'lar"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "GitHub kaynak kod arastirmasi: opencode, Claude Code ekosistemi, acik AI editorler, NVIDIA NIM ve OpenRouter provider kanitlari"
---

# Kaynak Kod Arastirmasi

**Zorunlu Baglantilar:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [platform-arastirma](12-platform-arastirmasi.md) · [web-prompt-derlemesi](../prompts/web-prompt-derlemesi.md)

> **Tek cumlelik sorumluluk:** *Benzer bir proje (AI coding agent + editor) hangi
> acik kaynaklardan model alinir, provider'lar nasil baglanir* sorusunun
> kaynakli tek cevabidir.

---

## §0 Ozet (Answer First)

- **opencode referans alinacak proje:** `github.com/anomalyco/opencode` — MIT,
  203k+ yıldız, terminal TUI; `build`/`plan` built-in ajanları + `general` subagent.
- **Claude Code hukuki:** sızıntı repoları değil; resmi `code.claude.com/docs`
  (skills, slash-commands) + UI klonları (ör. `shenxuxing/claude-code-ui`) referans.
- **Provider kanıtı kesin:** NIM = OpenAI + Anthropic uyumlu tek uç; OpenRouter =
  tek uçtan 300+ model/70+ sağlayıcı.
- **Vault etkisi:** vault `opencode.json` iddiası (kök AGENTS §7) bu araştırmayla
  uygulanabilir hale geldi — kapı GATE-003 sonrası.

## §1 Incelenen kaynaklar

| # | Kaynak | Ne ögrenildi | Etiket |
|---|---|---|---|
| 1 | `github.com/anomalyco/opencode` | MIT, 203243★/26490 fork; TUI; `build`+`plan` ajan, `@general` subagent; kurulum `npm i -g opencode-ai` / `scoop` / `brew` | `WEB_VERIFIED — github.com/anomalyco/opencode (2026-10-04)` |
| 2 | `shenxuxing/claude-code-ui` | Claude Code/Agent SDK için modern chat UI klonu | `WEB_VERIFIED — github.com (2026-10-04)` |
| 3 | `code.claude.com/docs/en/skills.md` | SKILL.md = YAML frontmatter (`name`, `description` tetik cümleleri, `argument-hint`, `disable-model-invocation`) + gövde; `/skill-name` ile çağırılır | `WEB_VERIFIED (2026-10-04)` |
| 4 | `code.claude.com/docs/en/slash-commands.md` | Komut = `.md` dosyası; frontmatter `description/allowed-tools/argument-hint/model`; gövde prompt + `$ARGUMENTS` | `WEB_VERIFIED (2026-10-04)` |
| 5 | `openrouter.ai` | Base URL `https://openrouter.ai/api/v1`, `sk-or-` anahtar, `provider/model` slug, OpenAI SDK drop-in | `WEB_VERIFIED — openrouter.ai/docs (2026-10-04)` |
| 6 | `build.nvidia.com/llms.txt` | NIM Base URL `https://integrate.api.nvidia.com/v1`, Bearer auth, `/v1/chat/completions` + `/v1/messages` (Anthropic-uyumlu) | `WEB_VERIFIED (2026-10-04)` |
| 7 | `devexpress.com/subscriptions/whats-new` | **v26.1 mevcut** (29.07.2026); v26.1'den itibaren NuGet.org tek kaynak | `WEB_VERIFIED (2026-10-04)` |

## §2 Benzer proje iskeleti (model alınacak şema)

```text
proje/
  opencode.json          # provider + agent tanimlari (GATE-003'te yazilir)
  .ai/                   # vault: .rules .templates prompts knowledge ...
  src/
    Presentation/        # WinForms: *Form.cs + *Form.Designer.cs + RibbonForm
    Infrastructure/      # ADO.NET + Dapper (G2)
  tests/
  Directory.Packages.props  # DevExpress v26.1 dahil CPM
```

| # | opencode.1'den alınan | Kaynak kanıtı |
|---|---|---|
| 1 | `.ai/` vault yapısı (16 klasör) | `C:\versacoder.1` envanteri |
| 2 | ajan tanımları `.agents/*-agent.md` (7 adet) | aynı vault + §1 madde 1 (build/plan ayrımı) |
| 3 | skills/commands biçimi | §1 madde 3-4 (resmi format) |
| 4 | provider yapılandırması | §1 madde 5-6 (NIM/OpenRouter uçları) |

## §3 Yasak hatirlatma

| # | Kaynak | Neden taşınmaz |
|---|---|---|
| 1 | "Leaked" Claude Code / Cursor repoları | hukuki belirsizlik + G1: bilgi alınır, metin kopyalanmaz |
| 2 | Repo README/CODE metinleri | cümle kopyası yasak; yalnız şema/fikir (G1) |
| 3 | Anahtar/endpoint değerleri | düz metin kutuda; gerçek anahtar hiçbir dosyada yok (G12) |

## §4 Acik sorular

| # | Soru | Durum |
|---|---|---|
| 1 | DevExpress **25.2.8** mi, **26.1** mi? (12-rapor §6) | `CONFLICT DETECTED` — kullanıcı cevabı bekleniyor |
| 2 | `opencode.json` hedefe GATE-003'te mi yazılacak? | kapı kuralı gereği onay bekleniyor |
