using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Abstractions.Services;

/// <summary>
/// Log servisi arayüzü — AI tarafından yazılır, insanlar tarafından okunur.
/// Append-only prensibi ile çalışır. Full metadata desteği:
/// session ID, task ID, duration, token kullanımı, stack trace,
/// context snapshot, performans metrikleri.
/// .ai/ dizininde JSON formatında saklanır.
/// </summary>
public interface ILogService
{
    // Write operations (AI writes)
    Task LogAsync(AuditLogLevel level, string agent, string action, string message,
        CancellationToken cancellationToken = default);
    Task LogWithContextAsync(AuditLogLevel level, string agent, string action, string message,
        Guid? sessionId = null, Guid? taskId = null, long? durationMs = null,
        int? tokenUsage = null, Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);
    Task LogErrorAsync(string agent, string action, string message,
        string? errorCode = null, string? stackTrace = null, string? innerException = null,
        Guid? sessionId = null, Guid? taskId = null, CancellationToken cancellationToken = default);
    Task LogPerformanceAsync(string agent, string action, string message,
        long durationMs, double? cpuTimeMs = null, long? memoryUsageBytes = null,
        CancellationToken cancellationToken = default);

    // Read operations (humans read)
    Task<List<AuditLog>> GetLogsAsync(int? limit = null, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetLogsByLevelAsync(AuditLogLevel level, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetLogsByAgentAsync(string agent, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetLogsBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetLogsByTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetLogsByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> SearchLogsAsync(string searchTerm, CancellationToken cancellationToken = default);

    // Statistics
    Task<int> GetLogCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<AuditLogLevel, int>> GetLogCountsByLevelAsync(CancellationToken cancellationToken = default);
    Task<long?> GetTotalTokenUsageAsync(Guid? sessionId = null, CancellationToken cancellationToken = default);
    Task<double?> GetAverageDurationAsync(CancellationToken cancellationToken = default);

    // Export
    Task<string> ExportToJsonAsync(CancellationToken cancellationToken = default);
    Task<string> ExportToCsvAsync(CancellationToken cancellationToken = default);
}
