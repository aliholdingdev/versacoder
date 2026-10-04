---
title: "ADR-024 — CoreMusic Vault Dokunulmaz: Yapı Referansı, İçerik Değil"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "C:\\www\\coremusic.net\\.ai klasörünün statüsü: salt okunur, yalnız yapı referansı, içerik kopyalanamaz"
---

# ADR-024 — CoreMusic Vault Dokunulmaz: Yapı Referansı, İçerik Değil

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-002](ADR-002-handwritten-content-no-copypaste.md) · [ADR-003](ADR-003-vault-structure-500md-frontmatter.md)

> **Tek cümlelik karar:** `C:\www\coremusic.net\.ai` klasörü **kesinlikle dokunulmazdır (salt okunur)**; yalnız **yapı** referansı olarak okunur, **hiçbir içerik/metin kopyalanmaz** ve oraya **yazılmaz** — karar verildi.

---

## §0 Özet (Answer First)

Bu klasör **başka bir projeye** aittir: okumak serbest, dokunmak ve kopyalamak yasaktır. İki yasak ayrı ayrı anlamlıdır: **(1) yazma yok** — hedefte herhangi bir `write`/`edit`/`delete` işlemi G7 ihlali ve geri alınamaz hasar demektir; **(2) kopya yok** — içeriği alıp vault'a taşımak G1 ihlalidir. Öğrenilebilir olan **ölçü ve düzendir** (dosya sayısı, klasör adları, kural biçimi); örneğin "≈600 MD · kök 30–88 KB" ölçeği [ADR-003](ADR-003-vault-structure-500md-frontmatter.md)'ün çıpası olarak zaten böyle kullanıldı.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Başka vault'a yazma / oradan kopyalama yasağı (G1) | `CLAUDE.md` G1 · [ADR-002](ADR-002-handwritten-content-no-copypaste.md) |
| C2 | Vault ölçek çıpası bu klasörün **yerel envanterinden** alındı | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) Context |
| C3 | Kopya taraması cümle bazlı yapılır | [ADR-002](ADR-002-handwritten-content-no-copypaste.md) |
| C4 | Yıkıcı işlem öncesi onay (G7) — orada **onay da yetmez** | `CLAUDE.md` G7 · G11 |
| C5 | Aynı statüde ikinci referans: geçici opencode ağacı | [ADR-023](ADR-023-opencode-reference-readonly.md) |
| C6 | Bozuk link denetimi yalnız **bizim** vault'umuzu tarar | `scripts/check-links.ps1` |

---

## §2 Karar

**Karar:** `C:\www\coremusic.net\.ai` **dokunulmaz salt okunur referans**tır; yalnız **yapı/ölçü** okunur, **içerik kopyalanmaz**, hedefe **yazılmaz** — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Statü | **dokunulmaz** — `write`/`edit`/`delete`/`rename` **0 işlem** | C4 |
| D2 | Okuma | yalnız **yapı + ölçü** (dosya sayısı, KB aralığı, klasör adı, kural biçimi) | C2 |
| D3 | Kopya | metin/kod/prompt **kopyalanamaz** (tek cümle dahil) | C1 · C3 |
| D4 | Vault'a taşınan | yalnız **sayı ve gözlem** (ör. "600 MD hedefi"), biçim değil | C2 |
| D5 | Bağlantı | oraya **göreli link yazılmaz**; `.ai` vault linkleri yalnız kendi içimizde | C6 |
| D6 | İhlalde | G7/G1 ihlali → işlem **geri alınır**, `log.md`'ye kayıt, gerekirse kullanıcıya escalation | C1 · C4 |
| D7 | Emsal | geçici opencode ağacı da **aynı statüdedir** | C5 |

**Neden:** İki yasak birbirini tutmaz: yazma yasağı hedefi korur, kopya yasağı **bizim** kalitemizi korur. [ADR-002](ADR-002-handwritten-content-no-copypaste.md) zaten içeriği sıfırdan yazmayı şart koşuyor; bu ADR onun **dış ortam** ayağını kapatır. Ölçü/çıpa kullanmak kopya değildir — sayı uydurmak ise kanıtsızlıktır (G9); bu yüzden D4, "ölçüyü al, biçimi alma" der.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Başka projeye zarar riski 0 | hedefe yazma işlemi yok (`git status` orada) |
| Kopya yasağı ölçülebilir | cümle/kod taraması = 0 (ADR-002) |
| Ölçek çıpası korunur (ölçü kullanılabilir) | ADR-003 çıpası geçerli |
| Statü tek satırda açık | D1–D7 |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| İyi örnekleri görmek için okumak bazen cazip gelir | yalnız D2 kapsamındaki okuma; şüphe → `reviewer` |
| Referans vault tarihsiz kalabilir | tarih **ölçüm notu** olarak `reports/`'a yazılır |
| Okuma, bağlam bütçesini yer | `AGENTS.md` §6: kesit oku, dosya bütünü değil |
| Emsal statüler (D7) unutulabilir | bu ADR §4'te [ADR-023](ADR-023-opencode-reference-readonly.md) ile eşleştirildi |

**NEEDS_VALIDATION:** Klasörün **tam yolu ve mevcut durumu** (`Test-Path`) bu ADR yazılırken doğrulanmadı → ilk kullanımda `Test-Path` çıktısı `reports/`'a yapıştırılır. Yol değişse de **statü (D1–D7) değişmez**; yalnız `evidence` satırı güncellenir.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-002](ADR-002-handwritten-content-no-copypaste.md) | extends | kopya yasağının **fiziksel referans** ayağı |
| [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) | related | oradan alınan **ölçü çıpası** (yalnız sayı) |
| [ADR-023](ADR-023-opencode-reference-readonly.md) | related | aynı statüdeki ikinci salt okunur referans |
| [ADR-021](ADR-021-vault-ssot-single-index.md) | related | dış klasöre link yazılmaz |
| [ADR-022](ADR-022-git-commit-gate004.md) | related | depoya giren içeriğin kaynağı denetimi |

---

## §5 Kabul Kriterleri

- [ ] Hedef klasörde **0 yazma işlemi** (referans `git status` ile doğrulandı)
- [ ] Vault'ta hedef kökenli kopya cümle **0** (ADR-002 taraması)
- [ ] Hedefe **göreli link** yok (`check-links.ps1` bu klasörden raporlanmıyor)
- [ ] `evidence` içinde `Test-Path` sonucu yazılı (`NEEDS_VALIDATION` kapatıldı)
- [ ] `reports/` ölçüm notu tarihli (ölçü alındı, biçim alınmadı)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `CLAUDE.md` G1 + ADR-002/003.
