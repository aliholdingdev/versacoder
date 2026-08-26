using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Commands;

public class SendPromptCommand : IRequest<Result<MessageDto>>
{
    public Guid SessionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AgentName { get; set; }
}
