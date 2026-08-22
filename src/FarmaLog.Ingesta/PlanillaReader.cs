using ClosedXML.Excel;

namespace FarmaLog.Ingesta
{
    internal sealed class PlanillaReader
    {
        private const int DeliveryColumn = 1;
        private const int FirstDataRow = 2;

        public IReadOnlyList<PlanillaRow> Read(Stream planilla)
        {
            using XLWorkbook book = new(planilla);
            IXLWorksheet sheet = book.Worksheet(1);

            List<PlanillaRow> rows = [];

            foreach (var row in sheet.RowsUsed())
            {
                if (row.RowNumber() < FirstDataRow) continue;

                string delivery = row.Cell(DeliveryColumn).GetString().Trim();

                if (string.IsNullOrWhiteSpace(delivery)) continue;

                rows.Add(new PlanillaRow(
                    row.RowNumber(),
                    delivery,
                    Text(row, 2), Text(row, 3), Text(row, 4),
                    Text(row, 5), Text(row, 6), Text(row, 7),
                    Text(row, 8), Text(row, 9), Text(row, 10),
                    Text(row, 11), Text(row, 12), Text(row, 13)));
            }

            return rows;
        }

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

        private static string Normalize(string raw) =>
            string.Join(' ', raw.Replace('\n', ' ').Replace('\r', ' ')
                .Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToLowerInvariant();
    }
}
