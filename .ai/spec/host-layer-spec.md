---
title: "VersaCoder — Host Katmanı Şartnamesi (l6 Host)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l6 (N38–N40): bootstrap pipeline sırası, tek DI kayıt noktası, config köprüsü ve açılış bütçesi (≤ 2 sn)"
---

# VersaCoder — Host Katmanı Şartnamesi (`spec/host-layer-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys](../keys.md) · [WORKFLOW](../WORKFLOW.md) · [architecture/index](../architecture/index.md) · [protocol-layer-spec](protocol-layer-spec.md) · [infra-layer-spec](infra-layer-spec.md) · [build-spec](build-spec.md)

> **Tek cümlelik sorumluluk:** *Uygulama ne sırayla açılır, hangi servisler nerede kaydedilir ve yapılandırma hangi köprüden tipli seçeneğe dönüşür* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

l6 = **tek giriş ve tek DI kapısı**: bootstrap pipeline (config → DB migration → provider → protocol → tool registry → UI/CLI) sırası sabittir, DI kaydı **yalnız** `di-registration` düğümünde yapılır, config `appsettings`/ortam değişkeninden **config-bridge** ile tipli `IOptions`'a gider; açılış bütçesi **≤ 2 sn** ve her adım başarısızsa uygulama **fail-fast** ile durur (sessiz devam yok).

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | N38 bootstrap-pipeline · N39 di-registration · N40 config-bridges | Servis **davranışı** (l4/l5 spec'leri) |
| 2 | Açılış sırası, hata durumunda durma kuralı, açılış ölçümü | Runtime mesaj döngüsü: [protocol-layer-spec](protocol-layer-spec.md) |
| 3 | DI yaşam süresi seçimi (`singleton`/`scoped`/`transient`) | Somut sağlayıcı kurulumu: [infra-layer-spec](infra-layer-spec.md) |
| 4 | Secret'ın açılışta çözülmesi (DPAPI/AES akışı) | Saklama biçimi: [security-spec](security-spec.md) |
| 5 | `#if` kullanımına izin verilen tek makro katman | `#if` kuralının kendisi: [build-spec](build-spec.md) |

> **YASAK:** DI kaydını forma/sınıfa dağıtmak; config'i kodda `Environment.GetEnvironmentVariable` ile dağınık okumak; açılışta kullanıcıya düşmeden devam eden **sessiz hata**; GATE-003 kapalıyken uygulama kodu çalıştırmak (G12).

---

## §2 Davranış

### §2.1 Bootstrap pipeline (sıra değişmez)

| # | Adım | Ne yapar | Başarısızsa |
|---|---|---|---|
| 1 | `config-bridges` | `appsettings.json` + ortam değişkeni + secret → tipli seçenek | Uygulama **açılmaz**, net hata |
| 2 | `di-registration` | Tek yerde servis kayıtları (l0..l6) | Çakışan kayıt = fail-fast |
| 3 | `migrations` | Saf SQL migration + checksum doğrulama | Uygulama **başlamaz** (migration-spec) |
| 4 | `provider` | DB/LLM sağlayıcı sırası kurulur (ADR-012 / ADR-011) | Uygulama **açılmaz** |
| 5 | `protocol + tool registry` | Şemalar ve tool listesi doldurulur | Çakışan tool adı = açılışta hata |
| 6 | UI / CLI | WinForms `Application.Run` ya da CLI komutu | — |

### §2.2 DI ve config köprüsü

```
  appsettings.json -+
  ortam degiskeni  -+--▶ IConfigBridge --▶ tipli secenekler --▶ DI (N39)
  secret store     -+        (N40)          (record / IOptions)     |
                                                                    v
                        tek kayit noktasi: AddVersaCoder(services)
                                  (N39 - baska yerde AddTransient YASAK)
```

| # | Kural | Denetim |
|---|---|---|
| 1 | Servis kaydı **tek metotta** toplanır; ikinci `Add*` noktası bulunursa RED | Kod taraması |
| 2 | Yaşam süresi **açıklamalı** seçilir: paylaşılan durum `singleton`, HTTP/DB factory `transient`, oturum işi `scoped` | Review notu |
| 3 | Config yalnız bridge'den okunur; ham anahtar/konfigurasyon kodda **geçmez** (SEC-01/02) | Tarama: literal anahtar = 0 |
| 4 | Aşağı katman (l0..l5) **host'a bakamaz**; yalnız port arayüzü görür | NetArchTest |
| 5 | Açılış adımları ölçülebilir (stopwatch) ve loglanır | Açılış logu ≤ 2 sn |

### §2.3 Açılış bütçesi

| Adım | Bütçe | Aşım davranışı |
|---|---|---|
| Config + DI | ≤ 200 ms | Log + devam (kritik değilse) |
| Migration | ≤ 500 ms (uygulanmamış yoksa) | Uzunsa migration logu rapora |
| Provider + registry | ≤ 300 ms | Fail-fast |
| UI/CLI ilk görünür | **toplam ≤ 2 sn** | `reports/`'a stopwatch satırı (GATE-004 kanıtı) |

---

## §3 TFM Notu

| Bacak | Durum | Not |
|---|---|---|
| `net48` | WinForms `Application.Run` + `App.config` köprüsü olabilir | `#if` yalnız host başlatma API farkında |
| `net10.0-windows` | `ApplicationConfiguration.Initialize()` akışı | Ortak pipeline **aynı** sırayla çalışır |
| Paylaşımlı | İki bacakta **aynı** DI seti | Farklı kayıt listesi = RED (G6) |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| HO-01 | Pipeline sırası | `StartupPipelineTests`: adım sırası sabit |
| HO-02 | Fail-fast: migration hatasında açılış durur | Sahte bozuk migration → uygulama başlamaz |
| HO-03 | DI tek yerde | İkinci kayıt noktası taraması = 0 |
| HO-04 | Açılış ≤ 2 sn | Stopwatch çıktısı `reports/` altına |
| HO-05 | İki bacak aynı DI listesi | Her iki TFM'de açılış testi yeşil |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
