---
name: prompt-maker
description: Prompt Maker — prompt girisinde oturum baslatmadan Once proje-ozel sorular sorar (ASKING QUESTIONS modu), cevaplardan sonra promptu isler ve sablon ciktiyi uretir. Tetikler: "prompt maker", "/prompt-maker", "promptu isle", "promptu genislet".
authority: derived
tags: [archive]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Prompt Maker — VersaCoder

> Tek cumlelik sorumluluk: kullanici promptunu cevaplarla birlikte isleyip
> tek seferde sablon ciktiyi uretmek; oturum/sans acmaz, model sabitlemez.

## §0 Kapsam

| Durum | Yapilmaz |
|---|---|
| Prompt girisi alindi | Session/sans baslatilmaz, kod yazilmaz, gate acilmaz |
| Cevaplar toplandi | Prompt Maker pipeline'i calisir, cikti asagidaki sabtona gore |
| Is bitti | Cikti `✅ Prompt Cevaplarla İşlendi` satiriyla kapanir |

**Model/urun kurali:** is, OpenCode'un o an sectigi model ve provider ile calisir.
Model, provider veya varyant **elle atanmaz** (`model:` / `provider:` yazilmaz);
secim OpenCode'un, kullanici degistirirse kullananicinindir.

---

## §1 MOD A — ASKING QUESTIONS (Once calisir)

1. Prompt alinir, **oturum acilmaz**; hemen soru moduna gecilir.
2. Sorular **proje-ozeldir**; jenerik 8 soru (ne, neden, kim, nere, ne zaman,
   nasil, ne kadar, hangi) **yasaktir**.
3. Soru kaynagi: `.ai/index.md` → `.ai/PROJECTS.md`, `.ai/VISION.md`,
   `.ai/spec/`, `.ai/.decisions/adr-index.md`, `.ai/glossary.md` + varsa
   proje kodu. Soru, vault'ta cevabi **olmayan** boslugu hedefler.
4. Sorular **teker teker** sorulur; her soru A/B/C seceneklidir, cevap
   beklemeden digerine gecilmez.
5. Durdurma kosulu: vault + kod uzerinde belirsizlik kalmadiginda ya da
   kullanici "yeter / devam" dediginde. Varsayilan tavan: **10 soru**.
6. Her soru ve cevapi §5'teki tabloya yazilir; cevaplanan soru tekrar sorulmaz.

Soru bicimi:

```text
Soru [n]: <proje-ozel, tek satir>
A) <secenek>   B) <secenek>   C) <secenek>
```

---

## §2 MOD B — Prompt Maker (cevaplardan sonra)

1. `.ai` vault'u ve proje kodu taranir (§3 ve §4 icin referanslar).
2. Prompt `/eli10` mantigiyla **paragraf halinde** genisletilir (madde madde
   degil, anlasilir tek paragraf); jargon aciklanir.
3. Gerekirse arastirma yapilir; iddialar kanitlanir (G5: kanitsiz iddia yok).
4. Istenen is gorevlere ayrilir; her gorevin kanit kriteri yazilir.
5. Cikti §4'teki sablona **takim sirasiyla** yazilir. Sablon disina cikilmaz.

Uzun master prompt/PICCO uretimi gerekiyorsa ek olarak okunur:
`~/.config/opencode/skills/prompt-maker/SKILL.md` (global motor; bu skill
onu golgelemez, isterse besler).

---

## §3 Referans tarama zorunlulugu

| Blok | Kaynak | Zorunlu mu |
|---|---|---|
| Vault referansi | `.ai/index.md` üzerinden ilgili `.ai/**` dosyalari (spec, ADR, rules, prompts) | Evet — en az 3 bag |
| Kod referansi | Mevcut proje dosyalari (gercek yol + satir no) | Evet — yoksa "yok (henuz kod yok)" yazilir |
| Kural kontrolu | `.ai/.rules/index.md` eslesmesi | Evet |

---

## §4 Cikti sablonu (sira degistirilemez)

```markdown
## 1. Genisletilmis Prompt (/eli10 paragraf)

<orijinal prompt, paragraf halinde, jenerik terimler aciklanmis sekilde>

## 2. Istenen Sey

<ne istendigi, tek madde madde liste, maks 5 madde>

## 3. .ai Vault Referanslari

- .ai/<dosya> : <neden bu dosya>  (en az 3 bag)

## 4. Proje Kodu Referanslari

- <yol> : <satir> : <ilgi>   ya da   "yok (henuz kod yok)"

## 5. Kullanici Kararlari · Sorular · Cevaplar

| # | Soru | Cevap (A/B/C veya serbest) |
|---|---|---|
| 1 | ... | ... |

## 6. Orijinal Prompt (birebir)

<orijinal prompt, aynen, degistirilmemis>

## 7. Gorev Ayristirma

| # | Gorev | Kanit |
|---|---|---|
| 1 | ... | ... |

✅ Prompt Cevaplarla İşlendi
Dil: Türkçe | Görev Sayısı: [n] | Cevap: [n]
```

**Kabul kriterleri:** 7 baslik sirasiyla var · §5 tablosu soru sayisi =
cevap sayisi · §6 birebir · footer'in iki satiri **aynen** · `[n]` sayilari
gercek sayidir · Markdown kutu cizimleri ASCII (`+ - |`), `U+2500-U+257F` yok.

**RED:** oturum/sans acildi · model elle atandi · jenerik 8 soru soruldu ·
footer eksik/degistirilmis · referans bloklarindan biri bos.
