using Microsoft.EntityFrameworkCore;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Data.Repositories;

/// <summary>
/// AuditLog repository implementasyonu — Append-only prensibi ile çalışır.
/// EF Core ile SQLite WAL desteği.
/// </summary>
public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(VersaCoderDbContext context) : base(context)
    {
    }

    public override async Task<AuditLog> AddAsync(AuditLog entity, CancellationToken cancellationToken = default)
    {
        // Append-only: Mevcut logları asla değiştirme
        return await base.AddAsync(entity, cancellationToken);
    }

    public override Task UpdateAsync(AuditLog entity, CancellationToken cancellationToken = default)
    {
        // Append-only: Log güncellenemez
        throw new InvalidOperationException("Audit logs are append-only and cannot be modified.");
    }

    public override Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Append-only: Log silinemez
        throw new InvalidOperationException("Audit logs are append-only and cannot be deleted.");
    }

    public async Task<List<AuditLog>> GetByLevelAsync(AuditLogLevel level, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.Level == level)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetByAgentAsync(string agent, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.Agent == agent)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.SessionId == sessionId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.TaskId == taskId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.Action == action)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(l => l.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToLower();
        return await _dbSet
            .Where(l => l.Message.ToLower().Contains(term)
                || l.Action.ToLower().Contains(term)
                || l.Agent.ToLower().Contains(term))
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByLevelAsync(AuditLogLevel level, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(l => l.Level == level, cancellationToken);
    }

    public async Task<Dictionary<AuditLogLevel, int>> GetLevelCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .GroupBy(l => l.Level)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
    }

    public async Task<long?> GetTotalTokenUsageAsync(Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(l => l.TokenUsage.HasValue);
        if (sessionId.HasValue)
            query = query.Where(l => l.SessionId == sessionId.Value);

        return (await query.SumAsync(l => l.TokenUsage!.Value, cancellationToken));
    }

    public async Task<double?> GetAverageDurationAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.DurationMs.HasValue)
            .AverageAsync(l => l.DurationMs!.Value, cancellationToken);
    }
}
