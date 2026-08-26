namespace VersaCoder.Domain.Enums;

/// <summary>
/// Task bağımlılık türleri — Gantt chart desteği için 4 çeşit bağımlılık.
/// </summary>
public enum DependencyType
{
    /// <summary>
    /// Bitiş-Başlangıç: Bağımlı task, bağımlı olduğu task bitmeden başlayamaz.
    /// En yaygın bağımlılık türü.
    /// </summary>
    FINISH_TO_START = 0,

    /// <summary>
    /// Başlangıç-Başlangıç: Her iki task da aynı anda başlayabilir.
    /// Paralel çalışmalar için kullanılır.
    /// </summary>
    START_TO_START = 1,

    /// <summary>
    /// Bitiş-Bitfinish: Her iki task da aynı anda bitmelidir.
    /// Senkronize teslimatlar için kullanılır.
    /// </summary>
    FINISH_TO_FINISH = 2,

    /// <summary>
    /// Başlangıç-Bitfinish: Bağımlı task, bağımlı olduğu task başlamadan bitemez.
    /// Nadir kullanılır.
    /// </summary>
    START_TO_FINISH = 3
}
