---
title: "VersaCoder — Yapılandırma Şablonu"
type: template
category: template
status: active
version: 1.0.0
updated: 2026-09-25
authority: reference
scope: "yapilandirma sablonu: app settings, secim siniflari ve baglama kurallari"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# VersaCoder — Yapılandırma Şablonu

**Zorunlu Bağlantılar:** [[.templates/index]] · [[.templates/CLAUDE]] · [[../architecture/ascii-diagram-template]] · [[../adr/adr-architecture-template]] · [[./provider-template]] · [[../session-log-template]]

> **Tek cümlelik sorumluluk:** Bu dosya, VersaCoder'da **bir değerin nerede yaşadığı, hangi sırada ezildiği ve nasıl doğrulandığı** sorusunun tek kaynağıdır (SSOT). Gömülü ayar, sürümü olmayan paket ve doğrulamasız başlatma reddedilir.

---

## §1 Amaç

### §1.1 Neden bu şablon zorunlu

Yapılandırma sessizce bozulur: bir ortam değişkeni unutulur, bir `Condition` atlanır, bir paket sürümü açıkta bırakılır ve derleme yine yeşil kalır — sonuç ancak üretimde (veya `net48` hedefinde) görünür. Bu şablon üç şeyi tek yerde dayatır: **öncelik sırası** (§3.1), **MSBuild rol ayrımı** (§3.3), **fail-fast** (§3.8).

| # | Bu şablon neyi zorunlu kılar | Kırılırsa ne olur |
|---|---|---|
| 1 | Tek öncelik sırası (§3.1) | Aynı değer makineden makineye farklı okunur |
| 2 | MSBuild rol ayrımı (§3.3) | Dosyalar birbirini ezer, "props neden uygulanmıyor" tartışması |
| 3 | Koşullu özellik kuralı (§3.4) | `net48` derlenir ama çalışma zamanında kırılır |
| 4 | Tip-güvenli options pattern (§3.6) | Eksik anahtar çalışma zamanında değil, derlemede fark edilmez |
| 5 | Secret ≠ config (§3.7) | Repo'da açık anahtar |
| 6 | Fail-fast doğrulama (§3.8) | Uygulama eksik ayarla açılır, sonra kaos |
| 7 | Sürüm kilidi (§3.9) | DevExpress 25.2.8 dışına kayma → ADR-001 ihlali |

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

Kanıt `2026-09-25` tarihinde `C:\versacoder` üzerinde okunarak alındı. Kanıt yoksa **uydurulmaz**.

| # | İddia | Durum | Kanıt |
|---|-------|-------|-------|
| E1 | Depo `C:\versacoder` var, git'li | ✅ DOĞRULANDI | `git status` → `?? .ai/`, `.git` klasörü mevcut |
| E2 | Depo içeriği = `.ai/` + `.git/` | ✅ DOĞRULANDI | `Get-ChildItem -Force` → 2 girdi |
| E3 | `Directory.Packages.props` (CPM) | ❌ YOK | `Test-Path = False` — kurulacak |
| E4 | `Directory.Build.props` | ❌ YOK | `Test-Path = False` — kurulacak |
| E5 | `global.json` (SDK 10.0.301 / latestFeature) | ❌ YOK | `Test-Path = False` — beyan, doğrulanmadı |
| E6 | `.gitignore` | ❌ YOK | `Test-Path = False` — kurulacak |
| E7 | `.NET SDK 10.0.401` kurulu | ✅ DOĞRULANDI | `dotnet --list-sdks` → `10.0.401` |
| E8 | DevExpress **25.2.8** NuGet önbellekte | ❌ YOK | `~/.nuget/packages/devexpress*` = 0 girdi (ADR-001 kilidi beyanı) |
| E9 | `*.csproj`, `appsettings*.json` | ❌ YOK | Recursive sayım = 0 / 0 |
| E10 | `.ai/.templates/index.md` `#24 configuration-template` (Part B, `T-VER`, devops-engineer) | ✅ OKUNDU | envanter 38.816 bayt, §3.3 satır #24 |
| E11 | `ascii-diagram-template.md` (biçim referansı) | ✅ OKUNDU | 38.121 bayt / 644 satır |
| E12 | `adr/adr-template.md` | ❌ YOK | `adr/` altında böyle dosya yok |
| E13 | `.ai/.templates/infrastructure/` klasörü | ❌ YOK | `Test-Path = False` — bu iş kapsamında açıldı |
| E14 | `.ai/architecture/index.md`, `.ai/.rules/`, `.ai/keys.md`, `.ai/log.md` | ❌ YOK | `.ai/` içinde tek girdi: `.templates/` |
| E15 | Bu dosya yazıldı | ✅ BU DOSYA | `.ai/.templates/infrastructure/configuration-template.md` |

**Yorum yasağı:** Var olmayan dosya için "şurada ayarlanmış" denmez; satır `⚠️ VERIFICATION REQUIRED` olarak kalır. §1.2'de `❌ YOK` olan her dosya için bu şablon **hedef şablondur** (iskelet §3.9'da).

### §1.3 Yapılandırma Türü → Öncelik Eşlemesi

| # | Tür | Tipik Değer | Bu Şablonda? | Öncelik (§3.1) |
|---|-----|-------------|--------------|----------------|
| 1 | Uygulama ayarı (DB yolu, timeout) | `appsettings*.json` | ✅ Tam | 3–4 |
| 2 | Secret (API key, bağlantı dizesi parolası) | env / user-secrets / DPAPI | ✅ Tam | 1–2 |
| 3 | Ortam adı | `Development` \| `Production` | ✅ Tam | — (seçici) |
| 4 | Derleme özelliği (TFM, TreatWarningsAsErrors) | `Directory.Build.props` | ✅ Tam | MSBuild |
| 5 | Paket sürümü (CPM) | `Directory.Packages.props` | ✅ Tam | MSBuild |
| 6 | SDK sürümü | `global.json` | ✅ Tam | CLI |
| 7 | Çalışma zamanı komut satırı argümanı | `--katalog ./db` | ✅ Tam | 1 (en yüksek) |
| 8 | Kod varsayılanı (`??` / initializer) | son savunma hattı | ✅ Tam | 5 (en düşük) |
| 9 | Veritabanı şeması / migration | — | ❌ | migration-template |
| 10 | Sağlayıcı adaptör ayarı (endpoint/model) | — | Kısmen | provider-template §3.5 |

### §1.4 Neden Öncelik Sırası Tek Kaynak Olmalı

**Önemli:** "Farklı dosyada, farklı sıra" en pahalı yapılandırma hatasıdır. VersaCoder'da tek sıra vardır, §5.3 ile **kanıtlanır** ve yalnız bu dosyada değişir; diğer dosyalar sırayı **tekrar yazmaz**, yalnız buraya bağlanır (SRP, G12).

---

## §2 Kapsam

**İçinde:** değer yaşam döngüsü (katman + öncelik), MSBuild dosyaları (`Directory.Build.props`, `global.json`, `Directory.Packages.props`), koşullu özellik + ikili TFM, ortam adları, options pattern, secret + `.gitignore`, fail-fast, sürüm kilidi, salt-okunur komutlar.
**Dışında:** sağlayıcı adaptörü (→ [[./provider-template]]), veri erişimi/şema (ADR-004), ADR kararı (→ adr şablonları), ekran/UI (GATE-003).

### §2.1 Dosya Tipi → Sorumlu Eşlemesi

| # | Dosya / Konu | Sorumlu Rol | Bu Şablondan mı? |
|---|--------------|-------------|-------------------|
| 1 | `appsettings.json`, `appsettings.Production.json` | devops-engineer | ✅ Evet |
| 2 | `Directory.Build.props`, `Directory.Packages.props`, `global.json` | devops-engineer | ✅ Evet (§3.3) |
| 3 | `.gitignore` (secret satırları) | devops-engineer + security | ✅ Evet (§3.7) |
| 4 | Options sınıfları + doğrulama (`Host`) | developer | ✅ Evet (§3.6, §3.8) |
| 5 | Secret kaynak seçimi | security | ✅ Evet (§3.7) |
| 6 | Sağlayıcı endpoint/model değeri | developer | ❌ → provider-template §3.5 |
| 7 | Migration / `schema_version` | database-optimizer | ❌ → migration-template |
| 8 | `ADR-001` (DevExpress kilidi) | architect | ❌ → adr-devexpress-template |
| 9 | `.ai/keys.md` | security | ❌ YOK (§1.2 E14) |

### §2.2 Kapsam Sınırı Testi

Üretilecek dosya/değişikliğe sırayla sor; **ilk `EVET`** doğru şablonu verir:

1. **Bir değerin "hangi sırada okunduğu" sorusu mu?** → `EVET` = bu şablon.
2. **Bir MSBuild dosyasının rolü/koşulu sorusu mu?** → `EVET` = bu şablon (§3.3–§3.4).
3. **Secret'ın nereye konacağı / `.gitignore` sorusu mu?** → `EVET` = bu şablon (§3.7).
4. **Sorun sağlayıcının kendisi mi (timeout, hata kodu)?** → provider-template.
5. **Sorun sürüm/DevExpress kilidi kararının gerekçesi mi?** → adr-devexpress-template; bu şablon yalnız **kilidi uygular** (§3.9).

---

## §3 Mimari

### §3.1 Katman / Tip Karşılaştırma Tablosu (öncelik sırası)

| Öncelik | Kaynak | Nerede Yaşar | Kim Değiştirir | Kabul | Yasak |
|---|---|---|---|---|---|
| 1 (en yüksek) | Çalışma zamanı argümanı | `Program.cs` / CLI parser | operatör (bir kezlik) | ✅ | kalıcı saklama |
| 2 | Ortam değişkeni (`VERSA__BOLGE`) | OS / konteyner | devops | ✅ | repo'ya yazmak |
| 3 | `appsettings.{Ortam}.json` | uygulama katmanı | devops | ✅ | secret (dolu) |
| 4 | `appsettings.json` | uygulama katmanı | developer | ✅ | secret, gizli değer |
| 5 | `dotnet user-secrets` | makine (gizli defter) | developer | ✅ (yalnız Development) | Production'a taşımak |
| 6 (en düşük) | Kod varsayılanı (`?? 30`) | `.cs` | developer | ⚠️ | stratejik değeri gizlemek |

| Kural | Ayrıntı |
|---|---|
| Okuma sırası | 6 → 1 tersinden okunur; **en son yazan kazanır** (1 en yüksek) |
| Zorunlu anahtar | Options sınıfında `required` veya §3.8 doğrulaması ile kontrol edilir |
| Kod varsayılanı | Yalnız asgari değerler (timeout=30 sn); stratejik değer kodda **yasak** |
| Secret | Öncelik 3–4'te **asla** dolu; yalnız §3.7 kaynaklarında |

### §3.2 Öncelik Uygulama Örneği (dolu — üretim standardı)

`DbPath` anahtarı için dört kaynağın aynı anda var olduğu senaryo:

| Kaynak | Değer | Etki |
|---|---|---|
| kod (`?? "./versacoder.db"`) | `./versacoder.db` | yalnız 3–6 hiç yoksa |
| `appsettings.json` | `"DbPath": "./data/versacoder.db"` | Development'ta bunu ezer |
| `appsettings.Production.json` | `"DbPath": "D:\\vc\\versacoder.db"` | Production'da bunu ezer |
| `VERSA_DBPATH` env | `E:\mnt\vc.db` | **en son → kazanır** |
| `--katalog E:\mnt` argümanı | `E:\mnt\versacoder.db` | **1 numara → her şeyi ezer** |
### §3.3 MSBuild Dosyaları Roller Tablosu

| Dosya | Zorunlu mu | Tek Sorusu | İçeriği | Kırılırsa |
|---|---|---|---|---|
| `Directory.Build.props` | ✅ | "Her projede ortak **ne**?" | `TargetFrameworks`, `LangVersion`, `TreatWarningsAsErrors`, `Nullable` | Projeler birbirine kayar |
| `Directory.Packages.props` | ✅ (CPM) | "Paket sürümü **nerede**?" | `ManagePackageVersionsCentrally=true` + `PackageVersion` listesi | Sürüm projelere dağılır |
| `global.json` | ✅ | "Hangi **SDK**?" | `sdk.version`, `rollForward` | Makineden makineye farklı SDK |
| `appsettings*.json` | ✅ (Host) | "Çalışma zamanında **ne**?" | bağlantı yolu, timeout, ortam adı | Kod içinde magic değer |
| `.csproj` | ✅ | "Bu projeye **özgü** ne?" | `ProjectReference`, TFM istisnası | Ortak özellik tekrarlanır |

**Rol sınırı (öncelikli çakışma kuralı):**

| Çakışan Özellik | Kazanan | Neden |
|---|---|---|
| Paket sürümü: `.csproj` vs CPM | **CPM** — `.csproj`'de `<Version>` yasak | Sürüm tek yerde |
| `TargetFrameworks`: props vs `.csproj` | props + `.csproj` yalnız **istisnada** override | TFM tek yerde |
| `TreatWarningsAsErrors`: props vs CLI | CLI (geçici) / props (kalıcı) | Bilinçli istisna |
| `Nullable`: props vs `.csproj` | props | Tutarlılık |

### §3.4 Çoklu Hedef — Koşullu Özellik (`net48;net10.0-windows`)

```xml
<!-- Directory.Build.props: TFM tek kaynaktan; istisna koşullu yazılır -->
<PropertyGroup>
  <TargetFrameworks>net48;net10.0-windows</TargetFrameworks>
  <LangVersion>latest</LangVersion>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <Nullable>enable</Nullable>
</PropertyGroup>

<!-- Koşullu özellik: TFM'e göre ÖZEL davranış; koşulsuz atama YASAK (G4) -->
<PropertyGroup Condition="'$(TargetFramework)' == 'net48'">
  <UseWindowsForms>true</UseWindowsForms>
  <DefineConstants>$(DefineConstants);NET48</DefineConstants>
</PropertyGroup>
```

| Kural | Doğru | Yanlış |
|---|---|---|
| Koşul yazımı | `'$(TargetFramework)' == 'net48'` (tırnak + tek eşitlik) | `$(TargetFramework)==net48` (tırnaksız → sessiz false) |
| Platform kodu | `#if NET48 … #endif` | çalışma zamanında tip kontrolü |
| Tek hedefe özel paket | `<PackageReference … Condition="…">` | iki TFM'e de koyup `net48`'de kırmak |
| `.csproj`'de TFM | yalnız istisna (`<TargetFramework>` ile ezmek) | her projede tekrar yazmak |
| Doğrulama | iki hedef de derlenir (§5.3 C5) | yalnız modern hedefle test |

### §3.5 Ortam Adları

| Ortam | Ne değişir | Dosya | Başlangıç |
|---|---|---|---|
| `Development` | debug, verbose log, user-secrets aktif | `appsettings.json` + `appsettings.Development.json` | varsayılan |
| `Production` | fail-fast, minimal log, secret yalnız env/DPAPI | `appsettings.Production.json` (repo'da **şablon**) | `--environment Production` / `ASPNETCORE_ENVIRONMENT` |
| Test | sabit, deterministik değerler | test projesi içi | test runner |
| (yeni) | yalnız `§6.3` ile eklenir | — | — |

| Kural | Ayrıntı |
|---|---|
| Ortam adı | Tek nokta ile yazılır (`Production`, `Prod` YASAK — dosya eşleşmez) |
| Production'da Development dosyası | Yüklenir ama **ezilir**; gizli değer oraya konmaz |
| 9 | Bilinmeyen ortam | §3.8 fail-fast → bilinmeyen ortam adı = başlangıç reddi |

### §3.6 Tip-Güvenli Okuma (Options Pattern)

```csharp
// Host katmanı: kayıt — anahtar adı tek yerde, string dağınıklığı yok
builder.Services.AddOptions<VersaOptions>()
    .Bind(builder.Configuration.GetSection(VersaOptions.SectionName))
    .ValidateDataAnnotations()      // [Required], [Range]
    .ValidateOnStart();             // FAIL-FAST: başlangıçta doğrula (§3.8)

public sealed class VersaOptions
{
    public const string SectionName = "Versa";
    [Required] public string DbPath { get; init; } = "";
    [Range(1, 600)] public int TimeoutSeconds { get; init; } = 30;
}
```

| Yöntem | Kabul | Neden |
|---|---|---|
| `IOptions<T>` / `IOptionsSnapshot<T>` | ✅ | Tip güvenliği + tek anahtar |
| `Configuration["Versa:DbPath"]` (ham string) | ⚠️ yalnız 1 yerde | Anahtar yazım hatası çalışma zamanında |
| `Environment.GetEnvironmentVariable` doğrudan | ❌ | Öncelik sırası çöker (§3.1) |
| Kod içi sabit (`const string DbPath = …`) | ❌ (stratejik) | Değiştirilemez yapılandırma |

### §3.7 Secret Yönetimi + `.gitignore`

| # | Secret Kaynağı | Ortam | Kabul | Not |
|---|---|---|---|---|
| 1 | Ortam değişkeni | Production/CI | ✅ | 1 numara (§3.1) |
| 2 | DPAPI korumalı yerel dosya | Production (Windows) | ✅ | `CurrentUser` kapsamı |
| 3 | `dotnet user-secrets` | Development | ✅ | repo dışı defter |
| 4 | `appsettings.Production.json` | — | ❌ dolu | yalnız anahtar **adı** |
| 5 | `.cs` / repo / `.ai/` | — | ❌ | **KESİN YASAK (G1)** |

`.gitignore` zorunlu satırları:

```gitignore
# secret ve üretim katmanı — bu 4 satır olmadan repo açılmaz (G7)
appsettings.Production.json
secrets.json
*.user
.env
# üretilen: bin, obj, kullanıcı tercihleri
[Bb]in/
[Oo]bj/
*.suo
```

| Denetim | Beklenen |
|---|---|
| Repoda `appsettings.Production.json` var mı | 0 |
| Repoda `secrets.json` / `.env` var mı | 0 |
| Dolu secret deseni (§5.3 C4) | 0 satır |

### §3.8 Doğrulama + Fail-Fast

| Doğrulama | Ne Kontrol Edilir | Başarısızlıkta |
|---|---|---|
| Options `ValidateOnStart` | `required` + `[Range]` | **uygulama açılmaz**, anlamlı mesaj |
| Bağlantı yolu | `DbPath` yazılabilir dizin mi | açılışta `InvalidOperationException` |
| Ortam adı | bilinen 3 değerden biri mi | açılışta red (§3.5) |
| TFM bütünlüğü | iki hedef de derleniyor mu | CI kırmızı (§5.3 C5) |
| Sürüm kilidi | DevExpress tam `25.2.8` mi | §3.9 → kırmızı |
| Secret temizliği | açık anahtar 0 satır | §5.3 C4 → kırmızı |

**Fail-fast ilkesi:** Eksik/zorunlu değer **başlangıçta** reddedilir; `null` ile sessiz devam yasak (G8).

### §3.9 Sürüm Kilidi + Dolu Örnek + Tam Kalıp

| Paket / Araç | Kilitlenen Sürüm | Kaynak | Yükseltme |
|---|---|---|---|
| DevExpress | **25.2.8** (tam) | ADR-001 | **YASAK** — ADR kararı olmadan |
| .NET SDK | `10.0.401` (kurulu, ✅ E7) | §1.2 | `global.json` + §6.3 |
| TFM | `net48;net10.0-windows` | §3.4 | mimari karar |
| Veri erişimi | ADO.NET + Dapper | ADR-004 | EF Core'a geçiş yasak |
| CPM paket sürümleri | `Directory.Packages.props` tek nokta | §3.3 | tek satır `patch` |

Dolu örnek — `Directory.Packages.props` iskeletinin doldurulmuş hâli:

```text
Directory.Packages.props (CPM)
  ManagePackageVersionsCentrally = true
  DevExpress.Win                    25.2.8   <- ADR-001, yuvarlak surum YASAK
  DevExpress.Win.Forms              25.2.8
  Dapper                            <- ADR-004, versiyon kilidi (sabit)
  Microsoft.Data.SqlClient          <- sabit
  .csproj icinde <Version>          <- YASAK (G3): surum yalniz burada
```

Yeni yapılandırma eklerken kopyalanacak kalıp (yalnız `{{…}}` doldurulur):

````markdown
### {{AYAR_ADI}} — Yapılandırma Kaydı

| Alan | Değer |
|---|---|
| Anahtar | `{{BOLUM}}:{{ANAHTAR}}` |
| Tür / varsayılan | `{{TUR}}` / `{{VARSAYILAN}}` |
| Öncelik kaynağı | §3.1 sıra no `{{ONCELIK}}` (env / user-secrets / json / kod) |
| Options sınıfı | `{{SINIF_ADI}}.{{OZELLIK}}` — `[Required]`/`[Range]` ile |
| Ortam farkı | Development `{{DEV_DEGER}}` · Production `{{PROD_DEGER}}` (dolu secret YASAK) |
| Doğrulama | §3.8 → `ValidateOnStart` · başarısızlıkta açılış reddi |
| `.gitignore` | `{{GIZLI_DOSYA}}` satırı var mı → §3.7 |
| MSBuild etkisi | `{{MSBUILD_DOSYASI}}` (yoksa `—`) |
| Doğrulama komutu | §5.3 `C{{NO}}` → beklenen `{{BEKLENEN}}` |
| Onay | devops-engineer → security → reviewer |
````

---

## §4 Kurallar

### §4.1 Uygulama Sırası

| Sıra | İş | Neden önce |
|---|---|---|
| 1 | `global.json` (SDK kilidi) | Farklı SDK = farklı derleme |
| 2 | `Directory.Build.props` (TFM + ortak özellik) | Her şeyin üstü |
| 3 | `Directory.Packages.props` (CPM) | Paket sürümleri proje yazılmadan sabitlenir |
| 4 | `.gitignore` (secret/üretim satırları) | İlk commit'ten önce |
| 5 | Options sınıfı + `ValidateOnStart` | Okuma sözleşmesi |
| 6 | `appsettings.json` (+ ortam dosyaları) | Anahtar adları Options ile eşleşmeli |
| 7 | Secret kaynağı (§3.7) | Dolu değer yalnız orada |
| 8 | Fail-fast doğrulamaları (§3.8) | En son, ama başlangıçtan önce |
| 9 | §5.3 komutları + §6.1 checklist | Kapı öncesi son durak |

### §4.2 Sorumluluk ve Onay

| Aşama | Rol | Ne Onaylar | Süre |
|---|---|---|---|
| 1 | devops-engineer | §3.1–§3.5 + MSBuild dosyaları | 35 dk |
| 2 | developer | Options + doğrulama (§3.6, §3.8) | 25 dk |
| 3 | security | §3.7 secret + `.gitignore` + §5.3 C4 | 15 dk |
| 4 | reviewer | §6.1 checklist (17 madde) | 15 dk |
| 5 | master-orchestrator | Quality Report (§5.4) → `log.md` | 5 dk |

### §4.3 Ölçek / Standartlar

| Öğe | Değer | Sapma |
|---|---|---|
| Öncelik sırası | 6 katman (§3.1) | Yeni katman = §6.3 onayı |
| Dosya başına MSBuild | 1 `props` + 1 `packages` + 1 `global.json` | İkinci dosya RED (rol belirsizliği) |
| TFM | `net48;net10.0-windows` | Tek hedefe düşürme = ADR |
| DevExpress | tam `25.2.8` | Herhangi bir sapma = ADR-001 ihlali |
| `.gitignore` zorunlu satır | ≥ 4 (§3.7) | Eksik = G7 |
| Doğrulama | fail-fast zorunlu | G8 |
| Bu şablon dosyası | 480–660 satır / 15–32 KB | §6.1 K13–K14 |

### §4.4 Değişiklik Kontrolü

| Değişiklik | Sürüm Etkisi | Zorunlu Kayıt |
|---|---|---|
| Yeni anahtar (aynı bölüm) | `patch` | §3.9 kalıp satırı |
| Öncelik sırası değişikliği | `major` | §3.1 tablosu + `log.md` + tüm tüketici denetimi |
| MSBuild rol değişikliği | `minor` | §3.3 tablosu |
| Paket sürümü değişikliği | `minor` | CPM satırı; DevExpress ise **ADR-001** |
| Yeni ortam adı | `minor` | §3.5 tablosu |
| Secret kaynağı değişimi | `minor` | security onayı |

---

### §4.5 Hard Guardrails (Otomatik DUR)

| # | Guardrail | İhlal Sonucu | Tespit |
|---|-----------|--------------|--------|
| G1 | Secret açık metin (`.cs`, `appsettings.json`, repo, `.ai/`) | **DUR** — anahtar iptal + taşınma | §5.3 C4 |
| G2 | Sürümü olmayan paket (`PackageReference` + `<Version>`, floating `*`/`x.y.*`) | **DUR** — CPM'e taşınır | §5.3 C2, C6 |
| G3 | CPM açıkken `.csproj` içinde `<Version>` | **DUR** | §5.3 C6 |
| G4 | Koşulsuz atama ile TFM'e özel özellik (§3.4) | **DUR** — `Condition` eklenir | §5.3 C5 |
| G5 | İki TFM'de farklı davranış, `#if NET48` olmadan | **DUR** | gözden geçirme |
| G6 | Kod içi gömülü stratejik değer (bağlantı yolu, anahtar) | **DUR** — Options'a | §6.1 K8 |
| G7 | `.gitignore`'da §3.7 satırları eksik | **DUR** | §5.3 C7 |
| G8 | Doğrulamasız başlatma / null ile sessiz devam | **DUR** — `ValidateOnStart` | §3.8 |
| G9 | DevExpress sürümü `25.2.8` dışında | **DUR** — ADR-001 ihlali | §5.3 C3 |
| G10 | EF Core referansı (ADR-004) / string concat SQL | **DUR** | vault G2/G10 |
| G11 | Öncelik sırasının kodda/BAŞKA dosyada tekrar yazılması | **DUR** — SSOT ihlali | §7.2 R5 |
| G12 | `coremusic`/versacoder.ai içeriği kopyası; mojibake (`Ã`, `Â`) | **DUR** | §5.3 C8 |

---

## §5 Workflow

### §5.1 Adım Bazlı Akış

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|---|---|---|---|---|
| 1 | Değer envanteri | kod + ihtiyaç listesi | anahtar tablosu | devops-engineer | 20 dk |
| 2 | Öncelik atama | §3.1 | sıra no | devops-engineer | 10 dk |
| 3 | `global.json` + props + CPM | §3.3 | 3 MSBuild dosyası | devops-engineer | 25 dk |
| 4 | Koşullu özellik + TFM | §3.4 | `Condition` blokları | developer | 20 dk |
| 5 | `.gitignore` | §3.7 | ≥ 4 satır | devops-engineer | 5 dk |
| 6 | Options + doğrulama | §3.6, §3.8 | options sınıfı | developer | 30 dk |
| 7 | `appsettings*` anahtarları | §3.2 | JSON dosyaları | devops-engineer | 20 dk |
| 8 | Secret kaynağı | §3.7 | env/user-secrets kaydı | security | 15 dk |
| 9 | Fail-fast testi | §3.8 | eksik ayarla açılış reddi | testing | 20 dk |
| 10 | §5.3 + §6.1 | — | 0 ihlal | reviewer | 15 dk |
| 11 | Quality Report | §5.4 | `log.md` satırı | orchestrator | 5 dk |
| | **Toplam** | | | | **≈ 185 dk (3,1 sa)** |

### §5.2 Hata Modları

| # | Hata Modu | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Öncelik ezmesi | env ayarı "hiç okunmuyor" | §3.1 sırası + §5.3 C1 |
| 2 | Props sessiz uygulanmıyor | özellik projede yok | `.csproj` import/scope (§3.3) |
| 3 | Koşul yanlış yazımı | TFM'e özel özellik yok | tırnak + tek eşitlik (§3.4) |
| 4 | CPM ihlali | sürüm projede tekrar | `<Version>` sil, `PackageVersion`'a taşı (G3) |
| 5 | Sürüm kayması | DevExpress ≠ 25.2.8 | CPM satırını kilitle (G9) |
| 6 | Secret sızıntısı | C4 > 0 | anahtar iptal + §3.7 kaynağı |
| 7 | Ortam adı yazım hatası | dosya yüklenmiyor | tek nokta kuralı (§3.5) |
| 8 | Sessiz null devam | "boş db yolu" hatası geç krizde | `ValidateOnStart` (G8) |
| 9 | `global.json` yok | SDK makineye göre değişir | §5.3 C0 ile kilitle |
| 10 | Kod varsayılanı çatışması | iki yerde farklı default | tek yer (§3.1, 6. satır) |

### §5.3 Salt Okunur Doğrulama Komutları

> Aşağıdaki 8 komut **hiçbir dosyayı değiştirmez**. C0–C3, C5–C6 mevcut diski; C4, C7 henüz kurulmamış katmanı tarar (hedef dosya yoksa 0 satır = geçer).

```powershell
# C0) SDK KİLİDİ — kurulu SDK + global.json tutarlılığı
dotnet --list-sdks
if (Test-Path 'C:\versacoder\global.json') { Get-Content 'C:\versacoder\global.json' -Raw }
else { 'global.json YOK — §1.2 E5 (kurulacak, §4.1 Adım 1)' }
```

```powershell
# C1) MSBUILD DOSYALARI — 3/3 var mı, CPM anahtarı açık mı
'Directory.Build.props','Directory.Packages.props','global.json','.gitignore' |
  ForEach-Object { [PSCustomObject]@{ Dosya = $_; Var = (Test-Path "C:\versacoder\$_") } } |
  Format-Table -AutoSize
Get-Content 'C:\versacoder\Directory.Packages.props' -ErrorAction SilentlyContinue |
  Select-String 'ManagePackageVersionsCentrally'
```

```powershell
# C2) PAKET SÜRÜMÜ — floating/CMS ihlali 0 olmalı (G2)
Select-String -Path 'C:\versacoder\**\*.csproj' `
  -Pattern 'Version\s*=\s*"[^"]*[\*]|VersionPrefix' -ErrorAction SilentlyContinue |
  Select-Object Path, LineNumber, Line
```

```powershell
# C3) DEVEXPRESS KİLİDİ — tam 25.2.8 dışına çıkan satır 0 olmalı (G9)
Select-String -Path 'C:\versacoder\Directory.Packages.props' `
  -Pattern 'DevExpress' -ErrorAction SilentlyContinue |
  Where-Object { $_.Line -notmatch '25\.2\.8' } | Select-Object LineNumber, Line
```

```powershell
# C4) SECRET TARAMASI — açık anahtar 0 satır OLMALI (G1)
Select-String -Path 'C:\versacoder\src\**\*.cs','C:\versacoder\**\appsettings*.json' `
  -Pattern 'sk-[A-Za-z0-9]{16,}|(?i)(password|apikey|api[_-]?key|secret)"?\s*[:=]\s*"[^"$]{8,}' `
  -ErrorAction SilentlyContinue | Select-Object Path, LineNumber, Line
```

```powershell
# C5) İKİLİ TFM — props + istisnalar; EF Core 0 satır (G4, G10)
Get-Content 'C:\versacoder\Directory.Build.props' -ErrorAction SilentlyContinue |
  Select-String 'TargetFrameworks|Condition'
Select-String -Path 'C:\versacoder\**\*.csproj','C:\versacoder\Directory.Packages.props' `
  -Pattern 'EntityFramework' -ErrorAction SilentlyContinue | Select-Object Path, Line
```

```powershell
# C6) CPM İHLALİ — .csproj içinde <Version> olan satır 0 olmalı (G3)
Select-String -Path 'C:\versacoder\**\*.csproj' `
  -Pattern '<Version>|<PackageReference[^>]*>\s*<Version>' -ErrorAction SilentlyContinue |
  Select-Object Path, LineNumber, Line
```

```powershell
# C7) ÖLÇÜM — bu dosya: satır 480-660, KB 15-32, gerçek mojibake 0
$f = 'C:\versacoder\.ai\.templates\infrastructure\configuration-template.md'
"satu: {0} (hedef 480-660)" -f (Get-Content -LiteralPath $f -Encoding UTF8).Count
"KB:   {0} (hedef 15-32)"    -f [math]::Round((Get-Item -LiteralPath $f).Length / 1KB, 1)
#    Gerçek mojibake 0 olmalı; bu dosyadaki kasıtlı tarama satırları (C7'nin kendi
#    deseni ve §6.2 red satırı) hariç — ikisi de bilinçli örnek metindir.
"mojibake: {0} (hedef 0)"    -f (Select-String -LiteralPath $f -Pattern 'Ã|Â|\ufffd' |
  Where-Object { $_.Line -notmatch 'Select-String -LiteralPath \$f|Mojibake \(' } |
  Measure-Object).Count
```

**Geçme kriteri:** C2, C3, C4, C5(EF satırı), C6 = 0 satır · C0/C1 bilgi çıktısı (`Var` sütunu `True` · `ManagePackageVersionsCentrally=true`) · C7 üçü de aralıkta.

### §5.4 Quality Report

```markdown
## Yapılandırma Kalite Raporu — YYYY-MM-DD

| Metrik | Değer | Eşik | Durum |
|--------|-------|------|-------|
| MSBuild 3'lüsü (props/packages/global.json) | N/3 | 3/3 | 🟢/🔴 |
| C1 CPM anahtarı `true` | evet/hayır | evet | 🟢/🔴 |
| C4 açık secret satırı | N | 0 | 🟢/🔴 |
| C3 DevExpress ≠ 25.2.8 | N | 0 | 🟢/🔴 |
| C6/C5 ihlali (`<Version>` / EF Core) | N | 0 | 🟢/🔴 |
| Fail-fast (`ValidateOnStart`) | var/yok | var | 🟢/🔴 |
| §4.5 G1..G12 ihlali | N | 0 | 🟢/🔴 |

**Karar:** 🟢 `log.md`'ye düşüldü / 🔴 düzeltmeye gönderildi
```

---

## §6 Doğrulama

### §6.1 Kontrol Listesi

- [ ] K1 Frontmatter 7 alan (`title, type, category, status, version, updated, authority: primary`)
- [ ] K2 H1 + `**Zorunlu Bağlantılar:**` 6 bağlantı
- [ ] K3 §1.1–§1.4 dolu; §1.2 disk kanıtlı + `⚠️`/`❌` işaretli
- [ ] K4 §2.1–§2.2 dolu
- [ ] K5 §3.1–§3.9 arası eksiksiz; §3.9 `{{…}}` kalıbı ````markdown ```` içinde
- [ ] K6 Her §'de en az bir tablo; toplam ≥ 25 tablo satırı
- [ ] K7 §3.1 öncelik tablosu 6 satır, sıra yazılı
- [ ] K8 §3.3 MSBuild rol tablosu + çakışma kazananı
- [ ] K9 §3.4 koşullu özellik kod bloğu (`Condition` + `net48`)
- [ ] K10 §3.6 options pattern kod bloğu (`ValidateOnStart`)
- [ ] K11 §3.7 secret + `.gitignore` (≥ 4 zorunlu satır)
- [ ] K12 §4.5 = 12 guardrail (≥10); §4.1–§4.4 dolu
- [ ] K13 §5.3 ≥ 5 salt-okunur PowerShell (8 komut); §5.4 Quality Report
- [ ] K14 Satır 480–660 · KB 15–32
- [ ] K15 §6.2 red sinyalleri + §6.3 geçiş + §6.5 envanter kontrolleri
- [ ] K16 §7.1 referans + §7.2 Red Team ≥ 10 satır
- [ ] K17 Gerçek mojibake = 0 (C7); `{{…}}` yalnız §3.9 kalıp bloklarında

### §6.2 Red Sinyalleri (Otomatik DUR)

| Sinyal | Aksiyon |
|---|---|
| C4 > 0 satır | **DUR** — anahtar iptal + log temizliği |
| C3 > 0 satır | **DUR** — sürüm `25.2.8`'e çekilir (ADR-001) |
| C6 > 0 satır | **DUR** — `<Version>` CPM'e taşınır |
| C5'te `EntityFramework` | **DUR** — ADO.NET + Dapper (ADR-004) |
| C1 = `False` | **DUR** — §4.1 adımları 1–3 tamamlanır |
| Öncelik sırası başka dosyada tekrar | **DUR** — sil, §3.1'e bağlan (G11) |
| Koşulsuz TFM ataması | **DUR** — `Condition` (G4) |
| `ValidateOnStart` yok | **DUR** — fail-fast (G8) |
| `{{…}}` kalıp bloğu dışında | **DUR** — doldur |
| Satır < 480 veya > 660 | **DUR** — §4.3 ölçeği |
| Mojibake (`Ã`, `Â`) | **DUR** — UTF-8 BOM'suz yeniden yaz |

### §6.3 Sürüm / Geçiş Kuralları

| Değişiklik | Etki | Zorunlu |
|---|---|---|
| Yeni anahtar (aynı bölüm) | `patch` | §3.9 kalıp + test |
| Yeni yapılandırma bölümü | `minor` | §3.6 options sınıfı + doğrulama |
| Öncelik sırası değişikliği | `major` | tüm tüketici denetimi + `log.md` |
| Yeni ortam adı | `minor` | §3.5 tablosu + fail-fast listesi |
| DevExpress sürüm değişikliği | `major` | **ADR-001 kararı** (G9) |
| TFM değişikliği | `major` | ADR + §3.4 + iki hedef derlemesi |

### §6.4 İlgili Dosyalar

| Dosya | İlişki |
|---|---|
| `.ai/.templates/infrastructure/configuration-template.md` | **bu dosya** (değer yaşam döngüsü SSOT) |
| `.ai/.templates/infrastructure/provider-template.md` | secret'ın tüketicisi; §3.5'e bağlanır |
| `.ai/.templates/index.md` | envanter `#24` (Part B, `T-VER`, devops-engineer) |
| `.ai/.templates/architecture/ascii-diagram-template.md` | biçim referansı (7 bölüm, Quality Report, Red Team) |
| `.ai/.templates/adr/adr-architecture-template.md` | TFM/katman kararı (yapı referansı) |
| `.ai/.templates/adr/adr-devexpress-template.md` | 25.2.8 kilidinin karar kaynağı |

### §6.5 Envantere Özgü Kontroller

| # | Kontrol | Beklenen | Komut |
|---|---|---|---|
| K1 | `infrastructure/` klasöründe 2 şablon | 2 | `Get-ChildItem …\infrastructure -Filter *.md` |
| K2 | Envanter satırı `#24` hedefi | `appsettings*.json` | index.md §3.3 |
| K3 | DevExpress yazımı `25.2.8` (her yerde aynı) | tutarlı | §5.3 C3 |
| K4 | `net48;net10.0-windows` yazımı | tek biçim | gözden geçirme |
| K5 | `coremusic` geçen satır | 0 | `Select-String -Pattern 'coremusic'` |
| K6 | `EntityFramework` geçen satır | 0 | §5.3 C5 |
| K7 | `{{…}}` kalıp bloğu dışında | 0 | C7 + gözden geçirme |
| K8 | Guardrail adedi | 12 | §4.5 |

---

## §7 Referanslar

### §7.1 VersaCoder Bağlantıları

| # | Kaynak | Yol | Ne İçin | Durum |
|---|---|---|---|---|
| 1 | Şablon envanteri | `.ai/.templates/index.md` | `#24` kaydı, kalite sözleşmesi §3.7 | ✅ OKUNDU |
| 2 | Dizin kuralı | `.ai/.templates/CLAUDE.md` | erişim sırası | ✅ OKUNDU |
| 3 | Biçim referansı | `.ai/.templates/architecture/ascii-diagram-template.md` | 7 bölüm, §5.3, §5.4, §7.2 | ✅ OKUNDU |
| 4 | ADR yapı referansı | `.ai/.templates/adr/adr-architecture-template.md` | TFM/katman iskeleti (yalnız yapı) | ✅ OKUNDU |
| 5 | İkiz şablon | `.ai/.templates/infrastructure/provider-template.md` | secret tüketimi | ✅ BU İŞ |
| 6 | Altın standart ADR | `.ai/.templates/adr/adr-template.md` | — | ❌ YOK (§1.2 E12) |
| 7 | MSBuild hedefleri | `Directory.Build.props`, `Directory.Packages.props`, `global.json`, `.gitignore` | §3.3–§3.7 | ❌ YOK (E3–E6) |
| 8 | Vault kök/kurallar | `.ai/architecture/index.md`, `.ai/.rules/`, `.ai/log.md` | kural + kayıt | ❌ YOK (E14) |

### §7.2 Red Team — Sık Yapılan Hatalar

| # | Hata | Saldırı/Kırılma | Yakalama | Doğrusu |
|---|---|---|---|---|
| 1 | Secret'ı `appsettings.json`'a yazmak | Repo sızıntısı → DB/LLM erişimi | §5.3 C4 | env / user-secrets / DPAPI (§3.7) |
| 2 | `.gitignore`'siz ilk commit | Üretim katmanı + `.env` geçmişte | §5.3 C7 / G7 | §3.7 4 zorunlu satır |
| 3 | Floating paket sürümü (`*`, `x.y.*`) | Sessiz sürüm kayması → kırık build | C2, G2 | CPM + tam sürüm |
| 4 | `.csproj`'de `<Version>` (CPM açıkken) | İki sürüm kaynağı → çatışma | C6, G3 | sürüm yalnız `Directory.Packages.props` |
| 5 | Öncelik sırasını kod tekrarıyla değiştirmek | İki gerçek kaynağı olan sıra | G11 | yalnız §3.1 (SSOT) |
| 6 | Koşulsuz TFM ataması | `net48`'de sessiz yanlış davranış | C5, G4 | `Condition="'$(TargetFramework)' == 'net48'"` |
| 7 | İki TFM'de `#if`siz farklı kod | Modernde yeşil, legacy'de kırık | G5 | `#if NET48` + iki hedef derlemesi |
| 8 | DevExpress'i "küçük" sürümle güncellemek | ADR-001 ihlali, WinForms kırılabilir | C3, G9 | tam `25.2.8`, yükseltme ADR'li |
| 9 | EF Core'a dönmek | ADR-004 ihlali | C5, G10 | ADO.NET + Dapper |
| 10 | Doğrulamasız başlangıç (null ile devam) | Boş DB yolu, geç kriz | G8 | `ValidateOnStart` + fail-fast |
| 11 | Ortam adı yazım hatası (`Prod`) | Dosya yüklenmez, yanlış ayarla açılış | §3.5 | tek nokta + bilinen adlar |
| 12 | Kod içine gömülü stratejik değer | Değiştirilemez yapılandırma | G6 | Options + §3.1 sırası |

**SSUT beyanı:** Bu dosya VersaCoder yapılandırmasının **tek** kaynağıdır; kopyalayan her dosya yalnız bu dosyaya bağlanır.
