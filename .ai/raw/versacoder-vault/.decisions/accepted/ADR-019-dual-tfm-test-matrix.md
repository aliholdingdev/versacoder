---
title: "ADR-019 — İki Bacak Build + Test Matrisi Zorunlu"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "net48 × net10.0-windows için build/test matrisi, hangi testin hangi bacakta koştuğu ve kanıt zorunluluğu"
hedef_bacak: "net48 + net10.0-windows"
evidence: "ADR-001 (iki bacak) · ADR-012 (SQLite yalnız net10 test) · spec/test-strategy.md · CLAUDE.md G5"
---

# ADR-019 — İki Bacak Build + Test Matrisi Zorunlu

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [spec/test-strategy](../../spec/test-strategy.md) · [ADR-001](ADR-001-dual-target-net48-net10.md)

> **Tek cümlelik karar:** Her değişiklik **iki bacakta da** `dotnet build` ve `dotnet test` ile doğrulanır; **tek bacakta yeşil = tamamlanmış değildir**, istisnalar yalnız bu ADR'deki matriste yazılıdır — karar verildi.

---

## §0 Özet (Answer First)

İki bacak şartı ([ADR-001](ADR-001-dual-target-net48-net10.md)) yazıda ucuz, **kanıtta pahalıdır**: `net48`'de derlenmeyen bir paket `net10`'da sorunsuz gözükür ve ancak üretimde patlar. Bu ADR tek sayfalık matrisi kilitler: **her test sınıfı iki bacakta koşar**, tek istisna SQLite entegrasyonudur (`Microsoft.Data.Sqlite` yalnız `net10.0-windows`, [ADR-012](ADR-012-dual-tfm-data-provider.md) D3). Kanıt zorunludur: `dotnet build` + `dotnet test` çıktıları **sayılarıyla** rapora yazılır; "geçiyor" yazımı G5 ihlalidir.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | İki bacak zorunlu: `net48` + `net10.0-windows`, ortak kaynak | [ADR-001](ADR-001-dual-target-net48-net10.md) |
| C2 | "Bitti" iddiası build/test çıktısız yasak | `CLAUDE.md` G5 |
| C3 | SQLite sağlayıcısı yalnız `net10.0-windows` entegrasyon testlerinde (D4) | [ADR-012](ADR-012-dual-tfm-data-provider.md) D3–D4 |
| C4 | Mimari kuralı (katman yönü, ters bağımlılık) NetArchTest ile doğrulanır | `spec/test-strategy.md` §3 |
| C5 | Test kapsamı/coverage kararı `tester` yetkisinde | `AGENTS.md` §3.2/6 |
| C6 | `#if` yalnız platform/API farkı olabilir | `CLAUDE.md` G6 |

---

## §2 Karar

**Karar:** Build + test **her iki bacakta** zorunludur; matris aşağıdadır ve **istisna yalnız matriste yazılıdır**, tek bacakta yeşil sonuç **kabul edilmez** — karar verildi.

| # | İş / test türü | `net48` | `net10.0-windows` | Not / dayanak |
|---|---|:---:|:---:|---|
| M1 | `dotnet build` | ✅ zorunlu | ✅ zorunlu | 0 hata, her iki bacak |
| M2 | Birim test (I/O'suz, deterministik) | ✅ | ✅ | aynı test kaynağı iki bacakta |
| M3 | Entegrasyon test (gerçek şema) | — | ✅ | **istisna**: SQLite yalnız `net10` (C3) |
| M4 | Mimari test (NetArchTest katman yönü) | ✅ | ✅ | ters bağımlılık 0 ihlal (C4) |
| M5 | UI form açılış testi | ✅ | ✅ | DevExpress v26.1 (ADR-004/015) |
| M6 | Kanıt | ✅ | ✅ | sayılar raporda; "geçiyor" yazımı G5 (C2) |

**Ek kararlar:**

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Komut | `dotnet build -f net48` · `dotnet build -f net10.0-windows` · `dotnet test` (her bacak) | C1 |
| D2 | Red eşiği | bacaklardan **herhangi biri** kırmızıysa iş `reviewer`'dan geçmez | C2 |
| D3 | İstisna dile getirilir | test kapsam dışıysa **gerekçe + `NEEDS_VALIDATION`** yazılır | `AGENTS.md` §3.2/6 |
| D4 | `#if` | yalnız platform/API farkı; test mantığı `#if` içinde **olmaz** | C6 |

**Neden:** Matrisin kendisi **sözleşmedir**: hangi testin hangi bacakta koştuğu baştan yazıldığından "o testi net10'da koştum, yeter" tartışması başlamaz. M3 istisnası **gizlenmez**, tabloya yazılır — gizli istisna zamanla unutulur ve güvenlik açığı üretir. Tek bacak kırmızıyken PASS vermeme (D2), kanıt disiplinini inceleme aşamasında zorunlu kılar.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| İkinci bacak kırığı erken yakalanır | `net48` build hatası = iş durur |
| İstisnalar yazılı ve sınırlı | M3 tek istisna, gerekçeli |
| Kanıt sayısal | build/test çıktısı raporda (`0 hata` / `6 geçti · 1.4 sn`) |
| Kapsam tartışması başlamadan biter | matris tek sayfa, `spec/test-strategy.md` ile uyumlu |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Her iş için **2× build + 2× test** süresi | paralel koşu + [ADR-014](ADR-014-slnx-and-cpm.md) tek çözüm ile tek komut |
| SQLite istisnası `net48` veri yolunu kör bırakır | `net48` veri testleri sahte sağlayıcı/SqlServer (ADR-012 D4) |
| Bazı paketler yalnız bir bacakta desteklenir | restore/build erken (PART 5); fark = **NEEDS_VALIDATION** |
| Matris elle güncellenirse bayatlar | matris `spec/test-strategy.md` içinde tek kaynak |

**NEEDS_VALIDATION:** M3 istisnası dışındaki tüm testlerin **gerçekten** `net48`'de koşabildiği, ilk tam test turunda (GATE-005) kanıtlanır; koşamayan test listesi `reports/`'a yazılır ve gerekçesiyle matris'e işlenir.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-001](ADR-001-dual-target-net48-net10.md) | extends | iki bacak kararının **kanıt** kısmını kurar |
| [ADR-012](ADR-012-dual-tfm-data-provider.md) | related | M3 istisnasının sağlayıcı gerekçesi |
| [ADR-016](ADR-016-no-ef-core-restated.md) | related | katman sınırının test edildiği yer |
| [ADR-014](ADR-014-slnx-and-cpm.md) | related | matrisin tek komutta koşulduğu çözüm |
| [ADR-027](ADR-027-component-catalog-5000.md) | related | "testli" durumunun katalog kanıtı |

---

## §5 Kabul Kriterleri

- [ ] Tek komutla iki bacak build = 0 hata (çıktı raporda)
- [ ] `dotnet test` her iki bacakta çalışıyor; istisna **yalnız** M3
- [ ] NetArchTest = 0 ihlal (her iki bacak)
- [ ] Tek bacak kırmızıyken `reviewer` PASS **vermiyor** (inceleme kaydı)
- [ ] Matris `spec/test-strategy.md` ile birebir aynı

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: ADR-001 + ADR-012 + `CLAUDE.md` G5/G6.
