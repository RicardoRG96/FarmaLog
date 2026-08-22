using ClosedXML.Excel;

namespace FarmaLog.Ingesta
{
    internal sealed class LectorDePlanilla
    {
        private const int ColumnaDelivery = 1;
        private const int PrimeraFilaDeDatos = 2;

        public string LeerPrimerDelivery(Stream planilla)
        {
            using XLWorkbook libro = new(planilla);
            IXLWorksheet hoja = libro.Worksheet(1);

            string valor = hoja.Cell(PrimeraFilaDeDatos, ColumnaDelivery).GetString().Trim();

            return string.IsNullOrWhiteSpace(valor)
                ? throw new InvalidOperationException("La primera fila de datos no trae Delivery.")
                : valor;
        }
    }
}
