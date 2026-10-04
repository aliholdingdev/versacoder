---
title: "ADR-020 — Diyagramlar UTF-8 Box-Drawing, Mermaid Varsa Yanında Karşılığı"
type: adr
category: decisions
status: superseded
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "vault belgelerindeki diyagram biçimi: UTF-8 kutu çizim karakterleri, mermaid bloğu ve istisnalar"
---

# ADR-020 — Diyagramlar UTF-8 Box-Drawing, Mermaid Varsa Yanında Karşılığı

> **SUPERSEDED (2026-10-01):** Bu ADR'nin **karakter seti kararı (D1, §2)**,
> [ADR-029](ADR-029-ascii-box-drawing-only.md) tarafından **ezilmiştir** — kutular
> ASCII `+ - |` ile çizilir, Unicode `U+2500-U+257F` yasaktır (kullanıcı kararı,
> CLAUDE G4). **Mermaid kuralı (D3) geçerliliğini korur.** `superseded_by` gövde
> notudur; 8 alan kuralı nedeniyle frontmatter'a alan eklenmez. Bu ADR arşiv
> kaydıdır, silinmez (adr-index §3.6).

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) · [ADR-013](ADR-013-layered-architecture-52-nodes.md)

> **Tek cümlelik karar:** Vault belgelerindeki tüm diyagramlar **UTF-8 box-drawing** karakterleriyle (`+ - | + +`) çizilir; bir yere **mermaid** bloğu konmuşsa **yanında düz metin/kutu karşılığı da zorunludur** — karar verildi.

---

## §0 Özet (Answer First)

Mermaid, çizim aracı olmayan editörde **boş kutu** olarak düşer ve içerik kaybolur; markdown okuyan her araç onu işleyemez. Bu yüzden diyagramın **birincil biçimi düz metin kutu çizimidir**: UTF-8, monospace, `find-mojibake.ps1`'den temiz. Mermaid **yasak değildir**, ikincil anlatımdır ve **her zaman yanında** okunabilir karşılığı bulunur. Bu ADR yalnız **vault belgelerini** kapsar; `ui-design/` wireframe'leri saf ASCII'dir (kapsam dışı, aşağıdaki tablo).

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | MD dosyası encoding ve link bütçesine tabidir; bozuk encoding = ihlal | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) §karar 2–3 · `CLAUDE.md` G4 |
| C2 | Encoding denetimi sıfır sonuç bekler | `scripts/find-mojibake.ps1` → 0 dosya hedefi |
| C3 | Vault'ta kutu çizimli diyagramlar zaten kullanılıyor (katman ağacı, akış) | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2.3 |
| C4 | Belge kalıbı "tablo **veya** 1 kutu-cizim diyagramı" ister | `.templates/new-doc-contract.md` §2 |
| C5 | Metin denetimi/kopya_diff'i düz metin üzerinden yürür | [ADR-002](ADR-002-handwritten-content-no-copypaste.md) |
| C6 | 500+ MD'de görsel tutarlılık, tek biçime bağlıdır | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) |

---

## §2 Karar

**Karar:** Diyagram **birincil biçimi UTF-8 box-drawing düz metindir**; mermaid bloğu yalnız **yanında karşılığı varsa** kabul edilir, aksi halde incelemede RED — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Karakter seti | `+ + + + - | + + + + + ▼ ▶` (UTF-8, BOM'suz) | C1 · C4 |
| D2 | Biçim | monospace blok (` ```text `), ≤ ~100 karakter genişlik | C6 |
| D3 | Mermaid | **izinli ama ikincil**; bloğun hemen altında aynı akışın düz metin karşılığı **zorunlu** | C5 |
| D4 | Kod bloğu içinde link **sayılmaz** | `check-links.ps1` zaten kod bloklarını atlar | `scripts/check-links.ps1` |
| D5 | Encoding | kutu karakterleri mojibake taramasından **temiz** çıkmalı | C2 |
| D6 | Kapsam dışı | `ui-design/NN-<ekran>.md` wireframe = **saf ASCII** (`AGENTS.md` §9.5) | — |

**Neden:** Diyagram, içeriğin **kendisidir**; araç bağımlı anlatım, 500+ dosyalık vault'ta tek bir renderer eksiğiyle tüm katman anlatımını siler. Kutu çizimi her düz metin editöründe aynı görünür ve diff'te **satır satır** okunur — mermaid düğüm adı değişince tüm blok kırmızıya döner. Mermaid'in tamamen yasaklanmaması (D3), görsel isteyen okur için kapıyı açık tutar ama **tek doğru** nu metinde bırakır.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Diyagram her araçta okunur, diff'lenebilir | düz metin; renderer gerekmez |
| Encoding denetimi temiz geçer | `find-mojibake.ps1` → 0 |
| Tek biçim → 500+ dosyada tutarlılık | `new-doc-contract.md` §2 kalıbı |
| Mermaid olsa bile içerik kaybolmaz | D3 zorunlu karşılık |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Kutu diyagramı elle hizalamak zahmetli | şablon blokları (`.templates/architecture/`) kopyalanıp uyarlanır |
| Geniş grafikler 100 karaktere sığmaz | derinlik sınırlanır; sığmıyorsa **iki tablo + ok** kullanılır |
| Mermaid + karşılık = iki kez bakım | karşılık **tek** anlatımdır; mermaid güncellenmezse incelemede RED |
| Bazı okurlar mermaid'i tercih eder | D3 ikinci anlatım olarak kalır |

**NEEDS_VALIDATION:** Kutu karakterlerinin **terminal + Obsidian + VS önizleme** üçünde aynı hizalanıp hizalanmadığı, ilk GATE-004a taramasında görsel olarak teyit edilir; fark çıkarsa şablon blok genişliği **90 karaktere** düşürülür ve bu ADR `version` bump ile güncellenir.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) | extends | MD biçim ve link kurallarına diyagram biçimi ekler |
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | related | kutu çizimli katman ağacının mevcut örneği |
| [ADR-002](ADR-002-handwritten-content-no-copypaste.md) | related | düz metin = kolay denetim, kopya taraması |
| [ADR-021](ADR-021-vault-ssot-single-index.md) | related | diyagramların tek girişten bağlanması |
| [ADR-010](ADR-010-cli-desktop-shared-core.md) | related | diyagramlar bu ADR'nin proje ayrımını anlatır |

---

## §5 Kabul Kriterleri

- [ ] `ui-design/` dışındaki yeni diyagramlar `+-|++` ile çiziliyor
- [ ] Her mermaid bloğunun altında düz metin karşılığı var
- [ ] `find-mojibake.ps1` → **0 dosya**
- [ ] Diyagram blokları ` ```text ` içinde (link taramasını bozmuyor)
- [ ] `ui-design/` wireframe'leri saf ASCII kaldı (kapsam dışı kontrol)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: ADR-003 + `new-doc-contract.md` §2.
2026-10-01 — `status: superseded` (D1 karakter seti); ezen: [ADR-029](ADR-029-ascii-box-drawing-only.md); D3 mermaid devrede.
