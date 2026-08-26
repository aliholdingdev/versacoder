using VersaCoder.Domain.Events;

namespace VersaCoder.Domain.Interfaces;

public interface IDomainEventBus
{
    Task PublishAsync<T>(T domainEvent, CancellationToken cancellationToken = default) where T : DomainEvent;
}
