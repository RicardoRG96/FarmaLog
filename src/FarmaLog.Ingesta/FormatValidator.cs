using System.Globalization;

namespace FarmaLog.Ingesta
{
    internal static class FormatValidator
    {
        private const string DateFormat = "dd/MM/yyyy";

        public static IReadOnlyList<ValidationError> Validate(PedidoGroup pedido)
        {
            List<ValidationError> errors = new();

            foreach (PlanillaRow row in pedido.Rows)
            {
                Required(row.CuentaCliente, "Cuenta de Cliente");
                Required(row.DireccionDespacho, "Código de Despacho");
                Required(row.Sku, "Código de Artículo");
                Required(row.Cantidad, "Cantidad");
                Required(row.EstadoInventario, "Estado del Inventario");
                Required(row.EsCenabast, "Pedido Cenabast");

                if (IsSi(row.EsCenabast) && string.IsNullOrWhiteSpace(row.DocumentoVentaCenabast))
                {
                    errors.Add(new ValidationError(
                       "Falta el Documento de Venta Cenabast", "",
                        pedido.NumeroDelivery,
                        "La columna es obligatoria cuando Pedido Cenabast es SI."));
                }

                if (!string.IsNullOrWhiteSpace(row.FechaEntrega) && 
                    !DateOnly.TryParseExact(row.FechaEntrega, DateFormat,
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    errors.Add(new ValidationError(
                        "Formato de fecha inválido", row.FechaEntrega,
                        pedido.NumeroDelivery,
                        $"Use formato {DateFormat} (ej: 03/12/2026)."));
                }

                void Required(string value, string column)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        errors.Add(new ValidationError(
                            $"Falta {column}", "", pedido.NumeroDelivery,
                            $"Complete la columna '{column}' en todas las filas del pedido."));
                }
            }

            return errors;
        }

        private static bool IsSi(string value) =>
            value.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase);
    }
}
