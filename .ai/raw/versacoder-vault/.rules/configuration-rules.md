---
title: "Yapılandırma Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Yapılandırma kaynağı, öncelik sırası, secret dışı kuralları, iki bacakta okuma, doğrulama ve değişiklik yetkisi"
---

# Yapılandırma Kuralları (`.rules/configuration-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [test-strategy](../spec/test-strategy.md) · [security-rules.md](security-rules.md) · [logging-rules.md](logging-rules.md) · [data-access-rules.md](data-access-rules.md)

> **Tek cümlelik sorumluluk:** *Değer nereden okunur, kimin önceliği kazandığı, hangi değerin kodda yasak olduğu ve değişikliğin kimin onayını gerektirdiği* sorusunun tek cevabıdır.
> Secret **gizliliği** [security-rules.md](security-rules.md) ile birlikte okunur; çelişirse güvenlik kazanır.

---

## §0 Özet (Answer First)

Yapılandırma **tek kapıdan** okunur: kod asla `Environment.GetEnvironmentVariable` / sabit string ile değer almaz, her şey `IConfiguration` benzeri tek bir yükleyiciye gider. Öncelik: **gizli (secret) > ortam değişkeni > yerel dosya > varsayılan**; secret **hiçbir zaman dosyada veya repo'da durmaz** (ADR-018). Her değer **başlangıçta doğrulanır** (eksik/geçersiz → uygulama **açılmaz**, çalışma ortasında `KeyNotFound` yok). Değiştirilebilir değer kodda **literal yazılmaz**; yeni yapılandırma anahtarı = yeni ADR + kullanıcı onayı (G8) gerekir; kural değişikliği yalnız `architect` onayıyla girer.

---

## §1 Kurallar

| # | Kural | İyi örnek | Kötü örnek | İhlalde kimi RED |
|---|---|---|---|---|
| C1 | Tek okuma kapısı | `cfg.Get<int>("Query:TimeoutSeconds")` | kod içi sabit `30` | `reviewer` |
| C2 | Koddaki literal yasak | `Options.Query.TimeoutSeconds` | `const int Timeout = 30;` | `reviewer` |
| C3 | Secret dosyada/repo'da durmaz | secret store / ortam değişkeni (ADR-018) | `appsettings.json` içinde `Password=` | `reviewer` (**CRITICAL**) |
| C4 | Öncelik sırası sabit | secret → env → dosya → varsayılan | dosyanın env'i ezmesi | `architect` |
| C5 | Başlangıçta doğrulama | yoksa **açılma hatası** + hangi anahtar | çalışma anında `KeyNotFoundException` | `architect` |
| C6 | Varsayılan kodda, override ortamda | `TimeoutSeconds` varsayılan 30 | ortam yoksa boş/çökme | `reviewer` |
| C7 | Bağlantı dizesi parçalanır | `Host` + `Port` + `Db` ayrı anahtarlar | tek string'i kodda birleştir | `reviewer` |
| C8 | Ortam adı sabittir | `Development` / `Production` | makineye özel `Prod2` | `architect` |
| C9 | İki bacakta aynı anahtar | `net48` ve `net10` aynı sözlüğü okur | bacak başına farklı anahtar | `architect` |
| C10 | Değişiklik kaydı | `log.md` + kapı gerekiyorsa gate | sessiz düzenleme | `architect` |

---

## §2 Öncelik ve Kaynak

| Sıra | Kaynak | Ne taşır | Repo'da mı? | Not |
|---|---|---|---|---|
| 1 | Secret store / ortam değişkeni | parola, API anahtarı, connection string parçası | **Hayır** | ADR-018; `sk-***` maskesi |
| 2 | Ortam değişkeni | ortam geçişi (URL, bayrak, seviye) | Hayır | CI/CD tarafından yazılır |
| 3 | Yerel yapılandırma dosyası | süre, eşik, yol, sayfa boyutu | Evet (secret **hariç**) | sürüm kontrolünde |
| 4 | Kod içi varsayılan | güvenli, değişmesi beklenmeyen başlangıç | Evet | en düşük öncelik |

**Değiştirme yasağı:** 1. ve 2. satırdaki değerleri **dosyaya kopyalamak** yasaktır; 3. satırda secret içeriği varsa repo taraması yakalar ve **CRITICAL**'dır.

---

## §3 Doğrulama (başlangıç)

| # | Doğrulama | Başarısızlıkta davranış |
|---|---|---|
| 1 | Zorunlu anahtarlar mevcut | **Açılmaz**; eksik anahtar adı loga (`level=ERROR`, ADR-026) |
| 2 | Tür / aralık (0 < timeout ≤ 300) | **Açılmaz**; hangi anahtar, hangi değer |
| 3 | Dosya okunabilir (UTF-8) | **Açılmaz**; yolu ve kodlamayı yaz |
| 4 | Secret değeri boş/placeholder (`CHANGE_ME`) | **Açılmaz** (secret-store kuralı, ADR-018) |
| 5 | Bilinmeyen anahtar | Uyarı (`WARN`) — silinmiş anahtarın izi |

```
baslangic
   |
   +-- dosya/env oku ----+--> dogrula (anahtar, tur, aralik)
                         |           |
                         |     basarisiz -> HATA + acilmaz (kullaniciya tek mesaj)
                         |           |
                         +----- gecerli -> tek configuration nesnesi (immutable)
                                                  |
                                                  v
                                    uygulama boyunca YALNIZ okuma
```

---

## §4 İki Bacakta Okuma

| Bacak | Mekanizma | `#if` kapsamı |
|---|---|---|
| `net48` (WinForms) | ortak `IConfiguration`-benzeri yükleyici + JSON okuma | yalnız taşıyıcı (ör. dosya yolu kökü) |
| `net10.0-windows` | aynı yükleyici, aynı anahtar sözlüğü | aynı |

**Kural (G6):** `#if` yalnız **taşıyıcı** içindir; anahtar adı, varsayılan ve doğrulama **ortaktır**. Farklı anahtar adı = G6 ihlali → `architect` RED.

---

## §5 İstisnalar (ADR gerekir mi?)

| # | İstisna durumu | ADR gerekir mi | Onay |
|---|---|---|---|
| I1 | Yeni yapılandırma anahtarı eklemek | Hayır — §1 C1–C5'e uyar | `architect` |
| I2 | Yeni kaynak eklemek (ör. uzak servis / vault) | **Evet** (G8, teknoloji + güvenlik) | kullanıcı |
| I3 | Secret'ı dosyaya geçici yazmak | **Yasak**, ADR yok | — |
| I4 | Ortam bazlı davranış (`if (env == "Prod")`) | **Evet** (davranış çatalı) | `architect` |
| I5 | Test için değer geçersiz kılmak | Hayır — test kendi kopyasını verir | `tester` |

---

## §6 Yasaklar

| # | Yasak | Karşılığı |
|---|---|---|
| 1 | Kodda literal süre/eşik/URL | C2 |
| 2 | Dosyada secret / connection string tam hâli | C3 · ADR-018 |
| 3 | Çalışma anında eksik anahtar istisnası | C5 |
| 4 | Ortam için `#if` ile kod çatallama | §4 (yalnız taşıyıcı) |
| 5 | Onaysız yeni kaynak/teknoloji | I2 · G8 |

---

## §7 Doğrulama

- [ ] Kodda literal yapılandırma değeri **0** (grep ile)
- [ ] Repo'da secret **0** (`sk-***`, parola, tam connection string)
- [ ] Eksik/geçersiz anahtarda uygulama **açılmıyor** ve hangi anahtar loglanıyor
- [ ] `net48` ve `net10` aynı anahtar sözlüğünü okuyor (`#if` yalnız taşıyıcı)
- [ ] `log.md`'de yapılandırma değişikliği kaydı var

---

## §8 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Secret store | `.ai/.decisions/accepted/ADR-018-secret-store.md` |
| 2 | İki bacak (TFM) | `.ai/.decisions/accepted/ADR-001-dual-target-net48-net10.md` |
| 3 | Yapısal log (hata çıktısı) | `.ai/.decisions/accepted/ADR-026-logging-structured.md` |
| 4 | Anayasa (G1..G12) | `.ai/CLAUDE.md` |
| 5 | Loglama kuralları | `.ai/.rules/logging-rules.md` |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `CLAUDE.md` G3/G8 + ADR-001/018.
