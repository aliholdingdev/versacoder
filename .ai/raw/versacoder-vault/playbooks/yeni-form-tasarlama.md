---
title: "yeni-form-tasarlama - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "Yeni WinForms ekrani icin wireframe -> Form + Designer.cs + resx zinciri - GATE-003, DevExpress v26.1, build kaniti"
---

# yeni-form-tasarlama — Yeni Form Tasarlama

**Zorunlu Baglantilar:** [vault index](../index.md) · [CLAUDE.md](../CLAUDE.md) · [WORKFLOW.md](../WORKFLOW.md) · [wireframe-uretme](wireframe-uretme.md)

> **Tek cumlelik sorumluluk:** *Yeni ekran nasil wireframe'ten calisan forma donusur, sira kimde* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Sira degismez: **wireframe (ui-designer) -> GATE-003 kullanici onayi -> Form uclemesi -> olay baglayicilari -> build**. Onaysiz form yazmak G12 ihlalidir, is RED olur ve part basa doner. Kontroller yalniz DevExpress **v26.1**; `RibbonForm` kullanilacaksa stil **yalniz** `RibbonControlStyle.Office2010`. Toplam ≈40 dk (onay suresi haric).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Ekran adi master prompt'ta (01..07) | `.ai/VISION.md` 7 ekran listesi |
| 2 | ASCII wireframe yazilmis ve onaylanmis | `.ai/ui-design/NN-<ekran>.md` + GATE-003 kaydi |
| 3 | DevExpress yalniz v26.1 kurulu | `Directory.Packages.props` tek surum satiri |
| 4 | Mimari katman sozlesmesi (hangi katman veri verir) | `.ai/architecture/` node dosyasi |
| 5 | Build ortami calisiyor | `dotnet build` -> 0 hata (mevcut durum) |

---

## §2 Adimlar

**Toplam ≈40 dk** · yapan: `ui-designer` (1-3) + `developer` (4-5)

1. **Wireframe'i kilitle** — box-cizim ASCII, kontrol listesi v26.1 disina cikmaz; kullanici onayi alinmadan adim 2'ye gecilmez (HARD STOP).
   - Dosya: `.ai\ui-design\NN-<ekran>.md`
   - Cikti: GATE-003 onayi (`log.md` satiri)

2. **Form uclemesini olustur** — `.Form.cs` + `.Form.Designer.cs` + `.Form.resx` ayni anda; `resx` **elle degil** designer uzerinden uretilir.
   - Ornek: `src\VersaCoder.UI\Screens\MainIde.Form.cs` (+ `.Designer.cs`, `.resx`)
   - Cikti: designer acilir, `ResourceManager` adlari bozulmaz

3. **Kontrol setini bagla** — `RibbonForm` + `RibbonControlStyle.Office2010`; grid/tree tek `v26.1` API ile.
   - Dosya: `*.Form.Designer.cs`
   - Cikti: `Select-String -Path 'src\**\*.cs' -Pattern 'RibbonControlStyle\.\w+'` -> yalniz `Office2010`

4. **Olaylari yonlendir** — code-behind yalniz olay firlatir; is mantigi `Application` katmanina gider (200+ satir code-behind = RED).
   - Dosya: `*.Form.cs` + ilgili handler
   - Cikti: derlenen, I/O icermeyen olay metotlari

5. **Build + kendi gozunu gec** — iki bacak da derlenmeden devre gecersizdir.
   - Komut: `dotnet build C:\versacoder\<sln> -v m`
   - Cikti: `0 hata` (net48 + net10.0-windows)

---

## §3 Kanit

| # | Kanit | Beklenen cikti |
|---|---|---|
| K1 | GATE-003 onayi | `log.md`'de "GATE-003 kapandi" satiri |
| K2 | Uc dosya | `<ekran>.Form.cs` + `.Designer.cs` + `.resx` ayni klasorde, 3 dosya |
| K3 | Ribbon stili | `Select-String` ciktisi = yalniz `Office2010` |
| K4 | Build | `dotnet build` -> `0 hata` (her iki bacak) |
| K5 | Designer | VS/Rider'da form acilir, hata vermez |

**Ornek cikti (K4):**

```text
Derleme tamamlandı. 0 Hata. 0 Uyarı.
VersaCoder.UI -> bin\Debug\net48\VersaCoder.UI.dll
VersaCoder.UI -> bin\Debug\net10.0-windows\VersaCoder.UI.dll
```

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Wireframe | `.ai\ui-design\NN-<ekran>.md` -> `status: draft`, onayi geri al |
| 2 | Form uclemesi | 3 dosyayi birlikte sil (tekini silme, tutarsizlik kalir) |
| 3 | `ResourceManager` adlari bozulduysa | designer'i ac, `resx`'i yeniden kaydet; elle duzeltme yasak |
| 4 | Olay baglayicilari | `*.Form.cs`'den handler'i kaldir, `.Designer.cs`'e dokunma |
| 5 | Build kirildi | son yeşil commit'e `git revert <sha>` + tekrar `dotnet build` |

