---
title: AGENTS - repo koku OpenCode talimatlari (oturum boot)
type: boot-instruction
category: root
status: active
version: 1.0.0
updated: 2026-10-01
authority: SSOT
scope: "Repo koku; oturum baslangici, okuma sirasi, ajan dagitimi, cikti kutusu, kapi kurali"
---

# AGENTS - Oturum boot dosyasi

> **Tek cumlelik sorumluluk:** bu dosya oturumun OpenCode tarafindan otomatik
> yuklenen tek talimatidir; okuma sirasini, ajan dagitimini, cikti kutusunu ve
> kapi kuralini dayatar. Kural metni degil, **yonlendirme** tasir (SSOT: `.ai/**`).

## §0 Cevap uslubu (hiz)

- Cevap **ilk satirda**: komut / yol / snippet once, aciklama sonra.
- Uzun dusunce yurutme yok: karar ver, uygula, kaniti goster. Emin olmadigin
  tek seyi **tek** kisa soruyla sor.
- Liste en fazla **5 madde**; her adim tek is sinirli; sure somut dk/sn.
- Hata: konum + neden + duzeltme. 3 basarisiz denemede **dur**, supheli
  varsayimi adlandir (3-STRIKE, `.ai/AGENTS.md` §4).
- Gereksiz giris/ozet/oneri yazma; is bitince kisa kutu don.

## §1 Oturum baslangici - okuma sirasi (zorunlu)

1. Kok md'ler sirayla: `CLAUDE.md` -> `AGENTS.md` (bu) -> `README.md` -> `WORKFLOW.md`.
2. `.ai/.rules/index.md` + goreve **eslesen** kural dosyalari (16 dosyanin tamami degil).
3. Gorevle ilgili `.ai/**` dosyalari; kaybolma halinde navigasyon `.ai/index.md` §3.
4. Ic is akisi: hedef dosyayi **oku** -> kodu yaz -> `rules-check` calistir ->
   hata kurtarma dongusu -> UI kontrol -> tarayici testi -> commit.

## §2 Ajan dagitimi (matris: `.ai/AGENTS.md` §1)

| Is tipi | Ajan | Kanit |
|---|---|---|
| Mimari / katman | `architect` | ADR + `layer-boundary-check` 13 soru |
| Kod / refactor | `developer` | diff + `dotnet build` + `dotnet test` |
| Inceleme | `reviewer` | suc dosya + satir + dolu `RISK` satiri |
| Test / kapsam | `tester` | `dotnet test` ciktisi + kapsam raporu |
| Guvenlik | `security-auditor` | OWASP maddesi + severity |
| UI / ekran | `ui-designer` | `01-start-page`..`07-settings` + glossary |
| Matris disi | `general` | komut ciktisi / satir no (kendi sozu degil) |

- Bu 7 ajan `opencode.json`'da **dogrudan** `.ai/.agents/*-agent.md` dosyalarina
  baglidir; profil metni **kopyalanmaz**, tek kaynaktadir.
- Ayni anda en fazla **3** paralel ajan; ayni dosyaya iki yazici yok (seriye al).
- `general` her isi yapabilir ama matristeki **uzman isi yapamaz** (yanlis kanit riski).
- Devirde 4 madde zorunlu: `KALAN IS / DOSYA / KANIT / SINIR`.

## §3 Cikti kutusu (her ajan, saf ASCII)

```text
+--------------------------------------------------+
| BASLIK : 2-4 kelime, ne yapildi                  |
| KAPSAM : dokunulan dosya yollari veya "yok"      |
| KARAR  : secim + tek satir gerekce                |
| RISK   : en fazla 1 madde; yoksa "yok"            |
| KANIT  : komut ciktisi / satir no / test sonucu   |
+--------------------------------------------------+
```

- `KANIT` bos ise kutu **islenmez** (G5); `RISK` bos birakilamaz.
- Unicode kutu cizimleri (`U+2500-U+257F`) **yasak**; `+ - |` ile cizilir.
- Bu 5 alanli ajan kutusu ile `WORKFLOW.md`'deki 4 satirli kanit kutusu
  (`KANIT/DOSYA/EKSIK/SONRAKI`) **ayni sey degildir**; biri digerinin yerine gecmez.

## §4 Kapilar (kapiyi yalniz kullanici acar)

`GATE-004a (vault) -> GATE-003 (UI onayi, HARD STOP) -> GATE-004 (iskelet + ilk commit) -> GATE-005 (test) -> GATE-006 (teslim)`

- GATE-003 onayindan **once `src/**` altina tek satir `.cs` bile yazilmaz** (G12);
  onaysiz yazilan geri alinir. Is gecikirse `GATE-OPEN` etiketiyle dur, kod yazma.
- Ilk git commit **yalniz** GATE-004'te; `git add -A` yasak (dosya bazli `add`).
- Kapinin RED verdigi is **ureten ajana** geri doner, baska ajana devredilmez.

## §5 Kizil cizgiler (metin: `.ai/CLAUDE.md` G1..G12)

- Sifir halluksinasyon (G9): iddia etiket + kaynak icermiyorsa **silinir**.
- Kanitsiz "bitti/hazir/tamam" is bitmis sayilmaz (G5).
- Yikici islem (silme, overwrite, `reset --hard`, `push --force`) **onay** ister (G7).
- `.ai/**` Markdown'ta **8 alanli** YAML frontmatter zorunlu (G4).
- Veri erisimi ADO.NET + Dapper, parametreli sorgu; **EF Core yasak** (G2).

## §6 Prompt Maker

`/prompt-maker <prompt>` -> `prompt-maker` skill'i: oturum/sans **acilmaz**, once
**ASKING QUESTIONS** modu (proje-ozel sorular, jenerik 8 soru yasak), cevaplardan
sonra skill'in §4 sablonu (`✅ Prompt Cevaplarla İşlendi`). Model/provider
**elle atanmaz**.

## §7 Komutlar ve yetenekler

- Komutlar: `/prompt-maker <prompt>` · `/rules [dosya]` · `/gate` (`.opencode/commands/`).
- Skill'ler: `prompt-maker`, `rules-check` (`.opencode/skills/`).
- Ajanlar: `opencode.json` -> `.ai/.agents/*-agent.md` (7 adet).
