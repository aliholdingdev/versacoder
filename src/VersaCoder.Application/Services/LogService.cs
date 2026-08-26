using System.Text;
using System.Text.Json;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Application.Services;

/// <summary>
/// Log servisi implementasyonu — AI tarafından yazılır, insanlar tarafından okunur.
/// Append-only prensibi ile çalışır. Full metadata desteği sunar.
/// </summary>
public class LogService : ILogService
{
    private readonly IAuditLogRepository _logRepository;

    public LogService(IAuditLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    #region Write Operations (AI writes)

    public async Task LogAsync(AuditLogLevel level, string agent, string action, string message,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog(level, agent, action, message);
        await _logRepository.AddAsync(log, cancellationToken);
        await _logRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task LogWithContextAsync(AuditLogLevel level, string agent, string action, string message,
        Guid? sessionId = null, Guid? taskId = null, long? durationMs = null,
        int? tokenUsage = null, Dictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog(level, agent, action, message);

        if (sessionId.HasValue) log.SetSession(sessionId.Value);
        if (taskId.HasValue) log.SetTask(taskId.Value);
        if (durationMs.HasValue) log.SetDuration(durationMs.Value);
        if (tokenUsage.HasValue) log.SetTokenUsage(tokenUsage.Value);
        if (metadata != null) log.SetMetadata(metadata);

        await _logRepository.AddAsync(log, cancellationToken);
        await _logRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task LogErrorAsync(string agent, string action, string message,
        string? errorCode = null, string? stackTrace = null, string? innerException = null,
        Guid? sessionId = null, Guid? taskId = null, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog(AuditLogLevel.ERROR, agent, action, message);

        if (sessionId.HasValue) log.SetSession(sessionId.Value);
        if (taskId.HasValue) log.SetTask(taskId.Value);
        log.SetError(errorCode, innerException, stackTrace);

        await _logRepository.AddAsync(log, cancellationToken);
        await _logRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task LogPerformanceAsync(string agent, string action, string message,
        long durationMs, double? cpuTimeMs = null, long? memoryUsageBytes = null,
        CancellationToken cancellationToken = default)
    {
        var log = new AuditLog(AuditLogLevel.INFO, agent, action, message);
        log.SetDuration(durationMs);
        log.SetPerformanceMetrics(cpuTimeMs, memoryUsageBytes);

        await _logRepository.AddAsync(log, cancellationToken);
        await _logRepository.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Read Operations (Humans read)

    public async Task<List<AuditLog>> GetLogsAsync(int? limit = null, CancellationToken cancellationToken = default)
    {
        var logs = await _logRepository.GetAllAsync(cancellationToken);
        if (limit.HasValue)
            logs = logs.OrderByDescending(l => l.Timestamp).Take(limit.Value).ToList();
        return logs;
    }

    public async Task<List<AuditLog>> GetLogsByLevelAsync(AuditLogLevel level, CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetByLevelAsync(level, cancellationToken);
    }

    public async Task<List<AuditLog>> GetLogsByAgentAsync(string agent, CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetByAgentAsync(agent, cancellationToken);
    }

    public async Task<List<AuditLog>> GetLogsBySessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetBySessionIdAsync(sessionId, cancellationToken);
    }

    public async Task<List<AuditLog>> GetLogsByTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetByTaskIdAsync(taskId, cancellationToken);
    }

    public async Task<List<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
    }

    public async Task<List<AuditLog>> GetLogsByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetByActionAsync(action, cancellationToken);
    }

    public async Task<List<AuditLog>> SearchLogsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _logRepository.SearchAsync(searchTerm, cancellationToken);
    }

    #endregion

    #region Statistics

    public async Task<int> GetLogCountAsync(CancellationToken cancellationToken = default)
    {
        var all = await _logRepository.GetAllAsync(cancellationToken);
        return all.Count;
    }

    public async Task<Dictionary<AuditLogLevel, int>> GetLogCountsByLevelAsync(CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetLevelCountsAsync(cancellationToken);
    }

    public async Task<long?> GetTotalTokenUsageAsync(Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetTotalTokenUsageAsync(sessionId, cancellationToken);
    }

    public async Task<double?> GetAverageDurationAsync(CancellationToken cancellationToken = default)
    {
        return await _logRepository.GetAverageDurationAsync(cancellationToken);
    }

    #endregion

    #region Export

    public async Task<string> ExportToJsonAsync(CancellationToken cancellationToken = default)
    {
        var logs = await _logRepository.GetAllAsync(cancellationToken);
        var jsonObjects = logs.OrderByDescending(l => l.Timestamp).Select(l => l.ToJson());
        return $"[\n{string.Join(",\n", jsonObjects)}\n]";
    }

    public async Task<string> ExportToCsvAsync(CancellationToken cancellationToken = default)
    {
        var logs = await _logRepository.GetAllAsync(cancellationToken);
        var sb = new StringBuilder();

        // Header
        sb.AppendLine("Id,Timestamp,Level,Agent,Action,Message,SessionId,TaskId,DurationMs,TokenUsage");

        // Rows
        foreach (var log in logs.OrderByDescending(l => l.Timestamp))
        {
            sb.AppendLine(string.Join(",",
                EscapeCsv(log.Id.ToString()),
                EscapeCsv(log.Timestamp.ToString("o")),
                EscapeCsv(log.Level.ToString()),
                EscapeCsv(log.Agent),
                EscapeCsv(log.Action),
                EscapeCsv(log.Message),
                EscapeCsv(log.SessionId?.ToString() ?? ""),
                EscapeCsv(log.TaskId?.ToString() ?? ""),
                EscapeCsv(log.DurationMs?.ToString() ?? ""),
                EscapeCsv(log.TokenUsage?.ToString() ?? "")
            ));
        }

        return sb.ToString();
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }

    #endregion
}
