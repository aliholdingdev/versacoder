namespace VersaCoder.Domain.Enums;

/// <summary>
/// Task durum değerleri — Durum makinesi ile yönetilir.
/// İzin verilen geçişler: NEW→IN_PROGRESS, IN_PROGRESS→COMPLETED/ON_HOLD/FAILED, vb.
/// </summary>
public enum TaskItemStatus
{
    /// <summary>Yeni oluşturuldu, henüz başlanmadı</summary>
    NEW = 0,

    /// <summary>Çalışılıyor</summary>
    IN_PROGRESS = 1,

    /// <summary>Beklemede (dış bağımlılık veya manuel duraklatma)</summary>
    ON_HOLD = 2,

    /// <summary>İptal edildi</summary>
    CANCELLED = 3,

    /// <summary>Başarıyla tamamlandı</summary>
    COMPLETED = 4,

    /// <summary>Başarısız oldu — yeniden denenebilir</summary>
    FAILED = 5,

    /// <summary>İnsan incelemesinde — review sonrası devam edilebilir</summary>
    REVIEW = 6
}
