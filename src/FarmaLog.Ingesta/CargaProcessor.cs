using Azure.Messaging.ServiceBus;
using System.Globalization;

namespace FarmaLog.Ingesta
{
    internal sealed class CargaProcessor(PlanillaReader reader, ServiceBusSender emisor)
    {
        // DEUDA DECLARADA: nivel ARCHIVO. Vienen de la sesión y del desplegable de
        // pantalla, que no existen todavía. Hoy son constantes.
        private const string CodigoLaboratorio = "23";
        private const string TipoOrdenVenta = "23F1";

        public async Task<ProcessingResult> ProcesarAsync(Stream planilla, CancellationToken ct)
        {
            var pedidos = PedidoGrouper.Group(reader.Read(planilla));

            // FASE 1 — validar TODO. Un solo error aborta el archivo completo.
            var errors = pedidos
                .SelectMany(FormatValidator.Validate)
                .Select(e => e.ToString())
                .ToList();

            if (errors.Count > 0)
                return new ProcessingResult(0, errors);

            // FASE 2 — recién ahora se publica. Cero mensajes si hubo un solo error.
            foreach (var pedido in pedidos)
                await emisor.SendMessageAsync(Construir(pedido), ct);

            return new ProcessingResult(pedidos.Count, []);
        }

        private static ServiceBusMessage Construir(PedidoGroup pedido)
        {
            var header = pedido.Rows[0];

            var message = new RegistrarSolicitudIngreso(
                CodigoLaboratorio: CodigoLaboratorio,
                NumeroDelivery: pedido.NumeroDelivery,
                CuentaCliente: CodigosD365Mapper.MapCuentaCliente(CodigoLaboratorio, header.CuentaCliente),
                DireccionDespacho: CodigosD365Mapper.MapDireccionDespacho(CodigoLaboratorio, header.DireccionDespacho),
                TipoOrdenVenta: TipoOrdenVenta,
                EsCenabast: header.EsCenabast.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase),
                DocumentoVentaCenabast: NullIfEmpty(header.DocumentoVentaCenabast),
                Observacion: NullIfEmpty(header.Observacion),
                FechaEntrega: string.IsNullOrWhiteSpace(header.FechaEntrega)
                    ? DateOnly.FromDateTime(DateTime.Today).AddDays(5)
                    : DateOnly.ParseExact(header.FechaEntrega, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                OrdenCompra: NullIfEmpty(header.OrdenCompra),
                Urgencia: header.Urgencia.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase),
                Lineas: [.. pedido.Rows.Select(f => new LineaDeMensaje(
                    f.Sku, int.Parse(f.Cantidad), f.EstadoInventario, NullIfEmpty(f.Lote)))]);

            return new ServiceBusMessage(BinaryData.FromObjectAsJson(message))
            {
                MessageId = pedido.NumeroDelivery
            };
        }

        private static string? NullIfEmpty(string v) => string.IsNullOrWhiteSpace(v) ? null : v;
    }
}
