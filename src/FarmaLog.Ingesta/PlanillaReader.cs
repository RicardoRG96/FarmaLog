using ClosedXML.Excel;

namespace FarmaLog.Ingesta
{
    internal sealed class PlanillaReader
    {
        private const int DeliveryColumn = 1;
        private const int FirstDataRow = 2;

        public IReadOnlyList<FilaCruda> Read(Stream planilla)
        {
            using XLWorkbook book = new(planilla);
            IXLWorksheet sheet = book.Worksheet(1);

            List<FilaCruda> rows = [];

            foreach (var row in sheet.RowsUsed())
            {
                if (row.RowNumber() < FirstDataRow) continue;

                string delivery = row.Cell(DeliveryColumn).GetString().Trim();

                if (string.IsNullOrWhiteSpace(delivery)) continue;

                rows.Add(new FilaCruda(row.RowNumber(), delivery));
            }

            return rows;
        }
    }
}
