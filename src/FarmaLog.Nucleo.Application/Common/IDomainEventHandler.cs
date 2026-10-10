using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Application.Common
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken ct);
    }
}
