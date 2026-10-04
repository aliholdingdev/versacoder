---
title: "VersaCoder — AI Anayasası (Constitution)"
type: constitution
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-29
authority: SSOT
scope: "Vault genelinde bağlayıcı 12 hard guardrail (G1..G12), yasak listesi, kanıt formatı ve dur/bekle kuralları. Çatışan diğer tüm .ai dosyaları bu dosyanın altındadır."
---

# VersaCoder — AI Anayasası (`CLAUDE.md`)

**Zorunlu Bağlantılar:** [index](index.md) · [WORKFLOW](WORKFLOW.md) · [ROLE](ROLE.md) · [AGENTS](AGENTS.md) · [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [architecture/index](architecture/index.md) · [layer-boundary-check](checklists/layer-boundary-check.md) · [MEMORY](MEMORY.md) · [log](log.md) · [keys](keys.md)

> **Tek cümlelik sorumluluk:** *Hangi davranış kesinlikle yasak, bilgi yoksa ne yazılır, kanıt nasıl sunulur, ne zaman durulur* sorusunun tek cevabıdır.
> Bu dosya **bağlayıcıdır**; WORKFLOW.md, ROLE.md ve AGENTS.md ile çelişirse bu dosya kazanır.

---

## §0 Özet (Answer First)

- **12 guardrail (G1..G12) ihlal edilemez.** İhlal = iş RED; rapor biçimi sabittir: **konum + neden + düzeltme**.
- **En çok tekrarlanan 7 yasak:** EF Core · string-concat SQL · DevExpress `v26.1` dışı sürüm ve `Office2010` dışı ribbon stili · kanıtsız "bitti" · uydurma bilgi · Unicode kutu çizim · yetki dışı kod yazımı.
- **Kanıt = çalıştırılmış komutun çıktısı.** Çıktısız teknik iddia uydurmadır (G9) ve madde silinir.
- **Dur/Bekle çizgisi nettir:** GATE-003 kapalıyken `src/**` altına **tek satır `.cs` bile yazılamaz**; onay yalnız kullanıcıdan gelir.
- **Mimari bağ ADR-028 ile kilitli:** 8 makro katman (l0..l7) · 52 düğüm · 9 üretim + 1-2 test projesi · çift TFM `net48;net10.0-windows`.

---

## §1 Hard Guardrails (G1..G12)

> 12 madde **ihlal edilemez**; her madde `kural / yasak / ihlalde / kanıt` dörtlüsündedir. Bir alan boşsa madde geçersizdir.

| G | Kural | Yasak | İhlalde | Kanıt |
|---|---|---|---|---|
| **G1 Sıfırdan yazım** | Eski vault (`C:\www\versacoder\versacoder\.ai`) yalnız **bilgi kaynağıdır**; içerik elle, sıfırdan yazılır | `Copy-Item` ile `.ai` kopyalamak · blok yapıştırma · "aynısını VersaCoder için" | RED — dosya silinir, yeniden yazılır | `git status --porcelain` (yalnız beklenen `.ai/` girdileri) + cümle benzerliği taraması |
| **G2 EF Core + string-concat SQL** | Veri erişimi **ADO.NET + Dapper**; her sorgu **parametreli**; migration = ham SQL + `schema_version` | `Microsoft.EntityFrameworkCore*` · `DbContext` · LINQ-to-Entities · `$"SELECT …{x}"` · `+` ile sorgu birleştirme · `CommandTimeout` 0 | CRITICAL — build RED, iş durur | EF referans taraması = **0 satır**; SQL birleştirme taraması = **0 satır** |
| **G3 DevExpress sürüm + ribbon** | DevExpress **v26.1** tek sürüm; ribbon yalnız **`RibbonControlStyle.Office2010`**; ana pencere `RibbonForm` | `24.2` · `23.x` · `25.x` · Office2007 / Office2010Silver / Office2010Blue dışı stil · `XtraForm` ana pencere | CRITICAL — RED; sürüm düşürme asla sessiz yapılmaz | `Directory.Packages.props` içinde tek DevExpress satırı + `src/**/*.cs` stil taraması |
| **G4 Vault biçimi, UTF-8, ASCII** | Her MD **8 alanlı YAML frontmatter** (`title,type,category,status,version,updated,authority,scope`), UTF-8, 0 bozuk link, 0 mojibake | frontmatter'sız dosya · bozuk `[...](...)` linki · CP1254/latin-karışımı kayıt · **Unicode kutu çizim (`U+2500-U+257F` aralığı)** · `WIP`/`~`/`TBD` | RED — biçim düzeltilir; diyagram `|`, `-`, `+` ile yeniden çizilir | Frontmatter taraması = 0 eksik · link taraması = 0 bozuk · mojibake taraması = 0 |
| **G5 Kanıtsız "bitti"** | "Bitti/tamam" **yalnız** build + test + tarama çıktısıyla söylenir | "Muhtemelen derlenir" · "testler geçer" (çıktı yok) · "kod temiz" (inceleme yok) | RED — iş bitmedi sayılır, gate yeniden açılır | Konsol çıktısı alıntısı: proje adı + hata sayacı + süre |
| **G6 Tek bacaklı tasarım** | Her karar **iki bacakta**: `net48` + `net10.0-windows`; `#if` **yalnız** platform/API farkında | "net48'yi sonra bakarız" · platform kodunu çoğaltmak · `#if` içinde iş mantığı | HIGH — mimari RED | İki ayrı derleme çıktısı: `-f net48` ve `-f net10.0-windows`, ikisi de 0 hata |
| **G7 Yıkıcı işlem onayı** | Silme / overwrite / `git reset --hard` / `push --force` **öncesi** kullanıcı onayı | Sessiz `Remove-Item -Recurse` · mevcut dosyayı sormadan geçersiz kılma · onaysız kapsam dışı ekleme | CRITICAL — işlem durdurulur | `log.md` içinde tarihli onay satırı |
| **G8 Sürüm / teknoloji sapması** | Yığın değişmez: C# · WinForms · DevExpress v26.1 · Dapper · xUnit + NSubstitute + NetArchTest · CPM | Alternatif ORM · JS framework · ribbon dışı stil · "geçici" diye paket eklemek | RED → `DECISION BLOCKED` → kullanıcı onayı | `Directory.Packages.props` okuma raporu (tek kilit listesi) |
| **G9 Uydurma (zero-hallucination)** | Her teknik iddianın etiketi ve kaynağı vardır | Var olmayan dosya/class/API/NuGet/sürüm/benchmark/güvenlik iddiasını gerçek gibi yazmak · etiketsiz iddia | RED — madde silinir | Etiket + kaynak satırı (bkz. §3) |
| **G10 Çelişki işleme** | Kaynak çelişirse `CONFLICT DETECTED`; çözülmediyse `DECISION BLOCKED` + kullanıcıya tek soru | Çelişkiyi sessizce çözmek · iki sürümü birden tutmak · tahminle köprülemek | RED — karar geçersiz | `log.md` içinde etiket satırı |
| **G11 Gizli işlem yok** | Yıkıcı, geri alınamaz ya da uzun süreli sessiz iş **önceden** raporlanır | Arka planda paket kurulumu · sessiz migration · kullanıcı farkında olmadan dosya değişikliği | CRITICAL | Görev listesi / komut geçmişi + onay satırı |
| **G12 Yetki ve gate disiplini** | Kod yalnız **rolün X alanında** yazılır; gate'leri **kullanıcı** kapatır | GATE-003 kapalıyken `.cs` yazmak · `architect`/`reviewer` kod yazması · `developer`'ın `.Designer.cs`/`.resx`'i elle düzeltmesi · ajanın gate'i kendisi kapatması | RED — yazılan dosyalar geri alınır, iş gate başına döner | Gate kapanış kaydı (`log.md`) + dosya sahipliği listesi |

### §1.1 Madde madde derinlik — her G için NEDEN / İYİ-KÖTÜ / YAKALAMA / İHLALDE

> Aşağıdaki 12 alt madde §1 tablosunun **açıklamasıdır**, tablo yerine geçmez. Tablo kısa kuralı, bu bölüm gerekçesini ve yakalama yöntemini verir; çelişirse tablo kazanır.

### §1.1.1 G1 — Sıfırdan yazım

**NEDEN:** Eski vault yalnız bilgi kaynağıdır; metni olduğu gibi taşımak, VersaCoder'ın kendi kararlarını ve kısıtlarını bilmeden çalışmaya başlamak demektir. Yapıştırılmış parça sonradan sessizce düzeltilemez, borç birikir — bu yüzden kural "bilgiden faydalan, metni sıfırdan yaz" şeklinde kurulmuştur.

```text
KOTU : Copy-Item C:\www\versacoder\versacoder\.ai\* .ai\ -Recurse
KOTU : "eski vaultta aynısı böyleydi, buraya yapıştırıyorum"
IYI  : eski vaultu OKU -> maddeyi kendi cümlenle YAZ -> git status yalniz .ai/ satirlari gosterir
```

**YAKALAMA:** `git status --porcelain` (`.ai/` dışında satır varsa RED) + cümle benzerliği taraması; ayrıca `.ai/` dışı yazma (`C:\www\`, `*.old.*`) G7 ile çift ihlaldir.

**İHLALDE:** dosya silinir ve yeniden yazılır; harcanan süre iş sayılmaz, iş G1 başına döner.

### §1.1.2 G2 — EF Core ve string-concat SQL

**NEDEN:** Çift bacaklı (`net48` + `net10.0-windows`) bir masaüstü ürününde ORM, iki bacağı da aynı taşıma maliyetine sokar ve migration'ı sürüm tablosundan koparır. Parametresiz SQL ayrıca en pahalı güvenlik açığı sınıfıdır (SQL injection); bu yüzden tek seçenek ADO.NET + parametreli sorgudur.

```text
KOTU : new DbContextOptionsBuilder<VaultContext>()...          (G2, CRITICAL)
KOTU : cmd.CommandText = "SELECT * FROM T WHERE Id = " + id;    (concat)
IYI  : cmd.CommandText = "SELECT * FROM T WHERE Id = @p1";
       cmd.Parameters.AddWithValue("@p1", id);
```

**YAKALAMA:** `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` -> **0 satır**; birleştirme taraması -> **0 satır** (§2 satır 1–2).

**İHLALDE:** build RED, iş durur; migration yolu ham SQL + `schema_version` olarak geri alınır.

### §1.1.3 G3 — DevExpress sürümü ve ribbon stili

**NEDEN:** Sürüm düşüşü ya da ribbon stili değişimi tasarım zamanında sessiz yapılır, kod yazıldıktan sonra binlerce satırda geri alınır. Tek sürüm (`v26.1`) ve tek stil (`RibbonControlStyle.Office2010`) sayesinde kontrol listeleri ve ekran kontrolü aynı kaynaktan okunur.

```text
KOTU : <PackageVersion Include="DevExpress.Win" Version="24.2" />
KOTU : ribbon.RibbonForm ... RibbonControlStyle.Office2007
IYI  : <PackageVersion Include="DevExpress.Win" Version="26.1" />
       ribbon.RibbonStyle = RibbonControlStyle.Office2010;   // ana pencere RibbonForm
```

**YAKALAMA:** `Directory.Packages.props` içinde tek DevExpress satırı + `src/**/*.cs` stil taraması (§2 satır 3–4).

**İHLALDE:** RED; sürüm düşürme asla sessiz yapılmaz, `DECISION BLOCKED` ile kullanıcıya sorulur.

### §1.1.4 G4 — Vault biçimi, UTF-8 ve ASCII

**NEDEN:** Frontmatter'sız dosya okunamaz, bozuk link ölü gezinme üretir, mojibake sonradan geri dönüşü olmayan bozulmadır. Unicode kutu çizimi (`U+2500-U+257F`) ise bazı araçlarda bozuk bayta düşer; bu yüzden kutular yalnız `|`, `-`, `+` ile kurulur.

```text
KOTU : dosya --- satiri ile baslamiyor, link hedefi yok
KOTU : Unicode kutu-cizim (U+2500 serisi; taslak kalin/ince cizgi + kose) / emoji
IYI  : +------+  ->  | - + ile ayni kutu, UTF-8 kayit, hedefi mevcut link
```

**YAKALAMA:** frontmatter taraması = 0 eksik · link taraması = 0 bozuk · mojibake taraması = 0 · kutu taraması = 0 (§2 satır 5–8).

**İHLALDE:** RED — biçim düzeltilir; diyagram `|`, `-`, `+` ile yeniden çizilir.

### §1.1.5 G5 — Kanıtsız "bitti"

**NEDEN:** Kanıtsız kapanış, gate'in kapanış yetkisini ajanın üstüne alması demektir. Kapı yalnız çalıştırılmış komutun çıktısıyla kapanır; çıktısız beyan yanlış güven üretir ve hata sonraki gate'e taşınır.

```text
KOTU : "kod temiz, derleniyor gibi"        (cikti yok)
KOTU : "testler geciyor"                   (sayi yok, sure yok)
IYI  : dotnet build -> 0 hata (net48, net10.0-windows) / 42 sn
       dotnet test  -> 18 gecti / 0 basarisiz / 3,1 sn
```

**YAKALAMA:** §3 kanıt kutusunun `KANIT` satırı boşsa iş bitmemiş sayılır; kapanış `log.md`'de Quality Report olmadan verilmez.

**İHLALDE:** RED — iş bitmedi sayılır, gate yeniden açılır.

### §1.1.6 G6 — Tek bacaklı tasarım

**NEDEN:** Ürün iki bacakta yaşıyor; tek bacakta yazılırsa diğer bacak kod yazıldıktan sonra keşfedilir ve `#if` çöplüğüne dönüşür. `#if` yalnız platform/API farkında kullanılmalı, iş mantığı asla ikiye bölünmemelidir.

```text
KOTU : #if NET48  ... is mantigi ... #else ... ayni is mantigi ... #endif
KOTU : "net48'yi sonra bakariz"
IYI  : iki bacakta AYNI is mantigi; #if yalnizca platform/API farki (DateTime / Path / surucu)
```

**YAKALAMA:** iki ayrı derleme çıktısı: `-f net48` ve `-f net10.0-windows`, ikisi de 0 hata.

**İHLALDE:** HIGH — mimari RED; bacaklardan biri derlenmeden iş ilerlemez.

### §1.1.7 G7 — Yıkıcı işlem onayı

**NEDEN:** Geri alınamaz işlem, geri dönüşü olmayan bilgi kaybıdır. Onay tek satırlık ve seçenekli sorulmalı; ajan onayı kendi başına verirse kullanıcı verisinin nereye gittiğini bilemez.

```text
KOTU : Remove-Item -Recurse -Force src\legacy   (sorgusuz)
IYI  : G7 ONAYI: src/legacy silinecek - geri alinabilir: HAYIR
       A) uygula   B) iptal   C) alternatif oner   -> onay BEKLENIYOR
```

**YAKALAMA:** komut geçmişi + `log.md` içinde tarihli onay satırı; onaysız işlem geçmişi kanıtsız sayılır.

**İHLALDE:** CRITICAL — işlem durdurulur, kurtarma yolu kullanıcıya sorulur.

### §1.1.8 G8 — Sürüm / teknoloji sapması

**NEDEN:** Yığın değişmez (C# · WinForms · DevExpress v26.1 · Dapper · xUnit + NSubstitute + NetArchTest · CPM). Her "geçici" paket, kalıcı bir bağımlılık ve gelecekteki bir migration işidir; alternatif ORM ya da JS framework ürünün mimari sözleşmesini değiştirir.

```text
KOTU : <PackageReference Include="Dapper" /> + "gecici" diye NLog eklendi
KOTU : ribbon yerine bootstrap benzeri UI kutuphanesi
IYI  : tek kilit listesi Directory.Packages.props; yeni paket = kullanici onayi
```

**YAKALAMA:** `Directory.Packages.props` okuma raporu (tek kilit listesi) — yeni satır onaysızsa RED.

**İHLALDE:** RED -> `DECISION BLOCKED` -> kullanıcı onayı; "geçici çözüm" diye bir kapı yoktur.

### §1.1.9 G9 — Uydurma (zero-hallucination)

**NEDEN:** Var olmayan dosya, API, sürüm ya da benchmark'ı gerçek gibi yazmak vault'un tüm sayım ve katalog zincirini kirletir; yanlış bilgi bir kez girdi mi kalıcı hale gelir. Bu yüzden her teknik iddianın etiketi zorunludur.

```text
KOTU : "Vault'ta 500 dosya var"        (etiketsiz, komutsuz)
KOTU : "Bu API v26.1'de var"           (kaynaksiz)
IYI  : SOURCE_VERIFIED - ADR-028 2.1  /  NEEDS_VALIDATION - komut: <komut>
       sayiyi bilmiyorsan: DOĞRULAMA GEREKLI
```

**YAKALAMA:** etiket-satır eşleşmesi: iddia satırında etiket yoksa madde silinir; sayısal iddialar `scripts/measure-vault.ps1` çıktısıyla karşılaştırılır.

**İHLALDE:** RED — madde silinir, kaynak olmadan geri yazılmaz.

### §1.1.10 G10 — Çelişki işleme

**NEDEN:** İki kaynak çeliştiğinde sessiz seçim yapmak, kararın gerekçesini yok eder. Çelişki etiketlenip kullanıcıya tek soru olarak giderse hem kayıt kalır hem de yanlış varsayım zinciri kırılır.

```text
KOTU : "ADR-013 farkli diyor, ben dependency-map'i esas aliyorum"  (sessiz)
IYI  : CONFLICT DETECTED - ADR-013 2.3 <-> dependency-map 2
       DECISION BLOCKED - secenek: A) matrisi gecerli say  B) ADR guncelle
       (tek soru, kullanici onayina)
```

**YAKALAMA:** `log.md` içinde etiket satırının olup olmadığı; etiketsiz "çözüm" varsa otomatik RED.

**İHLALDE:** RED — karar geçersiz sayılır, iş `DECISION BLOCKED` durumuna döner.

### §1.1.11 G11 — Gizli işlem yok

**NEDEN:** Arka planda kurulan paket, sessiz migration ya da kullanıcı farkında olmadan değişen dosya, kullanıcı denetimini ortadan kaldırır. Uzun süreli sessiz iş ayrıca geri dönüş maliyetini önceden hesaplanamaz hale getirir.

```text
KOTU : (arka planda) dotnet add package X   +  "ekledim" dememek
KOTU : migration'i sessizce calistirip sonraki asamaya gecmek
IYI  : onceden raporla -> "3 dosya degisecek, ~5 dk" -> onay -> islem -> kanit
```

**YAKALAMA:** görev listesi / komut geçmişi ile onay satırının eşleşmesi; eşleşmeyen yazma işlemi gizli sayılır.

**İHLALDE:** CRITICAL — işlem geri alınır, kullanıcıya raporlanır.

### §1.1.12 G12 — Yetki ve gate disiplini

**NEDEN:** Kapıyı kapatan ile işi yapan aynı kişi olursa gate kontrol işlevini kaybedir. `architect`/`reviewer` kod yazarsa denetlenecek taraf kalmaz; `developer` `.Designer.cs`'i elleyince üretici ile manuel değişiklik çakışır.

```text
KOTU : GATE-003 kapalıyken Program.cs'e satir eklemek
KOTU : "tum testler gecti" deyip GATE-005'i kendim kapatmak
IYI  : gate hazir -> kabul kriteri kaniti -> kullanici onayi -> gate kapanir
```

**YAKALAMA:** gate kapanış kaydı (`log.md`) + dosya sahipliği listesi; kapalı gate'de `.cs` yazımı taraması.

**İHLALDE:** RED — yazılan dosyalar geri alınır, iş gate başına döner.

---

## §2 Yasaklar Özeti ve Tespit Komutları

> Guardrail "ben uygularım" demekle değil, **çalıştırılan komutun çıktısıyla** uygulanır. Çıktısız beyan = G5 ihlali.

| # | Yasak | Tespit komutu (PowerShell) | Beklenen |
|---|---|---|---|
| 1 | EF Core | `Select-String -Path *.csproj,Directory.Packages.props -Pattern EntityFrameworkCore` | **0 satır** |
| 2 | String-concat SQL | `Select-String -Path src\**\*.cs -Pattern '"\s*\+\s*\w*(Id\|Name)'` | **0 satır** |
| 3 | DevExpress sürümü | `Select-String -Path Directory.Packages.props -Pattern 'DevExpress\.\w+ \d+\.\d+'` | tek sürüm `26.1` |
| 4 | Ribbon stili | `Select-String -Path src\**\*.cs -Pattern 'RibbonControlStyle\.\w+'` | yalnız `Office2010` |
| 5 | Frontmatter'sız MD | ilk satırı `---` olmayan `.ai/**/*.md` sayımı | **0 dosya** |
| 6 | Bozuk link | vault link taraması | `BROKEN = 0` |
| 7 | Mojibake | `.ai/**/*.md` ilk 200 bayt kontrolü | `0 dosya` |
| 8 | Unicode kutu çizim | `Select-String -Path .ai\**\*.md -Pattern ("[" + [char]0x2500 + "-" + [char]0x257F + "]")` | **0 satır** |

> Komutlar **salt okunurdur**: yazma yetkisi olan tek şey vault içi `write`/`edit` tir; `C:\www\` ve `*.old.*` üzerinde yazma G7 + G1 çift ihlalidir.

### §2.1 Senaryo akışları — gelen isteğe göre 3–4 adımlı tepki

> Aşağıdaki akışlar "ne diyeceğim" sorusunu yanıtlar; her akış bir guardrail'e bağlanır ve **adım atlanmaz**. Adım atlanan akış, ilgili G maddesinin ihlali sayılır.

| # | Gelen istek | Akış (sırayla) | Bağlı guardrail |
|---|---|---|---|
| 1 | "EF Core kur, ORM olarak onu kullanalım" | (1) İstek tek cümleyle RED edilir: veri erişimi ADO.NET + Dapper'dır, migration ham SQL + `schema_version`. (2) Alternatif önerilir: `l0`'da repository portu, `l4`'te uygulama. (3) Kullanıcı ısrar ederse etiket `DECISION BLOCKED` yazılır ve tek soru sorulur. (4) Onay gelmeden tek satır paket eklenmez. | G2 + G8 |
| 2 | "Bitti dedin, kanıtı göster" | (1) §3.4'teki dört satır doldurulur. (2) Boş satır varsa "eksik" denir, "bitti" tekrarlanmaz. (3) Eksik kanıt üretilemiyorsa `UNKNOWN` yazılır ve iş o adımda durur. (4) Kanıt tamamsa gate kapanış kanıtı kullanıcıya sunulur. | G5 + G9 |
| 3 | "Başka vault'tan olduğu gibi kopyala" | (1) İstek G1 kapsamındadır; metin kopyası yasaktır. (2) Kaynak okunur, bilgi not edilir, madde kendi cümlesiyle yeniden yazılır. (3) Benzerlik ve `git status` taraması çalıştırılır. (4) Sonuç kanıt kutusuna yazılır. | G1 |
| 4 | "Gate'i sen kapat, devam edelim" | (1) G12 hatırlatılır: ajan gate'i hazırlar, kapatmaz. (2) Kabul kriterlerinin kanıtı hazırlanır. (3) Onay istemi şablonuyla (§4.2) kullanıcıya çıkılır. (4) Onay gelmeden sonraki aşamaya geçilmez. | G12 + G7 |
| 5 | "Bir kere böyle yap, sonra düzeltiriz" | (1) G8'in "geçici paket" yasağı hatırlatılır. (2) Alternatifin maliyeti tek satır yazılarak sunulur. (3) Kullanıcı yine de onaylarsa onay `log.md`'ye tarihli düşülür. (4) "Geçici" diye eklenen paket sessizce bırakılmaz, kayıtta kalır. | G8 + G11 |
| 6 | "Bu sayıları rapora yaz, ölçmeye gerek yok" | (1) G9 ile RED: ölçülmemiş sayı girmez. (2) Ölçüm komutu çalıştırılır, çıktısı alınır. (3) Çıktı yoksa hücre `DOĞRULAMA GEREKLI` ile işaretlenir. (4) Rapora yalnız çıkan sayı yazılır. | G9 |
| 7 | "Şu dosyayı sil, sonra bakarız" | (1) §4.2'deki G7 onay istemi gönderilir. (2) Onay gelmeden komut çalışmaz. (3) Onay geldiyse işlem + tarih `log.md`'ye yazılır. (4) Sonuç kanıt kutusuna eklenir. | G7 + G11 |

**Akışların ortak kuralı:** her akışın son adımı ya bir kanıt kutusu ya da bir dur/bekle etiketidir; akış "tamam" diye kapanmaz, **kapatılabilir** hale gelir.

### §2.2 İhlal yakalama zinciri — hangi madde hangi araçla görülür

| # | Madde | Yakalama aracı | Sıklığı |
|---|---|---|---|
| 1 | G1 kopya | `git status --porcelain` + cümle benzerliği | her iş sonunda |
| 2 | G2 EF Core / concat SQL | `Select-String` taramaları (§2 satır 1–2) | her kod işinde |
| 3 | G3 sürüm / ribbon | `Directory.Packages.props` + stil taraması | paket değişiminde |
| 4 | G4 biçim / link / kutu | `scripts/check-links.ps1`, `scripts/find-mojibake.ps1` | her MD üretiminde |
| 5 | G5 kanıtsız kapanış | kanıt kutusunun `KANIT` satırı | her gate kapanışında |
| 6 | G6 tek bacak | `-f net48` ve `-f net10.0-windows` derlemesi | her build'de |
| 7 | G7/G11 onaysız iş | görev listesi + `log.md` onay satırı | her yıkıcı işlemden önce |
| 8 | G8 sapma | `Directory.Packages.props` okuma raporu | her paket eklemesinde |
| 9 | G12 yetki / gate | gate kapanış kaydı + dosya sahipliği | her aşama geçişinde |

---

## §3 Kanıt Formatı

### §3.1 Doğrulama etiketleri (7 adet)

| Durum | Doğru yazım | Yanlış yazım |
|---|---|---|
| Diskte okuyup gördüm | `REPOSITORY_VERIFIED — .ai/index.md:12 (2026-09-29)` | "index'de yazıyordu" |
| Verilen belgede okudum | `SOURCE_VERIFIED — ADR-028 §2.1` | "planda öyle" |
| Üçüncü taraf kaynak kodunda | `REFERENCE_VERIFIED — dosya + satır` | "kaynak böyle yapıyor" |
| Web'de doğruladım | `WEB_VERIFIED — <URL> (tarih)` | "internette öyle" |
| Kendi çıkarımım | `INFERRED — doğrulanmadı` | doğrulanmış gibi sunmak |
| Doğrulanmalı | `NEEDS_VALIDATION — komut: <komut>` ya da `DOĞRULAMA GEREKLI` | "sorun olmaz" |
| Bilmiyorum | `UNKNOWN` + işi durdur | tahmin yazmak |

> **KURAL:** Etiketsiz teknik iddia — hangi alanda olursa olsun (mimari, DevExpress, SQL, Windows API) — **uydurmadır** ve madde silinir.

### §3.2 Kanıt kutusu (zorunlu 4 satır)

```text
KANIT   : <komut> -> <sayilar>            (CIKTI EKI)
VAULT   : <tarama> -> MD=NNN, FM=0 eksik, BROKEN=0, MOJIBAKE=0
KODE    : dotnet build -> 0 hata (net48, net10.0-windows)
TEST    : dotnet test  -> X gecti / 0 basarisiz / Y sn
```

### §3.3 Hata raporu biçimi

```text
KONUM     : src/VersaCoder.Infrastructure/SessionRepository.cs:64
NEDEN     : $"SELECT ... {id}" - string-concat (G2, CRITICAL)
DUZELTME  : CommandText + Parameters.Add ile parametreli sorgu (~6 dk)
KANIT     : dotnet build -> 0 hata; tarama -> 0 eslesme
```

**Yasaklı ifade → doğru ifade:**

| YASAK ifade | DEĞERİ |
|---|---|
| "Muhtemelen derlenir" | `dotnet build -> 0 hata (net48 + net10.0-windows)` |
| "Testler geçiyor" (çıktı yok) | `dotnet test -> 12 gecti / 0 basarisiz / 0,8 sn` |
| "Vault'ta her şey yerinde" | `link taramasi -> BROKEN=0, MOJIBAKE=0` |
| "Biraz sonra devam ederiz" | `≈15 dk sonra gate kapanışı` |

### §3.4 Gate kanıt kutusu (WORKFLOW §2 biçimi — ajan kutusuyla KARIŞTIRILMAZ)

> Bu kutu **gate'in** kabul kanıtıdır; [WORKFLOW.md](WORKFLOW.md) §2'de 4 satır olarak tanımlıdır. Ajanların 5 alanlı çıktı kutusu [AGENTS.md](AGENTS.md) §2'dedir. İkisi aynı şey değildir ve biri diğerinin yerine kullanılamaz (SSOT).

```text
KANIT   : <sayac> -> MD=NNN, FM=0 eksik, BROKEN=0, MOJIBAKE=0
DOSYA   : .ai/CLAUDE.md, .ai/WORKFLOW.md, .ai/ROLE.md, .ai/AGENTS.md
EKSIK   : YOK   (varsa: DOĞRULAMA GEREKLI maddesi yazilir)
SONRAKI : <adim, ~ sure>
```

| Satır | Ne taşır | Boş bırakılır mı | Boşsa ne olur |
|---|---|---|---|
| `KANIT` | Çalıştırılmış komutun çıktısı (sayı ya da PASS/RED) | Hayır | G5 ihlali — iş bitmedi sayılır |
| `DOSYA` | Dokunulan dosya yolları; hiç dokunulmadıysa `yok` | Hayır | Kapsam belirsiz — gate bekler |
| `EKSIK` | Bilinmeyen / doğrulanmamış madde; yoksa `YOK` | Hayır | `DOĞRULAMA GEREKLI` yazılır, madde kapanmaz |
| `SONRAKI` | Sıradaki adım + tahmini süre | Hayır | Sıra belirsiz — iş durur |

**Doldurma örneği (biçim, sayılar uydurma değildir):**

```text
KANIT   : powershell -NoProfile -ExecutionPolicy Bypass -File scripts/measure-vault.ps1 -> MD=<olculen>, FM=0, MOJIBAKE=0
DOSYA   : .ai/index.md, .ai/CLAUDE.md
EKSIK   : DOĞRULAMA GEREKLI - scripts/check-links.ps1 ciktisi alinmadi
SONRAKI : check-links calistir (~2 dk)
```

**Karıştırma belirtisi:** kutuda `BASLIK` ya da `RİSK` satırı görülmüşse bu ajan kutusudur, gate kutusu değildir; gate kanıtı geçersiz sayılır ve dört satırla tekrar istenir.

---

## §4 Dur / Bekle Kuralları

### §4.1 Durduran 5 durum

| # | Durum | Etiket | Yapılacak |
|---|---|---|---|
| 1 | **GATE-003 kapalı** (wireframe onayı yok) | `GATE-OPEN` | `.cs` yazımı **yasak**; okuma/planlama serbest |
| 2 | Yıkıcı işlem gerekiyor | `G7 ONAYI` | Tek satır onay sorusu + A/B/C; onay gelmeden komut çalışmaz |
| 3 | Yeni paket / sürüm / ORM | `G8` | Kullanıcı onayı; "geçici çözüm" yok |
| 4 | Aynı hata 3 düzeltmede çözülmedi | `3-STRIKE` | Kod yazmayı bırak, şüpheli varsayımı **adlandır**, A/B/C ile sor |
| 5 | Bilgi yok / çelişki çözülmedi | `UNKNOWN` / `DECISION BLOCKED` | Madde yazma, işi durdur |

### §4.2 Onay istemi şablonu

```text
G7 ONAYI GEREKIYOR
  ISLEM  : <tam yol> - <ne yapilacak>
  ETKI   : <geri alinabilir mi: EVET/HAYIR + kurtarma yolu>
  SECENEK: A) uygula   B) iptal   C) alternatif oner
  BEKLEME: onay gelmeden ISLEM YAPILMAZ
```

### §4.3 Gate'ler kimin (özet)

| Gate | Ne kapatır | Kapatan |
|---|---|---|
| GATE-004a | Vault yeşil (500 MD · 0 bozuk link · 0 mojibake) | kullanıcı (kanıt: [WORKFLOW](WORKFLOW.md) §2) |
| **GATE-003** | **UI onayı — HARD STOP** | **yalnız kullanıcı** |
| GATE-004 | İskelet + iki bacak build + ilk commit | kullanıcı (kanıt: `dotnet build`) |
| GATE-005 | Test + NetArchTest 0 ihlal | kullanıcı (kanıt: `dotnet test`) |
| GATE-006 | Teslim | kullanıcı |

> **KURAL:** Ajan gate'i **hazırlar**, asla **kapatmaz**. Gate ayrıntısı [WORKFLOW.md](WORKFLOW.md) §2'dedir; burası yalnız bağlayan çizgidir.

### §4.4 3-STRIKE sonrası ne olur — durum tablosu

| # | 3. strike sonrası durum | Etiket | Ajanın yapacağı | Kullanıcının göreceği |
|---|---|---|---|---|
| 1 | Aynı hata üç kez tekrarladı | `3-STRIKE` | Kod yazmayı bırak, şüpheli varsayımı **adlandır**, A/B/C ile sor | Bloke raporu (6 satır) |
| 2 | Gate kapalı, iş kod gerektiriyor | `GATE-OPEN` | Okuma ve planlama serbest; tek satır `.cs` yazma | Gate kapanış istemi |
| 3 | Yıkıcı işlem gerekiyor | `G7 ONAYI` | Tek satırlık onay sorusu + A/B/C | Onay şablonu (§4.2) |
| 4 | Yeni paket / sürüm / ORM isteği | `G8` | Alternatifi öner, "geçici çözüm" kapısı olmadığını yaz | `DECISION BLOCKED` + tek soru |
| 5 | Bilgi yok ya da çelişki çözülmedi | `UNKNOWN` / `DECISION BLOCKED` | Madde yazma, işi o adımda durdur | `DOĞRULAMA GEREKLI` maddesi |

**Ortak sonuç:** beş durumun beşinde de iş **durmaz, askıya alınır**; askıya alma `log.md`'ye yazılır ve iş askıdayken başka aşamaya sıçramaz.

---

## §5 Katman Yasakları (ADR-028 bağlayıcı)

**Bağlayıcı kaynak:** [ADR-028 — Katmanlı Mimari Baz Kararı](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [architecture/index](architecture/index.md) · [layer-boundary-check](checklists/layer-boundary-check.md)

| Makro katman | Proje | Kesinlikle YASAK |
|---|---|---|
| `l0` Abstractions (N01-N05) | `VersaCoder.Abstractions` | DevExpress · ADO.NET · dosya/saat okuyan somut sınıf |
| `l1` Domain (N06-N11) | `VersaCoder.Domain` | Infrastructure/Host/UI `using` · SQL · `DateTime.Now` |
| `l2` Application (N12-N18) | `VersaCoder.Application` | Infrastructure'i tanıma · UI tipini iş mantığında taşımak |
| `l3` Crosscutting (N19-N24) | `VersaCoder.Crosscutting` | Kural iş mantığı · `l0` dışına bağımlılık |
| `l4` Infrastructure (N25-N32) | `VersaCoder.Infrastructure` | EF Core · string-concat SQL · parametresiz `Execute` |
| `l5` Protocol (N33-N37) | `VersaCoder.Protocol` | UI tipine referans · runtime üretim SQL'i |
| `l6` Host (N38-N40) | `VersaCoder.Host` | İş mantığı (yalnız kompozisyon + startup) |
| `l7` UI + CLI (N41-N44) | `VersaCoder.Desktop` + `VersaCoder.Cli` | İş mantığı · yetkisiz `.Designer.cs`/`.resx` müdahalesi |
| test | `VersaCoder.Tests` (+ gerekirse `VersaCoder.ArchitectureTests`) | Üretim kodunda "küçük düzeltme" |

**İzinli yön (saf ASCII):**

```text
 l7-ui  ------>  l6-host  ------>  l5-protocol
                      |                  |
                      v                  v
              l4-infrastructure --> l2-application --> l1-domain --> l0-abstractions
                      |                     |                 ^
                      +--> l3-crosscutting --+                 |
                             (l3 yalniz l0'a baglanir)          |
                                                                |
                     (her katman iceriye yalniz interface ile baglanir)
```

**İki doğrudan ok yasağı (8x8 matris):** `l4 -> l2` **X** · `l5 -> l4` **X** · `l3` yalnız `l0`. Matrisin tamamı NetArchTest `LayerRulesTests` ile aynı kaynaktan gelir; detay [layer-boundary-check](checklists/layer-boundary-check.md).

### §5.1 ADR-028 özet tablosu — 8 katman × proje × sorumluluk

> Bu tablo **özet**tir; bağlayıcı metin [ADR-028](.decisions/accepted/ADR-028-layered-baseline-9-projects.md) §2.1'dedir. Tablo ile ADR çelişirse ADR kazanır ve çelişki `CONFLICT DETECTED` ile raporlanır.

| # | Makro katman | Proje (.csproj) | N-kod | Sorumluluk (ne taşıyor) | Sınırını bozan şey |
|---|---|---|---|---|---|
| 1 | `l0` Abstractions | `VersaCoder.Abstractions` | N01-N05 | Arayüzler ve sözleşmeler; hiçbir somut uygulama | Somut sınıf, dış paket referansı, DevExpress |
| 2 | `l1` Domain | `VersaCoder.Domain` | N06-N11 | İş kuralları ve varlıklar; saf C#, yan etkisiz | `Infrastructure`/`UI` `using` · SQL · `DateTime.Now` |
| 3 | `l2` Application | `VersaCoder.Application` | N12-N18 | Kullanıcı senaryoları; portları çağırır, sırayı yönetir | Infrastructure'i tanımak · UI tipini iş mantığında taşımak |
| 4 | `l3` Crosscutting | `VersaCoder.Crosscutting` | N19-N24 | Kesit sorumlulukları: log, hata biçimi, zaman/saat sarmalayıcı | Kural iş mantığı · `l0` dışına bağımlılık |
| 5 | `l4` Infrastructure | `VersaCoder.Infrastructure` | N25-N32 | ADO.NET + Dapper sorguları, dosya, ağ, migration uygulaması | EF Core · string-concat SQL · parametresiz `Execute` |
| 6 | `l5` Protocol | `VersaCoder.Protocol` | N33-N37 | Araçlarla konuşma sözleşmesi: araç çağrısı, akış (stream) | UI tipine referans · çalışma zamanı üretim SQL'i |
| 7 | `l6` Host | `VersaCoder.Host` | N38-N40 | Kompozisyon ve başlangıç: doğru kayıtlar, doğru yaşam döngüsü | İş mantığı yazmak (yalnız kurulum + birleştirme) |
| 8 | `l7` UI + CLI | `VersaCoder.Desktop` + `VersaCoder.Cli` | N41-N44 | RibbonForm ekranları ve komut satırı yüzeyi | İş mantığı · yetkisiz `.Designer.cs`/`.resx` müdahalesi |
| — | test | `VersaCoder.Tests` (+ gerekirse `VersaCoder.ArchitectureTests`) | — | Üretim kodunu sınar; NetArchTest yön denetimini çalıştırır | Üretim kodunda "küçük düzeltme" |

**Tek satırlık okuma yönü:** `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0`; `l3` yalnız `l0`'a bakar; `l4` ile `l2` arasında doğrudan ok **yok** (port `l0`'da, uygulaması `l4`'te).

**Yeni tip nereye yazılır (hızlı karar):** tip arayüzse `l0`, iş kuralıysa `l1`, senaryoysa `l2`, kesit ise `l3`, somut dış dünya erişimiyse `l4`, araç sözleşmesiyse `l5`, kurulum ise `l6`, ekran/komut ise `l7`. Kararsız kaldığında tahmin yazma: `DOĞRULAMA GEREKLI` + [architecture/index.md](architecture/index.md).

---

## §6 Sayısal Gerçekler (uydurma yasak)

| # | Sayı | Anlam | Kaynak |
|---|---|---|---|
| 1 | **8 makro katman / 52 düğüm** | l0..l7 + N01-N44 alt katman (klasör/namespace düzeyi) | ADR-028 §2.1 |
| 2 | **9 üretim + 1-2 test projesi** | en fazla 11 `.csproj` × 2 TFM = 22 hedef | ADR-028 §2.1 |
| 3 | **5440 katalog kaydı** | `count-components` = **PASS, 0 marj** (tek satır silinirse RED) | ADR-028 C4/§4 |
| 4 | **K3 = 618 giriş / 616 benzersiz** | K3 envanteri **kapsam dışı**; kataloga girmez | ADR-028 §2.2 |
| 5 | **CPM'te test paketleri yok** | `NetArchTest.Rules` · `xunit` · `Microsoft.NET.Test.Sdk` · `NSubstitute` `PackageVersion` satırları eklenmedi → **NU1008 riski** | ADR-028 C5/§2.3 |
| 6 | **500 MD** | GATE-004a eşiği; ölçüm gate kapanışında alınır | bu vaultun gate eşiği |
| 7 | **7 ekran** | wireframe kapsamı: `01-start-page` … `07-settings` | [VISION](VISION.md) |

> Bir hücrede kaynak yoksa hücreye **`DOĞRULAMA GEREKLI`** yazılır; tahmin sayı girmez (G9).

---

## §7 SSS (Anayasa — 8 Soru)

**S1 — Başka bir `.ai` dosyası bu anayasayla çelişirse?** Bu dosya kazanır; çelişki ayrıca `G10` etiketiyle raporlanır ve ilgili dosya düzeltilir.

**S2 — Kanıtım yoksa ne yazarım?** `UNKNOWN` ya da `NEEDS_VALIDATION` (`DOĞRULAMA GEREKLI`) — ve işi o adımda durdurursun.

**S3 — Bu anayasayı kim değiştirebilir?** Yalnız kullanıcı. G maddesi eklemek/silmek kullanıcı onayı ister ve `log.md`'ye kaydedilir.

**S4 — Bu anayasa ile başka bir `.ai` dosyasındaki kural aynı şeyi mi söylüyor?** Söylüyorsa tekrar etme, yalnız [index.md](index.md) üzerinden bağ ver; farklı söylüyorsa bu dosya kazanır ve fark `log.md`'ye not edilir. Aynı madde iki dosyada ayrı ayrı yaşarsa SSOT ihlali başlar.

**S5 — Kanıt kutusu ile ajan çıktı kutusunu karıştırırsam ne olur?** Gate kanıtı geçersiz sayılır: 5 alanlı kutu (`BASLIK/KAPSAM/KARAR/RİSK/KANIT`) ajanın özetidir, 4 satır (`KANIT/DOSYA/EKSIK/SONRAKI`) gate'in kabul kanıtdır. Farklı yerde kullanılan kutu, o gate'de yeniden istenir.

**S6 — "Bilmiyorum" demek işi batırır mı?** Hayır; gizlemek batırır. `UNKNOWN` + işi o adımda durdurmak doğru davranıştır (§3.1). Tahmin yazmak G9 ihlalidir ve madde silinir — silinen madde, yazılmamış sayılır.

**S7 — G maddeleri sırasıyla ezberlenmeli mi?** Hayır; §2'deki 8 tespit komutu maddelerin çoğunu otomatik yakalar. Ezber yerine akış bilgisi yeterlidir: neyi yaptığını söyle → hangi G'ye dokunuyor → kanıtı üret → kutuyu doldur.

**S8 — Bir madde birden fazla G'ye giriyorsa hangisi geçerli?** En ağır olanı (`CRITICAL` > `HIGH` > `RED`) raporlanır, ayrıca ilgili tüm maddeler `log.md`'ye tek satırda etiketlenir. Ağırlık sırası §1 tablosundaki `İhlalde` sütunundan okunur.

---

## §8 Doğrulama

- [ ] Frontmatter 8 alan, ilk satır `---`, `authority: SSOT`, `updated: 2026-09-29`
- [ ] G1..G12 tek tek `kural/yasak/ihlalde/kanıt` dörtlüsünde; 7 ana yasak açıkça yazılı
- [ ] §3'te 7 doğrulama etiketi + 4 satır kanıt kutusu + hata raporu üçlüsü
- [ ] §4'te 5 dur/bekle durumu + G7 onay şablonu
- [ ] §5 ADR-028'e bağlı, diyagram **saf ASCII** (Unicode kutu çizim yok)
- [ ] §6 sayıları yalnız kaynaklı; kaynaksız hücre `DOĞRULAMA GEREKLI`
- [ ] Tüm markdown linkleri yalnız izinli hedeflere (bkz. başlık bağlantıları)

### §8.1 Kırmızı ekip — anayasanın en sık delinen 8 yeri

| # | Delik | Belirtisi | Yakalama | Düzeltme |
|---|---|---|---|---|
| 1 | "Geçici paket" eklenmiş | `Directory.Packages.props`'ta onaysız satır | §2 satır 3 | Paketi kaldır ya da `G8` onayı al |
| 2 | Kanıt kutusu `KANIT` satırı boş | Kapanışta komut çıktısı yok | §3.4 tablosu | Komutu çalıştır, çıktıyı yaz; yoksa `UNKNOWN` |
| 3 | Diyagramda Unicode çizim | `U+2500-U+257F` taraması > 0 | §2 satır 8 | `|`, `-`, `+` ile yeniden çiz |
| 4 | Katman oku ters | `l4 -> l2` ya da `l5 -> l4` kullanan kod | NetArchTest `LayerRulesTests` | Portu `l0`'a, uygulamayı `l4`'e taşı |
| 5 | Ajanın gate'i kapatması | `log.md`'de onay satırı yok, gate kapanmış | §4.3 tablosu | Gate'i yeniden aç, kapanışı kullanıcıya sor |
| 6 | Etiketsiz iddia | "olması gerekir" / "herhalde" benzeri ifade | §3.1 tablosu | Etiketi ekle ya da maddeyi sil |
| 7 | Tek bacakta bırakılmış kod | Bir TFM'de derleniyor, diğeri hata veriyor | `-f net48` ve `-f net10.0-windows` | `#if`'i yalnız platform farkına indir |
| 8 | Kopya metin | Cümleler eski vaultla birebir aynı | G1 benzerlik taraması | Maddeleri kendi cümlenle yeniden yaz |

**Ortak belirti:** sekiz deliğin yedisi **kanıt eksikliği** olarak görünür; bu yüzden §3.4 kutusu doldurulmadan hiçbir iş "bitti" sayılmaz.

---

## §9 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Katman/proje baz kararı (bağlayıcı) | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` |
| 2 | Süreç ve gate'ler | `WORKFLOW.md` |
| 3 | Rol ve karar hiyerarşisi | `ROLE.md` |
| 4 | Ajan dağıtım matrisi | `AGENTS.md` |
| 5 | Katman sınırı kontrol listesi | `checklists/layer-boundary-check.md` |
| 6 | Navigasyon · karar defteri · oturum durumu | `index.md` · `brain.md` · `MEMORY.md` |
