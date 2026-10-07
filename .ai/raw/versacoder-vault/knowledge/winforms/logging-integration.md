---
title: "logging-integration - VersaCoder"
type: knowledge
category: winforms
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "WinForms uygulamasinda log yazimi: kutudan cikan Trace/TextWriterTraceListener hatti, dosya boyutu/rotasyon, sirlari loglamama ve net48 + net10.0-windows farklari"
---

# logging-integration — WinForms icin log hatti (paket gerekmez)

**Zorunlu Baglantilar:** [index](../index.md) · [coding-standards](../../.rules/coding-standards.md) · [security-rules](../../.rules/security-rules.md) · [exception-boundary-ui](exception-boundary-ui.md) · [startup-pipeline-ui](startup-pipeline-ui.md)

> **Tek cumlelik sorumluluk:** *Uygulama nereye, hangi bicimde ve hangi seviyede log yazar* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Log hatti **kutudan cikan** `System.Diagnostics.Trace` + `TextWriterTraceListener` uzerine kurulur: iki bacakta da ek paket gerekmez (G8'e takilmazsin). Dosya `%LocalAppData%\VersaCoder\logs\versacoder-yyyyMMdd.log`, yazim **kilitli** (coklu thread), cikti **UTF-8**. Seviye sabittir: `TRACE` ayrinti, `WARN` geri alinabilir anormallik, `ERROR` hata (stacak izi ile). **Sirlar (anahtar, parola, token) asla loglanmaz.** Log, "bitti" kaniti degildir; kanit `dotnet build` / `dotnet test` ciktisidir (G5).

---

## §1 Log seviyeleri

| Seviye | Ne gecer | Ornek |
|---|---|---|
| `TRACE` | döngü ici ayrinti (varsayilan kapali) | "grid 42 satir yuklendi" |
| `INFO` | is akisi adimi | "oturum acildi, kullanici=..." |
| `WARN` | geri alinabilir anormallik | "ayar dosyasi bozuk, varsayilana donuldu" |
| `ERROR` | hata + stacak izi | "kayit hatasi: SqlException(...)" |
| `FATAL` | uygulama devam edemez | "baslatma hatasi, kapaniyor" |

> **KURAL:** Log mesaji **kullanici metni degil, olay** tarif eder; uretilmis metnin tamami girintisiz yazilmaz.

## §2 Kutudan cikan hatt (kod)

```csharp
var logDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "VersaCoder", "logs");
Directory.CreateDirectory(logDir);

var listener = new TextWriterTraceListener(
    Path.Combine(logDir, "versacoder-" + DateTime.Now.ToString("yyyyMMdd") + ".log"));
Trace.Listeners.Add(listener);
Trace.AutoFlush = true;
Trace.WriteLine("INFO  baslatma tamam, surum=" + assemblyVersion);
```

Coklu thread'de ayni dosyaya yazim `Trace` icinde kilitlidir; ek kilit gerekmez. Gunluk **rotasyonu** gunluk dosya adi ile yapilir; eski gunler silme islemi uygulama acilisinda tek dongude yapilir.

| Dosya ayari | Deger | Neden |
|---|---|---|
| Konum | `%LocalAppData%\VersaCoder\logs` | kullanici yazma izni, makine degil |
| Kodlama | UTF-8 (BOM'suz) | iki bacak ayni bayti okur |
| Otomatik flush | `true` | crash oncesi satir kaybolmasin |
| Boyut siniri | gunluk dosya + adet limiti | disk dolmasin |

## §3 Iki bacak: net48 vs net10.0-windows

| Konu | net48 | net10.0-windows |
|---|---|---|
| `Trace` / `TextWriterTraceListener` | `System.Diagnostics` - kutuda | ayni - kutuda |
| Varsayilan listener | `DefaultTraceListener` (Debugger'a) | ayni |
| Ek paket (MEL / Serilog) | NuGet = **G8** onayi gerekir | NuGet = **G8** onayi gerekir |
| Dosya yolu API | `Environment.GetFolderPath` - ayni | ayni |
| C# dili | **7.3** | C# 14 |

Log **altyapisi** icin paket istemek karar degistirir (G8) - onay yoksa `Trace` hatti kalir ve bu bilincli kisaltmadir.

## §4 Yapilmayanlar

| # | Yasak | Sonuc | Duzeltme |
|---|---|---|---|
| 1 | Parola / anahtar / token loglama | dosya siza | maskele (`***`) ya da loglama |
| 2 | Logu tek global statikte tutmak | test edilemez, kilitlenmis dosya | `ILogger` benzeri ince arayuz, enjekte edilir |
| 3 | Log hatasini sessiz yutmak | sessiz log kaybi | log yazim hatasi -> `Debug.WriteLine` geri dususu |
| 4 | "log var" diye gecmis gecirmek | G5 ihlali | kanit = build/test ciktisi |
| 5 | Ham SQL'i loglamak (parametreli olsa bile) | veri sizi | sadece sorgu adi + sure |

## §5 Kardes Dosyalar

| Konu | Dosya |
|---|---|
| Hata siniri (log'un ana musteri) | [exception-boundary-ui](exception-boundary-ui.md) |
| Dosya yazim kalibi (atomik) | [file-dialogs-io](file-dialogs-io.md) |
| Ayar dosyasi konumu | [settings-user-config](settings-user-config.md) |
| Acilista listener kurulumu | [startup-pipeline-ui](startup-pipeline-ui.md) |
