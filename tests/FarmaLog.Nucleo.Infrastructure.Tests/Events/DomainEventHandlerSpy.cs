using FarmaLog.Nucleo.Application.Common;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Events
{
    public sealed class DomainEventHandlerSpy : IDomainEventHandler<EventoDePrueba>
    {
        private readonly List<EventoDePrueba> _events = new();

        public IReadOnlyCollection<EventoDePrueba> Events => _events;

        public Task HandleAsync(EventoDePrueba domainEvent, CancellationToken ct)
        {
            _events.Add(domainEvent);
            return Task.CompletedTask;
        }
    }
}
