---
title: "ui-design/flows.md - 11 Wireframe Arasi Gecis Akislari (GATE-003)"
type: spec
category: ui
status: active
version: 1.0.0
updated: 2026-10-02
authority: reference
scope: "giris -> 01-start-page -> 02-main-ide -> 03/04/05/06 -> 07-settings omurgasi, 08-ascii-art-viewer + 09-terminal + 09-cli-console + 10-ai-chat-console giris/cikisi ve her wireframe in en az 1 girisi + 1 cikisi (gate-003-ui-onay soru 8 kaniti)"
tags: [ui-design]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# flows.md - Ekranlar Arası Geçiş Akışları

**Zorunlu Baglantilar:** [[ui-design/readme|ui-design/index]] · [[ui-design/01-start-page|01-start-page]] · [[ui-design/02-main-ide|02-main-ide]] · [[ui-design/07-settings|07-settings]] · [[ui-design/08-ascii-art-viewer|08-ascii-art-viewer]] · [[definitions/vision|VISION]] · [[governance/workflow|WORKFLOW]] · [[checklists/gate-003-ui-onay|gate-003 checklist]]

> **Tek cumlelik sorumluluk:** Uygulama girisinden 8 ekrana nasil ulasilip her ekrandan
> nereye cikildigi sorusunun tek cevabidir.

---

## §0 Ozet (Answer First)

Ana omurga: `giriş -> 01-start-page -> 02-main-ide -> 03/04/05/06 -> 07-settings`.
Sekizinci ekran `08-ascii-art-viewer` yalnızca `02-main-ide` ribbon'indaki **Dosya
menusunden** acilir ve kapandiginda **`02-main-ide`'ye doner**. Her ekranın en az
**1 giriş + 1 çıkışı** asagidaki §2 tablosunda kayitlidir (checklist soru 8).

| # | Kural | Deger |
|---|---|---|
| 1 | Kabuk | `02-main-ide` is yapmaz, yalniz tasir (DIGER EKRANLAR BURADA - VISION §2.4) |
| 2 | Modal ekranlar | `07-settings` ve disa aktar diyalogu cagiran ekrana doner |
| 3 | Panel gecisleri | `02-main-ide` Görünüm sayfasi 03/04/05/06 panelini ac/kapatir |
| 4 | Yeni ekran | 9. ekran = kapsam degisikligi, onay ister (README §1) |

---

## §1 ASCII ok diyagrimi

```text
+--------------------------------------------------------------------+
| A) ANA OMURGA                                                      |
|                                                                    |
| [GİRİS: uygulama acilisi]                                          |
|       |                                                            |
|       v                                                            |
| 01-start-page --[son proje cift tık]--> 02-main-ide                |
|       |                                                            |
|       +--["Yeni Proje" tile]--> 03-solution-explorer               |
|       +--["Ayarlar" tile]-----> 07-settings                        |
|                                                                    |
| 02-main-ide cikislari:                                             |
|       +--> 03-solution-explorer   [sol dock]                       |
|       +--> 04-code-editor         [merkez sekme]                   |
|       +--> 05-ai-chat             [sağ dock üst]                   |
|       +--> 06-agent-panel         [sağ dock alt]                   |
|       +--> 07-settings            [ribbon Ayarlar, modal]          |
|       +--> 08-ascii-art-viewer    [Dosya menüsü]                   |
|       +--> 01-start-page          [merkez: Son proje yok]          |
+--------------------------------------------------------------------+
| B) IKINCIL OKLAR (donguler ve geri donusler)                       |
|                                                                    |
| 03 --[.cs cift tık]-------------> 04                               |
| 05 --["Dosyaya uygula" + onay]--> 04                               |
| 05 --[görev kuyrugu]------------> 06                               |
| 06 --[görev cift tık]-----------> 04                               |
| 06 --[F5 duzeltme]--------------> 05                               |
| 04 --[sekme kapanış]------------> 02                               |
| 07 --[Kaydet / Vazgeç]----------> 01 ya da 02 (cagiran ekran)      |
| 08 --["[x]" kapanış]------------> 02                               |
| 02 --[Görünüm sayfasi]----------> 03/04/05/06 panel ac/kapa        |
+--------------------------------------------------------------------+
```

Ok aciklamasi: `-->` yonlu geçiş, `+-->` dal, `[...]` tetik. Kutu cizimi yalniz
`+ - |` ile uretilir (G4 / ADR-029).

---

## §2 Giriş / Çıkış tablosu (her ekran icin en az 1 + 1) - checklist soru 8 kaniti

| # | Ekran | Giriş (kaynak -> tetik) | Çıkış (hedef -> tetik) |
|---|---|---|---|
| 1 | `01-start-page` | [GİRİS] uygulama acilisi | `02-main-ide` (son proje cift tık) · `03-solution-explorer` ("Yeni Proje") · `07-settings` ("Ayarlar") |
| 2 | `02-main-ide` | `01-start-page` (cift tık) · `08-ascii-art-viewer` (kapanış) | `03` sol dock · `04` merkez sekme · `05` sağ dock üst · `06` sağ dock alt · `07` ribbon Ayarlar · `08` Dosya menüsü · `01` merkez "Son proje yok" |
| 3 | `03-solution-explorer` | `01-start-page` ("Yeni Proje" tile) · `02-main-ide` (sol dock) | `04-code-editor` (.cs cift tık) |
| 4 | `04-code-editor` | `03-solution-explorer` (.cs cift tık) · `05-ai-chat` ("Dosyaya uygula" onayı) · `06-agent-panel` (görev cift tık) · `02-main-ide` (merkez sekme) | `02-main-ide` (sekme kapanış / kabuga donus) |
| 5 | `05-ai-chat` | `02-main-ide` (sağ dock üst) · `06-agent-panel` (F5 duzeltme) | `04-code-editor` (uygulanan değişiklik) · `06-agent-panel` (görev kuyrugu) |
| 6 | `06-agent-panel` | `02-main-ide` (sağ dock alt) · `05-ai-chat` (görev kuyrugu) | `04-code-editor` (görev cift tık) · `05-ai-chat` (F5 duzeltme) |
| 7 | `07-settings` | `01-start-page` ("Ayarlar" tile) · `02-main-ide` (ribbon Ayarlar) | `01-start-page` ya da `02-main-ide` (Kaydet / Vazgeç - cagiran ekran) |
| 8 | `08-ascii-art-viewer` | `02-main-ide` (Dosya menüsü) | `02-main-ide` ("[x]" kapanış / Alt+F4) |
| 9 | `09-terminal` | `02-main-ide` (alt dock Terminal sekmesi) | `02-main-ide` (panel kapanisi - ekran degismez) |
| 10 | `09-cli-console` | `02-main-ide` (ribbon CLI) | `02-main-ide` (Ctrl+W sekme kapanisi) |
| 11 | `10-ai-chat-console` | `02-main-ide` (AI Chat dock "Sekmede Ac") | `02-main-ide` (Ctrl+W / son sekme ise kabuk) |

**Sayim:** 11 wireframe · girisi olmayan wireframe **0** · cikisi olmayan wireframe **0**.

---

## §3 Geçiş tablosu (tetik bazli)

| # | Geçiş | Tetik | Kaynak (kanit) |
|---|---|---|---|
| 1 | [GİRİS] -> `01-start-page` | uygulama acilisi, ilk ekran | VISION §2.3/1 |
| 2 | `01` -> `02-main-ide` | son proje satirina cift tık | 01 §3/1 · VISION §2.3/4 |
| 3 | `01` -> `03-solution-explorer` | "Yeni Proje" tile | 01 §3/2 |
| 4 | `01` -> `07-settings` | "Ayarlar" tile (LLM / görünüm) | 01 §1 wireframe tile |
| 5 | `02` -> `03-solution-explorer` | sol dock (Solution Explorer) | 02 §0 sol dock satırı |
| 6 | `02` -> `04-code-editor` | merkez XtraTabControl (dosya sekmesi) | 02 §0 merkez üst satırı |
| 7 | `02` -> `05-ai-chat` | sağ dock üst (AI Chat) | 02 §0 sağ dock üst satırı |
| 8 | `02` -> `06-agent-panel` | sağ dock alt (Agent Paneli) | 02 §0 sağ dock alt satırı |
| 9 | `02` -> `07-settings` | ribbon Ayarlar -> modal diyalog | 02 §0 Modal satırı |
| 10 | `02` -> `08-ascii-art-viewer` | Dosya menüsü | 08 §0 giriş · GATE-003 paketi |
| 11 | `02` -> `01-start-page` | merkez "Son proje yok -> 01-start-page" | 02 §1 durumlar |
| 12 | `03` -> `04-code-editor` | .cs düğümüne cift tık (aciksa sekmeye gider) | 03 §3/1 |
| 13 | `04` -> `02-main-ide` | sekme kapanış / kabuga donus | 02 §0 merkez satırı |
| 14 | `05` -> `04-code-editor` | "Dosyaya uygula" onayı (diff + yazim) | 05 §3/3 · VISION §2.7/5 |
| 15 | `05` -> `06-agent-panel` | istek görev kuyruğuna düşer | VISION §2.8/1 |
| 16 | `06` -> `04-code-editor` | görev ağacında cift tık (dosya varsa) | 06 §3/3 |
| 17 | `06` -> `05-ai-chat` | F5 duzeltme yazimi | VISION §2.8/4 |
| 18 | `07` -> `01` / `02` | Kaydet / Vazgeç (modal kapanış) | 07 §3/2, §3/5 |
| 19 | `08` -> `02-main-ide` | "[x]" kapanış | 08 §1 pencere kabuğu #1 |
| 20 | `02` -> `03/04/05/06` | Görünüm ribbon sayfasi (panel ac/kapat) | 02 §3/2 |

---

## §4 Istisnalar ve durumlar

| # | Durum | Davranis |
|---|---|---|
| 1 | `02-main-ide` acilista bos | merkez "Son proje yok" satırı + `01-start-page`'e geçiş (beyaz bos alan yasak) |
| 2 | `07-settings` acilirken cagiran | cagiran ekrani hafizada tutulur; Vazgeç ayni ekrana doner |
| 3 | `08` kucuk pencere (< 900x600) | galeri + özellik paneli gizlenir, tuval kalir; kapanış yine `02`'ye (08 §4/5) |
| 4 | Kirli sekme kapanisi (`04`) | onay diyalogu; vazgeç çıkışı iptal eder (04 §3/2) |
| 5 | Akış iptali (`05`) | "Durduruldu" + Gönder etkin; ekran degismez (05 §1 durumlar) |

---

## §5 Dogrulama

| # | Kontrol | Beklenen | Kanit |
|---|---|---|---|
| 1 | Her wireframe en az 1 giris + 1 cikisi | 11/11 | §2 tablosu (giris ve cikis hucresi dolu) |
| 2 | Ana omurga 01 -> 02 -> 03/04/05/06 -> 07 | §1/A | §3 satır 2, 5-9 |
| 3 | `08` girisi yalniz `02` (Dosya menüsü) | §3 satır 10 | 08 §3/3 (Ctrl+O) + 02 ribbon Dosya |
| 4 | `08` kapanisi `02`'ye doner | §3 satır 19 | 08 §1 pencere kabuğu #1 |
| 5 | Kutu cizim `U+2500-U+257F` | 0 satır | checklist soru 3 taramasi (bu dosya dahil) |

---

## §6 Kanit

| # | Kaynak | Ne kanitlar |
|---|---|---|
| 1 | [[ui-design/readme|01..08 wireframe]] §3 etkileşim kuralları | tetik -> hedef eslesmesi |
| 2 | [[definitions/vision|VISION]] §2.3-§2.9 | ekran akış adimlari |
| 3 | [[checklists/gate-003-ui-onay|gate-003 checklist]] soru 8 | giriş/çıkış zorunlulugu |
| 4 | [[governance/workflow|WORKFLOW]] | flows.md eslikci dosya sarti |
