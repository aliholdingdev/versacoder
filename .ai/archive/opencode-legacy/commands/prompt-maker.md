---
description: Prompt'u oturum acmadanOnce proje-ozel sorularla karsilayip isler (ASKING QUESTIONS -> Prompt Maker)
---

PROMPT MAKER GIRISI

Orijinal prompt: $ARGUMENTS

1. Oturum/sans ACMA, kod yazma, gate acma, commit atma.
2. `prompt-maker` skill'ini yukle (skill tool, id: `prompt-maker`) ve §1'e gore
   hemen **MOD A / ASKING QUESTIONS**'a gec: sorular proje-ozel olmali
   (kaynak: `.ai/PROJECTS.md`, `.ai/VISION.md`, `.ai/spec/`, `.ai/.decisions/adr-index.md`,
   `.ai/glossary.md` + varsa proje kodu); jenerik 8 soru yasak; teker teker sor,
   A/B/C secenekli, cevap bekle; varsayilan tavan 10 soru.
3. Cevaplar tamamlaninca §2 **MOD B**'yi calistir: /eli10 paragraf genisletme ->
   `.ai` vault referansi -> proje kodu referansi -> karar/soru/cevap tablosu ->
   orijinal prompt birebir -> gorev ayristirma -> footer.
4. Cikti skill §4 sablonunu **takim sirasiyla** verir; footer satirlari aynen:
   `✅ Prompt Cevaplarla İşlendi`
   `Dil: Türkçe | Görev Sayısı: [n] | Cevap: [n]`
5. Model, provider veya varyant **elle atama**; OpenCode'un o an sectigi model ile calis.
