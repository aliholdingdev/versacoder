---
title: Katalog Dugumu N41 - Shell ve Ribbon
type: catalog-node
category: l7-ui
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: masaustu kabuk, ribbon ve durum cubugu
---

# Katalog Dugumu N41 - Shell ve Ribbon

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l7 UI |
| Proje | VersaCoder.Desktop |
| Kapsam | masaustu kabuk, ribbon ve durum cubugu |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l7-ui/N41-shell-ribbon.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Shell; ribbon, komut cubugu, durum cubugu ve dock hostunu tek pencerede toplar.
- Ribbon grubu ve dugmeleri komut kaydina baglidir, dogrudan is yapmaz.
- Tema yoneticisi ve duzen onaylari kayitli kalipb, uygulama yeniden acilinca geri gelir.
- Kisayol eslemesi komut kaydindan uretir, ekranda tekrar tanimlanmaz.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Shell iskeletini, ribbon grubunu ve komut baglarini uretir | Is mantigi yurutur (l2/l5 ise) |
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
| 1 | RibbonForm | Ana pencere kabugu |
| 2 | RibbonModule | Ribbon modul kaydi |
| 3 | RibbonGroup | Ribbon grubu |
| 4 | RibbonButton | Komut dugmesi |
| 5 | StatusBarHost | Durum cubugu hostu |
| 6 | DockHost | Panel demirleme hostu |
| 7 | ThemeManager | Tema yoneticisi |
| 8 | CommandBar | Komut cubugu |
| 9 | LayoutPreset | Duzen onayi |
| 10 | KeyBinding | Kisayol eslemesi |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | RibbonForm | RibbonForm | ana pencere |
| 2 | RibbonForm | RibbonModule | view sekmesi |
| 3 | RibbonModule | RibbonGroup | dosya |
| 4 | RibbonModule | RibbonButton | kaydet |
| 5 | RibbonGroup | StatusBar | hazir |
| 6 | RibbonButton | LayoutPreset | varsayilan |

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
| ShellKurulumTesti | RibbonForm | olusturulan |
| TemaDegistirmeTesti | RibbonForm | degisen |
| KomutBagTesti | StatusBarHost | baglanan |
| KisayolTesti | StatusBarHost | eslesen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Komut sarmalayici | iki is | tek komut kaydi |
| Dock kaybi | panel kaybi | preset kaydi |
| Tema tutarsizligi | gorunum | tek tema kaynagi |

## §8 Disbaglanti

| Dis Kaynak | Yon | Not |
| --- | --- | --- |
| ADR-028 | - | VersaCoder.Desktop baz projeleri |
| ADR-013 | - | 52 dugum |
| layer-matrix.md | referans | yon matrisi |
| N19-logging | - |

## §9 Kanit

| Kanit | Durum |
| --- | --- |
| Klasor | .ai/architecture/catalog/ (duz) |
| Dosya | N41-shell-ribbon.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Desktop |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N41-001 | `IRibbonForm` | interface | RibbonForm sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-002 | `RibbonForm` | class | RibbonForm cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-003 | `RibbonFormDto` | record | RibbonForm veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-004 | `RibbonFormMapper` | class | RibbonForm <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-005 | `RibbonFormValidator` | class | RibbonForm dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-006 | `RibbonFormFactory` | class | RibbonForm uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-007 | `RibbonFormOptions` | record | RibbonForm yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-008 | `RibbonFormProvider` | class | RibbonForm saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-009 | `RibbonFormRegistry` | class | RibbonForm kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-010 | `RibbonFormConverter` | class | RibbonForm bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-011 | `RibbonFormTests` | class | RibbonForm testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-012 | `RibbonFormBuilder` | class | RibbonForm uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | RibbonFormTests | planli |
| N41-013 | `IRibbonModule` | interface | RibbonModule sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-014 | `RibbonModule` | class | RibbonModule cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-015 | `RibbonModuleDto` | record | RibbonModule veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-016 | `RibbonModuleMapper` | class | RibbonModule <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-017 | `RibbonModuleValidator` | class | RibbonModule dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-018 | `RibbonModuleFactory` | class | RibbonModule uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-019 | `RibbonModuleOptions` | record | RibbonModule yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-020 | `RibbonModuleProvider` | class | RibbonModule saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-021 | `RibbonModuleRegistry` | class | RibbonModule kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-022 | `RibbonModuleConverter` | class | RibbonModule bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-023 | `RibbonModuleTests` | class | RibbonModule testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-024 | `RibbonModuleBuilder` | class | RibbonModule uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | RibbonModuleTests | planli |
| N41-025 | `IRibbonGroup` | interface | RibbonGroup sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-026 | `RibbonGroup` | class | RibbonGroup cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-027 | `RibbonGroupDto` | record | RibbonGroup veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-028 | `RibbonGroupMapper` | class | RibbonGroup <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-029 | `RibbonGroupValidator` | class | RibbonGroup dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-030 | `RibbonGroupFactory` | class | RibbonGroup uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-031 | `RibbonGroupOptions` | record | RibbonGroup yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-032 | `RibbonGroupProvider` | class | RibbonGroup saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-033 | `RibbonGroupRegistry` | class | RibbonGroup kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-034 | `RibbonGroupConverter` | class | RibbonGroup bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-035 | `RibbonGroupTests` | class | RibbonGroup testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-036 | `RibbonGroupBuilder` | class | RibbonGroup uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | RibbonGroupTests | planli |
| N41-037 | `IRibbonButton` | interface | RibbonButton sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-038 | `RibbonButton` | class | RibbonButton cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-039 | `RibbonButtonDto` | record | RibbonButton veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-040 | `RibbonButtonMapper` | class | RibbonButton <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-041 | `RibbonButtonValidator` | class | RibbonButton dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-042 | `RibbonButtonFactory` | class | RibbonButton uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-043 | `RibbonButtonOptions` | record | RibbonButton yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-044 | `RibbonButtonProvider` | class | RibbonButton saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-045 | `RibbonButtonRegistry` | class | RibbonButton kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-046 | `RibbonButtonConverter` | class | RibbonButton bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-047 | `RibbonButtonTests` | class | RibbonButton testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-048 | `RibbonButtonBuilder` | class | RibbonButton uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | RibbonButtonTests | planli |
| N41-049 | `IStatusBarHost` | interface | StatusBarHost sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-050 | `StatusBarHost` | class | StatusBarHost cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-051 | `StatusBarHostDto` | record | StatusBarHost veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-052 | `StatusBarHostMapper` | class | StatusBarHost <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-053 | `StatusBarHostValidator` | class | StatusBarHost dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-054 | `StatusBarHostFactory` | class | StatusBarHost uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-055 | `StatusBarHostOptions` | record | StatusBarHost yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-056 | `StatusBarHostProvider` | class | StatusBarHost saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-057 | `StatusBarHostRegistry` | class | StatusBarHost kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-058 | `StatusBarHostConverter` | class | StatusBarHost bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-059 | `StatusBarHostTests` | class | StatusBarHost testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-060 | `StatusBarHostBuilder` | class | StatusBarHost uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | StatusBarHostTests | planli |
| N41-061 | `IDockHost` | interface | DockHost sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-062 | `DockHost` | class | DockHost cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-063 | `DockHostDto` | record | DockHost veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-064 | `DockHostMapper` | class | DockHost <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-065 | `DockHostValidator` | class | DockHost dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-066 | `DockHostFactory` | class | DockHost uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-067 | `DockHostOptions` | record | DockHost yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-068 | `DockHostProvider` | class | DockHost saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-069 | `DockHostRegistry` | class | DockHost kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-070 | `DockHostConverter` | class | DockHost bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-071 | `DockHostTests` | class | DockHost testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-072 | `DockHostBuilder` | class | DockHost uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | DockHostTests | planli |
| N41-073 | `IThemeManager` | interface | ThemeManager sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-074 | `ThemeManager` | class | ThemeManager cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-075 | `ThemeManagerDto` | record | ThemeManager veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-076 | `ThemeManagerMapper` | class | ThemeManager <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-077 | `ThemeManagerValidator` | class | ThemeManager dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-078 | `ThemeManagerFactory` | class | ThemeManager uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-079 | `ThemeManagerOptions` | record | ThemeManager yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-080 | `ThemeManagerProvider` | class | ThemeManager saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-081 | `ThemeManagerRegistry` | class | ThemeManager kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-082 | `ThemeManagerConverter` | class | ThemeManager bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-083 | `ThemeManagerTests` | class | ThemeManager testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-084 | `ThemeManagerBuilder` | class | ThemeManager uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | ThemeManagerTests | planli |
| N41-085 | `ICommandBar` | interface | CommandBar sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-086 | `CommandBar` | class | CommandBar cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-087 | `CommandBarDto` | record | CommandBar veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-088 | `CommandBarMapper` | class | CommandBar <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-089 | `CommandBarValidator` | class | CommandBar dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-090 | `CommandBarFactory` | class | CommandBar uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-091 | `CommandBarOptions` | record | CommandBar yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-092 | `CommandBarProvider` | class | CommandBar saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-093 | `CommandBarRegistry` | class | CommandBar kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-094 | `CommandBarConverter` | class | CommandBar bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-095 | `CommandBarTests` | class | CommandBar testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-096 | `CommandBarBuilder` | class | CommandBar uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | CommandBarTests | planli |
| N41-097 | `ILayoutPreset` | interface | LayoutPreset sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-098 | `LayoutPreset` | class | LayoutPreset cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-099 | `LayoutPresetDto` | record | LayoutPreset veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-100 | `LayoutPresetMapper` | class | LayoutPreset <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-101 | `LayoutPresetValidator` | class | LayoutPreset dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-102 | `LayoutPresetFactory` | class | LayoutPreset uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-103 | `LayoutPresetOptions` | record | LayoutPreset yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-104 | `LayoutPresetProvider` | class | LayoutPreset saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-105 | `LayoutPresetRegistry` | class | LayoutPreset kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-106 | `LayoutPresetConverter` | class | LayoutPreset bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-107 | `LayoutPresetTests` | class | LayoutPreset testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-108 | `LayoutPresetBuilder` | class | LayoutPreset uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | LayoutPresetTests | planli |
| N41-109 | `IKeyBinding` | interface | KeyBinding sozlesmesi (port) | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-110 | `KeyBinding` | class | KeyBinding cekirdek uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-111 | `KeyBindingDto` | record | KeyBinding veri transferi modeli | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-112 | `KeyBindingMapper` | class | KeyBinding <-> DTO esleme | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-113 | `KeyBindingValidator` | class | KeyBinding dogrulama kurallari | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-114 | `KeyBindingFactory` | class | KeyBinding uretimi ve varsayilanlari | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-115 | `KeyBindingOptions` | record | KeyBinding yapilandirmasi | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-116 | `KeyBindingProvider` | class | KeyBinding saglayici uygulamasi | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-117 | `KeyBindingRegistry` | class | KeyBinding kayit/katalog yonetimi | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-118 | `KeyBindingConverter` | class | KeyBinding bicim donusumu | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
| N41-119 | `KeyBindingTests` | class | KeyBinding testleri (xUnit) | VersaCoder.Desktop | net48;net10.0-windows | - | planli |
| N41-120 | `KeyBindingBuilder` | class | KeyBinding uretici (builder) | VersaCoder.Desktop | net48;net10.0-windows | KeyBindingTests | planli |
