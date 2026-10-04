---
title: "Görev Kuyruğu Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Sıradaki görevlerin sırası, öncelik kuralları, çalıştırma/pause/iptal yaşam döngüsü ve kalıcılık"
---

# Görev Kuyruğu Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [session-spec](session-spec.md) · [agent-panel](agent-panel-spec.md) · [notification](notification-spec.md)

> **Tek cümlelik sorumluluk:** *Bir görev ne zaman kuyruğa girer, hangi sırayla çalışır, nasıl duraklatılır/iptal edilir* sorusunun tek cevabıdır.
> Durumun **gösterimi** [agent-panel](agent-panel-spec.md)’tedir; durumun **kaydı** [session-spec](session-spec.md)/[persistence](persistence-spec.md) üzerindedir.

---

## §0 Özet (Answer First)

Kuyruk **tek çalıştırıcıdır**: aynı anda en fazla **1** `running` görev (eşzamanlılık = kullanıcı ayarı, varsayılan 1). Sıra: **öncelik (High > Normal > Low)** → eklenme zamanı (FIFO). Durum makinesi `created → running → paused → completed | failed`; geçersiz geçiş **reddedilir** (VC-PRD-021). İptal 2 sn içinde sonucu üretir ve yarım tool çağrısını kilitli bırakmaz (VC-PRD-022). Kuyruk durumu diske yazılır; uygulama kapanınca `running` görevler `paused` işaretlenir (VC-PRD-085).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Sıralama, öncelik, çalıştırma politikası | Paneldeki sütun/filtre görünümü | [agent-panel](agent-panel-spec.md) |
| 2 | Durum makinesi + geçersiz geçiş reddi | Oturum/transcript içeriği | [session-spec](session-spec.md) |
| 3 | Pause/Resume/Cancel gecikme sözleşmesi | Tool şeması ve izin onayı | [tool-spec](tool-spec.md) |
| 4 | Kuyruğun kalıcı olması ve kurtarma | Kayıt yolu/formatı | [persistence](persistence-spec.md) |
| 5 | "Görev bitti/başarısız" bildirimi tetiği | Toast/bildirim biçimi | [notification](notification-spec.md) |

---

## §2 Davranış

### §2.1 Durum makinesi

```
        +----------- iptal -----------+
        v                            |
 [created] --baslat--> [running] --+--> [completed]
                  ^        |       |
                  |        +--> [paused] --devam--> [running]
                  |                            |
                  +-------- tekrar ------------+
                                             v
                                          [failed]  (+ hata kodu)
```

### §2.2 Sıra ve öncelik

| # | Kural | Değer |
|---|---|---|
| Q1 | Öncelik sırası | `High > Normal > Low` |
| Q2 | Aynı öncelikte | FIFO (eklenme zamanı) |
| Q3 | Aynı anda çalışan | varsayılan **1**; ayar ile 2'ye kadar (`NEEDS_VALIDATION`) |
| Q4 | `paused` görev sırayı **işgal etmez** | arkadaki görev başlar |
| Q5 | Öncelik değiştirme | yalnız `created`/`paused` görevde; `running`'de yok |

### §2.3 Yaşam döngüsü gecikmeleri

| # | Geçiş | Sözleşme | Aşılırsa |
|---|---|---|---|
| L1 | `created → running` | ≤ 500 ms (atama) | uyarı + tekrar kuyruğa |
| L2 | `running → paused` | ≤ 2 sn | `failed(cancelled)`'a düşer |
| L3 | `running → completed/failed` | tool çağrısı bitişi | — |
| L4 | Tekrar çalıştır (resume) | tamamlanan adım **tekrarlanmaz** | transcript tutarsızlığı = RED |
| L5 | Çökme kurtarma | `running` → `paused` + otomatik işaretleme | öksüz `running` = 0 |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Zamanlayıcı/kuyruk | `System.Threading` + ortak kod | aynı | **fark yok** |
| Kalıcılık | SQLite (ADO.NET + Dapper) | aynı | fark yok |
| İptal token'ı | `CancellationToken` | aynı | fark yok |

> Kuyruk çekirdeği ortak kaynaktır; `#if` yok (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Sıralama (öncelik + FIFO) | birim test (sabit `IClock`) | 5/5 sıra doğru |
| 2 | Geçersiz geçiş reddi | birim test | `completed → running` = 0 kabul |
| 3 | İptal gecikmesi | birim test + kronometre | ≤ 2 sn |
| 4 | Kurtarma (kapanışta `running`) | entegrasyon | `paused` işaretlenir |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
