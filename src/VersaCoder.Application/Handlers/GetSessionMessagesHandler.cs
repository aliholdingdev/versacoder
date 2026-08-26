using MediatR;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Application.Common;
using VersaCoder.Application.Queries;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Handlers;

public class GetSessionMessagesHandler : IRequestHandler<GetSessionMessagesQuery, Result<List<MessageDto>>>
{
    private readonly IMessageRepository _messageRepository;

    public GetSessionMessagesHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result<List<MessageDto>>> Handle(GetSessionMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetBySessionIdPagedAsync(
            request.SessionId, request.Page, request.PageSize, cancellationToken);

        var dtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SessionId = m.SessionId,
            Role = m.Role,
            Content = m.Content,
            AgentName = m.AgentName,
            Timestamp = m.Timestamp,
            Metadata = m.Metadata
        }).ToList();

        return Result<List<MessageDto>>.Success(dtos);
    }
}
