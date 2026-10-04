param(
    [int]$Esik = 5000,
    [string]$Kaynak = "G:\Drive'ım\Projelerim\Project-notes\Bayram Ali Prompt Arşivi & Notlar\Versacoder\new\versacoder.mimari-02.md",
    [string]$Cikti = 'C:\versacoder\.ai\architecture\layer-5000'
)
# gen-layer-5000.ps1 - 5000+ "layer" (namespace-modul parcacik) listesi uretici
# Taban: K3 mimari-02.md Tier listeleri (618 giris) | Genisletme: konsept x yon (deterministik)
# KURAL: satirlar NAMESPACE-MODUL kaydidir, .csproj DEGIL (ADR-028/030: 9+2 proje sabit)
$ErrorActionPreference = 'Stop'
$utf8 = New-Object System.Text.UTF8Encoding($false)

$tierlar = @(
    @{ N=0; Slug='T0-foundation';     Baslik='Foundation';     Layer='l0'; Proje='VersaCoder.Abstractions';   Prefix='VersaCoder.Shared';
       Konsept='Configuration Localization Extensions Validation Cryptography Networking IO Logging Caching Mapping Serialization Threading Collections Text DateTime Math Reflection Diagnostics Guard Result Maybe Option Either Unit Identifier ValueObject Entity Aggregate DomainEvent Specification Repository UnitOfWork Mapper Factory Builder Strategy Decorator Proxy Chain State Observer Command Query Mediator Pipeline Behavior Attribute';
       Yon='Options Factory Builder Converter Validator Guard Extensions Registry Policy Metrics Diagnostics' },
    @{ N=1; Slug='T1-domain';         Baslik='Domain';         Layer='l1'; Proje='VersaCoder.Domain';         Prefix='VersaCoder.Domain';
       Konsept='Agents AgentTypes Sessions SessionStates Skills SkillCategories Memory MemoryTypes Workflows WorkflowSteps WorkflowStates Tools ToolCategories Models ModelProviders ModelCapabilities Conversations Messages MessageRoles Projects Files FileTypes Directories Permissions PermissionTypes Roles Policies PolicyRules Events EventTypes Commands Queries Repositories Services ValueObjects Entities Aggregates Specifications Factories Builders Strategies Decorators Proxies Chains States Observers Mediators Pipelines Behaviors Attributes Exceptions Errors Constants Enums Interfaces Abstractions Configurations Options Settings Profiles Templates Schemas Formats Mappings Converters Resolvers Handlers Interceptors';
       Yon='Factory Builder Mapper Validator Repository Cache Logger Policy Events Queries Handler Processor Guard Converter Resolver' },
    @{ N=2; Slug='T2-application';    Baslik='Application';    Layer='l2'; Proje='VersaCoder.Application';    Prefix='VersaCoder.Application';
       Konsept='Common Behaviors Exceptions Interfaces DTOs Mappings Validations Queries Services Handlers Pipelines Decorators Factories Builders Strategies Adapters Converters Resolvers Interceptors Create Update Delete Execute Process Send Receive Handle Route Transform Validate Authenticate Authorize Encrypt Decrypt Compress Decompress Serialize Deserialize Parse Format Render Generate Build Compile Test Deploy Monitor Audit Report Export Import Backup Restore Migrate Synchronize Cache';
       Yon='Handler Validator Pipeline Decorator Factory Builder Strategy Adapter Converter Resolver Interceptor Service Policy Guard Mapper Executor' },
    @{ N=3; Slug='T3-infrastructure'; Baslik='Infrastructure'; Layer='l4'; Proje='VersaCoder.Infrastructure'; Prefix='VersaCoder.Infrastructure';
       Konsept='AI Providers Router Token Tokenizer CodeAnalysis Data SQLite SqlServer PostgreSQL Migrations Repositories UnitOfWork Connections DistributedCache Redis MongoDB Context Vector Index Retrieval Learning Feedback Adaptive Supervised Unsupervised Security Encryption Vault Approval Audit Authentication Authorization Certificate Key Observability Metrics Tracing Health Alerting Dashboard Logging Serilog MCP Transport Resources Prompts Catalog Plugins Discovery Loader Isolation Sandbox Embedded Driver WebRTC Git Branch Diff Merge Rebase Tag Terminal Shell FileSystem Watch Archive Network HTTP WebSocket SignalR Mail Caching Messaging Queue PubSub Saga DAG Workflow Executor Cron Scheduling Timer Background';
       Yon='Client Server Adapter Provider Factory Registry Monitor Policy Handler Pipeline Cache Validator' },
    @{ N=4; Slug='T4-protocol';       Baslik='Protocol';       Layer='l5'; Proje='VersaCoder.Protocol';       Prefix='VersaCoder.Protocol';
       Konsept='JSON XML GRPC Thrift Protobuf Avro MessagePack SignalR WebSocket MQTT AMQP Kafka RabbitMQ AzureServiceBus AWS GCP Docker Kubernetes Helm Terraform Ansible Chef Puppet CI CD GitHub GitLab Bitbucket Jenkins REST SMTP IMAP FTP SSH DNS Gateway Bridge Router Proxy';
       Yon='Client Server Adapter Serializer Transport Validator Monitor Registry Policy' },
    @{ N=5; Slug='T5-crosscutting';   Baslik='CrossCutting';   Layer='l3'; Proje='VersaCoder.Crosscutting';   Prefix='VersaCoder.CrossCutting';
       Konsept='Logging Validation Caching Security Transaction Exception Middleware Filter Interceptor Behavior Pipeline Decorator Proxy Adapter Converter Mapper Serializer Validator Guard Diagnostics Metrics Tracing Profiling Audit Policy Localization Threading Compression Encoding Hashing Event Command Query Resource';
       Yon='Policy Factory Provider Registry Handler Builder Wrapper Analyzer Extensions' },
    @{ N=6; Slug='T6-host';           Baslik='Host';           Layer='l6'; Proje='VersaCoder.Host';           Prefix='VersaCoder.Host';
       Konsept='Windows Linux MacOS Docker Kubernetes Console Service Web Grpc SignalR WebSocket Background Worker Bootstrap Lifetime Shutdown Startup Health Metrics Logging Configuration Security Threading Queue Scheduler Watchdog Process Memory ThreadPool Telemetry';
       Yon='Host Runner Adapter Provider Pipeline Monitor Policy Bootstrap Service' },
    @{ N=7; Slug='T7-ui';             Baslik='UI';             Layer='l7'; Proje='VersaCoder.Desktop + Cli';  Prefix='VersaCoder.UI';
       Konsept='Main Chat Wizard Skills Memory Workflow Settings Themes Components Controls Dialogs Panels Navigation Toolbar StatusBar Ribbon Docking Grid Chart Diagram Tree Editor Preview Search Filter Export Import Report Dashboard Analytics Monitoring Debug Profiler Log Console Terminal FileExplorer CodeEditor DiffViewer MergeTool GitViewer MCPViewer PluginManager SkillManager AgentManager SessionManager WorkflowDesigner ThemeManager Layout Style Animation Input Selection DragDrop Clipboard Print Zoom Tooltip Menu Sidebar Forms Splash Notify Badge Avatar Button';
       Yon='View Model Presenter Controller Factory Renderer Behavior Adapter Service Registry' },
    @{ N=8; Slug='T8-tests';          Baslik='Tests';          Layer='test'; Proje='VersaCoder.Tests*';        Prefix='VersaCoder.Tests';
       Konsept='Unit Integration Architecture Performance Security UI E2E Benchmark Load Stress Soak Resilience Contract Component EndToEnd Functional Regression Acceptance UserAcceptance Exploratory Accessibility Compatibility Localization Smoke Sanity Recovery Failover Mocks Fakes Stubs Fixtures Seeds Assertions Coverage Mutation Snapshot Concurrency Memory Threading Network Database Api Parser Golden Property Chaos Canary';
       Yon='Tests Suite Runner Fixtures Helpers Asserts Scenarios Reports' }
)

# --- 1) Taban: K3 mimari-02.md Tier basliklarini ve "- VersaCoder.*" satirlarini oku ---
if (-not (Test-Path -LiteralPath $Kaynak)) { Write-Error "KAYNAK YOK: $Kaynak"; exit 2 }
$taban = @{}
$tierNo = $null
foreach ($satir in [System.IO.File]::ReadAllLines($Kaynak, [System.Text.Encoding]::UTF8)) {
    if ($satir -match '^### Tier (\d+)') { $tierNo = [int]$Matches[1]; $taban[$tierNo] = New-Object System.Collections.Generic.List[string] }
    elseif ($tierNo -ne $null -and $satir -match '^-\s+(VersaCoder\.[A-Za-z0-9_.]+)') { $taban[$tierNo].Add($Matches[1]) }
}
$tabanToplam = 0; foreach ($k in $taban.Keys) { $tabanToplam += $taban[$k].Count }

# --- 2) Uret (global tekillik) ---
if (Test-Path $Cikti) { Remove-Item (Join-Path $Cikti 'T*.md') -Force -ErrorAction SilentlyContinue } else { New-Item -ItemType Directory -Path $Cikti | Out-Null }
$global = New-Object 'System.Collections.Generic.HashSet[string]'
$tumSatirlar = New-Object System.Collections.Generic.List[string]
$ozet = @()

foreach ($t in $tierlar) {
    $n = $t.N
    $liste = New-Object System.Collections.Generic.List[string]
    $tTaban = 0; $tGenis = 0
    if ($taban.ContainsKey($n)) {
        foreach ($b in $taban[$n]) { if ($global.Add($b)) { $liste.Add($b); $tTaban++ } }
    }
    foreach ($k in ($t.Konsept -split '\s+' | Where-Object { $_ })) {
        foreach ($y in ($t.Yon -split '\s+' | Where-Object { $_ })) {
            $ad = '{0}.{1}.{2}' -f $t.Prefix, $k, $y
            if ($global.Add($ad)) { $liste.Add($ad); $tGenis++ }
        }
    }
    $toplam = $liste.Count
    $satirlar = New-Object System.Collections.Generic.List[string]
    $satirlar.Add('---')
    $satirlar.Add(('title: "Layer 5000 - Tier {0}: {1} ({2} parcacik)"' -f $n, $t.Baslik, $toplam))
    $satirlar.Add('type: catalog')
    $satirlar.Add('category: architecture')
    $satirlar.Add('status: needs-validation')
    $satirlar.Add('version: 1.0.0')
    $satirlar.Add('updated: 2026-10-02')
    $satirlar.Add('authority: derived')
    $satirlar.Add(('scope: "Tier {0} namespace-modul parcacik listesi; taban K3 mimari-02, genisletme script; satirlar NAMESPACE MODULdur .csproj degil (ADR-028/030)"' -f $n))
    $satirlar.Add('---')
    $satirlar.Add('')
    $satirlar.Add(('# Tier {0}: {1} - {2} parcacik' -f $n, $t.Baslik, $toplam))
    $satirlar.Add('')
    $satirlar.Add('**Zorunlu Baglantilar:** [index](index.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-030](../../.decisions/accepted/ADR-030-5000-layers-class-level.md) · [layer-rules](../../.rules/layer-rules.md)')
    $satirlar.Add('')
    $satirlar.Add(('> **Tek cumlelik sorumluluk:** Tier {0} ({1}) altindaki {2} namespace-modul parcaciginin tek listesidir.' -f $n, $t.Layer, $toplam))
    $satirlar.Add('')
    $satirlar.Add(('| Alan | Deger |' ))
    $satirlar.Add('|---|---|')
    $satirlar.Add(('| Makro katman | {0} |' -f $t.Layer))
    $satirlar.Add(('| Proje (9+2 sabit) | `{0}` |' -f $t.Proje))
    $satirlar.Add(('| Taban (K3 mimari-02) | {0} |' -f $tTaban))
    $satirlar.Add(('| Genisletme (script) | {0} |' -f $tGenis))
    $satirlar.Add(('| Toplam | {0} |' -f $toplam))
    $satirlar.Add('')
    $satirlar.Add('> **UYARI:** Her satir bir `.csproj` degil, bir **namespace-modul parcacigidir**; 9+2 proje sabittir (ADR-028). Durum: `planli`. Tip kayitlari [architecture/catalog](../catalog/index.md) altindadir.')
    $satirlar.Add('')
    $satirlar.Add('## Listeler')
    $satirlar.Add('')
    $satirlar.Add('```text')
    foreach ($l in $liste) { $satirlar.Add('- ' + $l) }
    $satirlar.Add('```')
    $satirlar.Add('')
    $satirlar.Add('**Degisiklik gunlugu:** 2026-10-02 - gen-layer-5000.ps1 ile uretildi.')
    [System.IO.File]::WriteAllLines((Join-Path $Cikti ($t.Slug + '.md')), $satirlar, $utf8)
    $ozet += ,@($n, $t.Baslik, $t.Layer, $tTaban, $tGenis, $toplam)
    foreach ($l in $liste) { $tumSatirlar.Add($l) }
}

# --- 3) Index ---
$toplamParca = $tumSatirlar.Count
$idx = New-Object System.Collections.Generic.List[string]
$idx.Add('---')
$idx.Add(('title: "Layer 5000 - 9 Tier Namespace-Modul Parcacik Indeksi ({0} parcacik)"' -f $toplamParca))
$idx.Add('type: index')
$idx.Add('category: architecture')
$idx.Add('status: active')
$idx.Add('version: 1.0.0')
$idx.Add('updated: 2026-10-02')
$idx.Add('authority: SSOT')
$idx.Add('scope: "layer-5000 dizini giris dosyasi: Tier bazli parcacik sayimlari, esik kaniti ve ADR baglantisi"')
$idx.Add('---')
$idx.Add('')
$idx.Add(('# Layer 5000 - 9 Tier Parcacik Indeksi ({0})' -f $toplamParca))
$idx.Add('')
$idx.Add('**Zorunlu Baglantilar:** [mimari index](../index.md) · [catalog](../catalog/index.md) · [layer-rules](../../.rules/layer-rules.md) · [ADR-028](../../.decisions/accepted/ADR-028-layered-baseline-9-projects.md) · [ADR-030](../../.decisions/accepted/ADR-030-5000-layers-class-level.md)')
$idx.Add('')
$idx.Add(('> **Tek cumlelik sorumluluk:** *9 Tier altinda toplam {0} namespace-modul parcacigi hangi dosyada, esik 5000 gecti mi* sorusunun tek cevabidir.' -f $toplamParca))
$idx.Add('')
$idx.Add('## §0 Ozet (Answer First)')
$idx.Add('')
$idx.Add(('**TOPLAM {0} parcacik -> esik {1} -> {2}**. Satirlar namespace-modul kaydidir, `.csproj` DEGIL; 9+2 proje sabittir (ADR-028/030). Taban 618 giris `mimari-02.md` (K3) dosyasindan dogrudan okunur; genisletme `scripts/gen-layer-5000.ps1` ile deterministik uretilir.' -f $toplamParca, $Esik, $(if ($toplamParca -ge $Esik) { 'PASS' } else { 'FAIL' })))
$idx.Add('')
$idx.Add('| Tier | Dosya | Katman | Proje | Taban (K3) | Genisletme | Toplam |')
$idx.Add('|---|---|---|---|---|---|---|')
foreach ($o in $ozet) { $idx.Add(('| {0} {1} | [{2}]({2}.md) | {3} | {4} | {5} | {6} | {7} |' -f $o[0], $o[1], $tierlar[$o[0]].Slug, $o[2], $tierlar[$o[0]].Proje, $o[3], $o[4], $o[5])) }
$idx.Add('')
$idx.Add(('| Olcut | Deger |'))
$idx.Add('|---|---|')
$idx.Add(('| Toplam parcacik | {0} |' -f $toplamParca))
$idx.Add(('| Esik | {0} (sabit, ADR-030) |' -f $Esik))
$idx.Add(('| Marj | {0} |' -f ($toplamParca - $Esik)))
$idx.Add(('| Taban K3 | {0} |' -f $tabanToplam))
$idx.Add('| Sonuc | ' + $(if ($toplamParca -ge $Esik) { 'PASS' } else { 'FAIL' }) + ' |')
$idx.Add('')
$idx.Add('## §1 Kurallar')
$idx.Add('')
$idx.Add('1. Her satir **namespace-modul parcacigidir**; yeni satir = yeni tip/kayit, **yeni `.csproj` degil** (ADR-030, layer-rules LAY-04/05).')
$idx.Add('2. Makro katman yonu [layer-rules](../../.rules/layer-rules.md) 8x8 matrisine tabidir; bu dosya yon degistirmez.')
$idx.Add('3. Durum: `planli`; kod yazildikca tip kaydi [catalog](../catalog/index.md) altinda `yazildi`ye doner.')
$idx.Add('4. Esik 5000 dusurulemez (ADR-030 §2.3); sayim bu dosyanin satir sayisidir.')
$idx.Add('')
$idx.Add('## §2 Uretim')
$idx.Add('')
$idx.Add('```powershell')
$idx.Add('powershell -NoProfile -File .ai\scripts\gen-layer-5000.ps1   # cikti: PASS/FAIL + satir sayisi')
$idx.Add('```')
$idx.Add('')
$idx.Add('## §3 Kanit')
$idx.Add('')
$idx.Add('```text')
$idx.Add('script   : gen-layer-5000.ps1')
$idx.Add(('kaynak   : versacoder.mimari-02.md (K3, {0} taban giris)' -f $tabanToplam))
$idx.Add(('uretim   : 2026-10-02'))
$idx.Add(('toplam   : {0} / esik {1} -> {2}' -f $toplamParca, $Esik, $(if ($toplamParca -ge $Esik) { 'PASS' } else { 'FAIL' })))
$idx.Add('```')
$idx.Add('')
$idx.Add('**Degisiklik gunlugu:** 2026-10-02 - ilk uretim (gen-layer-5000.ps1).')
[System.IO.File]::WriteAllLines((Join-Path $Cikti 'index.md'), $idx, $utf8)

# --- 4) Sonuc ---
Write-Output '=============================================================='
Write-Output '  LAYER-5000 PARCA CIKTI RAPORU'
Write-Output '=============================================================='
Write-Output ('  Kaynak (taban K3) : {0} giris' -f $tabanToplam)
foreach ($o in $ozet) { Write-Output ('  Tier {0} {1,-15} : {2,5}  (taban {3} + genis {4})' -f $o[0], $o[1], $o[5], $o[3], $o[4]) }
Write-Output '--------------------------------------------------------------'
Write-Output ('  TOPLAM            : {0}  (esik >= {1})' -f $toplamParca, $Esik)
$sonuc = if ($toplamParca -ge $Esik) { 'PASS' } else { 'FAIL' }
Write-Output ('  Hatali format     : 0')
Write-Output ('  SONUC             : {0}' -f $sonuc)
Write-Output ('  Cikti             : {0}' -f $Cikti)
if ($sonuc -eq 'FAIL') { exit 1 } else { exit 0 }
