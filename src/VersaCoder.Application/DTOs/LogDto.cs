using VersaCoder.Domain.Enums;

namespace VersaCoder.Application.DTOs;

/// <summary>
/// Log DTO'su — UI ve API arası veri transferi için.
/// </summary>
public record LogDto
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    public AuditLogLevel Level { get; init; }
    public string Agent { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;

    // Context
    public Guid? SessionId { get; init; }
    public Guid? TaskId { get; init; }

    // Performance
    public long? DurationMs { get; init; }
    public int? TokenUsage { get; init; }
    public int? InputTokens { get; init; }
    public int? OutputTokens { get; init; }

    // Metadata
    public Dictionary<string, object> Metadata { get; init; } = new();
    public string? StackTrace { get; init; }

    // Context snapshot
    public string? CurrentFile { get; init; }
    public int? LineNumber { get; init; }
    public string? MethodName { get; init; }
    public string? ClassName { get; init; }

    // Performance metrics
    public double? CpuTimeMs { get; init; }
    public long? MemoryUsageBytes { get; init; }

    // Error info
    public string? ErrorCode { get; init; }
    public string? InnerExceptionMessage { get; init; }
}

/// <summary>
/// Log istatistikleri DTO'su.
/// </summary>
public record LogStatisticsDto
{
    public int TotalLogs { get; init; }
    public Dictionary<AuditLogLevel, int> CountsByLevel { get; init; } = new();
    public long? TotalTokenUsage { get; init; }
    public double? AverageDurationMs { get; init; }
    public DateTime? EarliestLog { get; init; }
    public DateTime? LatestLog { get; init; }
}
