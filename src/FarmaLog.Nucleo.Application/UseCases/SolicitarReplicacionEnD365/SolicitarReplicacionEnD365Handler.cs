using FarmaLog.Nucleo.Application.Common;
using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Solicitudes;
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
            ReplicarPedidoEnD365Message replicarPedidoMessage = Map(domainEvent);

            OutboxMessage outboxMessage = new(
                Destination: Destino, MessageId: domainEvent.SolicitudId.ToString(), Payload: replicarPedidoMessage);

            _outbox.Add(outboxMessage);

            return Task.CompletedTask;
        }

        private static ReplicarPedidoEnD365Message Map(SolicitudDeIngresoRegistrada domainEvent) =>
            new(
                CodigoLaboratorio: domainEvent.CodigoLaboratorio.Codigo,
                CuentaCliente: domainEvent.CuentaCliente.Cuenta,
                DireccionDespacho: domainEvent.DireccionDespacho.Direccion,
                TipoOrdenVenta: domainEvent.TipoOrdenVenta.Tipo,
                EsCenabast: domainEvent.EsCenabast,
                DocumentoVentaCenabast: domainEvent.DocumentoVentaCenabast?.Documento,
                Observacion: domainEvent.Observacion,
                FechaEntregaSolicitada: domainEvent.FechaEntregaSolicitada,
                NumeroDelivery: domainEvent.NumeroDelivery.Numero,
                OrdenCompra: domainEvent.OrdenCompra,
                Urgencia: domainEvent.Urgencia,
                Lineas: [.. domainEvent.Lineas.Select(l => 
                    new LineaDePedidoMessage(
                        Sku: l.Sku, 
                        Cantidad: l.Cantidad, 
                        EstadoInventario: l.EstadoInventario, 
                        Lote: l.Lote))]);
    }
}
