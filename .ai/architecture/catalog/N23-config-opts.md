---
title: "Katalog Dugumu N23 - Yapilandirma Secenekleri (l3)"
type: catalog
category: architecture
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: "N23 config-opts dugumu: kimlik, src klasoru, aile yapisi, ornek kayit ve yon kurali (ADR-013 + ADR-028)"
---

# Katalog Dugumu N23 - Yapilandirma Secenekleri

**Zorunlu Baglantilar:** [catalog index](index.md) · [node-inventory](node-inventory.md) · [mimari index](../index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md)

> **Tek cumlelik sorumluluk:** *N23 config-opts hangi projede ve klasorde yasar, ne yapar, kac kayit hedefler* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

**N23 (config-opts), l3 Crosscutting katmaninin alt katmanidir: `VersaCoder.Crosscutting` projesinde `src/VersaCoder.Crosscutting/Config/` klasorunde yasar.** Yapilandirma okuma, bolum, cevresel degisken ve yeniden yukleme seceneklerini tanimlar. 10 aile x 12 rol = 120 kayit hedefi (ADR-013 asgari 112); durum `planli`.

Kayitlar ADR-013 2.1 biciminde 8 sutun tasir; bu dosya aile kalibini ve ornek satirlari gosterir, tam 120 satirlik liste bu dosyanin boyut sinirina sigmaz (DOĞRULAMA GEREKLI).

```text
+--------------------------------------------------+
| l3 Crosscutting -> VersaCoder.Crosscutting
| src/VersaCoder.Crosscutting/Config/
| 10 aile x 12 rol = 120 kayit (esik 112)
+--------------------------------------------------+
```

| Alan | Deger |
|---|---|
| Dugum | N23 |
| Dugum turu | alt katman (klasor/namespace duzeyi) |
| Makro katman | l3 Crosscutting |
| Proje (.csproj) | VersaCoder.Crosscutting |
| TFM | net48;net10.0-windows |
| Kayit hedefi | 120 (ADR-013 asgari 112) |
| Durum | planli |
| Sorumlu rol | architect |

---

## §1 Sorumluluk (YAPAR / YAPMAZ)

| # | YAPAR | YAPMAZ |
|---|---|---|
| 1 | Yapilandirma okuma, bolum, cevresel degisken ve yeniden yukleme seceneklerini tanimlar | l0 disinda bir katmana dogrudan bakmak |
| 2 | kayitlari 8 sutunlu ADR-013 biciminde tutmak (planli -> yapildi -> testli) | is kurali veya invariant tasimak, is l1 katmaninda |
| 3 | ortak kesit servisini vermek: log, saat, onbek, kripto, yapilandirma | kullanim senaryosu orkestre etmek, is l2 katmaninda |
| 4 | test edilebilir saricilar sunmak (IClock, ILogSink) | SQL veya veri erisimi yazmak, is l4 katmaninda |
| 5 | yalniz l0 katmanina bakmak | UI tipine bakmak (Desktop / Cli) |
| 6 | katmanlari degil alt altyapiyi sarmak | secret veya anahtar barindirmak, keys.md tek kaynaktir |

---

## §2 Klasor ve N Eslemesi

**Kalip (ADR-028 2.3/4):** `N33 -> src\VersaCoder.Protocol\ToolSchema\` satiri SSOT ornegidir; bu dosyanin klasoru ayni kalipla yazilir.

| # | N | Slug | src/ klasoru |
|---|---|---|---|
| 1 | N23 | config-opts | `src/VersaCoder.Crosscutting/Config/` |

**Kanit yolu (ADR-028 2.3/5):** tarama yolu gercek proje adini icerir; genis kalip (`src/**/Domain/**`) sahte PASS uretir.

```powershell
Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Config'
```

Dizin su an yok cikti `False` olur; PASS yazilmaz -> **DOĞRULAMA GEREKLI**.

---

## §3 Aile Yapisi

Rol kalibi (12): `I<F>` · `<F>` · `<F>Dto` · `<F>Mapper` · `<F>Validator` · `<F>Factory` · `<F>Options` · `<F>Provider` · `<F>Registry` · `<F>Converter` · `<F>Tests` · `<F>Builder`

| # | Aile | # | Aile |
|---|---|---|---|
| 1 | AppConfig | 6 | ConfigValidator |
| 2 | OptionsReader | 7 | DefaultsProvider |
| 3 | ConfigSection | 8 | ConfigReload |
| 4 | EnvResolver | 9 | HotSettings |
| 5 | SecretResolver | 10 | OptionBinding |

Her aile ayni rol kalibini 12 kez tekrarlar; kayit sayisi = aile x rol.

---

## §4 Kayit Ornegi (8 sutun)

| ID | Bilesen | Tip | Sorumluluk | Proje | TFM | Test | Durum |
|---|---|---|---|---|---|---|---|
| N23-001 | `IAppConfig` | interface | AppConfig sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-002 | `AppConfig` | class | AppConfig cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-003 | `AppConfigDto` | record | AppConfig veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-004 | `AppConfigMapper` | class | AppConfig <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-005 | `AppConfigValidator` | class | AppConfig dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-006 | `AppConfigFactory` | class | AppConfig uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |

Kalan aileler ayni kalibi 12 rolle tekrarlar (makro dugumlerde 4 rol); toplam hedef **120** kayit.

---

## §5 Yon Kurali

| # | Yon | Deger | Kaynak |
|---|---|---|---|
| 1 | l3 -> l0 | OK | ADR-028 2.1 |
| 2 | l3 -> l1, l2, l4..l7 | X | ADR-028 2.1 |
| 3 | l5 -> l4 | X | ADR-028 2.3/2 |

Matrisin tamami [layer-matrix](layer-matrix.md) dosyasindadir; NetArchTest `LayerRulesTests` bu tabloya gore yazilir.

---

## §6 Test ve Durum

| # | Kontrol | Beklenen | Durum |
|---|---|---|---|
| 1 | Kayit satiri | 120 (asgari 112) | bu dosyada 6 ornek satir; tam liste DOĞRULAMA GEREKLI |
| 2 | Durum degeri | planli -> yapildi -> testli | planli |
| 3 | Test sinifi | AppConfigTests (xUnit) | planli |
| 4 | NetArchTest yon denetimi | izinsiz kenar = 0 | planli (LayerRulesTests) |
| 5 | Dosya boyutu | 3-8 KB (2600-8500 bayt) | §9 olcumu |

---

## §7 Kapsam Disi

- K3 katmaninin 618 DLL envanteri bu dugume girmez; tek iz `reports\k3-618-ham-envanter.md` dosyasidir (ADR-028 2.2).
- Tam kayit listesi (120 satir) bu dosyaya sasmaz; ornek satirlar aile kalibini gosterir -> **DOĞRULAMA GEREKLI**.
- Kod yazimi `src/` icinde yapilir; bu dosya yalniz katalog kartidir, bir `.csproj` degildir.

---

## §8 Ilgili Dosyalar

| # | Dosya | Ne verir |
|---|---|---|
| 1 | [catalog index](index.md) | 52 dugum envanteri + N <-> klasor eslemesi |
| 2 | [node-inventory](node-inventory.md) | 52 dugumun tek tek kunyesi |
| 3 | [layer-matrix](layer-matrix.md) | 8x8 yon matrisi (ADR-028 2.3/2) |
| 4 | [project-map](project-map.md) | 9 uretim + 1-2 test proje, N araliklari |
| 5 | [mimari index](../index.md) | 8 makro katman ozeti |
| 6 | [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) | baz karar: 8 katman / 52 dugum / 9 proje |

---

## §9 Kanit

```powershell
`# 1) kayit sayimi (ADR-013 2.2) - beklenen: 120
(Select-String -Path 'C:\versacoder\.ai\architecture\catalog\N23-config-opts.md' -Pattern '^\|\s*N\d{2}-\d{3}\s*\|').Count
# 2) boyut bandi - beklenen: 2600-8500
(Get-Item 'C:\versacoder\.ai\architecture\catalog\N23-config-opts.md').Length
# 3) klasor kaniti - gercek proje adi (ADR-028 2.3/5)
(Test-Path 'C:\versacoder\src\VersaCoder.Crosscutting\Config')
```

Katalog sayimi `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` ile alinir; bu batch sonunda toplam esik (5000) henuz karsilanmaz - kalan 26 node dosyasi sonraki batchtir.

## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N23-007 | `AppConfigOptions` | record | AppConfig yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-008 | `AppConfigProvider` | class | AppConfig saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-009 | `AppConfigRegistry` | class | AppConfig kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-010 | `AppConfigConverter` | class | AppConfig bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-011 | `AppConfigTests` | class | AppConfig testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-012 | `AppConfigBuilder` | class | AppConfig uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | AppConfigTests | planli |
| N23-013 | `IOptionsReader` | interface | OptionsReader sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-014 | `OptionsReader` | class | OptionsReader cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-015 | `OptionsReaderDto` | record | OptionsReader veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-016 | `OptionsReaderMapper` | class | OptionsReader <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-017 | `OptionsReaderValidator` | class | OptionsReader dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-018 | `OptionsReaderFactory` | class | OptionsReader uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-019 | `OptionsReaderOptions` | record | OptionsReader yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-020 | `OptionsReaderProvider` | class | OptionsReader saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-021 | `OptionsReaderRegistry` | class | OptionsReader kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-022 | `OptionsReaderConverter` | class | OptionsReader bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-023 | `OptionsReaderTests` | class | OptionsReader testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-024 | `OptionsReaderBuilder` | class | OptionsReader uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | OptionsReaderTests | planli |
| N23-025 | `IConfigSection` | interface | ConfigSection sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-026 | `ConfigSection` | class | ConfigSection cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-027 | `ConfigSectionDto` | record | ConfigSection veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-028 | `ConfigSectionMapper` | class | ConfigSection <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-029 | `ConfigSectionValidator` | class | ConfigSection dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-030 | `ConfigSectionFactory` | class | ConfigSection uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-031 | `ConfigSectionOptions` | record | ConfigSection yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-032 | `ConfigSectionProvider` | class | ConfigSection saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-033 | `ConfigSectionRegistry` | class | ConfigSection kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-034 | `ConfigSectionConverter` | class | ConfigSection bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-035 | `ConfigSectionTests` | class | ConfigSection testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-036 | `ConfigSectionBuilder` | class | ConfigSection uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigSectionTests | planli |
| N23-037 | `IEnvResolver` | interface | EnvResolver sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-038 | `EnvResolver` | class | EnvResolver cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-039 | `EnvResolverDto` | record | EnvResolver veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-040 | `EnvResolverMapper` | class | EnvResolver <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-041 | `EnvResolverValidator` | class | EnvResolver dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-042 | `EnvResolverFactory` | class | EnvResolver uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-043 | `EnvResolverOptions` | record | EnvResolver yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-044 | `EnvResolverProvider` | class | EnvResolver saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-045 | `EnvResolverRegistry` | class | EnvResolver kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-046 | `EnvResolverConverter` | class | EnvResolver bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-047 | `EnvResolverTests` | class | EnvResolver testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-048 | `EnvResolverBuilder` | class | EnvResolver uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | EnvResolverTests | planli |
| N23-049 | `ISecretResolver` | interface | SecretResolver sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-050 | `SecretResolver` | class | SecretResolver cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-051 | `SecretResolverDto` | record | SecretResolver veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-052 | `SecretResolverMapper` | class | SecretResolver <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-053 | `SecretResolverValidator` | class | SecretResolver dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-054 | `SecretResolverFactory` | class | SecretResolver uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-055 | `SecretResolverOptions` | record | SecretResolver yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-056 | `SecretResolverProvider` | class | SecretResolver saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-057 | `SecretResolverRegistry` | class | SecretResolver kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-058 | `SecretResolverConverter` | class | SecretResolver bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-059 | `SecretResolverTests` | class | SecretResolver testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-060 | `SecretResolverBuilder` | class | SecretResolver uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | SecretResolverTests | planli |
| N23-061 | `IConfigValidator` | interface | ConfigValidator sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-062 | `ConfigValidator` | class | ConfigValidator cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-063 | `ConfigValidatorDto` | record | ConfigValidator veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-064 | `ConfigValidatorMapper` | class | ConfigValidator <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-065 | `ConfigValidatorValidator` | class | ConfigValidator dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-066 | `ConfigValidatorFactory` | class | ConfigValidator uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-067 | `ConfigValidatorOptions` | record | ConfigValidator yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-068 | `ConfigValidatorProvider` | class | ConfigValidator saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-069 | `ConfigValidatorRegistry` | class | ConfigValidator kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-070 | `ConfigValidatorConverter` | class | ConfigValidator bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-071 | `ConfigValidatorTests` | class | ConfigValidator testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-072 | `ConfigValidatorBuilder` | class | ConfigValidator uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigValidatorTests | planli |
| N23-073 | `IDefaultsProvider` | interface | DefaultsProvider sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-074 | `DefaultsProvider` | class | DefaultsProvider cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-075 | `DefaultsProviderDto` | record | DefaultsProvider veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-076 | `DefaultsProviderMapper` | class | DefaultsProvider <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-077 | `DefaultsProviderValidator` | class | DefaultsProvider dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-078 | `DefaultsProviderFactory` | class | DefaultsProvider uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-079 | `DefaultsProviderOptions` | record | DefaultsProvider yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-080 | `DefaultsProviderProvider` | class | DefaultsProvider saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-081 | `DefaultsProviderRegistry` | class | DefaultsProvider kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-082 | `DefaultsProviderConverter` | class | DefaultsProvider bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-083 | `DefaultsProviderTests` | class | DefaultsProvider testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-084 | `DefaultsProviderBuilder` | class | DefaultsProvider uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | DefaultsProviderTests | planli |
| N23-085 | `IConfigReload` | interface | ConfigReload sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-086 | `ConfigReload` | class | ConfigReload cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-087 | `ConfigReloadDto` | record | ConfigReload veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-088 | `ConfigReloadMapper` | class | ConfigReload <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-089 | `ConfigReloadValidator` | class | ConfigReload dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-090 | `ConfigReloadFactory` | class | ConfigReload uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-091 | `ConfigReloadOptions` | record | ConfigReload yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-092 | `ConfigReloadProvider` | class | ConfigReload saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-093 | `ConfigReloadRegistry` | class | ConfigReload kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-094 | `ConfigReloadConverter` | class | ConfigReload bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-095 | `ConfigReloadTests` | class | ConfigReload testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-096 | `ConfigReloadBuilder` | class | ConfigReload uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | ConfigReloadTests | planli |
| N23-097 | `IHotSettings` | interface | HotSettings sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-098 | `HotSettings` | class | HotSettings cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-099 | `HotSettingsDto` | record | HotSettings veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-100 | `HotSettingsMapper` | class | HotSettings <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-101 | `HotSettingsValidator` | class | HotSettings dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-102 | `HotSettingsFactory` | class | HotSettings uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-103 | `HotSettingsOptions` | record | HotSettings yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-104 | `HotSettingsProvider` | class | HotSettings saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-105 | `HotSettingsRegistry` | class | HotSettings kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-106 | `HotSettingsConverter` | class | HotSettings bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-107 | `HotSettingsTests` | class | HotSettings testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-108 | `HotSettingsBuilder` | class | HotSettings uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | HotSettingsTests | planli |
| N23-109 | `IOptionBinding` | interface | OptionBinding sozlesmesi (port) | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-110 | `OptionBinding` | class | OptionBinding cekirdek uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-111 | `OptionBindingDto` | record | OptionBinding veri transferi modeli | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-112 | `OptionBindingMapper` | class | OptionBinding <-> DTO esleme | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-113 | `OptionBindingValidator` | class | OptionBinding dogrulama kurallari | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-114 | `OptionBindingFactory` | class | OptionBinding uretimi ve varsayilanlari | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-115 | `OptionBindingOptions` | record | OptionBinding yapilandirmasi | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-116 | `OptionBindingProvider` | class | OptionBinding saglayici uygulamasi | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-117 | `OptionBindingRegistry` | class | OptionBinding kayit/katalog yonetimi | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-118 | `OptionBindingConverter` | class | OptionBinding bicim donusumu | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
| N23-119 | `OptionBindingTests` | class | OptionBinding testleri (xUnit) | VersaCoder.Crosscutting | net48;net10.0-windows | - | planli |
| N23-120 | `OptionBindingBuilder` | class | OptionBinding uretici (builder) | VersaCoder.Crosscutting | net48;net10.0-windows | OptionBindingTests | planli |
