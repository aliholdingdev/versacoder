using System.Text.Json;
using System.Text;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

namespace VersaCoder.Infrastructure.Reporting;

/// <summary>
/// PDF rapor dışa aktarıcı — PDFsharp kütüphanesi ile PDF formatında raporlar oluşturur.
/// Tablo formatında raporlar, başlık ve alt bilgi desteği.
/// </summary>
public class PdfExporter
{
    /// <summary>
    /// JSON verisini PDF formatına dönüştürür.
    /// </summary>
    public byte[] ExportFromJson(string json, string title = "Report")
    {
        var document = new PdfDocument();
        document.Info.Title = title;
        document.Info.Creator = "VersaCoder Reporting System";
        document.Info.CreationDate = DateTime.UtcNow;

        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var textFormatter = new XTextFormatter(gfx);

        // Title
        var titleFont = new XFont("Arial", 20, XFontStyle.Bold);
        gfx.DrawString(title, titleFont, XBrushes.DarkBlue,
            new XRect(40, 40, page.Width - 80, 40), XStringFormats.TopLeft);

        // Date
        var dateFont = new XFont("Arial", 10, XFontStyle.Italic);
        gfx.DrawString($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
            dateFont, XBrushes.Gray,
            new XRect(40, 85, page.Width - 80, 20), XStringFormats.TopLeft);

        // Parse and render data
        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        if (data != null)
        {
            int y = 120;
            var headerFont = new XFont("Arial", 12, XFontStyle.Bold);
            var contentFont = new XFont("Arial", 10, XFontStyle.Regular);

            foreach (var kvp in data)
            {
                // Check page overflow
                if (y > page.Height - 50)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    textFormatter = new XTextFormatter(gfx);
                    y = 40;
                }

                // Key
                gfx.DrawString($"{kvp.Key}:", headerFont, XBrushes.Black,
                    new XRect(40, y, 200, 20), XStringFormats.TopLeft);

                // Value
                var value = kvp.Value?.ToString() ?? "";
                if (value.Length > 100)
                    value = value[..100] + "...";

                gfx.DrawString(value, contentFont, XBrushes.DarkGray,
                    new XRect(250, y, page.Width - 300, 20), XStringFormats.TopLeft);

                y += 25;
            }
        }

        // Footer
        var footerFont = new XFont("Arial", 8, XFontStyle.Italic);
        gfx.DrawString("VersaCoder Reporting System", footerFont, XBrushes.Gray,
            new XRect(40, page.Height - 40, page.Width - 80, 20), XStringFormats.BottomLeft);

        using var stream = new MemoryStream();
        document.Save(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Task listesini PDF formatına dönüştürür (tablo formatında).
    /// </summary>
    public byte[] ExportTaskTable(string title, List<Dictionary<string, object>> tasks, List<string> columns)
    {
        var document = new PdfDocument();
        document.Info.Title = title;
        document.Info.Creator = "VersaCoder Reporting System";

        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        // Title
        var titleFont = new XFont("Arial", 16, XFontStyle.Bold);
        gfx.DrawString(title, titleFont, XBrushes.DarkBlue,
            new XRect(40, 30, page.Width - 80, 30), XStringFormats.TopLeft);

        int startY = 70;
        int rowHeight = 20;
        double[] columnWidths = CalculateColumnWidths(columns, page.Width - 80);

        // Header row
        DrawTableRow(gfx, columns.Select(c => c.ToUpper()).ToList(),
            40, startY, columnWidths, rowHeight,
            new XFont("Arial", 9, XFontStyle.Bold),
            XBrushes.White, XBrushes.SteelBlue);

        // Data rows
        for (int row = 0; row < tasks.Count; row++)
        {
            int y = startY + (row + 1) * rowHeight;

            // Check page overflow
            if (y > page.Height - 50)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = 40;
                startY = 40 - rowHeight;

                DrawTableRow(gfx, columns.Select(c => c.ToUpper()).ToList(),
                    40, startY + rowHeight, columnWidths, rowHeight,
                    new XFont("Arial", 9, XFontStyle.Bold),
                    XBrushes.White, XBrushes.SteelBlue);
            }

            var values = columns.Select(col =>
            {
                var value = tasks[row].GetValueOrDefault(col);
                var str = value?.ToString() ?? "";
                return str.Length > 30 ? str[..30] + "..." : str;
            }).ToList();

            var bgColor = row % 2 == 0 ? XBrushes.White : XBrushes.LightGray;
            DrawTableRow(gfx, values, 40, y, columnWidths, rowHeight,
                new XFont("Arial", 9, XFontStyle.Regular),
                XBrushes.Black, bgColor);
        }

        using var stream = new MemoryStream();
        document.Save(stream);
        return stream.ToArray();
    }

    private static double[] CalculateColumnWidths(List<string> columns, double totalWidth)
    {
        var widths = new double[columns.Count];
        var equalWidth = totalWidth / columns.Count;

        for (int i = 0; i < columns.Count; i++)
            widths[i] = equalWidth;

        return widths;
    }

    private static void DrawTableRow(XGraphics gfx, List<string> values,
        double x, double y, double[] columnWidths, int height,
        XFont font, XBrush textColor, XBrush backgroundColor)
    {
        // Background
        double totalWidth = columnWidths.Sum();
        gfx.DrawRectangle(backgroundColor, x, y, totalWidth, height);

        // Border
        gfx.DrawRectangle(XPens.LightGray, x, y, totalWidth, height);

        // Text
        double currentX = x;
        for (int i = 0; i < values.Count && i < columnWidths.Length; i++)
        {
            var rect = new XRect(currentX + 5, y + 2, columnWidths[i] - 10, height - 4);
            gfx.DrawString(values[i], font, textColor, rect, XStringFormats.TopLeft);
            currentX += columnWidths[i];
        }
    }
}
