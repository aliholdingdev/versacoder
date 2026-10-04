---
title: "ADR-009 — Tool Çalıştırma: Timeout, CancellationToken, İzin Modeli"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "araç yürütücüsünün süre sınırı, iptal yayılımı, çalışma klasörü hapsi ve izin/onay modeli"
---

# ADR-009 — Tool Çalıştırma: Timeout, CancellationToken, İzin Modeli

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-006](ADR-006-opencode-tool-contract.md) · [ADR-022](ADR-022-git-commit-gate004.md)

> **Tek cümlelik karar:** Her araç çağrısı **varsayılan 60 sn timeout** (`bash` için 300 sn), **iptal edilebilir `CancellationToken`**, **çalışma klasörü = çalışma alanı kökü** ve **açık izin listesi** altında çalışır; yıkıcı işlem **onaysız yürütülmez** — karar verildi.

---

## §0 Özet (Answer First)

`bash` aracı işletim sistemine kapı açar; kapı sınırsız olursa tek kötü çağrı makineyi kilitleyebilir. Bu ADR üç katman kurar: **süre** (timeout), **iptal** (`CancellationToken` yayılımı + süreç ağacının sonlandırılması), **izin** (okuma / çalışma alanı yazma / yürütme / kapalı ağ + onay eşiği). Varsayılan güvenli olandır: izin verilmeyen işlem **reddedilir**, açık onay ister. SQL tarafındaki süre zaten zorunludur (`CommandTimeout`, [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md)); bu ADR aynı disiplini **süreç** tarafına taşır.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | `bash` aracı LLM çıktısını işletim sistemine geçirir → doğrulanmamış sınırdır | `AGENTS.md` §8.1 · [ADR-006](ADR-006-opencode-tool-contract.md) D2 |
| C2 | Yıkıcı işlem (`Remove-Item -Recurse`, `git reset`) onay ister | `CLAUDE.md` G7 · G11 |
| C3 | Vault dışı yazım tespit edilir (`C:\www\` altına yazılmış dosya = ihlal) | `CLAUDE.md` §denetim G1 |
| C4 | Aynı hatada 3 deneme sonrası durulur; sonsuz bekleme bu döngüyü kilitler | `AGENTS.md` §8.1 |
| C5 | SQL komutlarında süre açıkça set edilmek zorunda | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) §karar 2 |
| C6 | Gate sırası bozulmadan kod yazılmaz; ilk commit GATE-004'tedir | [ADR-022](ADR-022-git-commit-gate004.md) |

---

## §2 Karar

**Karar:** Araç yürütücüsü **timeout + iptal + çalışma alanı hapsi + izin listesi** dörtlüsüyle sınırlandırılır; onay gerektiren yıkıcı işlem, onay gelmeden **hiçbir koşulda** yürütülmez — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Varsayılan timeout | 60 sn (`read`/`grep`/`glob`/`list`: 10 sn · `bash`: **300 sn**) | C4 |
| D2 | İptal | her `ExecuteAsync` `CancellationToken` alır; iptal → süreç **ağaç birlikte** sonlandırılır, sonuç `Cancelled` döner | C4 |
| D3 | Çalışma klasörü | süreç `CancellationToken` + **çalışma alanı kökü** içinde doğar; kök dışı yazma `ToolResult.Success=false` | C3 |
| D4 | İzin listesi | 4 klasör: `read` (varsayılan) · `write-workspace` · `execute` · `network-off` (varsayılan kapalı) | C1 |
| D5 | Onay eşiği | çalışma alanı dışı silme · `git reset`/`push`/`commit` · kök dizin değişikliği → **onay ister** (G7/G11) | C2 · C6 |
| D6 | Uzun iş | timeout'a düşen iş **arka plan görevi** olarak taşınmaz; yeniden planlanır (`AGENTS.md` §8.1) | C4 |
| D7 | Denetim | her yürütme: araç · argüman özeti · süre · sonuç kodu → structured log (bkz. [ADR-026](ADR-026-logging-structured.md)) | C5 |

**Neden:** Timeout tek başına yeterli değildir: timeout olduktan sonra torun süreç hayatta kalırsa (kabuk boru hattı) kilitlenme devam eder, bu yüzden **ağaç sonlandırma** D2'de açıkça yazılır. İzin modelinin varsayılanı **kapalı** olması, G1 ihlalini (vault dışı yazım) kaynağında engeller; `CLAUDE.md` bunu **sonradan** yakalar, bu ADR **önceden** engeller. Onay eşiğinin GATE-004 ile ilişkilendirilmesi, ilk commit öncesi geri alınamaz işlem riskini kapatır.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Sonsuz süreç kilitlenmesi kapalı | timeout testi: 900 sn'lik `bash` → 300 sn'de `Cancelled` |
| Vault dışı yazma önceden engellenir | çalışma alanı hapsi testi (D3) |
| Yıkıcı işlem onaysız yürütülemez | onay verilmeyen çağrıda red testi (D5) |
| Denetlenebilirlik | her çağrı structured log satırı (D7) |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| 300 sn'lik meşru uzun derleme/indirme kırılabilir | timeout **araç başına** yapılandırılabilir; varsayılan değişmez |
| Ağaç sonlandırma, `-NoProfile` çocuğu dahil her şeyi öldürür | yalnız o ağaca ait süreçler (PID tabanlı) |
| Onay eşiği ajan döngüsünü durdurur (bekleme) | bilinçli: `AGENTS.md` §5.2 gereği gate/onay inline'da kalır |
| Ağ varsayılan kapalı → `bash` ile paket indirme çalışmaz | `execute` izni + G8 onayı ile açılır |

**NEEDS_VALIDATION:** `net48` bacağında süreç ağacının güvenilir sonlandırılması (imza/API farklılığı) **PART 5'te** iki bacak testiyle doğrulanır; doğrulanmazsa D2'de yalnız `net10.0-windows` için ağaç sonlandırma, `net48` için PID sonlandırma fallback'i tanımlanır.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-006](ADR-006-opencode-tool-contract.md) | related | bu ADR, sözleşmenin **çalışma zamanı** kısmını tamamlar |
| [ADR-007](ADR-007-agent-hierarchy-session.md) | related | izinlerin hangi oturumda geçerli olduğu |
| [ADR-026](ADR-026-logging-structured.md) | related | yürütme denetiminin log biçimi (l3) |
| [ADR-022](ADR-022-git-commit-gate004.md) | related | `git` komutlarının onay eşiği ve gate sırası |
| [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) | related | süre disiplininin SQL tarafındaki emsali (`CommandTimeout`) |

---

## §5 Kabul Kriterleri

- [ ] Timeout değeri adlandırılmış sabittir; araç başına override edilebilir
- [ ] `CancellationToken` → süreç iptali testi **her iki bacakta** yeşil
- [ ] Çalışma alanı dışı yazma **reddedilen** testi var
- [ ] Onaysız yıkıcı komut testi: işlem **hiç başlamıyor**
- [ ] Her yürütme için 1 structured log satırı (biçim [ADR-026](ADR-026-logging-structured.md))

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: master prompt PART 7 + `CLAUDE.md` G7/G11.
