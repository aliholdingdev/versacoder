namespace VersaCoder.Application.DTOs;

public class ContextDto
{
    public string Source { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int TokenCount { get; set; }
    public int Priority { get; set; }
}
