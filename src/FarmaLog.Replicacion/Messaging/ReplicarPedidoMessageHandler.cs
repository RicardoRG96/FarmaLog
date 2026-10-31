using FarmaLog.Replicacion.Contracts;
using FarmaLog.Replicacion.D365;
using FarmaLog.Replicacion.Publishing;
using System.Diagnostics;
using static FarmaLog.Replicacion.D365.RespuestaD365;

namespace FarmaLog.Replicacion.Messaging
{
    public sealed class ReplicarPedidoMessageHandler(
        ID365Client client, IResultadoReplicacionPublisher publisher)
    {
        public async Task<MessageDestination> HandleAsync(string messageId, ReplicarPedidoEnD365Message message, CancellationToken cancellationToken)
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

            RespuestaD365 response = await client.CrearPedidoVentaAsync(pedido, cancellationToken);

            ResultadoReplicacionMessage Result(string resultado, string? numeroPedido, IReadOnlyList<string> motivos) =>
                new(resultado, numeroPedido, message.CodigoLaboratorio, message.NumeroDelivery, motivos);

            ResultadoReplicacionMessage result = response switch
            {
                Aceptado aceptado => Result(ResultadoReplicacion.Aceptada, aceptado.NumeroPedido, []),
                YaExistia yaExistia => Result(ResultadoReplicacion.Aceptada, yaExistia.NumeroPedido, []),
                Rechazado rechazado => Result(ResultadoReplicacion.Rechazada, null, [.. rechazado.Motivos]),
                _ => throw new UnreachableException($"Respuesta de D365 no contemplada: {response.GetType().Name}")
            };

            await publisher.PublishAsync(messageId, result, cancellationToken);

            return MessageDestination.Complete;
        }
    }
}
