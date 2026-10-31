using FarmaLog.Replicacion.Contracts;
using FarmaLog.Replicacion.D365;

namespace FarmaLog.Replicacion
{
    public sealed class ReplicarPedidoMessageHandler(ID365Client client)
    {
        public async Task<RespuestaD365> HandleAsync(ReplicarPedidoEnD365Message message, CancellationToken cancellationToken)
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

            return await client.CrearPedidoVentaAsync(pedido, cancellationToken);
        }
    }
}
