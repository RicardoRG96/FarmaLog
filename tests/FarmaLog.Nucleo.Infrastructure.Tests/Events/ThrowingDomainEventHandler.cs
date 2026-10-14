using FarmaLog.Nucleo.Application.Common;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Events
{
    public sealed class ThrowingDomainEventHandler : IDomainEventHandler<EventoDePrueba>
    {
        public Task HandleAsync(EventoDePrueba domainEvent, CancellationToken ct)
        {
            throw new DomainExceptionDePrueba("Used for testing purposes");
        }
    }
}
