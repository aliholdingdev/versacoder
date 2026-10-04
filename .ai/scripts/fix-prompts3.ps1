# fix-prompts3.ps1 — 3 eksik prompt'un frontmatter + ZB + TCS tamiratini yapar
# Sahip: architect · Guncelleme: 2026-09-30 · authority: SSOT
# Kapsam: prompts\gate-kapanis-prompt.md, risk-farkindaligi-prompt.md, saf-refactor-prompt.md
# Not: PS 5.1 bu dosyayi ANSI okur — betikte saf ASCII (bolum isareti [char]0xA7).
[CmdletBinding()]
param([string]$Dir = 'C:\versacoder\.ai\prompts')
$ErrorActionPreference = 'Stop'
$sec = [char]0xA7

$jobs = @{}
$jobs['gate-kapanis-prompt.md'] = @{
    title = 'Gate Kapanis Prompt'
    scope = 'Gate kapanisinda 4 satirlik kanit kutusunun degerlendirmesi ve kapat/RED karari icin prompt'
    zb    = '[index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [session-kapanis](session-kapanis-prompt.md) | [docs](docs-prompt.md) | [TODOS](../TODOS/index.md)'
    tcs   = 'Gate kapanisinda 4 satirlik kanit kutusunun dolup dolmadigini degerlendirir; kapat veya RED kararinin tek cevabidir.'
}
$jobs['risk-farkindaligi-prompt.md'] = @{
    title = 'Risk Farkindaligi Prompt'
    scope = 'Erken asama risk kesfi ve onceliklendirme (risk-degerlendirme on adimi) icin prompt'
    zb    = '[index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [VISION.md](../VISION.md) | [risk-degerlendirme](risk-degerlendirme-prompt.md) | [panel-oylama](panel-oylama-prompt.md) | [TODOS](../TODOS/index.md)'
    tcs   = 'Erken asamada risk kesfi yapip onceliklendirir; risk-degerlendirme promptuna girecek girdinin tek cevabidir.'
}
$jobs['saf-refactor-prompt.md'] = @{
    title = 'Saf Refactor Prompt'
    scope = 'Refactor oncesi kapsam siniri tespiti ve guvenli refactor kisitlari icin prompt'
    zb    = '[index.md](index.md) | [AGENTS.md](../AGENTS.md) | [WORKFLOW.md](../WORKFLOW.md) | [ROLE.md](../ROLE.md) | [refactor](refactor-prompt.md) | [developer](developer-prompt.md) | [TODOS](../TODOS/index.md)'
    tcs   = 'Refactor oncesi kapsam sinirini belirler; guvenli refactor kisitlarinin tek cevabidir.'
}

foreach ($n in $jobs.Keys) {
    $j = $jobs[$n]
    $p = Join-Path $Dir $n
    $lines = [System.Collections.Generic.List[string]]::new()
    foreach ($l in [IO.File]::ReadAllLines($p)) { $lines.Add($l) }

    # 1) frontmatter degisimi (ilk '---' ve onu izleyen '---' arasi)
    $end = -1
    for ($i = 1; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -eq '---') { $end = $i; break }
    }
    if ($end -lt 0) { throw ("frontmatter kapanis yok: " + $n) }
    $newFm = @(
        '---'
        ('title: "' + $j.title + '"')
        'type: prompt'
        'category: prompts'
        'status: active'
        'version: 1.0'
        'updated: 2026-09-30'
        'authority: ".ai/AGENTS.md"'
        'owner: "VersaCoder / General"'
        ('scope: "' + $j.scope + '"')
        '---'
    )
    $lines.RemoveRange(0, $end + 1)
    $lines.InsertRange(0, [string[]]$newFm)

    # 2) H1 sonrasi Zorunlu Baglantilar + Tek cumlelik sorumluluk
    $h1 = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^# ') { $h1 = $i; break }
    }
    if ($h1 -lt 0) { throw ("H1 yok: " + $n) }
    $ins = @(
        ''
        ('**Zorunlu Baglantilar:** ' + $j.zb)
        ''
        ('> **Tek cumlelik sorumluluk:** ' + $j.tcs)
        ''
    )
    $lines.InsertRange($h1 + 1, [string[]]$ins)

    # 3) typo + bozuk related yolu + §0 basligi
    $txt = ($lines -join "`r`n")
    $txt = $txt.Replace('Adim adim yurtey', 'Adim adim yurutme')
    $txt = $txt.Replace('Kanim kurallari', 'Kanit kurallari')
    $txt = $txt.Replace('../playbooks/risk-degerlendirme.md', 'risk-degerlendirme-prompt.md')
    $txt = [regex]::Replace($txt, '(?m)^## 0\. ', ('## ' + $sec + '0 Ozet (Answer First) - '))

    [IO.File]::WriteAllText($p, $txt, (New-Object System.Text.UTF8Encoding($false)))
    Write-Host ("  duzeltildi: {0}" -f $n)
}

# dogrulama
Write-Host ''
foreach ($n in $jobs.Keys) {
    $p = Join-Path $Dir $n
    $zb  = [bool](Select-String -Path $p -Pattern 'Zorunlu Baglantilar' -Quiet)
    $tcs = [bool](Select-String -Path $p -Pattern 'Tek cumlelik sorumluluk' -Quiet)
    $au  = [bool](Select-String -Path $p -Pattern '^authority:' -Quiet)
    $fm  = (Select-String -Path $p -Pattern '^(title|type|category|status|version|updated|authority|owner|scope):').Count
    Write-Host ("  {0,-32} ZB={1,-5} TCS={2,-5} auth={3,-5} fm={4}" -f $n, $zb, $tcs, $au, $fm)
}
$typos = (Get-ChildItem -LiteralPath $Dir -Filter *.md | Where-Object {
    (Select-String -Path $_.FullName -Pattern 'yurtey|Kanim' -Quiet)
}).Count
Write-Host ("  kalan typo dosyasi: {0}" -f $typos)
exit 0
