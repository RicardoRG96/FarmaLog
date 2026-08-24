using System.Globalization;

namespace FarmaLog.Ingesta
{
    internal static class FormatValidator
    {
        private const string DateFormat = "dd/MM/yyyy";

        public static IReadOnlyList<ValidationError> Validate(PedidoGroup pedido)
        {
            List<ValidationError> errors = new();

            PlanillaRow header = pedido.Rows[0];

            Required(header.CuentaCliente, "Cuenta de Cliente");
            Required(header.DireccionDespacho, "Código de Despacho");
            Required(header.EsCenabast, "Pedido Cenabast");

            if (IsSi(header.EsCenabast) && string.IsNullOrWhiteSpace(header.DocumentoVentaCenabast))
            {
                errors.Add(new ValidationError(
                    "Falta el Documento de Venta Cenabast", "",
                    pedido.NumeroDelivery,
                    "La columna es obligatoria cuando Pedido Cenabast es SI."));
            }

            if (!string.IsNullOrWhiteSpace(header.FechaEntrega) &&
                !DateOnly.TryParseExact(header.FechaEntrega, DateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                errors.Add(new ValidationError(
                    "Formato de fecha inválido", header.FechaEntrega,
                    pedido.NumeroDelivery,
                    $"Use formato {DateFormat} (ej: 03/12/2026)."));
            }

            RequiredInAnyRow(r => r.Sku, "Código de Artículo");
            RequiredInAnyRow(r => r.Cantidad, "Cantidad");
            RequiredInAnyRow(r => r.EstadoInventario, "Estado del Inventario");

            return errors;

            void RequiredInAnyRow(Func<PlanillaRow, string> column, string columnName)
            {
                if (pedido.Rows.Any(r => string.IsNullOrWhiteSpace(column(r))))
                    errors.Add(new ValidationError(
                        $"Falta {columnName}", "", pedido.NumeroDelivery,
                        $"Complete la columna '{columnName}' en todas las filas del pedido."));
            }

            void Required(string value, string column)
            {
                if (string.IsNullOrWhiteSpace(value))
                    errors.Add(new ValidationError(
                        $"Falta {column}", "", pedido.NumeroDelivery,
                        $"Complete la columna '{column}' en todas las filas del pedido."));
            }
        }

        private static bool IsSi(string value) =>
            value.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase);
    }
}
