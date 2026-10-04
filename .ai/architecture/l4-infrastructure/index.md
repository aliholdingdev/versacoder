---
title: "l4 Infrastructure — Katman Şartnamesi (VersaCoder)"
type: architecture
category: l4-infrastructure
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "l4 Infrastructure katmanı: sorumluluk sınırı, girenler/çıkanlar, ADR-028 yasak yönleri, N25-N32 + N49, VersaCoder.Infrastructure, bağımlılık listesi ve test stratejisi"
---

# l4 Infrastructure — Katman Şartnamesi

**Zorunlu Baglantilar:** [mimari indeks](../index.md) · [katalog](../catalog/index.md) · [bagimlilik haritasi](../00-overview/dependency-map.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [layer-boundary-check](../../checklists/layer-boundary-check.md) · [test stratejisi](../../spec/test-strategy.md)

> **Tek cumlelik sorumluluk:** *Uygulamanın fiziksel dünyayla (veritabanı, dosya, süreç, git, LLM ağı) temasının nerede, hangi portlarla ve hangi yasaklarla kurulduğu* sorusunun tek cevabıdır.

---

## §0 Ozet (Answer First)

l4, **fiziksel dünyayla temas eden tek katmandır**: `l0` portlarının somut uygulayıcısıdır, çağrılan taraftır ve yukarıya yalnız `Result<T>` ile domain event döner. SQL yalnız burada yazılır (ADO.NET + Dapper, parametreli, `CommandTimeout` zorunlu), migration saf SQL + checksum ile yürür, dosya yazma atomiktir. Katman `l4 -> l2` ve `l4 -> l5/l6/l7` kenarlarını **kullanamaz** (ADR-028 §2.1); `l2 -> l4` de yasaktır — iki katman arasında doğrudan ok yoktur, port `l0`'da, uygulaması `l4`'tedir. Klasördeki 10 dosya: bu şartname (`index.md`), N25..N32 alt düğümleri ve N49 makro düğümü.

---

## §1 Sorumluluk

**Tablo A — Bu katman NE YAPAR**

| # | Sorumluluk | Kanıt | Çıktı |
|---|---|---|---|
| A1 | `l0` portlarını uygulamak (`IDbConnectionFactory`, `IUnitOfWork`, `IFileSystemStore`, `IGitClient`) | implementasyon derlenir, port mock ile test edilir | `.cs` adapter |
| A2 | Parametreli SQL yürütmek (`@p1..@pN`), her komutta `CommandTimeout` | kod taramasında concat SQL = 0 | sorgu |
| A3 | Ham istisna taşımadan `Result<T>`'ye eşlemek | her istisna tabloda bir satır bulur | `Result<T>` |
| A4 | Migration'ı saf SQL + `__Migrations` checksum ile uygulamak | değişen dosya runner'ı durdurur | `MigrationReport` |
| A5 | Atomik dosya yazma ve sınırlı timeout'lu süreç çalıştırmak | `.tmp` + `File.Replace`; kill testi | dosya / `ToolResult` |

**Tablo B — Bu katman NE YAPMAZ**

| # | Yapmaz | Neden | İhlalde |
|---|---|---|---|
| B1 | Use case, DTO, komut/query tanımı (l2) | katman yönü (ADR-028) | NetArchTest RED |
| B2 | Mesaj/şema doğrulama (l5) | şema ≠ eylem | gate RED |
| B3 | DI kaydı / config yükleme (l6) | composition root tekeli | NetArchTest RED |
| B4 | Ekran, ribbon, mesaj kutusu (l7) | sunum katmanın işi değil | review RED |
| B5 | EF Core / `DbContext` / string concat SQL | ADO.NET + Dapper (ADR-005) | G2 / G5 RED |

---

## §2 Bu Katmana GIRENLER

| # | Giren | Kaynak | Hangi kapıdan geçer |
|---|---|---|---|
| 1 | Port çağrısı (`IUnitOfWork`, `IRepository`) | `l2` use case | `l0` arayüzü, somut tip yok |
| 2 | Bağlantı ayarı (şifreli `ConnectionString`) | `l6` config köprüsü (N40) | DPAPI çözülmüş değer |
| 3 | Migration tetikleyici | `l6` açılış borusu (N38) | `IMigrationRunner.RunAsync` |
| 4 | Araç çağrısı (`ToolCall`) | `l5` şeması (N33) | geçerli `ToolResult` şeması |
| 5 | Ekran niyeti (`commandId`) | `l7` | `ICommandDispatcher` (l2) üzerinden |

---

## §3 CIKANLAR

| # | Çıkan | Alıcı | Biçim |
|---|---|---|---|
| 1 | `Result<T>` / `Result.Fail(...)` | `l2`, `l6` | tek dönüş tipi |
| 2 | Domain event yayını | `l6` subscriber | `l0` events-abi |
| 3 | Kalıcı veri (`versacoder.db`) | dosya | parametreli SQL |
| 4 | `MigrationReport` | `l6` | ad + checksum + süre |
| 5 | Log kaydı | `l3` `ILogSink` portu | süre + satır (hassas değer yok) |

---

## §4 Yasak Yonler (ADR-028 matrisi)

```text
  ust katmanlar (bakamaz)
        l7   l6   l5   l4
 l4  |   X    X    X    -
        |
        v  bakabilir (goreceli): l0 portlari + izinli alt tipler
```

| Yön hücrei | Değer | Anlam |
|---|---|---|
| `l4 -> l2` | **X** | sıradaki `l4 -> l2` oku sıralamadır, kenar değildir |
| `l4 -> l5`, `l4 -> l6`, `l4 -> l7` | **X** | üst katmana hiçbir koşulda bakış |
| `l2 -> l4` | **YOK** | port `l0`'da, uygulaması `l4`'te; doğrudan ok yok |
| `l4 -> l3` | **X** (matris) | şartname `l3`'ü bakılabilir sayar → `DOĞRULAMA GEREKLİ`, `dependency-map.md` kazanır |
| `l4 -> l0` | `?` | ADR-028 §2.3/6 portları `l0`'a yazar → `DOĞRULAMA GEREKLİ` |

---

## §5 N Kodlari

| N | Alt katman | N | Alt katman |
|---|---|---|---|
| N25 | ado-dapper | N29 | fs-tools |
| N26 | sqlite-provider | N30 | git-adapter |
| N27 | sqlserver-provider | N31 | llm-http |
| N28 | migrations | N32 | tool-runner |
| N49 | makro l4 (5 kit) | — | — |

Seviye kuralı: N kodları klasör/namespace düzeyindedir, birer `.csproj` değildir.

---

## §6 Projeler

| Proje | TFM | Rol |
|---|---|---|
| `VersaCoder.Infrastructure` | `net48;net10.0-windows` | bu katmanın tek projesi (9 üretimden biri) |
| `VersaCoder.Tests` | çift bacak | unit + entegrasyon (`Category=Infrastructure`) |
| `VersaCoder.ArchitectureTests` | çift bacak | NetArchTest yön denetimi |

`net48` ve `net10.0-windows` **aynı kaynak dosyayı** paylaşır; `#if` yalnız platform farkındadır (iş mantığı, SQL metni, provider seçimi `#if` içine girmez).

---

## §7 Bagimlilik Listesi

| Hedef | Referans | Koşul |
|---|---|---|
| `VersaCoder.Abstractions` (l0) | portlar, `Result`, `EntityId` | doğrudan `ProjectReference` |
| `VersaCoder.Domain` (l1) | aggregate/entity tipleri | matriste `.` → geçişli, `DOĞRULAMA GEREKLİ` |
| `VersaCoder.Crosscutting` (l3) | `ILogSink`, `ICacheStore`, `ISystemClock` | matriste `X` → `dependency-map.md` çözülünce kesinleşir |
| BCL + CPM paketleri | ADO.NET, `Microsoft.Data.Sqlite`, `Microsoft.Data.SqlClient`, Dapper | sürüm `Directory.Packages.props` |

**Dış bağımlılık yasağı:** EF Core, Npgsql, MySql, Mongo paketleri bu projede geçersizdir (ADR-005 / ADR-028 §2.1).

---

## §8 Test Stratejisi

| Test | Ne kapsar | Hedef |
|---|---|---|
| Unit (`VersaCoder.Tests`) | port mock, `Result` eşlemeleri, atomik yazma | **%90** satır |
| Integration | gerçek `versacoder.db`, `@pN` sorgular, migration sırası | her port ≥ 1 yol |
| Arch (NetArchTest) | `l4 -> l2/l5/l6/l7` kenarı, concat SQL, EF referansı | **0 ihlal** |
| Perf | sorgu ≤ 50 ms, açık bağlantı 0 | bütçe aşılmaz |

Koşum: `dotnet test --filter Category=Infrastructure` · 13 soru için [layer-boundary-check](../../checklists/layer-boundary-check.md).

---

## §9 Kanit

| # | İddia | Kanıt komutu | Beklenen |
|---|---|---|---|
| 1 | Dosya boyutu 2.600–8.500 bayt | `Get-Item` ile `.Length` | aralık içinde |
| 2 | Bozuk link yok | `powershell -NoProfile -ExecutionPolicy Bypass -File C:\versacoder\.ai\scripts\check-links.ps1` | `0` |
| 3 | Frontmatter 9 alan, ilk satır `---` | gözden geçirme | 9/9 |
| 4 | N25..N32 + N49 + index = 10 dosya | klasör listesi | 10 |
| 5 | Emoji / Unicode kutu / mojibake yok | `measure-vault.ps1` | `0` |

`src/` henüz diskte olmadığından (§6.3) kod bazlı ihlal sayıları `DOĞRULAMA GEREKLİ` kalır; ilk build sonrası NetArchTest çıktısı bu tabloya işlenir.
