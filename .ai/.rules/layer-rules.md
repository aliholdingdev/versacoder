---
title: "VersaCoder — Katman Kurallari (Tek Hedef 5000+, Yon, Katalog)"
type: rules
category: rules
status: active
version: 2.1.0
updated: 2026-10-04
authority: SSOT
scope: "VersaCoder katmanli mimari kurallari: 3 eksen (tip/dugum/yon grubu) TEK hedefe bagli (5000+ tip + 5000+ dugum + 5000+ uretim .dll), yon grubunun 1 = 1 fiziksel .dll tanimi ve 4 adimli bolme kurali, olculmus 8x8 baslangic matrisi, katalog disiplini (ID/8 sutun/durum) ve iki sayacin ayri raporlanmasi; NetArchTest + count-components.ps1 + gen-layer-5000.ps1 ile denetim"
---

# VersaCoder — Katman Kurallari (`.rules/layer-rules.md`)

**Zorunlu Baglantilar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [solid-rules.md](./solid-rules.md) · [coding-standards.md](./coding-standards.md) · [data-access-rules.md](./data-access-rules.md) · [dependency-map](../architecture/00-overview/dependency-map.md) · [layering-5000-spec](../spec/layering-5000-spec.md) · [layer-5000-check](../checklists/layer-5000-check.md) · [ADR-013](../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) · [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-030](../.decisions/accepted/ADR-030-5000-layers-class-level.md)

> **Tek cumlelik sorumluluk:** *Bir tip hangi katmana yazilir, hangi kenarlar aciktir, hangi kademedeyiz, 5000+ nasil kanitlanir ve ihlalde kim RED verir* sorusunun tek cevabi.
> Bu dosya karar **gerekcesini** degil, kural **metnini** tasir (SRP): gerekce [ADR-013](../.decisions/accepted/ADR-013-layered-architecture-52-nodes.md) / [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) / [ADR-030](../.decisions/accepted/ADR-030-5000-layers-class-level.md) icindedir.

---

## §0 Ozet (Answer First)

**UC farkli sayi, UC farkli sey — karistirilmaz; hepsi ayni hedefe gider (`>= 5000`):**

| Terim | Ne sayar | Bugun (2026-10-02) | Hedef | Eksik | Kaynak |
|---|---|---|---|---|---|
| **Katman (tip)** | tip kaydi (katalog satiri) | **5440** | >= 5000 | **0 (ASILDI)** | `count-components.ps1` |
| **Dugum** | namespace/klasor (katalog dosyasi `N<NN>-*.md`) | **44 alt** (+8 makro = 52 dosya) | >= 5000 | **4956** | `count-components.ps1` "Kayit dosyasi" |
| **Yon grubu** | **fiziksel uretim `.dll` = `.csproj` = matris satiri** | **9** | >= 5000 | **4991** | `src/**/*.csproj` sayimi |

**ARA KADEME YOKTUR** (100 / 500 / 1000 ve `8·12·16·20` merdiveni **iptal**, 2026-10-02 kullanici karari, G7). Olculen her ara sayi artik "eksik kalmistir" diye yazilir; **ust kademe diye adlandirilmaz** (G5).

**Bugun 2 ekseni acik (KADeME DRIFT):**

| Eksen | Bugun | Hedef | Durum |
|---|---|---|---|
| Tip (K) | 5440 | 5000 | **ASILDI** (marj 440) |
| Dugum (N) | 44 alt dugum | 5000 | **EKSIK 4956** |
| Yon grubu (Y) | 9 uretim `.dll` (+2 test = 11 toplam) | 5000 | **EKSIK 4991** |

> **P (paket) ekseni Y'ye merge** edildi: ikisi de `.csproj` sayar. Eski `9 + N + 2` formulu (11/16/23/40) **iptal** (ADR-030 v2.1.0 §2.4).

**Topoloji (ADR-030 §2.4):** uretim `.csproj` sayisi = yon grubu sayisi; **bugun 9, hedef 5000+**; test projeleri (2) **ayri** sayilir ve matris satiri **degildir**.
**Dugum = namespace/klasor** (kullanici karari 2026-10-02): dugum artinca yogunluk duser — 44 dugumde 5440/44 = **124 kayit/dugum**, 5000 dugumde beklenen **1.1**. Seyrelme **bilinçli kabul** edilmistir; yogunluk esigi **konmaz**, raporlanir (G5).

**Drift kapanisi:** yalnizca TODOS/06 Faz K0 + A..D (ve sonrasi) ile kapanir; kapanmadan "hazir" denemez (G5).

---

## §1 Kapsam

**Icinde:** 12 kural (LAY-01..LAY-12) — yon, kapali kenar, kademe, topoloji, katalog kaydi, cift TFM, otomatik denetim.

**Disinda:** SOLID ilke anlatimi ([solid-rules.md](./solid-rules.md)) · tipin **ne is yaptigi** (ilgili katman spec'i) · gate sirasi ([../WORKFLOW.md](../WORKFLOW.md)) · veri erisimi ([data-access-rules.md](./data-access-rules.md) — **EF Core yasak**, bu dosya tekrarlamaz) · UI wireframe sahipligi ([ui-rules.md](./ui-rules.md)) · test piramidi ([testing-rules.md](./testing-rules.md)).

> **KURAL:** Katman siniri **test edilen** bir kuraldir: NetArchTest yoksa "yon dogru" iddiasi kanitsizdir (G5).
> **YASAK:** Yeni `.csproj` acarak katman "cozmek" · katalog satirini kod yazilmadan `yazildi` yapmak · esik 5000'i marj gerekcesiyle dusurmek · iki sayaci birlestirip tek "5000+" kaniti cikarmak.

### §1.1 Yon Matrisi (8x8 — tek kaynak: `architecture/00-overview/dependency-map.md`)

```text
 satir \ sutun |  l0   l1   l2   l3   l4   l5   l6   l7
 --------------+-----------------------------------------
  l0           |   -    -    -    -    -    -    -    -
  l1           |  OK   OK   OK   X    OK   OK   OK   OK
  l2           |  OK   OK   -    X    X    X    X    X
  l3           |  OK   X    X    -    X    X    X    X
  l4           |  OK   OK   X    X    -    X    X    X
  l5           |  OK   OK   OK   X    X    -    X    X
  l6           |  OK   OK   OK   X    OK   OK   -    X
  l7           |  OK   OK   OK   X    X    X   OK    -

  sutun ozeti  : l0=herkes · l1=l2,l4,l5,l6 · l2=l5,l6,l7
                 l3=HICBIRI · l4=l6 · l5=l6,l7 · l6=l7 · l7=HICBIRI
  toplam       : 64 hucre -> 20 OK / 40 X / 4 kenar (-)
```

- **Zincir (insa sirasi)** `l7 -> l6 -> l5 -> l4 -> l2 -> l1 -> l0` bir **siradir**, kenar degil; `l4 -> l2` bu yuzden `X` (dependency-map §2, madde 3).
- **Kapali 3 kenar:** `l5 -> l4`, `l4 -> l2`, `l2 -> l4` — hepsi `l0` portu uzerinden dolasir.
- **Bu matris boyutu hedefe dogru BUYUR:** hedef **5000 yon grubu** -> matris **5000 x 5000 = 25.000.000 hucre** (ADR-030 v2.1.0 §2.3).
  Ara kademe **yok** (100/500/1000 ve 8·12·16·20 **iptal**). Yeni satir/sutun **yalniz** §1.2'deki bolme kurali ile eklenir; keyfi `l99` yasaktir.
  Elle **25.000.000 hucre yazilmaz** — kural seti **uretilir**; uretim ornegi henuz **olculmemistir** (DOĞRULAMA GEREKLI, G9).
- **`l3` gelen oklari:** matriste `X`, ADR-028 sessiz -> **DOĞRULAMA GEREKLI**. Yeni ADR satirina kadar `X` yazilir.
- **Olculmus baslangic:** bugunku 8x8 (20 OK / 40 X) **olcum olarak kalir**; yeni gruplar bu deseni **bozamaz** (bozarsa NetArchTest RED).

### §1.2 Yon grubu = 1 fiziksel `.dll` (tek hedef 5000+) — ADR-030 §2.3

**Grup = 1 uretim `.dll` = 1 `.csproj` = matrisin 1 satiri.** (kullanici karari 2026-10-02, G7)
**Test projeleri yon grubu degildir** (matriste yalniz bagimli uc; toplam `.csproj` = 9 uretim + 2 test = 11).

**Yeni grup turetme kurali (4 adim, uydurulmaz — adlar `versacoder.old.*` klasorlerinden gelir):**

| # | Adim | Kural |
|---|---|---|
| 1 | Bolme esigi | Bir `.dll` icinde **6'dan fazla** sorumluluk (modul) birikirse **bolunmek zorundadir** |
| 2 | Ad kaynagi | Parca adi eski ağaçlarin **gercek klasor adlarindan** alinir (olculmus 36 klasor/ağac, G9) |
| 3 | Kayit | `architecture/layer-5000/yon-gruplari.md`'ye `l<N>` satiri yazilir **once** |
| 4 | Kapı | ancak ondan sonra `.csproj` acilir: `architect` + kullanici onayi + GATE-004 (LAY-06) |

**Bugunun olculmus gruplari (9):**

| Yon grubu | `.csproj` | Durum |
|---|---|---|
| `l0` Foundation | `VersaCoder.Abstractions` | VAR |
| `l1` Domain | `VersaCoder.Domain` | VAR |
| `l2` Application | `VersaCoder.Application` | VAR |
| `l3` CrossCutting | `VersaCoder.Crosscutting` | VAR |
| `l4` Infrastructure | `VersaCoder.Infrastructure` (semsiye) | VAR — 29 alt paket **planli** |
| `l5` Protocol | `VersaCoder.Protocol` | VAR |
| `l6` Host | `VersaCoder.Host` | VAR |
| `l7` UI-Desktop | `VersaCoder.Desktop` | VAR |
| `l8` UI-Cli | `VersaCoder.Cli` | VAR |

> **Olcum notu (G9):** eski 8x8 matriste `l7 UI` **2 `.dll` tasiyordu**; "1 grup = 1 `.dll`" kuraliyla `l7` **ikiye ayrildi** -> bugun **9 grup**. `dependency-map.md` hala **8x8** yaziyor -> **DOĞRULAMA GEREKLI**: 9x9'a cevrilmeden NetArchTest kurali gecerli degildir (§1.1).

**29 paket -> grup iliskisi:** 29 `Infrastructure.*` paketi **kendiliginden 29 grup degildir**; §1/1 kuralina gore bolunur (Faz A 5 · B 7 · C 7 · D 10 = 29 paket, acilan grup **adedi kural belirler**). Adet **olculmeden yazilmaz** (G5/G9): `yon-gruplari.md` §2.3 acik noktasi.

> **YASAK:** kademe merdiveni yazmak (`8 · 12 · 16 · 20` / `100 · 500 · 1000`) · bu dosyaya satir yazmadan `.csproj` acmak · ad uydurmak · test projesini yon grubu saymak.

### §1.3 Dugum ekseni (tek hedef 5000+)

| Dugum | Hedef | Bugun | Eksik | Katalog etkisi |
|---|---|---|---|---|
| alt dugum (`N01..N44` + yeni `N45..`) | **>= 5000** | **44 alt dugum** | **4956** | +4956 dosya |
| makro (`N45..N52` mevcut 8 makro) | bolme ile artar | **8 makro** | — | yeni makro = yeni yon grubu (§1.2) |
| toplam katalog dosyasi | — | **52** (44 alt + 8 makro) | — | + `node-inventory.md` = 53 |

- **ARA KADEME YOK:** 100 / 500 / 1000 basamaklari **kaldirildi**; arada olculen sayi "eksik" olarak yazilir (G5).
- **Dugum = namespace/klasor** (kullanici karari). Yeni dugum acmak `.csproj` **acmaz** (LAY-06); makro gruplar yon grubu adimindan (§1.2) gecer.
- **YoGunluk raporlanir, esiklenmez:** 44 dugumde 5440/44 = **124 kayit/dugum**; 5000 dugumde beklenen 5440/5000 = **1.1**. Seyrelme kabul edilmistir; buna karsi yogunluk esigi **koymak yasak** (G5: olcumsuz iddia).

---

## §2 Kurallar (LAY-01 .. LAY-12)

**Bicim (aaaaa.md §4.2):** `LAY-NN · bag · neye emreder · neyi yasaklar · ihlalini kim bulur · kaniti hangi arac`.

| ID | Bag | Neye emreder | Neyi yasaklar | Ihlalini bulan | Kaniti hangi arac |
|---|---|---|---|---|---|
| **LAY-01** | ADR-028 §2.3 · dependency-map §2 | Baglilik okunu **yalniz** dependency-map 8x8 matrisindeki `OK` hucreleri kadar ac | Matriste `X` olan herhangi bir `using` / proje referansi (20 OK disi her kenar) | `reviewer` / `architect` | NetArchTest `LayerRulesTests` (64 hucre) |
| **LAY-02** | ADR-028 §2.3 · ADR-030 §2.3 | `l5 -> l4`, `l4 -> l2`, `l2 -> l4` cagrisini **yalniz** `l0` interface'i uzerinden yap | Uc kapali kenardan birinin dogrudan cagrilmasi | `architect` | NetArchTest — ihlal dosyasi + satir no |
| **LAY-03** | ADR-030 §2.3 | `l3` (CrossCutting) icinde **yalniz** `l0` referansi bulundur | `l1`/`l2`/`l4` -> `l3` referansi; CrossCutting'in herhangi bir katmani kullanmasi | `architect` | NetArchTest — `l3` hedefli kural |
| **LAY-04** | ADR-030 §2.3 · layer-rules §1.2 | Yon grubunu **1 uretim `.dll` = 1 `.csproj` = 1 matris satiri** olarak kur; bir `.dll` **6'dan fazla** modul tasiyorsa **bol** ve adi `yon-gruplari.md`'ye **once** yaz | Kademe merdiveni yazmak (`8·12·16·20`, `100/500/1000`) · `yon-gruplari.md` satiri olmadan yeni `.csproj` · ad uydurmak · test projesini yon grubu saymak | `architect` | `yon-gruplari.md` satiri + `.csproj` sayimi + matris boyutu (hedef 5000x5000) |
| **LAY-05** | ADR-030 §2.4 | Topolojiyi **olcumle** kur: uretim `.csproj` sayisi = yon grubu sayisi (bugun **9**, hedef **5000+**); test projesi **ayri** sayilir | `9 + N + 2` gibi **iptal edilmis formullerle** iddia etmek (11/16/23/40) · test projesini uretime karistirmak | `architect` | `Get-ChildItem src -Recurse -Include *.csproj` + `tests` ayri -> `yon-gruplari.md` ile eslesme |
| **LAY-06** | ADR-030 §2.4 · ADR-028 3 kapi | Yeni `.csproj`'yi yalniz architect karari + kullanici onayi + GATE-004 ile ac; `yon-gruplari.md`'ye satir **ondan once** gelmis olmali | **Yeni tip icin** acilmis proje · satiri olmayan onaysiz `.csproj` | `architect` | dosya sayimi + `yon-gruplari.md` satiri + `git status` + `log.md` GATE kaydi |
| **LAY-07** | ADR-013 §1 · ADR-030 §2.1 | Her C# tipi icin `architecture/catalog/N<NN>-*.md` icinde **8 sutunlu** kayit ac: `ID \| Tip \| Tur \| Sorumluluk \| Proje \| TFM \| Test \| Durum` | Kayitsiz tip · eksik sutun · tekil olmayan tip adi | `reviewer` | `count-components.ps1` -> `Hatali format 0` |
| **LAY-08** | ADR-030 §2.1 | `durum` sutununu **3 degerden** biriyle yaz: `planli \| yazildi \| test-edildi` | `wip`, `done`, `bitti`, bos sutun gibi serbest metin | `reviewer` | sayac format kontrolu -> dis deger 0 |
| **LAY-09** | ADR-030 §2.1 | Hedef iddiasini **uc eksende** olculen sayla yaz: tip `>= 5000` **VE** dugum `>= 5000` **VE** uretim `.dll` `>= 5000` | Esik altinda "hedef asildi" iddiasi · esik dusurme (tip marji 440) · dugum ya da yon grubu eiginin yok sayilmasi · **ara kademe iddiasi** (G7 iptali) | `reviewer` | `count-components.ps1` -> `PASS` + `TOPLAM KAYIT` + `Kayit dosyasi` satirlari **VE** `.csproj` sayimi (**ucu birlikte**) |
| **LAY-10** | ADR-030 §2.4 | **Iki sayaci ayri ayri** raporla: tip kaydi (count-components) + namespace parcasi (gen-layer-5000) | Iki sayaci birlestirmek · tek kanit olarak 5834'u ya da 5440'i tek basina sunmak | `architect` | iki komut ciktisi **birlikte** (fark 394 yazilir) |
| **LAY-11** | ADR-001 · G6 | Her tipi **iki TFM'de** (`net48` + `net10.0-windows`) derle; `#if` yalniz platform farkinda | Tek TFM'li kayit · tek bacak build hatasi | `reviewer` | `dotnet build -f net48` + `-f net10.0-windows` -> ikisi 0 hata |
| **LAY-12** | ADR-030 §2.4 · SOL-06 | Denetimi **otomatik** calistir: yon = NetArchTest, kayit = iki sayac, kabul = layer-5000-check | "Dogru" iddiasinda komut cikti olmamasi; el inceleme ile gecis | `architect` | [layer-5000-check](../checklists/layer-5000-check.md) 14 madde + 3 sayac ciktisi |

> **YASAK:** `durum: yazildi` yazip kodu sonra "tamamlamak" (kanit zorunlu: sayac + 2 build + NetArchTest) — LAY-07/LAY-09 cift ihlalidir.
> **YASAK:** EF Core (G2 / [data-access-rules](./data-access-rules.md) / ADR-005) — bu dosya kapsam disi, ama katman ici her yerde gecerlidir.

### §2.1 Yeni tip ekleme akisi (spec VC-LAY-001 ile ayni 5 adim)

```text
tip lazim olur
    |
    +--> 1. hangi yon grubu?  architecture/layer-5000/yon-gruplari.md §2'den sec
    |                        (9 uretim .dll: l0 Foundation .. l8 UI-Cli)
    |                        orada yoksa §1 kurali ile bol ve satiri ORAYA yaz
    +--> 2. hangi dugum (N01..N44)?  -> architecture/catalog/N<NN>-*.md
    +--> 3. kayit satiri (8 sutun): ID | Tip | Tur | Sorumluluk | Proje |
    |                               TFM | Test | Durum
    +--> 4. kodu yaz (secilen yon grubunun `.dll` klasoru, cift TFM G6)
    +--> 5. olcu: count-components.ps1 -> PASS   +   dotnet build x2 -> 0 hata
                 +   gen-layer-5000.ps1 -> PASS   +   NetArchTest -> 0 ihlal
```

- **Adim 5'ten biri kirmizi ise tip `planli` kalir**; `planli -> yazildi` donusumu **kanitla** olur.
- **Proje acmak yasak** (LAY-06): 3 kapi yalniz gercek modul bolmesi ihtiyacinda acilir; adim 1'deki satir yoksa `.csproj` **acilmaz**.

### §2.2 Hedefe ilerleme — kademe yok, olcum var

| Eksen | Bugun | Hedef | Ne yapilir |
|---|---|---|---|
| Tip (K) | **5440** | 5000 | **ASILDI** — koddan once yazilan `planli` kayitlar `yazildi`'ya cevrilir (VC-LAY-009) |
| Dugum (N) | **44** | 5000 | +4956 katalog dosyasi (kod **degil**, katalog isidir) |
| Yon grubu (Y) | **9** | 5000 | +4991 `.csproj` — yalniz bolme kurali ile (§1.2), her biri GATE-004 + onay |

- **Ara kademe (SCALE-100/500/1000) YOKTUR** (G7 iptali): arada olculen sayi yalnizca **eksik** olarak yazilir; "ust kademe gecildi" cumlesi **yasaktir** (G5).
- Bir adimda uc eksenden **biri** bile geriye giderse ilerleme **sayilmaz**; eksik yazilir ve drift etiketi **dusmez**.

---

## §3 Ihlal ve RED Kosullari

| # | Ihlal | Kanit | Ne olur |
|---|---|---|---|
| 1 | Matris disi kenar (LAY-01/02/03) | `dotnet test --filter LayerRulesTests` -> basarisiz test | GATE-005 **RED**; isi ureten ajana doner |
| 2 | Onaysiz / `yon-gruplari.md` satiri olmayan `.csproj` (LAY-04/05/06) | dosya sayimi ile `yon-gruplari.md` satiri eslesmiyor ya da GATE kaydi yok | `DECISION BLOCKED` + G7; is geri alinir |
| 3 | Sayim < esik ya da hatali format > 0 (LAY-07/09) | `count-components.ps1` ciktisi | GATE-004a **RED**; TODOS/06 adim 2'ye don |
| 4 | Kodu olmayan `yazildi` satiri (LAY-07/08) | katalog-kod diff raporu | Satir `planli`'ya doner; `log.md`'ye `LAYER-5000-RED` |
| 5 | Tek bacak build hatasi (LAY-11) | `dotnet build -f net48` / `-f net10.0-windows` | G6 ihlali (HIGH); GATE-004 acilmaz |
| 6 | Tek kanitli "5000+" iddiasi (LAY-10) | tek sayac ciktisi var, ikincisi yok | Kutu RED (G5); iki cikti birlikte istenir |

**RED durumunda sirа:** soru no + kirmizi cikti `log.md`'ye etiketle -> isi **ureten ajana** geri dondur (baska ajana devretme, AGENTS §4) -> duzeltme ayni oturumda, kanit yeniden alinmadan madde `[x]` olmaz (G5).

---

## §4 Dogrulama

- [ ] Yon iddiasinda **matris** referansi var; dependency-map disinda kaynak yok (LAY-01)
- [ ] Yon grubu = **1 uretim `.dll`** ile eslesiyor (**9** @2026-10-02, hedef 5000, eksik **4991**); `yon-gruplari.md` satiri var (LAY-04)
- [ ] `LayerRulesTests` -> **0** ihlal (NetArchTest; matris boyutu ile eslesme)
- [ ] `count-components.ps1` -> **PASS** + `Hatali format 0` (tip >= 5000; **5440** @2026-10-02)
- [ ] Dugum sayisi hedefle karsilastirildi (**44 alt dugum** @2026-10-02, eksik **4956**) (LAY-09)
- [ ] `gen-layer-5000.ps1` -> **PASS** (**5834** @2026-10-02) — **ayri** raporlandi (LAY-10)
- [ ] Uretim `.csproj` sayisi = yon grubu sayisi (**9**; test **2** ayri); yenisi kapi gecti (LAY-05/06)
- [ ] Katalogda `durum` yalniz `planli | yazildi | test-edildi`; `yazildi` satirlari kodla eslesiyor
- [ ] Cift bacak build -> her ikisi 0 hata (G6) **+** `dotnet restore` -> 0 hata (ADR-030 §2.5/6, K6 acigi)
- [ ] `check-links.ps1` -> 0 bozuk link (bu dosyanin baglantilari dahil)

---

## §5 Referanslar

| # | Kaynak | Yol | Ne icin |
|---|---|---|---|
| 1 | Yon matrisi (8x8, 20 OK / 40 X — **olculmus baslangic**) | `architecture/00-overview/dependency-map.md` §2 | tek yon kaynagi |
| 2 | Tek hedef + topoloji (uretim `.csproj` = yon grubu) | `.decisions/accepted/ADR-030-*.md` §2.1-2.4 | LAY-04/05/09 |
| 3 | **Yon grubu tanimi + bolme kurali (9 -> 5000)** | `architecture/layer-5000/yon-gruplari.md` | LAY-04/06 |
| 4 | Katalog + sayim (iki sayac) | `spec/layering-5000-spec.md` · `scripts/count-components.ps1` · `scripts/gen-layer-5000.ps1` | LAY-07..LAY-10 |
| 5 | Kabul listesi (14 madde) | `checklists/layer-5000-check.md` | gate kaniti |
| 6 | 13 soruluk sinir denetimi | `checklists/layer-boundary-check.md` | mimari inceleme |
| 7 | Uygulama sirasi | `TODOS/06-5000-katman-kurulumu.md` | K0 + Faz A..D + kalan bosluk |

**Celiski notu (G10 — bu dosya cozmaz, kaydeder):** ADR-013 basligi "52 dugum" der; ADR-030 v2.1 "tek hedef 5000+ / 5440 tip kaydi" der. Iki sayi **farkli seviye**: 52 = dugum (namespace/klasor), 5440 = tip. Ayni seyi soylemiyorlar, **celiski degil**; yine de ADR-013 basligi "5000+ bilesen" ifadesini tasiyip kademe sozunu kullanmadigi icin `DOĞRULAMA GEREKLI` olarak kalir.

**Kalan acik celiskiler (bu dosya cozmaz, kaydeder):**
1. `dependency-map.md` **8x8** yaziyor; "1 grup = 1 `.dll`" kurali bugunu **9** yapar -> 9x9'a cevirme isi **acik** (yon-gruplari §2.1).
2. 5000x5000 matris icin **uretim kurali ornegi yok** -> DOĞRULAMA GEREKLI (ADR-030 §2.3).
3. 5000 `.csproj` restore olcumu yapilmadi (ADR-030 §3 K6, 575 csproj RED'i) -> GATE-004'te `dotnet restore` zorunlu.

**Degisiklik gunlugu:**
- 2026-10-02 — **v1.0.0** ilk yazim (10 kural, ADR-013/028/030 v1).
- 2026-10-02 — **v2.0.0** ADR-030 revizyonu ile birlikte: 10 -> **12 kural**, kademe/topoloji/iki sayac maddeleri (LAY-04/05/09/10) eklendi, her madde **ihlalini bulan ajan + kanit araci** sutunlarina tasinir (aaaaa.md §4.2 bicimi).
- 2026-10-02 — **v2.0.1** kullanici kararlari (G11): dugum ekseni (44 · 100 · 500 · 1000 · 5000) eklendi · yon grubu acilimi **8 · 12 · 16 · 20** (§1.2) + 29 paket-grup eslemesi · matris boyutu kademe ile 64/144/256/400 · LAY-04/09 bu kararlara baglandi · 3 ekseni drift tablosu §0'a alindi.
- 2026-10-02 — **v2.1.0** kullanici karari (G7): **tek hedef 5000+**, ara kademeler **silindi** · yon grubu = **1 fiziksel uretim `.dll`** · P ekseni Y'ye merge (4 -> 3 eksen) · §1.2 **4 adimli bolme kurali** (adlar eski ağaç klasorlerinden) + 9 olculmus grup · §1.3 tek hedef · §2.2 kademe gecisi -> "hedefe ilerleme" · `yon-gruplari.md` baglantisi eklendi · 3 acik celiski kaydi · `dotnet restore` kanit maddesi (K6).
