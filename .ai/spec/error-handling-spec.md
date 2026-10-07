---
title: "Hata Yönetimi Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Hata sınıflandırma (VC-E-NNNN), taşıma biçimi (Result<T>), kullanıcıya gösterim ve kayit"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Hata Yönetimi Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [notification](notification-spec.md) · [observability](observability-spec.md)

> **Tek cümlelik sorumluluk:** *Hata nasıl adlandırılır, katmanlar arasında nasıl taşınır, kullanıcıya nasıl gösterilir ve nereye kaydedilir* sorusunun tek cevabıdır.
> Hata **fırlatmak yerine taşınır**: `Result<T>` + `Error` (l0 sözleşmesi); UI'da `VC-E-NNNN` + insanca mesaj görünür.

---

## §0 Özet (Answer First)

Her hata **kod** (`VC-E-NNNN`), **kategori** ve **insanca mesaj** taşır. Katmanlar arasında `Result<T>` ile yürür; **istisna yalnız beklenmeyen çökme** düzeyindedir ve UI katmanında yakalanıp günlüğe düşer. Gösterim [notification](notification-spec.md) kanallarıyla, kayıt l3 `ILogSink` ile olur; kullanıcıya **ham stack trace** gösterilmez (yalnız teşhis kimliği görünür).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Kod tablosu, kategoriler, seviye eşlemesi | Bildirim/toast gösterim kanalları | [notification](notification-spec.md) |
| 2 | Taşıma biçimi (`Result<T>` / `Error`) | `Result<T>` tipinin tanımı (l0) | [architecture](../architecture/index.md) |
| 3 | UI'da gösterim dili ve gizleme kuralları | Metin/şablon kaynakları | [localization](localization-spec.md) |
| 4 | Günlüğe yazım (seviye + alanlar) | Log sink'i / dosya biçimi | [observability](observability-spec.md) |
| 5 | Yeniden deneme kararı (transient/non-transient) | Retry'ın uygulanması (l3) | [protocol-spec](protocol-spec.md) |

---

## §2 Davranış

### §2.1 Kategori ve seviye

| # | Kategori | Örnek kod | Seviye | UI davranışı |
|---|---|---|---|---|
| 1 | Doğrulama (girdi) | `VC-E-0101` | bilgi | alan altında metin, toast yok |
| 2 | Kaynak/erişim (dosya, dizin) | `VC-E-0201` | uyarı | toast + hata merkezi |
| 3 | Ağ/sağlayıcı (timeout, 401) | `VC-E-0301` | hata | toast + "Tekrar dene" |
| 4 | Veri/kalıcılık (DB, bozuk JSON) | `VC-E-0401` | hata | hata merkezi + kurtarma |
| 5 | Güvenlik (izin, kaçış) | `VC-E-0501` | kritik | üçlü kanal + olay kaydı |
| 6 | Beklenmeyen (istisna) | `VC-E-0999` | kritik | teşhis kimliği + günlük yolu |

### §2.2 Taşıma ve yakalama

```
 l4/l5 hatasi --> Result.Fail(Error{Code,Message,Fields})
                        |
                        v
              l2 handler  -->  karar: geri don | retry (transient, 1 deneme)
                        |
                        v
                l7 UI     -->  notification kanali + log (ILogSink)
                        |
            (yalniz beklenmeyen istisna)
                UI yakalar --> VC-E-0999 + teşhis id  ->  log -> kapanis
```

### §2.3 Kurallar

| # | Kural | Kabul |
|---|---|---|
| H1 | Her hata **kodlu**dur; kodu olmayan hata üretilmez | kod tablosunda karşılığı var |
| H2 | Kullanıcıya **ham stack trace** gösterilmez; `TeşnisNo` görünür | ekranda `at ...` = 0 |
| H3 | `catch` bloğu **boş** (yutulmuş) olamaz | boş catch = 0 (G11) |
| H4 | Yeniden deneme yalnız **transient**, en fazla **1** (l3) | 2. deneme = 0 |
| H5 | Aynı hata 3 kez çözülmezse **3-STRIKE** escalation (protokol §8.1) | rapor şablonu zorunlu |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| İstisna tipleri | BCL | BCL (aynı adlar) | **fark yok** |
| `Exception.ToString()` biçimi | ayrıntılı | ayrıntılı (daha zengin) | gösterim **her ikisinde de maskelenir** |
| Ağ hatası sınıflandırması | ortak kod | ortak kod | fark yok |

> `#if` yok; hata mantığı ortak kaynaktır (G6 hedefi: 0).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Kod ↔ kategori eşlemesi | birim test | her koddaki en az 1 satır |
| 2 | Boş catch / yutulma taraması | statik tarama | 0 |
| 3 | Maskeleme (kullanıcıya gösterim) | birim test (UI katmanı) | stack trace = 0 |
| 4 | Retry sınırı | birim test (l3) | transient = 1 tekrar |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
