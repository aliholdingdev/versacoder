namespace VersaCoder.Infrastructure.Config.Settings;

/// <summary>
/// UI ayarları.
/// </summary>
public class UiSettings
{
    /// <summary>Tema (Dark, Light, Auto).</summary>
    public string Theme { get; set; } = "Dark";

    /// <summary>Font boyutu.</summary>
    public int FontSize { get; set; } = 14;

    /// <summary>Yazı tipi.</summary>
    public string FontFamily { get; set; } = "Cascadia Code";

    /// <summary>Otomatik kaydetme aktif mi?</summary>
    public bool AutoSave { get; set; } = true;

    /// <summary>Minibar aktif mi?</summary>
    public bool ShowMinibar { get; set; } = true;

    /// <summary>Sol panel genişliği (piksel).</summary>
    public int LeftPanelWidth { get; set; } = 300;

    /// <summary>Sağ panel genişliği (piksel).</summary>
    public int RightPanelWidth { get; set; } = 300;

    /// <summary>Terminal yüksekliği (piksel).</summary>
    public int TerminalHeight { get; set; } = 200;
}
