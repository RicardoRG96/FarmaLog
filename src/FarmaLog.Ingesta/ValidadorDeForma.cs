using System.Globalization;

namespace FarmaLog.Ingesta
{
    internal static class ValidadorDeForma
    {
        private const string FormatoFecha = "dd/MM/yyyy";

        public static IReadOnlyList<ErrorDeValidacion> Validar(PedidoAgrupado pedido)
        {
            List<ErrorDeValidacion> errores = new();

            foreach (FilaCruda fila in pedido.Filas)
            {
                Obligatoria(fila.CuentaCliente, "Cuenta de Cliente");
                Obligatoria(fila.DireccionDespacho, "Código de Despacho");
                Obligatoria(fila.Sku, "Código de Artículo");
                Obligatoria(fila.Cantidad, "Cantidad");
                Obligatoria(fila.EstadoInventario, "Estado del Inventario");
                Obligatoria(fila.EsCenabast, "Pedido Cenabast");

                if (EsSi(fila.EsCenabast) && string.IsNullOrWhiteSpace(fila.DocumentoVentaCenabast))
                {
                    errores.Add(new ErrorDeValidacion(
                       "Falta el Documento de Venta Cenabast", "",
                        pedido.NumeroDelivery,
                        "La columna es obligatoria cuando Pedido Cenabast es SI."));
                }

                if (!string.IsNullOrWhiteSpace(fila.FechaEntrega) && 
                    !DateOnly.TryParseExact(fila.FechaEntrega, FormatoFecha,
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    errores.Add(new ErrorDeValidacion(
                        "Formato de fecha inválido", fila.FechaEntrega,
                        pedido.NumeroDelivery,
                        $"Use formato {FormatoFecha} (ej: 03/12/2026)."));
                }

                void Obligatoria(string valor, string columna)
                {
                    if (string.IsNullOrWhiteSpace(valor))
                        errores.Add(new ErrorDeValidacion(
                            $"Falta {columna}", "", pedido.NumeroDelivery,
                            $"Complete la columna '{columna}' en todas las filas del pedido."));
                }
            }

            return errores;
        }

        private static bool EsSi(string valor) =>
            valor.Trim().Equals("SI", StringComparison.OrdinalIgnoreCase);
    }
}
