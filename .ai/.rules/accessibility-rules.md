---
title: "Erişilebilirlik Kuralları - VersaCoder"
type: rules
category: rules
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "WinForms erişilebilirliği: klavye tam erişim, tab sırası/mnemonic, odak görünürlüğü, kontrast ve renk-karar, okuyucu metni, DPI; WCAG 2.1 AA karşılıkları"
---

# Erişilebilirlik Kuralları (`.rules/accessibility-rules.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [CLAUDE.md](../CLAUDE.md) · [keys.md](../keys.md) · [test-strategy](../spec/test-strategy.md) · [ui-rules.md](ui-rules.md) · [devexpress-rules.md](devexpress-rules.md)

> **Tek cümlelik sorumluluk:** *Ekran klavyeyle, klavyesiz ve düşük görmeyle kullanılabilir mi; kullanılamıyorsa hangi kural ihlal oldu ve kimi RED eder* sorusunun tek cevabıdır.
> Ekran **yerleşimi ve kontrol seçimi** [ui-rules.md](ui-rules.md) ile birlikte okunur; çelişirse erişilebilirlik kazanır (erişilebilirlik pazarlama tercihi değil, gate kuralıdır).

---

## §0 Özet (Answer First)

WinForms ekranı **klavyeyle tek başına** kullanılabilir olmalıdır: fare gerekmeden tüm eylemler, **Tab sırası görsel sırayla** ve **memonic** (`Alt+F`) ile erişilebilir; odak **her zaman görünür** (çerçeve/arka plan farkı ≥ 3:1); bilgi yalnız renkle taşınmaz (ek ikon/metin); DevExpress `Office2010` temasında varsayılan kontrast **en az 4.5:1** (normal metin) ve **3:1** (büyük metin/grafik). Ribbon kullanıldığında erişilebilirlik **QAT + klavye kısayolları** üzerinden sağlanır; yalnızca tıklanabilir ribbon öğeleri yasaktır. İhlal `reviewer` için **HIGH**, klavye tuzakları (odak tuzağı / kayıp odak) **CRITICAL**'dır.

---

## §1 Kurallar

| # | Kural | İyi örnek | Kötü örnek | İhlalde kimi RED |
|---|---|---|---|---|
| A1 | Tamamı klavyeyle | `Alt+S` kaydet, `F5` yenile | yalnız `OnClick` olan buton | `reviewer` |
| A2 | Tab sırası görsel sıra | `TabIndex` soldan sağa, yukarıdan aşağıya | odak zıplıyor | `reviewer` |
| A3 | Memonic tanımlı | `&Kaydet` → `Alt+K` | klavyesiz buton | `reviewer` |
| A4 | Odak görünür | 2 px çerçeve + zemin farkı | odak yok (nerede olduğum bilinmiyor) | `reviewer` (**HIGH**) |
| A5 | Odak tuzağı yok | döngüsel `Tab`, Escape ile çıkış | modal içinde Tab sonsuz döner | `reviewer` (**CRITICAL**) |
| A6 | Renk tek başına bilgi taşımaz | `HATA` + kırmızı ikon | yalnız kırmızı yazı | `reviewer` |
| A7 | Kontrast eşiği | ≥ 4.5:1 normal · ≥ 3:1 büyük/ikon | açık gri üstü gri | `reviewer` |
| A8 | Okunur metin | kısa etiket, cümle düzeni | `btn1` / `txt2` (AccessibileName yok) | `ui-designer` |
| A9 | Yeniden boyutlanabilir | `AutoScaleMode.Dpi`, %125/%150 | sabit piksel, kırpılan metin | `ui-designer` |
| A10 | Durum yalnız renk değil | + ikon + metin + `AccessibleDescription` | yeşil nokta = "bağlı" | `ui-designer` |
| A11 | Hata odaklanır | doğrulama hatasında hatalı alana git | hata yalnız toast, odak yerinde | `reviewer` |
| A12 | Zaman sınırı/animasyon yoksa | bekleme animasyonu iptal edilebilir | oturum kapanan tek tıklama | `tester` |

---

## §2 Klavye Haritası (asgari)

| Eylem | Tuş | Kural |
|---|---|---|
| Bir sonraki / önceki alan | `Tab` / `Shift+Tab` | döngü her zaman kapalı değil, **görünür** sıra |
| Eylem çalıştırma | `Alt+<mnemonic>` | her eylem butonunda bir mnemonic |
| Onay / iptal | `Enter` / `Esc` | modal formda varsayılan belli |
| Yenile | `F5` | listede `F5` her yerde aynı |
| Ribbon eylemi | `Alt`, sonra kısayol | QAT'a eklenmiş eylemler |

> **YASAK:** Yalnızca fareyle tetiklenen eylem (sağ tık menüsü **tek** erişim yoluysa) — `reviewer` RED.

---

## §3 Renk ve Kontrast

| # | Öğe | Eşik (WCAG 2.1 AA) | Ölçüm |
|---|---|---|---|
| 1 | Normal metin | ≥ **4.5:1** | zemin/ön plan oranı |
| 2 | Büyük metin (≥ 18 pt / 14 pt bold) | ≥ **3:1** | aynen |
| 3 | Odak çerçevesi | ≥ **3:1** | komşu zemin |
| 4 | Arayüz ikonu / sınır | ≥ **3:1** | aynen |
| 5 | Durum rengi | renk **+** ikon/metin | renk tek başına yasak |

---

## §4 DevExpress / Ribbon Karşılığı

| Erişilebilirlik ihtiyacı | WinForms karşılığı | Not |
|---|---|---|
| Eylem listesi | Ribbon `QAT` + klavye kısayolu | yalnız ribbon'a tıklanamaz (A1) |
| Kontrol adı | `AccessibleName` / `AccessibleDescription` | `ui-designer` sahibi |
| Okuyucu sırası | `Tab` sırası + `TabIndex` | `ui-designer` sahibi |
| Öğe görünür olmayabilir | her eylem için **klavye karşılığı** zorunlu | QAT'a ekle |
| Tema kontrastı | `RibbonControlStyle.Office2010` içinde doğrula | tema G3 ile kilitli (ADR-004), **renk paleti değişmez**; varyant gerekiyorsa ADR + kullanıcı |

---

## §5 Test ve Kanıt

| # | Test | Kim |
|---|---|---|
| 1 | Fare **0** ile senaryo tamamlanır (otomasyon + manuel) | `tester` |
| 2 | `TabIndex` sırası = okuma sırası (assert) | `tester` |
| 3 | Her eylem butonunda `AccessibleName` dolu (assert) | `tester` |
| 4 | Kontrast oranı ölçümü (zemin/ön plan) | `ui-designer` + `reviewer` |
| 5 | %125/%150 DPI'de metin kırpılmıyor | `tester` |
| 6 | Odak tuzağı yok (döngü + Escape) | `tester` |

**Kanıt biçimi:** test çıktısı ya da ekran görüntüsü + ölçüm değeri; **kanıtsız "erİŞilebilir" iddiası yasaktır** (G5).

---

## §6 İstisnalar (ADR gerekir mi?)

| # | İstisna | ADR gerekir mi | Onay |
|---|---|---|---|
| I1 | Teknik kısıt: DevExpress kontrolü mnemonic desteklemiyor | Hayır — **QAT/klavye karşılığı** zorunlu | `reviewer` |
| I2 | Erken erişim sürümünde eksik erişilebilirlik | **Evet** + görünür eksik listesi | kullanıcı |
| I3 | Kontrast eşiklerini karşılayan palet değişikliği | **Evet** (G3 teması kilitli) | kullanıcı |
| I4 | Ekran okuyucu testi (VoiceOver/NVDA) | Hayır — `tester` manuel kapsamına girer | `tester` |

---

## §7 Doğrulama

- [ ] Her eylem `Alt+<mnemonic>` ya da QAT kısayoluyla erişilebilir
- [ ] `TabIndex` görsel sıra ile aynı; odak **her** alanda görünür
- [ ] Renk tek başına bilgi taşımıyor (durum + ikon/metin)
- [ ] Kontrast: normal ≥ 4.5:1, büyük/ikon ≥ 3:1 (ölçüm kaydı)
- [ ] Fare 0 ile senaryo tamamlanır (test çıktısı raporda)

---

## §8 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Ekran yerleşimi ve kontrol seçimi | `.ai/.rules/ui-rules.md` |
| 2 | DevExpress sürüm ve ribbon kilidi | `.ai/.decisions/accepted/ADR-004-devexpress-universal-v261-office2010.md` |
| 3 | Form üçlüsü (`.cs`/`.Designer.cs`/`.resx`) | `.ai/.decisions/accepted/ADR-015-designer-resx-convention.md` |
| 4 | Kabul kriterleri / test | `.ai/spec/test-strategy.md` |
| 5 | Anayasa (G1..G12) | `.ai/CLAUDE.md` |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `CLAUDE.md` G3/G4/G5 + ADR-004/015.
