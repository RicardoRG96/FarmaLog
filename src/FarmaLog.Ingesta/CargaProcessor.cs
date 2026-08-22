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
            var pedidos = PedidoGrouper.Agrupar(reader.Read(planilla));

            // FASE 1 — validar TODO. Un solo error aborta el archivo completo.
            var errores = pedidos
                .SelectMany(FormatValidator.Validar)
                .Select(e => e.ToString())
                .ToList();

            if (errores.Count > 0)
                return new ProcessingResult(0, errores);

            // FASE 2 — recién ahora se publica. Cero mensajes si hubo un solo error.
            foreach (var pedido in pedidos)
                await emisor.SendMessageAsync(Construir(pedido), ct);

            return new ProcessingResult(pedidos.Count, []);
        }

        private static ServiceBusMessage Construir(PedidoGroup pedido)
        {
            var cabecera = pedido.Filas[0];

            var mensaje = new RegistrarSolicitudIngreso(
                CodigoLaboratorio: CodigoLaboratorio,
                NumeroDelivery: pedido.NumeroDelivery,
                CuentaCliente: CodigosD365Mapper.MapCuentaCliente(CodigoLaboratorio, cabecera.CuentaCliente),
                DireccionDespacho: CodigosD365Mapper.MapDireccionDespacho(CodigoLaboratorio, cabecera.DireccionDespacho),
                TipoOrdenVenta: TipoOrdenVenta,
                EsCenabast: cabecera.EsCenabast.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase),
                DocumentoVentaCenabast: Nulo(cabecera.DocumentoVentaCenabast),
                Observacion: Nulo(cabecera.Observacion),
                FechaEntrega: string.IsNullOrWhiteSpace(cabecera.FechaEntrega)
                    ? DateOnly.FromDateTime(DateTime.Today).AddDays(5)
                    : DateOnly.ParseExact(cabecera.FechaEntrega, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                OrdenCompra: Nulo(cabecera.OrdenCompra),
                Urgencia: cabecera.Urgencia.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase),
                Lineas: [.. pedido.Filas.Select(f => new LineaDeMensaje(
                    f.Sku, int.Parse(f.Cantidad), f.EstadoInventario, Nulo(f.Lote)))]);

            return new ServiceBusMessage(BinaryData.FromObjectAsJson(mensaje))
            {
                MessageId = pedido.NumeroDelivery
            };
        }

        private static string? Nulo(string v) => string.IsNullOrWhiteSpace(v) ? null : v;
    }
}
