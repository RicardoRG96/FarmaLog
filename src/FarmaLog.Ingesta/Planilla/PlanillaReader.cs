using ClosedXML.Excel;

namespace FarmaLog.Ingesta.Planilla
{
    internal sealed class PlanillaReader
    {
        private const int FirstDataRow = 2;

        public IReadOnlyList<PlanillaRow> Read(Stream planilla)
        {
            using XLWorkbook book = new(planilla);
            IXLWorksheet sheet = book.Worksheet(1);
            List<PlanillaRow> rows = [];

            foreach (var row in sheet.RowsUsed())
            {
                if (row.RowNumber() < FirstDataRow) continue;

                PlanillaRow parsed = new(
                    row.RowNumber(),
                    Text(row, 1), Text(row, 2), Text(row, 3),
                    Text(row, 4), Text(row, 5), Text(row, 6),
                    Text(row, 7), Text(row, 8), Text(row, 9),
                    Text(row, 10), Text(row, 11), Text(row, 12),
                    Text(row, 13));

                // Solo se descarta la fila ENTERAMENTE vacía (trampa 4: la hoja
                // reporta 17 filas y solo la 2 trae datos). Con cualquier dato la
                // fila existe — y si le falta el Delivery, eso es un ERROR del
                // usuario, no una ausencia. El reader lee; no decide qué es válido.
                if (IsEmpty(parsed)) continue;

                rows.Add(parsed);
            }

            return rows;
        }

        private static bool IsEmpty(PlanillaRow row) =>
            string.IsNullOrWhiteSpace(row.NumeroDelivery)
            && string.IsNullOrWhiteSpace(row.CuentaCliente)
            && string.IsNullOrWhiteSpace(row.DocumentoVentaCenabast)
            && string.IsNullOrWhiteSpace(row.Sku)
            && string.IsNullOrWhiteSpace(row.Cantidad)
            && string.IsNullOrWhiteSpace(row.Lote)
            && string.IsNullOrWhiteSpace(row.DireccionDespacho)
            && string.IsNullOrWhiteSpace(row.OrdenCompra)
            && string.IsNullOrWhiteSpace(row.EstadoInventario)
            && string.IsNullOrWhiteSpace(row.EsCenabast)
            && string.IsNullOrWhiteSpace(row.FechaEntrega)
            && string.IsNullOrWhiteSpace(row.Urgencia)
            && string.IsNullOrWhiteSpace(row.Observacion);

        // GetString() sobre TODAS: la columna A es formato General (un Delivery
        // numérico vuelve como double) y la K es texto. Leer como string es
        // uniforme y no destruye ceros a la izquierda.
        private static string Text(IXLRow row, int column) =>
            row.Cell(column).GetString().Trim();

        internal static IReadOnlyList<string> ReadNormalizedHeaders(Stream planilla)
        {
            using XLWorkbook book = new(planilla);
            IXLRow row = book.Worksheet(1).Row(1);

            return Enumerable.Range(1, 13)
                .Select(c => Normalize(row.Cell(c).GetString()))
                .ToList();
        }

        // Trampa 1: F1 es literalmente "Lote\n(opcional)" y K1 trae un espacio
        // ANTES del \n. Comparar contra "Lote" falla.
        private static string Normalize(string raw) =>
            string.Join(' ', raw.Replace('\n', ' ').Replace('\r', ' ')
                .Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToLowerInvariant();
    }
}
