using System.Text.Json;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Domain.Entities;

/// <summary>
/// Yapılandırılmış log entity'si — AI tarafından yazılır, insanlar tarafından okunur.
/// Full metadata desteği: session ID, task ID, duration, token kullanımı, stack trace,
/// context snapshot, performans metrikleri.
/// .ai/ dizininde JSON formatında saklanır.
/// </summary>
public class AuditLog
{
    private const int MaxMessageLength = 5000;
    private const int MaxActionLength = 200;
    private const int MaxAgentLength = 50;

    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public AuditLogLevel Level { get; set; }
    public string Agent { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    // Context
    public Guid? SessionId { get; set; }
    public Guid? TaskId { get; set; }
    public string? TaskListId { get; set; }

    // Performance
    public long? DurationMs { get; set; }
    public int? TokenUsage { get; set; }
    public int? InputTokens { get; set; }
    public int? OutputTokens { get; set; }

    // Metadata (JSON serialized)
    public string MetadataJson { get; set; } = "{}";
    public string? StackTrace { get; set; }

    // Context snapshot
    public string? CurrentFile { get; set; }
    public int? LineNumber { get; set; }
    public string? MethodName { get; set; }
    public string? ClassName { get; set; }

    // Performance metrics
    public double? CpuTimeMs { get; set; }
    public long? MemoryUsageBytes { get; set; }

    // Error info
    public string? ErrorCode { get; set; }
    public string? InnerExceptionMessage { get; set; }

    protected AuditLog() { }

    public AuditLog(AuditLogLevel level, string agent, string action, string message)
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        Level = level;
        Agent = ValidateAndTruncate(agent, MaxAgentLength, nameof(agent));
        Action = ValidateAndTruncate(action, MaxActionLength, nameof(action));
        Message = ValidateAndTruncate(message, MaxMessageLength, nameof(message));
    }

    public void SetSession(Guid sessionId)
    {
        SessionId = sessionId;
    }

    public void SetTask(Guid taskId)
    {
        TaskId = taskId;
    }

    public void SetDuration(long durationMs)
    {
        DurationMs = durationMs;
    }

    public void SetTokenUsage(int totalTokens, int? inputTokens = null, int? outputTokens = null)
    {
        TokenUsage = totalTokens;
        InputTokens = inputTokens;
        OutputTokens = outputTokens;
    }

    public void SetMetadata(Dictionary<string, object> metadata)
    {
        MetadataJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions
        {
            WriteIndented = false
        });
    }

    public Dictionary<string, object> GetMetadata()
    {
        if (string.IsNullOrWhiteSpace(MetadataJson) || MetadataJson == "{}")
            return new Dictionary<string, object>();

        return JsonSerializer.Deserialize<Dictionary<string, object>>(MetadataJson)
               ?? new Dictionary<string, object>();
    }

    public void SetContext(string? currentFile = null, int? lineNumber = null,
        string? methodName = null, string? className = null)
    {
        CurrentFile = currentFile;
        LineNumber = lineNumber;
        MethodName = methodName;
        ClassName = className;
    }

    public void SetPerformanceMetrics(double? cpuTimeMs = null, long? memoryUsageBytes = null)
    {
        CpuTimeMs = cpuTimeMs;
        MemoryUsageBytes = memoryUsageBytes;
    }

    public void SetError(string? errorCode = null, string? innerExceptionMessage = null, string? stackTrace = null)
    {
        ErrorCode = errorCode;
        InnerExceptionMessage = innerExceptionMessage;
        StackTrace = stackTrace;
    }

    /// <summary>
    /// Log'un JSON formatında temsilini döndürür.
    /// .ai/log.json dosyasına yazılır.
    /// </summary>
    public string ToJson()
    {
        var obj = new Dictionary<string, object>
        {
            ["id"] = Id.ToString(),
            ["timestamp"] = Timestamp.ToString("o"),
            ["level"] = Level.ToString(),
            ["agent"] = Agent,
            ["action"] = Action,
            ["message"] = Message
        };

        if (SessionId.HasValue) obj["sessionId"] = SessionId.Value.ToString();
        if (TaskId.HasValue) obj["taskId"] = TaskId.Value.ToString();
        if (DurationMs.HasValue) obj["duration"] = DurationMs.Value;
        if (TokenUsage.HasValue) obj["tokenUsage"] = TokenUsage.Value;
        if (InputTokens.HasValue) obj["inputTokens"] = InputTokens.Value;
        if (OutputTokens.HasValue) obj["outputTokens"] = OutputTokens.Value;

        var metadata = GetMetadata();
        if (metadata.Count > 0) obj["metadata"] = metadata;

        if (!string.IsNullOrEmpty(StackTrace)) obj["stackTrace"] = StackTrace;

        if (!string.IsNullOrEmpty(CurrentFile))
        {
            obj["contextSnapshot"] = new Dictionary<string, object>
            {
                ["currentFile"] = CurrentFile,
                ["lineNumber"] = LineNumber ?? 0,
                ["methodName"] = MethodName ?? "",
                ["className"] = ClassName ?? ""
            };
        }

        if (CpuTimeMs.HasValue || MemoryUsageBytes.HasValue)
        {
            obj["performanceMetrics"] = new Dictionary<string, object>
            {
                ["cpuTime"] = CpuTimeMs ?? 0,
                ["memoryUsage"] = MemoryUsageBytes ?? 0
            };
        }

        if (!string.IsNullOrEmpty(ErrorCode))
        {
            obj["errorCode"] = ErrorCode;
            if (!string.IsNullOrEmpty(InnerExceptionMessage))
                obj["innerExceptionMessage"] = InnerExceptionMessage;
        }

        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string ValidateAndTruncate(string value, int maxLength, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null or empty.", paramName);

        return value.Length > maxLength ? value[..maxLength] : value;
    }
}
