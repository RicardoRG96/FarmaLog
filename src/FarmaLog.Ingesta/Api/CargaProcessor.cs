using Azure.Messaging.ServiceBus;
using FarmaLog.Ingesta.Planilla;
using FarmaLog.Ingesta.Publishing;
using FarmaLog.Ingesta.Publishing.Contracts;
using FarmaLog.Ingesta.Validation;
using System.Globalization;

namespace FarmaLog.Ingesta.Api
{
    internal sealed class CargaProcessor(PlanillaReader reader, ServiceBusSender sender)
    {
        public async Task<ProcessingResult> ProcesarAsync(
            Stream planilla, string codigoLaboratorio, string tipoOrdenVenta, CancellationToken ct)
        {
            IReadOnlyList<PlanillaRow> rows = reader.Read(planilla);

            // ANTES de agrupar: sin Delivery no hay pedido al cual atribuir el
            // error. Único caso reportado por número de fila y no por pedido.
            List<string> orphanErrors = rows
                .Where(r => string.IsNullOrWhiteSpace(r.NumeroDelivery))
                .Select(r => $"Falta el Número Pedido del Laboratorio en la fila {r.RowNumber}. " +
                             "Complete la columna A o elimine la fila.")
                .ToList();

            IReadOnlyList<PedidoGroup> pedidos = PedidoGrouper.Group(
                [.. rows.Where(r => !string.IsNullOrWhiteSpace(r.NumeroDelivery))]);

            // FASE 1 — validar TODO. Un solo error aborta el archivo completo.
            List<string> errors = orphanErrors
                .Concat(pedidos.SelectMany(FormatValidator.Validate).Select(e => e.ToString()))
                .ToList();

            if (errors.Count > 0)
                return new ProcessingResult(0, errors);

            // FASE 2 — recién ahora se publica. Cero mensajes si hubo un solo error.
            foreach (var pedido in pedidos)
                await sender.SendMessageAsync(Build(pedido, codigoLaboratorio, tipoOrdenVenta), ct);

            return new ProcessingResult(pedidos.Count, []);
        }

        private static ServiceBusMessage Build(
            PedidoGroup pedido, string codigoLaboratorio, string tipoOrdenVenta)
        {
            // Tomar la primera fila es pérdida silenciosa si las cabeceras del
            // grupo discrepan. DEUDA: falta la validación de coherencia de
            // cabecera. Está aislada en esta línea a propósito.
            PlanillaRow header = pedido.Rows[0];

            // ParseExact e int.Parse sin Try son deliberados: la fase 1 ya
            // garantizó que son parseables. Si lanzan, es un bug propio.
            RegistrarSolicitudIngreso message = new(
                CodigoLaboratorio: codigoLaboratorio,
                NumeroDelivery: pedido.NumeroDelivery,
                CuentaCliente: CodigosD365Mapper.MapCuentaCliente(codigoLaboratorio, header.CuentaCliente),
                DireccionDespacho: CodigosD365Mapper.MapDireccionDespacho(codigoLaboratorio, header.DireccionDespacho),
                TipoOrdenVenta: tipoOrdenVenta,
                EsCenabast: IsSi(header.EsCenabast),
                DocumentoVentaCenabast: NullIfEmpty(header.DocumentoVentaCenabast),
                Observacion: NullIfEmpty(header.Observacion),
                FechaEntrega: string.IsNullOrWhiteSpace(header.FechaEntrega)
                    ? DateOnly.FromDateTime(DateTime.Today).AddDays(5)
                    : DateOnly.ParseExact(header.FechaEntrega, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                OrdenCompra: NullIfEmpty(header.OrdenCompra),
                Urgencia: IsSi(header.Urgencia),
                Lineas: [.. pedido.Rows.Select(r => new LineaDeMensaje(
                    r.Sku, int.Parse(r.Cantidad), r.EstadoInventario, NullIfEmpty(r.Lote)))]);

            return new ServiceBusMessage(BinaryData.FromObjectAsJson(message))
            {
                MessageId = pedido.NumeroDelivery
            };
        }

        private static bool IsSi(string value) =>
            value.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase);

        private static string? NullIfEmpty(string v) => string.IsNullOrWhiteSpace(v) ? null : v;
    }
}