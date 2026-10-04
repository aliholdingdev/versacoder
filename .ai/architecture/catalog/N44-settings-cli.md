---
title: Katalog Dugumu N44 - Ayarlar ve CLI
type: catalog-node
category: l7-ui
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: ayarlar formu ve komut satiri arayuzu
---

# Katalog Dugumu N44 - Ayarlar ve CLI

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l7 UI |
| Proje | VersaCoder.Desktop + VersaCoder.Cli |
| Kapsam | ayarlar formu ve komut satiri arayuzu |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l7-ui/N44-settings-cli.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Ayarlar formu satir bazinda ayar degerini dogrular ve geri alinabilir kaydeder.
- CLI programi argumanlari komut kaydindan cozer, komut metni kod icinde dagilir.
- Konsol renderer cikti bicimini tek yerden belirler, komutlar Console yazmaz.
- Cikis kodu haritasi her sonucu standart degerle dondurur, serbest kod yasaktir.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Ayar formunu ve CLI komut kabugunu uretir | Ayarin kendisini saklar (l4/l6 ise) |
| | l0 + l2 + l5 + l6 (l1 kabul) | - |
| | ekran, panel ve CLI etkilesimi (ne) | - |

## §2 Bagimlilik Yonu

| Yon | Durum | Aciklama |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l5 | EVET | asagidan tek yon |
| l6 | EVET | asagidan tek yon |
| l1 | HAYIR | yon asagi dogru |
| l3 + l4 | HAYIR | yon asagi dogru |

## §3 Aileler

| # | Aile | Sorumluluk |
| --- | --- | --- |
| 1 | SettingsForm | Ayarlar formu |
| 2 | SettingRow | Ayar satiri |
| 3 | LookUpSource | Bakim kaynagi |
| 4 | CliProgram | CLI program kabugu |
| 5 | ArgParser | Arguman ayrıştırıcı |
| 6 | CommandRegistry | Komut kaydi |
| 7 | ConsoleRenderer | Konsol renderlayici |
| 8 | HelpWriter | Yardim yazici |
| 9 | ExitCodeMap | Cikis kodu haritasi |
| 10 | VerbResolver | Fiil cozucu |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | SettingsForm | SettingsForm | genel |
| 2 | SettingsForm | SettingRow | theme |
| 3 | SettingRow | CliProgram | versacli |
| 4 | SettingRow | ArgParser | --path |
| 5 | LookUpSource | CommandRegistry | build |
| 6 | CliProgram | ExitCodeMap | 0 |

## §5 Yon Ozeti

| Katman | Izin | Yon |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l5 | EVET | asagidan tek yon |
| l6 | EVET | asagidan tek yon |
| l1 | HAYIR | yon asagi dogru |
| l3 + l4 | HAYIR | yon asagi dogru |

## §6 Testler

| Test | Aile | Dogruluk |
| --- | --- | --- |
| AyarDogrulamaTesti | SettingsForm | gecerli |
| ArgumanAyrıştırmaTesti | SettingsForm | ayrıştırılan |
| KomutCozmeTesti | ArgParser | bulunan |
| CikisKoduTesti | ArgParser | 0 |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Serbest konsol yazimi | daginik cikti | renderer zorunlu |
| Bilinmeyen arguman | yanlis komut | yardim mesaji |
| Cikis kodu serbestligi | belirsiz | harita zorunlu |

## §8 Disbaglanti

| Dis Kaynak | Yon | Not |
| --- | --- | --- |
| ADR-028 | - | VersaCoder.Desktop + VersaCoder.Cli baz projeleri |
| ADR-013 | - | 52 dugum |
| layer-matrix.md | referans | yon matrisi |
| N19-logging | - |

## §9 Kanit

| Kanit | Durum |
| --- | --- |
| Klasor | .ai/architecture/catalog/ (duz) |
| Dosya | N44-settings-cli.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Desktop\Settings |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N44-001 | `ISettingsForm` | interface | SettingsForm sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-002 | `SettingsForm` | class | SettingsForm cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-003 | `SettingsFormDto` | record | SettingsForm veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-004 | `SettingsFormMapper` | class | SettingsForm <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-005 | `SettingsFormValidator` | class | SettingsForm dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-006 | `SettingsFormFactory` | class | SettingsForm uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-007 | `SettingsFormOptions` | record | SettingsForm yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-008 | `SettingsFormProvider` | class | SettingsForm saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-009 | `SettingsFormRegistry` | class | SettingsForm kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-010 | `SettingsFormConverter` | class | SettingsForm bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-011 | `SettingsFormTests` | class | SettingsForm testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-012 | `SettingsFormBuilder` | class | SettingsForm uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | SettingsFormTests | planli |
| N44-013 | `ISettingRow` | interface | SettingRow sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-014 | `SettingRow` | class | SettingRow cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-015 | `SettingRowDto` | record | SettingRow veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-016 | `SettingRowMapper` | class | SettingRow <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-017 | `SettingRowValidator` | class | SettingRow dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-018 | `SettingRowFactory` | class | SettingRow uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-019 | `SettingRowOptions` | record | SettingRow yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-020 | `SettingRowProvider` | class | SettingRow saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-021 | `SettingRowRegistry` | class | SettingRow kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-022 | `SettingRowConverter` | class | SettingRow bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-023 | `SettingRowTests` | class | SettingRow testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-024 | `SettingRowBuilder` | class | SettingRow uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | SettingRowTests | planli |
| N44-025 | `ILookUpSource` | interface | LookUpSource sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-026 | `LookUpSource` | class | LookUpSource cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-027 | `LookUpSourceDto` | record | LookUpSource veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-028 | `LookUpSourceMapper` | class | LookUpSource <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-029 | `LookUpSourceValidator` | class | LookUpSource dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-030 | `LookUpSourceFactory` | class | LookUpSource uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-031 | `LookUpSourceOptions` | record | LookUpSource yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-032 | `LookUpSourceProvider` | class | LookUpSource saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-033 | `LookUpSourceRegistry` | class | LookUpSource kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-034 | `LookUpSourceConverter` | class | LookUpSource bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-035 | `LookUpSourceTests` | class | LookUpSource testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-036 | `LookUpSourceBuilder` | class | LookUpSource uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | LookUpSourceTests | planli |
| N44-037 | `ICliProgram` | interface | CliProgram sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-038 | `CliProgram` | class | CliProgram cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-039 | `CliProgramDto` | record | CliProgram veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-040 | `CliProgramMapper` | class | CliProgram <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-041 | `CliProgramValidator` | class | CliProgram dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-042 | `CliProgramFactory` | class | CliProgram uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-043 | `CliProgramOptions` | record | CliProgram yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-044 | `CliProgramProvider` | class | CliProgram saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-045 | `CliProgramRegistry` | class | CliProgram kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-046 | `CliProgramConverter` | class | CliProgram bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-047 | `CliProgramTests` | class | CliProgram testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-048 | `CliProgramBuilder` | class | CliProgram uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | CliProgramTests | planli |
| N44-049 | `IArgParser` | interface | ArgParser sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-050 | `ArgParser` | class | ArgParser cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-051 | `ArgParserDto` | record | ArgParser veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-052 | `ArgParserMapper` | class | ArgParser <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-053 | `ArgParserValidator` | class | ArgParser dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-054 | `ArgParserFactory` | class | ArgParser uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-055 | `ArgParserOptions` | record | ArgParser yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-056 | `ArgParserProvider` | class | ArgParser saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-057 | `ArgParserRegistry` | class | ArgParser kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-058 | `ArgParserConverter` | class | ArgParser bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-059 | `ArgParserTests` | class | ArgParser testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-060 | `ArgParserBuilder` | class | ArgParser uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | ArgParserTests | planli |
| N44-061 | `ICommandRegistry` | interface | CommandRegistry sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-062 | `CommandRegistry` | class | CommandRegistry cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-063 | `CommandRegistryDto` | record | CommandRegistry veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-064 | `CommandRegistryMapper` | class | CommandRegistry <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-065 | `CommandRegistryValidator` | class | CommandRegistry dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-066 | `CommandRegistryFactory` | class | CommandRegistry uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-067 | `CommandRegistryOptions` | record | CommandRegistry yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-068 | `CommandRegistryProvider` | class | CommandRegistry saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-069 | `CommandRegistryRegistry` | class | CommandRegistry kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-070 | `CommandRegistryConverter` | class | CommandRegistry bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-071 | `CommandRegistryTests` | class | CommandRegistry testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-072 | `CommandRegistryBuilder` | class | CommandRegistry uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | CommandRegistryTests | planli |
| N44-073 | `IConsoleRenderer` | interface | ConsoleRenderer sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-074 | `ConsoleRenderer` | class | ConsoleRenderer cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-075 | `ConsoleRendererDto` | record | ConsoleRenderer veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-076 | `ConsoleRendererMapper` | class | ConsoleRenderer <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-077 | `ConsoleRendererValidator` | class | ConsoleRenderer dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-078 | `ConsoleRendererFactory` | class | ConsoleRenderer uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-079 | `ConsoleRendererOptions` | record | ConsoleRenderer yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-080 | `ConsoleRendererProvider` | class | ConsoleRenderer saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-081 | `ConsoleRendererRegistry` | class | ConsoleRenderer kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-082 | `ConsoleRendererConverter` | class | ConsoleRenderer bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-083 | `ConsoleRendererTests` | class | ConsoleRenderer testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-084 | `ConsoleRendererBuilder` | class | ConsoleRenderer uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | ConsoleRendererTests | planli |
| N44-085 | `IHelpWriter` | interface | HelpWriter sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-086 | `HelpWriter` | class | HelpWriter cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-087 | `HelpWriterDto` | record | HelpWriter veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-088 | `HelpWriterMapper` | class | HelpWriter <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-089 | `HelpWriterValidator` | class | HelpWriter dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-090 | `HelpWriterFactory` | class | HelpWriter uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-091 | `HelpWriterOptions` | record | HelpWriter yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-092 | `HelpWriterProvider` | class | HelpWriter saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-093 | `HelpWriterRegistry` | class | HelpWriter kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-094 | `HelpWriterConverter` | class | HelpWriter bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-095 | `HelpWriterTests` | class | HelpWriter testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-096 | `HelpWriterBuilder` | class | HelpWriter uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | HelpWriterTests | planli |
| N44-097 | `IExitCodeMap` | interface | ExitCodeMap sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-098 | `ExitCodeMap` | class | ExitCodeMap cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-099 | `ExitCodeMapDto` | record | ExitCodeMap veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-100 | `ExitCodeMapMapper` | class | ExitCodeMap <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-101 | `ExitCodeMapValidator` | class | ExitCodeMap dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-102 | `ExitCodeMapFactory` | class | ExitCodeMap uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-103 | `ExitCodeMapOptions` | record | ExitCodeMap yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-104 | `ExitCodeMapProvider` | class | ExitCodeMap saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-105 | `ExitCodeMapRegistry` | class | ExitCodeMap kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-106 | `ExitCodeMapConverter` | class | ExitCodeMap bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-107 | `ExitCodeMapTests` | class | ExitCodeMap testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-108 | `ExitCodeMapBuilder` | class | ExitCodeMap uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | ExitCodeMapTests | planli |
| N44-109 | `IVerbResolver` | interface | VerbResolver sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-110 | `VerbResolver` | class | VerbResolver cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-111 | `VerbResolverDto` | record | VerbResolver veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-112 | `VerbResolverMapper` | class | VerbResolver <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-113 | `VerbResolverValidator` | class | VerbResolver dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-114 | `VerbResolverFactory` | class | VerbResolver uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-115 | `VerbResolverOptions` | record | VerbResolver yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-116 | `VerbResolverProvider` | class | VerbResolver saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-117 | `VerbResolverRegistry` | class | VerbResolver kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-118 | `VerbResolverConverter` | class | VerbResolver bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
| N44-119 | `VerbResolverTests` | class | VerbResolver testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N44-120 | `VerbResolverBuilder` | class | VerbResolver uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | VerbResolverTests | planli |
