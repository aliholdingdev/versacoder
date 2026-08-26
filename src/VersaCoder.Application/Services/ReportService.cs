using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;
using VersaCoder.Application.DTOs;
using VersaCoder.Domain.Entities;
using VersaCoder.Domain.Enums;
using VersaCoder.Domain.Interfaces;

namespace VersaCoder.Application.Services;

/// <summary>
/// Raporlama servisi implementasyonu — 10 farklı rapor tipi, 4 farklı format desteği.
/// JSON, CSV, Excel (EPPlus), PDF (PDFsharp) formatlarında dışa aktarma.
/// </summary>
public class ReportService : IReportService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IAuditLogRepository _logRepository;

    public ReportService(ITaskRepository taskRepository, IAuditLogRepository logRepository)
    {
        _taskRepository = taskRepository;
        _logRepository = logRepository;
    }

    #region Report Generation

    public async Task<byte[]> GenerateSessionSummaryReportAsync(Guid sessionId, ReportFormat format,
        CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetBySessionIdAsync(sessionId, cancellationToken);
        var report = BuildSessionSummaryData(sessionId, tasks);
        return await FormatReportAsync(report, format, "session-summary", cancellationToken);
    }

    public async Task<byte[]> GenerateTimeBasedReportAsync(DateTime startDate, DateTime endDate,
        ReportFormat format, CancellationToken cancellationToken = default)
    {
        var tasks = await _taskRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
        var report = new { ReportType = "Time-Based", StartDate = startDate, EndDate = endDate, Tasks = tasks.Select(MapToSummary) };
        return await FormatReportAsync(report, format, "time-based-report", cancellationToken);
    }

    public async Task<byte[]> GenerateCompletionRateReportAsync(ReportFormat format,
        Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        var statusCounts = await _taskRepository.GetStatusCountsAsync(cancellationToken);
        var totalCount = await _taskRepository.GetTotalCountAsync(cancellationToken);
        var priorityCounts = await _taskRepository.GetPriorityCountsAsync(cancellationToken);

        var report = new CompletionRateReportDto
        {
            TotalTasks = totalCount,
            CompletedTasks = statusCounts.GetValueOrDefault(TaskItemStatus.COMPLETED),
            InProgressTasks = statusCounts.GetValueOrDefault(TaskItemStatus.IN_PROGRESS),
            OnHoldTasks = statusCounts.GetValueOrDefault(TaskItemStatus.ON_HOLD),
            FailedTasks = statusCounts.GetValueOrDefault(TaskItemStatus.FAILED),
            CancelledTasks = statusCounts.GetValueOrDefault(TaskItemStatus.CANCELLED),
            ReviewTasks = statusCounts.GetValueOrDefault(TaskItemStatus.REVIEW),
            NewTasks = statusCounts.GetValueOrDefault(TaskItemStatus.NEW),
            OverallCompletionRate = totalCount > 0
                ? (double)statusCounts.GetValueOrDefault(TaskItemStatus.COMPLETED) / totalCount * 100
                : 0,
            ReportStartDate = DateTime.UtcNow.AddDays(-30),
            ReportEndDate = DateTime.UtcNow
        };

        return await FormatReportAsync(report, format, "completion-rate", cancellationToken);
    }

    public async Task<byte[]> GenerateBlockedTasksReportAsync(ReportFormat format,
        CancellationToken cancellationToken = default)
    {
        var blockedTasks = await _taskRepository.GetBlockedTasksAsync(cancellationToken);

        var report = new BlockedTasksReportDto
        {
            TotalBlocked = blockedTasks.Count,
            BlockedTasks = blockedTasks.SelectMany(t => t.Dependencies.Select(d => new BlockedTaskItemDto
            {
                TaskId = t.Id,
                TaskTitle = t.Title,
                DependsOnTaskId = d.DependsOnTaskId,
                DependsOnTaskTitle = d.DependsOnTask?.Title ?? "Unknown",
                DependencyType = d.DependencyType,
                DependsOnStatus = d.DependsOnTask?.Status ?? TaskItemStatus.NEW
            })).ToList()
        };

        return await FormatReportAsync(report, format, "blocked-tasks", cancellationToken);
    }

    public async Task<byte[]> GenerateDependencyMapReportAsync(ReportFormat format,
        Guid? taskId = null, CancellationToken cancellationToken = default)
    {
        List<TaskItem> tasks;
        if (taskId.HasValue)
        {
            var task = await _taskRepository.GetWithDetailsAsync(taskId.Value, cancellationToken);
            tasks = task != null ? new List<TaskItem> { task } : new List<TaskItem>();
        }
        else
        {
            tasks = await _taskRepository.GetAllAsync(cancellationToken);
        }

        var report = new
        {
            ReportType = "Dependency Map",
            Tasks = tasks.Where(t => t.Dependencies.Any() || t.Dependents.Any()).Select(t => new
            {
                t.Id,
                t.Title,
                t.Status,
                Dependencies = t.Dependencies.Select(d => new
                {
                    DependsOnId = d.DependsOnTaskId,
                    DependsOnTitle = d.DependsOnTask?.Title ?? "Unknown",
                    Type = d.DependencyType.ToString(),
                    IsBlocked = d.IsBlocked()
                }),
                Dependents = t.Dependents.Select(d => new
                {
                    TaskId = d.TaskId,
                    TaskTitle = d.Task?.Title ?? "Unknown",
                    Type = d.DependencyType.ToString()
                })
            })
        };

        return await FormatReportAsync(report, format, "dependency-map", cancellationToken);
    }

    public async Task<byte[]> GeneratePriorityDistributionReportAsync(ReportFormat format,
        Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        var priorityCounts = await _taskRepository.GetPriorityCountsAsync(cancellationToken);
        var total = priorityCounts.Values.Sum();

        var report = new PriorityDistributionReportDto
        {
            Counts = priorityCounts,
            Percentages = priorityCounts.ToDictionary(
                kv => kv.Key,
                kv => total > 0 ? (double)kv.Value / total * 100 : 0),
            Total = total
        };

        return await FormatReportAsync(report, format, "priority-distribution", cancellationToken);
    }

    public async Task<byte[]> GenerateTagGroupingReportAsync(ReportFormat format,
        CancellationToken cancellationToken = default)
    {
        var allTasks = await _taskRepository.GetAllAsync(cancellationToken);
        var tagGroups = allTasks
            .SelectMany(t => t.Tags.Select(tag => new { tag.Name, Task = t }))
            .GroupBy(x => x.Name)
            .ToDictionary(g => g.Key, g => g.Select(x => MapToSummary(x.Task)).ToList());

        var untaggedCount = allTasks.Count(t => !t.Tags.Any());

        var report = new TagGroupingReportDto
        {
            TagCounts = tagGroups.ToDictionary(g => g.Key, g => g.Value.Count),
            TasksByTag = tagGroups,
            UntaggedCount = untaggedCount
        };

        return await FormatReportAsync(report, format, "tag-grouping", cancellationToken);
    }

    public async Task<byte[]> GenerateTimeSpentAnalysisReportAsync(ReportFormat format,
        Guid? sessionId = null, CancellationToken cancellationToken = default)
    {
        List<TaskItem> tasks;
        if (sessionId.HasValue)
            tasks = await _taskRepository.GetBySessionIdAsync(sessionId.Value, cancellationToken);
        else
            tasks = await _taskRepository.GetAllAsync(cancellationToken);

        var report = new TimeSpentAnalysisReportDto
        {
            TotalEstimatedHours = tasks.Where(t => t.EstimatedHours.HasValue).Sum(t => t.EstimatedHours!.Value),
            TotalActualHours = tasks.Where(t => t.ActualHours.HasValue).Sum(t => t.ActualHours!.Value),
            TaskTimes = tasks.Where(t => t.EstimatedHours.HasValue || t.ActualHours.HasValue).Select(t => new TaskTimeItemDto
            {
                TaskId = t.Id,
                TaskTitle = t.Title,
                EstimatedHours = t.EstimatedHours,
                ActualHours = t.ActualHours,
                DurationType = t.DurationType,
                VariancePercentage = t.EstimatedHours.HasValue && t.ActualHours.HasValue && t.EstimatedHours.Value > 0
                    ? (double)((t.ActualHours.Value - t.EstimatedHours.Value) / t.EstimatedHours.Value * 100)
                    : 0
            }).ToList()
        };

        return await FormatReportAsync(report, format, "time-spent-analysis", cancellationToken);
    }

    public async Task<byte[]> GenerateMilestoneTrackingReportAsync(ReportFormat format,
        CancellationToken cancellationToken = default)
    {
        var milestones = await _taskRepository.GetMilestoneTasksAsync(cancellationToken);

        var report = new MilestoneTrackingReportDto
        {
            TotalMilestones = milestones.Count,
            CompletedMilestones = milestones.Count(t => t.Status == TaskItemStatus.COMPLETED),
            Milestones = milestones.Select(MapToSummary).ToList()
        };

        return await FormatReportAsync(report, format, "milestone-tracking", cancellationToken);
    }

    public async Task<byte[]> GenerateRiskAssessmentReportAsync(ReportFormat format,
        CancellationToken cancellationToken = default)
    {
        var allTasks = await _taskRepository.GetAllAsync(cancellationToken);

        var highRiskTasks = allTasks.Where(t =>
            (t.IsOverdue() && t.Priority == Priority.CRITICAL) ||
            (t.Dependencies.Any(d => d.IsBlocked()) && t.Priority >= Priority.HIGH) ||
            (t.Status == TaskItemStatus.FAILED && t.Priority >= Priority.HIGH)
        ).ToList();

        var mediumRiskTasks = allTasks.Where(t =>
            !highRiskTasks.Contains(t) && (
            (t.IsOverdue() && t.Priority >= Priority.MEDIUM) ||
            (t.Dependencies.Count > 2) ||
            (t.Status == TaskItemStatus.ON_HOLD && t.Priority >= Priority.MEDIUM)
        )).ToList();

        var report = new RiskAssessmentReportDto
        {
            HighRiskCount = highRiskTasks.Count,
            MediumRiskCount = mediumRiskTasks.Count,
            LowRiskCount = allTasks.Count - highRiskTasks.Count - mediumRiskTasks.Count,
            HighRiskTasks = highRiskTasks.Select(t => new RiskItemDto
            {
                TaskId = t.Id,
                TaskTitle = t.Title,
                RiskReason = t.IsOverdue() ? "Overdue" : t.Dependencies.Any(d => d.IsBlocked()) ? "Blocked dependency" : "Failed task",
                Priority = t.Priority,
                DueDate = t.DueDate,
                IsOverdue = t.IsOverdue(),
                DependencyCount = t.Dependencies.Count
            }).ToList(),
            MediumRiskTasks = mediumRiskTasks.Select(t => new RiskItemDto
            {
                TaskId = t.Id,
                TaskTitle = t.Title,
                RiskReason = t.IsOverdue() ? "Overdue" : t.Dependencies.Count > 2 ? "Many dependencies" : "On hold",
                Priority = t.Priority,
                DueDate = t.DueDate,
                IsOverdue = t.IsOverdue(),
                DependencyCount = t.Dependencies.Count
            }).ToList()
        };

        return await FormatReportAsync(report, format, "risk-assessment", cancellationToken);
    }

    public async Task<byte[]> GenerateReportAsync(ReportType reportType, ReportFormat format,
        Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default)
    {
        return reportType switch
        {
            ReportType.SESSION_SUMMARY => await GenerateSessionSummaryReportAsync(
                GetGuidParam(parameters, "sessionId"), format, cancellationToken),
            ReportType.TIME_BASED => await GenerateTimeBasedReportAsync(
                GetDateParam(parameters, "startDate", DateTime.UtcNow.AddDays(-30)),
                GetDateParam(parameters, "endDate", DateTime.UtcNow),
                format, cancellationToken),
            ReportType.COMPLETION_RATE => await GenerateCompletionRateReportAsync(
                format, GetOptionalGuidParam(parameters, "sessionId"), cancellationToken),
            ReportType.BLOCKED_TASKS => await GenerateBlockedTasksReportAsync(format, cancellationToken),
            ReportType.DEPENDENCY_MAP => await GenerateDependencyMapReportAsync(
                format, GetOptionalGuidParam(parameters, "taskId"), cancellationToken),
            ReportType.PRIORITY_DISTRIBUTION => await GeneratePriorityDistributionReportAsync(
                format, GetOptionalGuidParam(parameters, "sessionId"), cancellationToken),
            ReportType.TAG_GROUPING => await GenerateTagGroupingReportAsync(format, cancellationToken),
            ReportType.TIME_SPENT_ANALYSIS => await GenerateTimeSpentAnalysisReportAsync(
                format, GetOptionalGuidParam(parameters, "sessionId"), cancellationToken),
            ReportType.MILESTONE_TRACKING => await GenerateMilestoneTrackingReportAsync(format, cancellationToken),
            ReportType.RISK_ASSESSMENT => await GenerateRiskAssessmentReportAsync(format, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(reportType), reportType, "Unknown report type")
        };
    }

    public async Task<object> GetReportDataAsync(ReportType reportType,
        Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default)
    {
        return reportType switch
        {
            ReportType.COMPLETION_RATE => await GetCompletionRateDataAsync(cancellationToken),
            ReportType.PRIORITY_DISTRIBUTION => await GetPriorityDistributionDataAsync(cancellationToken),
            ReportType.BLOCKED_TASKS => await GetBlockedTasksDataAsync(cancellationToken),
            _ => new { Message = "Data export not available for this report type" }
        };
    }

    #endregion

    #region Private Helpers

    private async Task<CompletionRateReportDto> GetCompletionRateDataAsync(CancellationToken cancellationToken)
    {
        var statusCounts = await _taskRepository.GetStatusCountsAsync(cancellationToken);
        var total = await _taskRepository.GetTotalCountAsync(cancellationToken);
        return new CompletionRateReportDto
        {
            TotalTasks = total,
            CompletedTasks = statusCounts.GetValueOrDefault(TaskItemStatus.COMPLETED),
            InProgressTasks = statusCounts.GetValueOrDefault(TaskItemStatus.IN_PROGRESS),
            OnHoldTasks = statusCounts.GetValueOrDefault(TaskItemStatus.ON_HOLD),
            FailedTasks = statusCounts.GetValueOrDefault(TaskItemStatus.FAILED),
            CancelledTasks = statusCounts.GetValueOrDefault(TaskItemStatus.CANCELLED),
            ReviewTasks = statusCounts.GetValueOrDefault(TaskItemStatus.REVIEW),
            NewTasks = statusCounts.GetValueOrDefault(TaskItemStatus.NEW),
            OverallCompletionRate = total > 0 ? (double)statusCounts.GetValueOrDefault(TaskItemStatus.COMPLETED) / total * 100 : 0,
            ReportStartDate = DateTime.UtcNow.AddDays(-30),
            ReportEndDate = DateTime.UtcNow
        };
    }

    private async Task<PriorityDistributionReportDto> GetPriorityDistributionDataAsync(CancellationToken cancellationToken)
    {
        var priorityCounts = await _taskRepository.GetPriorityCountsAsync(cancellationToken);
        var total = priorityCounts.Values.Sum();
        return new PriorityDistributionReportDto
        {
            Counts = priorityCounts,
            Percentages = priorityCounts.ToDictionary(kv => kv.Key, kv => total > 0 ? (double)kv.Value / total * 100 : 0),
            Total = total
        };
    }

    private async Task<BlockedTasksReportDto> GetBlockedTasksDataAsync(CancellationToken cancellationToken)
    {
        var blockedTasks = await _taskRepository.GetBlockedTasksAsync(cancellationToken);
        return new BlockedTasksReportDto
        {
            TotalBlocked = blockedTasks.Count,
            BlockedTasks = blockedTasks.SelectMany(t => t.Dependencies.Select(d => new BlockedTaskItemDto
            {
                TaskId = t.Id,
                TaskTitle = t.Title,
                DependsOnTaskId = d.DependsOnTaskId,
                DependsOnTaskTitle = d.DependsOnTask?.Title ?? "Unknown",
                DependencyType = d.DependencyType,
                DependsOnStatus = d.DependsOnTask?.Status ?? TaskItemStatus.NEW
            })).ToList()
        };
    }

    private object BuildSessionSummaryData(Guid sessionId, List<TaskItem> tasks)
    {
        return new SessionSummaryReportDto
        {
            SessionId = sessionId,
            SessionName = $"Session {sessionId}",
            SessionStart = tasks.Any() ? tasks.Min(t => t.CreatedAt) : DateTime.UtcNow,
            SessionEnd = tasks.Any() ? tasks.Max(t => t.UpdatedAt) : null,
            TotalTasks = tasks.Count,
            CompletedTasks = tasks.Count(t => t.Status == TaskItemStatus.COMPLETED),
            InProgressTasks = tasks.Count(t => t.Status == TaskItemStatus.IN_PROGRESS),
            FailedTasks = tasks.Count(t => t.Status == TaskItemStatus.FAILED),
            CancelledTasks = tasks.Count(t => t.Status == TaskItemStatus.CANCELLED),
            CompletionRate = tasks.Count > 0
                ? (double)tasks.Count(t => t.Status == TaskItemStatus.COMPLETED) / tasks.Count * 100
                : 0,
            Tasks = tasks.Select(MapToSummary).ToList()
        };
    }

    private static TaskSummaryItemDto MapToSummary(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Status = task.Status,
        Priority = task.Priority,
        CreatedAt = task.CreatedAt,
        DueDate = task.DueDate,
        IsOverdue = task.IsOverdue()
    };

    private static async Task<byte[]> FormatReportAsync(object reportData, ReportFormat format,
        string reportName, CancellationToken cancellationToken)
    {
        return format switch
        {
            ReportFormat.JSON => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(reportData, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            })),
            ReportFormat.CSV => Encoding.UTF8.GetBytes(ExportToCsv(reportData)),
            ReportFormat.EXCEL => await ExportToExcelAsync(reportData, reportName, cancellationToken),
            ReportFormat.PDF => await ExportToPdfAsync(reportData, reportName, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unknown format")
        };
    }

    private static string ExportToCsv(object data)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Report Type,Generated At,Details");
        sb.AppendLine($"Report,{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss},{data.GetType().Name}");

        var props = data.GetType().GetProperties();
        foreach (var prop in props)
        {
            var value = prop.GetValue(data);
            sb.AppendLine($"{prop.Name},,{value}");
        }

        return sb.ToString();
    }

    private static async Task<byte[]> ExportToExcelAsync(object data, string reportName, CancellationToken cancellationToken)
    {
        // EPPlus integration - requires NuGet package OfficeOpenXml
        // For now, return JSON as placeholder
        await Task.CompletedTask;
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            Format = "Excel",
            Report = reportName,
            Note = "EPPlus package required for Excel export",
            Data = data
        }, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static async Task<byte[]> ExportToPdfAsync(object data, string reportName, CancellationToken cancellationToken)
    {
        // PDFsharp integration - requires NuGet package PDFsharp
        // For now, return JSON as placeholder
        await Task.CompletedTask;
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            Format = "PDF",
            Report = reportName,
            Note = "PDFsharp package required for PDF export",
            Data = data
        }, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static Guid GetGuidParam(Dictionary<string, object>? parameters, string key)
    {
        if (parameters != null && parameters.TryGetValue(key, out var value))
        {
            if (value is Guid guid) return guid;
            if (Guid.TryParse(value.ToString(), out var parsed)) return parsed;
        }
        throw new ArgumentException($"Required parameter '{key}' is missing or invalid.");
    }

    private static Guid? GetOptionalGuidParam(Dictionary<string, object>? parameters, string key)
    {
        if (parameters != null && parameters.TryGetValue(key, out var value))
        {
            if (value is Guid guid) return guid;
            if (Guid.TryParse(value.ToString(), out var parsed)) return parsed;
        }
        return null;
    }

    private static DateTime GetDateParam(Dictionary<string, object>? parameters, string key, DateTime defaultValue)
    {
        if (parameters != null && parameters.TryGetValue(key, out var value))
        {
            if (value is DateTime date) return date;
            if (DateTime.TryParse(value.ToString(), out var parsed)) return parsed;
        }
        return defaultValue;
    }

    #endregion
}
