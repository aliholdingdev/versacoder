---
title: "VersaCoder — Dağıtım Şartnamesi (Masaüstü + CLI)"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Masaüstü dağıtımı (MSI / ClickOnce / taşınabilir notlar) ve CLI dağıtımı — kurulum, güncelleme, geri alma davranışı"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Dağıtım Şartnamesi (`spec/deployment-spec.md`)

**Zorunlu Bağlantılar:** [index](../index.md) · [CLAUDE.md](../claude.md) · [keys](../governance/keys.md) · [WORKFLOW](../governance/workflow.md) · [architecture/index](../architecture/index.md) · [release-spec](release-spec.md) · [packaging-spec](packaging-spec.md) · [migration-spec](migration-spec.md)

> **Tek cümlelik sorumluluk:** *Masaüstü uygulaması ile CLI hangi araçla, hangi makineye ve nasıl geri alınarak dağıtılır* sorusunun tek cevabıdır.
> CI matrisi/rollback adımları `.rules/deployment-guide.md` §2–§4'tedir (tekrar yok); bu belge **dağıtım kanalı seçimi ve kurulum davranışı**dır.

---

## §0 Özet (Answer First)

Masaüstü için **üç kanal** değerlendirilir ve **tek biri** seçilir: **MSI (varsayılan kurulum), ClickOnce (otomatik güncelleme), taşınabilir klasör (taşınabilir/TEST)**; CLI **aynı artifact içinde ayrı klasör** olarak dağıtılır. DevExpress v26.1 runtime DLL'leri çıktının yanında gelir, DB migration **kurulum sonrası ilk açılışta** uygulanır, geri alma = önceki sürüm artifact'i + `--rollback`.

---

## §1 Kapsam / Kapsam Disi

| # | Kapsam | Kapsam dışı (nerede) |
|---|---|---|
| 1 | Masaüstü kanal seçimi (MSI / ClickOnce / taşınabilir) | Artifact üretimi: [release-spec](release-spec.md) |
| 2 | CLI dağıtımı ve ilk çalıştırma | Paket kilitleri: [packaging-spec](packaging-spec.md) |
| 3 | Kurulum sonrası ilk açılış (secret + migration) | Migration davranışı: [migration-spec](migration-spec.md) |
| 4 | Güncelleme ve geri alma | CI adımları: `.rules/deployment-guide.md` §2 |
| 5 | Runtime ön koşulları (.NET Framework 4.8 / .NET 10) | Açılsın mı sorusu değil, **nasıl** sorusu |

> **YASAK:** DevExpress v26.1 dışı runtime · `net48` çıktısını .NET 10 runtime'ında çalıştırmak · uygulama dışında elle migration · geri alma plansız dağıtım.

---

## §2 Davranış

### §2.1 Kanal seçimi

| Kanal | Ne için | Artı | Eksi | Karar |
|---|---|---|---|---|
| **MSI** | Genel kurulum | Sessiz kurulum, sürüm yükseltme | Güncelleme elle | **Varsayılan** |
| **ClickOnce** | Otomatik güncelleme | Kullanıcı güncel kalır | imzalama/URL yönetimi | OPSİYONEL (NEEDS_VALIDATION) |
| **Taşınabilir** | USB / test makinesi | Kurulum yok | Manuel güncelleme | TEST/DAĞITIM için |
| `dotnet publish` klasörü | Destek ekibi | Hızlı | Kullanıcıya uygun değil | Yalnız destek |

### §2.2 Dağıtım akışı

```
  artifact (release-spec §2.3)
        |
        +-- masaustu:  MSI kurulumu --▶ kisayol --▶ ilk acilis
        |                     |                     |
        |                     |          [1] secret prompt (yoksa)
        |                     |          [2] migration uygula (migration-spec)
        |                     |          [3] <= 2 sn acilis (B7)
        |                     v
        |              guncelleme: MSI upgrade (ayni PackageCode ailesi)
        |
        +-- CLI:       publish/<tfm>/cli/ --▶ PATH'e ekle --▶ VersaCoder.Cli --help
```

### §2.3 Kurulum öncesi/sonrası kontrol

| # | Kontrol | Aşılırsa |
|---|---|---|
| 1 | .NET Framework 4.8 (`net48` bacağı) kurulu mu | Kurulum açıkça uyarı verir |
| 2 | .NET 10 runtime (`net10` bacağı) | Eksikse kurulum **durur** (sessiz devam yok) |
| 3 | DevExpress v26.1 DLL'leri çıktıda | Eksik = kurulum RED |
| 4 | `sql/` migration dosyaları mevcut | İlk açılış migration'ı uygular |
| 5 | Geri alma | Önceki artifact indirilebilir + rollback notu release'te |

### §2.4 CLI dağıtımı

| # | Kural | Not |
|---|---|---|
| 1 | CLI, masaüstü artifact'inden **ayrı** klasörde (`cli/`) | GUI'siz makine |
| 2 | İki bacakta da aynı komut yüzeyi | `--help` aynı metni döner |
| 3 | Yapılandırma kullanıcı dizininde (secret hariç, güvenli) | `.gitignore` kapsamı |
| 4 | İlk çalıştırma `--doctor` benzeri ön-kontrol | .NET/DB/izin durumu raporlar |
| 5 | Sürüm bilgisi `--version` = release etiketi | `release-spec` §2.1 ile aynı numara |

---

## §3 TFM Notu

| Bacak | Dağıtım biçimi | Not |
|---|---|---|
| `net48` | MSI + makineye .NET Framework 4.8 | Self-contained **değil** |
| `net10.0-windows` | MSI / klasör + .NET 10 runtime (`win-x64`) | RID yalnız bu bacakta |
| İkisi birlikte | **Tek** MSI içinde iki bacak dosya ağacı ya da iki ayrı MSI | Karar: `NEEDS_VALIDATION` → `architect` |

---

## §4 Test / Kabul

| ID | Ne test edilir | Kanıt |
|---|---|---|
| DP-01 | MSI temiz makinede kurulur ve açılır | Kurulum + açılış ≤ 2 sn log'u |
| DP-02 | İlk açılışta secret prompt + migration | Temiz profil testi |
| DP-03 | Sürüm yükseltmesi (MSI upgrade) | Eski sürüm → yeni sürüm, veri kaybı yok |
| DP-04 | Rollback | Önceki artifact ile geri dönüş + veri sağlığı |
| DP-05 | CLI `--help` / `--version` | İki bacakta aynı çıktı |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect, GATE-004a batch).
