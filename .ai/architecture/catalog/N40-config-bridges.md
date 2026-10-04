---
title: Katalog Dugumu N40 - Yapilandirma Kopruleri
type: catalog-node
category: l6-host
status: needs-validation
version: 1.0.0
updated: 2026-10-04
authority: derived
scope: yapilandirma, sifre ve ortam kopruleri
---

# Katalog Dugumu N40 - Yapilandirma Kopruleri

**Zorunlu Baglantilar:** index.md | node-inventory.md | ../index.md | ../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md | ../../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md

> Dugum, katman sozlesmesini cevaplar ve kanitini baglar.

## §0 Ozet (Answer First)

| Alan | Deger |
| --- | --- |
| Katman | l6 Host |
| Proje | VersaCoder.Host |
| Kapsam | yapilandirma, sifre ve ortam kopruleri |
| Kaynak | C:/www/versacoder/versacoder/.ai/architecture/catalog/l6-host/N40-config-bridges.md |
| Sorumluluk | Dugum, katman sozlesmesini cevaplar ve kanitini baglar. |

- Kopruler dis yapilandirmayi tek bir gorunumde toplar, katmanlar ham dosya veya ortam bilgisi gormez.
- Tfm koprusu hedef cerceve bilgisini, ortam koprusu degiskenleri saglar.
- Sifre koprusu gizli degerleri keser ve loglanmasini engeller.
- Tum kopru degerleri dogrulanir, gecersiz deger acilista hata verir.
- Kayitlar en az 6 aile 120 ornek ile kanitlanir, gerisi namludur (kayit yok).

## §1 YAPAR / YAPMAZ

| YAPAR | YAPMAZ |
| --- | --- |
| Yapilandirma koprulerini uretir | Yapilandirmayi kendisi saklar (l4 is) |
| | l0 + l1 + l2 + l4 + l5 (l3 kabul) | - |
| | baslangic, DI kayitlari ve yapilandirma koprusu | - |

## §2 Bagimlilik Yonu

| Yon | Durum | Aciklama |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l1 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l4 | EVET | asagidan tek yon |
| l5 | EVET | asagidan tek yon |
| l3 + l7 | HAYIR | yon asagi dogru |

## §3 Aileler

| # | Aile | Sorumluluk |
| --- | --- | --- |
| 1 | TfmBridge | Hedef cerceve koprusu |
| 2 | EnvBridge | Ortam degiskeni koprusu |
| 3 | SecretBridge | Sifre koprusu |
| 4 | PathBridge | Yol koprusu |
| 5 | LoggerBridge | Loglayici koprusu |
| 6 | ClockBridge | Saat koprusu |
| 7 | DbBridge | Veritabani koprusu |
| 8 | LlmBridge | LLM koprusu |
| 9 | UiBridge | Arayuz koprusu |
| 10 | CliBridge | CLI koprusu |

## §4 Ornek Kayit (seri 120)

| Kayit | Aile | Alan1 | Alan2 |
| --- | --- | --- | --- |
| 1 | TfmBridge | TfmBridge | net10.0 |
| 2 | TfmBridge | EnvBridge | PATH |
| 3 | EnvBridge | SecretBridge | maskeli |
| 4 | EnvBridge | PathBridge | repo root |
| 5 | SecretBridge | LoggerBridge | dosemeli |
| 6 | PathBridge | ClockBridge | utc |

## §5 Yon Ozeti

| Katman | Izin | Yon |
| --- | --- | --- |
| l0 | EVET | asagidan tek yon |
| l1 | EVET | asagidan tek yon |
| l2 | EVET | asagidan tek yon |
| l4 | EVET | asagidan tek yon |
| l5 | EVET | asagidan tek yon |
| l3 + l7 | HAYIR | yon asagi dogru |

## §6 Testler

| Test | Aile | Dogruluk |
| --- | --- | --- |
| TfmKopruTesti | TfmBridge | donen deger |
| SifreKopruTesti | TfmBridge | maskelenen |
| GecersizDegerTesti | LoggerBridge | yakalanan |
| DogrulamaTesti | LoggerBridge | gecen |

## §7 Riskler

| Risk | Etki | Onlem |
| --- | --- | --- |
| Sizinti | guvenlik | maskeli log |
| Gecersiz deger | acilis hatasi | acilista dogrulama |
| Daginik yapilandirma | tutarsiz | tek gorunum |

## §8 Disbaglanti

| Dis Kaynak | Yon | Not |
| --- | --- | --- |
| ADR-028 | - | VersaCoder.Host baz projeleri |
| ADR-013 | - | 52 dugum |
| layer-matrix.md | referans | yon matrisi |
| N19-logging | - |

## §9 Kanit

| Kanit | Durum |
| --- | --- |
| Klasor | .ai/architecture/catalog/ (duz) |
| Dosya | N40-config-bridges.md |
| Klasor ornegi | C:\versacoder\src\VersaCoder.Host |
| Ornek kayit | 6/6 hedef (seri 120 icin) |
| Test | 4/4 hedef |

kayit tablosu §10'e port edildi (kaynak vault 53 dosya / 5440 kayit); esik 5000 karsilanir.


## §10 Kayit Tablosu (ADR-013 2.2 - kaynak vault port)

| N40-001 | `ITfmBridge` | interface | TfmBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-002 | `TfmBridge` | class | TfmBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-003 | `TfmBridgeDto` | record | TfmBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-004 | `TfmBridgeMapper` | class | TfmBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-005 | `TfmBridgeValidator` | class | TfmBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-006 | `TfmBridgeFactory` | class | TfmBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-007 | `TfmBridgeOptions` | record | TfmBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-008 | `TfmBridgeProvider` | class | TfmBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-009 | `TfmBridgeRegistry` | class | TfmBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-010 | `TfmBridgeConverter` | class | TfmBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-011 | `TfmBridgeTests` | class | TfmBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-012 | `TfmBridgeBuilder` | class | TfmBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | TfmBridgeTests | planli |
| N40-013 | `IEnvBridge` | interface | EnvBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-014 | `EnvBridge` | class | EnvBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-015 | `EnvBridgeDto` | record | EnvBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-016 | `EnvBridgeMapper` | class | EnvBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-017 | `EnvBridgeValidator` | class | EnvBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-018 | `EnvBridgeFactory` | class | EnvBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-019 | `EnvBridgeOptions` | record | EnvBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-020 | `EnvBridgeProvider` | class | EnvBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-021 | `EnvBridgeRegistry` | class | EnvBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-022 | `EnvBridgeConverter` | class | EnvBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-023 | `EnvBridgeTests` | class | EnvBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-024 | `EnvBridgeBuilder` | class | EnvBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | EnvBridgeTests | planli |
| N40-025 | `ISecretBridge` | interface | SecretBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-026 | `SecretBridge` | class | SecretBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-027 | `SecretBridgeDto` | record | SecretBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-028 | `SecretBridgeMapper` | class | SecretBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-029 | `SecretBridgeValidator` | class | SecretBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-030 | `SecretBridgeFactory` | class | SecretBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-031 | `SecretBridgeOptions` | record | SecretBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-032 | `SecretBridgeProvider` | class | SecretBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-033 | `SecretBridgeRegistry` | class | SecretBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-034 | `SecretBridgeConverter` | class | SecretBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-035 | `SecretBridgeTests` | class | SecretBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-036 | `SecretBridgeBuilder` | class | SecretBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | SecretBridgeTests | planli |
| N40-037 | `IPathBridge` | interface | PathBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-038 | `PathBridge` | class | PathBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-039 | `PathBridgeDto` | record | PathBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-040 | `PathBridgeMapper` | class | PathBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-041 | `PathBridgeValidator` | class | PathBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-042 | `PathBridgeFactory` | class | PathBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-043 | `PathBridgeOptions` | record | PathBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-044 | `PathBridgeProvider` | class | PathBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-045 | `PathBridgeRegistry` | class | PathBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-046 | `PathBridgeConverter` | class | PathBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-047 | `PathBridgeTests` | class | PathBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-048 | `PathBridgeBuilder` | class | PathBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | PathBridgeTests | planli |
| N40-049 | `ILoggerBridge` | interface | LoggerBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-050 | `LoggerBridge` | class | LoggerBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-051 | `LoggerBridgeDto` | record | LoggerBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-052 | `LoggerBridgeMapper` | class | LoggerBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-053 | `LoggerBridgeValidator` | class | LoggerBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-054 | `LoggerBridgeFactory` | class | LoggerBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-055 | `LoggerBridgeOptions` | record | LoggerBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-056 | `LoggerBridgeProvider` | class | LoggerBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-057 | `LoggerBridgeRegistry` | class | LoggerBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-058 | `LoggerBridgeConverter` | class | LoggerBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-059 | `LoggerBridgeTests` | class | LoggerBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-060 | `LoggerBridgeBuilder` | class | LoggerBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | LoggerBridgeTests | planli |
| N40-061 | `IClockBridge` | interface | ClockBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-062 | `ClockBridge` | class | ClockBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-063 | `ClockBridgeDto` | record | ClockBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-064 | `ClockBridgeMapper` | class | ClockBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-065 | `ClockBridgeValidator` | class | ClockBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-066 | `ClockBridgeFactory` | class | ClockBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-067 | `ClockBridgeOptions` | record | ClockBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-068 | `ClockBridgeProvider` | class | ClockBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-069 | `ClockBridgeRegistry` | class | ClockBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-070 | `ClockBridgeConverter` | class | ClockBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-071 | `ClockBridgeTests` | class | ClockBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-072 | `ClockBridgeBuilder` | class | ClockBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | ClockBridgeTests | planli |
| N40-073 | `IDbBridge` | interface | DbBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-074 | `DbBridge` | class | DbBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-075 | `DbBridgeDto` | record | DbBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-076 | `DbBridgeMapper` | class | DbBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-077 | `DbBridgeValidator` | class | DbBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-078 | `DbBridgeFactory` | class | DbBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-079 | `DbBridgeOptions` | record | DbBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-080 | `DbBridgeProvider` | class | DbBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-081 | `DbBridgeRegistry` | class | DbBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-082 | `DbBridgeConverter` | class | DbBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-083 | `DbBridgeTests` | class | DbBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-084 | `DbBridgeBuilder` | class | DbBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | DbBridgeTests | planli |
| N40-085 | `ILlmBridge` | interface | LlmBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-086 | `LlmBridge` | class | LlmBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-087 | `LlmBridgeDto` | record | LlmBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-088 | `LlmBridgeMapper` | class | LlmBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-089 | `LlmBridgeValidator` | class | LlmBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-090 | `LlmBridgeFactory` | class | LlmBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-091 | `LlmBridgeOptions` | record | LlmBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-092 | `LlmBridgeProvider` | class | LlmBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-093 | `LlmBridgeRegistry` | class | LlmBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-094 | `LlmBridgeConverter` | class | LlmBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-095 | `LlmBridgeTests` | class | LlmBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-096 | `LlmBridgeBuilder` | class | LlmBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | LlmBridgeTests | planli |
| N40-097 | `IUiBridge` | interface | UiBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-098 | `UiBridge` | class | UiBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-099 | `UiBridgeDto` | record | UiBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-100 | `UiBridgeMapper` | class | UiBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-101 | `UiBridgeValidator` | class | UiBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-102 | `UiBridgeFactory` | class | UiBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-103 | `UiBridgeOptions` | record | UiBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-104 | `UiBridgeProvider` | class | UiBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-105 | `UiBridgeRegistry` | class | UiBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-106 | `UiBridgeConverter` | class | UiBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-107 | `UiBridgeTests` | class | UiBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-108 | `UiBridgeBuilder` | class | UiBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | UiBridgeTests | planli |
| N40-109 | `ICliBridge` | interface | CliBridge sozlesmesi (port) | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-110 | `CliBridge` | class | CliBridge cekirdek uygulamasi | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-111 | `CliBridgeDto` | record | CliBridge veri transferi modeli | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-112 | `CliBridgeMapper` | class | CliBridge <-> DTO esleme | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-113 | `CliBridgeValidator` | class | CliBridge dogrulama kurallari | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-114 | `CliBridgeFactory` | class | CliBridge uretimi ve varsayilanlari | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-115 | `CliBridgeOptions` | record | CliBridge yapilandirmasi | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-116 | `CliBridgeProvider` | class | CliBridge saglayici uygulamasi | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-117 | `CliBridgeRegistry` | class | CliBridge kayit/katalog yonetimi | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-118 | `CliBridgeConverter` | class | CliBridge bicim donusumu | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
| N40-119 | `CliBridgeTests` | class | CliBridge testleri (xUnit) | VersaCoder.Host | net48;net10.0-windows | - | planli |
| N40-120 | `CliBridgeBuilder` | class | CliBridge uretici (builder) | VersaCoder.Host | net48;net10.0-windows | CliBridgeTests | planli |
