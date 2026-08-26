namespace VersaCoder.Domain.Enums;

/// <summary>
/// Rapor formatları — Dışa aktarım formatları.
/// EPPlus (Excel), PDFsharp (PDF), System.Text.Json (JSON), CSV desteği.
/// </summary>
public enum ReportFormat
{
    /// <summary>JSON formatı — API ve veri değişimi için</summary>
    JSON = 0,

    /// <summary>CSV formatı — Basit tablo verileri için</summary>
    CSV = 1,

    /// <summary>Excel formatı (.xlsx) — Detaylı analiz için</summary>
    EXCEL = 2,

    /// <summary>PDF formatı — Resmi raporlar için</summary>
    PDF = 3
}
