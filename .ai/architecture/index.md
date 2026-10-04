---
title: "Mimari İndeks — 8 Makro Katman / 52 Düğüm / 9 Proje"
type: architecture
category: architecture
status: active
version: 1.1.0
updated: 2026-10-04
authority: SSOT
scope: "VersaCoder'in 8 makro katmani (l0..l7), N01-N44 dugum eslemesi, 9 uretim + 1-2 test projesi, 8x8 yon matrisi ve Tier/618 envanterinin kapsam disisi - ADR-028 birebir; ADR-030 v2 ile 4 kademe (tip/dugum/paket/yon eksenleri) baglantisi"
---

# Mimari İndeks — 8 Makro Katman / 52 Düğüm / 9 Proje

**Zorunlu Bağlantılar:** [CLAUDE](../CLAUDE.md) · [WORKFLOW](../WORKFLOW.md) · [keys](../keys.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-013](../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) · [ADR-030](../.decisions/accepted/ADR-030-5000-layers-class-level.md) · [layer-rules](../.rules/layer-rules.md) · [layer-5000](layer-5000/index.md) · [count-components.ps1](../scripts/count-components.ps1)

> **Tek cümlelik sorumluluk:** VersaCoder'in katmanları, projeleri, düğümleri ve bağımlılık yönü nerede, hangi sırayla ve hangi yasaklarla tanımlıdır sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

**VersaCoder 8 makro katman (l0..l7) + 44 alt katman = 52 düğüm olarak modellenir; üretim 9 proje + 1-2 test projesi, hepsi çift TFM `net48;net10.0-windows` (ADR-028 §2.1).** Yön sırası `l7 → l6 → l5 → l4 → l2 → l1 → l0`; `l3` yalnız `l0`'a bakar; `l2` ile `l4` arasında **doğrudan ok YOK** (repository portu `l0`'da, uygulaması `l4`'te). K3'ün 9 Tier / 618 DLL'lik envanteri kataloga **girmez**; tek izi [k3-618-ham-envanter](../reports/k3-618-ham-envanter.md) raporudur (`status: needs-validation`, `authority: derived`).

**Disk ölçümü (2026-09-29, dizin listesi):** `architecture/` = **10 alt klasör, 0 dosya**. `00-overview/`, `catalog/` ve `l0..l7/` klasörleri **henüz oluşturulmadı (boş)** → o klasör dosyalarına link verilmez; yönlendirme yalnız bu dosya üzerinden yapılır. Bu dosya ADR-028'i **birebir** yineler; değişiklik gerekiyorsa önce ADR-028 değişir, sonra burası güncellenir.

---

## §1 Kapsam

**İçinde:** ADR-028 §2.1–2.3'in mimari özeti (katman tablosu, proje listesi, yön matrisi, N01–N44 eşlemesi, 6 düzeltme), 52 düğümün sayım kuralı, alt klasör durumu.
**Dışında:** katalog dosyalarının içeriği (`catalog/` henüz yok), bileşen kayıt tabloları, kaynak kod, ADR metninin tamamı (tek kaynak: [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md)).

### §1.1 Disk Kanıtları

| # | İddia | Durum | Kanıt |
|---|---|---|---|
| 1 | `architecture/index.md` (bu dosya) | VAR | bu dosya, 2026-09-29 |
| 2 | `architecture/` altında 11 alt klasör | VAR | dizin listesi **2026-10-02** |
| 3 | Bu 11 klasörün hiçbirinde dosya | **VAR** (02 + 56 + 8 klasör + 10 layer-5000) | dizin listesi 2026-10-02 |
| 4 | `scripts/count-components.ps1` | VAR | `scripts/` dizin listesi |
| 5 | "52 dosya / 5440 kayıt / PASS" | ✅ **ÖLÇÜLDÜ** (2026-10-02, bu oturum) | `count-components.ps1` → `TOPLAM KAYIT 5440 (esik >= 5000) · Hatali format 0 · SONUC PASS · Kayit dosyasi 52` |

> **Ölçüm notu (2026-10-02):** §1.1/5'teki `DOĞRULAMA GEREKLİ` **kapatıldı**; komut bu oturumda yeniden çalıştırıldı. Katalog diskte: 53 dosya = 44 alt düğüm (`N01..N44`) + 8 makro (`N45..N52`) + `node-inventory.md`. İkinci sayac `gen-layer-5000.ps1` → **5834 PASS** (fark 394; [layer-5000](layer-5000/index.md)).

### §1.2 ADR-030 v2 — 4 kademe, 4 eksen (2026-10-02)

| Eksen | Ne sayar | Bugun | Kademe esikleri | Durum |
|---|---|---|---|---|
| **Tip (K)** | katalog kayit satiri | 5440 | 100 · 500 · 1000 · 5000 | SCALE-5000 **ASILDI** |
| **Dugum (N)** | namespace/klasor (N01..) | **44** alt (+8 makro) | 44 · 100 · 500 · 1000 · 5000 | SCALE-100 **ASILMADI** (eksik 56) |
| **Paket (P)** | `.csproj` | **9 + 0 test** | 11 · 16 · 23 · 40 | SCALE-100 **EKSIK** (`.sln`=0) |
| **Yon grubu (Y)** | matris satiri | **8** (`l0..l7`) | 8 · **12** · **16** · **20** | SCALE-100 TAM |

**3 ekseni KADeME DRIFT** (tip SCALE-5000'de, dugum SCALE-100 altinda, paket SCALE-100'de eksik) — kapanis sirasi [TODOS/06](../TODOS/06-5000-katman-kurulumu.md) K0 · A · B · C · D · K1.
**Yon grubu acilimi + 29 paket eslemesi:** [layer-rules §1.2](../.rules/layer-rules.md). **Kademe gecisi kurallari:** [layering-5000-spec §2.3](../spec/layering-5000-spec.md).

---

## §2 Sekiz Makro Katman (ADR-028 §2.1 birebir)

| # | Makro katman | N aralığı | Proje (.csproj) | Sorumluluk (alt katmanlar) | Neden var |
|---|---|---|---|---|---|
| 1 | **l0 Abstractions** | N01–N05 | `VersaCoder.Abstractions` | ports-core · result-error · events-abi · extensions · constants | Sözleşmelerin tek yaşadığı yer; DIP'i ve yön kuralını kuran katman |
| 2 | **l1 Domain** | N06–N11 | `VersaCoder.Domain` | entities · value-objects · aggregates · domain-events · invariants · enums | İş kuralları framework'ten bağımsız kalsın diye |
| 3 | **l2 Application** | N12–N18 | `VersaCoder.Application` | commands · queries · handlers · dto-mappers · validators · use-cases · scheduling | Kullanım senaryoları; UI ve veri erişiminden ayrı |
| 4 | **l3 Crosscutting** | N19–N24 | `VersaCoder.Crosscutting` | logging · caching · clock-time · crypto-sec · config-opts · resilience | Ortak kesit tek yerde dursun; katmanları sarmasın diye yalnız `l0`'a bakar |
| 5 | **l4 Infrastructure** | N25–N32 | `VersaCoder.Infrastructure` | ado-dapper · sqlite-provider · sqlserver-provider · migrations · fs-tools · git-adapter · llm-http · tool-runner | `l0` portlarının somut uygulaması; tek projede toplanınca şişmesin diye iç disiplinle denetlenir |
| 6 | **l5 Protocol** | N33–N37 | `VersaCoder.Protocol` | tool-schema · messages · session-transcript · loop-policy · versioning-errors | Ajan↔istemci sözleşmesi sürümleştirilebilsin diye ayrıldı |
| 7 | **l6 Host** | N38–N40 | `VersaCoder.Host` | bootstrap-pipeline · di-registration · config-bridges | Uygulamayı birleştirir, DI'ı ve yapılandırmayı köprüler |
| 8 | **l7 UI + CLI** | N41–N44 | `VersaCoder.Desktop` + `VersaCoder.Cli` | shell-ribbon · panels-explorer · editor-ai-chat · settings-cli | İki giriş yüzeyi (masaüstü + komut satırı) ortak çekirdeği paylaşır |
| — | test | — | `VersaCoder.Tests` (+ gerekirse `VersaCoder.ArchitectureTests`) | birim + mimari (NetArchTest) testleri | Yön kuralını ilk günden denetler |

**Proje sayımı:** 9 üretim + 1-2 test = **en fazla 11 proje** (ADR-028 §2.1). Yeni `.csproj` yalnız üç kapıdan geçer: (1) tek cümle sorumluluk, (2) bağımlılık yönü, (3) ayrı TFM/paket seti.

---

## §3 Üretim Projeleri ve TFM

| # | Proje | Tür | TFM | Not |
|---|---|---|---|---|
| 1 | `VersaCoder.Abstractions` | üretim | `net48;net10.0-windows` | portlar burada |
| 2 | `VersaCoder.Domain` | üretim | `net48;net10.0-windows` | — |
| 3 | `VersaCoder.Application` | üretim | `net48;net10.0-windows` | — |
| 4 | `VersaCoder.Crosscutting` | üretim | `net48;net10.0-windows` | tek yazım `Crosscutting` (NC-02'deki `CrossCutting` iptal) |
| 5 | `VersaCoder.Infrastructure` | üretim | `net48;net10.0-windows` | ADO.NET + Dapper (ADR-005) |
| 6 | `VersaCoder.Protocol` | üretim | `net48;net10.0-windows` | — |
| 7 | `VersaCoder.Host` | üretim | `net48;net10.0-windows` | — |
| 8 | `VersaCoder.Desktop` | üretim | `net48;net10.0-windows` | WinForms + DevExpress v26.1 |
| 9 | `VersaCoder.Cli` | üretim | `net48;net10.0-windows` | — |
| 10 | `VersaCoder.Tests` | test | çift bacak | zorunlu |
| 11 | `VersaCoder.ArchitectureTests` | test | çift bacak | gerekirse (NetArchTest `LayerRulesTests`) |

**Hedef sayısı:** 9+2 proje × 2 bacak = **22 hedef** (ADR-028 §4); K3'ün 575 csproj × 2 = ~1150 hedef senaryosu RED edilmiştir.

---

## §4 Yön Matrisi (8 × 8)

**Katman sırası (istif, ADR-028 §2.1):** `l7 → l6 → l5 → l4 → l2 → l1 → l0` · `l3` yalnız `l0`.

**Okuma:** satır = bağımlı olan, sütun = bağımlı olunan.

| satır \ sütun | l0 | l1 | l2 | l3 | l4 | l5 | l6 | l7 |
|---|---|---|---|---|---|---|---|---|
| **l0** | - | X | X | X | X | X | X | X |
| **l1** | OK | - | X | X | X | X | X | X |
| **l2** | . | OK | - | X | X | X | X | X |
| **l3** | OK | X | X | - | X | X | X | X |
| **l4** | ? | . | X | X | - | X | X | X |
| **l5** | . | . | . | X | ? | - | X | X |
| **l6** | . | . | . | X | . | OK | - | X |
| **l7** | . | . | . | X | . | . | OK | - |

| Lejant | Anlam |
|---|---|
| `OK` | belgeli izinli doğrudan kenar (ADR-028 §2.1 sırası) |
| `X` | belgeli yasak kenar — tek yön kuralı: alt katman üst katmana bakmaz, geri dönüş yalnız `l0` interface'leri ile |
| `?` | belge çelişkisi → **DOĞRULAMA GEREKLİ** (aşağıda §4.1) |
| `.` | tekil olarak belirtilmemiş; geçiş (transitive) |
| `-` | aynı katman |

### §4.1 Hücre Notları (çelişkiler dahil)

| # | Hücre | Kural | Kaynak |
|---|---|---|---|
| 1 | `l2 → l4` | **YOK** — repository portu `l0`'da, uygulaması `l4`'te | ADR-028 §2.1 |
| 2 | `l4 → l2` | **X** — sıradaki `l4 -> l2` oku **sıradır, kenar değildir** | ADR-028 §2.1 + §2.3/2 |
| 3 | `l3 → l1`, `l3 → l2`, `l3 → l4..l7` | **X** — `l3` yalnız `l0`'a bakar | ADR-028 §2.1 · ADR-013 §2.3 |
| 4 | `l5 → l4` | **?** — §2.1 zinciri `->` der, §2.3/2 `l5->l4 X` yazar; `00-overview/dependency-map.md` henüz yok | DOĞRULAMA GEREKLİ |
| 5 | `l4 → l0` | **?** — §2.3/6 portları (`IHttpGateway`, `IPermissionPrompt`, `IAgentStreamSink`, `IWorkbenchContract`) `l0`'a yazar; `l4`'ün doğrudan hedefi matris yazılınca kesinleşir | DOĞRULAMA GEREKLİ |

**Tek kaynak kuralı:** 8×8 matrisin kesin hâli `architecture/00-overview/dependency-map.md`'ye yazılacaktır (ADR-028 §2.3/2); o dosya üretilene kadar bu tablo geçerlidir ve **NetArchTest `LayerRulesTests` bu tabloya göre yazılır.**

---

## §5 N01–N44 Düğüm Eşlemesi

| N | Alt katman | Makro | Proje | N | Alt katman | Makro | Proje |
|---|---|---|---|---|---|---|---|
| N01 | ports-core | l0 | Abstractions | N23 | config-opts | l3 | Crosscutting |
| N02 | result-error | l0 | Abstractions | N24 | resilience | l3 | Crosscutting |
| N03 | events-abi | l0 | Abstractions | N25 | ado-dapper | l4 | Infrastructure |
| N04 | extensions | l0 | Abstractions | N26 | sqlite-provider | l4 | Infrastructure |
| N05 | constants | l0 | Abstractions | N27 | sqlserver-provider | l4 | Infrastructure |
| N06 | entities | l1 | Domain | N28 | migrations | l4 | Infrastructure |
| N07 | value-objects | l1 | Domain | N29 | fs-tools | l4 | Infrastructure |
| N08 | aggregates | l1 | Domain | N30 | git-adapter | l4 | Infrastructure |
| N09 | domain-events | l1 | Domain | N31 | llm-http | l4 | Infrastructure |
| N10 | invariants | l1 | Domain | N32 | tool-runner | l4 | Infrastructure |
| N11 | enums | l1 | Domain | N33 | tool-schema | l5 | Protocol |
| N12 | commands | l2 | Application | N34 | messages | l5 | Protocol |
| N13 | queries | l2 | Application | N35 | session-transcript | l5 | Protocol |
| N14 | handlers | l2 | Application | N36 | loop-policy | l5 | Protocol |
| N15 | dto-mappers | l2 | Application | N37 | versioning-errors | l5 | Protocol |
| N16 | validators | l2 | Application | N38 | bootstrap-pipeline | l6 | Host |
| N17 | use-cases | l2 | Application | N39 | di-registration | l6 | Host |
| N18 | scheduling | l2 | Application | N40 | config-bridges | l6 | Host |
| N19 | logging | l3 | Crosscutting | N41 | shell-ribbon | l7 | Desktop |
| N20 | caching | l3 | Crosscutting | N42 | panels-explorer | l7 | Desktop |
| N21 | clock-time | l3 | Crosscutting | N43 | editor-ai-chat | l7 | Desktop |
| N22 | crypto-sec | l3 | Crosscutting | N44 | settings-cli | l7 | Cli + Desktop |

**Seviye kuralı:** N01–N44 **klasör/namespace** düzeyindedir; `architecture/catalog/` altında birer dosyadır, birer `.csproj` değildir (ADR-028 §2.1).

---

## §6 52 Düğüm ve Sayım

```text
52 dugum = 44 alt katman (N01..N44) + 8 makro ozet dugumu (N45..N52)
sayim birimi = "| N##-### |" ile baslayan katalog tablo satiri
```

| # | Kural | Değer | Kaynak |
|---|---|---|---|
| 1 | Katalog dosyası | 44 alt + 8 makro + index = **53 dosya** | ADR-013 §2.2 |
| 2 | Kayıt eşiği | **≥ 5000** (44 × ≥112 + 8 × ≥20 = ≥5088) | ADR-013 §2.2 |
| 3 | Sayım aracı | `scripts/count-components.ps1` (geçerli desen `N\d{2}`) | ADR-013 §2.2 |
| 4 | Gate eşiği | **5440 = ölçülen 5440** (2026-10-02; marj 440 — tek satır silinirse RED) | ADR-028 §4 · ADR-030 v2 §2.1 |
| 5 | Bugünkü sonuç | **PASS — ÖLÇÜLDÜ 2026-10-02**: `TOPLAM KAYIT 5440 · Hatali format 0 · SONUC PASS` | bu dosya §1.1/5 |

**Ölçüm komutu:** `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\count-components.ps1` → çıktıyı §1.1/5 satırına yaz; sonra `status: active` ile onaylanır.

---

## §7 Tier / 618 DLL Envanteri = Kapsam Dışı (ADR-028 §2.2)

| # | Kural | Sonuç |
|---|---|---|
| 1 | K3'ün 618 DLL listesi kataloga, N-ID'lere ve `count-components.ps1` sayımına **girmez** | kapsam dışı |
| 2 | Tek iz | [k3-618-ham-envanter](../reports/k3-618-ham-envanter.md) (`status: needs-validation`, `authority: derived`) + ADR-028 §3 |
| 3 | Sayım birimi tektir | `| N##-### |` ile başlayan katalog tablo satırı |
| 4 | Türetilmiş/tier verisi | **sütun değil**, etiketli rapor satırıdır |

Bilinen hatalar (618 giriş / 616 benzersiz / 2 birebir tekrar / 9 başlıktan 2'si yanlış / 4 DLL dosyada yok / "nihai değil") tek yerde: [k3-618-ham-envanter](../reports/k3-618-ham-envanter.md).

---

## §8 Alt Klasör Durumu (2026-10-02 yeniden ölçüldü)

`architecture/` altında **11 klasör VAR ve doludur** (2026-09-29'daki "0 dosya" ölçümü artık geçersiz):

| # | Klasör | İçerik | Durum |
|---|---|---|---|
| 1 | `00-overview/` | `dependency-map.md` · `system-overview.md` | **VAR** (2 dosya) |
| 2 | `catalog/` | 52 düğüm dosyası (`N01..N52`) + `index.md` + `layer-matrix.md` + `node-inventory.md` + `project-map.md` = **56 dosya** | **VAR** |
| 3-11 | `l0-abstractions/` .. `l7-ui/` (8 klasör) + `layer-5000/` | Tier/parçacık dosyaları | **VAR** (`layer-5000/` 10 dosya) |

**Yönlendirme kuralı:** mimari sorularda önce bu dosya okunur; link verilen her hedef `Test-Path` ile doğrulanır (bozuk link = RED).

---

## §9 İlk Build Öncesi 6 Zorunlu Düzeltme (ADR-028 §2.3 özeti)

| # | Hedef | Düzeltme (tek satır) |
|---|---|---|
| 1 | `Directory.Packages.props` | `NetArchTest.Rules`, `xunit`, `Microsoft.NET.Test.Sdk`, `NSubstitute` `PackageVersion` satırları |
| 2 | `dependency-map.md` + ADR-013 eki | 8×8 matris tek kaynak: `l4->l2 X`, `l5->l4 X`, `l3` yalnız `l0` |
| 3 | `checklists/layer-boundary-check.md` | "Yedi katman" → "Sekiz katman (l0..l7)" + yön satırı matrisle aynı |
| 4 | `architecture/catalog/index.md` | N ↔ klasör eşleme tablosu (`N33 -> src/VersaCoder.Protocol/ToolSchema/`), dosya başına `node: N##` |
| 5 | kanıt komutları | tarama yolu gerçek proje adları (`src/VersaCoder.Domain/**`); `src/**/Domain/**` sahte PASS üretir |
| 6 | TFM + l0 portları | Desktop designer bacağı belirlenir; test projeleri çift TFM; 4 port `l0`'a yazılır |

**Tam metin ve kanıt sütunları:** [ADR-028 §2.3](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md). Hepsi **GATE-004a kapanışından önce** tamamlanır ([WORKFLOW](../WORKFLOW.md)).

Ek yazı kuralı: proje adı tek yazım `VersaCoder.Crosscutting`; semantik stale denetimi ("katalog = kod") gate metnine eklenir — `Durum` değil, **kodla eşleşme** kontrolü.

---

## §10 Doğrulama

| # | Kontrol | Beklenen |
|---|---|---|
| 1 | Bu dosyanın §2 tablosu ADR-028 §2.1 ile satır satır aynı | 8 makro + 9 üretim + 1-2 test |
| 2 | §5'te N01–N44 eksiksiz ve benzersiz | 44 satır, tekrar yok |
| 3 | §4 matrisi 8 satır × 8 sütun | 64 hücre, 5 lejant |
| 4 | `count-components.ps1` yeniden çalıştırıldı, sonuç §1.1/5'e yazıldı | ✅ **2026-10-02: 5440 PASS** — DOĞRULAMA GEREKLİ kalktı |
| 5 | `check-links.ps1` | bozuk link 0 |

- [ ] Frontmatter 8 alan + `updated: 2026-09-29`
- [ ] Emoji yok, Unicode kutu çizim yok, mojibake yok
- [ ] Boş klasör dosyalarına link yazılmadı (§8)

---

## §11 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Katmanlı mimari baz kararı | `.ai/.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | bu dosyanın tek kaynağı (SSOT) |
| 2 | 52 düğüm / katalog formatı | `.ai/.decisions/accepted/ADR-013-layered-architecture-52-nodes.md` | N kodları, 8 sütunlu kayıt formatı |
| 3 | Sayım aracı | `.ai/scripts/count-components.ps1` | kayıt sayımı, gate Q5 |
| 4 | Katman sınırı denetimi | `.ai/checklists/layer-boundary-check.md` | 13 soru (düzeltme #3) |
| 5 | K3 ham envanteri | `.ai/reports/k3-618-ham-envanter.md` | kapsam dışı 618 DLL listesinin tek izi |
| 6 | Kapsam analizi | `.ai/reports/00-kapsam-analizi.md` | 52 düğüm kararının kayıt yeri |
| 7 | Şablon envanteri | `.ai/.templates/index.md` | mimari şablonlarının kaydı |
