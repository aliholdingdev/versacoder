namespace VersaCoder.Domain.Enums;

/// <summary>
/// Rapor türleri — 10 farklı rapor tipi desteklenir.
/// JSON, CSV, Excel ve PDF formatlarında dışa aktarılabilir.
/// </summary>
public enum ReportType
{
    /// <summary>Belirli bir session'daki tüm task'lar</summary>
    SESSION_SUMMARY = 0,

    /// <summary>Tarih aralığındaki task'lar</summary>
    TIME_BASED = 1,

    /// <summary>Task tamamlanma istatistikleri</summary>
    COMPLETION_RATE = 2,

    /// <summary>Bağımlılık nedeniyle bekleyen task'lar</summary>
    BLOCKED_TASKS = 3,

    /// <summary>Task bağımlılık ilişkileri haritası</summary>
    DEPENDENCY_MAP = 4,

    /// <summary>Öncelik bazlı gruplama ve dağılım</summary>
    PRIORITY_DISTRIBUTION = 5,

    /// <summary>Etiket bazlı analiz ve gruplama</summary>
    TAG_GROUPING = 6,

    /// <summary>Tahmini vs gerçek süre analizi</summary>
    TIME_SPENT_ANALYSIS = 7,

    /// <summary>Kilometre taşları takibi</summary>
    MILESTONE_TRACKING = 8,

    /// <summary>Yüksek riskli task'lar ve değerlendirmesi</summary>
    RISK_ASSESSMENT = 9
}
