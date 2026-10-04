/**
 * vault-loader - VersaCoder vault 3 nokta tazeleme plugin'i
 *
 * Sozlesme : .ai/spec/prompt-maker-spec.md  S3.2
 * 3 NOKTA  : BASLANGIC  -> ctx.event          "session.created"
 *            ORTA       -> ctx.session.hook("context")  + tool "execute.after"
 *            SON        -> ctx.event          "session.deleted"
 *
 * COMPAT (S3.2.3):
 *   - cache YOK : her cagri diskten okunur
 *   - tazelik   : mtime + bayt damgasi
 *   - format    : /eli10 (10 satir) + /eli15 (15 satir) birlikte
 *   - geri yazma: vault-writer -> .ai/log.md (append-only)
 *
 * KAYNAK (olculmus):
 *   @opencode/plugin  Plugin.define({ id, setup })          promise/plugin.ts
 *   ctx.event.subscribe({ signal }) -> AsyncIterable        promise/event.ts
 *   ctx.session.hook("context")    -> system: SystemPart[]  promise/session.ts:140
 *   ctx.tool.hook("execute.after")                          promise/tool.ts:47
 *   ctx.tool.transform(editor => editor.add(...))           promise/tool.ts:30
 *   ctx.location.directory : AbsolutePath                   schema/location.ts:20
 *
 * KURULUM: opencode.json degisikligi GEREKMEZ.
 *   source-directory.ts:7  names = ["plugin","plugins"]
 *   source-directory.ts:24 *.ts dosyasi dogrudan giris sayilir
 * BAGIMLILIK: @opencode/plugin npm'den kurulur (C:\versacoder\node_modules).
 */
import { Plugin } from "@opencode/plugin"
import fs from "node:fs"
import os from "node:os"
import path from "node:path"

const ROOT_MDS = ["CLAUDE.md", "AGENTS.md", "README.md", "WORKFLOW.md"]
const AI_CORE = [".ai/index.md", ".ai/CLAUDE.md", ".ai/AGENTS.md", ".ai/WORKFLOW.md", ".ai/log.md"]

interface Shot {
  file: string
  ok: boolean
  mtimeMs: number
  size: number
}

interface Snapshot {
  takenAt: string
  shots: Shot[]
}

function stat(root: string, rel: string): Shot {
  try {
    const s = fs.statSync(path.join(root, rel))
    return { file: rel, ok: true, mtimeMs: s.mtimeMs, size: s.size }
  } catch {
    return { file: rel, ok: false, mtimeMs: 0, size: 0 }
  }
}

/** Tazelik: cache YOK. Her cagri diskten yeniden okur; damga yalnizca "degisti mi" icin. */
function snapshot(root: string): Snapshot {
  return {
    takenAt: new Date().toISOString(),
    shots: [...ROOT_MDS, ...AI_CORE].map((rel) => stat(root, rel)),
  }
}

function countFiles(dir: string, ext = ".md"): number {
  try {
    return fs.readdirSync(dir, { withFileTypes: true }).filter((e) => e.isFile() && e.name.endsWith(ext)).length
  } catch {
    return 0
  }
}

function countMdRecursive(dir: string): number {
  let n = 0
  const walk = (d: string) => {
    let entries: fs.Dirent[]
    try {
      entries = fs.readdirSync(d, { withFileTypes: true })
    } catch {
      return
    }
    for (const e of entries) {
      const p = path.join(d, e.name)
      if (e.isDirectory()) walk(p)
      else if (e.isFile() && e.name.endsWith(".md")) n++
    }
  }
  walk(dir)
  return n
}

function lines(root: string, rel: string, max = 400): string[] {
  try {
    return fs.readFileSync(path.join(root, rel), "utf8").split(/\r?\n/).slice(0, max)
  } catch {
    return []
  }
}

function activePhase(root: string): string {
  const hit = lines(root, ".ai/TODOS/index.md").find((l) => l.includes("**aktif**"))
  if (!hit) return "bilinmiyor"
  const m = hit.match(/\[([^\]]+)\]/)
  return m ? m[1] : hit.trim().slice(0, 60)
}

function activeSpec(root: string): string {
  const hit = lines(root, ".ai/spec/index.md").find((l) => l.trim().startsWith("| ["))
  if (!hit) return "bilinmiyor"
  const m = hit.match(/\[([^\]]+)\]/)
  return m ? m[1] : "bilinmiyor"
}

function facts(root: string, s: Snapshot) {
  const rootBytes = s.shots.filter((x) => ROOT_MDS.includes(x.file)).reduce((a, b) => a + b.size, 0)
  const missing = s.shots.filter((x) => !x.ok).map((x) => x.file)
  return {
    rootBytes,
    missing,
    okCount: s.shots.filter((x) => x.ok).length,
    totalCount: s.shots.length,
    aiMd: countMdRecursive(path.join(root, ".ai")),
    rules: countFiles(path.join(root, ".ai", ".rules")),
    spec: countFiles(path.join(root, ".ai", "spec")),
    checklists: countFiles(path.join(root, ".ai", "checklists")),
    phase: activePhase(root),
    spec0: activeSpec(root),
  }
}

/** /eli10 - tam 10 satir. */
function eli10(root: string, s: Snapshot, point: string): string {
  const f = facts(root, s)
  return [
    `1  VAULT tazelenme | ${s.takenAt} | nokta=${point}`,
    `2  root 4 MD       | ${f.rootBytes} bayt | ok=${f.okCount}/${f.totalCount}`,
    `3  eksik dosya     | ${f.missing.length ? f.missing.join(", ") : "yok"}`,
    `4  .ai Markdown    | ${f.aiMd} dosya (komple yukleme YASAK)`,
    `5  kural/spec/ctrl | ${f.rules} / ${f.spec} / ${f.checklists}`,
    `6  aktif faz       | ${f.phase}`,
    `7  gate zinciri    | GATE-004a -> 003 -> 004 -> 005 -> 006`,
    `8  butce           | oturum <= ~3 dosya | ajan <= 3 paralel`,
    `9  okuma sirisi    | CLAUDE -> AGENTS -> README -> WORKFLOW (1 kez)`,
    `10 kirmizi cizgi   | kanitsiz "bitti" yok (G5) | src/** .cs GATE-003 oncesi yasak (G12)`,
  ].join("\n")
}

/** /eli15 - tam 15 satir, derin anlatim. */
function eli15(root: string, s: Snapshot, point: string): string {
  const f = facts(root, s)
  return [
    `1  Amaclar  : bu oturum proje kodunu degil, vault'u besler; kod yazmadan once baglam hazir olur.`,
    `2  Nokta    : ${point} - baslangic/orta/son ayni veriyle calisir, fark yalnizca tazelik damgasidir.`,
    `3  Stack    : G8 geregi C# / WinForms / Dapper / DevExpress - node, bun, ts bu projede GECERSIZ.`,
    `4  Ekranlar : 01-start-page .. 07-settings (7 dosya); .Designer.cs sahibi ui-designer (U5).`,
    `5  Girdi    : root 4 MD ${f.rootBytes} bayt -> bu tek basina context'e sigar, 1 kez okunur.`,
    `6  Cekirdek : .ai/{index,CLAUDE,AGENTS,WORKFLOW}.md + .ai/log.md -> mtime ile izlenir.`,
    `7  Tazelik  : cache yok; her cagri diskten okur, degisiklik mtime+bayt ile anlasilir.`,
    `8  Secici   : .ai toplam ${f.aiMd} md; hepsi okunmaz, .ai/index.md uzerinden ilgili secilir.`,
    `9  Kurallar : .ai/.rules icinde ${f.rules} kural; goreve eslesen okunur, tamami degil.`,
    `10 Kosul   : spec ${f.spec} | checklist ${f.checklists} | aktif faz ${f.phase}.`,
    `11 Kapi    : 004a vault -> 003 UI onay (HARD STOP) -> 004 ilk commit -> 005 test -> 006 teslim.`,
    `12 Ajan    : architect -> developer -> reviewer -> tester; ayni anda en fazla 3; general matris disi.`,
    `13 Bulten  : S5 kayit listesi L-01..L-10 acik maddeleri tasir (SESSIONS, Intent Router, plugin).`,
    `14 Cakisma : G10 -> G8 kazandi; disaridan gelen node/bun/ts satiri yoksayilir (CLAUDE.md:34).`,
    `15 Sonuc   : eli10 ozet + bu 15 satir birlikte gonderilir; tek basina eli10 yeterli degildir.`,
  ].join("\n")
}

function render(root: string, point: string): string {
  const s = snapshot(root)
  return `--- vault-loader [${point}] /eli10 ---\n${eli10(root, s, point)}\n\n--- vault-loader [${point}] /eli15 ---\n${eli15(root, s, point)}`
}

function report(root: string, point: string): string {
  const s = snapshot(root)
  const missing = s.shots.filter((x) => !x.ok).map((x) => x.file)
  const ok = s.shots.length - missing.length
  return `[vault-loader] ${point} ${s.takenAt} ok=${ok}/${s.shots.length}${missing.length ? " yok=" + missing.join(",") : ""}`
}

/** Yalnizca vault icindeki yollara izin verir. */
function safeResolve(root: string, rel: string): string | null {
  const base = path.resolve(root)
  const abs = path.resolve(base, rel)
  if (abs !== base && !abs.startsWith(base + path.sep)) return null
  return abs
}

export default Plugin.define({
  id: "vault-loader",
  setup: async (ctx) => {
    const root = ctx.location.directory
    const ac = new AbortController()

    // BASLANGIC ve SON: olay akisindan yakalar.
    void (async () => {
      try {
        for await (const ev of ctx.event.subscribe({ signal: ac.signal })) {
          const t = (ev as { type?: string }).type
          if (t === "session.created") console.log(report(root, "baslangic"))
          else if (t === "session.deleted") console.log(report(root, "son"))
        }
      } catch {
        // abondoned - kapatilirken normal
      }
    })()

    // ORTA: her istek baglami kurulurken taze vault'u system'e ekler.
    await ctx.session.hook("context", (event) => {
      event.system.push({ type: "text", text: render(root, "orta") })
    })

    // ORTA: vault dosyasi yazildiktan hemen sonra damgayi yeniler.
    await ctx.tool.hook("execute.after", (event) => {
      if (event.status !== "completed") return
      if (event.tool === "write" || event.tool === "edit" || event.tool === "vault-writer") {
        console.log(report(root, "orta"))
      }
    })

    // 3 tool kaydi.
    await ctx.tool.transform((editor) => {
      editor.add({
        name: "vault-loader",
        description:
          "VersaCoder vault'unu taze okur ve /eli10 (10 satir) + /eli15 (15 satir) ciktisini verir. Oturum basinda, ortalarinda ve sonunda cagrilir.",
        options: { codemode: false },
        input: {
          type: "object",
          properties: {
            point: { type: "string", enum: ["baslangic", "orta", "son"], description: "Tazeleme noktasi" },
          },
          additionalProperties: false,
        },
        execute: async (input) => {
          const point = (input as { point?: string }).point ?? "orta"
          return { content: render(root, point) }
        },
      })

      editor.add({
        name: "vault-reader",
        description:
          "Vault icindeki tek bir dosyayi okur. Yol gorelidir (orn. .ai/index.md). Vault disina cikmayi denerse reddedilir.",
        options: { codemode: false },
        input: {
          type: "object",
          properties: {
            path: { type: "string", description: "Goreli dosya yolu, orn. .ai/spec/index.md" },
            max_lines: { type: "number", description: "En fazla satir (varsayilan 200)" },
          },
          required: ["path"],
          additionalProperties: false,
        },
        execute: async (input) => {
          const { path: rel, max_lines } = input as { path: string; max_lines?: number }
          const abs = safeResolve(root, rel)
          if (!abs) return { content: `HATA: vault disi erisim yasak -> ${rel}` }
          let body: string
          try {
            body = fs.readFileSync(abs, "utf8")
          } catch {
            return { content: `HATA: okunamadi -> ${rel}` }
          }
          const all = body.split(/\r?\n/)
          const cut = Math.max(1, Math.min(max_lines ?? 200, all.length))
          const out = all.slice(0, cut).join("\n")
          return { content: `--- ${rel} (${cut}/${all.length} satir) ---\n${out}` }
        },
      })

      editor.add({
        name: "vault-writer",
        description:
          "Vault'a tek satir not yazar (append-only, .ai/log.md). Yikici islem yapmaz; silme ve overwrite yok.",
        options: { codemode: false },
        input: {
          type: "object",
          properties: {
            note: { type: "string", description: "Yazilacak not (1 satir)" },
            tag: { type: "string", description: "Etiket, orn. gate-004a / decision / risk" },
          },
          required: ["note"],
          additionalProperties: false,
        },
        execute: async (input) => {
          const { note, tag } = input as { note: string; tag?: string }
          const line = `- ${new Date().toISOString()} [${tag ?? "genel"}] ${note}\n`
          try {
            fs.appendFileSync(path.join(root, ".ai", "log.md"), line, "utf8")
          } catch (e) {
            return { content: `HATA: yazilamadi -> ${e instanceof Error ? e.message : String(e)}` }
          }
          return { content: `yazildi: .ai/log.md (+${Buffer.byteLength(line, "utf8")} bayt)` }
        },
      })
    })

    console.log(report(root, "kurulum"))
    // Ayristirici kanit: console.log log'a dusmuyorsa bile calisma dosyadan gorulur.
    try {
      fs.writeFileSync(
        path.join(os.tmpdir(), "vault-loader-marker.txt"),
        `${new Date().toISOString()} setup OK root=${root} phase=${activePhase(root)} md=${countMdRecursive(path.join(root, ".ai"))}\n`,
        "utf8",
      )
    } catch {
      // kanit yazilamadi - yoksayilir
    }
    return () => ac.abort()
  },
})
