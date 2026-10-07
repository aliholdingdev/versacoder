---
title: "LLM Wiki — Anayasa"
type: constitution
category: wiki-governance
version: 1.0.0
status: active
authority: SSOT
created: 2026-10-06
updated: 2026-10-06
---

# LLM Wiki — Anayasa (CLAUDE.md)

**Zorunlu Bağlantılar:** [[index]] · [[log]] · [[raw/README]] · [[sources/özet]]

---

## §1 Amaç

Bu wiki senin **uzun süreli hafızandır**. Kısa oturum hafızası silindiğinde bilgi burada kalır.

**Her oturumun başında okuma sırası (zorunlu):**

1. `CLAUDE.md` (bu dosya)
2. `index.md`
3. `log.md` → **son 10 kayıt**

Bu üç adım tamamlanmadan INGEST, QUERY veya LINT çalıştırılmaz.

---

## §2 Çoklu-LLM Kuralı (Bağımsız Çalışma)

- Wiki **herhangi bir LLM modeliyle** (Claude, ChatGPT, Gemini, Cursor, OpenCode vb.) bağımsız çalışır.
- Hiçbir model başka bir modelin oturum durumuna, hafızasına veya notlarına bağlı değildir; **tek bağımsız girdi bu `.ai/` klasörüdür**.
- Her model §1 okuma sırasını kendi başına yapar; modelsellik, oturum veya araç farkı wiki içeriğini değiştirmez.
- Bir model wiki'yi değiştirirse değişiklik **dosyaya** yansır, oturuma değil — böylece diğer model bir sonraki oturumda aynı durumu görür.

---

## §3 Klasör Yapısı

```
.ai/
├── CLAUDE.md      # anayasa (bu dosya)
├── index.md       # katalog
├── log.md         # append-only işlem defteri
├── raw/           # ham kaynaklar — ASLA değiştirilmez, sadece okunur
│   └── README.md
├── sources/       # her kaynağın özeti + ana tablo özet.md
└── wiki/          # kavram, kişi, araç, proje sayfaları
```

- **raw/**: ham kaynak (makale, transkript, not, PDF, web klibi). Değiştirilmez, silinmez, sadece okunur.
- **sources/**: her raw dosyası için bir özet dosyası + `sources/özet.md` ana tablosu.
- **wiki/**: işlenmiş bilgi sayfaları (kavram / kişi / araç / proje).

---

## §4 Dosya Adı Kuralı

- Küçük harf, **tire** ile ayrılmış: `agent-memory.md`
- Türkçe karakter yok, boşluk yok, uzantı `.md`
- Örn: `versacoder-proje-planlamasi-ai-00.md`, `ingest-proseduru.md`

---

## §5 Sayfa Şablonu

Her `wiki/` sayfasının başında frontmatter, sonra içerik, en altta **İlgili Sayfalar**:

```markdown
---
title: Sayfa Başlığı
type: kavram | kisi | arac | proje | kaynak
created: YYYY-MM-DD
updated: YYYY-MM-DD
sources:
  - raw/dosya.md
tags: [etiket1, etiket2]
---

İçerik…

## İlgili Sayfalar
- [[ilgili-sayfa]] — neden ilgili
```

**Zorunlu frontmatter alanları:** `title`, `type`, `created`, `updated`, `sources`, `tags`.
`sources` alanı boş bırakılamaz; kaynaksız iddia wiki kuralına aykırıdır.

---

## §6 Sayfalar Arası Bağlantı Kuralı

- Format: `[[sayfa-adı]]` (uzantısız, küçük harf-tire).
- İlgili **her** sayfaya çapraz link verilir (bidirectionally: hem gelen hem giden).
- Var olmayan bir sayfaya link verilmez (kırık link = LINT hatası).
- `index.md` gelen link saymaz; gerçek `wiki/` sayfaları gelen link üretir.

---

## §7 Üç İşlem

| İşlem | Tanım |
|-------|-------|
| **INGEST** | Yeni kaynağı işle: oku → özetle → wiki sayfaları üret/güncelle → index + log yaz. |
| **QUERY** | Soruya wikiden cevap ver: önce index, sonra ilgili sayfalar; cevap kaynağını belirt. |
| **LINT** | Sağlık kontrolü: yetim sayfa, kırık link, eksik frontmatter, indeks tutarsızlığı, log formatı vb. |

---

## §8 ZORUNLU KURAL (atlanamaz)

> Wikide **dosya oluşturduğunda, güncellediğinde veya sildiğinde**:
> 1. `index.md`'yi güncelle
> 2. `log.md`'ye kayıt ekle
>
> Bu iki adım hiçbir şekilde atlanamaz. Eksikse işlem tamamlanmamıştır.

---

## §9 INGEST Prosedürü

1. `raw/` içindeki henüz `sources/` altında özeti olmayan dosyaları tespit et.
2. Her dosyayı **baştan sona** oku.
3. Kullanıcıya en önemli **3-5 çıkarımı** kısaca söyle.
4. `sources/` altında o kaynağın özet dosyasını oluştur (`sources/kaynak-adi.md`).
5. Kaynaktan çıkan kavram, kişi, araç ve projeler için `wiki/` altında sayfa oluştur; sayfa zaten varsa **yeni bilgiyi ekleyerek güncelle**.
6. Yeni bilgi eski bilgiyle çelişiyorsa sayfaya **`⚠ Çelişki`** notu ekle ve iki kaynağı da belirt.
7. Sayfalar arasına `[[çapraz link]]` ekle.
8. `index.md`'yi güncelle.
9. `log.md`'ye tarihli **ingest** kaydı ekle (hangi kaynak, kaç sayfa oluştu, kaç sayfa güncellendi).

**Hedef:** Bir kaynak işlendiğinde wiki'de ortalama **5-15 sayfa** etkilenmesi normaldir.

### §9.1 sources/özet.md zorunluluğu

`raw/` altına yeni bir dosya eklendiğinde ve ingest yapıldığında `sources/özet.md`
tablosuna **yeni satır** eklenmeli ve üstteki **kaynak sayısı** ile **son güncellenme
tarihi** güncellenmelidir. **Bunu yapmadan ingest tamamlanmış sayılmaz.**

---

## §10 TARİH KURALı

- Tarihi **asla tahmin etme**.
- Her log kaydından önce terminalde gerçek tarihi al:

```bash
date "+%Y-%m-%d %H:%M"      # POSIX
Get-Date -Format "yyyy-MM-dd HH:mm"   # Windows PowerShell karşılığı
```

- Çıkan gerçek değeri kullan; uydurma tarih log'u geçersiz kılar.

---

## §11 LINT Çıktı Formatı

Her madde için `✅` (sorun yok) veya `❌` (sorun var) + bulunan dosya adları,
en sonda **öncelik sırasına göre düzeltme önerileri**.

> **Onay kapısı:** Kullanıcı onay vermeden hiçbir LINT bulgusu düzeltilmez.
> Onay sonrası: düzelt → `index.md`'yi güncelle → `log.md`'ye tarihli **lint**
> kaydı ekle (kaç sorun bulundu, kaçı düzeltildi).
