using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Application.Tests.Doubles
{
    public sealed class DomainEventDispatcherSpy : IDomainEventDispatcher
    {
        private readonly List<IDomainEvent> _dispatched = new();
        public IReadOnlyCollection<IDomainEvent> Dispatched => _dispatched;

        public Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default)
        {
            _dispatched.AddRange(domainEvents);
            return Task.CompletedTask;
        }
    }
}
