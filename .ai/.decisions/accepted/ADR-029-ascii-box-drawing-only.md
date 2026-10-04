---
title: "ADR-029 — Diyagramlar Saf ASCII (+ - |), Unicode Box-Drawing Yasak"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "vault belgelerindeki diyagram/kutu karakter seti: ASCII + - |, Unicode U+2500-U+257F yasagi, mermaid kuralinin devami"
---

# ADR-029 — Diyagramlar Saf ASCII (+ - |), Unicode Box-Drawing Yasak

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-020](ADR-020-no-mermaid-only-box-drawing.md) · [ascii-diagram-template](../../.templates/architecture/ascii-diagram-template.md)

> **Tek cümlelik karar:** Vault belgelerindeki tüm diyagram, ekran ve kutu
> anlatımları **yalnız ASCII `+`, `-`, `|`** ile çizilir; Unicode kutu çizim
> (`U+2500-U+257F`) yasaktır — ADR-020'nin karakter seti kararı (D1) bu ADR ile
> **superseded edilmiştir**; mermaid kuralı (ADR-020 D3: ikincil + yanındaki
> ASCII karşılığı zorunlu) **geçerliliğini korur** — karar verildi.

---

## §0 Özet (Answer First)

Diyagram birincil biçimi **ASCII düz metin kutudur**: `+` köşe/kesişim, `-` yatay,
`|` dikey. Unicode kutu çizim (`U+2500-U+257F`) 2026-10-01 ölçümünde **55 dosya /
766 satır** ihlal üretmişti; CLAUDE G4'ün (kullanıcı onaylı) işaretleme
**54 dosya / 14.120 karakter** ASCII'ye çevrildi ve kutu taraması **0 satır**
(§5). Ekran tel kafesleri (`ui-design/`) de bu standardın parçasıdır
(`ascii-diagram-template.md` 64 sütun, `wireframe-template.md` 66 sütun varyantı).
Mermaid yasak değildir ama **tek kaynak değildir**: yanında ASCII karşılığı yoksa
(ADR-020 D3) incelemede RED kalır.

---

## §1 Bağlam (Context)

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Kullanıcı kararı (2026-10-01): "ascii art diagram / screen view / ascii art box olacak" — ASCII kutu standarttır | `log.md` 2026-10-01 kayıtları (bu oturum) |
| C2 | CLAUDE G4 Unicode kutu çizim `U+2500-U+257F`'i RED sayar; yakalama = kutu taraması 0 satır | `CLAUDE.md` §1.1.4 · §2 komut 8 |
| C3 | ADR-020 D1 tersini dayatıyordu (UTF-8 box-drawing); çelişki ölçülüp kullanıcıya sunuldu | `log.md` #25 (55 dosya / 766 satır) |
| C4 | ASCII alfabesi tek şablonla sabit: `| - +` + oklar; genişlik 64 | `.templates/architecture/ascii-diagram-template.md` §Karakter Seti |
| C5 | Unicode kutu çizim bazı araçlarda bozuk bayta düşer, kopyala-yapıştırmada hizası bozulur (G4 gerekçesi) | `CLAUDE.md` §1.1.4 NEDEN |

---

## §2 Karar (Decision)

**Karar:** Diyagram **birincil biçimi ASCII `+ - |` düz metindir**; Unicode kutu
çizim (`U+2500-U+257F`) yasaktır — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Karakter seti | `+` (köşe/kesişim) · `-` (yatay) · `|` (dikey) — tek alfabe | C2 · C4 |
| D2 | Yasak | `U+2500-U+257F` aralığındaki tüm kutu çizim karakterleri · emoji · TAB | C2 |
| D3 | Oklar | `→ ← ↑ ↓ v` ve `->` izinli (G4 yalnız `U+2500-U+257F`'i yasaklar) | C4 · G4 |
| D4 | Mermaid | ADR-020 D3 devrede: **ikincil**, hemen yanında ASCII karşılığı zorunlu | ADR-020 D3 |
| D5 | Canonical şablon | `ascii-diagram-template.md` (64 sütun) · `wireframe-template.md` (66 sütun) | C4 |
| D6 | Denetim | kutu taraması → **0 satır**; kırık link / mojibake = 0 (G4 kapıları) | C2 |

### §2.2 Alternatifler

| # | Alternatif | Artı | Eksi | Neden elendi |
|---|---|---|---|---|
| A — ASCII `+ - |` (SEÇİLDİ) | her araçta aynı bayt; diff satır satır okunur; G4 ile uyumlu | köşe/kesişim elle hizalanır | hizalama şablon + doğrulama komutuyla garantilenir |
| B — Unicode box-drawing (ADR-020 D1) | kesişim tek karakterle doğru çıkar | G4 ihlali; bazı araçlarda bozuk bayt; ölçümden 55 dosya / 766 satır ihlal | G4 + kullanıcı kararı → elendi |
| C — mermaid tek kaynak | render'da temiz görünür | düz metin editörde kaybolur, denetlenemez | ADR-020 D3'te reddedildi (korunuyor) |

---

## §3 Sonuç (Consequences)

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| G4 kutu ihlali 0 | kutu taraması → 0 satır (§5) |
| Her araçta aynı görünür, kopya-diff temiz | düz metin; render gerekmez |
| ASCII + oklar tek alfabe = 500+ dosyada tutarlılık | `ascii-diagram-template.md` |
| ASCII karşılığı zorunlu → mermaid kaybolsa da içerik kalır | ADR-020 D3 |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Köşe hizası elle yapılır | şablon blokları kopyalanır + doğrulama komutu (ascii-diagram §5.3) |
| Estetik kayıp (düz köşeler) | oklar `→` izinli; vurgu tablo ile |
| Unicode kutuya alışmış okur | `wireframe-template.md` "Yasaklar" bölümü yönlendirir |

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-020](ADR-020-no-mermaid-only-box-drawing.md) | **supersedes** (D1) | karakter seti kararı buraya taşındı; mermaid D3 devrede |
| [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) | related | frontmatter + encoding kuralları (G4) |
| [ADR-025](ADR-025-gate003-ui-hard-stop.md) | related | wireframe ASCII tel kafes (D5) |
| [ADR-021](ADR-021-vault-ssot-single-index.md) | related | tek indeks / tek kayıt |

---

## §5 Kabul Kriterleri

- [ ] `U+2500-U+257F` kutu taraması → **0 satır** (`CLAUDE.md` §2 komut 8)
- [ ] Yeni diyagram/ekran blokları yalnız `+ - |` (+ izinli oklar) içeriyor
- [ ] Her mermaid bloğunun hemen altında ASCII karşılığı var (ADR-020 D3)
- [ ] `adr-index.md` defterinde: bu ADR `accepted`, ADR-020 `superseded` satırı dolu
- [ ] Frontmatter 8 alan (G4); `find-mojibake.ps1` → 0 dosya

---

## §6 Red Team (gerçek itiraz + cevap)

> **RED TEAM:** *"Unicode kutu çizim kesişimi tek karakterle doğru üretir; ASCII
> `+ - |` 500+ dosyalık vault'ta elle hizalama hatasına açık, estetik kayıp da
> var. Neden 'doğru görünen' yerine 'kırılgan görünen' seçildi?"*

**Cevap (rebuttal):** İtiraz iki noktada haklı, sonuçta yanlış. (1) "Kırılgan"
olçümle çürütüldü: sorun estetik değil **uyumluluk** — G4, Unicode kutunun bazı
araçlarda bozuk bayta düştüğünü ve kopyalamada hizasını bozduğunu define eder;
2026-10-01 ölçümünde asıl tutarsız taraf Unicode'tu (55 dosya / 766 satır ihlal).
(2) Hizalama riski `ascii-diagram-template.md`'deki doğrulama komutuyla **kanıt**
hâline getirilir (§5 madde 1: tarama 0 satır); hizasız blok yakalanır, uydurulmaz.
Estetik kayıp gerçek ama bedeli düşük: oklar `→` izinli, vurgu tabloya taşınır —
içerik kaybı yoktur.

**Değişiklik günlüğü:** 2026-10-01 — ilk yazım (kullanıcı kararı; kaynak:
CLAUDE G4 + ADR-020 + `log.md` 2026-10-01 ölçümleri).
