using VersaCoder.Domain.Enums;

namespace VersaCoder.Abstractions.Services;

/// <summary>
/// Raporlama servisi arayüzü — 10 farklı rapor tipi, 4 farklı format desteği.
/// Session bazlı, zaman bazlı, özet, engellenmiş, bağımlılık haritası,
/// öncelik dağılımı, etiket gruplama, zaman analizi, milestone, risk değerlendirme.
/// JSON, CSV, Excel (EPPlus), PDF (PDFsharp) formatlarında dışa aktarma.
/// </summary>
public interface IReportService
{
    // Session-based reports
    Task<byte[]> GenerateSessionSummaryReportAsync(Guid sessionId, ReportFormat format,
        CancellationToken cancellationToken = default);

    // Time-based reports
    Task<byte[]> GenerateTimeBasedReportAsync(DateTime startDate, DateTime endDate,
        ReportFormat format, CancellationToken cancellationToken = default);

    // Completion rate
    Task<byte[]> GenerateCompletionRateReportAsync(ReportFormat format,
        Guid? sessionId = null, CancellationToken cancellationToken = default);

    // Blocked tasks
    Task<byte[]> GenerateBlockedTasksReportAsync(ReportFormat format,
        CancellationToken cancellationToken = default);

    // Dependency map
    Task<byte[]> GenerateDependencyMapReportAsync(ReportFormat format,
        Guid? taskId = null, CancellationToken cancellationToken = default);

    // Priority distribution
    Task<byte[]> GeneratePriorityDistributionReportAsync(ReportFormat format,
        Guid? sessionId = null, CancellationToken cancellationToken = default);

    // Tag grouping
    Task<byte[]> GenerateTagGroupingReportAsync(ReportFormat format,
        CancellationToken cancellationToken = default);

    // Time spent analysis
    Task<byte[]> GenerateTimeSpentAnalysisReportAsync(ReportFormat format,
        Guid? sessionId = null, CancellationToken cancellationToken = default);

    // Milestone tracking
    Task<byte[]> GenerateMilestoneTrackingReportAsync(ReportFormat format,
        CancellationToken cancellationToken = default);

    // Risk assessment
    Task<byte[]> GenerateRiskAssessmentReportAsync(ReportFormat format,
        CancellationToken cancellationToken = default);

    // Generic report generator
    Task<byte[]> GenerateReportAsync(ReportType reportType, ReportFormat format,
        Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default);

    // Report data (for custom rendering)
    Task<object> GetReportDataAsync(ReportType reportType,
        Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default);
}
