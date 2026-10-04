---
title: "ADR-021 — Vault Navigasyonu Tek `index.md`; Tekrar Yasak (SRP)"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "vault navigasyonunun tek giriş noktası, alt dizin index rolleri ve navigasyon tekrarının yasaklanması"
---

# ADR-021 — Vault Navigasyonu Tek `index.md`; Tekrar Yasak (SRP)

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) · [ADR-013](ADR-013-layered-architecture-52-nodes.md)

> **Tek cümlelik karar:** Vault'un **tek navigasyon girişi `.ai/index.md`**'dir; her dizinde **en fazla bir** `index.md` yaşar, içerik **kendi dizinine** aittir ve **hiçbir liste iki dosyada tekrarlanmaz** — karar verildi.

---

## §0 Özet (Answer First)

Aynı listenin iki dosyada olması, günün birinde ikisini farklı güncellemektir; o gün "hangisi doğru?" sorusu yanıtlanamaz. Bu ADR üç sınır koyar: **(1)** `.ai/index.md` tek giriş kapısıdır, **(2)** alt dizin `index.md`'leri yalnız **kendi** dizinini listeler (alt dizin özetine inmez), **(3)** durum/karar gibi **gerçek** taşıyan alanların tek sahibi vardır — ADR durumununki `adr-index.md`'dir. `keys.md` bir **arama haritasıdır**, ikinci navigasyon değildir.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Vault hedefi ≥500 MD; 14 kök belge zorunlu | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) §karar 1 |
| C2 | İki gerçek (SSUT) ihlali: aynı bilgi iki yerde = çelişen kural | `adr-index.md` §1.3 · `CLAUDE.md` (SSOT) |
| C3 | ADR **durumunun** tek kaynağı `adr-index.md`'dir (gövde yalnız yansıtır) | `adr-index.md` §1.3/1–2 |
| C4 | Link bütçesi: var olmayan hedef = bozuk link, GATE-004a kırmızısı | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) §karar 3 · `scripts/check-links.ps1` |
| C5 | Kök belge 30–88 KB bandındadır; navigasyon şişerse bant kırılır | [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) |
| C6 | `keys.md` "anahtar kelime haritası" olarak tanımlı | `keys.md` · [ADR-018](ADR-018-secret-store.md) D3 |

---

## §2 Karar

**Karar:** `.ai/index.md` **tek navigasyon girişidir**; dizin `index.md`'leri yalnız kendi dizinini listeler, **durum/gerçek** taşıyan alanların tek sahibi vardır ve **hiçbir liste iki dosyada kopyalanmaz** — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Tek giriş | `.ai/index.md` → tüm ana dizinlere bağlantı | C1 · C5 |
| D2 | Dizin index'i | yalnız **kendi** alt öğelerini listeler; alt-dizin özeti yazmaz | C2 |
| D3 | Tekrar yasağı | aynı tablo/liste iki dosyada = **SSUT ihlali**, incelemede RED | C2 |
| D4 | Alan sahipliği | ADR durum → `adr-index.md` · rol/yetki → `.agents/` · kural → `.rules/` · harita → `keys.md` | C3 · C6 |
| D5 | `keys.md` | ikinci navigasyon **değildir**; yalnız konu → dosya eşlemesi | C6 |
| D6 | Link | navigasyon linkleri yalnız **var olan** hedefe; yoksa hedef üretilir ya da link kaldırılır | C4 |

**Neden:** SRP'nin belgeye uygulanmış hâli şudur: bir dosyanın **tek sorusu** olmalı. `index.md` "nereye giderim" sorusunu, `adr-index.md` "hangi karar geçerli" sorusunu cevaplar; aynı sütunu ikisine yazmak ilk güncellemeyi unutulan ikinci bir görev yapar. D2, alt dizinlerin kök listeyi kopyalamasını engeller — 500+ dosyada bu kopyalar hızla bayatlar ve bozuk link üretir (C4).

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| "Nereye giderim" tek soru, tek cevap | `.ai/index.md` tek giriş |
| Bayat liste riski azalır | tekrar eden tablo = 0 (inceleme) |
| Bozuk link yüzeyi küçülür | navigasyon linkleri tek dosyada |
| Alan sahipliği net | D4 tablosu |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Kök `index.md` her part'te güncellenmeli | part DoD'sunda "index'e link eklendi" maddesi |
| Dizin index'i tek başına okunursa bütünlük eksik görünür | her dizin index'i köke link verir (D1) |
| `keys.md` ile `index.md` arasındaki sınır karışabilir | D5: `keys.md` = eşleme, `index.md` = gezinme |
| Yeni dizin eklenince index unutulabilir | `check-links.ps1` + `measure-vault.ps1` part sonunda |

**NEEDS_VALIDATION:** `.ai/index.md`'nin 14 kök belgeyi ve tüm ana dizinleri kapsayı kapsamadığı, GATE-004a öncesi `check-links.ps1` + elle karşılaştırma ile teyit edilir; eksik varsa **link eklenir, liste kopyalanmaz**.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) | extends | dosya/link/frontmatter kurallarına navigasyon kuralı ekler |
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | related | `architecture/` dizin index'i bu kurala uyar |
| [ADR-020](ADR-020-no-mermaid-only-box-drawing.md) | related | navigasyon diyagramının biçimi |
| [ADR-018](ADR-018-secret-store.md) | related | `keys.md` harita kapsamının sınırı |
| [ADR-023](ADR-023-opencode-reference-readonly.md) | related | dış vault'tan navigasyon **kopyalanmaz** |

---

## §5 Kabul Kriterleri

- [ ] `.ai/index.md` var, ilk satır frontmatter, tüm ana dizinlere link veriyor
- [ ] Aynı liste/iki tablo iki dosyada **tekrar etmiyor** (inceleme)
- [ ] Dizin `index.md`'leri yalnız kendi dizinini listeliyor
- [ ] `check-links.ps1` → bu dosyalardan kaynaklı bozuk link **0**
- [ ] `keys.md` bir gezinme listesi içermiyor (yalnız eşleme)

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: ADR-003 + `adr-index.md` §1.3.
