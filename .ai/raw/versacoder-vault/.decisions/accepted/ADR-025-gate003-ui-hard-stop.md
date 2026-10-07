---
title: "ADR-025 — UI Hard Stop: Wireframe Onayı Olmadan Kod Yazilmaz"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "GATE-003 wireframe onayi ile kod yazim arasindaki kilit: onayli tel kafes yoksa .Form/.Designer.cs/.resx yazimi yasak"
hedef_bacak: "vault + kod (net48 + net10.0-windows)"
evidence: "CLAUDE.md G12 · WORKFLOW.md GATE-003 · ADR-004 (DevExpress v26.1 + Office2010) · ADR-013 (52 dugum)"
---

# ADR-025 — UI Hard Stop: Wireframe Onayı Olmadan Kod Yazılmaz

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [keys](../../keys.md) · [ADR-022](ADR-022-git-commit-gate004.md) · [ADR-004](ADR-004-devexpress-universal-v261-office2010.md)

> **Tek cümlelik karar:** **7 ekranın ASCII wireframe'i (`ui-design/NN-<ekran>.md`) kullanıcı tarafından GATE-003'te yazılı onaylanmadan hiçbir `.Form.cs` / `.Form.Designer.cs` / `.Form.resx` dosyası yazılmaz** — onay yoksa kod yazımı durur, kapı kapalıdır.

---

## §0 Özet (Answer First)

Form kodunu önce yazmak, ekran kararını **geri alınamaz bir maliyete** çevirir: wireframe değiştiğinde `.Designer.cs` + `.resx` + olay bağlayıcıları çöpe gider, `ResourceManager` adları kirlenir, review iki kez yapılır. Bu ADR kapıyı kilitler: `ui-design/NN-*.md` → **GATE-003 (kullanıcı onayı)** → yalnız sonra Form üçlüsü. Onay **kullanıcıya** aittir, hiçbir ajan ve subagent onay veremez (protokol §5.2 #7). İhlal G12'dir: iş RED olur, part baştan planlanır.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | 7 ekran tanımlı (`VISION.md` / `PROJECTS.md`) | `reports/00-kapsam-analizi.md` §4.1 |
| C2 | Ribbon `RibbonForm` + `RibbonControlStyle.Office2010` tek stil | [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) |
| C3 | `ui-designer` = `.Designer.cs`/`.resx` X sahibi; `developer` = iş mantığı | `.agents/AGENTS.md` §2.1 |
| C4 | Wireframe ASCII; Mermaid / Unicode kutu yok | ADR-020 |
| C5 | Onaysız kod = yeniden yazım riski (G12) | `CLAUDE.md` G12 |
| C6 | `.Designer.cs` elle yazılmaz, `.resx` designer üzerinden üretilir | `.agents/ui-designer.md` |

**Sorun:** Kapı kilitli değilse `developer`, wireframe okumadan başlar; sonuçta **iki gerçek** (kâğıt ekran ≠ kod ekran) doğar ve hangisinin doğru olduğu tartışması gate dışına taşar.

---

## §2 Karar

**Karar:** GATE-003 **HARD STOP** olarak uygulanır; onaylı wireframe yoksa kod yazımı durur — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Ön koşul | `ui-design/NN-<ekran>.md` + yazılı kullanıcı onayı | C1 · C5 |
| D2 | Onay yetkisi | **kullanıcı**; ajan/subagent onay **veremez** | protokol §5.2 #7 |
| D3 | Sıra | wireframe → GATE-003 → `.Form.cs` → `.Designer.cs` → `.resx` → olay bağlayıcı | C3 · C6 |
| D4 | Kontrol seti | DevExpress **v26.1** dışına çıkılmaz | C2 · [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) |
| D5 | Şekil | ASCII tel kafes (ADR-020); Unicode kutu yok | C4 |
| D6 | İhlal sonucu | iş **RED**, part baştan; kod çöpe yazılır | C5 |
| D7 | Devir | onay sonrası `ui-designer → developer`, 4 maddelik paket | protokol §4.2 |

**Neden:** Geri dönüş maliyeti kod yazıldıkça artar. `.Form.Designer.cs` yalnız `InitializeComponent` içerir ama `.resx` adları, layout sabitleri ve olay bağlayıcılarıyla **düğüm düğüm** bağlıdır; tek bir ekran kararı değişince tüm üçlü yeniden yazılır. Kilidin **kapıda** olması (sonra değil) maliyeti sıfıra yaklaştırır.

---

## §3 Alternatifler

| # | Alternatif | Tanım | Neden seçilmedi |
|---|---|---|---|
| A1 | **(SEÇİLDİ)** Kapıda kilit + yazılı onay | GATE-003'te kullanıcı onayı olmadan kod yok | — (karar bu) |
| A2 | Wireframe'e sonra dön (kod önce) | Hızlı başlangıç, wireframe kod yazılırken çizilir | C5 ihlali: değişiklik maliyeti kodla büyür, review iki kez |
| A3 | Otomatik onay (varsa dosya = onaylı) | `ui-design/*.md` varlığı tek başına yetsin | Onay yetkisi kullanıcıda (D2); dosya varlığı onay değildir |
| A4 | Kısmi onay (1-2 ekranla başla) | İlk ekranda başlat, gerisini paralel | 7 ekran tek Ribbon iskeletinde; parçalı onay layout çatışması üretir |

**Ret gerekçesi (A2–A4):** A2 maliyeti ertelemez, büyütür. A3 onay kavramını dosya varlığına indirger ve G12'yi anlamsızlaştırır. A4, `RibbonForm` client-area yerleşiminin tek parça olmasını bozar (ADR-013 katmanlarıyla da çakışır).

---

## §4 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Ekran değişikliği kod yazılmadan kapanır | D1 ön koşulu |
| `ui-designer` ↔ `developer` yetki sınırı netleşir | C3 · `.agents/AGENTS.md` §2.2 |
| Kontrol seti sürüm sapması üretmez (v26.1 + Office2010) | D4 · ADR-004 |
| Review tek turda kapanır (kâğıt ≠ kod çatışması yok) | D6 |
| ASCII wireframe diff'lenebilir, git'te okunur | D5 · ADR-020 |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Kapı tıkanır: kullanıcı onayı beklenir → iş durur | Onay talebi tek dosyada toplanır, 7 ekran **tek** turda sunulur |
| Wireframe yazımı kod yazımını geciktirir | `ui-designer` erken başlar; `developer` bu sırada Application katmanını kodlar |
| Onay kâğıt üstünde kalabilir | D6: onay kaydı rapora + `log.md`'ye yazılır, kanıtsız sayılmaz |
| Onay sonrası küçük değişiklik (renk/marj) yeniden kapı açtırır | **LOW** nitelikteki değişiklikler kapı dışındadır; yapı değişikliği kapıdadır |

**NEEDS_VALIDATION:** Onayın **şekli** (rapor satırı mı, `log.md` kaydı mı, imzalı checklist mi) yazılmadı → GATE-003 uygulama şablonunda teyit edilir.

---

## §5 Kabul Kriterleri

- [ ] 7 `ui-design/NN-<ekran>.md` dosyası frontmatter'lı ve ASCII tel kafes içeriyor
- [ ] Her ekran için yazılı onay kaydı raporda (tarih + ekran adı)
- [ ] Onaysız hiçbir `*.Form.Designer.cs` / `*.Form.resx` üretilmemiş (`git status` ile)
- [ ] Kontrol listesi v26.1 dışına çıkmıyor, stil = `RibbonControlStyle.Office2010`
- [ ] Devir paketi 4 maddeyle `developer`'a iletildi

---

## §6 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-004](ADR-004-devexpress-universal-v261-office2010.md) | uygulanır | kontrol seti ve ribbon stili kilidi (D4) |
| [ADR-013](ADR-013-layered-architecture-52-nodes.md) | related | ekranın ait olduğu katman ve bağımlılık yönü |
| [ADR-020](ADR-020-no-mermaid-only-box-drawing.md) | uygulanır | wireframe'in ASCII şekli (D5) |
| [ADR-022](ADR-022-git-commit-gate004.md) | related | bu onay GATE-004'ün ön koşuludur |
| [ADR-027](ADR-027-component-catalog-5000.md) | related | kontrol kataloğu, wireframe kontrol listesinin havuzu |

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `CLAUDE.md` G12 + `WORKFLOW.md` GATE-003.
