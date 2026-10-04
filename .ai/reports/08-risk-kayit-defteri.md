---
title: "VersaCoder — Risk Kayıt Defteri (R1..R10)"
type: report
category: reports
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "R1..R10 risk tablosu: kaynak cakismalari (7 vs 16 wireframe dahil), etki, sahip, azaltma ve kapanis kaniti"
---

# VersaCoder — Risk Kayıt Defteri (R1..R10)

**Zorunlu bağlantılar:** [index](../index.md) · [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [keys](../keys.md) · [00-kapsam-analizi](00-kapsam-analizi.md) · [01-web-kaynaklari](01-web-kaynaklari.md)

> **Tek cümlelik sorumluluk:** *Hangi risk açık, kimin sorumluluğunda ve hangi kanıtla kapanır* sorusunun tek cevabıdır.

---

## §0 Özet (Answer First)

**10 riskten 3'ü kapandı (R2 · R3 · R5), 1'i kısmi (R1), 6'sı açık.** En kritik açık madde **R1**: ekran listesi çakışması — master prompt **7 ekran** (02-main-ide …) ile `ui-design/README.md`'deki **farklı 7 ad** (02-project-new, 05-tasks, 06-terminal …) uyuşmuyor; 16 yedek wireframe ise zaten ekran sayılmıyor (00-kapsam karar 8). Ardından **R4** (500 MD hedefi: 216/500), **R6** (46 bozuk link), **R7** (9 mojibake dosya) ve **R9** (ADR-006..011 eksik) geliyor. Her satırda tek bir sahip var; kapanış yalnız kanıtla yapılır.

---

## §1 Risk Tablosu

| ID | Risk | Etki | Sahip | Azaltma / Aksiyon | Kapanış | Durum |
|---|---|---|---|---|---|---|
| **R1** | **Ekran listesi çakışması:** master prompt 7 ekran ≠ `ui-design/README.md` 7 ad ≠ yedek vault 16 WF | **Yüksek** | ui-designer | README'yi master prompt'a göre düzelt; 16 WF yalnız kontrol listesi referansı | `03-ui-design-raporu.md` §2 + README diff | 🟡 kısmi (karar verildi, README düzelmedi) |
| **R2** | `ai.03.md` **CR-only** satır sonu → parse hatası | Orta | architect | okuma anında normalize; diske CR-only yazılmaz | `measure-vault` → **CROnly 0** | ✅ kapandı |
| **R3** | ai.01'den **EF Core / DevExpress 24.2** sızıntısı | Yüksek | architect | `.rules/` 9 dosyada hard rule (G2/G3) | `.rules/data-access-rules.md` + `devexpress-rules.md` | ✅ kapandı (kod taraması açık) |
| **R4** | **~500 MD hedefi** zaman aşımı | Yüksek | docs (batch B1..B11) | `.templates/` kalıbı bir kez yaz, dağıt | `measure-vault` TotalMD ≥ 500 | 🔴 açık (ölçüm: 216/500) |
| **R5** | net48 + DevExpress v26.1 uyumluluğu | Düşük | architect | WEB GATE kaynağı | `01-web-kaynaklari.md` §2 | ✅ kapandı |
| **R6** | **Bozuk iç link** (1000+ link, 46 kırık) | Orta | ilgili batch sahibi (34 knowledge · 4 checklists · 2 playbooks) | `check-links.ps1` her batch sonunda | link = **0** | 🔴 açık |
| **R7** | **Mojibake 9 dosya** + `measure-vault.ps1` ANSI parse hatası | Orta | docs | `measure-vault-u8.ps1`; 9 dosyayı UTF-8 onar | `find-mojibake` = 0 | 🔴 açık |
| **R8** | `Microsoft.Data.Sqlite` net48 + net10 ikili desteği | Orta | developer | PART 5'te NuGet sayfasından teyit | NuGet kaydı → ADR-012 eki | 🔴 açık (`NEEDS_VALIDATION`) |
| **R9** | **ADR-006..011 yazılmadı** (PART 7 sözleşmeleri) | Yüksek | architect | 6 ADR, sırayla; `spec/` dosyaları hazır | `.decisions/accepted/ADR-006..011` | 🔴 açık |
| **R10** | **İlk git commit yok** (+ kökte yabancı `aaa.md`) | Orta | developer | commit yalnız GATE-004'te; `aaa.md` kapsam dışı | `git log` ilk commit | 🔴 açık |

---

## §2 Kaynak Çakışması Özeti (R1 ayrıntısı)

| Kaynak | Ekran sayısı | Adlar | Statü |
|---|---|---|---|
| Master prompt | **7** | `01-start-page` · `02-main-ide` · `03-solution-explorer` · `04-code-editor` · `05-ai-chat` · `06-agent-panel` · `07-settings` | ✅ **SSOT** |
| `ui-design/README.md` | 7 (farklı) | `02-project-new` · `03-editor` · `04-agent-chat` · `05-tasks` · `06-terminal` | ❌ düzeltilmeli |
| Yedek vault `WF-00..WF-15` | 16 | ana ekran, chat, wizard, session, agent, skill, memory … | yalnız **bilgi** |
| ai.01 BÖLÜM 12 | 8 | — | reddedildi (00-kapsam C4) |

---

## §3 Kural

| # | Kural | Yer |
|---|---|---|
| 1 | Risk kapanışı **yalnız kanıtla** (çıkış / dosya / ölçüm) | CLAUDE.md G5 |
| 2 | Kapanan risk satırı **silinmez**, ✅ + kanıt yazılır | bu dosya |
| 3 | Yeni risk → `00-kapsam-analizi.md` §3 ile birleştirilir, çelişki varsa `CONFLICT DETECTED` | ROLE.md §3 |
| 4 | Etki "Yüksek" olan açık risk → gate kapanışında **önce** kapatılır | WORKFLOW §3 |
| 5 | Sahiplik devri 4 maddelik devre paketiyle yapılır | AGENTS.md §4 |

---

## §4 Sayım

| Durum | Adet | ID'ler |
|---|---|---|
| ✅ kapandı | 3 | R2 · R3 · R5 |
| 🟡 kısmi | 1 | R1 |
| 🔴 açık | 6 | R4 · R6 · R7 · R8 · R9 · R10 |
| **Toplam** | **10** | R1..R10 |
