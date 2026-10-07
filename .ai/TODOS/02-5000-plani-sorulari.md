---
title: "TODOS Faz 02 — 5000+ Katmanlı Plan İçin 4 Soru"
type: todo
category: todos
status: active
version: 2.0.0
updated: 2026-10-07
authority: SSOT
scope: "5000+ maddelik katmanli mimari plani uretmek icin kullaniciya sorulacak 4 soru; cevap bekleniyor, vault isini bloklamaz"
---

# Faz 02 — 5000+ Katmanlı Plan Soruları

**Zorunlu Bağlantılar:** [TODOS index](index.md) · [Faz 01](01-vault-kapanis.md) · [VISION](../VISION.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [spec index](../spec/index.md) · [WORKFLOW](../WORKFLOW.md)

> **Tek cümlelik sorumluluk:** 5000+ maddelik katmanlı mimari planın hangi birimde,

### §0.1 Güncel Durum (2026-10-07 — ölçülmüş)

**Durum: BEKLIYOR — kullanici 4 cevabi**

| Gösterge | Değer | Hedef | Durum |
|---|---|---|---|
| Kapali fazi | 01 (RED) | 01 tamamlanmali | SIRA — 01 bitmeden acilmaz |
| Kaynak | ADR-030 (dosya YOK) | ADR-030 mevcut | DOĞRULAMA GEREKLİ |

> Kural: kullanici cevabi bekleyen is vault isini BLOKELAMAZ; 01 tamamlaninca 02 acilir (index §1).

> **Kaynak:** bu blok 2026-10-07 disk ölçümünden gelir (os.walk + git + dotnet); eski tarihli ölçümler tarihleriyle korunmuştur (append-only).

> nerede ve hangi derinlikte yazılacağını belirleyen 4 sorunun tek listesidir.

**Durum: BEKLİYOR** (kullanıcı cevabı) — ama **01 bitmeden sorulmaz** (sıra kuralı).

---

## §0 Özet (Answer First)

Plan hedefi: **5000+ madde**, katmanlı (l0-l7, ADR-028). Bunu yazmadan önce 4 bilinmeyen
kapatılmalı; aksi halde plan ya tekrar yazılır ya da yanlış yere yazılır. Sorular
**tek tek** sorulur (ROLE §3 tek soru kuralı); bu dosya sırayı saklar.

| # | Soru | Seçenekler | Varsayılan (öneri) |
|---|---|---|---|
| 1 | Planın birimi | tek dosya / faz başına dosya / maddenin kendisi task | **faz başına dosya** |
| 2 | Planın yeri | `.ai/plans/` / `spec/` / `reports/` | **`.ai/plans/` (yeni)** |
| 3 | İçerik kaynağı | vault SSOT + ADR-028 / port + G: arşiv / ikisi | **vault SSOT + port** |
| 4 | Derinlik + kapı | her madde checklist / %20 kapı / her faz sonu kapı | **her faz sonu kapı** |

---

## §1 Sorular (sırayla, tek tek)

### Soru 1 — Plan birimi ne olsun?

| Seçenek | Ne demek | Artı / Eksi |
|---|---|---|
| A | Tek dev dosya (5000 madde) | hızlı başlar / 300+ KB, band aşar (kök 88 KB) |
| **B** | Faz başına dosya (ör. 25 faz × 200 madde) | ölçülebilir, gate bağlanabilir / dosya sayısı artar |
| C | Her madde ayrı dosya | arşiv güzel / 5000 dosya, ölçülemez |

**Öneri B:** kök bandı (30-88 KB) ve `check-links.ps1` ile uyumlu.

### Soru 2 — Plan nereye yazılsın?

| Seçenek | Yol | Not |
|---|---|---|
| **A** | `.ai/plans/` (yeni dizin) | temiz; `index.md` + faz dosyaları |
| B | `spec/` | mevcut 40 dosya şişer; spec "ne olduğu", plan "ne yapılacak" |
| C | `reports/` | rapor = geçmiş; plan = gelecek, karışır |

**Öneri A:** SSOT bozulmaz (spec ≠ plan).

### Soru 3 — İçerik kaynağı ne olsun?

| Seçenek | Kaynak | Risk |
|---|---|---|
| **A** | vault SSOT (ADR-028, spec, .rules) + port kanıtı | en düşük |
| B | yalnız `C:\www\...\.ai` port + G: arşiv | tekrar, çelişki riski |
| C | A + web araştırma | doğrulama yükü (WEB GATE) |

**Öneri A:** uydurma bilgi yasak (G5); port zaten `C:\www`'da doğrulanmış.

### Soru 4 — Derinlik ve kapı (gate) ne olsun?

| Seçenek | Kural | Süre |
|---|---|---|
| A | her madde checklist | 5000 × kontrol = çok pahalı |
| **B** | %20'lik dilimler ara kapı | dengeli |
| C | yalnız faz sonu kapı | geç fark |

**Öneri B:** `checklists/` 31 dosya zaten var; ara kapılar mevcut checklist'e bağlanır.

---

## §2 Cevap Sonrası İş (faz 02'ye dönüş)

| # | Adım | Çıktı |
|---|---|---|
| 1 | 4 cevabı bu dosyaya **§4'e** işle | karar satırları |
| 2 | `.ai/plans/index.md` + faz iskeleti | plan dizini |
| 3 | Faz 03'e (iskelet) geç | sıra devamı |

- Cevap gelmeden **hiçbir plan dosyası yazılmaz** (yanlış varsayım = 2x iş).
- Vault işi (faz 01) bu beklemeyi **bloklamaz** — 01 paralel tamamlanır.

---

## §3 Kabul Kriterleri

- [ ] 4 soru **tek tek** sorulmuş (toplu anket yok)
- [ ] Her cevap §4'e yazılmış (sözlü kalmasın)
- [ ] Seçilen birim + yer + kaynak + kapı, `plans/index.md`'de ilan edilmiş
- [ ] Faz 01 tamamlanmış (sıra atlanmadı)

---

## §4 Cevaplar (doldurulacak)

| # | Soru | Cevap | Tarih |
|---|---|---|---|
| 1 | Plan birimi | **Her madde ayrı dosya** (C) — 2026-09-30'daki B cevabı bu kararla **iptal**; 5000 madde = 5000 dosya göze alındı | 2026-10-02 |
| 2 | Plan yeri | **.ai/plans/** (A - kullanici yetki devri: "nerede uygun gorsen oraya"; kalite: profesyonel senior standart) | 2026-09-30 |
| 3 | İçerik kaynağı | **Vault SSOT + port + web** (C: A + WEB GATE ile dış doğrulama) | 2026-10-02 |
| 4 | Derinlik + kapı | **%20'lik dilimler ara kapı** (B) | 2026-10-02 |

---

## §5 Kanıt

| # | Kanıt | Beklenen |
|---|---|---|
| P2-1 | §4 tablosu dolu | 4/4 cevap |
| P2-2 | `.ai/plans/index.md` | diskte, linkler geçerli |
| P2-3 | `check-links.ps1` | 0 bozuk link |
| P2-4 | `log.md` | cevapların tarihli kaydı |
