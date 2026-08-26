namespace VersaCoder.Domain.Enums;

/// <summary>
/// Log seviyeleri — Yapılandırılmış loglama için.
/// AI tarafından yazılır, insanlar tarafından okunur.
/// </summary>
public enum AuditLogLevel
{
    /// <summary>Genel bilgi mesajları</summary>
    INFO = 0,

    /// <summary>Uyarı mesajları — dikkat gerektirir</summary>
    WARN = 1,

    /// <summary>Hata mesajları — düzeltme gerektirir</summary>
    ERROR = 2,

    /// <summary>Kritik hatalar — derhal müdahale gerektirir</summary>
    CRITICAL = 3,

    /// <summary>Ayrıntılı hata ayıklama bilgileri</summary>
    DEBUG = 4,

    /// <summary>Detaylı izleme bilgileri</summary>
    TRACE = 5
}
