---
title: "Terminal Şartnamesi - VersaCoder"
type: spec
category: spec
status: active
version: 1.1.0
updated: 2026-10-02
authority: primary
scope: "CLI çıktısı yakalama, komut çalıştırma gösterimi, sanitization, timeout, iptal + VersaCoder CLI (VC-CLI-001..016) ile entegrasyonu, komut ağacı, exit code, geçmiş"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Terminal Şartnamesi — VersaCoder

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [testing-rules](../.rules/testing-rules.md) · [architecture](../architecture/index.md) · [versacoder-spec](versacoder-spec.md) · [tool-spec](tool-spec.md) · [error-handling](error-handling-spec.md) · [notification](notification-spec.md)

> **Tek cümlelik sorumluluk:** *Komut nasıl çalıştırılır, çıktısı satır satır nasıl yakalanır ve süreç ne zaman sonlandırılır* sorusunun tek cevabıdır.
> Terminal bir **dock görünümdür** (ayrı ekran değil — `versacoder-spec.md` §2 OQ-1); süreç başlatma l4'ün, komut güvenliği [tool-spec](tool-spec.md)'indir.

---

## §0 Özet (Answer First)

Görünüm: `DockManager` alt paneli içinde çıktı `MemoEdit` (**ReadOnly**) + komut girdisi `TextEdit` + durum alanı. Çıktı **satır bazlı** async okunur ve UI'ya artan eklenir. Çalışma dizini daima **proje köküdür**; komutlar `ArgumentList` ile gönderilir (string birleşimi yok, SEC-05). Süreç `timeout` ve kullanıcı iptali ile **tamamen** sonlandırılır; terminal "killed (timeout)" / "cancelled" yazar (VC-PRD-040..042).

---

## §1 Kapsam / Kapsam Dışı

| # | Kapsam içi | Kapsam dışı | Nerede |
|---|---|---|---|
| 1 | Komut satırı girdisi, geçmiş (↑/↓), çıktının gösterimi | Komutun güvenlik/izin sözleşmesi | [tool-spec](tool-spec.md) |
| 2 | Süreç durumu (çalışıyor/bitti/timeout) rozeti | Hata kodu sınıflandırma | [error-handling](error-handling-spec.md) |
| 3 | Iptal/timeout eylemi ve kullanıcıya bildirimi | Toast/bildirim biçimi | [notification](notification-spec.md) |
| 4 | Görünümün dock olarak yerleşimi ve temizlenmesi | Kabuk/pencere yönetimi | [ui-shell](ui-shell-spec.md) |
| 5 | Görev çıktısının Agent Panel'e taşınması | Görev kuyruğu | [task-queue](task-queue-spec.md) |

---

## §2 Davranış

### §2.1 Görünüm

```
+--------------------------------------------------------------+
| Terminal  ● running   cwd: C:\work\mysolution   [Stop] [Temizle]|
+--------------------------------------------------------------+
| $ dotnet build                                              |
|   VersaCoder.Core -> bin\Debug\net48\VersaCoder.Core.dll     |
|   Derleme başarılı. 0 Hata 0 Uyarı            1.9s          |
| $ ls                                                        |
|   src   tests   README.md                                   |
|   (imleç — çıktı satır akışı devam ediyor)                  |
+--------------------------------------------------------------+
| $ [komut yazın…                                ] [▶ Çalıştır]|
+--------------------------------------------------------------+
```

### §2.2 Çıktı yakalama

| # | Kural | Kabul |
|---|---|---|
| O1 | stdout ve stderr **ayrı** akıştan okunur, satır tamamlanınca eklenir | yarım satır UI'da görünmez |
| O2 | Ekran tamponu **2.000 satır** (ring); tamamı oturum dosyasında | bellek sınırlı |
| O3 | UTF-8; bozuk bayt U+FFFD olarak gösterilir, çökme yok | kodlama hatası = 0 çökme |
| O4 | Uzun süre sessiz çıkışta ilerleme göstergesi (G11) | "sessiz" durumu görünür |
| O5 | Süreç bittiğinde özeti satır olarak yazılır (`exit 0`, süre) | çıkış kodu görünür |

### §2.3 Komut ve süreç

| # | Olay | Davranış |
|---|---|---|
| C1 | Çalıştır | `WorkingDirectory` = proje kökü; argümanlar `ArgumentList` (SEC-05) |
| C2 | Tehlikeli kalıp | reddedilir + Türkçe mesaj; loglanır (SEC-05) |
| C3 | `Ctrl+C` / Stop | süreç ağzı kapatılır → 2 sn içinde yoksa kill |
| C4 | Timeout | süreç **tamamen** sonlandırılır; "killed (timeout)" |
| C5 | Bir komut sürerken ikinci giriş | pasif (tek süreç kuralı) |
| C6 | Kapanışta süreç açık | onay ile sonlandırılır; öksüz süreç bırakılmaz |

---

## §3 TFM Notu

| Konu | `net48` | `net10.0-windows` | Karar |
|---|---|---|---|
| Süreç başlatma | `ProcessStartInfo.ArgumentList` | aynı | **fark yok** |
| Konsol kodlaması | `StandardOutputEncoding = UTF8` | aynı | fark yok |
| Ağacı/dizin | BCL `Directory` | aynı | fark yok |

> Orak kaynak; `#if` yalnız Windows konsol API farkı gerekirse (hedef 0 — G6).

---

## §4 Test / Kabul

| # | Ne test edilir | Kanıt | Şart |
|---|---|---|---|
| 1 | Satır yakalama (kısmi + çok satırlı) | birim test (sahte süreç) | 0 kayıp satır |
| 2 | Timeout → süreç sonlanır | entegrasyon testi (temp dizin) | süreç sayısı 0'a iner |
| 3 | Sanitization | birim test | tehlikeli girdi reddi = 100% |
| 4 | Ring buffer sınırı | birim test | 2.000 satır üst sınırı |
| 5 | Bozuk link | `check-links.ps1` | 0 |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (GATE-004a batch).
