using FarmaLog.Nucleo.Application.Common;
using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Solicitudes.Events;

namespace FarmaLog.Nucleo.Application.UseCases.SolicitarReplicacionEnD365
{
    public sealed class SolicitarReplicacionEnD365Handler : IDomainEventHandler<SolicitudDeIngresoRegistrada>
    {
        private const string Destino = "replicar-pedido-d365";
        private readonly IOutbox _outbox;

        public SolicitarReplicacionEnD365Handler(IOutbox outbox)
        {
            _outbox = outbox;
        }

        public Task HandleAsync(SolicitudDeIngresoRegistrada domainEvent, CancellationToken ct)
        {
            OutboxMessage message = new(
                Destination: Destino, MessageId: domainEvent.SolicitudId.ToString(), Payload: new object());

            _outbox.Add(message);

            return Task.CompletedTask;
        }
    }
}
