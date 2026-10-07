---
title: "VersaCoder — Ajan Protokolü (Agent Protocol)"
type: registry
category: vault-root
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "Vault genelinde ajan tanımı, yetki matrisi, devre paketi, subagent kararı, context bütçesi, adlandırma, escalation, DoD ve SSS"
---

# VersaCoder — Ajan Protokolü (`AGENTS.md`)

**Zorunlu Bağlantılar:** [[index.md]] · [[keys.md]] · [[CLAUDE.md]] · [[WORKFLOW.md]] · [[ROLE.md]] · [[.agents/AGENTS.md]]

> **Tek cümlelik sorumluluk:** *Bu vault'ta "ajan" kimdir, neye yetkilidir, işi kime devreder, ne zaman subagent açılır, üç kez takılırsa kime escalite edilir, işi ne sayılır biter* sorusunun tek cevabıdır.
> Bu dosya **protokoldür**. Ajanın tekil görev tanımı [.agents/AGENTS.md](.agents/AGENTS.md) ve `.agents/<rol>.md` dosyalarındadır; çelişirse `.agents/` kazanır, bu dosya yalnız bağlantı verir (SRP).

---

## §0 Özet (Answer First)

Kayıtlı **5 ajan** var: `architect`, `developer`, `reviewer`, `tester`, `ui-designer`. Her ajanın **tek X (son söz) alanı** vardır; çatışmada X sahibi karar verir, mimari ∩ kod çatışmasında `architect` üstündür. İş devri **4 maddelik devre paketi** ile yapılır (üretilen · kanıt · bilinen eksik · sonraki adım) — paket eksikse devre geçersizdir. Aynı iş 2+ ajan bağlamını yakıyorsa veya tek dosyadan büyükse **subagent** açılır, değilse **inline** çalışılır. **3 başarısız düzeltmeden sonra kod yazmak yasaktır**; durulur, şüpheli varsayım adıyla birlikte kullanıcıya escalite edilir.

---

## §1 Amaç ve Kapsam

### §1.1 Bu dosya ne — ne değil

| Soru | Tek kaynak | Bu dosyada mı? |
|---|---|---|
| Ajan hangi dosyayı **yazar/okur/onaylar** | [`.agents/AGENTS.md`](.agents/AGENTS.md) §3 yetki matrisi | Hayır (yalnız §3.4'te özetlenir) |
| Ajanın **görev tanımı** (girdi → çıktı, checklist, guardrail) | [`architect.md`](.agents/architect.md) · [`developer.md`](.agents/developer.md) · [`reviewer.md`](.agents/reviewer.md) · [`tester.md`](.agents/tester.md) · [`ui-designer.md`](.agents/ui-designer.md) | Hayır |
| Vault genelinde **protokol**: devre paketi, subagent eşiği, context bütçesi, oturum adı, escalation, DoD, SSS | **bu dosya** | **Evet** |
| Kimin rolü, öncelik çatışması, ses kuralları | [ROLE.md](ROLE.md) | Hayır |
| Ajan hangi part/gate'te çalışır | [WORKFLOW.md](WORKFLOW.md) §2–§3 | Hayır |

> **KURAL:** İki dosya çakışırsa `.agents/AGENTS.md` (rol bazlı gerçek) kazanır, bu dosya (vault protokolü) yalnız **genel kural** koyar. Aynı bilgi iki yerde yazılırsa SSOT ihlali var → tekrar olan satır bu dosyadan silinir.

### §1.2 Ajan nedir (bu vault'ta)

**Ajan = belirli bir yetki alanına sahip, tek sorumluluklu, çıktısı ölçülebilir çalışma rolü.** Bir "sohbet tonu" değil, bir **işbirliği sözleşmesidir**.

| # | Ajanın 5 özelliği | Karşılığı olmayan şey |
|---|---|---|
| 1 | **Tek sorumluluk (SRP):** tek soru, tek çıktı kümesi | "Genel asistan" |
| 2 | **Sahiplik (X):** bir dosya/tip kümesinde son söz onda | Fikir beyanı |
| 3 | **Girdi-çıktı sözleşmesi:** eline ne gelir, ne çıkarır | Serbest sohbet |
| 4 | **Devre (handoff):** işi 4 maddelik paketle devreder | "devam et" mesajı |
| 5 | **Kanıt zorunluluğu:** çıktısı build/test/ölçümle kanıtlanır | Tahmini başarı |

### §1.3 Kapsam Dışı

| # | Bu dosyada DEĞİL | Nerede |
|---|---|---|
| 1 | Runtime agent çalışma zamanı (`VersaCoder.Protocol`, session, streaming) | `spec/session-spec.md` (PART 7, planlı) |
| 2 | opencode tool sözleşmesi (`read`/`write`/`edit`/`bash`/`grep`) | `spec/tool-spec.md` (PART 7, planlı) |
| 3 | MCP server / plugin / sandbox | `spec/mcp-spec.md` (PART 7, planlı) |
| 4 | Ajan rol metni (kimlik, misyon, tech stack) | `.agents/<rol>.md` |
| 5 | Gate onay yetkisi (kullanıcıya ait) | [WORKFLOW.md](WORKFLOW.md) §3 |

---

## §2 Kayıtlı 5 Ajan

### §2.0 Envanter Özeti

| # | Ajan | Tek cümle özet | Ana çıktı | Devreder → | DoD § |
|---|---|---|---|---|---|
| 1 | [`architect`](.agents/architect.md) | Sistemin **nasıl kurulacağına** ve hangi kararın **geri alınabilir** olduğuna karar verir | ADR + katman sözleşmesi + ASCII mimari | `developer` | §9.1 |
| 2 | [`developer`](.agents/developer.md) | Sözleşmeyi **derlenen koda** çevirir (`.cs` + test) | `.cs` + `dotnet build` çıktısı | `reviewer` | §9.2 |
| 3 | [`reviewer`](.agents/reviewer.md) | Kodun **geçip geçmediğine** PASS/RED verir | Bulgu listesi (seviye + konum + düzeltme) | PASS → `tester` · RED → `developer` | §9.3 |
| 4 | [`tester`](.agents/tester.md) | Davranışın **kanıtını** üretir | xUnit testleri + yeşil `dotnet test` | `reviewer` | §9.4 |
| 5 | [`ui-designer`](.agents/ui-designer.md) | Ekranın **görünümünü ve kontrol setini** kilitler | ASCII wireframe → Form + `.Designer.cs` + `.resx` | `developer` | §9.5 |

**Devre akışı (tek yön, geri dönüş yalnız RED ile):**

```
architect --(ADR + katman sozlesmesi)--> developer --(kod + build kaniti)--> reviewer
                                                                                  |
                                                       +---------- RED -----------+-------- PASS ------+
                                                       v                                                    v
                                                  developer                                           tester
                                                       ^                                            (test + coverage)
                                                       |                                                   |
                                                       +------------- RED ------------- bulgu --------------+
                                                                                                          v
                                                                                                       rapor (reports/)
ui-designer --(GATE-003 onayli wireframe)--> developer
```

> **UYARI:** `ui-designer` → `developer` devresi **yalnız [WORKFLOW.md](WORKFLOW.md) GATE-003 kapandıktan sonra** açılır. Onaysız wireframe ile kodlamaya başlamak G12 ihlalidir → iş RED, part baştan.

---

### §2.1 architect — Senior Software Architect

| Alan | Değer |
|---|---|
| **Rol özeti** | .NET & WinForms mimarisi, katman sözleşmesi, ADR defteri, teknoloji sürüm kilidi (`net48` + `net10.0-windows`, DevExpress **v26.1**, ADO.NET + **Dapper**) |
| **Yetki (X = son söz)** | `.decisions/**` (ADR) · `architecture/**` · `.rules/**` · `spec/**` · katman bağımlılık yönü · `#if` kullanım kuralı |
| **Yetkisiz olduğu yer** | `src/**/*.cs` implementasyonu · `.Designer.cs`/`.resx` · `tests/**` · wireframe onayı |
| **Girdi** | `reports/00-kapsam-analizi.md` · iki bacak kısıtı · `CLAUDE.md` G1..G12 |
| **Çıktı** | ADR (`.decisions/accepted/ADR-NNN-*.md`) + katman sözleşmesi + ASCII diyagram |
| **Devre →** | `developer`: `ADR + katman sözleşmesi + engel listesi` |
| **Devre ←** | `ui-designer` wireframe'i mimariye uygun mu (hızlı `X`) |
| **Sık ihlal ettiği guardrail** | G6 (tek bacaklı tasarım) · G8 (sürüm sapması) |

**Tipik iş akışı:**

```
1. Kapsam + iki bacak kisiti oku              (girdi: reports/00-kapsam-analizi.md)
2. Kaynak hiyerarşisi uygula                  (ROLE.md 3)
3. Celiski varsa CONFLICT DETECTED            (CLAUDE.md G10)
4. Karari ver -> ADR-NNN ac                   (alternatif >= 2, sonuc + ve -)
5. Katman sozlesmesini guncelle               (architecture/)
6. .rules/ karsiligini guncelle                (guardrail kacinilmaz yapilir)
7. log.md'ye kayit dus                         (tarih + karar + gerekce)
8. developer'a 4 maddelik devre paketi gonder
```

> **KURAL:** `architect` bir işi "bitti" diyemez; **kanıt = ADR dosyası + `.rules/` güncellemesi + `log.md` kaydı**. Kod derlendi diye mimari onaylanmaz.

---

### §2.2 developer — Senior C# Engineer

| Alan | Değer |
|---|---|
| **Rolözeti** | Clean Architecture + SOLID altında kod yazar; DDD/CQRS **yalnız gerektiği yerde**; her karar iki bacakta (`net48` + `net10.0-windows`) değerlendirilir |
| **Yetki (X = son söz)** | `src/**/*.cs` imzası, refactor biçimi, iç implementasyon ayrıntısı |
| **Yetkisiz olduğu yer** | Katman bağımlılık yönü (`architect`) · `.Designer.cs`/`.resx` son sözü (`ui-designer`) · test kapsamı (`tester`) · PASS/RED (`reviewer`) |
| **Girdi** | ADR + katman sözleşmesi (architect) · onaylı wireframe (ui-designer) |
| **Çıktı** | `.cs` + yeşil `dotnet build` çıktısı |
| **Devre →** | `reviewer`: `kod + build kanıtı + bilinen eksik` |
| **Devre ←** | `architect` (sözleşme) · `ui-designer` (form) · `reviewer` (RED bulgusu) |
| **Sık ihlal ettiği guardrail** | G2 (EF Core / string-concat SQL) · G5 (kanıtsız "bitti") · G6 (tek bacak) |

**Tipik iş akışı:**

```
1. Devre paketini oku (4 madde)                 -> eksikse GERI CEVIR
2. ADR + katman sozlesmesini oku                 -> sozlesmeye aykiri kod YAZMA
3. En kucuk derlenebir birimi yaz                (tek class / tek metot)
4. dotnet build                                  -> 0 hata degilse DUR
5. Kendi gozunu gec (SATR/STYLE kontrol)
6. reviewer'a devre paketi gonder
```

> **UYARI:** `developer`, `architect` sözleşmesini beğenmezse **sessizce es geçemez**. Ya `architect`'e geri döner (`CONFLICT DETECTED`) ya da kullanıcıya escalite eder. Sözleşmesiz kod = yeniden yazım riski.

---

### §2.3 reviewer — Code Reviewer

| Alan | Değer |
|---|---|
| **Rol özeti** | Kodun **geçerliliğini** ölçer: doğru-lük, güvenlik, performans, iki bacak uyumu, vault kuralları. Puansız, **seviyeli** bulgu üretir |
| **Yetki (X = son söz)** | `PASS` / `RED` kararı · bulgu seviyesi (CRITICAL/HIGH/MEDIUM/LOW) · `.ai/reports/**` bulgu raporu |
| **Yetkisiz olduğu yer** | Kodu kendisi düzeltmez (RED → `developer`) · test yazmaz (`tester`) |
| **Girdi** | `developer` devre paketi + ilgili diff |
| **Çıktı** | Bulgu listesi: `seviye · dosya:satır · neden · düzeltme` |
| **Devre →** | PASS → `tester` · RED → `developer` |
| **Devre ←** | `developer` (kod) |
| **Sık ihlal ettiği guardrail** | G5 (kanıtsız PASS) · G9 (uydurma API iddiası) |

**Bulgu biçimi (zorunlu şablon):**

```
[SEVIYE] dosya/yolu.cs:123
  NEDEN   : neden yanlis (kural referansi: CLAUDE.md G2 / ADR-004)
  DuzELTME: ne degistirilmeli (somut, tek islem)
  KANIT   : test/derleme/log satiri  -  ya da UNKNOWN
```

| Seviye | Anlam | Devresi | SLA |
|---|---|---|---|
| **CRITICAL** | Güvenlik / veri kaybı / anayasa ihlali | → `developer`, iş durur | ≈10 dk içinde geri dönüş |
| **HIGH** | Mimari sözleşmesi ihlali, iki bacak kırığı | → `developer` (gerekirse `architect`) | ≈30 dk |
| **MEDIUM** | Test açığı, tekrar, ihlal edilebilirlik | → `developer` | aynı oturum |
| **LOW** | İsim, yorum, stil | → kuyruğa alınır, işi kilitlemez | sonraki part |

> **KURAL:** `reviewer` PASS verirken **kanıt zorunlu**: `dotnet build` çıktısı ya da test log'u yoksa PASS geçersizdir (G5). "Bence temiz" PASS değildir.

---

### §2.4 tester — Test Engineer

| Alan | Değer |
|---|---|
| **Rol özeti** | Davranışın kanıtını üretir: xUnit + NSubstitute + NetArchTest; TDD gerekiyorsa RED-GREEN-REFACTOR; her iki bacakta da çalışır |
| **Yetki (X = son söz)** | `tests/**` · coverage hedefi · test scope'u (neyi kapsam dışı bıraktığı) |
| **Yetkisiz olduğu yer** | Üretim kodunu düzeltmez · bulgu seviyesi vermez (`reviewer`) |
| **Girdi** | `reviewer` PASS'ı + kabul kriterleri (`spec/` / ADR) |
| **Çıktı** | Test dosyaları + yeşil `dotnet test` çıktısı + coverage kanıtı |
| **Devre →** | `reviewer` (son doğrulama) → rapor |
| **Devre ←** | `reviewer` (PASS) |
| **Sık ihlal ettiği guardrail** | G5 (kanıtsız coverage) · G2 (EF Core ile test verisi) |

**Test katmanı sırası:**

```
1. Birim test        -> saf, I/O yok, net48 + net10 ayni kod
2. Entegrasyon test  -> SQLite (Microsoft.Data.Sqlite), gercek schema
3. Mimari test       -> NetArchTest: katman yonu + circular dependency YOK
4. Kanit             -> dotnet test ciktisi (sayaclar + sure) + coverage raporu
```

> **UYARI:** `tester`, üretim kodunda "küçük düzeltme" yapamaz. Test geçmiyorsa kök neden ister: üretim kodu ise → `developer`, test yanlışsa → kendi yetkisinde düzeltir ve `log.md`'ye not düşer.

---

### §2.5 ui-designer — DevExpress UI Engineer

| Alan | Değer |
|---|---|
| **Rol özeti** | 7 ekranın tel kafesini (ASCII wireframe) çizer, DevExpress **v26.1** kontrol setini kilitler, `RibbonForm` + `RibbonControlStyle.Office2010` kalıbına uygun Form üretir |
| **Yetki (X = son söz)** | `ui-design/**` · `.Designer.cs` · `.resx` · kontrol seçimi · tema/layout |
| **Yetkisiz olduğu yer** | İş mantığı (`.cs` code-behind yalnız olay yönlendirici) · ADR · test kapsamı |
| **Girdi** | `VISION.md`/`PROJECTS.md` 7 ekran listesi · `reports/00-kapsam-analizi.md` §4.1 kontrol adayları |
| **Çıktı** | `ui-design/NN-<ekran>.md` (ASCII wireframe) → GATE-003 → Form + `.Designer.cs` + `.resx` |
| **Devre →** | `developer` (**yalnız GATE-003 sonrası**) |
| **Devre ←** | `architect` (mimari kısıt `X`) |
| **Sık ihlal ettiği guardrail** | G3 (sürüm/stil sapması) · G4 (bozuk link, frontmatter'sız MD) · G12 (onaysız kodlama) |

**Wireframe → Form zinciri:**

```
ui-design/NN-<ekran>.md        (ASCII tel kafes + kontrol listesi)
        |
        v
  GATE-003  -- KULLANICI ONAYI (HARD STOP) --
        |
        v
<Ekran>.Form.cs + <Ekran>.Form.Designer.cs + <Ekran>.Form.resx
        |
        v
  developer: olay baglayicilari + Application katmanina cagri
```

> **UYARI:** `.resx` elle DEĞİL designer üzerinden üretilir; `ResourceManager` adı bozulursa `G3/G4` ihlali doğar. Elle müdahale gerekiyorsa `developer` değil, **`ui-designer`** yapar.

---

## §3 Yetki Matrisi (Karar Sahipliği)

### §3.1 Dosya yetkisi → nereye bakılır

Dosya bazlı R/W/X matrisi **[`.agents/AGENTS.md`](.agents/AGENTS.md) §3** dosyasındadır ve tekrarlanmaz (SSOT). Bu dosya yalnız **karar** yetkisini tanımlar:

### §3.2 Karar yetkisi (hangi ajan hangi KARARA son söz der)

| # | Karar | X (son söz) | Danışılan | Escalation |
|---|---|---|---|---|
| 1 | Katman bağımlılık yönü, proje referansı | `architect` | `developer` | kullanıcı |
| 2 | Yeni teknoloji / NuGet / sürüm ekleme | `architect` | `developer` | **kullanıcı (G8)** |
| 3 | Metot imzası, refactor biçimi, iç yapı | `developer` | `architect` (sözleşme ihlaliyse) | `architect` |
| 4 | Kod PASS / RED | `reviewer` | `tester` | kullanıcı (2× RED sonrası) |
| 5 | Bulgu seviyesi (CRITICAL..LOW) | `reviewer` | `architect` (HIGH ise) | kullanıcı |
| 6 | Test kapsamı, coverage hedefi, skip gerekçesi | `tester` | `reviewer` | kullanıcı |
| 7 | Kontrol seçimi, layout, tema, wireframe | `ui-designer` | `architect` (mimari kısıt) | kullanıcı |
| 8 | `.Designer.cs` / `.resx` düzeni | `ui-designer` | — | — |
| 9 | Vault kuralı değişikliği (`.rules/`) | `architect` | `reviewer` (bulgu) | kullanıcı |
| 10 | **Gate onayı** (GATE-001..006) | **kullanıcı** | ilgili ajan raporu | — |
| 11 | Kapsam değişikliği (ne eklenir/ne çıkar) | **kullanıcı** | `architect` | — |
| 12 | Ürün metni / ekran adı / marka | **kullanıcı** | `ui-designer` | — |

> **KURAL:** X sahibi **kararı gerekçelendirir**, uygulamayı kendisi yapmak zorunda değildir. Örneğin `reviewer` RED verir ama düzeltmeyi `developer` yapar.

### §3.3 Çatışma çözümü sırası (öncelikten aşağı)

```
1. Ilgili satirdaki X sahibi karar verir
2. Celiski mimari ∩ kod ise           -> architect ustundur
3. Celiski kanitli ise (build/test)   -> kanit kazanir, gorus degildir
4. Celiski CLAUDE.md G1..G12 ile ise -> anayasa kazanir, muzakere yok
5. Hala cozulmediyse                  -> DECISION BLOCKED -> kullaniciya sor
```

| Durum | Yazılı etiket | Aksiyon |
|---|---|---|
| Kaynaklar çelişiyor | `CONFLICT DETECTED` | Hiyerarşiyi uygula ([ROLE.md](ROLE.md) §3) |
| Hiyerarşi de çözmedi | `DECISION BLOCKED` | Kullanıcıya 1 kısa soru |
| Bilgi elimizde yok | `UNKNOWN` | Uydurma, işleme al |
| Doğrulanmalı | `NEEDS_VALIDATION` | İşaretle, devam et |
| Çıkarım | `INFERRED` | Açıkça "çıkarım" diye yaz |

### §3.4 Yetki ihlali — belirtileri ve sonucu

| İhlal | Belirti | Sonuç |
|---|---|---|
| `developer` `.resx`'i ellemiş | `Designer.cs` ≠ `.resx` tutarsız, form açılmıyor | RED → `ui-designer`'a devret |
| `tester` üretim kodunu düzeltmiş | Test yeşil ama davranış değişmiş | Değişiklik geri alınır → `developer` |
| `reviewer` kendi bulgusunu kapatmış | Aynı hata production'da tekrar | PASS iptal → 2. inceleme `architect` |
| `architect` kod yazmış | Sözleşme + implementasyon tek elde | Devasa review riski → parçala |
| `ui-designer` iş mantığı yazmış | Code-behind 200+ satır | Taşınma → `Application` katmanı |

---

## §4 Devre (Handoff) Paketi Protokolü

### §4.1 Kural

Bir ajan işi başka ajana **sözel olarak değil, 4 maddelik paketle** devreder. Paket eksikse devre **geçersizdir** ve alan ajan işe başlamaz → rol geri çevirir.

### §4.2 Paket şeması (zorunlu 4 madde)

```markdown
## DEVRE: <kaynak_rol> -> <hedef_rol>   (tarih, WORKFLOW part/gate)

1. NE URETILDI      : dosya yollari (virgulle, tam yol) + kisa ne oldugu
2. KANIT            : dotnet build / dotnet test / link-tarama ciktisi (sayilarla)
3. BILINEN EKSIK    : eksik veya NEEDS_VALIDATION maddeleri - yoksa "YOK"
4. SONRAKI ADIM     : hedef rolun baslayacagi TEK somut ilk adim
```

### §4.3 Dolu örnek (geçerli)

```markdown
## DEVRE: developer -> reviewer   (2026-09-25, PART 5 / GATE-004)

1. NE URETILDI      : src/VersaCoder.Data/Repositories/SessionRepository.cs,
                      src/VersaCoder.Data/Repositories/SessionRepository.cs'a esli
                      tests/VersaCoder.Data.Tests/SessionRepositoryTests.cs (6 test)
2. KANIT            : dotnet build -> 0 hata (net48 + net10.0-windows, 2 proje);
                      dotnet test  -> 6 gecti, 0 basarisiz, 1.4 sn
3. BILINEN EKSIK    : WAL modu testi yok -> NEEDS_VALIDATION (ADR-003 baglantili)
4. SONRAKI ADIM     : reviewer, SessionRepository.cs satir 40-72 arasi parametreli
                      sorgulari G2 (EF Core / string-concat) acisindan incelesin
```

### §4.4 Geçersiz (RED) örnekler

| Hatalı devre | Neden geçersiz | Doğrusu |
|---|---|---|
| "Kodu yazdım, bakar mısın?" | 1. ve 2. madde yok | Dosya yolu + build çıktısı |
| "Build geçti diye düşünüyorum" | Kanıt tahmini (G5) | Çıktıyı yapıştır, sayı ver |
| "Birkaç eksik var ama önemli değil" | 3. madde belirsiz | Maddeleri tek tek yaz ya da `YOK` |
| "Devam et" | 4. madde yok | Hedefin ilk somut adımı |
| 4 madde var, gate atlanmış | G12 | Önce [WORKFLOW.md](WORKFLOW.md) §3 gate kontrolü |

### §4.5 Devre zamanı ve slası

| Devre | Beklenen süre | Geç kalırsa |
|---|---|---|
| `architect` → `developer` | ADR + sözleşme ≈20 dk | Bekleme → `DECISION BLOCKED` değilse 2. ajan inline devam |
| `developer` → `reviewer` | Build yeşil ise ≈15 dk | Süre dolunca `reviewer` çağrılır (senkron) |
| `reviewer` → `developer` (RED) | ≈10 dk ilk yanıt | 3 RED döngüsü → escalation §8 |
| `reviewer` → `tester` | ≈5 dk | Doğrudan teste geç |
| `ui-designer` → `developer` | GATE-003 sonrası ≈5 dk | Onay yoksa devre **yok** |

---

## §5 Subagent mı, Inline mı?

### §5.1 Karar ağacı (ASCII)

```
YENI IS GELDI
  |
  +-- (a) Tek dosyalik mi, < 30 dk mi? ----------------- HAYIR -> SUBAGENT
  |
  +-- (b) 2+ ajanin baglamini mi yiyor? ---------------- EVET  -> SUBAGENT
  |
  +-- (c) X sahibi baska rol mu? ------------------------ EVET  -> SUBAGENT
  |
  +-- (d) Paralel yurutulebilir mi (bagimlilik yok)? --- EVET  -> SUBAGENT
  |
  +-- (e) Uzun sureli okuma/olcum mu ( > 8 dosya )? ---- EVET  -> SUBAGENT
  |
  +-- hepsi HAYIR ---------------------------------------> INLINE calis
```

### §5.2 Kural tablosu

| # | Durum | Karar | Gerekçe |
|---|---|---|---|
| 1 | Tek dosya düzeltmesi, net kapsam | **inline** | Bağlam kurma maliyeti işten büyük |
| 2 | Aynı anda 2+ dosya okunacak, biri büyük (≥ 8 KB) | **subagent** | Ana oturum bağlamı taşar |
| 3 | İş başka rolün X alanına giriyor | **subagent** | Yetki ihlali riski |
| 4 | 2 iş bağımsız ve paralel yürüyebilir | **subagent (2)** | Duvar saati kısalır |
| 5 | Belirsizlik var, kullanıcıya soru gerekecek | **inline** | Soru-yanıt döngüsü subagent'ta kopar |
| 6 | Yıkıcı işlem (silme/overwrite/git reset) | **inline + onay** | G7 onayı doğrudan kullanıcıdan |
| 7 | Gate onayı bekleniyor (GATE-003 vb.) | **inline + dur** | Subagent onay veremez |
| 8 | Aynı iş 2. kez subagent'ta başarısız | **inline + escalation** | §8.1 kuralı |

> **UYARI:** Subagent **gate atlayamaz**, **onay veremez**, **kullanıcıya soru soramaz** (raporlaır ve bekler). Bu üçü gerekiyorsa iş inline'da kalır.

### §5.3 Subagent açılış paketi (context birleştirme)

Subagent'a yalnız **gerekli** bağlam gider; tam dosya değil, **kesit**:

| # | Gönderilen | Boyut | Gönderilmeyen |
|---|---|---|---|
| 1 | Görev tek cümle + kabul kriteri | ≤ 300 B | Proje tarihi, geçmiş tartışmalar |
| 2 | İlgili dosya yolları (kesit, tamamı değil) | ≤ 4 KB | Tüm vault |
| 3 | İlgili kural (G-madde / ADR özeti) | ≤ 1 KB | Tüm `CLAUDE.md` |
| 4 | Beklenen çıktı şablonu | ≤ 1 KB | — |
| 5 | Devre paketi hedefi (kime sonuç gidecek) | ≤ 200 B | — |

**Toplam subagent açılış bağlamı ≤ 8 KB.** Aşarsa özetle, dosya yolu ver, subagent kendisi okusun.

---

## §6 Context Bütçesi Kuralları

### §6.1 Sayısal limitler

| # | Limit | Değer | Aşılırsa |
|---|---|---|---|
| 1 | Tek turda okunan dosya | **≤ 8 dosya** | Özetle + yola devred |
| 2 | Bir subagent açılış bağlamı | **≤ 8 KB** | Kesit gönder |
| 3 | Devre paketi boyutu | **≤ 2 KB** | Bağlantıya indir, tek satır özet yaz |
| 4 | Tek cevapta madde | **≤ 5 madde** | Tabloya böl, ikinci tura at |
| 5 | Tek turda dokunulan dosya | **≤ 3 dosya** | Parçala, sırayla |
| 6 | Aktif açık subagent | **≤ 2** | 3. talep kuyruğa |
| 7 | Cevap sonundaki "sonraki adım" | **1 adet, < 2 dk** | Listeleme yasak |
| 8 | Ayrıntı okuma (`.Designer.cs`, 1000+ satır) | **yarıya kadar** (`offset`/`limit`) | Grep ile hedefle |

### §6.2 Bağlam hijyeni

| # | Kural | Neden |
|---|---|---|
| 1 | Oturum başında yalnız `index.md` oku, konuya göre `keys.md`'den git | 14 kök dosya okumak bütçeyi 1 turda bitirir |
| 2 | Bulunan bilgiyi **yola geri yaz** (rapor/`log.md`), oturum belleğine değil | SSOT |
| 3 | Aynı bilgi 2. kez okunuyorsa → yanlış yere yazılıyor demektir | Tekrar = israf |
| 4 | Uzun dosyada `grep` ile hedefle, dosyanın tamamını yükleme | Bütçe |
| 5 | Çıktı uzunsa **önce cevap**, ayrıntı sonraki tura | ADHD çıktı kuralı ([ROLE.md](ROLE.md) §2) |

### §6.3 Bütçe ihlal belirtileri

| Belirti | Kök neden | Düzeltme |
|---|---|---|
| Aynı soruyu 2. kez soruyorsan | Bağlam kaybı | `log.md` / rapora yaz |
| Cevaplar gittikçe kısalıyor, madde patlıyor | Bütçe taşması | 2 tur'a böl |
| "Hangi dosyadaydık?" sorusu | Okuma sırası kopmuş | `keys.md` §2'ye dön |
| Eski bilgiyle yeni karar | Stale context | İlgili dosyayı yeniden oku (§6.2.5) |

---

## §7 Ajan Oturum Adlandırma Kuralları

### §7.1 Biçim

```
<rol>-<konusu>-<kisaltma>
   |        |         |
   |        |         +-- 2-4 kelime, kucuk harf, tire (max 32 karakter)
   |        +------------ 1-4 kelime, kucuk harf, tire
   +--------------------- architect | developer | reviewer | tester | ui-designer
```

**Geçerli örnekler:**

| Oturum adı | Anlam |
|---|---|
| `architect-katman-sozlesmesi` | Katman bağımlılık sözleşmesi yazımı |
| `developer-session-repo-dapper` | `SessionRepository` Dapper implementasyonu |
| `reviewer-g2-sql-taramasi` | EF Core / string-concat taraması |
| `tester-sqlite-entegrasyon` | SQLite entegrasyon testleri |
| `ui-designer-02-main-ide-wireframe` | 02-main-ide ASCII wireframe |
| `architect-adr-006-mcp` | ADR-006 MCP kararı |

**Geçersiz örnekler:**

| Hatalı ad | Sorun | Doğrusu |
|---|---|---|
| `test` / `yeni` / `deneme` | Rol + konu yok | `tester-<konu>` |
| `Architect-Üretim` | Büyük harf + Türkçe karakter | `architect-uretim` |
| `architect-katman-sozlesmesi-ve-tum-mimari-degisiklikleri-ve-adr` | 64+ karakter | `architect-katman-sozlesmesi` |
| `session-1` | Anlam yok, sıralanamaz | Rol önekli ad |

### §7.2 Adlandırma kuralları

| # | Kural | Yasak |
|---|---|---|
| 1 | Her ad **rol öneki** ile başlar | Rol'süz oturum |
| 2 | Küçük harf + ASCII + tire | Boşluk, Türkçe karakter, alt çizgi |
| 3 | Uzunluk ≤ 48 karakter | Uzun cümle adları |
| 4 | Aynı iş devam ediyorsa ad **aynı kalır** | Her turda yeni ad |
| 5 | İşi biten oturum adı `log.md`'ye yazılır | Ad kaybı |
| 6 | Paralel subagent'lar **farklı** rol öneki taşır | 2 tane `developer-*` aynı anda |

> **KURAL:** Ad, **işin** kimliğini taşır; oturum kimliği değil. Aynı iş `architect`'te kalmışsa, kod yazımına geçerken ad `developer-` önekinin altına **yeni oturum** olarak taşınır (devre paketi ile).

---

## §8 Hata / Escalation Yolları

### §8.1 Üç başarısız düzeltme kuralı (3-Strike)

```
Ayni hatayi duzeltme denemesi #1  ->  hala kirmizi   ->  tekrar dene
Ayni hatayi duzeltme denemesi #2  ->  hala kirmizi   ->  tekrar dene
Ayni hatayi duzeltme denemesi #3  ->  hala kirmizi   ->  DUR  (kod yazma yasak)
                                          |
                                          v
                        SUPLI VARSAYIMI ADLANDIR + rapor et + bekle
```

**3-Strike sonrası zorunlu çıktı şablonu:**

```markdown
## DURUM: 3 basarisiz deneme - kod yazmayi biraktim

- HATA        : <konum> : <neden> (kopyalanmis hata metni, 1-3 satir)
- DENEMELER   : 1) <ne denendi>  2) <ne denendi>  3) <ne denendi>
- SUPHLI VARSAYIM : <hangi varsayimin yanlis olabilecegini tek cumlede ADLANDIR>
- KANIT       : <build/test ciktisindan alinti>
- SECENEKLER  : A) <oneri>   B) <oneri>   C) <oneri>
- SORU        : <kullaniciya tek kisa soru>
```

> **UYARI:** "Supheli varsayım" **adlandirilmadan** rapor geçersizdir. "Bilmiyorum" değil; **hangi varsayımın yanlış olabileceği** yazılır. Örn: *"Varsayım: net48 bacağında DevExpress v26.1 `TileControl` desteğini varsayıyorum — doğrulanmadı (NEEDS_VALIDATION)."*

### §8.2 Escalation merdiveni (ASCII)

```
SEVIYE 0  ajan kendi yetki alani icinde dener            (max 3 deneme)
   |  basarisiz
   v
SEVIYE 1  ait oldugu rolun ust yetkilisine git
          developer -> architect   |   tester -> reviewer
   |  yine cozulmediyse
   v
SEVIYE 2  CROSS-ROL: ilgili X sahibi ile ortak inceleme
          (orn. build hatasi: developer + architect)
   |  yine cozulmediyse
   v
SEVIYE 3  KULLANICIYA ESCALATION
          - 3 basarisiz deneme raporu (ustteki sablon)
          - ya da DECISION BLOCKED (karar bende degil)
          - ya da G8/G7 (teknoloji degisikligi / yikici islem)
   |
   v
SEVIYE 4  ISI DONDUR  ->  WORKFLOW.md 9 (bloke protokolu)
          ilgili gate'e geri don, kapsam kucult, yeniden planla
```

### §8.3 Ne zaman hangi seviye

| Tetikleyici | Seviye | Etiket |
|---|---|---|
| Aynı hata 3 düzeltmede çözülmedi | **3** | `3-STRIKE` |
| Kaynak çelişti ve hiyerarşi çözmedi | **3** | `DECISION BLOCKED` |
| Yeni paket/sürüm/ORM isteği | **3** | `G8` (kullanıcı onayı) |
| Silme / overwrite / `git reset` | **3** | `G7` (onay) |
| Gate-onayı gereken adım | **0 → dur** | `GATE-OPEN` |
| Mimari sözleşmeye aykırı istek | **1** | `architect` |
| Güvenlik açığı bulgusu | **1 → 3** | `CRITICAL` |
| Bilgi yok | **0** | `UNKNOWN` (uydurma yasak) |

### §8.4 Bloke durumda yapılmayanlar

| # | Yasak | Neden |
|---|---|---|
| 1 | Kör farklı bir çözüm denemeye devam etmek | Zaman/yapı israfı, gizli kırık bırakır |
| 2 | Hatayı "geçici yamayla" kapatmak | Sonraki part'te patlar (R3/R5) |
| 3 | Kanıtsız "bu sefer oldu" demek | G5 ihlali |
| 4 | Kullanıcıya soru sormadan kapsam değiştirmek | Kapsam yetkisi kullanıcıda (§3.2 #11) |
| 5 | Blokeyi raporlamadan unutmak | `log.md` kaydı zorunlu |

---

## §9 Ajan başına Definition of Done (DoD)

> **KURAL:** Bir ajan "bitti" diyebilmek için kendi DoD'unun **tamamını** işaretlemelidir. Eksik madde = iş bitmedi = devre geçersiz.

### §9.1 architect — DoD

- [ ] ADR `.decisions/accepted/ADR-NNN-<konu>.md` altında ve `status: accepted`
- [ ] `hedef_bacak: "net48 + net10.0-windows"` alanı dolu
- [ ] Alternatif ≥ 2 yazılmış, sonuçların (+) ve (−) tarafı var
- [ ] Katman bağımlılık kuralı ASCII diyagramda görünür (saf ASCII)
- [ ] İlgili `.rules/` dosyası bu kararı **zorunlu** kılıyor
- [ ] `log.md`'ye tarihli kayıt düşüldü
- [ ] İlgili gate'e ([WORKFLOW.md](WORKFLOW.md) §3) bağlantı verildi
- [ ] Devre paketi 4 maddelik ve `developer`'a iletildi

### §9.2 developer — DoD

- [ ] Kod ADR + katman sözleşmesine uygun (içe bağımlılık yalnız `interface`)
- [ ] Her karar iki bacakta değerlendirildi (`net48` + `net10.0-windows`); `#if` yalnız platform farkı
- [ ] EF Core **0** referans; SQL yalnız parameterized (`CommandTimeout` setli)
- [ ] DevExpress **yalnız v26.1**, ribbon **yalnız `RibbonControlStyle.Office2010`**
- [ ] `dotnet build` → **0 hata** (her iki bacak)
- [ ] Kendi gözden geçirmesi yapıldı (dosya adı, `#region` yok, `null` yolu)
- [ ] Devre paketi 4 madde + build çıktısı ile `reviewer`'a iletildi

### §9.3 reviewer — DoD

- [ ] Her bulgu `SEVIYE · dosya:satır · neden · düzeltme` biçiminde
- [ ] Kanıt satırı var (build/test çıktısı ya da `UNKNOWN`)
- [ ] G1..G12 maddeleri tek tek kontrol edildi
- [ ] İki bacak (`net48` / `net10`) birlikte incelendi
- [ ] Karar açık: **PASS** ya da **RED** (belirsiz PASS yok)
- [ ] RED ise `developer`'a 4 maddeyle geri döndü
- [ ] Sonuç `.ai/reports/**` altına yazıldı

### §9.4 tester — DoD

- [ ] Birim testler I/O'suz ve deterministik (seed'li, zaman bağımsız)
- [ ] Entegrasyon testleri gerçek `schema` üzerinde (SQLite)
- [ ] NetArchTest: katman yönü + circular dependency **0** ihlal
- [ ] `dotnet test` → tümü yeşil, sayılar raporda (geçen/başarısız/süre)
- [ ] Coverage kanıtı var ya da kapsam dışı gerekçesi yazılı
- [ ] Her iki bacakta da test çalışıyor
- [ ] `reviewer` son doğrulamasından geçti

### §9.5 ui-designer — DoD

- [ ] `ui-design/NN-<ekran>.md` frontmatter'lı, **ASCII** wireframe (Unicode kutu yok)
- [ ] Kontrol listesi DevExpress **v26.1** dışına çıkmıyor
- [ ] `RibbonForm` + `RibbonControlStyle.Office2010` korunuyor
- [ ] **GATE-003 kullanıcı onayı alındı** (aksi halde DoD açık kalır)
- [ ] `.Form.cs` + `.Designer.cs` + `.resx` üçlüsü eksiksiz ve tutarlı
- [ ] `ResourceManager` adları bozulmadı
- [ ] Devre paketi `developer`'a iletildi

### §9.6 Ortak DoD (tüm ajanlar)

| # | Madde | Kanıt |
|---|---|---|
| 1 | Frontmatter 8 alan + `authority: SSOT` | Dosyanın ilk 10 satırı |
| 2 | Bozuk iç link yok | link taraması = **0** |
| 3 | Kanıtsız "bitti" yok | build/test çıktısı |
| 4 | `UNKNOWN` / `NEEDS_VALIDATION` işaretli | ilgili satır |
| 5 | Yıkıcı işlem öncesi onay alındı | onay kaydı |
| 6 | Rapor `(n/8)` biçiminde, ≤ 5 madde, ≈ zaman tahmini | rapor başlığı |

---

## §10 SSS (FAQ — 10 Soru)

### S1 — `.agents/AGENTS.md` ile `AGENTS.md` (bu dosya) arasındaki fark ne?

`.agents/AGENTS.md` **rol kaydıdır**: hangi ajan hangi dosyayı yazar/onaylar, hangi görev tanımına sahip. Bu dosya **vault protokolüdür**: devre paketi biçimi, subagent eşiği, context bütçesi, oturum adı, escalation, DoD, SSS. Çelişirse `.agents/` kazanır.

### S2 — Aynı işi hem `architect` hem `developer` yapabilir mi?

Hayır. Karar `architect`te, uygulama `developer`'dadır. `architect` kod yazarsa review yükü tek elde toplanır (SRP ihlali). İstisna: **spike/teknoloji doğrulaması** — kısa, `log.md`'ye not düşülür ve kod main'e geçmeden `developer`'a devredilir.

### S3 — `reviewer` ile `tester` arasındaki fark ne?

`reviewer` **kodu okur**, statik ve kural uyumunu ölçer, PASS/RED verir. `tester` **davranışı çalıştırır**, test ve coverage üretir. `reviewer` test yazmaz, `tester` PASS/RED vermez. Sıra: `developer` → `reviewer` (PASS) → `tester` → rapor.

### S4 — Devre paketini yazmazsam ne olur?

Alan ajan işe **başlamaz**; paket eksikse rolü geri çevirir. Pratikte bu, tur kaybı ve bağlam yeniden kurmadır. Paket ≤ 2 KB tutun; uzun kanıt dosyaya, özet pakete.

### S5 — Ne zaman subagent açmalıyım?

Beş tetikleyiciden biri varsa: (a) ≥ 8 dosya okunacak, (b) 2+ ajan bağlamı, (c) başkasının X alanı, (d) paralel yürüyebilir, (e) uzun ölçüm/okuma. Hiçbiri yoksa **inline** çalış — subagent kurma maliyeti işten büyükse YAGNI.

### S6 — Subagent gate geçebilir mi?

Hayır. GATE-001..006 onayları **kullanıcıya** aittir; subagent en fazla **hazırlar ve raporlar**. GATE-003 (UI onayı) öncesi subagent kod yazdıramaz (G12).

### S7 — 3 başarısız düzeltme kuralı tam olarak neyi durduruyor?

**Aynı hataya** yönelik düzeltme girişimlerini durdurur. Yeni bir hipotez = yeni deneme sayılır ama o da **adlandırılmış** olmalı. Kuralın amacı: sonsuz döngü yerine **şüpheli varsayımın adıyla** escalation.

### S8 — Context bütçesi aşılırsa ne olur?

Cevap kalitesi düşer, tekrar başlar. Aşım belirtisi görürsen işi **2 tur'a böl**, uzun içeriği **yola geri yaz** (rapor/`log.md`), dosyayı **grep ile hedefle**. Subagent açılış bağlamı > 8 KB ise kesit gönder.

### S9 — Ajan adı neden önemli?

Paralel subagent'ları ayırt eder, `log.md`'de işin izini sürer, escalation raporunun hangi işten geldiğini gösterir. Ad = iş kimliği, oturum kimliği değil (§7.2).

### S10 — Kim gate onayı verir, kim teknik karar?

Gate onayı (GATE-001..006) **kullanıcıya** aittir. Teknik kararlar §3.2'deki X sahiplerinde. Yeni teknoloji/sürüm/ORM isteği her koşulda **kullanıcı onayı** ister (G8).

---

## §11 Sık Yapılan Hatalar (Red Team)

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| 1 | Devre paketini 4 madde olmadan göndermek | Alan rol soru geri yollar, tur kaybı | §4.2 şablonunu doldur |
| 2 | Kanıtsız "bitti" (G5) | Sonraki part'te patlayan hata | Build/test çıktısını pakete ekle |
| 3 | X alanına girmek (yetki aşımı) | `developer` `.resx`'i ellemiş, form kırık | §3.2 + `.agents/AGENTS.md` §3 |
| 4 | 3 denemeden sonra denemeye devam | Aynı hata 8. kez | §8.1 `3-STRIKE` raporu |
| 5 | Subagent'a tüm bağlamı göndermek | İlk yanıt 30+ sn, özet kaybı | §5.3 ≤ 8 KB kesit |
| 6 | Onaysız kodlamaya başlamak (G12) | Wireframe değişince kod çöp | GATE-003 HARD STOP |
| 7 | Rol öneksiz oturum adı | `log.md`'de iz bulunamıyor | §7.1 biçimi |
| 8 | `UNKNOWN` yerine tahmin yazmak | Uydurma API/dosya | G9 etiketleri |
| 9 | Aynı bilgiyi 2 dosyaya yazmak | SSOT ihlali, çelişen kural | Tek kaynak, diğeri link |
| 10 | Escalation'u gizlemek | Kullanıcı sorunu sonradan öğrenir | §8.3 tablosuna göre etiketle |
| 11 | Ajanı "sohbet tonu" sanmak | Cevaplar madde/bütçe/kuralsız | [ROLE.md](ROLE.md) §2 |
| 12 | `reviewer`'ın kendi bulgusunu kapatması | Aynı hata tekrar | 2. inceleme `architect` |

---

## §12 Doğrulama

- [ ] Frontmatter 8 alan, `authority: SSOT`
- [ ] 5 ajanın tamamı §2'de, her biri özet + yetki + devre üçlüsünde
- [ ] §3.2 karar yetkisi tablosunda her satırda tek bir **X** var
- [ ] §4.2 devre paketi **tam 4 madde**
- [ ] §5 karar ağacı saf ASCII (Unicode kutu çizim yok)
- [ ] §7.1 oturum adı biçimi örnekli (geçerli + geçersiz)
- [ ] §8.1 `3-STRIKE` + şüpheli varsayım şablonu mevcut
- [ ] §9'da 5 ajanın DoD'u + ortak DoD var
- [ ] §10 = 10 SSS, §11 = 12 hata satırı
- [ ] Bu dosyadaki tüm markdown linkleri diskte mevcut
- [ ] `.agents/AGENTS.md` bu dosyaya yalnızca **protokol** olarak bağlanıyor, tekrar yok

---

## §13 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Ajan kaydı (rol yetki matrisi SSOT) | `.ai/.agents/AGENTS.md` |
| 2 | Ajan görev tanımları (5 dosya) | `.ai/.agents/{architect,developer,reviewer,tester,ui-designer}.md` |
| 3 | Ajan şablonu | `.ai/.templates/agents/agents-template.md` |
| 4 | Sub-agent şablonu | `.ai/.templates/agents/subagent-template.md` |
| 5 | Süreç + gate'ler | `.ai/WORKFLOW.md` |
| 6 | Mutlak rol + ses kuralları | `.ai/ROLE.md` |
| 7 | Anayasa (G1..G12) | `.ai/CLAUDE.md` |
| 8 | Anahtar kelime haritası | `.ai/keys.md` |
| 9 | Kapsam analizi (kararlar, riskler) | `.ai/reports/00-kapsam-analizi.md` |
| 10 | Kök belge şablonu | `.ai/.templates/documentation/root-md-template.md` |
