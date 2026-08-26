using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Abstractions.Repositories;

/// <summary>
/// AuditLog repository arayüzü — IRepository&lt;AuditLog&gt; üzerine inşa edilmiş
/// log'a özgü sorguları tanımlar. Append-only prensibi ile çalışır.
/// </summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<List<AuditLog>> GetByLevelAsync(AuditLogLevel level, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByAgentAsync(string agent, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<int> GetCountByLevelAsync(AuditLogLevel level, CancellationToken cancellationToken = default);
    Task<Dictionary<AuditLogLevel, int>> GetLevelCountsAsync(CancellationToken cancellationToken = default);
    Task<long?> GetTotalTokenUsageAsync(Guid? sessionId = null, CancellationToken cancellationToken = default);
    Task<double?> GetAverageDurationAsync(CancellationToken cancellationToken = default);
}
