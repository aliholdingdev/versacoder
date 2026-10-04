---
title: "ADR-015 — Form Üçlüsü: .cs + .Designer.cs + .resx"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "WinForms ekran dosyasının üçlü yapısı, dosya adlandırma ve designer ile elle düzenlemenin sınırı"
---

# ADR-015 — Form Üçlüsü: .cs + .Designer.cs + .resx

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) · [ADR-025](ADR-025-gate003-ui-hard-stop.md)

> **Tek cümlelik karar:** Her ekran **`<Ekran>.Form.cs` + `<Ekran>.Form.Designer.cs` + `<Ekran>.Form.resx`** üçlüsü olarak yaşar; `.Designer.cs` ve `.resx` **yalnız tasarımcı üzerinden** üretilir ve **son söz `ui-designer`'ındadır** — karar verildi.

---

## §0 Özet (Answer First)

WinForms'ta üç dosya tek ekranı temsil eder ve biri elle bozulursa ekran **açılmaz**. Bu karar dosya adını (`<Ekran>.Form.*`), sahipliği (`ui-designer`) ve müdahale sınırını kilitler: **`developer` `.resx`'i elleyemez**; `.Form.cs` yalnızca olay yönlendiricisidir, iş mantığı `l2 Application` katmanına gider. `ResourceManager` adları bozulursa `G3`/`G4` ihlali doğar. Ekranın **ne** olduğu önce [ADR-025](ADR-025-gate003-ui-hard-stop.md) GATE-003 onayında belli olur; onaysız `.cs` yazılmaz.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | `ui-designer` `.Designer.cs` ve `.resx` konusunda **X (son söz)** sahibidir | `AGENTS.md` §3.2/8 |
| C2 | `.resx` elle değil **tasarımcı üzerinden** üretilir; `ResourceManager` adı bozulursa form kırılır | `AGENTS.md` §2.5 UYARI |
| C3 | `developer` `.resx`'i ellemişse bulgu biçimi RED'dir (yetki ihlali) | `AGENTS.md` §3.4 |
| C4 | Ekran kontrol seti DevExpress **v26.1**, ribbon **yalnız `RibbonControlStyle.Office2010`** | [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) |
| C5 | 7 ekranın wireframe'i onaylanmadan `.cs` yazılmaz | [ADR-025](ADR-025-gate003-ui-hard-stop.md) |
| C6 | Code-behind 200+ satır olursa iş mantığı yanlış yerdedir | `AGENTS.md` §3.4 |

---

## §2 Karar

**Karar:** Ekran = **üç dosya** (`Form.cs` · `Form.Designer.cs` · `Form.resx`), tasarım dosyaları **tasarımcıdan** gelir, müdahale yetkisi **`ui-designer`'da** ve yalnız GATE-003 onayından sonra kod yazılır — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Dosya adı | `<Ekran>.Form.cs` · `<Ekran>.Form.Designer.cs` · `<Ekran>.Form.resx` | C1 |
| D2 | `.Form.cs` içeriği | partial sınıf + **olay yönlendirici** (tıklama → handler çağrısı); iş mantığı **yok** | C6 |
| D3 | `.Designer.cs` | tasarımcı üretir; elle düzenleme **yalnız `ui-designer`** ve gerekçeli | C1 · C2 |
| D4 | `.resx` | tasarımcı üretir; `ResourceManager` adları **değiştirilemez** | C2 |
| D5 | Kontrol seti | DevExpress **v26.1** dışına çıkılmaz; ribbon `Office2010` | C4 |
| D6 | Ön koşul | `.Form.cs` yalnız **GATE-003 onayı sonrası** yazılır | C5 |
| D7 | İş mantığı | `l2 Application` (command/query/handler); code-behind ≤ ~100 satır hedefi | C6 |

**Neden:** Tasarımcı dosyaları **üretim çıktısıdır**, elle yazılan kod değil; bu yüzden sahiplik `ui-designer`'da toplanır ve `developer`'ın erişimi sınırlandırılır. Üç dosyanın tek elde toplanmaması (partial sınıf) tasarım ile davranışı ayırır: tasarım değişince kod değişmez. Ön koşulun (D6) hard stop olması, wireframe değişince kodun çöp olmasını engeller — en pahalı yeniden yazım budur.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Form her zaman açılır (tasarımcı tutarlılığı) | Visual Studio tasarımcısında açılış testi |
| Yetki net: tasarım değişikliği tek sahipli | `AGENTS.md` §3.2/8 |
| İş mantığı test edilebilir (code-behind'da değil) | `l2` handler birim testi |
| Kontrol/stil sapması erken yakalanır | `CLAUDE.md` G3 taraması: yalnız `Office2010` |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Tasarımcı dosyasını elle açmak cazip gelir | D3: elle müdahale yalnız `ui-designer`, gerekçeli |
| `ResourceManager` adı koptuğunda form sessizce kırılır | form açılış testi + `.resx` ad denetimi |
| Üç dosya taşınırken biri unutulabilir | taşıma = üçlü birlikte; eksik dosya build hatası |
| Code-behind yine şişebilir | D7 hedef + `reviewer` MEDIUM bulgusu |

**NEEDS_VALIDATION:** DevExpress **v26.1** tasarımcısının `net48`/`net10.0-windows` iki bacakta da aynı `.Designer.cs` çıktısını üretip üretmediği **GATE-003 sonrasında** ilk ekran üzerinde doğrulanır; fark çıkarsa `.Designer.cs` **tek bacakta üretilip** diğerine kopyalanır ve bu ADR güncellenir.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) | related | kontrol seti ve ribbon stili kilidi |
| [ADR-025](ADR-025-gate003-ui-hard-stop.md) | related | `.Form.cs` yazılmadan önceki onay ön koşulu |
| [ADR-010](ADR-010-cli-desktop-shared-core.md) | related | form yalnız `l7` hostunda yaşar |
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | related | iş mantığının gideceği katman (l2) |
| [ADR-026](ADR-026-logging-structured.md) | related | olay yönlendiricisinin log çıktısı |

---

## §5 Kabul Kriterleri

- [ ] Her ekran = tam üç dosya; eksik üçlü build'i kırıyor
- [ ] `.Form.cs` içinde iş mantığı **yok** (code-behind ≤ ~100 satır)
- [ ] `.resx`/`.Designer.cs` değişikliği yapan rol = `ui-designer` (inceleme kaydı)
- [ ] `RibbonControlStyle.*` taraması yalnız `Office2010` döndürüyor (G3)
- [ ] Form açılış testi her iki bacakta yeşil

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `AGENTS.md` §2.5 + ADR-004.
