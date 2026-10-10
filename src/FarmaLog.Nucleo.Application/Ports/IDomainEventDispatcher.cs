using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Application.Ports
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default);
    }
}
