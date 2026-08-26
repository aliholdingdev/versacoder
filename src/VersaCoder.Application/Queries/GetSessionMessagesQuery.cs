using MediatR;
using VersaCoder.Application.Common;
using VersaCoder.Application.DTOs;

namespace VersaCoder.Application.Queries;

public class GetSessionMessagesQuery : IRequest<Result<List<MessageDto>>>
{
    public Guid SessionId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
