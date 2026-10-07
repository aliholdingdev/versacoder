---
title: "Şablon — README"
type: template
category: templates/documentation
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "README sablonu: tanitim, kurulum, kullanim ve baglanan belgeler"
---

# Şablon — README (`readme-template.md`)

**Zorunlu Bağlantılar:** [index.md](../index.md) · [docs-md-template.md](docs-md-template.md)

> **Tek cümlelik sorumluluk:** Depo / klasör / katman README'sinin "aç, 30 saniyede anla" düzenini tanımlar.

---

## §1 Kapsam

**İçinde:** depo kökü `README.md`, `src/<Katman>/README.md`, `tests/README.md`.
**Dışında:** `.ai/**` belgeleri (`docs-md-template.md`), wiki sayfaları, API doc.

---

## §2 İskelet

```markdown
---
title: "<Ürün veya Katman> — README"
type: readme
category: docs
status: active
version: 1.0.0
updated: 2026-09-25
authority: SSOT
scope: "<bu README'nin kapsadığı tek dağı>"
---

# <Ad>

> <Tek cümle: bu ne, kime için.>

## 1. Ne Yapar (Answer First)

<3 maddeyi geçmeyen doğrudan cevap.>

## 2. Hızlı Başlangıç

```bash
# ön koşullar
dotnet --version          # >= 10.0 (net10.0-windows bacağı için)
# derleme (iki bacak)
dotnet build
# test
dotnet test
```

## 3. Katmanlar / Kullanım

| Katman | Sorumluluk | Referans |
|---|---|---|
| … | … | … |

## 4. Yapılandırma

| Anahtar | Varsayılan | Not |
|---|---|---|

## 5. Testler

<komut + filtre örneği>

## 6. Mimari Not

<1 ASCII diyagram veya `.ai/architecture/` linki>

## 7. Referanslar

| # | Kaynak | Yol |
|---|---|---|
```

---

## §3 README Kuralları

| # | Kural | Yasak |
|---|---|---|
| 1 | İlk 20 satır tek başına anlaşılmalı | Uzun tarihçe girişimi |
| 2 | Komut **kopyalanabilir** olmalı | Yorumlu, çalışmayan komut |
| 3 | Sürüm sabitlemesi `global.json` + `Directory.Packages.props` ile uyumlu | Elle yazılmış farklı sürüm |
| 4 | Her tablo satırı gerçek disk/ kod karşılığına sahip | Uydurma bileşen |
| 5 | `.ai/` linkleri göreli (`./.ai/index.md`) | Kırık mutlak yol |
| 6 | DevExpress **yalnız v26.1**, EF Core **yok** | Sürüm sapması |

---

## §4 Doğrulama

- [ ] Frontmatter 8 alan
- [ ] "Ne Yapar" ≤ 3 madde
- [ ] Komutlar bu makinede çalışıyor (kanıt: çıktı)
- [ ] Tablo satırları disk/kod ile eşleşiyor
- [ ] Linkler kırık değil
- [ ] Varsayım/kanıt ayrımı açık (`NEEDS_VALIDATION` işaretli)

---

## §5 Referanslar

| # | Kaynak | Yol |
|---|---|---|
| 1 | Genel doküman şablonu | `.ai/.templates/documentation/docs-md-template.md` |
| 2 | Kök MD şablonu | `.ai/.templates/documentation/root-md-template.md` |
| 3 | Kurallar | `.ai/.rules/coding-standards.md` |
