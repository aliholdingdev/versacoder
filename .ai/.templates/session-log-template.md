---
title: "Oturum Kaydı Şablonu — .ai/log.md ve .ai/sessions/*.md"
type: template
category: session-log
status: active
version: "1.0.0"
updated: "2026-09-25"
authority: reference
scope: "oturum kaydi sablonu: log.md ve sessions/*.md icin zorunlu satir bicimi ve alanlar"
tags: [template]
source: "mevcut vault (baseline 4fc84f9, 2026-10-07; buyuk harfli linkler oncesi)"
supersedes: ""
---

**Zorunlu Bağlantılar:** [[.rules/index|Kural Dizini]] · [[.rules/performance-guidelines|Token Bütçesi]] · [[.rules/coding-standards|Kanıt ve Doğrulama]] · [[adr-index|ADR Dizini]] · [[definitions/AGENTS|Ajan Kataloğu]] · [[governance/WORKFLOW|Kapılar]] · [[session-log-template|Bu Şablon]]

# Oturum Kaydı Şablonu — `.ai/log.md` + `.ai/sessions/*.md`

## §1 Amaç

### §1.1 Neden bu şablon zorunlu

VersaCoder'da her oturum iki yerde iz bırakır: **ana kütük** `.ai/log.md` (tek satır özet, hep güncel) ve **oturum dosyası** `.ai/sessions/<oturum-id>.md` (kanıtlar, token, öğrenilenler). İkisi ayrı yazılmazsa üç şey bozulur:

1. **Süreklilik kopar** — sonraki oturum "neyi bitirmiştim?" sorusuna cevap bulamaz; iş iki kez yapılır.
2. **Token bütçesi görünmez olur** — 50.000 token/oturum sınırı (`[[.rules/performance-guidelines|Token Bütçesi]]`) ancak `TOKEN:` satırları toplanarak denetlenebilir; sayaç yoksa sınır ihlal edilir ve kimse fark etmez.
3. **Kanıt zinciri kırılır** — `claim → evidence` kuralı (`[[.rules/coding-standards|Kanıt ve Doğrulama]]`) yalnızca "hangi komut, hangi çıktı, hangi satır" kaydedildiğinde denetlenebilir; sonuçsuz iddia yazılamaz.

Şablon, bu üç denetim noktasını **zorunlu alan** haline getirir: `İlerleme (n/8)`, `Kullanılan Token`, `Kanıtlar`.

### §1.2 Disk Kanıtları ve Boşluk tablosu (YAGNI)

Aşağıdaki satırlar bu şablonun dayandığı vault dosyalarıdır. Var olan **okundu**, var olduğu varsayılan **işaretlendi**:

| İddia | Disk durumu | Karar |
|---|---|---|
| `.ai/.rules/` altında 7 kural dosyası var | Dosya adları okundu | Kullanılır |
| `.ai/log.md` ve `.ai/sessions/` ana kütük düzeni | Dizin yapısı okundu | Kullanılır |
| 50.000 token/oturum bütçesi | Kural dosyası adından doğrulandı, içerik okunmadı | `⚠️ VERIFICATION REQUIRED` |
| `GATE-000..005` kapı numaraları | ADR dizininde geçiyor | Kullanılır, kapı içerikleri okunmadı |
| ADR-001..008 numaraları | ADR dizini dosya adlarında görüldü | Kullanılır |
| 14 ajanlık katalog + 8 alt-ajan disiplini | `.ai/.agents/` dizininde görüldü | Kullanılır |
| 7 ekran `01-start-page`..`07-settings` | Ekran dizininde görüldü | Kullanılır |

`⚠️ VERIFICATION REQUIRED` işaretli satırlar **okunmadan kullanılmaz**; kullanacak ilk oturum onları okuyup işareti kaldırmakla yükümlüdür. İşaretsiz iddia yazılamaz.

### §1.3 Kullanım Türleri

| Tür | Ne zaman | Hedef dosya | Kim yazar |
|---|---|---|---|
| Ana kütük satırı | Her oturum kapanışında | `.ai/log.md` | Oturumu yürüten ajan |
| Oturum dosyası | Oturum başlar başlamaz | `.ai/sessions/<id>.md` | Oturumu yürüten ajan |
| Checkpoint güncelleme | Her `checkpoint` aşamasında | `.ai/sessions/<id>.md` | Oturumu yürüten ajan |
| Devir notu | `handover` aşamasında | `.ai/log.md` + oturum dosyası | Devreden ajan |
| Düzeltme kaydı | Kanıt sonradan bulunursa | Aynı dosya, `§4.4` ile | Bulan ajan |

### §1.4 Neden zorunlu

Şablon **zorunludur**; `.ai/log.md` ve `.ai/sessions/*.md` bu kalıp dışında üretilemez. Gerekçe: ana kütük tek satırdır, oturum dosyası kanıt yükünü taşır; ikisini ayırmayan düzen `log.md`'yi 500 satır ölü metne çevirir, tek dosyaya sığmayan kanıt da kaybolur. Kalıbı değiştirmek `[[.rules/index|Kural Dizini]]` üzerinden `§4.4` işlemini gerektirir.

## §2 Kapsam

| Kapsam | Kapsam Dışı |
|---|---|
| `.ai/log.md` tek satır kütük düzeni | Kod içi loglama (`ILogger`, konsol çıktısı) |
| `.ai/sessions/<id>.md` oturum dosyası | ADR, PRD, mimari doküman (`[[adr-index|ADR Dizini]]`) |
| `İlerleme (n/8)` ve `TOKEN:` sayaçları | Test raporu, coverage çıktısı |
| Checkpoint / handover / düzeltme kaydı | Ekran dokümantasyonu (`.ai/screens/`) |
| Kanıt satırları (komut → çıktı → dosya) | Git commit mesajları (commit ayrı kurallara bağlı) |

### §2.1 Alan → Kullanım yeri eşlemesi

| Alan | Yazıldığı yer | Okuduğu rol | Denetleyen |
|---|---|---|---|
| `Oturum Kimliği` | Her iki dosya | Tüm ajanlar | `§6.1/1` |
| `İlerleme (n/8)` | Oturum dosyası + `log.md` özeti | Devralan ajan | `§6.1/3` |
| `Kullanılan Token` | Oturum dosyası, her checkpoint'te | Bütçe denetçisi | `§6.1/4`, `G10` |
| `Kararlar` | Oturum dosyası | `architect`, `reviewer` | `§6.1/8` |
| `Kanıtlar` | Oturum dosyası | `reviewer` | `§6.1/9`, `G9` |
| `Dosya Değişiklikleri` | Oturum dosyası | Devralan ajan | `§6.1/13` |

### §2.2 Kapsam Sınırı Testi

Bu şablon **zorunludur**; 5 şartın **hepsi** true olmalı:

1. İş `.ai/log.md` veya `.ai/sessions/` dışında bir dosyaya yazılıyorsa → bu şablon değil.
2. İçerik karar/ADR ise → `[[adr-index|ADR Dizini]]`.
3. İçerik kural/yasak ise → `[[.rules/index|Kural Dizini]]`.
4. İçerik kod/test ise → ilgili ajan şablonu, oturum dosyasına yalnızca **özet satırı** düşer.
5. Şüphe varsa → kısa cevap: **oturum dosyasına yaz, `log.md`'yi tek satırda tut.**

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

`title, type, category, status, version, updated, authority` — 7 alan zorunlu; `authority: primary` hiçbir sürümde değiştirilemez.

```yaml
---
title: "Oturum Kaydı — <OTURUM_ID> · <TARİH>"
type: session-log
category: session-log
status: active          # draft | active | archived
version: "1.0.0"
updated: "YYYY-MM-DD"
authority: primary
---
```

### §3.2 H1 iskeleti

```markdown
# Oturum Kaydı — {{OTURUM_ID}} ({{TARİH}})

**Zorunlu Bağlantılar:** [[log|Ana Kütük]] · [[.rules/performance-guidelines|Token Bütçesi]] · [[governance/WORKFLOW|Kapılar]]
```

### §3.3-§3.9 Zorunlu 9 bölümün markdown iskeleti

```markdown
### §3.3.1 Oturum Kimliği
| Alan | Değer |
|---|---|
| Oturum ID | {{OTURUM_ID}} |
| Tarih | {{TARİH}} |
| Yürüten rol | {{ROLE}} |
| Ana kütük satırı | `.ai/log.md` → `{{LOG_LINE}}` |

### §3.3.2 Görev
{{GOREV_TANIMI}} — tek paragraf, kabul ölçütüyle birlikte.

### §3.4 İlerleme (n/8)
- [ ] 1/8 Keşif — dosyalar okundu
- [ ] 2/8 Plan — adımlar yazıldı
- [ ] 3/8 Uygulama — kod değişti
- [ ] 4/8 Test — `dotnet test` yeşil
- [ ] 5/8 Doğrulama — kanıt komutu çalıştı
- [ ] 6/8 İnceleme — `reviewer` onayı
- [ ] 7/8 Kapı — {{GATE_ID}} geçildi
- [ ] 8/8 Kapanış — `log.md` tek satırı güncellendi

### §3.5 Kullanılan Token
| Checkpoint | `TOKEN:` değeri | Kümülatif | Bütçe (50.000) |
|---|---|---|---|
| C0 başlangıç | {{TOKEN_C0}} | {{TOKEN_C0}} | Kalan: {{KALAN_0}} |
| C1 | {{TOKEN_C1}} | {{TOKEN_C1}} | Kalan: {{KALAN_1}} |
| C2 | {{TOKEN_C2}} | {{TOKEN_C2}} | Kalan: {{KALAN_2}} |

### §3.6 Kararlar
| # | Karar | Gerekçe | Dayanak |
|---|---|---|---|
| K1 | {{KARAR}} | {{GEREKCE}} | `ADR-00X` / kural |

### §3.7 Kanıtlar
| # | İddia | Kanıt komutu | Çıktıdan satır | Sonuç |
|---|---|---|---|---|
| P1 | {{IDDA}} | `{{KOMUT}}` | {{SATIR}} | {{PASS_FAIL}} |
| P? | Henüz doğrulanmadı | — | — | `⚠️ VERIFICATION REQUIRED` |

### §3.8 Öğrenilenler
- {{OGRENILEN_1}} — ne işe yarar, gelecekte neyi değiştirir.

### §3.9.1 Sonraki Adım
{{SONRAKI_ADIM}} — tek eylem, tahmini süre, bloke eden şey yoksa "YOK".

### §3.9.2 Dosya Değişiklikleri
| Dosya | İşlem | Satır/boyut |
|---|---|---|
| {{DOSYA_YOLU}} | eklendi / değişti / silindi | {{SAYI}} |
```

### §3.10 Doldurulmuş gerçek VersaCoder örneği — `.ai/sessions/2026-09-25-token-budget.md`

```markdown
# Oturum Kaydı — ses_20260925_01 (2026-09-25)

**Zorunlu Bağlantılar:** [[log|Ana Kütük]] · [[.rules/performance-guidelines|Token Bütçesi]] · [[governance/WORKFLOW|Kapılar]]

### §3.3.1 Oturum Kimliği
| Alan | Değer |
|---|---|
| Oturum ID | `ses_20260925_01` |
| Tarih | 2026-09-25 |
| Yürüten rol | `master-orchestrator` |
| Ana kütük satırı | `.ai/log.md` → `2026-09-25 | ses_20260925_01 | token-budget kuralı | GATE-003 | 16.400/50.000` |

### §3.3.2 Görev
`.ai/.rules/token-budget` kuralını 50.000 token/oturum sınırını uygulayacak şekilde yaz; `log.md` kütüğüne kümülatif sayaç satırı ekle. Kabul: `TOKEN:` toplamı 50.000'i geçerse oturum `handover`a döner.

### §3.4 İlerleme (n/8)
- [x] 1/8 Keşif — `.ai/.rules/` 7 dosya okundu
- [x] 2/8 Plan — 4 adım yazıldı
- [x] 3/8 Uygulama — `token-budget` dosyası eklendi
- [x] 4/8 Test — `dotnet test` yeşil (xunit, hedef %90)
- [x] 5/8 Doğrulama — kanıt komutu çalıştı
- [ ] 6/8 İnceleme — `reviewer` onayı bekleniyor
- [ ] 7/8 Kapı — `GATE-003` geçilmedi
- [ ] 8/8 Kapanış — `log.md` tek satırı güncellendi
**Durum: 5/8**

### §3.5 Kullanılan Token
| Checkpoint | `TOKEN:` değeri | Kümülatif | Bütçe (50.000) |
|---|---|---|---|
| C0 başlangıç | 1.200 | 1.200 | Kalan: 48.800 |
| C1 | 6.900 | 8.100 | Kalan: 41.900 |
| C2 | 8.300 | 16.400 | Kalan: 33.600 |

### §3.6 Kararlar
| # | Karar | Gerekçe | Dayanak |
|---|---|---|---|
| K1 | Sayaç `log.md`'de tek satırda tutuldu | 500 satırlık kütük okunamaz hale gelir | `§1.4` |
| K2 | Aşım otomatik `handover` | Kuralı ihlal eden oturum üretmeye devam edemez | `G10` |

### §3.7 Kanıtlar
| # | İddia | Kanıt komutu | Çıktıdan satır | Sonuç |
|---|---|---|---|---|
| P1 | Kural dosyası diskte var | `(Get-Item '.ai/.rules/token-budget').Length` | `4.812` | PASS |
| P2 | Testler yeşil | `dotnet test --no-build` | `Passed! - Failed: 0` | PASS |
| P3 | Sayaç 50.000'i aşmıyor | `Select-String -Path '.ai/log.md' -Pattern 'TOKEN'` | `16.400/50.000` | PASS |

### §3.8 Öğrenilenler
- `log.md` tek satırı, oturum dosyasındaki `Kümülatif` sütunuyla eşitlenmeli; ikisi fark ederse kütük yanlıştır.

### §3.9.1 Sonraki Adım
`reviewer` onayı (6/8) — tahmini 4 dk; bloke eden şey **YOK**.

### §3.9.2 Dosya Değişiklikleri
| Dosya | İşlem | Satır/boyut |
|---|---|---|
| `.ai/.rules/token-budget` | eklendi | 61 satır |
| `.ai/log.md` | değişti | +1 satır |
| `.ai/sessions/2026-09-25-token-budget.md` | eklendi | bu dosya |
```

### §3.11 Kopyala-yapıştır tam kalıp (tümü `{{PLACEHOLDER}}`)

```markdown
# Oturum Kaydı — {{OTURUM_ID}} ({{TARİH}})

**Zorunlu Bağlantılar:** [[log|Ana Kütük]] · [[.rules/performance-guidelines|Token Bütçesi]] · [[governance/WORKFLOW|Kapılar]]

### §3.3.1 Oturum Kimliği
| Alan | Değer |
|---|---|
| Oturum ID | {{OTURUM_ID}} |
| Tarih | {{TARİH}} |
| Yürüten rol | {{ROLE}} |
| Ana kütük satırı | `.ai/log.md` → {{LOG_LINE}} |

### §3.3.2 Görev
{{GOREV_TANIMI}}

### §3.4 İlerleme (n/8)
- [ ] {{N}}/8 {{ASAMA}} — {{KANIT_KISA}}
- [ ] {{N}}/8 {{ASAMA}} — {{KANIT_KISA}}
**Durum: {{N}}/8**

### §3.5 Kullanılan Token
| Checkpoint | `TOKEN:` değeri | Kümülatif | Bütçe (50.000) |
|---|---|---|---|
| {{CHECKPOINT}} | {{TOKEN_N}} | {{KUMULATIF}} | Kalan: {{KALAN}} |

### §3.6 Kararlar
| # | Karar | Gerekçe | Dayanak |
|---|---|---|---|
| K{{N}} | {{KARAR}} | {{GEREKCE}} | {{DAYANAK}} |

### §3.7 Kanıtlar
| # | İddia | Kanıt komutu | Çıktıdan satır | Sonuç |
|---|---|---|---|---|
| P{{N}} | {{IDDA}} | {{KOMUT}} | {{SATIR}} | {{PASS_FAIL}} |

### §3.8 Öğrenilenler
- {{OGRENILEN}}

### §3.9.1 Sonraki Adım
{{SONRAKI_ADIM}} — {{SURE}} — bloke eden: {{BLOKE}}

### §3.9.2 Dosya Değişiklikleri
| Dosya | İşlem | Satır/boyut |
|---|---|---|
| {{DOSYA}} | {{ISLEM}} | {{SAYI}} |
```

## §4 Kurallar

### §4.1 Durum / Aşama döngüsü

`draft → active → checkpoint → handover → archived`; `checkpoint → active` geri dönüş serbesttir, `archived` yalnızca `§4.4` ile yeniden açılır.

```
[draft] → [active] → [checkpoint] → [active]        (düzeltme döngüsü)
              ↓             ↓
         [handover] → [archived]                    (kapanış)
```

### §4.2 Onay / Rol akışı

| Aşama | Yazan rol | Onaylayan rol | Kanıt |
|---|---|---|---|
| `draft` oluşturma | Otu. yürüten ajan | — | Dosya varlığı |
| `active → checkpoint` | Otu. yürüten ajan | — | `TOKEN:` + `İlerleme` satırı |
| `checkpoint → handover` | Otu. yürüten ajan | `reviewer` | `§3.7` PASS satırları |
| `handover → archived` | Devralan ajan | `master-orchestrator` | `log.md` tek satırı |
| `archived → active` | İsteyen ajan | `architect` | `§4.4` kaydı |

### §4.3 Öncelik / Ölçek tablosu

| Öncelik | Ne için | Ölçek |
|---|---|---|
| P0 | Bütçe aşımı (`G10`), kanıtsız iddia (`G9`) | Oturum durur |
| P1 | `n/8` ilerleme geride, kapı reddi | Aynı oturumda çözülür |
| P2 | Eksik `Dosya Değişiklikleri` satırı | Sonraki checkpoint |
| P3 | Biçim/eklenti düzeltmeleri | `§4.4` ile |

### §4.4 Değişiklik kontrolü

1. `authority: primary` hiçbir sürümde değiştirilemez (`G2`).
2. `## §1`..`## §7` başlıklarından biri silinemez (`G3`).
3. `archived` kayıt yalnızca `architect` onayıyla yeniden açılır; geçmiş satırlar **silinmez**, üstüne düzeltme satırı eklenir.
4. Her değişiklik `version` artırır ve `updated` tarihini günceller.
5. `TOKEN:` değeri **geriye dönük düşürülemez**; yanlış sayı sonraki checkpoint'te düzeltilir ve not düşülür.

### §4.5 Hard Guardrails tablosu

| ID | Guardrail | Kural kaynağı | Tespit yöntemi | İhlal Sonucu |
|---|---|---|---|---|
| **G1** | Frontmatter 7 alan eksiksiz | `§3.1` | İlk 10 satır | Dosya geçersiz sayılır |
| **G2** | `authority: primary` değişmez | Vault kuralı | Diff | Kayıt geri alınır |
| **G3** | `## §1`..`## §7` silinemez | Bu şablon | Başlık sayısı = 7 | Kalıp revert |
| **G4** | `İlerleme (n/8)` 8 satır | `§3.4` | `— [ ]`/`— [x]` sayımı | Checkpoint kabul edilmez |
| **G5** | EF Core / `DbContext` anılmaz | `ADR-004` | Dosya taraması | Satır silinir |
| **G6** | `TOKEN:` kümülatif artan | `G10` | Sütun sırası | Oturum `handover` |
| **G7** | Bütçe ≤ 50.000 / oturum | Token bütçesi kuralı | Toplam satırı | Oturum durdurulur |
| **G8** | Kanıtsız iddia yazılamaz | `§3.7` | `⚠️ VERIFICATION REQUIRED` yoksa | Satır silinir, `REVIEW` |
| **G9** | Mojibake yasak | Dil kuralı | `\xc3\xbc`, `â„¢`, `ðŸ` taraması | Dosya yeniden yazılır |
| **G10** | `log.md` tek satırlık kütük | `§1.4` | Satır uzunluğu / tarih tekrarı | Özet bölünür |
| **G11** | Geçmiş satırlar silinemez | `§4.4` | Eski `TOKEN:` değeri | Kayıt geri alınır |
| **G12** | `Sonraki Adım` tek eylem | `§3.9.1` | Paragraf sayımı | Alan daraltılır |

## §5 Workflow

### §5.1 Adım tablosu ve zaman bütçesi

| # | Adım | Girdi | Çıktı | Rol | Süre |
|---|---|---|---|---|---|
| 1 | Keşif | Görev tanımı | Okunan dosya listesi | Otu. yürüten | 3 dk |
| 2 | Kimlik kurulumu | `§3.11` kalıbı | Oturum dosyası `draft` | Otu. yürüten | 2 dk |
| 3 | `log.md` satırı | Oturum ID | Tek satır kayıt | Otu. yürüten | 1 dk |
| 4 | Görev + (n/8) 1/8 | Kabul ölçütü | `§3.3.2`, `§3.4` | Otu. yürüten | 3 dk |
| 5 | Uygulama 3/8 | Plan | Kod/`docs` değişikliği | İlgili ajan | 15 dk |
| 6 | Kanıt toplama 5/8 | Değişiklik | `§3.7` PASS satırları | Otu. yürüten | 5 dk |
| 7 | Token checkpoint | `TOKEN:` değeri | `§3.5` kümülatif | Otu. yürüten | 2 dk |
| 8 | İnceleme 6/8 | Oturum dosyası | `reviewer` onayı | `reviewer` | 4 dk |
| 9 | Kapı 7/8 | Kanıtlar | `GATE-00X` sonucu | Kapı sahibi rol | 5 dk |
| 10 | Kapanış 8/8 | Onay | `log.md` günceli + `archived` | Otu. yürüten | 3 dk |
| — | **Toplam** | — | — | — | **43 dk** |

### §5.2 Hata Modları tablosu

| Hata | Belirti | İlk tepki | Kurtarma |
|---|---|---|---|
| E1 Dosya yok | `.ai/sessions/<id>.md` bulunamadı | `§3.11` kalıbıyla oluştur | `log.md` satırını ekle |
| E2 Bölüm eksik | Başlık sayısı < 7 | Kalıbı yeniden yapıştır | `G3` kontrolü |
| E3 Bütçe aşımı | Kümülatif > 50.000 | Oturumu `handover`a al | `G7`, devir notu yaz |
| E4 İlerleme takılı | 3/8 iki checkpoint üst üste | Adımı yeniden çerçevele | `Sonraki Adım`'ı tek eyleme indir |
| E5 Kanıt reddi | `reviewer` PASS satırını düşürdü | `⚠️ VERIFICATION REQUIRED` ekle | Yeniden çalıştır, satırı geri koy |
| E6 Mojibake | `\xc3\xbc`, `â„¢`, `ðŸ` | Dosyayı yeniden yaz | `G9` |
| E7 Kütük kayması | `log.md` ≠ oturum `Kümülatif` | Oturum dosyasını esas al | Kütük satırını düzelt, not düş |
| E8 Çakışma | İki oturum aynı dosyayı değiştirdi | `architect` arbitrajı | `§4.4` kaydı aç |

### §5.3 Salt-okunur PowerShell komut bloğu

```powershell
# 1 — log.md tek satırlık kütük olmalı (çok sayıda sonuç olmamalı)
(Select-String -Path 'C:\versacoder\.ai\log.md' -Pattern 'ses_').Count  # çok az olmalı

# 2 — Oturum dosyası sekiz bölüm içermeli (7 sonuç olmamalı)
(Select-String -Path 'C:\versacoder\.ai\sessions\{{OTURUM_ID}}.md' -Pattern '^### §3\.').Count  # 9 olmalı

# 3 — İlerleme 8 satır (4 tamamlanmış dâhil) olmalı
(Select-String -Path 'C:\versacoder\.ai\sessions\{{OTURUM_ID}}.md' -Pattern '^- \[ \] \d/8').Count  # 8 olmalı

# 4 — Token kümülatifi bütçeyi aşmamalı (G7)
(Select-String -Path 'C:\versacoder\.ai\sessions\{{OTURUM_ID}}.md' -Pattern 'TOKEN').Count  # ≥ 1 olmalı

# 5 — Mojibake olmamalı (G9)
(Select-String -Path 'C:\versacoder\.ai\sessions\{{OTURUM_ID}}.md' -Pattern '\xc3\xbc|â„¢|ðŸ').Count  # 0 olmalı

# 6 — EF Core anılmamalı (G5)
(Select-String -Path 'C:\versacoder\.ai\sessions\{{OTURUM_ID}}.md' -Pattern 'DbContext').Count  # 0 olmalı
```

## §6 Doğrulama

### §6.1 Onay kutusu listesi

- [ ] 1. Frontmatter `title, type, category, status, version, updated, authority` 7 alan eksiksiz
- [ ] 2. `authority: primary` yazıyor, `updated` BUGÜN tarihinde
- [ ] 3. `## §1 Amaç` … `## §7 Referanslar` — tam 7 bölüm, sırası aynı
- [ ] 4. H1 hemen frontmatter altında, `**Zorunlu Bağlantılar:**` satırı var
- [ ] 5. `§1.2` tablosunda `⚠️ VERIFICATION REQUIRED` satırı duruyor
- [ ] 6. `§2` kapsam/kapsam dışı tablosu ve `§2.2` 5 şartlı sınır testi mevcut
- [ ] 7. `§3.3..§3.9` arasında 9 zorunlu bölümün iskeleti mevcut
- [ ] 8. `§3.10` gerçek VersaCoder örneği tam ve somut (placeholder yok)
- [ ] 9. `§3.11` kalıbındaki tüm alanlar `{{PLACEHOLDER}}`
- [ ] 10. `§3.4 İlerleme (n/8)` tam 8 madde, `(n/8)` biçiminde
- [ ] 11. `§4.5` içinde 12 guardrail (`G1`..`G12`) ve "İhlal Sonucu" sütunu var
- [ ] 12. `§5.1` en az 9 adım + zaman bütçesi satırı var
- [ ] 13. `§5.2` en az 7 hata modu var
- [ ] 14. `§5.3` içinde 6 salt-okunur PowerShell komutu, her biri "— N olmalı" yorumlu
- [ ] 15. `§6.1` bu liste 16 madde, `§6.2` "Otomatik DUR" sütunu var
- [ ] 16. `§7.2` red-team tablosu en az 10 satır; ç ğ ı İ ö ş ü doğru, mojibake yok

### §6.2 Red Sinyalleri tablosu (Otomatik DUR)

| Sinyal | Koşul | Sonuç |
|---|---|---|
| Token körlüğü | `§3.5` boş ya da kümülatif artmıyor | **DUR** — checkpoint yok |
| İlerleme şişirmesi | `n/8` 8/8 iken kanıt yok | **DUR** — `G8` |
| Kütük şişmesi | `log.md` > 1 satır / oturum | **DUR** — `G10` |
| Geçmiş silinmesi | Eski `TOKEN:` değeri kayıpsa | **DUR** — `G11` |
| Tek eylem ihlali | `Sonraki Adım` paragrafı 2+ eylem | **DUR** — `G12` |
| İşaret taşıma | `⚠️ VERIFICATION REQUIRED` gerekçesiz kaldırılmışsa | **DUR** — `§1.2` |

### §6.3 Sürüm / Aşama geçiş kuralları

| Geçiş | Şart | Rol |
|---|---|---|
| `draft → active` | 7 alan + H1 + `log.md` satırı | Otu. yürüten |
| `active → checkpoint` | `TOKEN:` + `n/8` güncel | Otu. yürüten |
| `checkpoint → handover` | `§3.7` tümü PASS ya da işaretli | `reviewer` |
| `handover → archived` | `log.md` = kümülatif, kapı sonucu | `master-orchestrator` |
| `archived → active` | `§4.4` kaydı + `version` artışı | `architect` |

### §6.4 Quality Report markdown bloğu

```markdown
### Quality Report — {{OTURUM_ID}} ({{TARİH}})
- Bölüm: 7/7 · İskelet: 9/9 · Onay: 16/16
- İlerleme: {{N}}/8 · Token: {{KUMULATIF}}/50.000 · Kapı: {{GATE_ID}} {{SONUC}}
- Guardrail: G1..G12 ihlali **YOK** · Kanıt: {{PASS}} PASS, {{FAIL}} FAIL
- Kalan risk: {{RISK}} · Sonraki adım: {{SONRAKI_ADIM}} ({{SURE}})
- Sonuç: **{{KABUL | RED | HANDOVER}}**
```

### §6.5 Şablona özgü ek kontroller

1. Her `checkpoint`ta `§3.5` kümülatif satırı ile `.ai/log.md` özeti **eşit** (eşit değilse `E7`).
2. `§3.7` kanıt satırı en az bir gerçek PowerShell komutu ve gerçek çıktı satırı içerir.
3. Oturum dosyası arşivlendikten sonra **yeni kanıt satırı eklenmez**; düzeltme ayrı satır olur.

## §7 Referanslar

### §7.1 VersaCoder bağlantıkları tablosu

| Bağlantı | Ne için | Yol (`.ai` göreli) |
|---|---|---|
| [[log|Ana Kütük]] | Tek satır özet | `log.md` |
| [[ops/index|Oturum Dizini]] | Dosya listesi | `sessions/` |
| [[.rules/performance-guidelines|Token Bütçesi]] | 50.000 sınırı | `.rules/token-budget` |
| [[.rules/coding-standards|Kanıt ve Doğrulama]] | `claim → evidence` | `.rules/evidence-and-verification` |
| [[.rules/index|Kural Dizini]] | 7 kural dosyası | `.rules/` |
| [[adr-index|ADR Dizini]] | ADR-001..008 | `adr/` |
| [[governance/WORKFLOW|Kapılar]] | GATE-000..005 | `gates/` |
| [[definitions/AGENTS|Ajan Kataloğu]] | 14 ajan + 8 disiplin | `.agents/` |

### §7.2 Red Team — Sık Yapılan Hatalar tablosu

| # | Hata | Belirti | Düzeltme |
|---|---|---|---|
| RT1 | `log.md`'ye uzun özet yazıldı | Dosya 100+ satır | Tek satıra indir |
| RT2 | Token yalnızca son checkpoint | Kümülatif sıçrar | Her checkpoint'te yaz |
| RT3 | `(n/8)` yerine yüzde | `%62` yazıldı | `5/8` biçimine çevir |
| RT4 | Kanıt komutu uydurma | Çıktı satırı yok | Komutu çalıştır, çıktıyı yapıştır |
| RT5 | `⚠️` gerekçesiz kaldırma | İşaret yok, kanıt da yok | İşareti geri koy |
| RT6 | Oturum dosyasında ADR yazımı | `## §1 Amaç` yerine karar metni | ADR'yi `adr/`'ye taşı |
| RT7 | `TOKEN:` geri düşürme | Kümülatif azaldı | Düzeltmeyi yeni satırda yap |
| RT8 | Mojibake | `\xc3\xbc`, `â„¢` | `§5.3/5` komutuyla temizle |
| RT9 | İki oturum aynı dosyada | Satır kayması | `architect` arbitrajı, `§4.4` |
| RT10 | `Sonraki Adım` çok eylemli | Paragraf bloğu | Tek eyleme indir |
| RT11 | Kapı sonucusuz kapanış | `GATE` satırı yok | Kapıyı geçmeden `archived` yapma |
