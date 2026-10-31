using FarmaLog.Replicacion.Contracts;
using FarmaLog.Replicacion.D365;
using FarmaLog.Replicacion.Publishing;

namespace FarmaLog.Replicacion
{
    public sealed class ReplicarPedidoMessageHandler(
        ID365Client client, IResultadoReplicacionPublisher publisher)
    {
        public async Task HandleAsync(string messageId, ReplicarPedidoEnD365Message message, CancellationToken cancellationToken)
        {
            PedidoVentaD365 pedido = new(
                CodigoLaboratorio: message.CodigoLaboratorio,
                CuentaCliente: message.CuentaCliente,
                DireccionDespacho: message.DireccionDespacho,
                TipoOrdenVenta: message.TipoOrdenVenta,
                EsCenabast: message.EsCenabast,
                DocumentoVentaCenabast: message.DocumentoVentaCenabast,
                Observacion: message.Observacion,
                FechaEntregaSolicitada: message.FechaEntregaSolicitada,
                NumeroDelivery: message.NumeroDelivery,
                OrdenCompra: message.OrdenCompra,
                Urgencia: message.Urgencia,
                Lineas: [.. message.Lineas.Select(l => new LineaPedidoVentaD365(
                    Sku: l.Sku, Cantidad: l.Cantidad, EstadoInventario: l.EstadoInventario, Lote: l.Lote))],
                Almacen: "FLC1");

            RespuestaD365.Aceptado response = (RespuestaD365.Aceptado)await client.CrearPedidoVentaAsync(pedido, cancellationToken);

            await publisher.PublishAsync(
                correlationId: messageId,
                resultado: new(
                    Resultado: "Aceptada",
                    NumeroPedidoD365: response.NumeroPedido,
                    CodigoLaboratorio: message.CodigoLaboratorio,
                    NumeroDelivery: message.NumeroDelivery,
                    MotivosDeRechazo: []),
                cancellationToken: cancellationToken);
        }
    }
}
