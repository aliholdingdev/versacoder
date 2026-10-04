---
title: "Web Prompt Derlemesi - ChatGPT / Claude / Cursor / Skill / Command / Plan"
type: prompt
category: prompts
status: active
version: 1.1.0
updated: 2026-10-04
authority: SSOT
scope: "Webden ve yerel arsivden (G: el10 92 part + new 7 MD, .ai/reports 12-13) toplanan prompt/skill/command/plan orneklerinin bicim kurallari ve ornek kutulari - vault prompt uretimi icin tek toplama noktasi"
---

# Web Prompt Derlemesi

**Zorunlu Baglantilar:** [index](../index.md) · [CLAUDE](../CLAUDE.md) · [AGENTS](../AGENTS.md) · [kaynak-kod-raporu](../reports/13-kaynak-kod-arastirmasi.md)

> **Tek cumlelik sorumluluk:** *ChatGPT/Claude/Cursor promptları, skill.md,
> slash command ve plan promptları hangi biçimde yazılır* sorusunun web
> örnekli tek cevabidir (toplama noktası: `.ai/prompts/`).

---

## §0 Ozet (Answer First)

6 biçim: **system prompt** (rol + XML etiket + az örnek) · **user prompt** (açık,
somut, format tarifli) · **cursor rule** (`description/alwaysApply/globs` frontmatter) ·
**SKILL.md** (`name` + tetikleyici `description` + gövde) · **slash command**
(`$ARGUMENTS` + `argument-hint`) · **plan prompt** (faz 1 sohbet, faz 2 detaylı plan).
Hepsi vault kuralıyla uyumlu: Markdown + frontmatter + kanıt satırı.
Yerel kaynaklar §1 satır 7-10'da: `.ai/reports` 12-13 + G: el10 92 part + `new` 7 MD.

## §1 Kaynak envanteri

| # | Kaynak | Tür | Etiket |
|---|---|---|---|
| 1 | `github.com/f/prompts.chat` (eski Awesome ChatGPT Prompts) | prompt kütüphanesi | `WEB_VERIFIED — 2026-10-04` |
| 2 | `github.com/LouisShark/chatgpt_system_prompt` | sızdırılmış GPT sistem promptları (eğitim amaçlı) | `WEB_VERIFIED — 2026-10-04` |
| 3 | `github.com/mustvlad/ChatGPT-System-Prompts` | kategori bazlı sistem promptları | `WEB_VERIFIED — 2026-10-04` |
| 4 | `docs.anthropic.com/.../system-prompts` | Claude resmi: rol ver, XML etiket, 3-5 örnek | `WEB_VERIFIED — 2026-10-04` |
| 5 | `github.com/asgeirtj/system_prompts_leaks` (OpenAI/Codex `plan_mode.md`) | plan modu promptu | `WEB_VERIFIED — 2026-10-04` |
| 6 | `github.com/cursorrulespacks/cursorrules-collection` (110+ `.mdc`) | cursor rule formatı | `WEB_VERIFIED — 2026-10-04` |
| 7 | `.ai/reports/13-kaynak-kod-arastirmasi.md` | yerel: GitHub / acik kaynak kod arastirmasi | `YEREL_KAYNAK — 2026-10-04` |
| 8 | `.ai/reports/12-platform-arastirmasi.md` | yerel: platform arastirmasi | `YEREL_KAYNAK — 2026-10-04` |
| 9 | `G:\...\Versacoder\new\versacoder-analiz-el10\` (part-00..91 = 92 dosya) | yerel: el10 analiz seti (part-80 dinamik prompt sentezi, part-89 zero-hallucination) | `YEREL_KAYNAK — 92 dosya` |
| 10 | `G:\...\Versacoder\new\` (`coremusic-vault-template.md` + `versacoder.*` = 7 MD) | yerel: vault yapi ornegi + proje planlama notlari | `YEREL_KAYNAK — 7 dosya` |

> **Yerel G: arsiv yolu (tam):** `G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\new\`
> Kural: bu arsivden yalnizca **bicim + fikir** alinir, metin kopyalanmaz (G1);
> icerik hedef vault'ta yeniden uretilir. Sayilar olcum 2026-10-04 (`Get-ChildItem`).

## §2 Biçim örüntüleri (nasıl yazılır)

| # | Biçim | Frontmatter / yapı | Kritik kural |
|---|---|---|---|
| 1 | System prompt | rol + `<etiket>` blokları + son hatırlatma | 3-5 örnek, `<example>` içinde; uzun promptta sona kısa hatırlatma |
| 2 | Cursor rule `.mdc` | `description:` `alwaysApply: true/false` `globs:` | agent mode `.mdc` tercih eder; `.cursorrules` legacy |
| 3 | SKILL.md | `name` + `description` (tetik cümleleri, 1536 karakter sınırı) + `argument-hint` | `/skill-name` ile çağrılır |
| 4 | Slash command `.md` | `description` `allowed-tools` `argument-hint` `model` | gövdede `$ARGUMENTS` |
| 5 | Plan prompt | 3 faz: sohbetle plan → niyet+uygulama detayı → teslim | "another engineer'a devredilebilir" detay |
| 6 | Vault kutu promptu | yok (düz gövde) | 5 alan `+ - |` kutu, `KANIT` sayısal |

## §3 Örnek kutular (adapt edilmiş, kopya değil - G1)

```text
# SYSTEM (Claude usulu)
You are <rol + uzmanlik>. Kurallar: <kural 1>, <kural 2>.
<guidance>...kisa kural...</guidance>
Cevap formati: <madde 1>, <madde 2>. Emin degilsen "dogrulanmadi" de.

# COMMAND (.claude/commands/commit.md benzeri)
---
description: Create a git commit
argument-hint: [message]
---
Git commit at: $ARGUMENTS. Once `git status` goster, dosya bazli `git add`.

# PLAN (Codex plan_mode.md usulu)
Faz 1: kullaniciyla sohbet ederek plani netlestir.
Faz 2: niyet + uygulama detayini yaz (baska muhendise devredilebilir).
Faz 3: kesinlestir; onay bekle, kod yazma.
```

## §4 RED sinyalleri

| # | Sinyal | Düzeltme |
|---|---|---|
| 1 | Sızıntı prompt metni birebir kopya | G1: yalnız biçim + fikir alınır |
| 2 | Skill'de tetikleyici `description` yok | tetik cümlelerini yaz (1536 kr.) |
| 3 | `$ARGUMENTS` kullanılmayan komut | argümanı gövdeye bağla |
| 4 | Frontmattersiz derleme dosyası | 8 alan zorunlu (contract §1) |
| 5 | Kanıtsız "araştırma bitti" | §1 tablosundaki URL yoksa `DOĞRULAMA GEREKLI` |
