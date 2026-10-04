---
title: "ADR-023 — opencode Referans Kopyası Salt Okunur, Kod Kopyası Yok"
type: adr
category: decisions
status: accepted
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "%TEMP%\\opencode-fresh referans ağacının statüsü: salt okunur, yalnız yapı bilgisi, kod kopyası yasağı"
---

# ADR-023 — opencode Referans Kopyası Salt Okunur, Kod Kopyası Yok

**Zorunlu Bağlantılar:** [index](../index.md) · [adr-index](../adr-index.md) · [CLAUDE.md](../../CLAUDE.md) · [ADR-002](ADR-002-handwritten-content-no-copypaste.md) · [ADR-006](ADR-006-opencode-tool-contract.md)

> **Tek cümlelik karar:** `%TEMP%\opencode-fresh` altındaki taze referans ağacı **salt okunurdur**: yalnız **yapı/terim** öğrenilir, **kod/metin kopyalanmaz**, klasör `C:\versacoder` içine taşınmaz/değiştirilmez — karar verildi.

---

## §0 Özet (Answer First)

Referans bir ağacı okumak faydalıdır; ondan **satır almak** yasağı (G1) ihlalidir. Bu ADR üç sınır koyar: **(1)** erişim **salt okunur** (hiçbir `write`/`edit` çağrısı hedefte değil), **(2)** alınan yalnız **yapı ve terim** (hangi klasör var, hangi kavram kullanılıyor), **(3)** üretilen içerik **sıfırdan yazılır** (ADR-002). Araç sözleşmemizi ([ADR-006](ADR-006-opencode-tool-contract.md)) onların tool setinden **esinlenerek** kurarız, ama implementasyonu kendimiz yazarız — `ITool` kodu oradan **gelmez**.

---

## §1 Bağlam

| # | Gerçek | Kanıt |
|---|---|---|
| C1 | Başka vault'tan kopyalama yasağı; tek kelime kopya = reddedilir | `CLAUDE.md` G1 · [ADR-002](ADR-002-handwritten-content-no-copypaste.md) |
| C2 | Yasaklı/salt-okunur referans yalnız **yapı** referansıdır | [ADR-024](ADR-024-coremusic-vault-immutable.md) |
| C3 | opencode tool seti bizim sözleşme kararımızın girdisi | [ADR-006](ADR-006-opencode-tool-contract.md) C1–C2 |
| C4 | Vault MD'lerinde kopya taraması yapılır (kalıp cümle kontrolü) | [ADR-002](ADR-002-handwritten-content-no-copypaste.md) |
| C5 | Referans ağaç geçici dizindedir; sistem temizliği onu silebilir | `C:\Users\...\Temp` davranışı |
| C6 | Yazma yetkisi klasör bazlı; okuma serbesttir | `AGENTS.md` §3.1 (dosya yetki matrisi) |

---

## §2 Karar

**Karar:** `%TEMP%\opencode-fresh` **salt okunur referans**tır; yalnız yapı/terim öğrenilir, **kod ve metin kopyalanmaz**, hedefe **hiçbir yazma** yapılmaz — karar verildi.

| # | Karar | Değer | Dayanak |
|---|---|---|---|
| D1 | Erişim | **salt okunur**: `read`/`grep`/`glob` serbest; `write`/`edit` **yasak** | C6 |
| D2 | Alınan | yalnız **yapı + terim** (klasör, dosya rolü, kavram adı) | C1 |
| D3 | Alınmayan | kod, cümle, prompt metni, README paragrafı, config değeri | C1 · C4 |
| D4 | Üretim | her içerik **sıfırdan** yazılır (ADR-002) | C1 |
| D5 | Taşınma | referans ağacı `C:\versacoder` içine **kopyalanmaz**, linklenmez | C1 · C2 |
| D6 | Ömürlülük | ağaç silinirse **iş kaybı yoktur** (hiçbir şey ona bağlı değil) | C5 |
| D7 | İlişki | tool sözleşmesi **esinlenmedir**; `ITool` implementasyonu özgün | C3 |

**Neden:** G1'in ölçümü **cümle benzerliğidir**; en kolay ihlal yolu da "bir dosyayı olduğu gibi almak"tır. Salt okunur statü, okumayı **serbest**, almayı **yasağı** bırakır — bu, ekibe hangi davranışın kabul edileceğini tek satırda söyler. D5, referansın repoda ikinci gerçek kaynağına dönüşmesini engeller (aynı klasörde olmak, kopyalama değildir ama kopyalamayı kolaylaştırır).

---

## §3 Sonuç

**Artı (+)**

| Artı | Kanıt / ölçüt |
|---|---|
| Kopya riski kaynağında kapalı | `write`/`edit` hedefte yok; MD kopya taraması 0 |
| Yapı öğrenmesi korunur | klasör/terim çıkarımı serbest |
| Referans kaybı zararsız | D6: hiçbir çıktı ona bağlı değil |
| Özgün implementasyon | `ITool` kodu bizim yazımımız (ADR-006 kabul kriteri) |

**Eksi (−) / Riskler**

| Eksi | Azaltım |
|---|---|
| Referans geçici dizinde; bir gün silinir | D6 + not düşülür; yeniden indirilebilir |
| "Yalnız yapı" sınırı yorumlanabilir | D2/D3 listesi net; şüphe varsa `reviewer` RED |
| Referansı okumak extra bağlam yakar | `AGENTS.md` §6 bütçesi; kesit oku |
| Esinlenme ile kopya sınırı bazen bulanık görünür | cümle/kod **kopyası** = G1; kavram paylaşımı serbest |

**NEEDS_VALIDATION:** `%TEMP%\opencode-fresh` ağacının **henüz mevcut olup olmadığı ve nereye indirildiği** doğrulanmadı (`NEEDS_VALIDATION`) → ilk kullanımda yol rapora yazılır. Yol ağaç olmasa bile bu ADR'in **kuralı** geçerlidir: herhangi bir dış referans aynı statüdedir.

---

## §4 İlgili ADR'ler

| ADR | İlişki | Ne alır / ne verir |
|---|---|---|
| [ADR-002](ADR-002-handwritten-content-no-copypaste.md) | extends | kopya yasağının **dış referans** uygulaması |
| [ADR-006](ADR-006-opencode-tool-contract.md) | related | tool setinden **esinlenme**, kod kopyası değil |
| [ADR-024](ADR-024-coremusic-vault-immutable.md) | related | aynı statüdeki ikinci referans vault |
| [ADR-021](ADR-021-vault-ssot-single-index.md) | related | dış navigasyon listeleri vault'a taşınmaz |
| [ADR-022](ADR-022-git-commit-gate004.md) | related | depoya giren içeriğin kaynağı kontrolü |

---

## §5 Kabul Kriterleri

- [ ] Referans yolu rapora yazılmış (ya da `NEEDS_VALIDATION` işaretli)
- [ ] `git status` içinde referans kökenli dosya **yok**
- [ ] Vault MD'lerinde dış vault'tan alınmış cümle **0** (ADR-002 taraması)
- [ ] `ITool` kaynağı özgün yazım (kod incelemesi)
- [ ] `write`/`edit` hedefi hiçbir zaman `%TEMP%\opencode-fresh` değil

**Değişiklik günlüğü:** 2026-09-25 — ilk yazım (architect). Kaynak: `CLAUDE.md` G1 + ADR-002.
