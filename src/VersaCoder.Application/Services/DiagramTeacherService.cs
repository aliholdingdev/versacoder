using VersaCoder.Abstractions.Services;

namespace VersaCoder.Application.Services;

public class DiagramTeacherService : IDiagramTeacher
{
    public Task<DiagramContext> TeachDiagramAsync(string diagramContent, string format, CancellationToken cancellationToken = default)
    {
        var context = new DiagramContext
        {
            Id = Guid.NewGuid().ToString(),
            Content = diagramContent,
            Format = format
        };

        return Task.FromResult(context);
    }

    public Task<string> ConvertToCodeAsync(DiagramContext context, string targetLanguage, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"// Generated from diagram: {context.Format}\n// Language: {targetLanguage}");
    }
}
