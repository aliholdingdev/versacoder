---
title: "ADR Şablonu — Secret Yönetimi, Authentication, Oturum, Kripto ve Threat Model Varyantı"
type: template
category: adr
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "guvenlik ADR sablonu: secret yonetimi, auth, oturum, kripto ve threat model"
---

# ADR Şablonu — Secret Yönetimi, Authentication, Oturum, Kripto ve Threat Model Varyantı

**Zorunlu Bağlantılar:** [adr-template](adr-template.md) · [adr-index](adr-index.md) · [Vault](../../index.md) · [Karar Defteri](../../.decisions/index.md) · [security-rules](../../.rules/security-rules.md) · [keys](../../keys.md) · [ARCHITECTURE](../../architecture/index.md)

---

## §1 Amaç

### §1.1 Neden bu varyant gerekli

VersaCoder LLM trafiği üç yoldan geçer: **OpenAI-uyumlu API** (`ProviderFactory`), **Ollama local** (`OllamaProvider`) ve ücretsiz sağlayıcı (`OpenCodeFreeProvider`); yönlendirme `SmartModelRouter` ile yapılır. Bu yolların hepsi anahtar, endpoint ve oturum verisi taşır. Güvenlik kararı bir "İyi pratik" cümlesiyle değil, **tehdit, saldırı yüzeyi, kontrol ve doğrulama kanıtı** ile yazılmalıdır; aksi halde ADR, sıradan bir tercih belgesine döner.

Bu varyant zorunlu olduğu durumlar:

1. **Secret yönetimi** — anahtar/parola/connection string nerede tutulur, kim okur, nasıl döner (`keys.md` bir sözlüktür, değer deposu değildir).
2. **Authentication / yetki** — uygulama içi erişim, API çağrısı, rol/izin modeli.
3. **Oturum** — süre, yenileme, sonlandırma, eşzamanlılık.
4. **Kripto** — şifreleme/özetleme seçimi, nonce/IV, anahtar uzunluğu.
5. **Threat model** — varlık, tehdit, etki, azaltıcı kontrol tablosu (§3.3).

Kanıt zorunludur: **tarama çıktısı = 0 secret**, **threat model satırı ≥ varlık sayısı**.

### §1.2 Disk Kanıtları ve Boşluk (YAGNI)

| # | İddia | Disk Kanıtı (2026-09-25) | Durum |
|---|---|---|---|
| E1 | `.ai/keys.md` anahtar sözlüğü olarak var | `C:\versacoder` yalnız `.git` gösterdi | ⚠️ VERIFICATION REQUIRED |
| E2 | `.rules/security-rules.md` yasak/mecburi listesi tutar | `.ai/.rules/` henüz yok | ⚠️ VERIFICATION REQUIRED |
| E3 | Sağlayıcılar: `ProviderFactory`, `SmartModelRouter`, `OllamaProvider`, `OpenCodeFreeProvider` | Kod diskte değil | ⚠️ VERIFICATION REQUIRED |
| E4 | Streaming yanıt + oturum yenileme davranışı | Kod yok | ⚠️ VERIFICATION REQUIRED |
| E5 | Secret taraması = 0 eşleşme | Tarama yapılamadı | ⚠️ VERIFICATION REQUIRED |
| E6 | Bu şablon yazıldı | `.ai/.templates/adr/adr-security-template.md` | ✅ Doğrulandı |
| E7 | Kopyalama yasak listesi (CoreMusic / vs-aivault) | Erişim yasak | 🔒 Kopyalama YASAK |

**YAGNI kararı:** "Güvenli" iddiası tek başına yeterli değildir; tarama çıktısı yoksa satır `⚠️` kalır ve `§5.2` H1'e bağlanır. ADR'ye hiçbir gerçek anahtar/parola yazılır mı? **Hayır** — yazılan yalnızca *ad, sahip, rotasyon periyodu* (G3).

### §1.3 Karar Türleri tablosu

| Karar Türü | Tipik Konu | Bu varyant? | Kanıt |
|---|---|---|---|
| Secret saklama/erişim | `keys.md`, ortam değişkeni, rotasyon | ✅ Evet | Tarama = 0 + sözlük satırı |
| Authentication / yetki | API çağrısı, rol denetimi | ✅ Evet | Yetki matrisi |
| Oturum | Süre, yenileme, kapatma | ✅ Evet | Süre sayısı + test |
| Kripto seçimi | Özet/şifreleme, nonce | ✅ Evet | Parametre tablosu |
| Threat model | Varlık → tehdit → kontrol | ✅ Evet | §3.3 tablosu dolu |
| Şema/migration | `schema_version`, saf SQL | Hayır → `adr-data-template.md` | Migration tablosu |
| DevExpress sürümü | 25.2.8, Office2010 | Hayır → `adr-devexpress-template.md` | Sürüm kilidi |
| Dosya/dizin adı, log biçimi | Geçici tercih | Hayır → `adr-nygard-template.md` | 1 sayfa ADR |

### §1.4 Neden web araştırması zorunlu

Güvenlik iddiaları doğrulanmadan tekrarlanan bilgilerdir; kaynağı olmayan iddia tehlikelidir:

1. **Algoritma/parametre iddiası** — özet/şifreleme uzunluğu, mod (CBC/GCM) → resmi standart/üretici belgesi.
2. **Varsayılan davranış** — kütüphanenin varsayılan timeout/nonce davranışı → paket belgesi.
3. **Zafiyet iddiası** — "eski sürüm etkilenir" → advisory/CVE kaydı; kaynak yoksa iddia yazılmaz.
4. **Yasak/mecburi** — "bu projede X yasak" → `.ai/.rules/security-rules.md` (yerel kaynak esastır).
5. **Sağlayıcı politikası** — OpenAI-uyumlu endpoint/Ollama local güvenlik varsayılanları → üretici dokümantasyonu.

Her satır `§1.4`'e URL + erişim tarihiyle girer; tarihsiz satır `§6.1` kutusundan düşer.

---

## §2 Kapsam

| Alan | Kapsam (BU ŞABLON) | Kapsam Dışı |
|---|---|---|
| Secret | Saklama yeri, erişim, rotasyon, log temizliği | Anahtar değerinin kendisi (hiçbir ADR'ye girmez) |
| Authentication | Sağlayıcı çağrısı, uygulama içi yetki | UI tasarımı (7 ekran) |
| Oturum | Süre, yenileme, eşzamanlılık, kapatma | Veri kalıcılığı (`schema_version`) |
| Kripto | Algoritma + parametre + nonce/IV | Uygulama mimarisi geneli |
| Threat model | Varlık → tehdit → etki → kontrol | Penetrasyon testi çalıştırma |
| Log/telemetri | Sızıntı önleme, maskeleme | Log formatı (Nygard hafif) |

### §2.1 Dosya Tipi → Sorumlu eşlemesi

| Dosya Tipi | Sorumlu Rol | Bu şablon? | Not |
|---|---|---|---|
| `.ai/keys.md` | `security` | ✅ Evet | Sözlük: ad/sahip/rotasyon; değer yok |
| `.ai/.rules/security-rules.md` | `security` | Kısmen | Kural ≠ karar; ADR buraya bağlanır |
| `.ai/.decisions/ADR-NNN-security-*.md` | `security` + `architect` | ✅ Evet | Bu şablonla yazılır |
| Kod (sağlayıcı/oturum) | `developer` | Kısmen | Kanıt: tarama + test |
| `.ai/log.md`, `MEMORY.md` | `master-orchestrator` | Hayır | Secret oraya da girmez |
| `.ai/architecture/index.md` | `architect` | Kısmen | Yetki/oturum akışı |
| `.ai/keys.md` değerleri | `devops-engineer` | Hayır | Değer ortam değişkeni/secret store |

### §2.2 Kapsam Sınırı Testi (4 soru)

1. **S1 Secret, auth, oturum, kripto veya tehdit modeli içeriyor mu?** → `EVET` beklenir.
2. **S2 Tek paragrafla "güvenli" iddiası yeterli mi?** → `HAYIR` beklenir (tehdit tablosu şart).
3. **S3 Tarama çıktısı (0 secret) üretilebiliyor mu?** → `EVET` beklenir.
4. **S4 Şema/migration veya DevExpress stili ana konu mu?** → `HAYIR` beklenir.

S1 `HAYIR` → `adr-nygard-template.md`. S4 `EVET` → yanlış varyant, **DUR**.

---

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

```yaml
---
title: "ADR-NNN — <ingilizce-slug>"
type: adr
category: security
status: draft            # draft | review | accepted | rejected | superseded
version: 0.1.0
updated: 2026-09-25
authority: SSOT
supersedes: null
superseded_by: null
decision_date: 2026-09-25
risk: H                  # güvenlik kararları H
reversible: false
related_gates: [GATE-004]
related_rules: [security-rules, coding-standards]
template: adr-security-template.md
---
```

### §3.2 H1 iskeleti

```markdown
# ADR-NNN: <İngilizce karar başlığı>
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [security-rules](../../.rules/security-rules.md) · [keys](../../keys.md)
```

### §3.3 §1 İskeleti (Amaç)
```markdown
## §1 Amaç
### §1.1 Neden bu varyant gerekli     ← secret/auth/oturum/kripto gerekçesi
### §1.2 Disk kanıtları + tarama sonucu (iddia | kanıt | durum)
### §1.3 Karar türü ve seçilen varyant
### §1.4 Web araştırması: URL + erişim tarihi
```
### §3.4 §2 İskeleti (Kapsam)
```markdown
## §2 Kapsam  →  | Kapsam | Kapsam Dışı |
### §2.1 Dosya tipi → sorumlu
### §2.2 Kapsam sınırı testi: S1..S4
```
### §3.5 §3 İskeleti (Karar)
```markdown
## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar    ← "…karar verildi." ile biter
### §3.2 Secret akışı: kaynak | saklama | okuyan | rotasyon
### §3.3 Threat model: varlık | tehdit | etki | azaltıcı kontrol
### §3.4 Oturum + kripto parametreleri: süre, nonce, algoritma
```
### §3.6 §4 İskeleti (Sonuçlar)
```markdown
## §4 Kurallar ve Sonuçlar
### §4.1 Kazanımlar (ölçülebilir: 0 secret, N kontrol, N saniye oturum)
### §4.2 Ödenen bedel (operasyon yükü, UX kısıtı)
### §4.3 Guardrails → bu şablon §4.5
```
### §3.7 §5 İskeleti
```markdown
## §5 Workflow  →  §5.1 11 adım | §5.2 hata modları | §5.3 salt-okunur PowerShell
```
### §3.8 §6 İskeleti
```markdown
## §6 Doğrulama  →  §6.1 ≥18 kutu | §6.2 red sinyalleri | §6.3 sürüm | §6.4 report | §6.5 güvenlik kontrolleri
```
### §3.9 §7 İskeleti
```markdown
## §7 Referanslar  →  §7.1 bağlantıklar | §7.2 Red Team (hata | neden yanlış | doğrusu)
```

### §3.10 Doldurulmuş gerçek VersaCoder örneği (ADR-010 llm-provider-secret-flow)

Dosya: `.ai/.decisions/review/ADR-010-llm-provider-secret-flow.md` (numara tahsisi: `.decisions/index.md` sonraki boş slot)

```markdown
---
title: "ADR-010 — LLM Provider Secret Flow"
type: adr
category: security
status: review
version: 0.2.0
updated: 2026-09-25
authority: SSOT
supersedes: null
superseded_by: null
decision_date: 2026-09-25
risk: H
reversible: false
related_gates: [GATE-004]
related_rules: [security-rules, coding-standards]
template: adr-security-template.md
---
# ADR-010: LLM Provider Secret Flow (`keys.md` sözlük + ortam değişkeni)
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [security-rules](../../.rules/security-rules.md) · [keys](../../keys.md)

## §1 Amaç
### §1.1 Neden bu varyant gerekli
`ProviderFactory` üç sağlayıcı kurar: OpenAI-uyumlu API, `OllamaProvider` (local) ve
`OpenCodeFreeProvider`; seçim `SmartModelRouter` ile yapılır ve yanıt streaming alınır. Her
çağrıda kimlik bilgisi belleğe ve isteğe girer. Karar, anahtarın nereden okunduğunu, log'a
sızıp sızmadığını ve oturumun ne zaman kapandığını tanımlamak zorunda.
### §1.2 Disk kanıtları
| İddia | Kanıt | Durum |
|---|---|---|
| `.ai/keys.md` yalnız ad/sahip/rotasyon içeriyor | Dosya diskte yok (2026-09-25) | ⚠️ VERIFICATION REQUIRED |
| Kaynak kodda/ADR'de secret = 0 eşleşme | Tarama yapılamadı | ⚠️ VERIFICATION REQUIRED |
| Ollama local çağrıda ağ dışına anahtar çıkmıyor | Kod yok | ⚠️ VERIFICATION REQUIRED |
| Oturum/istek timeout değeri tanımlı | Kod yok | ⚠️ VERIFICATION REQUIRED |
### §1.3 Karar türü
Secret + auth + oturum → bu varyant (`adr-security-template.md`).
### §1.4 Araştırılan kaynaklar
| Kaynak | URL | Erişim | İddia |
|---|---|---|---|
| OpenAI-uyumlu API kimlik belgesi | resmi API belgesi | 2026-09-25 | `Authorization` başlığı biçimi |
| Ollama yapılandırma | resmi doküman | 2026-09-25 | Local uç nokta varsayılanı |

## §2 Kapsam
| Kapsam | Kapsam Dışı |
|---|---|
| Sağlayıcı kimlik bilgisi akışı | Model kalitesi/yönlendirme mantığı |
| Oturum/istek timeout, kapatma | Şema/migration (ADR-004) |
| Log maskeleme | DevExpress görünüm güvenliği (ADR-001) |
### §2.1 Dosya tipi → sorumlu
`keys.md` → `security`; `ProviderFactory`/`OllamaProvider` kodu → `developer`; tarama → `reviewer`.
### §2.2 Kapsam sınırı testi
S1 **EVET** (secret + oturum). S2 **HAYIR** (tehdit tablosu şart). S3 **EVET** (tarama 0).
S4 **HAYIR** (şema/stil ana konu değil).

## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar
Sağlayıcı kimlik bilgileri depoya **asla yazılmaz**: yalnız ortam değişkeninden okunur; `.ai/keys.md`
yalnız anahtar adı, sahibi ve rotasyon periyudunu tutar; her istekte timeout uygulanır ve yanıt
başlıkları/log satırları maskelenir — secret akışı, oturum ve log maskelemesi karar verildi.
### §3.2 Secret akışı
| Kaynak | Saklama | Okuyan | Rotasyon |
|---|---|---|---|
| OpenAI-uyumlu API anahtarı | Ortam değişkeni | `ProviderFactory` | 90 gün |
| Ollama local | Kimlik yok (local) | `OllamaProvider` | — |
| Ücretsiz sağlayıcı anahtarı | Ortam değişkeni | `OpenCodeFreeProvider` | 30 gün |
| `.ai/keys.md` | Sözlük (ad/sahip) | `security` | Kayıt tutar |
### §3.3 Threat model
| Varlık | Tehdit | Etki | Azaltıcı kontrol |
|---|---|---|---|
| API anahtarı | Kaynak koda/log'a yazma | H | Tarama = 0 + maskeleme |
| İstek logu | Başlık/beden sızıntısı | H | `Authorization` maskesi |
| Ollama local | Ağ üzerinden dinleme | M | Local uç nokta + ağ kısıtı |
| Oturum/istek | Süresiz bekleme | M | Timeout + iptal |
| `keys.md` | Depoya yanlışlıkla değer yazma | H | Sözlük formatı + denetim |
### §3.4 Oturum + kripto parametreleri
- İstek timeout: {{SAYI}} sn; iptal: `CancellationToken` zinciri.
- Yenileme: anahtar oturumu yok; her çağrı yeni kimlik doğrular.
- Kripto: yalnız taşıma katmanı (HTTPS/localhost); özel şifreleme gerekçesi yoksa **yazılmaz** (YAGNI).

## §4 Kurallar ve Sonuçlar (özet — §4.5/§5.1/§6.1 karşılıklarıyla yazılır)
- §4.1: 0 secret eşleşmesi; 5 threat model satırı; timeout = {{SAYI}} sn.
- §4.2: operasyon yükü (rotasyon 90/30 gün), local Ollama'da kimlik yokluğu riski.
- §4.3: G1, G3 (secret yasak), G5, G7 (biçim), G8 (oturum), G9 (threat model), G14.
```

### §3.11 Kopyala-yapıştır tam kalıp (`{{PLACEHOLDER}}` alanlı)

```markdown
---
title: "{{ADR_ID}} — {{ingilizce-slug}}"
type: adr
category: security
status: draft
version: 0.1.0
updated: {{YYYY-MM-DD}}
authority: SSOT
supersedes: {{ADR_ID|NULL}}
superseded_by: null
decision_date: {{YYYY-MM-DD}}
risk: H
reversible: false
related_gates: [GATE-004]
related_rules: [security-rules, coding-standards]
template: adr-security-template.md
---
# {{ADR_ID}}: {{INGILIZCE_KARAR_BASLIGI}}
**Zorunlu Bağlantılar:** [.decisions/index](../index.md) · [security-rules](../../.rules/security-rules.md) · [keys](../../keys.md)

## §1 Amaç
### §1.1 Neden bu varyant gerekli
{{SECRET/AUTH/OTURUM/KRIPTO GEREKÇESİ — tek paragraf}}
### §1.2 Disk kanıtları
| İddia | Kanıt (yol / tarama çıktısı) | Durum |
|---|---|---|
| {{IDDIYA_1}} | {{YOL_1}} | {{✅|⚠️ VERIFICATION REQUIRED}} |
| {{IDDIYA_2}} | {{TARAMA_SONUCU}} | {{✅|⚠️ VERIFICATION REQUIRED}} |
### §1.3 Karar türü ve seçilen varyant
{{TUR}} → adr-security-template.md.
### §1.4 Araştırılan kaynaklar
| Kaynak | URL | Erişim | İddia |
|---|---|---|---|
| {{KAYNAK}} | {{URL}} | {{YYYY-MM-DD}} | {{IDDIYA}} |

## §2 Kapsam
| Kapsam | Kapsam Dışı |
|---|---|
| {{KAPSAM}} | {{KAPSAM_DISI}} |
| {{KAPSAM_2}} | {{KAPSAM_DISI_2}} |
### §2.1 Dosya tipi → sorumlu
{{DOSYA_TIPI}} → {{ROL}}; {{DOSYA_TIPI_2}} → {{ROL_2}}.
### §2.2 Kapsam sınırı testi
1. S1 secret/auth/oturum/kripto/threat model içeriyor mu? {{EVET|HAYIR}}.
2. S2 tek paragraf yeterli mi? {{EVET|HAYIR}}.
3. S3 tarama çıktısı (0 secret) üretilebiliyor mu? {{EVET|HAYIR}}.
4. S4 şema/stil ana konu mu? {{EVET|HAYIR}}.

## §3 Mimari / Karar (Decision)
### §3.1 Seçilen karar
{{KARAR — "…karar verildi." ile biter}}
### §3.2 Secret akışı
| Kaynak | Saklama | Okuyan | Rotasyon |
|---|---|---|---|
| {{KAYNAK}} | {{SAKLAMA}} | {{OKUYAN}} | {{ROTASYON}} |
### §3.3 Threat model
| Varlık | Tehdit | Etki | Azaltıcı kontrol |
|---|---|---|---|
| {{VARLIK}} | {{TEHDIT}} | {{L|M|H}} | {{KONTROL}} |
### §3.4 Oturum + kripto
- Timeout: {{SAYI}} sn · Yenileme: {{BICIM}} · Algoritma: {{ALGORITMA|YAGNI}}

## §4 Kurallar ve Sonuçlar
### §4.1 Kazanımlar
- {{KAZANIM}} — {{ÖLÇÜT (0 secret, N kontrol, N sn)}}
### §4.2 Ödenen bedel
- {{BEDEL}} — {{MALİYET}}
### §4.3 Guardrails
{{G_LISTESİ}} — tamı bu şablon §4.5.

## §5 Workflow
### §5.1 Uygulama adımları
| # | Adım | Girdi | Çıktı | Yapan |
|---|---|---|---|---|
| 1 | {{ADIM}} | {{GIRDI}} | {{CIKTI}} | {{ROL}} |
### §5.2 Hata modları
| Kod | Hata | Belirti | Düzeltme |
|---|---|---|---|
| H1 | {{HATA}} | {{BELIRTI}} | {{DUZELTME}} |
### §5.3 Salt-okunur doğrulama
~~~powershell
# 1 — {{AMAÇ}} olmalı: {{BEKLENEN}}
Select-String -Path "{{DOSYA}}" -Pattern "sk-|api_key|password="
~~~

## §6 Doğrulama
### §6.1 Onay kutuları
- [ ] 01 Frontmatter 11 alan eksiksiz, authority: SSOT
- [ ] 02 §1–§7 bölümlerinin 7'si de mevcut
- [ ] 03–18 … (bu şablon §6.1 listesi)
### §6.2 Red sinyalleri
| Sinyal | Koşul | Aksiyon |
|---|---|---|
| RS-1 | {{KOŞUL}} | DUR + `review`'a dön |
### §6.3 Sürüm/supersede
{{MAJOR/MINOR/PATCH + zincir}}
### §6.4 Quality Report
{{§6.4 kalıp bloğu}}
### §6.5 Güvenlik ek kontrolleri
{{EK-1..EK-5}}

## §7 Referanslar
### §7.1 VersaCoder bağlantıkları
| Bağlantı | Yol | Rol |
|---|---|---|
| {{AD}} | `{{YOL}}` | {{ROL}} |
### §7.2 Red Team — sık yapılan hatalar
| Hata | Neden yanlış | Doğrusu |
|---|---|---|
| {{HATA}} | {{NEDEN}} | {{DOĞRUSU}} |
```

---

## §4 Kurallar

### §4.1 Durum döngüsü

```text
  draft --yazıldı--> review --4. aşama--> accepted
     ^                  |                    |
     | düzeltme         | red sinyali         | yeni ADR eziyor
     +------------------+--------------------> rejected / superseded

  superseded: eski ADR silinmez; superseded_by alanına yeni ADR yazılır.
```

| Durum | Anlamı | Kim koyar | Çıkış koşulu |
|---|---|---|---|
| `draft` | Tehdit tablosu yazılıyor | `security` | §6.1 ≥18 kutu + tarama 0 |
| `review` | Güvenlik denetiminde | `security` + `reviewer` | RS-1..RS-7 yok |
| `accepted` | Güvenlik kuralı bağlayıcı | `master-orchestrator` | 4. aşama + defter + `security-rules` senkronu |
| `rejected` | Kabul edilemez risk | `master-orchestrator` | Gerekçe ≥3 satır + alternatif |
| `superseded` | Yeni tehdit/exploit bilgisi ezdi | `docs-writer` | `superseded_by` dolu |

### §4.2 4 aşamalı onay akışı

| Aşama | Ad | Yapan | Girdi | Çıktı | Süre |
|---|---|---|---|---|---|
| 1 | Tehdit modeli + tarama | `security` | Kod/AQDebug | §3.3 + §1.2 | ≤45 dk |
| 2 | Mimari denetim | `architect` | ADR | Akış/oturum uyumu | ≤30 dk |
| 3 | Kod incelemesi | `reviewer` | Revize ADR | Secret taraması + maskeleme kanıtı | ≤45 dk |
| 4 | Kabul / red | `master-orchestrator` | §6.4 report | `accepted`/`rejected` + defter | ≤30 dk |

**Bütçe: 150 dakika.** Aşama 4'te `.ai/.rules/security-rules.md` ile çelişki varsa karar `rejected` olur; kural dosyası ADR'den **türetilir**.

### §4.3 Risk ölçeği L / M / H

| Seviye | Tanım | Güvenlik eşikleri | Onay | Geri dönüş |
|---|---|---|---|---|
| **L** | Maskeleme deseni, log alanı | Yeni log alanı maskesi | Aşama 1–2 | Dakika |
| **M** | Timeout değeri, yenileme biçimi | Oturum süresi, iptal akışı | Aşama 1–3 | Saat |
| **H** | Secret saklama yeri, kripto seçimi, yetki modeli | Anahtar taşıma, algoritma değişimi | 1–4 + `related_gates` | Gün–hafta (rotasyon + revizyon) |

### §4.4 Frozen / değişiklik kontrolü (2 istisna)

`accepted` güvenlik ADR'si frozen'dır. Yalnız 2 istisna:

| İstisna | Koşul | Yapılabilir | Yasak |
|---|---|---|---|
| **1 — Kanıt/zafiyet güncellemesi** | Yeni CVE/advisory, tarama çıktısının güncellenmesi | `version.minor` + `updated` + §7.1 revizyon satırı | Threat model satırını sessizce silmek |
| **2 — Yeni kontrolle supersede** | Daha güçlü kontrol eski kararı ezdi | Yeni ADR; eski `superseded` + `superseded_by` | Eski dosyayı silmek, `security-rules`'u sessiz değiştirmek |

Zafiyet bilgisi gelince bekleme yok: ADR hemen `review`'a alınır (istisna 1).

### §4.5 Hard Guardrails tablosu (G1..G14)

| # | Guardrail | Kural | İhlal Sonucu |
|---|---|---|---|
| G1 | Frontmatter SSOT | `authority: SSOT` + 11 alan eksiksiz | Aşama 1 **DUR**; `draft` |
| G2 | Slug İngilizce, metin Türkçe | mojibake yasak | Commit geri alınır |
| G3 | Secret metne giremez | Anahtar/parola/connection string ADR'ye, `.ai`'a, log'a yazılamaz | **RED**: `rejected` + kaynak temizliği + `security` bildirimi |
| G4 | Yetki açık yazılır | Her erişim/çağrı için yetki satırı (kim, ne, hangi koşul) | Yetkisiz satır → Aşama 2 **DUR** |
| G5 | Kanıt zorunlu | Tarama çıktısı (0 secret) ve threat model satırı olmadan `accepted` yok | `⚠️` + H1; <3 kanıt → `draft` |
| G6 | Kopyalama yasak | CoreMusic / vs-aivault `.ai` içeriği kopyalanamaz | ADR **reddedilir** |
| G7 | Biçim/parametre sabit | Özet/şifreleme uzunluğu ve mod §3.4'te sayısal yazılır | Sayısız karar → Aşama 2 **DUR** |
| G8 | Oturum/timeout zorunlu | Timeout değeri ve iptal yolu yoksa akış onaylanmaz | Aşama 4 **DUR** |
| G9 | Threat model zorunlu | §3.3'te varlık sayısı kadar satır | Eksik satır → `draft`; eksik varlık → Aşama 4 **DUR** |
| G10 | Coverage korunur | Güvenlik kodu coverage %90 altına düşemez | Job kırmızı; ADR `review` |
| G11 | TreatWarningsAsErrors | Yeni kod uyarısız derlenmeli | Derleme kırılırsa karar uygulanamaz |
| G12 | Defter + kural senkronu | `accepted` → `index.md` + `security-rules.md` güncelleme | Eksik senkron → fiilen `draft` |
| G13 | Supersede zinciri | Ezilen ADR `superseded` + `superseded_by`; silinmez | Zincir koparsa defter denetimi **DUR** |
| G14 | Salt-okunur doğrulama | §5.3 yalnız `Get-*`/`Select-String`/`Test-Path` | Yazma komutlu blok silinir |

---

## §5 Workflow

### §5.1 11 adımlı tablo + zaman bütçesi

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|---|---|---|---|---|
| 1 | Karar anını yakala | GATE-004 / `log.md` | "ADR gerekli" | `master-orchestrator` | 5 dk |
| 2 | Varyant seç | §1.3 + §2.2 | Bu şablon | `architect` | 5 dk |
| 3 | Numara tahsis et | `.decisions/index.md` | `ADR-NNN` | `docs-writer` | 5 dk |
| 4 | Varlık envanteri | Kod + `keys.md` | §3.3 varlık sütunu | `security` | 20 dk |
| 5 | Secret taraması | §5.3 komut 1–3 | 0 eşleşme | `reviewer` | 15 dk |
| 6 | Threat model yaz | Varlık listesi | §3.3 tam satırlar | `security` | 25 dk |
| 7 | Web araştırması | Belge/CVE | §1.4 URL + tarih | `researcher` | 20 dk |
| 8 | Karar + sonuçlar | Model + tarama | §3.1–§3.4, §4.1–§4.3 | `security` | 25 dk |
| 9 | Doğrula | §6.1 kutular | İşaretli liste | `reviewer` | 15 dk |
| 10 | Quality Report | §6.4 kalıp | `report` bloğu | `reviewer` | 10 dk |
| 11 | Kabul + defter + kural | Report | `accepted` + `index.md` | `master-orchestrator` | 15 dk |

**Bütçe: 155 dakika** → §4.2 150 dk ile çatışırsa adım 7 kısaltılır, **adım 5 (tarama) kısaltılamaz**.

### §5.2 Hata Modları tablosu

| Kod | Hata modu | Belirti | Kurtarma | Guardrail |
|---|---|---|---|---|
| H1 | Kanıtsız "güvenli" iddiası | §1.2/§3.3 boş | Tarama + model satırı üret, `draft` | G5 |
| H2 | Secret metne yapıştırdı | `sk-`, `password=`, `api_key` eşleşmesi | Değeri sil, ortam değişkenine taşı, `rejected` | G3 |
| H3 | Log sızıntısı | Yanıt/log satırında başlık görünüyor | Maskeleme deseni ekle | G3 |
| H4 | Süresiz oturum/istek | Timeout yok | Timeout + iptal yolu ekle | G8 |
| H5 | Eksik threat model | Varlık 5, satır 3 | Kalan varlıkları ekle; `draft` | G9 |
| H6 | Sayısız kripto parametresi | "güçlü algoritma" yazıyor | Uzunluk + mod + nonce yaz | G7 |
| H7 | Kural/ADR çelişkisi | `security-rules.md` başka söylüyor | Türetmede düzelt; Aşama 4 DUR | G12 |
| H8 | Frozen ihlili | `accepted` metin değişikliği | Geri al; yeni ADR yaz | §4.4 |
| H9 | Yetki matrisi eksik | "gerekirse bakılır" | Yetki satırını yaz | G4 |

### §5.3 Salt-okunur PowerShell komut bloğu (6 komut)

```powershell
# 1 — Kaynak kodda olası secret eşleşmesi ne olmalı: 0 (G3)
(Select-String -Path ".\**\*.cs" -Pattern "sk-|api_key\s*=|password\s*=" -ErrorAction SilentlyContinue).Count

# 2 — ADR/ai içinde bağlantı dizesi ne olmalı: 0 (G3)
(Select-String -Path ".\.ai\**\*.md" -Pattern "Data Source=|ConnectionString=" -ErrorAction SilentlyContinue).Count

# 3 — keys.md satır sayısı ne olmalı: 0 değil (sözlük var), değer sütunu yok
(Select-String -Path ".\.ai\keys.md" -Pattern "\|").Count

# 4 — security-rules.md var mı olmalı: True (G12)
Test-Path -Path ".\.ai\rules\security-rules.md"

# 5 — Threat model satırı geçen ADR ne olmalı: en az 1
(Select-String -Path ".\.ai\decisions\*.md" -Pattern "Threat model").Count

# 6 — Bu şablonun satır sayısı ne olmalı: 480-660
(Get-Content -Path ".\.ai\.templates\adr\adr-security-template.md").Count
```

Blok salt-okunurdur (G14): `Set-`, `Remove-`, `New-`, `Invoke-Expression` yoktur. Değer taşıma/rotasyon işlemi ayrı, onaylı bir iş akışında yapılır.

---

## §6 Doğrulama

### §6.1 Onay kutusu listesi (18 madde)

- [ ] 01 — Frontmatter 11 alan eksiksiz (`title…template`).
- [ ] 02 — `authority: SSOT` birebir yazılı.
- [ ] 03 — `status` ∈ {draft, review, accepted, rejected, superseded}.
- [ ] 04 — H1 `ADR-NNN:` ile başlıyor.
- [ ] 05 — `**Zorunlu Bağlantılar:**` satırı var, göreli wiki-link (`keys.md` bağlantısı dahil).
- [ ] 06 — §1…§7 bölümlerinin 7'si de mevcut.
- [ ] 07 — §1.2 her satırda `✅` ya da `⚠️ VERIFICATION REQUIRED`.
- [ ] 08 — §1.4'te URL **ve** erişim tarihi var.
- [ ] 09 — §2.2 dört soru cevaplanmış (S1 EVET, S4 HAYIR).
- [ ] 10 — §3.2 secret akış tablosu dolu (kaynak/saklama/okuyan/rotasyon).
- [ ] 11 — §3.3 threat model satır sayısı ≥ varlık sayısı.
- [ ] 12 — §3.4'te timeout/oturum sayısı ve kripto parametresi yazılı (veya YAGNI gerekçesi).
- [ ] 13 — §4.1 kazanımları sayısal (0 secret, N kontrol, N sn).
- [ ] 14 — §4.5 G1..G14 = 14 satır, "İhlal Sonucu" dolu.
- [ ] 15 — §5.1 11 adım + bütçe (tarama adımı kısaltılamaz notu).
- [ ] 16 — §5.2 hata modu ≥7 satır.
- [ ] 17 — §5.3 6 salt-okunur komut, "— N olmalı" yorumlu.
- [ ] 18 — §6.5 güvenlik kontrolleri (EK-1..EK-5) doldurulmuş.
- [ ] 19 — §7.2 ≥10 satır, "doğrusu" dolu.
- [ ] 20 — Dosya adı `ADR-NNN-<slug>.md`; `risk: H` ise `related_gates` dolu.

### §6.2 Red Sinyalleri tablosu (Otomatik DUR)

| Sinyal | Tetikleyici | Sonuç | Sorumlu |
|---|---|---|---|
| RS-1 | Metinde secret deseni (`sk-`, `password=`, `Data Source=`) | **RED**: `rejected` + temizlik | `security` |
| RS-2 | §3.3 varlık sayısı > threat model satırı | Aşama 4 **DUR** | `security` |
| RS-3 | Timeout/oturum değeri yok | Aşama 4 **DUR** (G8) | `architect` |
| RS-4 | §1.4 tamamen boş | Aşama 2 **DUR** | `reviewer` |
| RS-5 | `security-rules.md` ile çelişki | Aşama 4 **DUR** (G12) | `master-orchestrator` |
| RS-6 | Log/yanıtta başlık veya beden açık | **DUR** + maskeleme | `developer` |
| RS-7 | Mojibake (bozuk kodlama artıkları) | Commit **DUR** | `reviewer` |

### §6.3 Sürüm / supersede kuralları tablosu

| Değişiklik | version | status | Ek işlem |
|---|---|---|---|
| Tarama çıktısı/URL güncellemesi | `patch` | Aynı | §7.1 revizyon satırı |
| Yeni tehdit satırı (aynı kontrol) | `minor` | `review` → `accepted` | §6.4 rapor yenilenir |
| Kontrol/saklama yeri değişikliği | **Yeni ADR** | Eski → `superseded` | `security-rules.md` yeniden türetilir |
| CVE/advisory gelmesi | `minor` (istisna 1) | `review` | Bekleme yok, 4. aşama hızlanır |
| Şablonun kendisi | Şablon `minor` | `active` | Doğrulanmış ADR'ler dokunulmaz |

### §6.4 Quality Report (markdown bloğu)

```markdown
## Quality Report — {{ADR_ID}}
- **Tarih:** {{YYYY-MM-DD}} | **Şablon:** adr-security-template.md v{{X.Y.Z}}
- **Durum:** {{…}} → hedef: accepted | **Risk:** {{L|M|H}} | **Reversible:** {{true|false}}
- **Secret taraması:** {{0 → TEMİZ|N → RED}} | **Log maskeleme:** {{VAR|YOK}}
- **Threat model:** {{N}} varlık / {{N}} satır → {{ESIT|EKSIK}}
- **Oturum:** timeout {{SAYI}} sn → {{VAR|YOK}} | **Kripto:** {{PARAMETRE|YAGNI + gerekçe}}
- **Onay kutusu:** {{NN}}/20 → {{YETERLİ|YETERSİZ}}
- **Kanıt:** ✅ {{N}}, ⚠️ {{N}} → {{KANIT_YETERLI|DEGIL}} | **Web kaynağı:** {{N}} URL + tarih
- **Red sinyali:** {{YOK|RS-N}} | **Guardrail ihlali:** {{YOK|G-N → İhlal Sonucu}}
- **Defter + security-rules senkronu:** {{İKİSİ DE|EKSIK}}
- **Karar özeti (1 cümle):** {{NE, NEDEN, NEYE MAL OLDU}}
- **Sonraki adım:** {{ROL + 1 eylem + süre}}
```

### §6.5 Varyant özgü ek kontroller (Güvenlik)

| # | Kontrol | Doğru koşul | Yanlışsa |
|---|---|---|---|
| EK-1 | Secret taraması | §5.3 komut 1–2 çıktısı 0 | G3 → **RED**, ADR `rejected` |
| EK-2 | Threat model derinliği | Her varlık için ≥1 azaltıcı kontrol | G9 → `draft` |
| EK-3 | Oturum/timeout | Değer + iptal yolu §3.4'te yazılı | G8 → Aşama 4 DUR |
| EK-4 | Rotasyon planı | §3.2'de periyot (gün) veya "—" + gerekçe | Eksik → §6.1 #10 düşer |
| EK-5 | Log temizliği | Maskeleme deseni + doğrulama adımı yazılı | RS-6 → DUR |

---

## §7 Referanslar

### §7.1 VersaCoder bağlantıkları tablosu

| Bağlantı | Göreli Yol | İlişki |
|---|---|---|
| Vault ana sayfa | `index.md` | Envanterin üst kaynağı |
| Anahtar sözlüğü | `keys.md` | Ad/sahip/rotasyon; değer girmez |
| Güvenlik kuralları | `.rules/security-rules.md` | G3, G4, G7 kaynak kuralı |
| Karar defteri | `.decisions/index.md` | Numara + supersede zinciri |
| Altın şablon | `.templates/adr/adr-template.md` | Uzun varyant |
| Kodlama standartları | `.rules/coding-standards.md` | G11 derleme kuralı |
| Mimarî | `architecture/index.md` | Sağlayıcı akışının yeri |
| Gate planı | `WORKFLOW.md` | `related_gates: [GATE-004]` |
| Mimari ADR | `.decisions/ADR-002-solution-tier-scope.md` | Sağlayıcı katmanı |
| DevExpress ADR | `.decisions/ADR-001-devexpress-version.md` | UI güvenliği çapraz bağ |

### §7.2 Red Team — Sık Yapılan Hatalar tablosu

| # | Hata | Neden Yanlış | Doğrusu |
|---|---|---|---|
| 1 | Anahtarı ADR'ye/Lua'ya yapıştırmak | Depo + gizli bilgi birleşimi (G3) | `keys.md` sözlük + ortam değişkeni |
| 2 | "Yerel Ollama güvenlidir, auth yok" | Yerel ağ/yazıcı yüzeyi hâlâ var | Tehdit satırı yaz, ağ kısıtı ekle |
| 3 | Threat model'siz "güvenli" kararı | Kanıtsız iddia (G5) | §3.3 tablosu, varlık = satır |
| 4 | Timeout yazmamak | Süresiz istek = kaynak tüketimi | §3.4'te süre + iptal yolu |
| 5 | Log maskesini "sonradan eklerim" | Sızıntı bir kez olur, geri alınamaz | Maskeleme kararı ADR ile aynı anda |
| 6 | Kripto algoritmasını sayısız seçmek | "Güçlü" ölçülemez | Uzunluk + mod + nonce (G7) |
| 7 | `security-rules.md`'yi ADR'de kopyalamak | İki SSOT, biri er geç yalan söyler | Kural dosyasına bağlantı (G12) |
| 8 | Yetkiyi "rol sonra eklenir" diye bırakmak | Yetkisiz erişim sonradan kapatılamaz | §3.x'te yetki satırı (G4) |
| 9 | Rotasyonu unutmak | Anahtar sonsuza dek yaşar | §3.2'de periyot zorunlu |
| 10 | Eski ADR'yi düzenleyerek zafiyet kapatmak | Tarihçe ve zincir kaybolur | §4.4 istisna 1 + yeni sürüm |
| 11 | Nygard hafifle güvenlik kararı vermek | 1 sayfa threat model'ü savunmaz | S1 `EVET` → bu varyant zorunlu |
