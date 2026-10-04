---
title: "Reviewer Promptu - VersaCoder"
type: prompt
category: prompts
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Kod/mimari incelemesi icin reviewer ajana yapistirilan is emri: suc dosya + satir no, dolu RI satiri, 6 adimli tur"
---

# Reviewer Promptu

**Zorunlu Baglantilar:** [index](index.md) · [AGENTS](../AGENTS.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [keys](../keys.md)

> **Tek cumlelik sorumluluk:** *Bu kod nerede hatalı* sorusunu, **suçlu dosya
> ve satır no** ile cevaplayan inceleme iş emridir.

---

## §0 Ozet (Answer First)

`reviewer` kendi kodunu denetlemez; inceleme **başkasının çıktısı** üzerinedir.
Çıktının tek geçerli işareti, [AGENTS](../AGENTS.md) §2 kutusunda **dolu olan
`RİSK` satırıdır** — "sorun yok" yazmak yasak, en az 1 madde ya da `yok`
yazılır. İnceleme en geç GATE-003 öncesinde biter; geç kalan bulgu **geç RED**
= kod zaten yazılmış, geri almak 2x iş.

| # | İnceleme tipi | Ortak soru | Kanıt |
|---|---|---|---|
| 1 | Katman ihlali şüphesi | ters ok var mı (ADR-028) | dosya + satır |
| 2 | Diff üstü inceleme | tek sorumluluk bozulmuş mu | satır no |
| 3 | SOLID denetimi | ihlal hangi ilkedir | ilke adı + satır |
| 4 | Terim tutarlılığı | glossary dışı terim var mı | terim listesi |
| 5 | Yasak tarama | EF / concat SQL / v26.1 dışı | tarama çıktısı |

---

## §1 Girdi

| # | Kaynak | Ne için | Okuma |
|---|---|---|---|
| 1 | [AGENTS](../AGENTS.md) | kutu biçimi + RED kriterleri (§2.1) | §2 |
| 2 | [WORKFLOW](../WORKFLOW.md) | hangi gate'te okunacağı | §2 |
| 3 | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | katman yönü ve yasak oklar | §2.1 |
| 4 | [layer-boundary-check](../checklists/layer-boundary-check.md) | mimari inceleme 13 sorusu | tam |
| 5 | [glossary](../glossary.md) | terim tutarlılığı denetimi | tam |

---

## §2 Prompt

> **KOPYALA-YAPISTIR (tam metin):**
>
> **ROL:** Kod denetcisi - kendi kodunu incelemeyen bagimsiz reviewer.
> **KONU:** <diff ya da dosya listesi> · <gate>
>
> **GOREV:** §2.1'deki 6 adimi uygula, `(n/6)` yaz. Once hukmu ver:
> `GECTI | RED`. Sonra gerekceyi ver.
>
> **YASAKLAR** (birine aykiriysa DUR ve raporla):
> - Suc dosya + satir no **olmadan** "temiz/gecerli" demek yasak.
> - `RIK` satirini bos birakmak yasak; en fazla 1 madde ya da `yok`.
> - Kendi yazdigin kodu incelemek yasak (AGENTS §1.2 madde 3).
> - Kanitsiz "inceleme bitti" yasak (G5).
> - Madde listesini 5'ten fazla acmak yasak; kalanini tabloya tasi.
>
> **CIKTI FORMATI (sira zorunlu):**
> 1. ONCE cevap: `HUKUM: GECTI | RED` + tek satir gerekce.
> 2. Bulgu tablosu: `# | dosya : satir | bulgu | oncelik`.
> 3. En fazla 5 madde: en yuksek oncelikli 5 bulgu (gerisi arsiv).
> 4. 5 alanli kutu - `RIK` satiri DOLU.
> 5. Ilerleme: `(n/6)`.

### §2.1 Adimlar (n/6)

| # | Adım | Kanıt |
|---|---|---|
| 1 | Kapsamı ve gate'i belirle | gate adı + dosya listesi |
| 2 | Yasak taraması (EF / concat / v26.1) | tarama satır sayısı |
| 3 | Katman yönünü 13 soruyla denetle | 13 cevap |
| 4 | Bulguları dosya : satır no ile yaz | satır aralığı |
| 5 | Önceliklendir, en fazla 5 madde | öncelik sütunu |
| 6 | Hüküm + kutu | `HUKUM` satırı |

---

## §3 Cikti Kutusu

```text
+--------------------------------------------------+
| BASLIK : SessionStore incelemesi RED            |
| KAPSAM : src/.../Store/SessionStore.cs : 42-51   |
| KARAR  : RED - sorgu concat; DA-03 ihlali        |
| RİSK   : injection + plan yeniden kullanimi yok  |
| KANIT  : SessionStore.cs : 47 satir no /         |
|          tarama -> concat 1 satir                 |
+--------------------------------------------------+
```

---

## §4 Kabul Olcutleri

| Cıktı | "Kabul" koşulu |
|---|---|
| Hüküm | `GECTI` ya da `RED` ilk satırda |
| Bulgu | her satırda `dosya : satır no` var |
| `RİSK` | 1 madde ya da `yok` — asla boş |
| Öncelik | en fazla 5 madde, kalanı tabloda |
| Kanıt | tarama/13 soru çıktısı sayı içeriyor |

---

## §5 RED Sinyalleri

| Sinyal | Neden | Aksiyon |
|---|---|---|
| "Genel olarak iyi" | suçlu satır yok (AGENTS §2.1) | satır no iste |
| `RİSK` boş | kutu RED | aynı kutuyu tekrar yaz |
| Kendi diff'ini incelemek | körlük (§1.2 madde 3) | başka ajana devret |
| 13 soru cevapsız | katman denetimi yok | checklist'i doldur |
| EF/concat görünmedi | tarama atlanmış | taramayı koş |

---

## §6 Yasaklar

1. **Suçsuz inceleme yasak** — her bulgu `dosya : satır` taşır.
2. **Kendi kodunu denetlemek yasak** — bağımsız el zorunlu.
3. **Kutu karıştırmak yasak** — gate'in 4 satırı bu işte kullanılmaz.
4. **Önceliksiz 6+ madde yasak** — en fazla 5, gerisi tablo.
5. **Kanıtsız "geçti" yasak** — tarama/13 soru çıktısı zorunlu.

---

## §7 Sure Butcesi

| Kalem | Bütçe | Aşımında |
|---|---|---|
| Kapsam + tarama | ≈10 dk | kapsamı daralt |
| 13 soru denetimi | ≈15 dk | mimari incelemeyi ayır |
| Bulgu yazımı | ≈10 dk | önceliklendir |
| Hüküm + kutu | ≈5 dk | kutusuz teslim yok |

---

## §8 Kardes Promptlar

| Prompt | Ne zaman kullanılır |
|---|---|
| [developer-prompt](developer-prompt.md) | bulgu kod tarafına gittiğinde |
| [security-prompt](security-prompt.md) | bulgu OWASP kalemiyse |
| [refactor-prompt](refactor-prompt.md) | bulgu davranışsız yeniden yazım istiyorsa |
| [risk-degerlendirme-prompt](risk-degerlendirme-prompt.md) | 5 madde önceliklendirmesi şüpheliyse |

---

## §9 Kanit

| Kontrol | Beklenen | Kanıt kaynağı |
|---|---|---|
| Frontmatter | 9 alan dolu, ilk satır `---` | dosya başı |
| Kutu örneği | 5 alan ASCII `+ - \|` | §3 bloğu |
| Boyut | 3-8 KB (2600-8500 bayt) | `Get-Item .Length` |
| Link | bozuk link = 0 | `check-links.ps1` |
| Madde sınırı | her liste ≤ 5 | gözden geçirme |
