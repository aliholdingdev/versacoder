using MediatR;
using VersaCoder.Application.Common;

namespace VersaCoder.Application.Commands;

public class RecordLearningCommand : IRequest<Result<bool>>
{
    public string Category { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public string? Source { get; set; }
}
