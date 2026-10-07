---
title: "VersaCoder — .templates/ Dizin Kuralı"
type: template
category: templates
status: active
version: 1.0.0
updated: 2026-10-05
authority: SSOT
scope: "şablon klasörü kuralı: ne yaşar, okuma sırası, kategori eşlemesi, şablon seçimi, onay, yasaklar, envantere satır"
---

# VersaCoder — .templates/ Dizin Kuralı
**Zorunlu Bağlantılar:** [index](index.md) · [session-log](session-log-template.md) · [adr](adr/adr-index.md) · [arch](architecture/architecture-template.md) · [form](frontend/form-template.md) · [CLAUDE](../../CLAUDE.md) · [WORKFLOW](../../WORKFLOW.md)

## §1 Amaç
### §1.1 Ne Yaşar?
Klasörde: `index.md` envanter · bu dosya kural · `session-log-template.md` kayıt · 11 klasörde 42 şablon = 51 MD.
### §1.2 Ne Üretmez?
Kural metni, ADR, envanter satırı, kod kopyalanmaz; yalnız bağlantı (G12). Tek dosya tek gerçek.
### §1.3 Disk Kanıtı
2026-10-06: `.ai/.templates/**` = 51 MD (49+bu 2); `.ai/.rules/` ve `.ai/architecture/` YOK → link yok (G7).
### §1.4 Okuma Sırası
`index.md` (şablon) → görev şablonu (iskelet) → kural (bağlantı). Ters okuma yasak; halka eksikse üretim başlamaz.
## §2 Kapsam
### §2.1 İçinde / Dışında
İçinde: klasör kuralı, okuma sırası, kategori eşlemesi, şablon seçimi, onay, satır açma. Dışında: şablon içeriği, ADR, kod.
### §2.2 Kapsam Sınırı Testi
Şablona sıra sor: kılavuz → bu dosya · iskelet → `*-template` · kayıt → `index.md` · karar → ADR · kod → `backend/*`.
## §3 Mimari
### §3.1 Klasör Ağacı
`adr`7 `agents`2 `architecture`4 `backend`3 `documentation`8 `frontend`3 `infrastructure`4 `other`2 `query`1 `testing`3 `ui-design`5 = 42 + kök 9 = 51 MD.
### §3.2 Kategori → Hedef
`adr`·`architecture`·`documentation`·`query`·`ui-design`→`.ai/<ad>` · `agents`→`.ai/.agents` · `backend`·`infrastructure`→`src/**` · `frontend`→`src/VersaCoder.Desktop/**` · `testing`→`tests/**`
### §3.3 Okuma Sırası
`index.md` → görev şablonu → kural; ikinci şablon RED; şablon yoksa `index.md` §5.1 Adım 1'den satır açılır.
### §3.4 Şablon Seçme
§2.2'deki ilk EVET şablonu belirler; iskelet uydurulmaz, yer tutucu temizlenir, ölçüm §5.3'e koşulur.
### §3.5 Tür Kodları
15 kod `index.md` §1.3 (`T-ENV`…`T-CLI`); klasör ≠ tür (`#43` `T-FE`, `other/` altında).
### §3.6 Yer Tutucu
Yer tutucu yalnız §3.9 kalıbında yaşar; başka yerde 0 (G4); sızan yer tutucu §6.2 R3.
### §3.7 Kalite Sözleşmesi
`index.md` §3.7: 8 alanlı frontmatter + `§1..§7` + alt bölümler; bu dosya G6'nın tek muafı (90–140 / 5–8).
### §3.8 Yazar / Onay
`docs-writer` (10) → `reviewer` (8) → `architect` (6) → `security` (5) → `master-orchestrator` (5); 3 RED → dur.
### §3.9 Envanter Kalıbı
`| {{NO}} | {{SABLON}} | .ai/.templates/{{BOLU}}/{{SABLON}}.md | {{TUR}} | {{HEDEF}} | {{DURUM}} | {{ROL}} | {{KB}} | {{PART}} |` (index §3.9)
### §3.10 Yasaklar
Vault kopyası (G1), tekrar (G12), bozuk link (G7), kanıtsız sayı (G9) girerse RED.
### §3.11 İzinli Bağlantı
Yalnız `.templates/**` + kök `CLAUDE.md`·`AGENTS.md`·`README.md`·`WORKFLOW.md`; `.ai/*` (index, .rules, adr, architecture) link değil.
## §4 Kurallar
### §4.1 Yazım Sırası
`index.md` satır aç → iskelet → yer tutucu 0 → link → ölçüm → tek geçişte `PLANLI→YAZILDI`.
### §4.2 Onay
§3.8 beş aşama; Quality Report oturum kaydına, red aşamaya 1'e döner.
### §4.3 Ölçüt
Şablon ≥ 480/27,0 KB · bu dosya 90–140/5–8 · envanter 480–780/26–46 · link 0 · katman `l0..l7`.
### §4.4 Değişiklik
metin `patch` · bölüm `minor`+Quality Report · ad/silme `major`+oturum kaydı · durum `patch`.
### §4.5 Hard Guardrails (Otomatik DUR)
| # | Guardrail | İhlal Sonucu |
|---|-----------|--------------|
| G1 | Harici vault'tan cümle (CoreMusic) | **DUR** — özgün yaz |
| G2 | ADO.NET + Dapper + `@p1..@pN` dışı veri erişimi | **DUR** |
| G3 | Frontmatter'sız Markdown (8 alan) | **DUR** — §5.3/2 |
| G4 | Kalıp dışında yer tutucu | **DUR** — §5.3/4 |
| G5 | Mojibake / BOM / karışık dil | **DUR** |
| G6 | < 480 satır (muaf: bu dosya) | **DUR** — §5.3/1 |
| G7 | Bozuk bağlantı | **DUR** — §5.3/3 |
| G8 | `GATE-003` onaysız ekran/kod | **DUR** |
| G9 | ADR'siz mimari karar / kanıtsız sayı | **DUR** |
| G10 | String concat SQL / parametresiz | **DUR** — `Query-Template` |
| G11 | `TreatWarningsAsErrors` + %90 coverage | **DUR** |
| G12 | Kural/envanter kopyası (SRP) | **DUR** — yalnız bağlantı |
## §5 Workflow
### §5.1 9 Adım
1 satır · 2 iskelet · 3 tür+hedef · 4 taslak · 5 yer tutucu 0 · 6 link · 7 ölçüm · 8 part · 9 onay (≈ 45 dk).
### §5.2 Hata Modları
Çift kayıt · sahipsiz `Sorumlu` · bozuk link · şişmiş envanter · yer tutucu sızıntısı · part sıçraması → §6.2.
### §5.3 Salt Okunur Komutlar
```powershell
$d='C:\versacoder\.ai\.templates\CLAUDE.md'; $b=Split-Path $d   # G6: 90-140/5-8
'{0} satır · {1:N1} KB' -f (Get-Content $d).Count, ((Get-Item $d).Length/1KB)
([regex]::Matches((Get-Content $d -Raw),']\(([^)#]+)\)')|%{ $_.Groups[1].Value }|?{ $_ -notmatch '^https?:' }|Sort-Object -Unique|?{ -not (Test-Path (Join-Path $b $_)) }).Count   # G7
$r=Get-Content $d -Raw; ([regex]::Matches($r,'\{\{[A-Z0-9_]+\}\}')).Count; ([IO.File]::ReadAllBytes($d)[0] -eq 0xEF); ($r -match '\uFFFD|\u00C3')   # G4/G5
(Select-String -Path "$b\index.md" -Pattern 'templates/CLAUDE.md').Line   # satır #2
(Get-ChildItem $b -Recurse -Filter *.md).Count   # 51
```
### §5.4 Quality Report
```markdown
## Quality Report — .templates/CLAUDE.md (YYYY-MM-DD HH:MM)
Ölçüm <satır>/<KB> · §6.1 17/17 · link 0 · yer tutucu §3.9 · BOM/mojibake 0 · G1..G12 0 · ONAY/RED (<gerekçe>)
```
## §6 Doğrulama
### §6.1 Kontrol Listesi (17 madde)
| # | Kontrol | Beklenen |
|---|---------|----------|
| K1 | Frontmatter 8 alan + SSOT | 8/8 |
| K2 | title + category: templates Türkçe | var |
| K3 | H1 + Zorunlu Bağlantılar | 1 |
| K4 | §1.1–§1.4 dolu | 4/4 |
| K5 | §2.1–§2.2 dolu | 2/2 |
| K6 | §3.1–§3.11 (11 alt bölüm) | 11/11 |
| K7 | §4.1–§4.5 + G1..G12 | 5+12 |
| K8 | §5.1–§5.4 + Quality Report | 4/4 |
| K9 | §5.3 salt-okunur komut | 4–6 |
| K10 | §6.1 madde | ≥ 17 |
| K11 | §6.2 red sinyalleri | var |
| K12 | §7.1 kaynak satırı | var |
| K13 | §7.2 satır | ≥ 8 |
| K14 | Satır 90–140 · KB 5–8 (G6 muaf) | var |
| K15 | Kırık bağlantı | 0 |
| K16 | Yer tutucu yalnız §3.9 | 10/0 |
| K17 | BOM · mojibake | 0/0 |
### §6.2 Red Sinyalleri
R1 §5.3/3>0 · R2 §6.1<17/17 · R3 yer tutucu · R4 BOM/mojibake · R5 ölçüt dışı · R6 `WIP`/boş `Durum` · R7 `Sorumlu` boş · R8 kural kopyası.
### §6.3 Geçiş Kuralları
Part yalnız ileri A → B → C → D; geri serbest, sıçrama yasak. Kapanış = §5.4 + oturum kaydı.
### §6.4 İlgili Dosyalar
Envanter [index](index.md) · kayıt [session-log](session-log-template.md) · iskelet [arch](architecture/architecture-template.md) · bkz §7.1
### §6.5 Envantere Özgü Kontroller
Satır #2 `Durum = YAZILDI` · `Tahmini KB 7,9` ↔ 5–8 · `.templates/` 51 MD · G6 muafı = bu dosya.
## §7 Referanslar
### §7.1 Kaynaklar
Tek gerçek [index](index.md) · kök [CLAUDE](../../CLAUDE.md) · kapı [WORKFLOW](../../WORKFLOW.md) · gold [form](frontend/form-template.md).
### §7.2 Red Team — Sık Yapılan Hatalar
| # | Hata | Doğrusu |
|---|------|---------|
| 1 | Kural metni buraya kopya | yalnız bağlantı (G12) |
| 2 | Envanter satırı bu dosyada | `index.md` §3.x |
| 3 | İskeleti buraya sığdırmak | ilgili `*--template` |
| 4 | Olmayana bağlantı | §3.11 (G7) |
| 5 | Yer tutucu ile yayınlamak | §5.3/4 (G4) |
| 6 | `Durum = WIP` / boş | `YAZILDI/PLANLI/MEVCUT` |
| 7 | `Sorumlu` boş | 14 rolden biri |
| 8 | Part atlamak | A → B → C → D (§6.3) |

**SSUT:** `.templates/` kuralının tek dizin dosyası bu; kural `.ai/.rules/`'ta, kayıt `index.md`'dedir — kopyalanan her satır ikinci gerçek kaynaktır (G12).
