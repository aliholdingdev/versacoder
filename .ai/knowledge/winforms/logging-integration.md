---
title: "logging-integration - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-29
authority: primary
scope: "WinForms uygulamasinda log yazimi: kutudan cikan Trace/TextWriterTraceListener hatti, dosya konumu/rotasyonu, sirlari loglamama ve net48 + net10.0-windows farklari"
tags: [knowledge]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# logging-integration - WinForms icin log hatti (paket gerekmez)

**Zorunlu Baglantilar:** [vault indeks](../index.md) · [logging-rules](../../.rules/logging-rules.md) · [security-rules](../../.rules/security-rules.md) · [settings-user-config](settings-user-config.md) · [composition-root-host-startup](composition-root-host-startup.md)
**Küme + Hub:** [winforms index](index.md) · [index](../../index.md) · [CLAUDE](../../claude.md) · [AGENTS](../../definitions/agents.md) · [brain](../../.decisions/brain.md) · [glossary](../../definitions/glossary.md) · [keys](../../governance/keys.md)
**Kardeşler (winforms · 3/32):** [accessibility-tab-and-mnemonics](accessibility-tab-and-mnemonics.md) · [anchor-dock-layout-rules](anchor-dock-layout-rules.md) · [async-await-ui-thread](async-await-ui-thread.md) · [async-loading-patterns](async-loading-patterns.md) · [async-ui-thread-marshalling](async-ui-thread-marshalling.md) · [async-void-event-handlers](async-void-event-handlers.md) · [backgroundworker-vs-task](backgroundworker-vs-task.md) · [cancellation-in-ui](cancellation-in-ui.md) · [cancellation-token-in-ui](cancellation-token-in-ui.md) · [control-disposal-and-memory](control-disposal-and-memory.md) · [controls-lifecycle-events](controls-lifecycle-events.md) · [data-binding-basics](data-binding-basics.md) · [designer-cs-anatomy](designer-cs-anatomy.md) · [designer-cs-trio-rule](designer-cs-trio-rule.md) · [designer-editing-rules](designer-editing-rules.md) · [designer-hand-edit-policy](designer-hand-edit-policy.md) · [di-container-lifetimes](di-container-lifetimes.md) · [docking-panels-patterns](docking-panels-patterns.md) · [errorprovider-validation-flow](errorprovider-validation-flow.md) · [exception-boundary-ui](exception-boundary-ui.md) · [form-close-and-dispose](form-close-and-dispose.md) · [form-close-cancel-pattern](form-close-cancel-pattern.md) · [form-constructor-vs-load](form-constructor-vs-load.md) · [form-load-and-shown-events](form-load-and-shown-events.md) · [layout-table-panel](layout-table-panel.md) · [partial-class-and-file-naming](partial-class-and-file-naming.md) · [splitcontainer-and-sizing](splitcontainer-and-sizing.md) · [validation-form-level](validation-form-level.md) · [winforms-rendering-performance](winforms-rendering-performance.md)

> **Tek cumlelik sorumluluk:** *Uygulama nereye, hangi bicimde ve hangi seviyede log yazar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Log hatti **kutudan cikan** `System.Diagnostics.Trace` + `TextWriterTraceListener` uzerine kurulur: `net48` ve `net10.0-windows` bacaklarinin ikisinde de ek paket gerekmez, bu bilincli bir kisaltmadir (log altyapisi icin NuGet istemek surum onayi gerektirir - bkz. [logging-rules](../../.rules/logging-rules.md)). Dosya `%LocalAppData%\VersaCoder\logs\versacoder-yyyyMMdd.log` altinda gunluk uretilir, yazim `Trace` icinde kilitlidir, cikti UTF-8'dir. **Sirlar (anahtar, parola, token) asla loglanmaz**; log "bitti" kaniti degildir, kanit build/test ciktisidir.

```text
+----------------------------------------------------------+
| Program.cs -> Trace.Listeners.Add(TextWriterTraceListener) |
|      |                                                   |
|      +--> Trace.WriteLine("INFO ...")   (kilitli yazim)   |
|      +--> gunluk dosya: versacoder-yyyyMMdd.log           |
|      +--> acilista eski gunler tek dongude silinir        |
+----------------------------------------------------------+
```

---

## §1 Log seviyeleri

| Seviye | Ne gecer | Ornek |
|---|---|---|
| `TRACE` | dongu ici ayrinti (varsayilan kapali) | "grid 42 satir yuklendi" |
| `INFO` | is akisi adimi | "oturum acildi" |
| `WARN` | geri alinabilir anormallik | "ayar dosyasi bozuk, varsayilana donuldu" |
| `ERROR` | hata + stacak izi | "kayit hatasi: saglayici hatasi" |
| `FATAL` | uygulama devam edemez | "baslatma hatasi, kapaniyor" |

Log mesaji **kullanici metni degil, olay** tarif eder; uretilmis metnin tamami girintisiz loga yazilmaz.

---

## §2 Kutudan cikan hatt

```csharp
var logDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "VersaCoder", "logs");
Directory.CreateDirectory(logDir);

var listener = new TextWriterTraceListener(
    Path.Combine(logDir, "versacoder-" + DateTime.Now.ToString("yyyyMMdd") + ".log"));
Trace.Listeners.Add(listener);
Trace.AutoFlush = true;
Trace.WriteLine("INFO  baslatma tamam");
```

| Dosya ayari | Deger | Neden |
|---|---|---|
| Konum | `%LocalAppData%\VersaCoder\logs` | kullanici yazma izni; makine degil, kullanici bazli |
| Kodlama | UTF-8 (BOM'suz) | iki bacak ayni bayti okur |
| Otomatik flush | `Trace.AutoFlush = true` | crash oncesi satir kaybolmasin |
| Rotasyon | gunluk dosya adi + adet limiti | disk dolmasin |
| Eski gun temizligi | acilista tek dongu | dosya karmasasi olmasin |

Coklu thread'de ayni dosyaya yazim `Trace` icinde kilitlidir; ek bir kilit nesnesi gerekmez.

---

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | `net48` | `net10.0-windows` |
|---|---|---|
| `Trace` / `TextWriterTraceListener` | `System.Diagnostics` - kutuda | ayni - kutuda |
| Varsayilan listener | `DefaultTraceListener` | ayni |
| Dosya yolu API | `Environment.GetFolderPath` | ayni |
| C# dili | **7.3** (kutuda `using var` yok) | C# 14 |
| MEL / Serilog gibi paket | NuGet onayi gerekir | NuGet onayi gerekir |

---

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Parola / anahtar / token loglama | dosya siza | maskele ya da loglama |
| 2 | Logu tek global statikte tutmak | test edilemez, kilitlenmis dosya | ince arayuz, enjekte edilir |
| 3 | Log yazim hatasini sessiz yutmak | sessiz log kaybi | `Debug.WriteLine` geri dususu |
| 4 | "log var" diye gecmis gecirmek | kanitsiz iddia | kanit = build/test ciktisi |
| 5 | Ham SQL'i loglamak (parametreli olsa bile) | veri sizi | yalniz sorgu adi + sure |

---

## §5 Kontrol listesi

| # | Adim | Kanit |
|---|---|---|
| 1 | Listener `Program.cs`'te bir kez ekleniyor | grep `Listeners.Add` = 1 |
| 2 | Log dizini kullanici altinda | kod incelemesi (yol) |
| 3 | Sirlar loga girmiyor | grep token/parola kalibi |
| 4 | Paket eklenmedi (kutudan cikan hatt) | proje dosyasinda paket yok |
| 5 | Iki bacakta `dotnet build` -> 0 hata | build ciktisi |

---

## §9 Kanit

| Kaynak dosya | Ne alindi |
|---|---|
| `knowledge\winforms\logging-integration.md` (kaynak vault) | seviye tablosu, `TextWriterTraceListener` hatti, dosya ayarlari, 5 yasak |
| `.rules\logging-rules.md` | loglama kurali; paket ekleme onay kosulu |
| `.rules\security-rules.md` | sirlari loglamama yasagi |
| `.ai\CLAUDE.md` | kanitsiz "bitti" yasagi |

> Kanitsiz iddia icin `DOĞRULAMA GEREKLI` yazilir; bu dosyada kaynak disi bilgi yoktur.
