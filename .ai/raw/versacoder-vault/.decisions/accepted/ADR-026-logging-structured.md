---
title: "ADR-026 — Structured Log: Seviye, Sablon ve İki Bacak"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Log satirinin bicimi: 5 seviye, tek satirlik structured sablon, net48 + net10.0-windows iki bacakta ayni sozlesme"
hedef_bacak: "net48 + net10.0-windows"
evidence: "CLAUDE.md G5 (kanit) · ADR-001 (iki bacak) · ADR-009 (yurutme denetimi) · ADR-018 (secret maskeleme)"
---

# ADR-026 — Structured Log: Seviye, Şablon ve İki Bacak

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [keys](../../keys.md) · [ADR-018](ADR-018-secret-store.md) · [ADR-001](ADR-001-dual-target-net48-net10.md)

> **Tek cümlelik karar:** Tüm loglar **tek bir structured şablonla**, **5 seviyeden** biriyle ve **her iki bacakta (`net48` + `net10.0-windows`) aynı alan adlarıyla** yazılır — serbest metin logu ve bacak başına farklı format yasaktır.

---

## §0 Özet (Answer First)

Log, kanıtın **kalıcı** hâlidir (G5): build/test çıktısı kaybolur, log kalır. Bu ADR üç şeyi kilitler: **(1)** beş seviye `TRACE|DEBUG|INFO|WARN|ERROR`, **(2)** tek satır `ts · level · ctx · evt · msg · data` şablonu, **(3)** iki bacakta **aynı alan adları ve sıralama**. `#if` yalnız **sağlayıcı** ucu için kullanılır, **şablon ortaktır**. `ERROR` her zaman `ctx` + stack taşır ve rapora giren **tek** log sınıfıdır.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Proje iki bacakta derlenir: `net48` + `net10.0-windows` | [ADR-001](ADR-001-dual-target-net48-net10.md) |
| C2 | Kanıtsız "bitti" yasak; log = kanıt arşivi | `CLAUDE.md` G5 |
| C3 | Yürütme denetimi her araç çağrısında 1 satır log ister | [ADR-009](ADR-009-tool-execution-sandbox.md) D7 |
| C4 | Secret log'a asla düz yazılmaz | [ADR-018](ADR-018-secret-store.md) D5 |
| C5 | Serbest metin logu → aranamaz, birleştirilemez | kod incelemesi (G5 belirtisi) |
| C6 | `#if` yalnız platform farkında; iş mantığı ortak | `.agents/developer.md` §3.5 |

**Sorun:** Her bacak kendi logger'ını kullanınca **alan adları** ayrışır (`time` vs `ts`); log birleşimi (merge) yapılamaz, `ERROR` satırı aranamaz, iki bacak karşılaştırması imkânsızlaşır.

---

## §2 Karar

**Karar:** Tek structured şablon + 5 seviye + iki bacakta aynı sözleşmedir; sağlayıcı `#if` içinde, şablon ortaktır — karar verildi.

### §2.1 Seviyeler

| Seviye | Ne zaman | Zorunlu alan | Üretime çıkar mı? |
|---|---|---|---|
| `TRACE` | Tek çağrı içi izleme | `evt` | Hayır |
| `DEBUG` | Geliştirme kararı izi | `evt` + `data` | Hayır |
| `INFO` | İş yaşam döngüsü (başladı/bitti) | `evt` + `durum` | Evet |
| `WARN` | Düşmedi ama indirgenebilir anormallik (retry, fallback) | `evt` + `neden` | Evet |
| `ERROR` | İşlem başarısız; kullanıcıya dönen hata | `evt` + `hata` + `ctx` (+ stack) | Evet |

### §2.2 Şablon (tek satır, alan sırası sabit)

```text
ts=<ISO8601> level=<L> ctx=<modul/mag> evt=<eylem> msg=<tek cumle> data=<k=v;...>
```

```text
ORNEK:
ts=2026-09-25T10:41:07.412Z level=INFO  ctx=data/session evt=query.done msg="session listesi donduruldu" data=n=42;ms=12
ts=2026-09-25T10:41:09.005Z level=ERROR ctx=data/session evt=query.fail msg="sorgu basarisiz" data=hata=SqlException;kod=-2
```

| # | Kural | Ayrıntı |
|---|---|---|
| 1 | **Alan sırası değişmez** | `ts level ctx evt msg data` — değiştirilen birleştirme bozar |
| 2 | `ts` her zaman UTC + milisaniye | `DateTimeOffset.UtcNow.ToString("o")` |
| 3 | `data` `k=v;` ile ayrılır, **serbest metin yok** | değerlerde `;` `=` kaçışlanır |
| 4 | `msg` tek cümle, nokta yok | `data` ayrıntıyı taşır |
| 5 | Secret değeri `data`'ya **yazılmaz** | yalnız `ad=maske` (ADR-018) |

### §2.3 İki bacak

| Bacak | Sağlayıcı | `#if` kapsamı | Şablon |
|---|---|---|---|
| `net48` | kayıtlı logger (ör. dosya/sink) | yalnız kayıt uç noktası | **aynı** |
| `net10.0-windows` | `Microsoft.Extensions.Logging` (console/file) | yalnız kayıt uç noktası | **aynı** |

```
            +--------------------------------------+
            |        ORTAK KATMAN (LOG SABLONU)    |
            |  ts level ctx evt msg data  + bicim  |
            +------------------+-------------------+
                               |
            +------------------+-------------------+
            |            #if  (yalniz uc nokta)    |
            +------------------+-------------------+
                               |
        +----------------------+----------------------+
        | net48                |      net10.0-windows |
        | sink : dosya/olay     |      sink : M.E.Logging
        +-----------------------+----------------------+
```

**Neden:** Ortak şablon, aynı assert'in iki bacakta da geçmesini sağlar (`ts` hariç alanlar sabit). Sağlayıcı `#if` içinde kaldığı için G6 ihlal edilmez.

---

## §3 Alternatifler

| # | Alternatif | Tanım | Neden seçilmedi |
|---|---|---|---|
| A1 | **(SEÇİLDİ)** Elle yazılmış structured satır + ortak şablon | Sablon ortak, sağlayıcı `#if` | — |
| A2 | Serbest metin logu | `Log.Info("...")`, biçim yok | C5: aranamaz, birleştirilemez, G5 kanıtı zayıf |
| A3 | Tam JSON satırı | Her satır bir JSON nesnesi | `net48`'de ek bağımlılık (serializer) + okunabilirlik düşer; C6 gereksiz `#if` doğurur |
| A4 | Kütüphane loglaması (ör. 3. parti) | Şablonu paket belirler | Sürüm kilidi G8'e düşer (kullanıcı onayı); şablon bizim değil |

**Ret gerekçesi (A2–A4):** A2 kanıt üretmez. A3, ortak şablonu serielleştiriciye bağlar ve `net48` bacağını şişirir. A4 teknoloji kararı olduğu için **kullanıcı onayı (G8)** ister; o onay alınmadan vault'a yazılamaz.

---

## §4 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| İki bacak logu tekleştirilebilir | alan adları + sırası birebir aynı |
| `ERROR` araması deterministik | `level=ERROR` + `evt=` filtresi |
| Kanıt arşivi (G5) | log satırı rapora eklenebilir |
| `#if` yığını sınırlı kalır | yalnız sağlayıcı ucu `#if` içinde |
| Secret sızıntısı log'da kapatılır | ADR-018 maskesi + `data` kuralı |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Elle şablon = kayan string | Şablon **tek** yardımcı sınıfta; test assert'i şablonu kilitler |
| `net48`'de ISO-8601 + UTC davranışı farklılığı | `DateTimeOffset.UtcNow` kullan, `DateTime.Now` **asla** |
| Performans: log string birleştirme | `TRACE`/`DEBUG` üretimde kapalı; seviye kontrolü concatenation'dan önce |
| `data` kaçışı ihmal edilirse parse kırılır | `;`/`=` kaçış birim testi (`tester`) |

**NEEDS_VALIDATION:** `net48` bacağında kullanılacak logger sağlayıcısı **seçilmedi** (bağımlılık G8 gerektirir) → kullanıcı onayına sunulur; onaya kadar şablon + seviyeler bağlayıcıdır, sağlayıcı değil.

---

## §5 Kabul Kriterleri

- [ ] Log yardımcı sınıfı ortak projede; `#if` yalnız sağlayıcı ucu
- [ ] 5 seviye enum olarak tanımlı
- [ ] Şablon assert'i ile alan sırası kilitli (test yeşil)
- [ ] `level=ERROR` her zaman `ctx` + `hata` içeriyor (test)
- [ ] Secret taraması: log çıktılarında `sk-***`/parola **0** (ADR-018)

---

## §6 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-001](ADR-001-dual-target-net48-net10.md) | dayanak | iki bacak ve ortak kaynak kuralı |
| [ADR-009](ADR-009-tool-execution-sandbox.md) | uygulanır | her araç yürütmesinin 1 satır denetim logu |
| [ADR-018](ADR-018-secret-store.md) | uygulanır | `data` alanındaki maskeleme kuralı |
| [ADR-025](ADR-025-gate003-ui-hard-stop.md) | related | olay yönlendirici log çıktısının biçimi |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `CLAUDE.md` G5/G6.
