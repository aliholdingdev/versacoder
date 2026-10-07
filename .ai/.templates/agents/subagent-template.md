---
title: Alt-Ajan Görev Çağrısı Şablonu
type: template
category: agents
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "alt-ajan gorev sablonu: gorev tanimi, girdi/bagimlilik ve tamamlama olcutu"
---

# Alt-Ajan Görev Çağrısı Şablonu — parent → child

**Zorunlu Bağlantılar:** [[index|index]] · [[AGENTS]] · [[WORKFLOW]] · [[ARCHITECTURE]] · [[glossary]] · [[.rules/coding-standards]] · [[.rules/security-rules]]

> Bu dosya **şablondur, içerik değil**. `## §1`..`## §7` başlıkları **silinemez, adlandırılamaz, sıralanamaz**.

---

## §1 Amaç

### §1.1 Neden bu şablon zorunlu

Alt-ajan disiplinleri `explore, code-reviewer, test-writer, security-auditor,
performance-engineer, csharp-developer, typescript-pro, python-pro` olarak tanımlıdır.
Parent rol (örn. `developer`) bir disiplini çağırdığında promptu bu şablonla kurulur.

Serbest yazılmış görev çağrısı üç hata üretir:

1. **Bağlam paketi eksik** — child keşif turu yapar, 8.000 token'ı dosya okumaya harcar.
2. **Çıktı sözleşmesi yok** — child düz metin döner, parent işleyemez, iş iki kez yapılır.
3. **Checkpoint yok** — timeout'a kadar tek turda koşar; sonuç sıfırdan başlar.

`§3` iskeleti paketi, `§4.5` guardrail'ları ihlal sonucunu, `§5.1` zaman ve token
bütçesini, `§6.1` kabulü zorunlu kılar.

### §1.2 Disk Kanıtları ve Boşluk tablosu (YAGNI)

Her satır **iddiadır**. Kanıt üretilene kadar satır silinmez, `⚠️ VERIFICATION REQUIRED`
ile bekletilir. Doğrulanmayan yapı eklenmez (YAGNI).

| # | İddia | Beklenen disk kanıtı | Durum | Boşluk / Aksiyon |
|---|---|---|---|---|
| 1 | Disiplin tanımları var | `.ai/AGENTS.md` | ⚠️ VERIFICATION REQUIRED | 8 disiplin tutmuyorsa `ADR-007` |
| 2 | Görev çağrıları kayıtlı | `.ai/log.md` | ⚠️ VERIFICATION REQUIRED | Yoksa oturum şablonuyla açılır |
| 3 | Rol dosyaları mevcut | `.ai/.agents/*.md` | ⚠️ VERIFICATION REQUIRED | Eksik rol agent şablonuyla yazılır |
| 4 | Vault kök indeksi var | `.ai/index.md` | ⚠️ VERIFICATION REQUIRED | Yoksa `GATE-000` içinde üretilir |
| 5 | Görev arşivi klasörü | `.ai/tasks/*.md` | **KAPSAM DIŞI** | Üretilmez; kayıt `log.md` + `sessions/` |
| 6 | Alt-ajan kendi ADR'si | `.ai/adr/ADR-00*.md` | **KAPSAM DIŞI** | ADR yalnız `architect` |

**Okuma kuralı:** işaretli satır kanıt sunulana kadar **geçersizdir**; `§6.1` işaretlenemez,
görev `completed` sayılmaz.

### §1.3 Kullanım Türleri

| Kullanım türü | Ne zaman | Kullanılan bölüm | Yapan | Çıktı |
|---|---|---|---|---|
| Yeni görev çağrısı | Parent işi parçaladığında | `§3.11` kalıbı | Çağıran rol | Tek prompt + geri dönüş |
| Devam ettirme | Checkpoint sonrası | `§3.5` checkpoint tablosu | Aynı child | Artımlı çıktı |
| Timeout kurtarma | Süre dolduğunda | `§3.8` hata tablosu | Parent | Yeniden çerçeveleme |
| Disiplin seçimi | Birden fazla aday varken | `§2.1` eşleme | `master-orchestrator` | Doğru child + bütçe |
| Görev kabulü | Child döndüğünde | `§6.1` + `§6.2` | `reviewer` | Kabul / redd |

### §1.4 Neden zorunlu

1. **Tek promptta teslim** — `§3.4` Bağlam Paketi, child'ın keşif turunu sıfırlar.
2. **Ölçülebilir kapanış** — `§3.6` Çıktı Sözleşmesi olmayan görev `completed` olamaz.
3. **Kurtarılabilirlik** — `§3.5` checkpoint'leri olmadan timeout tüm işi götürür.
4. **Bütçe disiplini** — 50.000 token/oturum paylaştırılır; `§5.1` aşımında oturum kapanır.
5. **Sorumluluk izi** — `§3.7` geri dönüş formatı, parent'ın hangi sonucu aldığını yazar.

---

## §2 Kapsam

| Kapsam (bu şablon kapsar) | Kapsam Dışı (KAPMAZ) |
|---|---|
| Parent→child görev promptu iskeleti | Rol profili tanımı (agent şablonu) |
| Bağlam paketi, çıktı sözleşmesi, geri dönüş formatı | `.ai/log.md` kayıt formatı (oturum şablonu) |
| Checkpoint, zaman ve token bütçesi | ADR kararı içeriği ve `GATE-*` metinleri |
| Timeout / hata / retry yönetimi | Build pipeline yapılandırması |
| Disiplin → görev tipi eşlemesi (`§2.1`) | `.rules/*.md` kural metinleri |
| Guardrail G1..G12 ve ihlal sonucu | `.csproj` / `.cs` kod dosyaları |

### §2.1 Disiplin → Görev Tipi eşlemesi

| Disiplin | Uygun görev tipi | Çağıran rol | Çıktı biçimi | Bütçe payı |
|---|---|---|---|---|
| `explore` | Dosya/bulgu keşfi | Her rol | Yol + satır listesi | 4.000 token |
| `code-reviewer` | Diff incelemesi | `reviewer` | Bulgu tablosu (P0..P3) | 6.000 token |
| `test-writer` | Test üretimi | `testing` | `*Tests.cs` + senaryo | 8.000 token |
| `security-auditor` | OWASP taraması | `security` | Risk listesi + kanıt | 6.000 token |
| `performance-engineer` | Profil / ölçüm | `developer` | Metrik tablosu | 5.000 token |
| `csharp-developer` | C# kod üretimi | `developer` | `.cs` diff | 8.000 token |
| `typescript-pro` | TS/JS üretimi | `ui-designer` | `.ts` diff | 6.000 token |
| `python-pro` | Betik / araç | `devops-engineer` | `.py` betiği | 5.000 token |

### §2.2 Kapsam Sınırı Testi

Bu şablon **zorunludur**; 5 şartın **hepsi** true olmalı:

1. Çağrı parent→child yönündedir; child hiçbir zaman parent'ı çağırmaz.
2. Tek bir prompt içinde tek bir disiplin çağrılır (`§2.1`).
3. Girdi dosya yolu ve satır aralığıyla belirtilmiştir (soyut talep yok).
4. Çıktı biçimi `§3.7` geri dönüş formatına uymaktadır.
5. Zaman ve token bütçesi `§5.1` içinde kalmaktadır.

**Ret testi (herhangi biri true ise bu şablon kullanılmaz):**

| Soru | Cevap = EVET ise | Doğru şablon |
|---|---|---|
| Dosya bir rol profili mi? | Agent şablonu | `.templates/agents/agents-template.md` |
| Dosya `.ai/log.md` satırı mı? | Oturum kaydı | `.templates/session-log-template.md` |
| Dosya bir ADR kararı mı? | ADR şablonu | `.templates/adr/adr-template.md` |
| Dosya `.rules/` içinde mi? | Kural dosyası | Şablon yok; 7 dosya korunur |
| Dosya `.csproj` / `.cs` mi? | Kod dosyası | Şablon yok; `.rules/coding-standards` |

---

## §3 Mimari

### §3.1 Frontmatter YAML bloğu

Görev çağrısı `.ai/log.md` içine gömülü olduğu için ayrı dosyaya frontmatter yazılmaz;
ancak kalıp `type: subagent-task` alanını **promptun ilk satırında** taşır.

```yaml
---
title: <Disiplin> — Görev Çağrısı
type: subagent-task
category: agents
status: dispatched | checkpoint | completed | failed
version: 1.0.0
updated: <YYYY-MM-DD>
authority: SSOT
---
```

Alan adları: `title, type, category, status, version, updated, authority`. Alan adı ya da
tipi bozuksa **G1**, `authority` değeri değişmişse **G2** ihlali sayılır.

### §3.2 H1 iskeleti

```markdown
# <Disiplin> — Görev Çağrısı ({{TASK_ID}})

**Zorunlu Bağlantılar:** [[index|index]] · [[AGENTS]] · [[WORKFLOW]] · [[glossary]]
· [[.rules/<ilgili-2-kural>]] |

> <Tek cümlelik görev özeti — 20 kelimeyi geçmez.>
```

### §3.3-§3.9 Zorunlu 8 bölümün markdown iskeleti

Tek çit içinde, sırayla. Başlıklar aynen korunur.

```markdown
### §3.3 Görev Kimliği
| Alan | Değer |
|---|---|
| Görev ID | {{TASK_ID}} |
| Çağıran (parent) | {{PARENT_ROLE}} |
| Çağrılan (child) | {{DISCIPLINE}} |
| Öncelik | {{P0_P1_P2_P3}} |
| Tetikleyici | {{TRIGGER}} |

### §3.4 Bağlam Paketi (üstlenecek her şey tek promptta)
| Öğe | Değer |
|---|---|
| Amaç | {{WHY_ONE_SENTENCE}} |
| Dosya + aralık | {{FILE_PATH}}:{{LINE_RANGE}} |
| İlgili ADR / kural | {{ADR_OR_RULE}} |
| Beklenen davranış | {{EXPECTED_BEHAVIOUR}} |
| Yapılmayacak | {{OUT_OF_SCOPE}} |

### §3.5 Çıktı Sözleşmesi
| Alan | Zorunlu | Biçim |
|---|---|---|
| Özet | Evet | 1-3 cümle |
| Bulgu/dosya satırları | Evet | `dosya:satır — cümle` |
| Kanıt | Evet | Komut çıktısı veya `git diff` |
| Kesinlik | Evet | `KESİN` / `⚠️ VERIFICATION REQUIRED` |

### §3.6 Checkpoint Noktaları
| # | Tetik | Çıktı | Aksiyon |
|---|---|---|---|
| C1 | Keşif bitti | Bulunan yollar | Parent'a 1 satır rapor |
| C2 | 5 dakika geçti | Ara sonuç | Dur + özetle |
| C3 | 2 deneme başarısız | Hata metni | Escalation (`§3.8`) |

### §3.7 Zaman Bütçesi
| Kaynak | Üst sınır | Aşım sonucu |
|---|---|---|
| Süre | {{MAX_MINUTES}} dk | Timeout → `failed` |
| Token | {{MAX_TOKEN}} | G10 → oturum kapanır |
| Araç çağrısı | {{MAX_TOOL_CALLS}} | Artan çağrı → DUR |

### §3.8 Bağımlılıklar
| Bağımlılık | Tür | Beklenir mi? |
|---|---|---|
| {{DEP_1}} | Dosya / çıktı | Evet — yoksa DUR |
| {{DEP_2}} | Rol onayı | Evet — `architect` |
| {{DEP_3}} | Ortam (build, araç) | Hayır — koşmuyorsa atla |

### §3.9.1 Geri Dönüş Formatı
**Geri dönüş (prompt sonu):**
`SONUÇ: KABUL|RED` · `BULGU: <adet>` · `KANIT: <komut>` · `TOKEN: <n>` · `AÇIK: <risk|YOK>`

### §3.9.2 Hata Yönetimi
| Hata | Belirti | İlk tepki | Escalation |
|---|---|---|---|
| Timeout | Süre doldu | Checkpoint çıktısını gönder | Parent yeniden çerçeveler |
| Belirsizlik | 2 geçerli çözüm | Soru üret, **tahmin etme** | `architect` |
| Yetki aşımı | G12 tetiklendi | İşlemi geri al | `security` |
| Kanıt yok | İddia + çıktı yok | `⚠️ VERIFICATION REQUIRED` yaz | Parent reddeder |
```

### §3.10 Doldurulmuş gerçek VersaCoder örneği — `code-reviewer` çağrısı

```markdown
# code-reviewer — Görev Çağrısı (TASK-0042)

**Zorunlu Bağlantılar:** [[index|index]] · [[AGENTS]] · [[WORKFLOW]] · [[glossary]] · [[.rules/coding-standards]]

> RibbonForm olay aboneliklerinin sızıntısını incele ve bulguları satır kanıtıyla ver.

### §3.3 Görev Kimliği
| Alan | Değer |
|---|---|
| Görev ID | `TASK-0042` |
| Çağıran (parent) | `reviewer` |
| Çağrılan (child) | `code-reviewer` |
| Öncelik | P1 |
| Tetikleyici | `GATE-004` kod inceleme adımı |

### §3.4 Bağlam Paketi (üstlenecek her şey tek promptta)
| Öğe | Değer |
|---|---|
| Amaç | `VersaCoder.Desktop` içindeki Form kodunda olay aboneliği sızıntısı bulmak |
| Dosya + aralık | `src/VersaCoder.Desktop/Forms/*.cs` — tüm `+=` ve `-=` kullanımları |
| İlgili ADR / kural | `ADR-005` (elle Designer.cs), `.rules/solid-rules` madde 1 |
| Beklenen davranış | Her `+=` için Form kapanışında eşleşen `-=` olmalı |
| Yapılmayacak | Dosya düzenleme, refactor önerisi üretme, yeni dosya açma |

### §3.5 Çıktı Sözleşmesi
| Alan | Zorunlu | Biçim |
|---|---|---|
| Özet | Evet | 1-3 cümle |
| Bulgu satırları | Evet | `src/.../FormX.cs:120 — abonelik kapatılmamış` |
| Kanıt | Evet | `Select-String -Pattern '\+=' çıktısı` |
| Kesinlik | Evet | `KESİN` ya da `⚠️ VERIFICATION REQUIRED` |

### §3.6 Checkpoint Noktaları
| # | Tetik | Çıktı | Aksiyon |
|---|---|---|---|
| C1 | Keşif bitti | Bulunan `.cs` dosya sayısı | 1 satır rapor |
| C2 | 5 dakika geçti | Taranan klasör | Dur + özetle |
| C3 | 2 deneme başarısız | Hata metni | `reviewer`'a escalation |

### §3.7 Zaman Bütçesi
| Kaynak | Üst sınır | Aşım sonucu |
|---|---|---|
| Süre | 12 dk | Timeout → `failed` |
| Token | 6.000 | G10 → oturum kapanır |
| Araç çağrısı | 40 | Artan çağrı → DUR |

### §3.8 Bağımlılıklar
| Bağımlılık | Tür | Beklenir mi? |
|---|---|---|
| `git diff HEAD~1` çıktısı | Çıktı | Evet — yoksa DUR |
| `reviewer` onayı | Rol | Evet |
| DevExpress tasarımcısı | Ortam | Hayır — koşmuyorsa atla |

### §3.9.1 Geri Dönüş Formatı
**Geri dönüş (prompt sonu):**
`SONUÇ: KABUL|RED` · `BULGU: <adet>` · `KANIT: <komut>` · `TOKEN: <n>` · `AÇIK: <risk|YOK>`

### §3.9.2 Hata Yönetimi
| Hata | Belirti | İlk tepki | Escalation |
|---|---|---|---|
| Timeout | 12 dk doldu | C1/C2 çıktısını gönder | `reviewer` yeniden çerçeveler |
| Belirsizlik | Kapsam net değil | Soru üret, **tahmin etme** | `architect` |
| Yetki aşımı | Dosya düzenleme denemesi | İşlemi geri al | `security` |
| Kanıt yok | Bulgu + log yok | `⚠️ VERIFICATION REQUIRED` yaz | Parent reddeder |
```

### §3.11 Kopyala-yapıştır tam kalıp (tümü `{{PLACEHOLDER}}`)

```markdown
---
title: {{DISCIPLINE}} — Görev Çağrısı
type: subagent-task
category: agents
status: dispatched
version: 1.0.0
updated: {{YYYY-MM-DD}}
authority: SSOT
---

# {{DISCIPLINE}} — Görev Çağrısı ({{TASK_ID}})

**Zorunlu Bağlantılar:** [[index|index]] · [[AGENTS]] · [[WORKFLOW]] · [[glossary]] · [[.rules/{{RULE_1}}]]

> {{TASK_TAGLINE_20_WORDS_MAX}}

### §3.3 Görev Kimliği
| Alan | Değer |
|---|---|
| Görev ID | {{TASK_ID}} |
| Çağıran (parent) | {{PARENT_ROLE}} |
| Çağrılan (child) | {{DISCIPLINE}} |
| Öncelik | {{P0_P1_P2_P3}} |
| Tetikleyici | {{TRIGGER}} |

### §3.4 Bağlam Paketi (üstlenecek her şey tek promptta)
| Öğe | Değer |
|---|---|
| Amaç | {{WHY_ONE_SENTENCE}} |
| Dosya + aralık | {{FILE_PATH}}:{{LINE_RANGE}} |
| İlgili ADR / kural | {{ADR_OR_RULE}} |
| Beklenen davranış | {{EXPECTED_BEHAVIOUR}} |
| Yapılmayacak | {{OUT_OF_SCOPE}} |

### §3.5 Çıktı Sözleşmesi
| Alan | Zorunlu | Biçim |
|---|---|---|
| Özet | Evet | 1-3 cümle |
| Bulgu/dosya satırları | Evet | `dosya:satır — cümle` |
| Kanıt | Evet | Komut çıktısı veya `git diff` |
| Kesinlik | Evet | `KESİN` / `⚠️ VERIFICATION REQUIRED` |

### §3.6 Checkpoint Noktaları
| # | Tetik | Çıktı | Aksiyon |
|---|---|---|---|
| C1 | Keşif bitti | Bulunan yollar | Parent'a 1 satır rapor |
| C2 | {{MINUTES_A}} dakika geçti | Ara sonuç | Dur + özetle |
| C3 | 2 deneme başarısız | Hata metni | Escalation (`§3.8`) |

### §3.7 Zaman Bütçesi
| Kaynak | Üst sınır | Aşım sonucu |
|---|---|---|
| Süre | {{MAX_MINUTES}} dk | Timeout → `failed` |
| Token | {{MAX_TOKEN}} | G10 → oturum kapanır |
| Araç çağrısı | {{MAX_TOOL_CALLS}} | Artan çağrı → DUR |

### §3.8 Bağımlılıklar
| Bağımlılık | Tür | Beklenir mi? |
|---|---|---|
| {{DEP_1}} | Dosya / çıktı | Evet — yoksa DUR |
| {{DEP_2}} | Rol onayı | Evet — {{APPROVER}} |
| {{DEP_3}} | Ortam (build, araç) | Hayır — koşmuyorsa atla |

### §3.9.1 Geri Dönüş Formatı
**Geri dönüş (prompt sonu):**
`SONUÇ: KABUL|RED` · `BULGU: <adet>` · `KANIT: <komut>` · `TOKEN: <n>` · `AÇIK: <risk|YOK>`

### §3.9.2 Hata Yönetimi
| Hata | Belirti | İlk tepki | Escalation |
|---|---|---|---|
| Timeout | Süre doldu | Checkpoint çıktısını gönder | Parent yeniden çerçeveler |
| Belirsizlik | 2 geçerli çözüm | Soru üret, **tahmin etme** | `architect` |
| Yetki aşımı | G12 tetiklendi | İşlemi geri al | `security` |
| Kanıt yok | İddia + çıktı yok | `⚠️ VERIFICATION REQUIRED` yaz | Parent reddeder |
```

---

## §4 Kurallar

### §4.1 Durum / Aşama döngüsü

`status` yalnızca dört değer alır; her geçiş `.ai/log.md` satırı ister.

```
 dispatched --ilk çıktı--► checkpoint --kabul--► completed
      |                        |                    |
      | timeout / hata         | 2 başarısızlık     | parent reddi
      ▼                        ▼                    ▼
    failed ◄--------------------+                retry (yeni TASK_ID)
 YASAK: completed ► dispatched (sessiz yeniden açılış) · YASAK: checkpoint'siz completed
```

| Durum | Anlamı | Kim koyar | Kim kapatır | `§6.1` zorunlu mu |
|---|---|---|---|---|
| `dispatched` | Prompt parent tarafından gönderildi | Çağıran rol | — | Hayır |
| `checkpoint` | C1/C2 çıktısı parent'a ulaştı | Child | Child | Kısmen |
| `completed` | `§3.9` formatı dolu + kanıt var | Child | `reviewer` | **Evet, 16/16** |
| `failed` | Timeout / G10 / G12 | Child ya da parent | `master-orchestrator` | Evet (hata notu) |

### §4.2 Onay / Rol akışı

| # | Adım | Yapan rol | Onay bekleyen | Redde yetkili | Zorunlu kanıt |
|---|---|---|---|---|---|
| A1 | Disiplin seçimi | Çağıran rol | `master-orchestrator` | `master-orchestrator` | `§2.1` eşlemesi |
| A2 | Bağlam paketi yazımı | Çağıran rol | `reviewer` | `reviewer` | `§3.4` 5 satır dolu |
| A3 | Bütçe ayırma | Çağıran rol | `quality-check` | `quality-check` | `§3.7` ≤ 50.000 payı |
| A4 | Çağrıyı gönderme | Çağıran rol | — | — | `TASK_ID` + `log.md` satırı |
| A5 | Checkpoint kabulü | Parent | `reviewer` | `reviewer` | C1/C2 çıktısı |
| A6 | Geri dönüş kabulü | `reviewer` | `master-orchestrator` | `master-orchestrator` | `§3.9` formatı + kanıt |

### §4.3 Öncelik / Ölçek tablosu

| Öncelik | Ad | Ne zaman | Süre sınırı | Token payı |
|---|---|---|---|---|
| **P0** | Kritik | Build kırık, secret sızıntısı, G5 | 5 dk — **Otomatik DUR** | 10.000 |
| **P1** | Yüksek | `GATE` kapısını bloke eden bulgu | 12 dk | 8.000 |
| **P2** | Normal | İyileştirme, test ekleme | 20 dk | 6.000 |
| **P3** | Düşük | Keşif, doküman, temizlik | 30 dk | 4.000 |
| **S/M/L** | 1 / 2-3 / 4+ disiplin | Tek çağrı / ardışık / paralel | 12 / 24 / 40 dk | 8.000 / 16.000 / 28.000 |

### §4.4 Değişiklik kontrolü

1. Görev çerçevesi değişirse **yeni `TASK_ID`** açılır; aynı ID ile yeniden gönderim **yasaktır**.
2. `authority: SSOT` hiçbir sürümde değiştirilemez (G2).
3. `completed → dispatched` sessiz yeniden açılış **yasaktır**; `retry` yeni ID ile olur.
4. Bağlam paketi (`§3.4`) gönderimden sonra değiştirilemez; değişiklik yeni görevdir.
5. Her dispatch, checkpoint ve sonuç `.ai/log.md`'ye tek satır yazar:
   `tarih | parent | child | TASK_ID | durum | token`.
6. Child asla `master-orchestrator`, `architect` veya `reviewer` rolüne **bürünemez** (G12).
7. Geri dönüş `KESİN` olmayan her satır `⚠️ VERIFICATION REQUIRED` taşır.
8. Aynı disipline ardışık 3 başarısız çağrı varsa durulur ve `master-orchestrator`'a gidilir.

### §4.5 Hard Guardrails tablosu

| ID | Guardrail | Kural kaynağı | Tespit yöntemi | İhlal Sonucu |
|---|---|---|---|---|
| **G1** | `type: subagent-task` ve 7 alan eksiksiz | `§3.1` | Prompt ilk satırı | Çağrı reddedilir |
| **G2** | `authority: SSOT` değişmez | Vault kuralı | Prompt diff | Çağrı geri alınır + `log.md` |
| **G3** | `## §1`..`## §7` silinemez | Bu şablon | Başlık sayısı = 7 | Kalıp revert, çağrı `failed` |
| **G4** | Tek promptta tek disiplin | `§2.1` | Prompt taraması | Çağrı iptal, ikiye bölünür |
| **G5** | EF Core / `DbContext` üretimi | `ADR-004` | Çıktı taraması | **Build durur**; sonuç reddi |
| **G6** | Yalnız `net48` + `net10.0-windows` | `ADR-003` | Çıktı diff | Sonuç reddi, yeniden çerçevele |
| **G7** | DevExpress `25.2.8` sürümü | `ADR-001` / `ADR-008` | Çıktı + `.csproj` | Sonuç reddi |
| **G8** | `Designer.cs`/`.resx` child tarafından üretilmez | `ADR-005` | Dosya imzası | Dosya revert, `security` |
| **G9** | Kanıtsız iddia yazılamaz | `§3.5` | `⚠️ VERIFICATION REQUIRED` yoksa | Satır silinir, `RED` verilir |
| **G10** | Toplam token ≤ 50.000 / oturum | Vault bütçesi | Oturum sayacı | Oturum kapanır, handover |
| **G11** | Mojibake yasak | Dil kuralı | `\xc3\xbc`, `â„¢`, `ðŸ` taraması | Çıktı reddi, yeniden yazım |
| **G12** | Child parent yetkisine bürünemez | `§4.4` madde 6 | Rol denetim kaydı | İşlem **geri alınır** + rapor |

---

## §5 Workflow

### §5.1 Adım tablosu ve zaman bütçesi

| # | Adım | Girdi | Çıktı | Yapan | Süre |
|---|---|---|---|---|---|
| 1 | İşin parçalanması | Görev metni | Alt iş listesi | Çağıran rol | 4 dk |
| 2 | Disiplin seçimi | `§2.1` tablosu | `{{DISCIPLINE}}` | `master-orchestrator` | 2 dk |
| 3 | Bağlam paketi | Dosya, ADR, kural | `§3.4` 5 satır | Çağıran rol | 6 dk |
| 4 | Çıktı sözleşmesi | Beklenen kanıt | `§3.5` 4 satır | Çağıran rol | 3 dk |
| 5 | Bütçe + checkpoint | `§3.7`, `§3.6` | Süre, token, C1-C3 | Çağıran rol | 3 dk |
| 6 | Prompt gönderimi | `§3.11` kalıbı | `TASK_ID` + `log.md` | Çağıran rol | 2 dk |
| 7 | Child çalışması | Tek prompt | Ara çıktılar | `{{DISCIPLINE}}` | ≤ `{{MAX}}` dk |
| 8 | Checkpoint toplama | C1/C2 çıktısı | 1 satır rapor | Parent | 3 dk |
| 9 | Geri dönüş denetimi | `§3.9` formatı | KABUL / RED | `reviewer` | 6 dk |
| 10 | Kanıt doğrulama | Komut çıktısı | Salt-okunur log | `quality-check` | 5 dk |
| 11 | Kayıt + kapanış | Sonuç + token | `.ai/log.md` satırı | Çağıran rol | 3 dk |

**Toplam zaman bütçesi: 43 dakika** (7. adım hariç). 7. adım kendi `§3.7` sınırını aşarsa
timeout tetiklenir; parent checkpoint çıktısıyla **yeniden çerçeveleme** yapar, aynı `TASK_ID`
ile tekrar göndermez (G-`§4.4` madde 1).

**Token bütçesi (toplam 50.000 / oturum):** keşif + paketleme (1-5) → %20 (10.000) ·
child çalışması (7-8) → %55 (27.500) · denetim + kanıt (9-10) → %17 (8.500) ·
kayıt (11) → %8 (4.000).

### §5.2 Hata Modları tablosu

| # | Hata modu | Belirti | Olası neden | İlk müdahale | Kurtarma |
|---|---|---|---|---|---|
| E1 | Bağlam paketi eksik | Child keşif turu atıyor | `§3.4` boş | Dur, paketi tamamla, yeni ID | Yeniden dispatch |
| E2 | Çıktı biçimi uymuyor | Düz metin döndü | Sözleşme yazılmamış | `§3.7` formatını hatırlat | `reviewer` RED verir |
| E3 | Timeout | Süre doldu, sonuç yok | Bütçe dar | Checkpoint çıktısını al | Yeni ID + kapsam daralt |
| E4 | Disiplin uyuşmazlığı | `explore` kod yazdı | `§2.1` atlandı | Çağrıyı iptal et | Doğru disiplinle tekrar |
| E5 | Yetki aşımı | G12 tetiklendi | Child rol büründü | İşlemi geri al | `security` + rapor |
| E6 | EF Core sızıntısı | G5, build durdu | Çıktı üretimi hatalı | `DbContext` çıktısını sil | Build yeşil olana dek |
| E7 | Bütçe taşması | `> 50.000` (G10) | Ardışık çağrılar | Oturumu kapat | Handover + yeni oturum |
| E8 | Kanıt yokluğu | `⚠️ VERIFICATION REQUIRED` açık | Doğrulama yapılmadı | `§5.3` salt-okunur komut | Parent satırı siler, RED |

### §5.3 Salt-okunur PowerShell komut bloğu

6 komut **yalnızca okur**; dosya değiştirmez. Çıktısı `§6.1` kutusunun kanıtıdır.

```powershell
# 1 — Görev kaydı log.md içinde var olmalı (0 olmamalı)
(Select-String -Path 'C:\versacoder\.ai\log.md' -Pattern 'TASK-').Count  # ≥ 1 olmalı

# 2 — Disiplin adı agent kataloğunda geçmeli (8 disiplinden biri)
Select-String -Path 'C:\versacoder\.ai\AGENTS.md' -Pattern 'code-reviewer|explore|test-writer'

# 3 — Prompt içinde tek disiplin geçmeli — 1 sonuç olmalı (G4)
(Select-String -InputObject $prompt -Pattern 'code-reviewer|explore|test-writer').Count  # 1 olmalı

# 4 — EF Core üretimi olmamalı — 0 sonuç olmalı (G5)
Select-String -Path 'C:\versacoder\.ai\sessions\*.md' -Pattern 'EntityFramework|DbContext'

# 5 — Mojibake olmamalı — 0 sonuç olmalı (G11)
Select-String -Path 'C:\versacoder\.ai\sessions\*.md' -Pattern '\xc3\xbc|\xc3\xb6|â„¢|\xe2\u20ac™|ðŸ|ï¿½'

# 6 — Oturum token sayacı 50.000'i geçmemeli (G10)
Select-String -Path 'C:\versacoder\.ai\log.md' -Pattern 'TOKEN: \d+'  # toplam ≤ 50.000 olmalı
```

---

## §6 Doğrulama

### §6.1 Onay kutusu listesi

Görev yalnızca **16/16** işaretlendiğinde `status: completed` alır.

- [ ] 1. Prompt `type: subagent-task` ve 7 alan eksiksiz başlıyor (G1)
- [ ] 2. `authority: SSOT` değiştirilmemiş (G2)
- [ ] 3. `## §1 Amaç` başlığı tam metinle duruyor (G3)
- [ ] 4. `## §2 Kapsam` içinde Kapsam ve Kapsam Dışı tabloları ikisi de var
- [ ] 5. `§2.1` disiplin → görev tipi eşlemesi 8 satır dolu
- [ ] 6. `§2.2` 5 şart + 5 ret sorusu dolu
- [ ] 7. `§3.3..§3.9` arasında 8 zorunlu bölümün iskeleti mevcut
- [ ] 8. `§3.10` gerçek VersaCoder örneği tam ve somut (placeholder yok)
- [ ] 9. `§3.11` kalıbındaki tüm alanlar `{{PLACEHOLDER}}`
- [ ] 10. `§3.4` Bağlam Paketi 5 satır dolu (tek promptta teslim)
- [ ] 11. `§3.6` checkpoint tablosu en az 3 satır (C1-C3)
- [ ] 12. `§4.5` tablosunda **en az 12** satır ve `İhlal Sonucu` sütunu var
- [ ] 13. `§5.1` adımları **en az 9** satır + zaman bütçesi yazılı
- [ ] 14. `§5.2` hata modları **en az 7** satır
- [ ] 15. `§5.3` içinde salt-okunur **6** PowerShell komutu, her biri "— N olmalı" yorumlu
- [ ] 16. `§7.2` Red Team tablosunda **en az 10** satır var

### §6.2 Red Sinyalleri tablosu (Otomatik DUR)

| # | Red sinyali | Tetikleyen koşul | Sonuç |
|---|---|---|---|
| R1 | Kanıtsız iddia | İşaret yok + kanıt yok | **Otomatik DUR** — `RED` verilir |
| R2 | Bölüm silinmiş | `## §` başlık sayısı ≠ 7 | **Otomatik DUR** — kalıp revert |
| R3 | Bağlam paketi eksik | `§3.4` satırlarından biri boş | **Otomatik DUR** — çağrı gönderilmez |
| R4 | Yasaklı teknoloji | G5 ihlali (EF Core) | **Otomatik DUR** — build kapalı |
| R5 | Çerceve dışı görev | Child parent rolüne büründü | **Otomatik DUR** — G12 |
| R6 | Disiplin uyuşmazlığı | `§2.1` dışı görev tipi | **Otomatik DUR** — yeniden seç |
| R7 | Bütçe taşması | Oturum `> 50.000` token | **Otomatik DUR** — handover zorunlu |
| R8 | Checkpointsiz kapanış | C1 yokken `completed` | **Otomatik DUR** — `failed` sayılır |

### §6.3 Sürüm / Aşama geçiş kuralları

| Geçiş | Koşul | Yeni `TASK_ID` | Onay | Zorunlu kanıt |
|---|---|---|---|---|
| — → `dispatched` | `§3.11` eksiksiz | Evet, ilk | Çağıran rol | `log.md` satırı |
| `dispatched → checkpoint` | C1/C2 çıktısı | Aynı | Parent | 1 satır rapor |
| `checkpoint → completed` | `§3.9` formatı + 16/16 | Aynı | `reviewer` | Komut çıktısı |
| `dispatched → failed` | Timeout / G10 / G12 | Kapanır | `master-orchestrator` | Hata metni |
| `failed → dispatched` | Yeniden çerçeveleme | **Yeni ID zorunlu** | `architect` | Yeni `§3.4` paketi |
| `completed → dispatched` | — | — | — | **YASAK** — G3 / `§4.4` |
| `checkpoint → completed` (C1'siz) | — | — | — | **YASAK** — R8 |

### §6.4 Quality Report markdown bloğu

`quality-check` bu kalıbı **aynen** doldurur ve `.ai/log.md`'ye iliştirir.

````markdown
### Quality Report — {{TASK_ID}} ({{DISCIPLINE}})
- **Tarih:** {{YYYY-MM-DD}} · **Kontrol eden:** `quality-check` + `reviewer` · **Sonuç:** PASS | FAIL

| Kontrol | Beklenen | Gerçek | Durum |
|---|---|---|---|
| Şablon boyutu | 480-660 satır / 15-32 KB | {{LINES}} / {{KB}} | OK/FAIL |
| Bölüm + frontmatter | 7 başlık / 7 alan | {{SECTIONS}} / {{FM}} | OK/FAIL |
| Guardrail satırı | ≥ 12 (G1..G12) | {{G}} | OK/FAIL |
| Adım / hata modu / komut | ≥ 9 / ≥ 7 / 6 | {{STEPS}} / {{ERR}} / {{PS}} | OK/FAIL |
| Onay kutusu + Red Team | ≥ 16 / ≥ 10 | {{CHECKS}} / {{RED}} | OK/FAIL |
| Token (bu görev) | ≤ {{MAX_TOKEN}} | {{USED}} | OK/FAIL |
| Mojibake / EF Core | 0 / 0 | {{MOJI}} / {{EF}} | OK/FAIL |

**Açık bulgular:** {{FINDINGS_OR_NONE}} · **Onay:** {{APPROVER}} · {{YYYY-MM-DD}}
````

### §6.5 Şablona özgü ek kontroller

1. **Tek promptta teslim** — `§3.4` içindeki dosya/ADR/kural bilgisi child'ın ayrıca okumasını
   gerektiriyorsa paket eksiktir; `R3` çalışır.
2. **Placeholder sızıntısı** — `§3.10` içinde `{{` geçmemeli; `§3.11` içinde yalnız
   `{{PLACEHOLDER}}` bulunmalı.
3. **Yerel göreli bağlantı** — `Zorunlu Bağlantılar` satırındaki her wiki-link `.ai/`
   köküne göreli; mutlak yol (`C:\...`) **yasak**.
4. **Türkçe bütünlüğü** — `ç ğ ı İ ö ş ü` doğru; `Ã`/`â` artıkları olmamalı (G11).
5. **Bütçe eşleşmesi** — `§3.7` token sınırı, `§5.1` pay dağılımını aşmamalı; aşarsa
   rapor `FAIL` verir ve `R7` çalışır.

---

## §7 Referanslar

### §7.1 VersaCoder bağlantıkları tablosu

| Kaynak | Yol (`.ai/` içine göreli) | Bu şablonla ilişkisi |
|---|---|---|
| Vault indeksi | `index.md` | Görev çağrılarına giriş kapısı |
| Agent kataloğu | `AGENTS.md` | 8 disiplin = `§2.1` eşlemesi |
| İş akışı | `WORKFLOW.md` | `§5.1` adımlarının üst tanımı |
| Mimari | `architecture/index.md` | `§3.4` bağlam paketinin ADR kaynağı |
| Oturum kaydı | `log.md` | `§4.4` madde 5 zorunlu satırı |
| Rol profili şablonu | `.templates/agents/agents-template.md` | `§2.1` yetki sınırının kaynağı |
| Oturum şablonu | `.templates/session-log-template.md` | Görev sonuçlarının kaydı |
| ADR şablonu | `.templates/adr/adr-template.md` | Yapı referansı (içerik kopyalanmaz) |
| Kod + güvenlik kuralları | `.rules/coding-standards`, `.rules/security-rules` | `§3.5` kesinlik ve kanıt kuralları |
| SOLID kuralları | `.rules/solid-rules` | `§3.4` mimari bağlam satırı |

### §7.2 Red Team — Sık Yapılan Hatalar tablosu

| # | Hata | Belirti | Neden yanlış | Doğrusu | Yakalayan kontrol |
|---|---|---|---|---|---|
| 1 | Bağlam olmadan çağırmak | Child 8.000 token okuyor | "Kendi bulsun" | `§3.4` 5 satır dolu gönderilir | R3, `§6.1` madde 10 |
| 2 | Çıktı sözleşmesi yazmamak | Düz metin döner | "Yeterli anlaşılır" | `SONUÇ/BULGU/KANIT` formatı | R2, `§6.1` madde 11 |
| 3 | Aynı `TASK_ID` ile tekrar | Sonuç üst üste biner | Acelenin etkisi | Retry = **yeni** `TASK_ID` | `§4.4` madde 1 |
| 4 | Checkpointsiz çalışmak | Timeout her şeyi götürür | "Hızlı bitsin" | C1-C3 zorunlu | R8 |
| 5 | Birden fazla disiplin tek promptta | `explore` kod yazıyor | Karışık çağrı | Tek prompt = tek disiplin | G4 |
| 6 | Child'ın ADR yazması | `.ai/adr/` dosyası değişti | Yetki aşımı | ADR yalnız `architect` | G12, `§1.2` satır 6 |
| 7 | EF Core üretimi | `DbContext` çıktıda | Eski alışkanlık | Dapper + `@p1..@pN` | G5 |
| 8 | Token sayısını yazmamak | `TOKEN:` alanı boş | Hafife alma | Geri dönüşünde `TOKEN: <n>` | G10 |
| 9 | Kanıtsız sonuç kabulü | Log yok, özet var | Kolaylık | `⚠️ VERIFICATION REQUIRED` | G9, R1 |
| 10 | Mojibake ile dönüş | `ÅŸ` görünüyor | Yanlış kodlama | UTF-8; `ç ğ ı İ ö ş ü` | G11, komut 5 |
| 11 | Şablonu 400 satıra indirmek | Dosya 12 KB | "Kısa olsun" | 480-660 satır / 15-32 KB | `§6.1` madde 16 |

> **Şablonun kendi doğrulaması:** her güncellemeden sonra `§5.3` komutları çalıştırılır,
> `§6.4` Quality Report üretilir, `§6.1` 16/16 kapanmadan görev `completed` olamaz.
