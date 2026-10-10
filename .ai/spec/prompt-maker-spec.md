---
title: "Prompt Maker ve Oturum Girdi Hattı Şartnamesi"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-01
authority: primary
scope: "Prompt Maker aracinin giris/kabul sozlesmesi (MOD A / MOD B), oturumun girdi hatti (Exploration Gate -> Instruction) ve sonradan eklenecek ozelliklerin kayit listesi"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Prompt Maker ve Oturum Girdi Hattı (`spec/prompt-maker-spec.md`)

**Zorunlu Bağlantılar:** [[spec/index|index]] · [[definitions/engine|../engine.md]] §1.1.1 ·
[[definitions/agents|../AGENTS.md]] §6 · [[checklists/index|../checklists/index.md]] ·
[[ops/todos/index|../TODOS/index.md]] · [[.rules/coding-standards|../.rules/coding-standards.md]]

> **Tek cümlelik sorumluluk:** prompt girildiğinde **kod yazılmadan önce** ne
> olduğu, soruların nereden geldiği ve çıktının hangi şablonla kapandığının tek
> cevabıdır.

---

## §0 Özet (Answer First)

- Prompt girilir → **oturum / kapı / model atanmaz**, hemen **MOD A (ASKING
  QUESTIONS)** açılır → cevaplar toplanır → **MOD B** şablonu üretilir.
- Soru kaynağı **dışarıdan değil**, vault'tan okunur: `.ai/index.md` → `spec/`,
  `.decisions/`, `glossary.md` + proje kodu. Jenerik 8 soru **yasak**.
- **Tavan 10 soru**; `yeter / devam` diyerek erken kapanabilir.
- Model/provider **elle atanmaz** — seçimi OpenCode yapar (AGENTS §6).
- Girdi hattının motor akıştaki yeri: [[definitions/engine|engine.md]] §1.1.1.

---

## §1 MOD A — ASKING QUESTIONS (önce çalışır)

| # | Kural | Değer |
|---|---|---|
| 1 | Oturum / kapı / commit | **açılmaz** |
| 2 | Soru tipi | **proje-özel**; jenerik 8 (ne, neden, kim, nere, ne zaman, nasıl, ne kadar, hangi) **yasak** |
| 3 | Soru kaynağı | `.ai/index.md` → `PROJECTS.md`, `VISION.md`, `spec/`, `.decisions/adr-index.md`, `glossary.md` + proje kodu |
| 4 | Hedef | vault'ta **cevabı olmayan** boşluğu sorar |
| 5 | Sıra | **teker teker**, her soru A/B/C seçenekli; cevap beklemeden diğerine geçilmez |
| 6 | Tavan | **10 soru** (`yeter / devam` ile erken kapanır) |
| 7 | Tekrar | cevaplanan soru **tekrar sorulmaz** |
| 8 | Model/provider | **elle atanmaz** (`model:` / `provider:` yazılmaz) |

**Soru biçimi:**

```text
? Soru [n]/10: [soru - vault'taki boslugu hedefler]
A) [secenek]
B) [secenek]
C) [secenek]
```

---

## §2 MOD B — Çıktı şablonu (sıra değişmez)

```text
[Mecut promptun /eli10 uzun paragraf hali - adim adim, yapay zekaya
 anlatir gibi derin aciklama]

[Istenilen sey nedir - acikla]

[Referanslar .ai vault'undan]
[Referanslar proje kodlarinda]

[Kullanici kararlari dogrultusunda:]
[Sorulan sorular]
[Sorulan sorularin cevaplari]

[Kullanicinin girdigi ORIJINAL promptun aynisi]

[Gorevlere bol]
```

**Footer (aynen iki satır):**

```text
✅ Prompt Cevaplarla İşlendi
Dil: Türkçe | Görev Sayısı: [n] | Cevap: [n]
```

| Sınır | Değer | Kaynak |
|---|---|---|
| Footer biçimi | bu iki satır **değiştirilemez** | `.opencode/skills/prompt-maker/SKILL.md` §4 |
| Orijinal prompt | **birebir**, kırpılmadan | MOD B §5 |
| Görev sayımı | `[n]` = gerçekten bölünen görev sayısı (uydurma yok, G5) | — |
| Dil | Türkçe | — |

---

## §3 Oturum girdi hattı (Exploration Gate → System Prompt)

Prompt Maker **tek başına çalışmaz**; onu besleyen hat aşağıdadır. Tam akış
[[definitions/engine|engine.md]] §1.1.1'de yaşar; burada **sözleşme** yazılır.

```text
 KULLANICI PROMPTU
      |
      v
[1] EXPLORATION GATE    proje yapisini kesfet  ->  explore ajan
      |                  SINIR: ayni anda en fazla 3 (AGENTS S5)
      v
[2] EXPLORATION CONTEXT kesif sonucunu SAKLA   ->  .ai/SESSIONS/ (DURUM: yok)
      |
      v
[3] PROMPT MAKER        proje baglamina gore N soru (jenerik 8 degil)
      |
      v
[4] INTENT ROUTER       tech stack'e gore confidence boost
      |                  + vault filtreleme      (DURUM: TANIM YOK)
      v
[5] INSTRUCTION         yalnizca ilgili vault dosyalarini yukle
      |                  BUTCE: 50K token        (DURUM: olcum YOK)
      v
[6] SYSTEM PROMPT       exploration context + routing guidance
```

### §3.1 Uygulama durumu (kanıt — 2026-10-01 ölçümü)

| # | Adım | Durum | Kanıt |
|---|---|---|---|
| 1 | Exploration Gate | ⚠️ **kısmen** | `explore` ajanı çağrılabiliyor; ama eşzamanlılık **3** ile sınırlı |
| 2 | Exploration Context | ❌ **yok** | `.ai/SESSIONS/` → `Test-Path = False` |
| 3 | Prompt Maker | ✅ **var** | `.opencode/{commands,skills}/prompt-maker/` |
| 4 | Intent Router | ❌ **yok** | vault + `.opencode/` içinde **0 eşleşme**; yalnız adım adı geçiyor |
| 5 | Instruction (50K) | ⚠️ **kural var, ölçüm yok** | bütçe kuralı yazılı, sayan mekanizma tanımsız |
| 6 | System Prompt | ⚠️ **kısmen** | sistem promptu OpenCode üretiyor; *routing guidance* eklenmemiş |

> **Sonuç:** 6 adımdan **2'si implemente, 4'ü eksik.** Eksikler §5 listesine
> yazıldı; bu dosya eksikleri **kapatmaz**, yalnız kaydeder.

### §3.2 Üç nokta tazeleme — `vault-loader` sözleşmesi

**Şart (kullanıcı):** oturumun **başında, ortalarında, sonunda** vault
yüklenmeli; **her geçişte veri taze** olmalı; **compat** yapısı zorunlu;
**session sonrası yeni veri vault'a geri yazılmalı.**

#### §3.2.1 Tetikleyici eşlemesi (ölçülmüş — tahmin değil)

| Nokta | Tetikleyici (plugin API `Hooks`) | İş |
|---|---|---|
| **Başlangıç** | `event` → `session.created` | Vault okunur, `/eli10 + /eli15` context'e girer |
| **Orta (yazım sonrası)** | `tool.execute.after` | Vault dosyası yazıldıysa anında taze oku |
| **Orta (her tur)** | `chat.message` | Cache'i düşür, `mtime` farkını çıkar |
| **Son** | `event` → `session.deleted` (+ `session.idle`) | Yeni veriyi `.ai/`'ye geri yaz |

**Kanıt (`dosya : satır`):**

```text
@opencode-ai/plugin/dist/index.d.ts :173  export interface Hooks {   <- hook arayuzu
                                :175  event?: (input: { event: Event }) => Promise<void>
                                :179  tool?: { [k]: ToolDefinition }  <- OZEL TOOL KAYDI
                                :187  "chat.message"?: ...
                                :249  "tool.execute.after"?: ...
@opencode-ai/sdk/dist/v2/gen/types.gen.d.ts
                                :583  type: "session.created"
                                :597  type: "session.deleted"
```

#### §3.2.2 Sınır — tetikleyici NEREDE bağlanamaz

| Yer | `hook` anahtarı | Sonuç |
|---|---|---|
| `C:\versacoder\opencode.json` | **YOK** (anahtarlar yalnız: `$schema`, `agents`) | Config'ten **bağlanamaz** |
| Global `opencode.json` / `.jsonc` | **YOK** (VAR: `plugin`, `command`) | Config'ten **bağlanamaz** |
| `opencode hook` CLI | **YOK** (subcommand listesinde yok) | Komutla **bağlanamaz** |
| `.opencode/{commands,skills}/*.md` | yok (yalnız metin) | Skill/komut **bağlayamaz** |
| **Plugin API** (`Hooks`) | **VAR** (`index.d.ts:173`) | **Tek geçerli yol** |

> **Sonuç:** 3 tetikleyici **yalnız bir OpenCode plugin'i** ile bağlanır.
> Konfigürasyon anahtarı, CLI komutu ve skill bu işi **yapamaz** (ölçüm).

#### §3.2.3 Compat yapısı (tazelik sözleşmesi)

| # | Kural | Değer |
|---|---|---|
| 1 | Cache | **YOK** — her çağrıda dosya diskten yeniden okunur |
| 2 | Tazelik anahtarı | `mtime` + bayt; değişmediyse yeniden yükleme |
| 3 | Zorunlu okuma | root 4 MD + `.ai/index.md` + `.ai/AGENTS.md` |
| 4 | Format | `/eli10` (özet) **ve** `/eli15` (derin) — birlikte |
| 5 | Geri yazma | `vault-writer` → `.ai/SESSIONS/` + ilgili `spec/TODOS/checklist` satırı |
| 6 | Bütçe | `.ai/` içinde **seçici** yükleme; komple yük **yasak** |

**Ölçüm (2026-10-01):**

| Kalem | Değer |
|---|---|
| Root 4 MD (`CLAUDE/AGENTS/README/WORKFLOW`) | **258 satır / 10.874 bayt** → başlangıçta sorunsuz |
| `.ai/` toplam Markdown | **503 dosya** → komple yükleme **imkânsız** |
| `.ai/CLAUDE.md` + `.ai/AGENTS.md` + `.ai/WORKFLOW.md` | 36.843 + 34.983 + 40.941 = **112.767 bayt** → tek başına bütçeyi aşar |
| `.ai/README.md` | **YOK** (`Test-Path = False`) → `[1]` adımında bu dosya **atlanır** |

> **UYARI:** `.ai/` içinde `README.md` **bulunmaz.** Kullanıcı varsayımıyla
> `[1]` adımı yazılamaz; adım `yok` olarak işaretlenir (G9).

---

## §4 Sınır (ne YAPMAZ)

- Oturum / kapı / commit **açılmaz**; kod **yazılmaz**.
- Model, provider veya varyant **elle atanmaz**.
- Jenerik 8 soru **sorulmaz**; cevapsız soru **tekrarlanmaz**.
- `.ai/SESSIONS/` **uydurulmaz** — dizin yoksa adım `[ ]` kalır (G9).
- Vault'ta karşılığı olmayan bileşen (Intent Router vb.) **varmış gibi
  yazılmaz**; durum sütunu `DURUM: TANIM YOK` olarak kalır.

---

## §5 Sonradan eklenecekler (kayıt listesi)

> **Bu bölüm bu işin TODO/TASK ayağıdır.** Her madde burada açılır; kapatıldığı
> zaman ilgili checklist/TODOS satırıyla eşleşir ve buradan `kapandi` olur.

| # | Eklenecek | Nereye yazılacak | Ön koşul | Durum |
|---|---|---|---|---|
| L-01 | `.ai/SESSIONS/` dizini + oturum kaydı | `TODOS/` + `checklists/gate-004a-*` | — | **açık** |
| L-02 | Intent Router tanımı (confidence boost + vault filtreleme) | `spec/` yeni dosya + `spec/index.md` satırı | L-01 | **açık** |
| L-03 | 50K token bütçe **ölçümü** (sayan mekanizma) | `spec/` + `.rules/performance-guidelines.md` | — | **açık** |
| L-04 | Prompt Maker TUI entegrasyonu (tuş + dialog) | `ui-design/` (hangi ekran?) + `.rules/ui-rules.md` | GATE-003 | **açık** |
| L-05 | Asking Questions modu: 50-100 adım versiyonu (tavan 10 ile çelişiyor) | **G10 çelişkisi** → ADR | kullanıcı kararı | **blokeli** |
| L-06 | `.ai/` ağacı düzeltmesi (18 dal → 15 gerçek klasör) | `index.md` §1 | — | **açık** |
| L-07 | Ana akış / SWARM / TARGET diyagramlarının master prompt kopyası | `.ai/prompts/` (kaynak: `engine.md` §1.1.1-3) | — | **açık** |
| L-08 | Explore ajanı 5-10 / max 100 isteği (AGENTS §5 = 3) | **G10 çelişkisi** → ADR | kullanıcı kararı | **blokeli** |
| L-09 | **`vault-loader`** — 3 nokta tazeleme plugin'i (`event` / `tool.execute.after` / `chat.message`), `/eli10 + /eli15` | **yeni TS plugin** + `opencode.json` → `plugin[]` + `npm i` | §3.2.2 (tek yol plugin API) | **açık** |
| L-10 | **`vault-reader` + `vault-writer`** — sorgu bazlı okuma + session sonu geri yazma (compat: `mtime`, cache yok) | `.opencode/plugin/` + `Hooks.tool` kaydı | L-09 | **açık** |

**Kural:** yeni madde bu tabloya eklenir; `spec/index.md`'ye **ayrı satır
açılmaz** (bu dosya zaten kayıtlıdır — SSP).

---

## §6 Açık sorular

| # | Soru | Not | Durum |
|---|---|---|---|
| Q-01 | Soru tavanı 10 mu, 50-100 mu? | L-05 ile aynı çelişki; repo skill `10`, dış şablon `50-100` | **DECISION BLOCKED** |
| Q-02 | `.ai/SESSIONS/` gerçek mi, `.ai/log.md` yeterli mi? | iki kayıt yeri olmasın (SSOT) | açık |
| Q-03 | Intent Router yoksa adım `[5]` neye dayanır? | "vault filtreleme" `.ai/index.md` §1.1 ile yapılabilir mi? | açık |
| Q-04 | L-04 hangi ekran? `05-ai-chat` mi, `01-start-page` mi? | GATE-003 sırası | açık |

---

## §7 Doğrulama

- [ ] §1'in 8 kuralı `.opencode/skills/prompt-maker/SKILL.md` ile birebir
- [ ] §2 footer'ı `AGENTS.md` §6 ile aynı iki satır
- [ ] §3.1 tablosundaki `Test-Path` / `0 eşleşme` çıktıları tekrar üretilebilir
- [ ] §5'te her satırın `Durum` alanı dolu (`açık` / `blokeli` / `kapandi`)
- [ ] §6'daki `DECISION BLOCKED` maddesi G10 için tek soruyla kullanıcıya sorulmuş
- [ ] Dosya `spec/index.md` §1'de kayıtlı (yeni satır **açılmaz**, mevcut satır güncellenir)
- [ ] Frontmatter **8 alan** (G4)
- [ ] Unicode kutu çizimi **0** (`U+2500-U+257F`) — yalnız `+ - |`

---

## §8 Bkz.

| Konu | Yer |
|---|---|
| Girdi hattının tam akışı | [[definitions/engine|engine.md]] §1.1.1 |
| Motor üretimi (üretim hattı) | [[definitions/engine|engine.md]] §1.1 |
| Gate zinciri | [[governance/workflow|WORKFLOW.md]] §1 |
| Kod yazım döngüsü 12 adım | [[governance/workflow|WORKFLOW.md]] §3.5 |
| Ajan matrisi / 3 paralel | [[definitions/agents|AGENTS.md]] §1, §5 |
| Prompt Maker skill'i | `.opencode/skills/prompt-maker/SKILL.md` |
