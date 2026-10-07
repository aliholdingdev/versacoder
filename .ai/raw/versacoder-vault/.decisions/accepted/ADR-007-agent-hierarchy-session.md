---
title: "ADR-007 — Ana Ajan + Alt Ajan: Oturum Hiyerarşisi"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-09-25
authority: SSOT
owner: architect
scope: "ana oturum / alt oturum modeli, yetki devri, derinlik sınırı, oturum adı ve kalıcılık"
hedef_bacak: "net48 + net10.0-windows"
evidence: "AGENTS.md §5 · §6 · §7 · ADR-013 §2 (l5 loop-policy) · ADR-005 (versacoder.db)"
---

# ADR-007 — Ana Ajan + Alt Ajan: Oturum Hiyerarşisi

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-008](ADR-008-transcript-compaction.md) · [ADR-006](ADR-006-opencode-tool-contract.md)

> **Tek cümlelik karar:** Çalışma zamanı **tek bir ana oturum + onun açtığı alt oturumlar** hiyerarşisinde yürür; alt oturum **yetenek devralır ama yetki devralmaz** (gate onayı yok), derinlik **2** ile sınırlıdır — karar verildi.

---

## §0 Özet (Answer First)

VersaCoder'da ajan, dosya değil **oturum** taşır: her oturumda `sessionId`, `role`, `parentSessionId`, transcript ve araç bütçesi vardır. Alt oturum, ana oturumun **sadece gerekli kesitini** alır (≤ 8 KB) ve işi 4 maddelik devre paketiyle geri verir. Alt oturum **gate onaylayamaz, kullanıcıya soru soramaz**; bunlar ana oturumda (inline) kalır. Oturum kaydı `versacoder.db` üzerinde yaşar; transcript boyutu [ADR-008](ADR-008-transcript-compaction.md) ile 8 MB'da kırpılır.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Subagent / inline ayrımı karar ağacıyla tanımlı; 5 tetikleyici var | `AGENTS.md` §5.1–5.2 |
| C2 | Subagent gate atlayamaz, onay veremez, kullanıcıya soru soramaz | `AGENTS.md` §5.2 UYARI |
| C3 | Alt oturum açılış bağlamı ≤ 8 KB; aktif alt oturum ≤ 2 | `AGENTS.md` §5.3 · §6.1 |
| C4 | Oturum adı rol öneklidir (`architect-katman-sozlesmesi`) | `AGENTS.md` §7.1 |
| C5 | Transcript kalıcı ve boyutlu; kırpma kuralı ayrı kararda | [ADR-008](ADR-008-transcript-compaction.md) |
| C6 | Oturum/döngü politikası `l5 loop-policy` düğümünde | [ADR-013](ADR-013-layered-architecture-52-nodes.md) §2 (N36) |
| C7 | Kalıcılık saf SQL + `schema_version` ile (ORM yok) | [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) |

---

## §2 Karar

**Karar:** Oturum modeli **tek ana oturum + en fazla 2 seviye alt oturum** olarak sabitlenir; alt oturum yalnız **yetki** (araç, okuma, yazma) devralır, **onay yetkisi** (GATE-001..006) asla devredilmez — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Hiyerarşi | `root` → `child` (maks. 1 seviye altta); `parentSessionId` zorunlu alan | C1 |
| D2 | Yetki devri | alt oturum = araç + salt-okunur vault bağlamı; **gate onayı yok** | C2 |
| D3 | Açılış paketi | ≤ 8 KB (görev 300 B · dosya kesiti 4 KB · kural 1 KB · çıktı şablonu 1 KB · devre 200 B) | C3 |
| D4 | Paralellik | aynı anda **≤ 2** alt oturum; 3. istek kuyruğa | C3 |
| D5 | Adlandırma | `<rol>-<konu>-<kisaltma>`, ≤ 48 karakter, ASCII/tire | C4 |
| D6 | Kalıcılık | `versacoder.db` `session` + `session_message` tabloları, Dapper ile yazılır | C7 |
| D7 | Dönüş | her alt oturum **4 maddelik devre paketi** ile kapanır; paket eksikse kapanış geçersiz | `AGENTS.md` §4.2 |

**Neden:** Tek kök, **"en güncel bağlam kimde"** sorusunun tek cevabı olmasını sağlar; ikinci bir kök olsa `log.md` ve defter iki doğru üretir (SSUT). 2 seviye sınırı, `AGENTS.md` §6.1 bütçesini korur: derinlik arttıkça devre paketi her katmanda özetlenir ve bilgi kaybı üstel büyür. Onayın devredilmemesi G12'yi yapısal olarak imkânsız kılar — alt oturumun gate'i geçecek yetkisi **yoktur**.

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Tek bağlam sahibi → "kimde kaldık" cevabı tek satır | `session.parentSessionId` sorgusu |
| Bütçe denetlenebilir (≤ 8 KB açılış, ≤ 2 paralel) | `AGENTS.md` §6.1 sayısal limitler |
| Gate yetkisi yapısal olarak devredilemez | D2 + `AGENTS.md` §5.2 UYARI |
| Oturumlar `log.md` ile eşleşir | ad = iş kimliği (`AGENTS.md` §7.2) |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Devre paketi her geçişte bilgi kaybedebilir | paket 4 madde zorunlu; eksik = geri çevirme |
| Derinlik 2, bazı işler için dar gelebilir | daralan iş **inline**'a alınır (`AGENTS.md` §5.2/5) |
| Alt oturum kullanıcıya soramaz → belirsizlikte bekler | ana oturum soruyu üstlenir; bekleme raporlanır |
| `session_message` tablosu büyüyorsa sorgu yavaşlar | [ADR-008](ADR-008-transcript-compaction.md) 8 MB kırpma + indeks |

**NEEDS_VALIDATION:** `session` / `session_message` şeması henüz diske yazılmadı; migration dosyası `ADR-005` kalıbıyla (`.sql/migrations/NNN-*.sql` + `schema_version`) PART 6'da üretilir ve `dotnet test` ile kanıtlanır.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-008](ADR-008-transcript-compaction.md) | related | transcript boyutu ve kırpma kuralı orada |
| [ADR-006](ADR-006-opencode-tool-contract.md) | related | alt oturumun kullandığı araç sözleşmesi orada |
| [ADR-009](ADR-009-tool-execution-sandbox.md) | related | alt oturumun araç yetkilerinin sınırı orada |
| [ADR-010](ADR-010-cli-desktop-shared-core.md) | related | oturum çekirdeği iki host tarafından ortak kullanılır |
| [ADR-005](ADR-005-ado-net-dapper-no-ef-core.md) | related | kalıcılık yolu (Dapper + saf SQL migration) |

---

## §5 Kabul Kriterleri

- [ ] Oturum modeli `l5 protocol` içinde tip olarak tanımlı, iki bacakta derleniyor
- [ ] `parentSessionId` zorunlu; 2. seviye açılışı **reddedilen** testi var
- [ ] Alt oturum gate çağrısı **özellikle** test edilmiş (erişim reddi)
- [ ] Açılış paketi boyutu kod içinde ölçülebilir ve > 8 KB ise uyarı veriyor
- [ ] `versacoder.db` üzerinde `session` migration'ı yeşil `dotnet test` ile kanıtlı

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: master prompt PART 7 + `AGENTS.md` §5–§7.
