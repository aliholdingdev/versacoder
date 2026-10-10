---
title: "TODOS Faz 04 — GATE-003: 7 Wireframe Kullanıcı Onayı (HARD STOP)"
type: todo
category: todos
status: blocked
version: 2.0.0
updated: 2026-10-07
authority: reference
scope: "7 wireframe'in kullaniciya sunulup onaylanmasi; onaysiz tek satir .cs yazilmaz — sunum sirasi ve onay kaydi"
tags: [ops]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

# Faz 04 — GATE-003: 7 Wireframe Onayı

**Zorunlu Bağlantılar:** [[ops/TODOS/index|TODOS index]] · [[ui-design/readme|ui-design README]] · [[checklists/gate-003-ui-onay|gate-003-ui-onay]] · [[ops/TODOS/03-faz-d-iskelet|Faz 03]] · [[definitions/vision|VISION]] · [[.rules/devexpress-rules|devexpress-rules]] · [[governance/workflow|WORKFLOW]]

> **Tek cümlelik sorumluluk:** 7 ekranın hangi sırayla sunulup neyin onaylanacağının ve

### §0.1 Güncel Durum (2026-10-07 — ölçülmüş)

**Durum: TEYIT GEREKLI — ui-design/ 14 MD var, onay kaydi logda**

| Gösterge | Değer | Hedef | Durum |
|---|---|---|---|
| ui-design/ MD | **14** | 7+ ek | OK — dosyalar mevcut |
| ui-design/README.md | VAR | VAR | OK |
| log GATE-003 ONAY kaydi | VAR (2026-10-01, 7+1 onay) | tarihli onay | TEYIT — gate durumu yeniden dogrulanmali |
| checklists/gate-003-ui-onay.md | **VAR** (13 madde) | VAR | OK — 2026-10-07 uretildi |
| src/ altinda .cs | **0** | 0 (onay oncesi) | OK — G12 temiz |

> LOG CELISKISI: log 2026-10-01 "GATE-003 KAPANDI 13/13" der; diskte ui-design dosyalari VAR ama gate durumu bu oturumda yeniden dogrulanmadi -> ONAYI YENIDEN TEYIT ET.

> **Kaynak:** bu blok 2026-10-07 disk ölçümünden gelir (os.walk + git + dotnet); eski tarihli ölçümler tarihleriyle korunmuştur (append-only).

> onay verilmeden **hiçbir `.cs` yazılmayacağının** tek listesidir.

**Durum: KAPALI** (03 iskelet biterse açılır). **HARD STOP — en pahalı kapı.**

---

## §0 Özet (Answer First)

Onay paketi **hazır**: `ui-design/` 8/8 (README + 7 wireframe, 5,1-6,4 KB).
Eksik olan **yalnız kullanıcı onayı**. Onay yoksa `VersaCoder.Desktop` içinde tek
satır bile yazılmaz — bu kural [[governance/workflow|WORKFLOW]] §2 ile aynı ve
ihlali tüm kodu başa döndürür (2x iş).

| # | Dosya | Ekran | Onay |
|---|---|---|---|
| 1 | [[ui-design/01-start-page|01-start-page]] | karşılama + son projeler | ⏳ |
| 2 | [[ui-design/02-main-ide|02-main-ide]] | ana pencere kabuğu | ⏳ |
| 3 | [[ui-design/03-solution-explorer|03-solution-explorer]] | çözüm ağacı | ⏳ |
| 4 | [[ui-design/04-code-editor|04-code-editor]] | kod editörü | ⏳ |
| 5 | [[ui-design/05-ai-chat|05-ai-chat]] | ajan sohbeti | ⏳ |
| 6 | [[ui-design/06-agent-panel|06-agent-panel]] | görev paneli | ⏳ |
| 7 | [[ui-design/07-settings|07-settings]] | ayarlar diyaloğu | ⏳ |

---

## §1 Sunum Sırası (akışa göre — 01 → 07)

| # | Ne gösterilir | Odak sorusu | Süre |
|---|---|---|---|
| 1 | Kabuk ortak bölgeleri (README §3) | ribbon/dock/statusbar düzeni uygun mu? | 2 dk |
| 2 | 01 → 02 → 03 (giriş yolu) | açılış + kabuk + ağaç akışı mantıklı mı? | 5 dk |
| 3 | 04 → 05 → 06 (üçlü) | editör/chat/panel bölünmesi doğru mu? | 5 dk |
| 4 | 07 (modal) | ayarlar ayrı diyalog olarak kabul mü? | 2 dk |
| 5 | Dosya üçlüsü + U5 sahipliği (README §2) | `.Designer.cs` sahipliği ui-designer'da | 1 dk |

- Her ekran için **3 seçenek**: `[onay]` / `düzeltme iste (madde yaz)` / `reddet (yeniden çiz)`.
- Toplu "hepsi tamam" kabul edilmez — **satır satır** onay (kör onay = geç RED).

---

## §2 Onay Sonrası İş

| # | Adım | Çıktı |
|---|---|---|
| 1 | Onayları `gate-003-ui-onay.md`'ye işle | 7/7 işaret |
| 2 | `ui-design/README.md` §4 tablosunu güncelle | durum `onaylı` |
| 3 | `log.md`'ye tarihli kayıt | onay kanıtı |
| 4 | Faz 05 (kod) açılır | Desktop koduna izin |

- **Düzeltme gelen ekran** için: ui-designer yeni wireframe çizer → 03'e değil,
  **yalnız o ekrana** geri dönülür (sıra korunur).

---

## §3 Kabul Kriterleri

- [ ] 7/7 ekran satır satır onaylanmış
- [ ] [[checklists/gate-003-ui-onay|gate-003-ui-onay]] tümü [x]
- [ ] Reddedilen/ düzeltme gelen varsa yeniden çizim + tekrar onay
- [ ] `log.md`'de tarihli onay kaydı
- [ ] Onaydan **önce** yazılmış `.cs` yok (tarama: `src/VersaCoder.Desktop/*.cs` = yalnız iskelet)

---

## §4 Yasaklar

| # | Yasak | Ne olur |
|---|---|---|
| 1 | Onaysız `Desktop` kodu | GATE-003 ihlali, kod silinir (2x iş) |
| 2 | `.Designer.cs`'i developer'ın eliyle değiştirmesi | U5 ihlali, onay zinciri kopar |
| 3 | Toplu/koşulsuz onay | geç RED; tekrar sunum |
| 4 | Kod içinde sabit string (resx yerine) | `.resx` boş kalır, review RED |

---

## §5 Kanıt

| # | Kanıt | Beklenen |
|---|---|---|
| P4-1 | `gate-003-ui-onay.md` | 7/7 [x] |
| P4-2 | `ui-design/README.md` §4 | durum sütunu `onaylı` |
| P4-3 | `log.md` | tarih + ekran adları |
| P4-4 | `src/` tarama | onaysız kod 0 satır |
