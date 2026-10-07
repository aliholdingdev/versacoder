---
title: "ADR-027 — Bilesen Katalogu 5000+: Sayim Araci ile Kanit"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "ADR-013 devami: >=5000 bilesen kaydinin nasil sayildigi, hangi aracla kanitlandigi ve katalog disina cikan kaydin sonucu"
hedef_bacak: "vault (katalog) + kod (net48 + net10.0-windows)"
evidence: "scripts/count-components.ps1 (-MinRecords 5000 -MinNodeFiles 52) · ADR-013 (52 dugum) · reports/00-kapsam-analizi.md §2 karar 9"
---

# ADR-027 — Bileşen Kataloğu 5000+: Sayım Aracı ile Kanıt

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [keys](../../keys.md) · [ADR-013](ADR-013-layered-architecture-52-nodes.md) · [ADR-022](ADR-022-git-commit-gate004.md)

> **Tek cümlelik karar:** **"5000+ bileşen" iddiası elle sayılmaz** — tek geçerlilik ölçütü `scripts/count-components.ps1` çıktısıdır: kayıt deseni `| NNN-### |`, eşik `-MinRecords 5000`, düğüm dosyası `-MinNodeFiles 52`; eşik altı **kanıtsız iddia** sayılır ve rapora yazılamaz.

---

## §0 Özet (Answer First)

ADR-013 mimariyi kurdu (8 makro + 44 alt = 52 düğüm, her düğümde bileşen kaydı tablosu); bu ADR **ölçümü** kurar. Kayıt = tablo satırı (`| N\d{2}-\d{3} |` ile başlayan, 8 sütunlu), başlık/ayraç/kod bloğu sayılmaz. Geçerli komut: `pwsh -File .ai/scripts/count-components.ps1 -MinRecords 5000` → **0 hata** ve **≥5000 kayıt** olmadan "5000+" ifadesi hiçbir vault dosyasında kullanılamaz (G5). Katalog dışı üretilen kayıt sayılmaz; bu, A4 (kod önce, envanter sonra) ve A5 (dosya başına bileşen) alternatiflerini kalıcı olarak kapatır.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Prompt `5000+ bileşen` şartı koyar | `reports/00-kapsam-analizi.md` §2 karar 9 |
| C2 | ADR-013 hedefi ≥5088 kayıt (44×≥112 + 8×≥20) | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2.2 |
| C3 | Sayım aracı tanımlı: desen `^\|\s*N\d{2}-\d{3}\s*\|` | `scripts/count-components.ps1` satır 35 |
| C4 | Kanıtsız "5000+" yasak (G5) | `CLAUDE.md` G5 |
| C5 | Katalog **önce**, kod sonra gelir (TDD/NetArchTest hedefi katalogdan çıkar) | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §3 A4 ret |
| C6 | `build-catalog.ps1` katalog iskeletini üretir, kayıt kendisi **yazılır** | `scripts/build-catalog.ps1` |

**Sorun:** 5000+ iddiası elle tutulabilir bir sayı değil; elle sayım iki kez farklı sonuç verir ve hiçbir gate'te denetlenemez. Ölçüm araca bağlanmazsa iddia **kanıtsız** kalır.

---

## §2 Karar

**Karar:** Tek sayım ölçütü `count-components.ps1`'dir; eşik altı sonuç "5000+" yasağını düşürür — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Kayıt tanımı | `| NNN-### |` ile başlayan 8 sütunlu tablo satırı | C3 |
| D2 | Sayım dışı | başlık, ayraç (`---`), kod bloğu, yorum satırı | D1 |
| D3 | Eşikler | `-MinRecords 5000` · `-MinNodeFiles 52` · `badFmt = 0` | C1 · C2 |
| D4 | Katalog kapsamı | `.ai/architecture/catalog/**` altındaki `*.md` | C6 |
| D5 | Katalog dışı kayıt | **sayılmaz**; rapor edilir, silinmez | A5 ret |
| D6 | Kanıt biçimi | komut + stdout (`Toplam`, `Dosya`, `Hata`) rapora yapıştırılır | C4 |
| D7 | Eşik altı sonuç | tüm "5000+" ifadeleri vault'tan **kaldırılır** ya da `NEEDS_VALIDATION` işaretlenir | C4 |

**Neden:** Örnek mürekkebi ölçülebilir olmayan iddia, gate'i kâğıt üstünde bırakır. Araca bağlanmış eşik, kataloğun **eksikliğini de görünür** kılar: sayı düşünce hangi düğümde eksiği olduğunu tablo satırından okursunuz.

---

## §3 Sayım Sözdizimi

```powershell
pwsh -File .ai/scripts/count-components.ps1                    # varsayilan esik 5000
pwsh -File .ai/scripts/count-components.ps1 -MinRecords 5000 -MinNodeFiles 52   # CI / GATE
pwsh -File .ai/scripts/count-components.ps1 -Root <yol>        # baska kok
```

| Çıktı alanı | Anlamı | Kızıl koşul |
|---|---|---|
| `Toplam` | geçen tüm kayıt satırları | `< 5000` |
| `Dosya` | katalogdaki MD sayısı | `< 52` |
| `Hata` (`badFmt`) | sütun düzeni bozuk satır | `> 0` |
| `[X] Katalog dizini yok` | katalog üretilmemiş | çıkış kodu 1 → önce `build-catalog.ps1 -Force` |

```
  GATE-004a kanit zinciri
  -----------------------
  build-catalog.ps1   ->  52 dugum iskeleti (dosya)
         |
         v
  elle yazilan satirlar ->  | NNN-### |  (8 sutun)
         |
         v
  count-components.ps1  ->  Toplam >= 5000 , Dosya >= 52 , Hata = 0
         |
         v
  rapor + log.md        ->  komut + stdout gomulu
```

---

## §4 Alternatifler

| # | Alternatif | Tanım | Neden seçilmedi |
|---|---|---|---|
| A1 | **(SEÇİLDİ)** Araçla sayım + eşik | Tek komut, CI'ya girer | — (karar bu) |
| A4 | Katalog olmadan kod yaz, sonra envanter çıkar | Hızlı başlangıç | C5: `5000+` kanıtsız kalır, TDD/NetArchTest hedefi belirsiz |
| A5 | Bileşen başına ayrı MD (5000 dosya) | 1 kayıt = 1 dosya | Dosya sayısı arama/denetimi bozar; link taraması ölçeklenemez |
| A6 | Elle sayım + tablo listesi | Tabloya elle toplam yaz | İki çalıştırma iki sonuç verir (C4); gate denetlenemez |
| A7 | Farklı eşik (ör. 3000) | Daha kolay geç | Prompt şartı `5000+`; eşik kullanıcı yetkisinde (G8) |

**Ret gerekçesi:** A4 kanıtı erteler, A6 ölçülemez, A5 maliyeti katlar, A7 kapsam değişikliği gerektirir. A1 tek başına **ölçülebilir, tekrarlanabilir ve gate'e eklenebilir**.

---

## §5 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| İddia = komut çıktısı | D6 stdout raporda |
| Eksik katalog anında görünür | `Toplam < 5000` → hangi düğümde eksik, tablodan okunur |
| CI'ya girer (GATE-004a) | D3 parametreleri |
| Format sapması yakalanır | `Hata > 0` → düzeltilecek satır listelenir |
| A4/A5/A6 kalıcı kapanır | §4 ret gerekçeleri |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Kopyala-yapıştır ile "şişirilebilir" (satır üret) | `Hata` + sütun kontrolü; içerik ADR-002 ile denetlenir |
| Araç `pwsh` gerektirir, bu oturumda çalıştırılamayabilir | `NEEDS_VALIDATION` işaretlenir, gate **kapanmaz** |
| Eşik değişikliği kapsam değişikliğidir | A7: yalnız kullanıcı onayıyla (G8) |
| Katalog elle güncellenmezse eskir | Katalog güncellemesi `developer` DoD'una eklenir |

**NEEDS_VALIDATION:** `count-components.ps1` bu batch kapsamında **çalıştırılamadı** (kabuk izni yok) → GATE-004a'da bir kez çalıştırılıp `Toplam`/`Dosya`/`Hata` sayıları bu dosyanın altına eklenir. Sayı gelmeden "5000+" ifadesi **kullanılmaz** (D7).

---

## §6 Kabul Kriterleri

- [ ] `count-components.ps1` → çıkış kodu 0, `Hata = 0`
- [ ] `Toplam ≥ 5000` ve `Dosya ≥ 52`
- [ ] Komut + stdout rapora ve `log.md`'ye gömülü
- [ ] Vault'ta `5000+` yazan her dosyada ölçüme bağlantı var
- [ ] Katalog dışı kayıt üretimi yok (D5 ihlali taraması)

---

## §7 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | devamı | 52 düğüm ve ≥5088 hedefi; bu ADR ölçümü ekler |
| [ADR-002](ADR-002-handwritten-content-no-copypaste.md) | related | katalog satırlarının elle/özgün üretilmesi |
| [ADR-003](ADR-003-vault-structure-500md-frontmatter.md) | related | katalog MD'leri 500 hedefine dahildir |
| [ADR-025](ADR-025-gate003-ui-hard-stop.md) | related | kontrol listesi havuzu = katalog bileşenleri |
| [ADR-022](ADR-022-git-commit-gate004.md) | related | ölçüm çıktısı GATE-004a kanıtıdır |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: ADR-013 §2.2 + `scripts/count-components.ps1`.
