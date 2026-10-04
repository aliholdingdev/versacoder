---
title: "Bildirim Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Toast/balon bildirim, hata merkezi (birikimli liste) ve sessizleştirme kuralları"
---

# Bildirim Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [error-handling](error-handling-spec.md) · [observability](observability-spec.md) · [agent-panel](agent-panel-spec.md)

> **Tek cümlelik sorumluluk:** *Bir olay kullanıcıya nasıl bildirilir, hatalar nerede biriktirilir ve hangi bildirim susturulur* sorusunun tek cevabıdır.
> Bildirim **gösterimdir**; hata kodu üretimi [error-handling](error-handling-spec.md), kaydı [observability](observability-spec.md) kapsamındadır.

---

## §0 Özet (Answer First)

Üç kanal vardır: **toast** (`AlertControl`, sağ-alt, 4 sn), **hata merkezi** (birikimli `GridControl` listesi, rozetli) ve **durum çubuğu** tek satır mesajı. Toast yalnız **kullanıcı eyleminin sonucu** ve **görev bitişi** için kullanılır; arka plan olaylarında sessizce hata merkezine düşer. Aynı hata 30 sn içinde tekrarlanırsa **birleştirilir** (sayaç +1). CRITICAL seviyesi toast + hata merkezi + durum çubuğu üçlüsünü birlikte kullanır.

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Toast/balon gösterimi, süre, tıklama | Hata kodu ve seviye üretimi | [error-handling](error-handling-spec.md) |
| 2 | Hata merkezi listesi (birikim, filtre, temizle) | Log dosyası / metrik | [observability](observability-spec.md) |
| 3 | Durum çubuğu mesajı | Kabuk yerleşimi | [ui-shell](ui-shell-spec.md) |
| 4 | Bildirim ayarları (sessiz saat, kapatma) | Ayarların kalıcılığı | [settings](settings-spec.md) · [persistence](persistence-spec.md) |
| 5 | Görev bitiş bildirimi tetiği | Görev durum makinesi | [task-queue](task-queue-spec.md) |

---

## §2 Davranış

### §2.1 Kanal matrisi

| # | Seviye | Toast | Hata merkezi | Durum çubuğu | Süre |
|---|---|---|---|---|---|
| 1 | Bilgi (işlem tamam) | ✓ | — | ✓ | 3 sn |
| 2 | Uyarı (yenileme gerekli) | ✓ | ✓ | ✓ | 4 sn |
| 3 | Hata (`VC-E-NNNN`) | ✓ | ✓ | ✓ | 5 sn |
| 4 | CRITICAL | ✓ (onaylı) | ✓ | ✓ | kapanana kadar |
| 5 | Arka plan sessiz olay | — | ✓ | — | — |

### §2.2 Akış

```
 olay (l2 sonucu)
      |
      +-- kullanıcı eylemi mi? -- EVET --► AlertControl (toast)
      |                                          |
      +-- arka plan ---------------------+       | tıkla
                                        ▼       ▼
                              Hata Merkezi ◄--  ilgili ekran/açıklama
                              (GridControl, filtre + temizle)
                                        |
                                        ▼
                              Dosya günlüğü (l3 ILogSink)
```

### §2.3 Kurallar

| # | Kural | Kabul |
|---|---|---|
| B1 | Aynı hata 30 sn içinde tekrar → **birleştirilir** (×N) | 10 hata = 1 toast + ×10 |
| B2 | Toast'lar **kuyruk** (en fazla 3 görünür, kalanlar birikir) | üst üste yığılma yok |
| B3 | Hata merkezi oturum boyunca yaşar; **Temizle** ile boşaltılır | yeniden açılışta 0 (kayıt logdadır) |
| B4 | Kullanıcı bir hatayı "artık gösterme" edebilir | aynı kod tekrar sessiz |
| B5 | Bildirimler ses **çıkarmaz**; DPI'da kesilme yok | erişilebilirlik (DX-14) |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Toast yüzeyi | DevExpress **v26.1** `AlertControl` | aynı | **fark yok** |
| Zamanlayıcı (kapanış) | `Timer` (UI thread) | aynı | fark yok |
| Bildirim kutusu | WinForms `NotifyIcon` balon (yalnız arka planda) | aynı | fark yok |

> Bildirim kodu `#if` içermez (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Seviye → kanal eşlemesi | birim test | 5 satır da doğru |
| 2 | Birleştirme penceresi (30 sn) | birim test (sabit `IClock`) | ×N sayacı doğru |
| 3 | Toast kuyruğu (3 üst sınır) | birim test | yığılma yok |
| 4 | Kritik olay üçlü kanalı | smoke | 3/3 görünür |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
