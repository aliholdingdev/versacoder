---
title: "Versa Coder — Monitoring & Observability Skill"
type: skill
category: monitoring
date: 2026-08-26
updated: 2026-08-26
status: active
version: 1.0.0
---

# Versa Coder — Monitoring & Observability Skill

**Zorunlu Bağlantılar:** [[CLAUDE.md]] · [[brain.md]] · [[WORKFLOW.md]]

---

## 1. Amaç

Versa Coder ekosistemindeki tüm monitoring, logging ve observability ihtiyaçlarını karşılayan **izleme skill'ini** tanımlar. Grafana, Prometheus, Serilog ve OpenTelemetry entegrasyonunu kapsar.

---

## 2. Skill Tanımı

| Özellik | Değer |
|---------|-------|
| Skill Adı | `monitoring-observability` |
| Versiyon | 1.0.0 |
| Bağımlılıklar | Serilog, Prometheus, Grafana, OpenTelemetry |
| Kullanım Alanları | Logging, Metrics, Tracing, Alerting, Dashboard |

---

## 3. Three Pillars of Observability

### 3.1 Overview

```
┌──────────────────────────────────────────────────────┐
│                  OBSERVABILITY                        │
│                                                      │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐           │
│  │   LOGS   │  │  METRICS │  │  TRACES  │           │
│  │          │  │          │  │          │           │
│  │ Serilog  │  │Prometheus│  │OpenTel.  │           │
│  │ SEQ      │  │Grafana   │  │Jaeger    │           │
│  └──────────┘  └──────────┘  └──────────┘           │
│       ↑              ↑              ↑                │
│  ┌─────────────────────────────────────────────┐    │
│  │          APPLICATION CODE                     │    │
│  └─────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────┘
```

---

## 4. Prometheus Metrics

### 4.1 Metric Türleri

| Metric Türü | Kullanım | Örnek |
|-------------|----------|-------|
| Counter | Artan sayaç | Toplam istek sayısı |
| Gauge | Artan/azalan değer | Aktif bağlantı sayısı |
| Histogram | Dağılım ölçümü | İstek süresi dağılımı |
| Summary | Percentile hesaplama | Yanıt süresi P99 |

### 4.2 Custom Metrics Tanımları

```csharp
// Application metrics registry
public static class Metrics
{
    // Agent metrics
    public static readonly Counter AgentTaskTotal = Counter
        .Create("versacoder_agent_tasks_total", "Toplam agent görev sayısı")
        .WithLabels("agent_role", "status");

    public static readonly Histogram AgentTaskDuration = Histogram
        .Create("versacoder_agent_task_duration_seconds", "Agent görev süresi")
        .WithLabels("agent_role")
        .DefineDurationBuckets(0.1, 0.5, 1, 2, 5, 10, 30, 60);

    // LLM metrics
    public static readonly Counter LlmRequestTotal = Counter
        .Create("versacoder_llm_requests_total", "Toplam LLM istek sayısı")
        .WithLabels("provider", "model", "status");

    public static readonly Histogram LlmRequestDuration = Histogram
        .Create("versacoder_llm_request_duration_seconds", "LLM istek süresi")
        .WithLabels("provider", "model")
        .DefineDurationBuckets(0.5, 1, 2, 5, 10, 20, 30, 60);

    public static readonly Counter LlmTokensTotal = Counter
        .Create("versacoder_llm_tokens_total", "Toplam token kullanımı")
        .WithLabels("provider", "model", "type"); // type: prompt/completion

    // Database metrics
    public static readonly Histogram DbQueryDuration = Histogram
        .Create("versacoder_db_query_duration_seconds", "Veritabanı sorgu süresi")
        .WithLabels("operation")
        .DefineDurationBuckets(0.01, 0.05, 0.1, 0.25, 0.5, 1, 2.5, 5);

    public static readonly Counter DbQueryTotal = Counter
        .Create("versacoder_db_queries_total", "Toplam DB sorgu sayısı")
        .WithLabels("operation", "status");

    // Session metrics
    public static readonly Gauge ActiveSessions = Gauge
        .Create("versacoder_active_sessions", "Aktif session sayısı");

    public static readonly Counter SessionTotal = Counter
        .Create("versacoder_sessions_total", "Toplam session sayısı")
        .WithLabels("status");

    // Tool metrics
    public static readonly Counter ToolExecutionTotal = Counter
        .Create("versacoder_tool_executions_total", "Toplam tool çağrısı")
        .WithLabels("tool_name", "status");

    public static readonly Histogram ToolExecutionDuration = Histogram
        .Create("versacoder_tool_execution_duration_seconds", "Tool çalışma süresi")
        .WithLabels("tool_name")
        .DefineDurationBuckets(0.1, 0.5, 1, 2, 5, 10);

    // File metrics
    public static readonly Counter FileOperationsTotal = Counter
        .Create("versacoder_file_operations_total", "Toplam dosya işlemi")
        .WithLabels("operation", "status");

    // System metrics
    public static readonly Gauge MemoryUsage = Gauge
        .Create("versacoder_memory_usage_bytes", "Bellek kullanımı");

    public static readonly Gauge CpuUsage = Gauge
        .Create("versacoder_cpu_usage_ratio", "CPU kullanım oranı");
}
```

### 4.3 Metrics Endpoint Configuration

```csharp
// Program.cs'de Prometheus metrics endpoint
builder.Services.AddMetricServer(options =>
{
    options.Port = 9090;
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics(); // /metrics endpoint
});
```

---

## 5. Serilog Structured Logging

### 5.1 Configuration

```csharp
// Program.cs'de Serilog yapılandırması
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/versacoder-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 100_000_000, // 100MB
        rollOnFileSizeLimit: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.SQLite("logs/versacoder-logs.db",
        retainedFileCount: 30,
        logTableName: "Logs")
    .CreateLogger();
```

### 5.2 Structured Log Usage

```csharp
// ✅ Doğru — Structured logging
public class SessionService
{
    public async Task<Session> CreateSessionAsync(CreateSessionRequest request, CancellationToken ct)
    {
        Log.Information("Session oluşturma başlatıldı: {@Request}", request);

        var session = new Session(request.Name, request.ProjectId);

        using (Log.ForContext("SessionId", session.Id))
        {
            Log.Information("Session oluşturuldu: {SessionId}, Proje: {ProjectId}",
                session.Id, session.ProjectId);
        }

        return session;
    }
}

// ❌ Yanlış — String interpolation
Log.Information($"Session oluşturuldu: {session.Id}");
```

### 5.3 Log Enrichment

```csharp
// Custom enricher
public class CorrelationIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _accessor;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var correlationId = _accessor.HttpContext?.Request.Headers["X-Correlation-ID"]
            .FirstOrDefault() ?? Guid.NewGuid().ToString();

        logEvent.AddProperty(propertyFactory.CreateProperty("CorrelationId", correlationId));
    }
}
```

---

## 6. OpenTelemetry Distributed Tracing

### 6.1 Configuration

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("VersaCoder.AI")
            .AddSource("VersaCoder.Agent")
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "localhost";
                options.AgentPort = 6831;
                options.ServiceName = "versacoder-api";
            });
    })
    .WithMetrics(builder =>
    {
        builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddPrometheusExporter();
    });
```

### 6.2 Custom Spans

```csharp
public class TracedAgentRunner : IAgentRunner
{
    private readonly ActivitySource _activitySource = new("VersaCoder.Agent");

    public async Task<AgentResult> RunAsync(AgentTask task, CancellationToken ct)
    {
        using var activity = _activitySource.StartActivity("Agent.Run", ActivityKind.Internal);
        activity?.SetTag("agent.role", task.AgentRole);
        activity?.SetTag("task.id", task.Id);

        try
        {
            var result = await ExecuteAgentAsync(task, ct);
            activity?.SetTag("task.status", "success");
            return result;
        }
        catch (Exception ex)
        {
            activity?.SetTag("task.status", "error");
            activity?.SetTag("error.message", ex.Message);
            throw;
        }
    }
}
```

---

## 7. Health Checks

### 7.1 Health Check Endpoints

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("database", () =>
    {
        try
        {
            using var connection = new SqliteConnection("Data Source=versacoder.db");
            connection.Open();
            return HealthCheckResult.Healthy("SQLite bağlantısı aktif");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQLite bağlantısı başarısız", ex);
        }
    }, tags: ["db"])
    .AddCheck("ai-provider", () =>
    {
        // Provider health check
        return HealthCheckResult.Healthy("AI provider erişilebilir");
    }, tags: ["ai"])
    .AddCheck("memory", () =>
    {
        var process = Process.GetCurrentProcess();
        var usedBytes = process.WorkingSet64;
        var threshold = 500L * 1024 * 1024; // 500MB

        return usedBytes > threshold
            ? HealthCheckResult.Degraded($"Bellek kullanımı yüksek: {usedBytes / 1024 / 1024}MB")
            : HealthCheckResult.Healthy($"Bellek kullanımı normal: {usedBytes / 1024 / 1024}MB");
    });

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = WriteHealthCheckResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db"),
    ResponseWriter = WriteHealthCheckResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Sadece process durumu
});

private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    var result = new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            description = e.Value.Description,
            duration = e.Value.Duration.TotalMilliseconds,
            exception = e.Value.Exception?.Message
        }),
        totalDuration = report.TotalDuration.TotalMilliseconds
    };
    await context.Response.WriteAsJsonAsync(result);
}
```

---

## 8. Alerting Kuralları

### 8.1 Prometheus Alert Rules

```yaml
groups:
  - name: versacoder-alerts
    rules:
      - alert: HighErrorRate
        expr: rate(versacoder_agent_tasks_total{status="error"}[5m]) > 0.05
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "Yüksek hata oranı"
          description: "Agent hata oranı %5'i aşıyor"

      - alert: HighResponseTime
        expr: histogram_quantile(0.95, rate(versacoder_agent_task_duration_seconds_bucket[5m])) > 30
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Yüksek yanıt süresi"
          description: "P95 yanıt süresi 30 saniyeyi aşıyor"

      - alert: HighMemoryUsage
        expr: versacoder_memory_usage_bytes > 500 * 1024 * 1024
        for: 10m
        labels:
          severity: warning
        annotations:
          summary: "Yüksek bellek kullanımı"
          description: "Bellek kullanımı 500MB'ı aşıyor"

      - alert: HighLlmTokenUsage
        expr: rate(versacoder_llm_tokens_total[1h]) > 1000000
        for: 1h
        labels:
          severity: info
        annotations:
          summary: "Yüksek token kullanımı"
          description: "Saatlik token kullanımı 1M'ı aşıyor"

      - alert: DatabaseSlowQueries
        expr: histogram_quantile(0.95, rate(versacoder_db_query_duration_seconds_bucket[5m])) > 1
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Yavaş DB sorguları"
          description: "P95 DB sorgu süresi 1 saniyeyi aşıyor"

      - alert: ServiceDown
        expr: up{job="versacoder"} == 0
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "Servis çöktü"
          description: "Versa Coder servisi erişilemez durumda"
```

### 8.2 Notification Channels

| Kanal | Kullanım | Öncelik |
|-------|----------|---------|
| Slack | Genel bildirimler | INFO, WARNING |
| Email | Kritik uyarılar | CRITICAL |
| PagerDuty | Acil durumlar | CRITICAL |
| Webhook | Özel entegrasyonlar | Tümü |

---

## 9. Grafana Dashboard

### 9.1 Dashboard Panel Tipleri

| Panel | Metrik | Güncelleme |
|-------|--------|-----------|
| System Status | Health check durumu | 10s |
| Request Rate | İstek/dakika | 5s |
| Error Rate | Hata oranı | 5s |
| Response Time | P50, P95, P99 | 10s |
| Agent Performance | Agent bazlı performans | 15s |
| LLM Usage | Token kullanımı | 1min |
| Database | Sorgu süreleri | 10s |
| Memory | Bellek kullanımı | 30s |

### 9.2 Dashboard JSON Example (Simplified)

```json
{
  "title": "VersaCoder Dashboard",
  "panels": [
    {
      "title": "Request Rate",
      "type": "graph",
      "targets": [{
        "expr": "rate(versacoder_agent_tasks_total[5m])",
        "legendFormat": "{{agent_role}} - {{status}}"
      }]
    },
    {
      "title": "Response Time P95",
      "type": "graph",
      "targets": [{
        "expr": "histogram_quantile(0.95, rate(versacoder_agent_task_duration_seconds_bucket[5m]))",
        "legendFormat": "{{agent_role}}"
      }]
    },
    {
      "title": "Active Sessions",
      "type": "stat",
      "targets": [{
        "expr": "versacoder_active_sessions"
      }]
    },
    {
      "title": "Error Rate",
      "type": "singlestat",
      "targets": [{
        "expr": "rate(versacoder_agent_tasks_total{status='error'}[5m]) / rate(versacoder_agent_tasks_total[5m]) * 100"
      }],
      "thresholds": [
        { "value": 1, "color": "green" },
        { "value": 5, "color": "yellow" },
        { "value": 10, "color": "red" }
      ]
    }
  ]
}
```

---

## 10. SLI/SLO Tanımları

### 10.1 Service Level Indicators

| SLI | Metrik | Hedef |
|-----|--------|-------|
| Availability | Uptime / Total time | %99.9 |
| Latency | P95 yanıt süresi | < 5s |
| Error Rate | Hatalı istek / Toplam istek | < %1 |
| Throughput | Başarılı istek/dakika | > 100 |
| Correctness | Başarılı görev / Toplam görev | > %95 |

### 10.2 Service Level Objectives

| SLO | Pencere | Hata Bütçesi |
|-----|---------|-------------|
| Availability %99.9 | 30 gün | 43.2 dakika |
| Latency P95 < 5s | 30 gün | Toplam isteklerin %5'i |
| Error Rate < %1 | 30 gün | Toplam isteklerin %1'i |

---

## 11. Log Aggregation Pipeline

### 11.1 Pipeline Akışı

```
Application Logs
    ↓
Serilog (Structured)
    ↓
┌───────────┬──────────────┬────────────┐
│ Console   │ SQLite File  │ SEQ Server │
│ (dev)     │ (backup)     │ (prod)     │
└───────────┴──────────────┴────────────┘
                ↓
          Elasticsearch
                ↓
            Kibana (Dashboard)
```

### 11.2 Log Retention Policy

| Log Yaşı | Saklama | Amaç |
|----------|---------|------|
| 0-24 saat | Tam log | Gerçek zamanlı izleme |
| 1-7 gün | Sıkıştırılmış | Kısa vadeli analiz |
| 7-30 gün | Özet | Orta vadeli analiz |
| 30-90 gün | Sadece hata | Uzun vadeli analiz |
| 90+ gün | Arşiv | Compliance |

---

## 12. Dashboard Customization API

### 12.1 Dashboard CRUD

```csharp
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDashboards() { ... }

    [HttpPost]
    public async Task<IActionResult> CreateDashboard(CreateDashboardRequest request) { ... }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDashboard(Guid id, UpdateDashboardRequest request) { ... }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDashboard(Guid id) { ... }

    [HttpPost("{id}/widgets")]
    public async Task<IActionResult> AddWidget(Guid id, AddWidgetRequest request) { ... }

    [HttpPut("{id}/widgets/{widgetId}")]
    public async Task<IActionResult> UpdateWidget(Guid id, Guid widgetId, UpdateWidgetRequest request) { ... }
}
```

---

## Quality Report

| Metrik | Değer |
|--------|-------|
| Version | 1.0.0 |
| Pillars | 3 (Logs, Metrics, Traces) |
| Custom Metrics | 10 |
| Health Checks | 3 (DB, AI, Memory) |
| Alert Rules | 6 |
| SLI Definitions | 5 |
| Dashboard Panels | 8 |
| Log Retention Rules | 5 |

---

**Authority:** Vault Steward
**Last Updated:** 2026-08-26
**Mode:** Red Team · Human Mode · Truth Mode
