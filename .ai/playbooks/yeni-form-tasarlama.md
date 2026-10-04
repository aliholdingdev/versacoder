---
title: "yeni-form-tasarlama - VersaCoder"
type: playbook
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Yeni WinForms ekrani icin wireframe -> Form + Designer + resx zinciri - GATE-003, DevExpress v26.1, build kaniti"
---

# yeni-form-tasarlama — Yeni Form Tasarlama

**Zorunlu Baglantilar:** [playbooks index](index.md) · [WORKFLOW](../WORKFLOW.md) · [AGENTS](../AGENTS.md) · [wireframe-uretme](../playbooks/wireframe-uretme.md) · [dual-tfm-build](../playbooks/dual-tfm-build.md)

> **Tek cumlelik sorumluluk:** *Yeni ekran nasil wireframe'ten calisan forma donusur, sira kimde* sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Sira **degistirilemez**: **wireframe (`ui-designer`) -> GATE-003 kullanici onayi -> Form uclemesi -> olay baglayicilari -> build**. Onaysiz form yazmak G12 ihlalidir, is RED olur ve basa doner. Kontroller yalniz DevExpress **v26.1**; `RibbonForm` kullanilacaksa stil **yalniz** `RibbonControlStyle.Office2010`. Tasarim sozlesmesi `ui-designer`'da, uygulama `developer`'da: ayni elde toplanirsa inceleme yapilamaz (AGENTS §1.2). Toplam ≈40 dk (onay suresi haric).

---

## §1 On Kosullar

| # | Ne hazir olmali | Kanit |
|---|---|---|
| 1 | Ekran adi kesin | [VISION](../VISION.md) 7 ekran listesi |
| 2 | ASCII wireframe onaylanmis | `.ai/ui-design/NN-<ekran>.md` + GATE-003 kaydi |
| 3 | DevExpress yalniz v26.1 | `Directory.Packages.props` tek surum satiri |
| 4 | Katman sozlesmesi (hangi katman veri verir) | [ADR-028](../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) |
| 5 | Build ortami calisiyor | dual-tfm-build §5 (0 hata) |

---

## §2 Adimlar

**Toplam ≈40 dk** · yapan: `ui-designer` (1-2) + `developer` (3-5)

| # | Adim | Komut / Cikti | Sure |
|---|---|---|---|
| 1 | Wireframe'i kilitle | saf ASCII + v26.1 kontrol listesi; onay alinmadan adim 2'ye gecilmez | 5 dk |
| 2 | Form uclemesini olustur | form + designer + resx ayni anda; resx elle degil | 10 dk |
| 3 | Kontrol setini bagla | `RibbonForm` + `RibbonControlStyle.Office2010` | 8 dk |
| 4 | Olaylari yonlendir | code-behind yalniz olay firlatir; is mantigi uygulama katmanina | 10 dk |
| 5 | Build + gozden gecirme | iki bacak `dotnet build` -> 0 hata | 7 dk |

---

## §3 Hata Modlari

| # | Belirti | Kok neden | Duzeltme |
|---|---|---|---|
| 1 | Form acilmiyor / ResourceManager hatasi | resx elle duzeltilmis | Designer acilir, resx yeniden kaydedilir (elle duzeltme yasak) |
| 2 | Ribbon stilinde baska deger | Kopya kod | `Office2010`'a cevrilir, tekrar build |
| 3 | 200+ satir code-behind | Is mantigi forma girmis | Mantik uygulama katmanina tasinir |
| 4 | Tek bacakta derleniyor | TFM/disiplin eksik | dual-tfm-build adimlari tekrarlanir |

---

## §4 Geri Alma

| # | Degisiklik | Tersi |
|---|---|---|
| 1 | Wireframe | dosya `status: draft`'a cevrilir, onay geri alinir |
| 2 | Form uclemesi | **3 dosya birlikte** silinir (teki degil) |
| 3 | ResourceManager adlari bozulduysa | designer acilir, resx yeniden kaydedilir |
| 4 | Olay baglayicilari | handler form dosyasindan kaldirilir, designer dosyasina dokunulmaz |
| 5 | Build kirildi | son yeşil commit'e `git revert <sha>` + iki bacak build |

---

## §5 Kabul Kriteri

- [ ] GATE-003 onayi `log.md`'de tarihli ("GATE-003 kapandi")
- [ ] Ekran klasorunde 3 dosya birlikte var (Form / Designer / resx)
- [ ] Ribbon stil taramasi yalniz `Office2010`
- [ ] `dotnet build` -> 0 hata (net48 + net10.0-windows)
- [ ] Form IDE'de aciliyor, hata vermiyor

---

## §6 Ornek Cikti

```text
+--------------------------------------------------+
| KANIT  : build -f net48        -> 0 Hata          |
|          build -f net10.0-windows -> 0 Hata       |
|          ribbon taramasi       -> yalniz Office2010|
| DOSYA  : Screens/<Ekran>.Form (+ Designer, resx)|
| EKSIK  : YOK                                       |
| SONRAKI : olay testlerini yaz (GATE-005, ~120 dk)  |
+--------------------------------------------------+
```

---

## §7 Butce ve Kontroller

| # | Kontrol | Komut | Beklenen |
|---|---|---|---|
| 1 | Ribbon | `Select-String -Path 'src\**\*' -Pattern 'RibbonControlStyle\.\w+'` | yalniz Office2010 |
| 2 | Uc dosya | ekran klasorunde dosya sayimi | 3 dosya |
| 3 | Build | iki bacak ayri kosum | 2 x `0 Hata` |
| 4 | Butce | adim tablosu | ≈40 dk (onay haric) |

---

## §8 Kapsam Disi

| # | Kapsam disi | Nereye gider |
|---|---|---|
| 1 | Wireframe'in kendisi | [wireframe-uretme](../playbooks/wireframe-uretme.md) |
| 2 | Onay sirasi / HARD STOP | [WORKFLOW](../WORKFLOW.md) §2.2 |
| 3 | Test yazimi | [dual-tfm-test](../playbooks/dual-tfm-test.md) |
| 4 | Katman/kapi acma | [yeni-katman-ekleme](../playbooks/yeni-katman-ekleme.md) |

---

## §9 Kanit

| # | Kaynak dosya | Ne kanitlar |
|---|---|---|
| 1 | `checklists/gate-003-ui-onay.md` | onay oncesi 13 soru |
| 2 | `checklists/winforms-designer-check.md` | designer/resx kurallari |
| 3 | `AGENTS.md` | `ui-designer` / `developer` siniri ve yanlis ajan maliyeti (§1.2) |
| 4 | `.decisions/accepted/ADR-028-layered-baseline-9-projects.md` | ekran kodunun gittigi katman |
