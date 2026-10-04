---
title: "Playbook İndeksi — VersaCoder playbooks/"
type: index
category: playbooks
status: active
version: 1.0.0
updated: 2026-10-04
authority: SSOT
scope: "Tekrarlanan is akislarinin (gate kurulumu, release, vault tamiri) kayit yeri: ornek basliklar, sablon baglantisi ve kabul kriteri"
---

# Playbook İndeksi (playbooks/)

**Zorunlu Bağlantılar:** [WORKFLOW](../WORKFLOW.md) · [CLAUDE](../CLAUDE.md) · [Şablon envanteri](../.templates/index.md) · [keys](../keys.md)

> **Tek cümlelik sorumluluk:** "Bu tekrarlanan iş nasıl yapılır?" sorusunun cevabının nerede yaşadığı ve playbook'ların hangi şablonla yazıldığıdır.

---

## §0 Özet (Answer First)

`playbooks/` yalnız **ikinci kez yapılacak işleri** yazar: adım adım, her adımda çıktı + kanıt + süre olan, kopyala-yürütilebilir akışlar. Bir kez yapılacak iş playbook değil, [WORKFLOW](../WORKFLOW.md) gate'idir; kural `.rules/`'tadır; karar ADR'dedir — playbook bunlara **link verir, metin kopyalamaz** (SSUT). Klasör durumu (2026-09-29): `playbooks/` = 0 dosya; bu indeks ilk kayıttır ve bu dosyanın §3'ündeki dört başlık **REZERVEDİR** (dosyalar henüz üretilmedi → link yazılmaz).

---

## §1 Ne Tur Tekrarlanan İş Akışı Buraya Yazılır

| # | Yazılır | Yazılmaz | Nereye gider |
|---|---|---|---|
| 1 | Gate kurulumu/kapanışı (gate kriteri + kanıt listesi + komut) | gate tanımı kendisi | [WORKFLOW](../WORKFLOW.md) |
| 2 | Release / sürüm hazırlığı (adım, komut, geri alma) | sürüm kararı | ADR / `PROJECTS.md` |
| 3 | Vault tamiri (bozuk link, mojibake, frontmatter onarımı) | tekil hata düzeltmesi | ilgili dosya yerinde |
| 4 | Yeni katman/form ekleme gibi sık tekrarlanan kurulumlar | mimari karar | [ADR](../.decisions/adr-index.md) |
| 5 | Geri alma/rollback adımları (her adımda geri dönüş yolu) | "gerekirse geri al" cümlesi | playbook § adım tablosu |

**Sınır testi (ilk EVET yeter):** "Aynı iş önümüzdeki 1 ayda 2. kez yapılacak mı?" → EVET = playbook; "Tek seferlik mi?" → tek sefer = gate/kural/görev.

---

## §2 Bir Playbook'un Zorunlu Bileşenleri

| # | Bileşen | Zorunlu içerik | Yoksa |
|---|---|---|---|
| 1 | Ön koşul | hangi gate açık, hangi dosya/elde ne var | RED |
| 2 | Adım tablosu | # · adım · komut/çıktı · süre · sahip | RED |
| 3 | Hata modları | belirti · kök neden · düzeltme (≥ 3 satır) | RED |
| 4 | Geri alma | hangi adımda ne geri alınır | RED |
| 5 | Kabul kriteri | ölçülebilir `[ ]` maddeleri | RED |

**Yazım şablonu:** `.templates/index.md` §2.1'de kayıtlı şablonlardan seçilir; şablonu olmayan tür önce `.templates/index.md` §5.1 Adım 1'den talep edilir.

---

## §3 Örnek Başlıklar (REZERVE — dosyalar henüz yok)

| # | Rezerve dosya adı | Kapsadığı iş | Çıktı |
|---|---|---|---|
| 1 | `gate-004a-kapanis-playbook.md` | GATE-004a kapanış sırası (500 MD, 0 bozuk link, 0 mojibake) | gate raporu satırı |
| 2 | `vault-tamiri-playbook.md` | bozuk link + mojibake + frontmatter onarımı | `check-links.ps1` = 0 |
| 3 | `yeni-katman-ekleme.md` | yeni katman/N düğümü ekleme akışı (ADR-028 kapıları) | katalog + matris güncellemesi |
| 4 | `yeni-form-tasarlama.md` | WinForms formu ekleme akışı (GATE-003 sonrası) | form + test |

> Bu dört ad **rezervedir**: `10-bilinen-eksikler.md` §2'de bu adlar bozuk-link kaydı olarak geçer; dosyalar üretilene kadar bu tablo **link değil, yazı**dır (G7).

---

## §4 Kabul Kriteri ve Yazım Akışı

| # | Kabul kriteri | Ölçüm |
|---|---|---|
| 1 | Her adımda **komut veya çıktı** var | adım tablosu, boş hücre 0 |
| 2 | Adım süreleri somut (dk) | "biraz/yaklaşık" yasak |
| 3 | `.rules/` ve ADR'den tekrar yok, yalnız link | metin karşılaştırması |
| 4 | 2. denemede çalışan tekrar kanıtı | en az 1 başarılı koşu kaydı |
| 5 | Bu indekste 1 satır kaydı | §3 tablosu |

**Yazım sırası:** rezerv adı seç → `.templates/index.md` şablonunu oku → kanıtı ölç → yaz → `check-links.ps1` → bu indekse kaydet.

---

## §5 Kapsam Dışı ve Boşluk Notu

| # | Kapsam dışı | Nerede |
|---|---|---|
| 1 | Gate tanımı, faz sırası, onay zinciri | `.ai/WORKFLOW.md` |
| 2 | Kök `.ai/index.md` (navigasyon) | 2026-09-29'da diskte YOK → link yazılmadı |
| 3 | Roller ve ajan dağılımı | `.ai/AGENTS.md` · `.ai/ROLE.md` |
| 4 | Kural metinleri | `.ai/.rules/*` |

---

## §6 Referanslar

| # | Kaynak | Yol | Ne için |
|---|---|---|---|
| 1 | Süreç/gate kaynağı | `.ai/WORKFLOW.md` | §1 sınır testi, gate playbook'larının üstü |
| 2 | Şablon envanteri | `.ai/.templates/index.md` | playbook şablonu seçimi (§2/§4) |
| 3 | Eksikler defteri | `.ai/reports/10-bilinen-eksikler.md` | rezerve adların bozuk-link kaydı |
| 4 | Rapor indeksi | `.ai/reports/index.md` | kanıt/ölçüm raporları |
| 5 | Şablon sözleşmesi | `.ai/.templates/new-doc-contract.md` | frontmatter + link kuralları |
