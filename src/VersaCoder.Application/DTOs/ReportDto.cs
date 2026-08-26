using VersaCoder.Domain.Enums;

namespace VersaCoder.Application.DTOs;

/// <summary>
/// Rapor DTO'su — Rapor sonuçları ve metadata için.
/// </summary>
public record ReportResultDto
{
    public ReportType ReportType { get; init; }
    public ReportFormat Format { get; init; }
    public DateTime GeneratedAt { get; init; }
    public byte[] Data { get; init; } = Array.Empty<byte>();
    public string? FileName { get; init; }
    public string? ContentType { get; init; }
    public int RecordCount { get; init; }
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Session özet rapor DTO'su.
/// </summary>
public record SessionSummaryReportDto
{
    public Guid SessionId { get; init; }
    public string SessionName { get; init; } = string.Empty;
    public DateTime SessionStart { get; init; }
    public DateTime? SessionEnd { get; init; }
    public int TotalTasks { get; init; }
    public int CompletedTasks { get; init; }
    public int InProgressTasks { get; init; }
    public int FailedTasks { get; init; }
    public int CancelledTasks { get; init; }
    public double CompletionRate { get; init; }
    public List<TaskSummaryItemDto> Tasks { get; init; } = new();
}

/// <summary>
/// Tamamlanma oranı rapor DTO'su.
/// </summary>
public record CompletionRateReportDto
{
    public int TotalTasks { get; init; }
    public int CompletedTasks { get; init; }
    public int InProgressTasks { get; init; }
    public int OnHoldTasks { get; init; }
    public int FailedTasks { get; init; }
    public int CancelledTasks { get; init; }
    public int ReviewTasks { get; init; }
    public int NewTasks { get; init; }
    public double OverallCompletionRate { get; init; }
    public Dictionary<Priority, double> CompletionByPriority { get; init; } = new();
    public DateTime ReportStartDate { get; init; }
    public DateTime ReportEndDate { get; init; }
}

/// <summary>
/// Engellenmiş task rapor DTO'su.
/// </summary>
public record BlockedTasksReportDto
{
    public int TotalBlocked { get; init; }
    public List<BlockedTaskItemDto> BlockedTasks { get; init; } = new();
}

/// <summary>
/// Öncelik dağılımı rapor DTO'su.
/// </summary>
public record PriorityDistributionReportDto
{
    public Dictionary<Priority, int> Counts { get; init; } = new();
    public Dictionary<Priority, double> Percentages { get; init; } = new();
    public int Total { get; init; }
}

/// <summary>
/// Etiket gruplama rapor DTO'su.
/// </summary>
public record TagGroupingReportDto
{
    public Dictionary<string, int> TagCounts { get; init; } = new();
    public Dictionary<string, List<TaskSummaryItemDto>> TasksByTag { get; init; } = new();
    public int UntaggedCount { get; init; }
}

/// <summary>
/// Zaman analizi rapor DTO'su.
/// </summary>
public record TimeSpentAnalysisReportDto
{
    public decimal TotalEstimatedHours { get; init; }
    public decimal TotalActualHours { get; init; }
    public double VariancePercentage { get; init; }
    public List<TaskTimeItemDto> TaskTimes { get; init; } = new();
}

/// <summary>
/// Milestone takip rapor DTO'su.
/// </summary>
public record MilestoneTrackingReportDto
{
    public int TotalMilestones { get; init; }
    public int CompletedMilestones { get; init; }
    public List<TaskSummaryItemDto> Milestones { get; init; } = new();
}

/// <summary>
/// Risk değerlendirme rapor DTO'su.
/// </summary>
public record RiskAssessmentReportDto
{
    public int HighRiskCount { get; init; }
    public int MediumRiskCount { get; init; }
    public int LowRiskCount { get; init; }
    public List<RiskItemDto> HighRiskTasks { get; init; } = new();
    public List<RiskItemDto> MediumRiskTasks { get; init; } = new();
}

// Supporting DTOs

public record TaskSummaryItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public TaskItemStatus Status { get; init; }
    public Priority Priority { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
}

public record BlockedTaskItemDto
{
    public Guid TaskId { get; init; }
    public string TaskTitle { get; init; } = string.Empty;
    public Guid DependsOnTaskId { get; init; }
    public string DependsOnTaskTitle { get; init; } = string.Empty;
    public DependencyType DependencyType { get; init; }
    public TaskItemStatus DependsOnStatus { get; init; }
}

public record TaskTimeItemDto
{
    public Guid TaskId { get; init; }
    public string TaskTitle { get; init; } = string.Empty;
    public decimal? EstimatedHours { get; init; }
    public decimal? ActualHours { get; init; }
    public DurationType DurationType { get; init; }
    public double VariancePercentage { get; init; }
}

public record RiskItemDto
{
    public Guid TaskId { get; init; }
    public string TaskTitle { get; init; } = string.Empty;
    public string RiskReason { get; init; } = string.Empty;
    public Priority Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public bool IsOverdue { get; init; }
    public int DependencyCount { get; init; }
}
