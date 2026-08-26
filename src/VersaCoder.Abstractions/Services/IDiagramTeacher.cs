namespace VersaCoder.Abstractions.Services;

public interface IDiagramTeacher
{
    Task<DiagramContext> TeachDiagramAsync(string diagramContent, string format, CancellationToken cancellationToken = default);
    Task<string> ConvertToCodeAsync(DiagramContext context, string targetLanguage, CancellationToken cancellationToken = default);
}

public class DiagramContext
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public List<DiagramNode> Nodes { get; set; } = new();
    public List<DiagramEdge> Edges { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class DiagramNode
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class DiagramEdge
{
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
