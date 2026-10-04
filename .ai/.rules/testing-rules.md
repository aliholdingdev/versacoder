---
title: "VersaCoder — Test Kuralları"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "VersaCoder test guardrail'ları — unit/integration ayrımı, katman test matrisi, unit testte EF/DevExpress yasağı, deterministik test, coverage hedefi, GATE-004/005 öncesi zorunlu testler, adlandırma, flaky politikası"
---

# VersaCoder — Test Kuralları (`.rules/testing-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [coding-standards.md](./coding-standards.md) · [data-access-rules.md](./data-access-rules.md) · [solid-rules.md](./solid-rules.md) · [../spec/test-strategy.md](../spec/test-strategy.md)

> **Tek cümlelik sorumluluk:** *Hangi test nerede, nasıl yazılır, neyi kanıtlamalı, kanıtsız "test geçti" nasıl geçersizdir* sorusunun tek cevabıdır.
> Bu dosya `CLAUDE.md` **G5 (kanıtsız tamamlama yasak)** guardrail'inin test derinliğidir; çelişirse `CLAUDE.md` kazanır.

---

## §1 Kapsam

**İçinde:** test yazım kuralları, katman matrisi, determinizm, coverage, gate öncesi zorunlu set, adlandırma, flaky politikası.

**Dışında:** piramit/fikstür/gate eşlemesinin geniş anlatımı (`../spec/test-strategy.md`), mimari test iddiaları (`solid-rules.md` SOL-06), rol sorumluluğu (`.agents/tester.md`).

> **KURAL:** "Bitti" yalnız `dotnet build` + `dotnet test` çıktısıyla söylenir — **her iki bacakta** (`net48`, `net10.0-windows`) (G5).
> **YASAK:** Kanıtsız yeşil iddiası · flaky testi `Retry` ile gizlemek · duvar saati/rastgelelikle kararsız test üretmek.

### §1.1 Test Türleri (bu vault'ta)

| Tür | Ne kanıtlar | Ortam | Hız |
|---|---|---|---|
| **Unit** | tek birim mantığı | bellek, fikstür yok, substitute (NSubstitute) | < 100 ms/test |
| **Integration** | gerçek kod + gerçek SQLite (geçici dosya) | `%TEMP%` altında silinen `.db` | < 2 sn/test |
| **Architecture** | katman bağımlılık kuralları | derleme analizi (NetArchTest) | < 5 sn |
| **Smoke/performans** | açılış, sorgu bütçesi | yerel, ölçüm eşikli | dakikalar |

---

## §2 Kurallar (TS-01 .. TS-15)

| ID | Kural | Neden | İhlal belirtisi | Düzeltme |
|---|---|---|---|---|
| TS-01 | **Unit/integration ayrımı net**: unit = tek tip + substitute, **veritabanı/ekran/dosya yok**; integration = gerçek SQLite + gerçek Dapper | İkisini karıştırmak yavaş, kırılgan test üretir | "Unit" testte `.db` dosyası açılıyor | Testi entegrasyon klasörüne taşı; DB gerekiyorsa gerçek fikstür |
| TS-02 | **Katman test matrisi** (§3) zorunlu: her katmanın *kendi* testi var; eksik katman = coverage **boşluğu**, test sayısına bakılmaz | Toplam test sayısı yanıltıcı; kör bölge kalır | Domain 200 test, Application 2 test | §3 satırındaki zorunlu test tipini ekle |
| TS-03 | **Unit testte EF Core ve DevExpress yok** (EF zaten yasak — G2): `DevExpress.*` tipleri unit testte **kullanılmaz**, yalnız soyutlamayla (DTO/handler) test edilir | Designer/DPI bağımlılığı testi masaüstüne kilitler, kararsızlaştırır | Unit testte `new RibbonForm()`/`GridControl` | Mantığı UI dışına çıkar (SOL-01/DX-11), handler üzerinden test et |
| TS-04 | **Deterministiklik**: `DateTime.Now/UtcNow`, `Random`, `Guid.NewGuid` **doğrudan** kullanılmaz → `TimeProvider`/`IRandom`/`IGuidFactory` seam'i (`migrate-static` mantığı); saat doğrudan gerekiyorsa test sabit saat kurar | Duvar saati/geçici test = flaky (§7) | Yarın başka sonuç veren tarih testi | Seam ekle; testte sabitle (`new FakeTimeProvider(t0)`) |
| TS-05 | **Ağ/LLM yok**: testler gerçek API/İnternet'e çıkmaz; provider davranışı kayıtlı/fake `IModelProvider` ile simüle edilir | API anahtarı, gecikme, sürüklenme = kararsızlık + maliyet | Testte gerçek `HttpClient` çağrısı | Substitute + sabit yanıt gövdesi |
| TS-06 | **Test verisi sahne ile kurulur**: her test kendi verisini üretir (builder/fixture); birbirine bağımlı sıra yok; **ortak mutable global durum yok** | Sıra bağımlı test = flaky + gizli koplaj | `// bu test X'ten sonra çalışmalı` | Veriyi test içinde kur; temizleme `IDisposable` fixture ile |
| TS-07 | **İptal/hata yolları test edilir**: yalnız mutlu yol testi yetersiz; null/bulunamadı/zaman aşımı/iptal senaryoları zorunlu | Üretimde patlayan yol hep hata yoludur | Her test yalnız `Assert.True(result.IsSuccess)` | §5 iddia çeşitliliği |
| TS-08 | **Adlandırma** (kod İngilizce — NC-18): `Method_Scenario_Expected` — `FindAsync_MissingSession_ReturnsFailure`; sınıf adı `<Sınıf>Tests`; okunur assertion mesajı | İsim senaryoyu anlatmıyorsa test belgedir, denetim değildir | `Test1`, `TestFoo`, `MyTest` | Yeniden adlandır; senaryoyu isme koy |
| TS-09 | **Assertion kalitesi**: "var mı" değil **neyin ne olduğu**; exception `Assert.ThrowsAsync<T>` ile tip+mesaj; koleksiyon sırası/eleman sayısı; kaynak durum (DB satırı) doğrulanır | Her zaman doğru görünen assertion = test yok | Yalnız `Assert.NotNull` / `Assert.True(true)` | Somut değer/kural assert et |
| TS-10 | **Coverage hedefi**: `Domain` + `Application` satır coverage **≥ %80**; tüm repo **≥ %70**; **yeni kod** için < %80 = inceleme RED; `Infrastructure` repository'leri entegrasyon testiyle karşılanır | Düşük coverage + G5 = kanıtsız teslim | Yeşil ama coverage artmayan PR | Kör bölgeye test yaz (`spec/test-strategy.md` §coverage) |
| TS-11 | **Şartname kapsamı**: spec'teki her `VC-XXX-NNN` gereksinimi için en az bir test **veya** açıkça gerekçeli `// test: VC-XXX-NNN kapsam disi (neden)` | Gereksinimsiz test = spec ile bağı gevşer | Spec'te madde, kodda test yok | Eşlemeyi `test-strategy.md` §gates tablosunda tut |
| TS-12 | **Gate öncesi zorunlu set** (§6) — GATE-004/GATE-005'ten önce **çalışır ve yeşil** olmalı | Gate, "yazıldı" ile "çalışıyor"ı ayırır (G5) | Gate sunumunda test yok | §6 setini çalıştır, çıktıyı rapora yapıştır |
| TS-13 | **Flaky = RED**: tekrar çalıştırınca değişen test **hata** sayılır; `Retry`/`Skip` ile gizlemek yasak; flaky test en geç **aynı part içinde** kök nedeniyle düzeltilir ya da gerekçesiyle kaldırılır | Flaky yeşil = yeşilin anlamı biter | `[Retry(3)]`, `Skip=` yaygın | Determinizm (TS-04/05/06) ihlalini bul, düzelt |
| TS-14 | **Test izolasyonu/paralelizm**: testler paralel koşar; aynı DB/fiziksel kaynak paylaşan testler xUnit *collection* ile sıralı; test ortamı (`%TEMP%` klasörü, port, dosya) her testte benzersiz | Paralelde çakışma = flaky | "database is locked" / dosya paylaşım hatası | Ortam başına benzersiz ad + collection |
| TS-15 | **Test kodu da kurala tabidir** (`CS-*`): `async` test `Task` döner, `async void` yok (örnek: `[Fact] public async Task`), duplicate değil yardımcı fabrika, commented-out test **silinir** | Ölü test = yanlış güven | Yorumlanmış `[Fact]` blokları | Sil ya da geri aç |

> **YASAK:** `Ignore`/`Skip` ile yeşil göstermek. Atlanan test raporlanır: ad + neden + kapanış part'ı.

---

## §3 Katman Test Matrisi (TS-02)

| Katman | Unit | Integration | Mimari | Not |
|---|---|---|---|---|
| `Abstractions` | **zorunlu** | — | — | `Result<T>`/event sözleşmeleri |
| `Domain` | **zorunlu (en yoğun)** | — | **zorunlu** (inner) | invariant + kural tabloları |
| `Application` | **zorunlu** (NSubstitute) | — | **zorunlu** (`Infrastructure` referansı 0) | handler/servis akışı |
| `Infrastructure` | ince eşleme | **zorunlu** (geçici SQLite) | — | parametreli SQL + timeout + migration |
| `Protocol` | **zorunlu** (şema/sürüm) | — | — | envelope round-trip, hata kodları |
| `Host` | — | **zorunlu** (composition smoke) | **zorunlu** (DI eksiksiz) | `GetRequiredService` denemesi |
| `UI (Desktop)` | **DevExpress'siz** mantık testi | — | **zorunlu** (UI katmanı dışarı DX referansı yok) | form kodu ince; manuel/GATE-003 |
| `Cli` | argument ayrıştırma | uçtan uca komut (yerel) | — | çıkış kodu sözleşmesi |

---

## §4 Determinizm Kuralı (TS-04) — BAD -> GOOD

```csharp
// BAD — duvar saati + rastgele: yarin farkli sonuc
[Fact]
public async Task Session_Expires_After_24h()
{
    var repo = new SessionRepository();          // gercek DB? -> TS-01 ihlali
    var age = DateTime.UtcNow - repo.CreatedAt;
    Assert.True(age.TotalHours > 24);            // her zaman gecmise gore dogru: bos test
}

// GOOD — sabit saat + kendi verisi + somut iddia
[Fact]
public async Task ExpireAsync_WhenSessionOlderThan24Hours_MarksSessionExpired()
{
    var t0 = new DateTimeOffset(2026, 9, 25, 12, 0, 0, TimeSpan.Zero);
    using var db = TestDb.New();                 // gecici SQLite (spec/test-strategy.md)
    var repo = new DapperSessionRepository(db.ConnectionFactory);
    var id = await repo.InsertAsync(TestData.Session(CreatedAt: t0.AddHours(-25)), CancellationToken.None);

    var result = await _service.ExpireAsync(id, CancellationToken.None);

    Assert.True(result.IsSuccess);
    var stored = await repo.FindAsync(id, CancellationToken.None);
    Assert.Equal(SessionState.Expired, stored!.State);   // kaynak durumu dogrulanir
}
```

| Kararsızlık kaynağı | Seam / çözüm |
|---|---|
| `DateTime.UtcNow` | `TimeProvider` (`GetUtcNow()`) |
| `Random` | `IRandom` / sabit tohum |
| `Guid.NewGuid` | `IGuidFactory` (testte sabit sıra) |
| gerçek LLM/HTTP | fake `IModelProvider` (TS-05) |
| ortak `.db` dosyası | benzersiz geçici dosya (TS-14) |
| paralelde port/dosya çakışması | collection + benzersiz ad |

---

## §5 İddia Çeşitliliği (TS-09)

| İyi | Kötü |
|---|---|
| `Assert.Equal(SessionState.Running, s.State)` | `Assert.NotNull(s)` |
| `Assert.Equal("VC-E-0003", ex.ErrorCode)` | `Assert.Throws<Exception>(...)` (genel tip) |
| `Assert.Equal(3, rows.Count)` + beklenen elemanlar | `Assert.True(rows.Count > 0)` |
| `Assert.Contains(r => r.Id == id, rows)` | `Assert.True(rows.Any())` |
| DB'de **kaynak durumu** doğrulaması | yalnız dönüş değerine bakmak |
| `await Assert.ThrowsAsync<ToolTimeoutException>(...)` | exception'ı `try/catch` + `Assert.True(false)` |

---

## §6 Gate Öncesi Zorunlu Test Seti (TS-12)

### §6.1 GATE-004 (Architecture Approval) öncesi

| # | Zorunlu | Kanıt |
|---|---|---|
| 1 | `dotnet build` **net48** → 0 hata | build çıktısı |
| 2 | `dotnet build` **net10.0-windows** → 0 hata | build çıktısı |
| 3 | Mimari testler (SOL-06 §4) yeşil | test çıktısı |
| 4 | EF Core paketi/referansı = 0 | tarama çıktısı |
| 5 | DevExpress yalnız UI'da; sürüm yalnız v26.1 | tarama çıktısı |

### §6.2 GATE-005 (Implementation Plan Approval) öncesi

| # | Zorunlu | Kanıt |
|---|---|---|
| 1 | `dotnet test` **her iki bacakta** yeşil | test çıktısı (adet + süre) |
| 2 | §3 matrisindeki zorunlu katman testleri **var ve yeşil** | dosya listesi |
| 3 | Coverage raporu: `Domain`+`Application` ≥ %80 | rapor dosyası |
| 4 | Migration + checksum doğrulama testi yeşil (DA-11) | test adı |
| 5 | Parameterized-only taraması = 0 ihlal (DA-03) | tarama çıktısı |
| 6 | Flaky test = 0 (`Skip`/`Retry` yok) | tarama çıktısı |

> **UYARI:** Bu tablolar `../spec/test-strategy.md` §gates eşlemesidir; iki dosya çelişirse gate tanımı (`WORKFLOW.md`) kazanır, eşleme güncellenir.

---

## §7 Review Checklist (incelemede işaretlenir)

- [ ] TS-01: test türü doğru klasörde (unit/integration ayrımı)
- [ ] TS-02: matriste eksiksiz katman kapsamı
- [ ] TS-03: unit testte `DevExpress.*`/Designer = 0
- [ ] TS-04: doğrudan `DateTime.UtcNow`/`Random`/`Guid.NewGuid` = 0
- [ ] TS-05: ağ çağrısı olan test = 0
- [ ] TS-06: testler kendi verisini kuruyor; sıra bağımlılığı yok
- [ ] TS-07: hata/iptal/bulunamadı yolları test edilmiş
- [ ] TS-08: `Method_Scenario_Expected` adlandırması
- [ ] TS-09: assertion somut (değer/tip/kaynak durumu)
- [ ] TS-10: coverage hedefi tutuyor (≥ %80 ilgili katman)
- [ ] TS-11: spec gereksinimi ↔ test eşlemesi
- [ ] TS-12: ilgili gate seti çalıştırılmış ve çıktısı kayıtlı
- [ ] TS-13: `Skip`/`Retry` yok; flaky kalmadı
- [ ] TS-14: paralel koşuda çakışma yok
- [ ] TS-15: `async void`/yorumlanmış test yok

---

## §8 Sık Yapılan İhlaller (Common Violations)

| # | İhlal | Tip | Semptom | Düzeltme |
|---|---|---|---|---|
| 1 | Testte gerçek saatle hesap | TS-04 | yarın kırmızı | `TimeProvider` sabitle |
| 2 | Testte `new GridControl()` | TS-03 | masaüstü bağımlı, yavaş | mantığı handler'a taşı |
| 3 | `[Retry(3)]` | TS-13 | sahte yeşil | kök nedeni determinizmde bul |
| 4 | Her yerde ortak `test.db` | TS-14 | kilit/çakışma | benzersiz geçici dosya |
| 5 | Yalnız mutlu yol | TS-07 | üretimde hata yolu patlar | hata/iptal senaryosu |
| 6 | `Assert.True(result.IsSuccess)` tek başına | TS-09 | ne doğrulanıyor belirsiz | somut durum assert'i |
| 7 | Spec maddesiz test yığını | TS-11 | kapsam sahte görünür | `VC-XXX-NNN` eşlemesi |
| 8 | Gate öncesi test çalıştırmama | TS-12 | G5 ihlali | §6 setini çalıştır |
| 9 | Gerçek API'ye giden test | TS-05 | anahtar/gecikme | fake provider |
| 10 | Yorumlanmış eski testler | TS-15 | yanlış güven | sil/geri aç |

---

## §9 Doğrulama

- [ ] 15 kural `ID | Kural | Neden | İhlal belirtisi | Düzeltme` biçiminde
- [ ] §3 katman matrisi tam; §6 gate seti GATE-004/005 ile uyumlu
- [ ] EF Core yalnız "yasak" olarak anılıyor
- [ ] Diyagram kullanılmadı; tablolar ASCII sınırlarında
- [ ] Frontmatter 8 alan, `authority: SSOT`, `updated: 2026-09-25`

---

## §10 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Anayasa (G5) | `../CLAUDE.md` |
| 2 | Anahtar: xUnit, NSubstitute, NetArchTest, coverage | `../keys.md` |
| 3 | Test stratejisi (piramit, fikstür, gate) | `../spec/test-strategy.md` |
| 4 | Mimari test iddiaları | `solid-rules.md` |
| 5 | Determinizm/kod kuralları | `coding-standards.md` |
| 6 | Veri erişim testi (geçici SQLite) | `data-access-rules.md` |
