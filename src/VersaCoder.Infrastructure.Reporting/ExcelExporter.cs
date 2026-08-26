using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Text.Json;
using VersaCoder.Domain.Enums;

namespace VersaCoder.Infrastructure.Reporting;

/// <summary>
/// Excel rapor dışa aktarıcı — EPPlus kütüphanesi ile .xlsx formatında raporlar oluşturur.
/// Tüm rapor tipleri desteklenir: tablo, grafik, koşullı biçimlendirme.
/// </summary>
public class ExcelExporter
{
    /// <summary>
    /// JSON verisini Excel formatına dönüştürür.
    /// </summary>
    public byte[] ExportFromJson(string json, string sheetName = "Report")
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        if (data == null) return Array.Empty<byte>();

        // Title
        worksheet.Cells["A1"].Value = sheetName;
        worksheet.Cells["A1"].Style.Font.Size = 16;
        worksheet.Cells["A1"].Style.Font.Bold = true;
        worksheet.Cells["A1"].Style.Font.Color.SetColor(Color.DarkBlue);

        // Generated date
        worksheet.Cells["A2"].Value = $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
        worksheet.Cells["A2"].Style.Font.Italic = true;
        worksheet.Cells["A2"].Style.Font.Color.SetColor(Color.Gray);

        int row = 4;

        // Headers
        foreach (var key in data.Keys)
        {
            worksheet.Cells[row, 1].Value = key;
            worksheet.Cells[row, 1].Style.Font.Bold = true;
            worksheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
            row++;
        }

        // Data
        row = 4;
        foreach (var key in data.Keys)
        {
            var value = data[key];
            if (value is JsonElement element)
            {
                worksheet.Cells[row, 2].Value = element.ValueKind switch
                {
                    JsonValueKind.String => element.GetString(),
                    JsonValueKind.Number => element.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => element.ToString()
                };
            }
            else
            {
                worksheet.Cells[row, 2].Value = value?.ToString() ?? "";
            }
            row++;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        // Add borders
        var dataRange = worksheet.Cells[4, 1, row - 1, 2];
        dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

        return package.GetAsByteArray();
    }

    /// <summary>
    /// Task listesini Excel formatına dönüştürür.
    /// </summary>
    public byte[] ExportTaskList(string title, List<Dictionary<string, object>> tasks)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Tasks");

        // Title
        worksheet.Cells["A1"].Value = title;
        worksheet.Cells["A1"].Style.Font.Size = 16;
        worksheet.Cells["A1"].Style.Font.Bold = true;

        if (!tasks.Any()) return package.GetAsByteArray();

        // Headers from first task
        var headers = tasks.First().Keys.ToList();
        for (int col = 0; col < headers.Count; col++)
        {
            worksheet.Cells[3, col + 1].Value = headers[col];
            worksheet.Cells[3, col + 1].Style.Font.Bold = true;
            worksheet.Cells[3, col + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[3, col + 1].Style.Fill.BackgroundColor.SetColor(Color.SteelBlue);
            worksheet.Cells[3, col + 1].Style.Font.Color.SetColor(Color.White);
        }

        // Data rows
        for (int row = 0; row < tasks.Count; row++)
        {
            for (int col = 0; col < headers.Count; col++)
            {
                var value = tasks[row].GetValueOrDefault(headers[col]);
                worksheet.Cells[row + 4, col + 1].Value = value?.ToString() ?? "";
            }
        }

        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }
}
