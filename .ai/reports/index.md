---
title: "Rapor İndeksi — VersaCoder reports/"
type: index
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "14 dosyalik reports/ klasorunun navigasyonu: her raporun tek satirlik ozeti, sorusu ve okuma sirasi"
---

# Rapor İndeksi (reports/)

**Zorunlu Bağlantılar:** [CLAUDE](../CLAUDE.md) · [WORKFLOW](../WORKFLOW.md) · [keys](../keys.md) · [VISION](../VISION.md) · [architecture/index](../architecture/index.md)

> **Tek cümlelik sorumluluk:** Vault'taki hangi rapor hangi soruyu cevaplar sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

`reports/` altında **14 dosya** var: 13 konu raporu + bu indeks (2026-09-30 dizin ölçümü). Okuma sırası gate'e göre: `00` → `01` → `02` → `03` → `04` → `05` → `06` → `07` → `08` → `10` → `11` → `09`; `k3-618-ham-envanter` yalnız mimari tartışmada başvurulur. Raporda geçen sayılar (216/500 MD, 46 bozuk link, 9 mojibake, 5440 kayıt) **2026-09-25 ölçümünün kaydıdır** — bugün geçerliyse yeniden ölçülür.

---

## §1 Rapor Tablosu (navigasyon)

| # | Dosya | Tek satır özet | Ne zaman açılır |
|---|---|---|---|
| 1 | [00-kapsam-analizi.md](00-kapsam-analizi.md) | Kaynak okuma çıktısı: girdi envanteri, 5 senior karar, 52 düğüm katman kararı, GATE-001/002 kanıtları (v1.1.0) | kapsam/karar ararken |
| 2 | [01-web-kaynaklari.md](01-web-kaynaklari.md) | WEB GATE: her karar ≥1 birincil kaynak URL + erişim tarihi; `net48` + `net10.0-windows` ikili bacak DevExpress v26.1 ile doğrulandı | teknoloji iddiası doğrulanacaksa |
| 3 | [02-vault-kurulum-raporu.md](02-vault-kurulum-raporu.md) | PART 3 / GATE-004a kurulum kanıtları: 52 düğüm dosyası, 5440 kayıt PASS, 46 bozuk link; gate açık kaldı | gate durumu sorulunca |
| 4 | [03-ui-design-raporu.md](03-ui-design-raporu.md) | `ui-design/` 8/8 paketin durumu: README + 7 wireframe, GATE-003 onayı BEKLİYOR; checklist 0/13 | wireframe durumu sorulunca |
| 5 | [04-proje-iskeleti-raporu.md](04-proje-iskeleti-raporu.md) | 9+2 proje iskeleti kararları; bu vault'ta `src/` = 0 → durum HENÜZ UYGULANMADI | iskelet sorulunca |
| 6 | [05-katman-kodlama-raporu.md](05-katman-kodlama-raporu.md) | l0..l7 kodlama sırası ve kanıt zinciri; uygulama başlamadı → DOĞRULAMA GEREKLI | kodlama planı sorulunca |
| 7 | [06-opencode-uyarlama-raporu.md](06-opencode-uyarlama-raporu.md) | AGENTS §5.1 kaza kaydı (8 paralel → 3), model `opencode/mimo-v2.6-flash-free`, strike kuralı §4.3 | ajan politikası sorulunca |
| 8 | [07-dogrulama-raporu.md](07-dogrulama-raporu.md) | Betik doğrulama: check-links 0, find-mojibake 0, MD ölçüm kaydı; tarihli ölçümler | doğrulama istenince |
| 9 | [08-risk-kayit-defteri.md](08-risk-kayit-defteri.md) | R1..R10 risk tablosu: 3 kapandı, 1 kısmi, 6 açık; her satırda tek sahip + kapanış kanıtı | risk/öncelik sorulunca |
| 10 | [09-teslim-raporu.md](09-teslim-raporu.md) | PART 8 / GATE-006: 8 part'tan 2'si kapandı, P3 kısmi, P4..P8 başlamadı; kanıtsız "bitti" yok | teslim durumu sorulunca |
| 11 | [10-bilinen-eksikler.md](10-bilinen-eksikler.md) | Açık eksiklerin tam listesi: 46 bozuk link, 9 mojibake, 216/500 MD, 3 NEEDS_VALIDATION (2026-09-25 ölçümü — tazelik bkz. `07`) | neyin eksik olduğu sorulunca |
| 12 | [11-sonraki-adimlar.md](11-sonraki-adimlar.md) | Sıradaki işlerin sırası, sahibi, süresi: GATE-004a kapanışı ≈2 sa, kalan toplam ≈5 sa 55 dk | iş planı istenince |
| 13 | [k3-618-ham-envanter.md](k3-618-ham-envanter.md) | K3 9 Tier / 618 DLL listesinin özeti + bilinen hatalar; kataloga girmez (`needs-validation`, `derived`) | mimari tartışmada |
| 14 | `index.md` (bu dosya) | rapor navigasyonu — her raporun tek satır özeti | ilk giriş |

---

## §2 Okuma Sırası (gate'e göre)

| # | Aşama | Önce oku | Ne alırsın |
|---|---|---|---|
| 1 | GATE-001 / 002 | `00` + `01` | kapsam kararları + kaynaklı doğrulama |
| 2 | GATE-004a | `02` + `10` | kurulum kanıtları + açık eksikler |
| 3 | önceliklendirme | `08` + `11` | sahipli riskler + sıralı işler |
| 4 | GATE-006 teslim | `09` | part/gate durumu + kalan işler |
| 5 | mimari tartışma | `k3-618-ham-envanter` + [architecture/index](../architecture/index.md) | kapsam dışı envanter + 52 düğüm |

---

## §3 Kapsam Kuralları

1. **Tek satır özet + link.** Rapordan alıntı burada tekrarlanmaz (SRP, G12); içerik kendi dosyasında yaşar.
2. **Yeni rapor = bu tabloya 1 satır.** Dosya adı `NN-konu.md` (kebab-case, ASCII), numara atlanmaz.
3. **Eksik numara normaldir.** `03..07` bu klasörde yok (üretilmedi); arada boşluk varsa `10-bilinen-eksikler.md`'ye bakılır.
4. **Sayı tarihlidir.** Ölçüm içeren cümlede tarih yoksa o sayı yeniden ölçülmeden kullanılmaz.
5. **Karar burada verilmez.** Mimari karar [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) içindir; rapor yalnız kanıt taşır.

---

## §4 Doğrulama

| # | Kontrol | Beklenen |
|---|---|---|
| 1 | Tabloda 9 satır = klasörde 9 dosya | 9 = 9 |
| 2 | Her link hedefi diskte var | bozuk link 0 |
| 3 | Frontmatter 8 alan + `updated: 2026-09-29` | 8/8 |
| 4 | Emoji, Unicode kutu çizimi, mojibake | 0 |
| 5 | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | 0 kırık |

- [ ] Her satırda tek satır özet + okuma zamanı var
- [ ] Rapor içeriği bu dosyaya kopyalanmadı
- [ ] Yeni rapor eklendiğinde §1 tablosu ve §0 sayımı güncellendi

---

## §5 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Kapılar ve fazlar | `.ai/WORKFLOW.md` | §2 okuma sırası |
| 2 | Kanıt ve etiket kuralları | `.ai/CLAUDE.md` | G5 (kanıtsız "bitti" yasak) |
| 3 | Anahtar haritası | `.ai/keys.md` | rapor anahtarları |
| 4 | Mimari indeks | `.ai/architecture/index.md` | 52 düğüm / N sayım birimi |
| 5 | Şablon envanteri | `.ai/.templates/index.md` | rapor şablonu kaydı |
