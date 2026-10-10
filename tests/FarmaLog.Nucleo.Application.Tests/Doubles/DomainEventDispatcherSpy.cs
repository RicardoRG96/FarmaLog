using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Application.Tests.Doubles
{
    public sealed class DomainEventDispatcherSpy : IDomainEventDispatcher
    {
        private readonly List<IDomainEvent> _dispatched = new();
        
        private readonly RepositorioDeSolicitudesEnMemoria? _repositorio;
        
        public DomainEventDispatcherSpy(RepositorioDeSolicitudesEnMemoria? repositorio = null)
        {
            _repositorio = repositorio;
        }

        public IReadOnlyList<IDomainEvent> Dispatched => _dispatched;
        public int? SavedCountAtDispatch { get; private set; }

        public Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default)
        {
            SavedCountAtDispatch = _repositorio?.Guardadas.Count;
            _dispatched.AddRange(domainEvents);
            return Task.CompletedTask;
        }
    }
}
