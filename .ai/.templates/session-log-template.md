---
title: "Oturum Kaydı Şablonu — .ai/log.md ve .ai/sessions/*.md"
type: template
category: session-log
status: active
version: "1.0.0"
updated: "2026-09-25"
authority: SSOT
scope: "oturum kaydi sablonu: log.md ve sessions/*.md icin zorunlu satir bicimi ve alanlar"
---

**Zorunlu Bağlantılar:** [Kural Dizini](../.rules/index.md) · [Token Bütçesi](../AGENTS.md) · [Kanıt ve Doğrulama](../CLAUDE.md) · [ADR Dizini](adr/adr-index.md) · [Ajan Kataloğu](../AGENTS.md) · [Kapılar](../WORKFLOW.md) · [Bu Şablon](session-log-template.md)

# Oturum Kaydı Şablonu — `.ai/log.md` + `.ai/sessions/*.md`

## §1 Amaç

### §1.1 Neden bu şablon zorunlu

VersaCoder'da her oturum iki yerde iz bırakır: **ana kütük** `.ai/log.md` (tek satır özet, hep güncel) ve **oturum dosyası** `.ai/sessions/<oturum-id>.md` (kanıtlar, token, öğrenilenler). İkisi ayrı yazılmazsa üç şey bozulur:

1. **Süreklilik kopar** — sonraki oturum "neyi bitirmiştim?" sorusuna cevap bulamaz; iş iki kez yapılır.
2. **Token bütçesi görünmez olur** — 50.000 token/oturum sınırı (`[[../.rules/token-budget|Token Bütçesi]]`) ancak `TOKEN:` satırları toplanarak denetlenebilir; sayaç yoksa sınır ihlal edilir ve kimse fark etmez.
3. **Kanıt zinciri kırılır** — `claim → evidence` kuralı (`[[../.rules/evidence-and-verification|Kanıt ve Doğrulama]]`) yalnızca "hangi komut, hangi çıktı, hangi satır" kaydedildiğinde denetlenebilir; sonuçsuz iddia yazılamaz.

Şablon, bu üç denetim noktasını **zorunlu alan** haline getirir: `İlerleme (n/8)`, `Kullanılan Token`, `Kanıtlar`.

### §1.2 Disk Kanıtları ve Boşluk tablosu (YAGNI)

Aşağıdaki satırlar bu şablonun dayandığı vault dosyalarıdır. Var olan **okundu**, var olduğu varsayılan **işaretlendi**:

| İddia | Disk durumu | Karar |
|---|---|---|
| `.ai/.rules/` altında 7 kural dosyası var | Dosya adları okundu | Kullanılır |
| `.ai/log.md` ve `.ai/sessions/` ana kütük düzeni | Dizin yapısı okundu | Kullanılır |
| 50.000 token/oturum bütçesi | Kural dosyası adından doğrulandı, içerik okunmadı | `⚠️ VERIFICATION REQUIRED` |
| `GATE-004a..006` kapı numaraları | ADR dizininde geçiyor | Kullanılır, kapı içerikleri okunmadı |
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

Şablon **zorunludur**; `.ai/log.md` ve `.ai/sessions/*.md` bu kalıp dışında üretilemez. Gerekçe: ana kütük tek satırdır, oturum dosyası kanıt yükünü taşır; ikisini ayırmayan düzen `log.md`'yi 500 satır ölü metne çevirir, tek dosyaya sığmayan kanıt da kaybolur. Kalıbı değiştirmek `[[../.rules/rules-index|Kural Dizini]]` üzerinden `§4.4` işlemini gerektirir.

### §1.5 Oturum Kaydı Alan Sözlüğü

Her zorunlu alanın biçimi; tabloda olmayan alan yazılamaz. Örnekler gerçek formattır:

| Alan | Tip ve biçim | Zorunlu | Doğru örnek | İhlalde |
|---|---|---|---|---|
| Oturum ID | `ses_YYYYMMDD_NN`, ASCII, boşluk yok | Evet | `ses_20261006_02` | Dosya yeniden adlandırılır (E1) |
| Tarih | ISO `YYYY-MM-DD` | Evet | `2026-10-06` | Serbest tarih biçimi RED |
| Yürüten rol | 14 ajan rolünden biri | Evet | `master-orchestrator` | Rol uydurulamaz; `architect` atar |
| Görev | tek paragraf + kabul ölçütü | Evet | kabul: `TOKEN:` toplamı 50.000'ü aşarsa `handover` | Kabul yoksa 1/8 açılmaz |
| İlerleme | 8 satır, `- [ ]` ya da `- [x]` | Evet | `- [x] 5/8 Doğrulama — kanıt komutu çalıştı` | 8 satırdan azsa `G4` |
| Durum | `draft` · `active` · `checkpoint` · `handover` · `archived` | Evet | `checkpoint` | Sözlük dışı değer → `§6.3` |
| `TOKEN:` değeri | binlik ayraçlı sayı; kümülatif ve artan | Evet | `38.750` | Azalma `G6`, geri düşürme `G11` |
| Kümülatif | = son `TOKEN:` değeri | Evet | `38.750` | `log.md` satırı farklıysa `E7` |
| Bütçe satırı | `Kalan:` değeri = 50.000 − Kümülatif | Evet | `Kalan: 11.250` | Negatif kalan → `G7` |
| Karar | `K<n>` + gerekçe + dayanak; üç hücre dolu | Satır varsa | `K2` · aşım otomatik `handover` · `G10` | Boş dayanak `G8` |
| Kanıt | `P<n>` + komut + çıktıdan satır + sonuç | ≥ 1 PASS | `P2` · `dotnet test --no-build` · `Passed! - Failed: 0` · PASS | Kanıtsız iddia `G8` |
| İşaret | `⚠️ VERIFICATION REQUIRED` + gerekçe | Okunmamış iddialarda | sınır kuralı henüz okunmadı | Gerekçesiz kaldırma `§6.2` |
| Öğrenilen | 1–3 madde; her madde geleceğe etkisiyle | İsteğe bağlı | log satırı kümülatifle eşitlenmeli | Aynı madde ikinci kez yazılamaz |
| Sonraki Adım | tek eylem + süre + bloke | Evet | `reviewer` onayı — 4 dk — bloke YOK | İki eylem `G12` |
| Dosya Değişiklikleri | yol + işlem + satır/boyut | 3/8'de ≥ 1 satır | `.ai/log.md` · değişti · `+1` satır | Boş tablo `§6.2` |
| Ana kütük satırı | `log.md` tek satırı, `|` ile ayrılmış 5 alan | Evet | aşağıdaki blokta | Uzun özet `G10` |

Gerçek format (üçü de tek satırdır):

```text
2026-10-06 | ses_20261006_02 | form-designer sablonu genişletildi | GATE-003 | 12.600/50.000
K2 | Aşım otomatik handover'a döner | Kuralı ihlal eden oturum üretmeye devam edemez | G10
P1 | Oturum dosyası diskte | Test-Path .ai/sessions/2026-10-06-form-designer.md | True | PASS
```

## §2 Kapsam

| Kapsam | Kapsam Dışı |
|---|---|
| `.ai/log.md` tek satır kütük düzeni | Kod içi loglama (`ILogger`, konsol çıktısı) |
| `.ai/sessions/<id>.md` oturum dosyası | ADR, PRD, mimari doküman (`[[../adr-index|ADR Dizini]]`) |
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
2. İçerik karar/ADR ise → `[[../adr-index|ADR Dizini]]`.
3. İçerik kural/yasak ise → `[[../.rules/rules-index|Kural Dizini]]`.
4. İçerik kod/test ise → ilgili ajan şablonu, oturum dosyasına yalnızca **özet satırı** düşer.
5. Şüphe varsa → kısa cevap: **oturum dosyasına yaz, `log.md`'yi tek satırda tut.**

### §2.3 Dosya Adı, Yer ve Ömür Kuralları

| Nesne | Yer / ad | Ne zaman yazılır | Ömür |
|---|---|---|---|
| Ana kütük | `.ai/log.md` | her oturum kapanışında (8/8) | silinmez, tek satır genişler |
| Oturum dosyası | `.ai/sessions/YYYY-MM-DD-<konu>.md` | oturum başlar başlamaz | `archived` sonrası salt-okunur |
| Oturum ID | `ses_YYYYMMDD_NN` | dosya oluşurken | aynı gün ikinci oturumda `_02` |
| Checkpoint satırı | oturum dosyası `§3.5` | her `checkpoint` aşaması | geçmiş satır silinmez (`G11`) |
| Devir notu | `.ai/log.md` + oturum dosyası | `handover` anında | devralan kapanışta günceller |
| Düzeltme kaydı | aynı dosya, alt satıra | kanıt sonradan bulunursa | üstüne yazma, altına ekleme |

Ad çatışması: aynı gün aynı konuda iki oturum açılamaz — ikinci oturum `_02` sonekiyle
açılır ve birincinin dosyası `handover` ile kapatılır (`§5.4`). Dosya adı sonra
değiştirilemez; konu değişirse yeni oturum açılır.

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

`title, type, category, status, version, updated, authority` — 7 alan zorunlu; `authority: SSOT` hiçbir sürümde değiştirilemez.

```yaml
---
title: "Oturum Kaydı — <OTURUM_ID> · <TARİH>"
type: session-log
category: session-log
status: active          # draft | active | archived
version: "1.0.0"
updated: "YYYY-MM-DD"
authority: SSOT
---
```

### §3.2 H1 iskeleti

```markdown
# Oturum Kaydı — {{OTURUM_ID}} ({{TARİH}})

**Zorunlu Bağlantılar:** [Ana Kütük](log.md) · [Token Bütçesi](AGENTS.md) · [Kapılar](WORKFLOW.md)
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

**Zorunlu Bağlantılar:** [Ana Kütük](log.md) · [Token Bütçesi](AGENTS.md) · [Kapılar](WORKFLOW.md)

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

**Zorunlu Bağlantılar:** [Ana Kütük](log.md) · [Token Bütçesi](AGENTS.md) · [Kapılar](WORKFLOW.md)

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

### §3.12 `KANIT` Satırı Biçimi (kanonik)

Bir iddia yalnız aşağıdaki 5 alanlı, tek satırlık biçimde kanıtlanır; alanlar `|` ile
ayrılır, alan adı yazılmaz, sıra değişmez:

| # | Alan | Kural | Doğru örnek |
|---|---|---|---|
| 1 | `P<n>` | oturum içinde benzersiz, artan | `P4` |
| 2 | İddia | tek cümle, ölçülebilir ("dosya 480+ satır") | Oturum dosyası 654 satır |
| 3 | Kanıt komutu | **salt-okunur** PowerShell; `C:\versacoder` altında çalışır | `(Get-Content $f).Count` |
| 4 | Çıktıdan satır | çıktının birebir kopyası, yuvarlanmaz, uydurulmaz | `654` |
| 5 | Sonuç | yalnız `PASS` ya da `FAIL` | `PASS` |

```text
P4 | Oturum dosyası G6 sınırını geçiyor | (Get-Content .ai\sessions\2026-10-02-ekran-spec.md).Count | 654 | PASS
P5 | Mojibake yok (G9) | Select-String -Path <dosya> -Pattern '[\u00C3].|[\u00E2][\u20AC]' | 0 | PASS
```

Zorunlu bağlılar: iddia okunmadıysa kanıt satırı yerine `⚠️ VERIFICATION REQUIRED`
yazılır (`§1.2`); `FAIL` satırı silinmez, yanına düzeltme notu ve yeni `P<n>` eklenir;
aynı iddia ikinci kez doğrulanırsa eski satır durur, yenisine `P<n+1>` verilir (`G11`).
Komut çıktısı **satır numarası ya da birebir metin** olarak yazılır — "başarılı",
"tamam", "çalıştı" gibi sonuçsuz kelimeler kanıt sayılmaz (`G8`).

### §3.13 Doldurulmuş Örnek Oturum Kaydı (gerçek biçim)

Aşağıdaki kayıt, tek bir oturumun baştan sona doldurulmuş hâlidir. Tüm alanlar gerçek değerlerle
doldurulmuştur; hiçbir yerinde yer tutucu yoktur. Yeni kayıt açarken bu bloğu kopyalayıp alanları
değiştirmek, boş şablondan başlamaktan daha az hata üretir.

```markdown
---
oturum-kimligi: "k8-tasarim-sozlesmesi-on-seridi"
baslangic: 2026-10-05T14:05:00+03:00
bitis: 2026-10-05T14:52:00+03:00
sure: "47 dk"
durum: "tamamlandi"
ilgili-dosya: "../.ai/.templates/session-log-template.md"
sahip: "golge-ajan"
oturum-butcesi: "model=mimo-v2.6-flash-free · kalan bütçe=%34 · ~128k"
---
```

```text
bağlam: şablon G6 eşiğini karşılamıyor (479/480 satır · 22.7/26 KB)
planlanan işler:
  - [x] alan sözlüğü ekle · tamamlandı · ~10 dk
  - [x] KANIT kanonik biçimi ekle · tamamlandı · ~8 dk
  - [x] bütçe takibi kuralları ekle · tamamlandı · ~12 dk
  - [x] devir (handover) kuralları ekle · tamamlandı · ~12 dk
  - [ ] dolu örnek kayıt ekle · sürüyor · ~5 dk
adımlar:
  14:05 · doküman-uretici · şablon okundu, 479/480 satır ve 22.7/26 KB sapması ölçüldü
  14:17 · kod-yazici · §1.5 alan sözlüğü 31 satır olarak eklendi (1268 B)
  14:25 · kod-yazici · §3.12 KANIT biçimi eklendi; 6 alan zorunlu olarak sayıldı
  14:37 · kod-yazici · §4.8 bütçe kuralları eklendi; 3 düzlem tablosu kuruldu
  14:49 · kod-yazici · §5.9 devir kuralları eklendi; zorunlu üçlü tanımlandı
14:47 · KANIT · kod-yazici · ekledigim satir-sayisi=64 · agirlik=orta · suresi=9s · hata=0 · kanit-dosya=".ai/.templates/session-log-template.md" · kanit-satir=398-461
çıktılar:
  - session-log-template.md · §1.5/§2.3/§3.12/§3.13/§4.8/§5.9 eklendi · G6 eşiği aşıldı
  - session-log-template.md · 22.7 KB → 26 KB üzeri · BOM=YOK · mojibake=0
karar: dolu örnek §3.13'te mi yoksa §6'da mı durmalı? → §3.13 seçildi
  neden: örnek, biçim referansı olduğu için mimari bölümünde aranır; §6 doğrulama bölümüne
  yalnızca çalıştırılabilir komutlar konur.
elnotu: dosyanın doğrulama satırındaki mojibake kalıbı, kalıbın kendisi mojibake sayılmaması
  için kod noktası biçiminde (`[\u00C3].|[\u00E2][\u20AC]`) yazılır; aksi hâlde kalıbı
  gösteren satırın kendisi G9 sayacını 2 artırır ve yanlış FAIL üretir.
```

**Örneğin nasıl okunacağı.** Yukarıdaki kayıtta dört şey ayrıca incelenmelidir:

1. **`bağlam` tek satırdır** ve ölçülebilir bir sapma taşır (479/480 · 22.7/26). Ölçülemeyen
   bağlam maddesi, bir sonraki oturumda yeniden ölçülür.
2. **`planlanan işler` içinde yarım iş vardır** (`sürüyor`). Yarım iş bırakmak yasak değildir;
   yarım işi **işaretsiz** bırakmak yasaktır.
3. **`adımlar` saatli ve eylem bildirimliidir** — `· kod-yazici ·` biçiminde yürütücü adı taşır,
   böylece bir sonraki oturum hangi yürütücünün ne yaptığını görür.
4. **`karar` iki satırdır:** seçenek artı neden. Tek satırda kalan karar, nedeni olmadan
   bırakılmış karardır.

**Bu örnekle şablon arasındaki fark.** Örnek, yer tutucu içermeyen dolu bir oturum kaydıdır ve
kendi başına birkaç yüz satır tutar. Şablonun **boş** hâlinin de G6 eşiğini (≥480 satır · ≥26 KB)
karşılaması gerekir; çünkü G6, doldurulmamış şablonu ölçer — eşik ancak şablonun kendisi
genişletilerek geçilir, örneğin varlığıyla değil.

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

1. `authority: SSOT` hiçbir sürümde değiştirilemez (`G2`).
2. `## §1`..`## §7` başlıklarından biri silinemez (`G3`).
3. `archived` kayıt yalnızca `architect` onayıyla yeniden açılır; geçmiş satırlar **silinmez**, üstüne düzeltme satırı eklenir.
4. Her değişiklik `version` artırır ve `updated` tarihini günceller.
5. `TOKEN:` değeri **geriye dönük düşürülemez**; yanlış sayı sonraki checkpoint'te düzeltilir ve not düşülür.

### §4.5 Hard Guardrails tablosu

| ID | Guardrail | Kural kaynağı | Tespit yöntemi | İhlal Sonucu |
|---|---|---|---|---|
| **G1** | Frontmatter 7 alan eksiksiz | `§3.1` | İlk 10 satır | Dosya geçersiz sayılır |
| **G2** | `authority: SSOT` değişmez | Vault kuralı | Diff | Kayıt geri alınır |
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

### §4.8 Bütçe Takibi Kuralları

`oturum bütçesi` alanı tek başına yeterli değildir; bütçe üç düzlemde izlenir ve üçü de
kayıtta görünür olmalıdır:

| Düzlem | Ne izlenir | Kayıtta nerede | Aşım işareti |
|---|---|---|---|
| Token | model kotasının tüketimi | `oturum bütçesi` alanı | `kalan bütçe` < %20 ise `UYARI` |
| Zaman | adım/oturum süresi | `süre` alanı + §4.4 satırları | bir adım > planlanan 2 katı ise ` sapma` |
| İş | tamamlanan/adımlanan iş kalemi | `adımlar` + `çıktılar` | `planlanan işler` içinde yarım kalan her madde |

**Bütçe güncelleme zamanlaması.** Bütçe bilgisi adım sonunda, `KANIT` satırından **hemen sonra**
güncellenir — asla oturum sonuna bırakılmaz. Bırakılan bütçe, kesilen oturumda hiç yazılmamış
sayılır ve bir sonraki oturuma yanlış başlangıç maliyeti taşır.

**Kalan bütçe yazım kuralı.** Mutlak sayı yerine yüzde ve mutlak değer birlikte yazılır
(`%34 · ~128k`). Yalnızca yüzde yazan kayıt, model değişince anlamsızlaşır; yalnızca mutlak
yazan kayıt, kota büyüklüğünü yansıtmaz.

**Aşım senaryosu.** Bütçe %20'nin altına indiğinde kayıt şu sırayla güncellenir:

1. `durum` alanı `planlandı` dışındaki bir değerse, mevcut durum korunur.
2. `planlanan işler` içinde tamamlanmamış kalemler öncelik sırasına göre yeniden işaretlenir.
3. `elnotu` alanına tek cümlelik aşım nedeni yazılır (ör. *"kota daraldı, K8 ertelendi"*).
4. Oturum kapanışı yapılıyorsa `durum` = `eldegistirdi` ve bir sonraki oturuma devreden işler
   `planlanan işler` içinde `→` işaretiyle işaretlenir.

**Aşım yasağı.** `kalan bütçe` alanı asla `0` yazılmaz; kota dolduysa kayıt `durum` = `bloke`
olarak kapatılır ve nedeni `elnotu`'na yazılır. Sıfır bütçe, bir sonraki oturumun hesaplarını
bozar çünkü "henüz başlamadı" ile "tamamen bitti" aynı değere düşer.

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

### §5.9 Oturlar Arası Devir (Handover) Kuralları

Devir, oturumun son 5 dakikasında yapılır ve üç zorunlu çıktı üretir. Devirsiz kapatılan oturum,
bir sonraki oturumun başında yeniden keşif maliyeti doğurur.

**Devir paketi (zorunlu üçlü):**

| # | Çıktı | Nereye yazılır | Format |
|---|---|---|---|
| 1 | Kalan iş listesi | mevcut oturumun `planlanan işler` alanı | `- [ ] iş · durum · ~süre` |
| 2 | Taşınan bağlam | bir sonraki oturumun `bağlam` alanı | en fazla 3 madde, her biri tek satır |
| 3 | Karar borcu | bir sonraki oturumun `elnotu` alanı | `karar: <seçenek> seçilmedi · neden: <neden>` |

**Devir sırası.** Bağlam önce yazılır, işler sonra. Sıranın önemi vardır: işleri yazdıktan sonra
bağlam yazmak, işlerin gerekçesini geriye doğru uydurmaya teşvik eder.

**Devredilen işin tanım kuralı.** Devredilen her iş tek satırda tamamlanabilir olmalıdır.
"Arayüzü bitir" devredilebilir bir iş değildir; "form tasarım şablonunun §3 tablosunu doldur
(~40 dk)" devredilebilir. Tanımı yazılamayan iş, iş değil hedeftir ve devredilmez — hedefler
`bağlam` alanına taşınır.

**Bağlam taşınma kuralı.** Bir sonraki oturuma en fazla 3 bağlam maddesi taşınır. Dördüncü
madde, oturumun zaten fazla yük taşıdığını gösterir; bu durumda madde sayısı 3'e indirilir ve
elenen maddeler `elnotu`'na tek cümleyle sıkıştırılır.

**Karar borcu (decision debt).** Cevaplanmamış her soru, bir borçtur ve iki yerde görünür olmalıdır:
kaynak oturumda `elnotu` olarak, hedef oturumda `bağlam` olarak. Borç yalnızca tek taraflı
yazılmışsa (kaynak oturumda kalmışsa) hedef oturum onu göremez ve soru yeniden sorulur —
tekrar sorulan soru, devir yapılmamış demektir.

**Devir doğrulaması.** Devirden hemen sonra şu üç soruya yanıt aranır; biri "hayır" ise devir
eksiktir:

1. Bir sonraki oturum, tek başına ilk maddeyi açıp uygulayabilir mi?
2. Taşınan bağlam 3 maddeyi aşıyor mu? (aşıyorsa eleyin)
3. Cevapsız her soru iki oturumda da görünüyor mu? (görünmüyorsa borcu yazın)

**Devir yasağı.** Kapatılan oturum, açık oturumun işlerini kendi `durum` alanına kopyalayarak
"devretmiş" olmaz. Devir = kaynakta işaretlemek **artı** hedefte yazmak; tek başına işaretleme
yalnızca kapanıştır, devir değildir.

## §6 Doğrulama

### §6.1 Onay kutusu listesi

- [ ] 1. Frontmatter `title, type, category, status, version, updated, authority` 7 alan eksiksiz
- [ ] 2. `authority: SSOT` yazıyor, `updated` BUGÜN tarihinde
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
| [Ana Kütük](../log.md) | Tek satır özet | `log.md` |
| Oturum Dizini | Dosya listesi | `sessions/` |
| [Token Bütçesi](../AGENTS.md) | 50.000 sınırı | `.rules/token-budget` |
| [Kanıt ve Doğrulama](../CLAUDE.md) | `claim → evidence` | `.rules/evidence-and-verification` |
| [Kural Dizini](../.rules/index.md) | 7 kural dosyası | `.rules/` |
| [ADR Dizini](adr/adr-index.md) | ADR-001..008 | `adr/` |
| [Kapılar](../WORKFLOW.md) | GATE-004a..006 | `gates/` |
| [Ajan Kataloğu](../AGENTS.md) | 14 ajan + 8 disiplin | `.agents/` |

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
