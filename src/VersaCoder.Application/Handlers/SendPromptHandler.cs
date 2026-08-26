using MediatR;
using VersaCoder.Abstractions.Repositories;
using VersaCoder.Abstractions.Services;
using VersaCoder.Application.Commands;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;
using VersaCoder.Domain.Entities;

namespace VersaCoder.Application.Handlers;

public class SendPromptHandler : IRequestHandler<SendPromptCommand, Result<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IAgentRunner _agentRunner;

    public SendPromptHandler(
        IMessageRepository messageRepository,
        ISessionRepository sessionRepository,
        IAgentRunner agentRunner)
    {
        _messageRepository = messageRepository;
        _sessionRepository = sessionRepository;
        _agentRunner = agentRunner;
    }

    public async Task<Result<MessageDto>> Handle(SendPromptCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session == null)
            return Result<MessageDto>.Failure($"Session with id {request.SessionId} not found");

        var userMessage = new Message(request.SessionId, "user", request.Content);
        await _messageRepository.AddAsync(userMessage, cancellationToken);

        var agentRequest = new AgentRequest
        {
            Prompt = request.Content,
            SessionId = request.SessionId,
            AgentName = request.AgentName
        };

        var agentResponse = await _agentRunner.RunAsync(agentRequest, cancellationToken);

        var assistantMessage = new Message(request.SessionId, "assistant", agentResponse.Content)
        {
            AgentName = agentResponse.AgentName
        };
        await _messageRepository.AddAsync(assistantMessage, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        var dto = new MessageDto
        {
            Id = assistantMessage.Id,
            SessionId = assistantMessage.SessionId,
            Role = assistantMessage.Role,
            Content = assistantMessage.Content,
            AgentName = assistantMessage.AgentName,
            Timestamp = assistantMessage.Timestamp
        };

        return Result<MessageDto>.Success(dto);
    }
}
