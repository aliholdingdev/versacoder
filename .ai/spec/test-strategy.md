---
title: "Test Stratejisi - VersaCoder"
type: spec
category: spec
status: active
version: 1.0.0
updated: 2026-09-25
authority: primary
scope: "Test piramidi, framework/runner kararı, kapsam hedefi (%90), sahte/gerçek sınırı, iki bacak test matrisi, gate kanıtları"
tags: [spec]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Test Stratejisi — VersaCoder

**Zorunlu Bağlantılar:** [[index|index]] · [[claude|CLAUDE.md]] · [[.rules/testing-rules|testing-rules]] · [[governance/workflow|WORKFLOW]] · [[spec/versacoder-spec|versacoder-spec]] · [[spec/tool-spec|tool-spec]] · [[spec/session-spec|session-spec]] · [[spec/protocol-spec|protocol-spec]]

> **Tek cümlelik sorumluluk:** *"Bu davranış gerçekten çalışıyor mu, hangi katmanda, hangi kanıtla"* sorusunun tek cevabıdır.
> **Kanıtsız "bitti" yasak** (ROLE.md §6): her DoD satırında çıktı dosyası ya da konsol kanıtı vardır.

---

## §0 Özet (Answer First)

**xUnit + NSubstitute + NetArchTest**, her iki bacakta (`net48` ve `net10.0-windows`). Hedef **satır kapsamı ≥ %90**; mimari kırılımı NetArchTest ile ölçülür. Veri katmanı testleri **gerçek SQLite şema** üzerinde (mock yok), LLM **yalnız sahte servisle** (gerçek çağrım — ADR-011 hattı harici). Gate kanıtı: GATE-005.

---

## §1 Kararlar

| # | Karar | Değer | Gerekçe |
|---|---|---|---|
| T1 | Test framework | **xUnit** | roleName: `tester` rolünün varsayılanı; paralel çalışır |
| T2 | Mock | **NSubstitute** | hafif, async destekli |
| T3 | Mimari test | **NetArchTest** | G1..G12 + katman izolasyonu otomatik |
| T4 | Kapsam | **satır ≥ %90** (rapor `reports/` altına) | DoD şartı |
| T5 | Runner | `dotnet test` (VSTest varsayılan) | iki bacakta da aynı komut |
| T6 | SQL sahtesi | **yok** — gerçek SQLite dosyası (temp) | ADR-005: sahte ORM davranışı gizler |
| T7 | LLM sahtesi | **ILLMProvider sahtesi** (NSubstitute) | maliyet + determinizm; gerçek doğrulama ayrı smoke test |
| T8 | WinForms | yalnız mantık (l2 handler) test edilir; Form olayları smoke | WinForms unit test kırılgan |

---

## §2 Test piramidi

```
+------------------------------------------------------------+
|  E2E / smoke  (%5)  — CLI uçtan uca, WinForms açılış       |
+------------------------------------------------------------+
|  Entegrasyon  (%25) — gerçek SQLite + Dapper + migration   |
|                      gerçek tool dosya I/O (temp dizin)    |
+------------------------------------------------------------+
|  Birim        (%70) — l1/l2 mantığı, l5 şema, l6 pipeline  |
|                      NSubstitute ile port sahteleri        |
+------------------------------------------------------------+
```

---

## §3 Katman ↔ test türü matrisi

| Katman | Birim | Entegrasyon | Mimari (NetArchTest) | Not |
|---|---|---|---|---|
| l0 Abstractions | tip/imza testi | — | ✓ (implementasyon yok) | yalnız sözleşme |
| l1 Domain | ✓ invariant | — | ✓ (BCL dışında referans yok) | saf C# |
| l2 Application | ✓ handler | ✓ (port sahtesiyle) | ✓ (l4'e bakamaz) | TDD öncelikli |
| l3 Crosscutting | ✓ | — | ✓ (yalnız l0'a bakar) | clock/log portu |
| l4 Infrastructure | ✓ (parser vb.) | **✓ gerçek DB** | ✓ (yalnız l6 bakabilir) | sahte DB yok |
| l5 Protocol | ✓ şema doğrulama | round-trip JSON | ✓ (l4'e bakamaz) | fail-closed testleri |
| l6 Host | ✓ pipeline sırası | ✓ açılış testi | ✓ (DI tek yer) | `#if` iki bacak |
| l7 UI | yalnız handler | smoke | ✓ (l1/l4'e bakamaz) | designer üçlüsü kontrolü |

---

## §4 İki bacak matrisi

| Bacak | Komut | Şart |
|---|---|---|
| `net48` | `dotnet test -f net48` | 0 hata (GATE-004/005) |
| `net10.0-windows` | `dotnet test -f net10.0-windows` | 0 hata |

> Orak kaynak, `#if` yalnız platform farkı (ADR-001). Test projeleri de **çift TFM**.

---

## §5 Adlandırma ve yerleşim

```
VersaCoder.Tests/
+- Domain/        DomainTests.cs          (l1)
+- Application/   <UseCase>Tests.cs       (l2)
+- Protocol/      <Şema>Tests.cs          (l5)
+- Host/          StartupPipelineTests.cs (l6)
+- Infrastructure/ Integration/           (l4 + gerçek SQLite)
+- Architecture/  LayerRulesTests.cs      (NetArchTest, G1..G12)
+- Cli/           CommandTests.cs         (l7 CLI)
```

- Dosya: `<SınıfUnderTest>Tests.cs`; metot: `Yap_ŞartBeklenen` (bkz. [[.rules/naming-conventions|naming-conventions]]).
- Her test tek davranışı sınar; `Assert` en az 1 (test-anti-patterns yasağı).

---

## §6 Gate kanıtları (GATE-005)
### §6.1 Build ortami notu (2026-10-02 olcumu — DX1001)

| # | Gercek | Kanit | Teste etkisi |
|---|---|---|---|
| 1 | Bu makine **build alabiliyor** (eski "build alamayiz" notu gecersiz) | `dotnet build src/VersaCoder.Cli -f net48` -> **0 Hata / 1.57 sn** (2026-10-02) | iki bacak build GATE-004 te calisabilir |
| 2 | DevExpress lisans anahtari **yok** | DX1001 uyari: `AppData\DevExpress\DevExpress_License.txt` indir deniyor | **tolerans mi RED mi** — kullanici karari; karar gelene kadar `UNKNOWN` (G9) |
| 3 | Uyari build i **dusurmedi** (0 Hata / 1 Uyari) | `dotnet build src/VersaCoder.Desktop -f net48` -> `0 Hata / 1 Uyari / 5.03 sn` | `TreatWarningsAsErrors` uygulanirsa RED olur; uyari **gizlenemez** (G11) |
| 4 | CLI ( DevExpress disi ) temiz | `dotnet build src/VersaCoder.Cli -f net10.0-windows` -> **0 Uyari / 0 Hata** | test piramidi etkilenmez |

**Kural:** DX1001 sayilmaz (build kaniti `0 Hata` sartir) ama gizlenemez — log.md de acik durur; lisans gelince bu satir `REPOSITORY_VERIFIED` olur.


| Kanıt | Çıktı | Şart |
|---|---|---|
| Birim testi | `dotnet test` konsol çıktısı | iki bacakta 0 failed |
| Kapsam raporu | `reports/` altına coverage çıktısı | ≥ %90 satır |
| Mimari test | `Architecture/LayerRulesTests.cs` | G1..G12 yeşil |
| SQL güvenlik | sahte/concat SQL taraması | 0 sonuç (G2) |
| Bozuk link | `scripts/check-links.ps1` | 0 (GATE-004a) |

---

## §7 Yapılmayanlar (scope dışı)

1. Gerçek ücretli LLM çağrısı (yalnız sahte + ayrı manuel smoke).
2. DevExpress görsel regresyon/fark testi (elle `ui-designer` onayı).
3. Performans yük testi — [[.rules/performance-guidelines|.rules/performance-guidelines]] kural seti ayrı; ölçüm PART 8.

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (tester + architect).
