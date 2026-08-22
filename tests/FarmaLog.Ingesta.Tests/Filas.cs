namespace FarmaLog.Ingesta.Tests
{
    internal static class Filas
    {
        public static FilaCruda Una(
            int numeroDeFila = 2, string delivery = "DEL-1", string cuenta = "778903671",
            string cenabast = "", string sku = "SKU-1", string cantidad = "10",
            string lote = "", string despacho = "778903671D1", string oc = "",
            string inventario = "DISPONIBLE", string esCenabast = "NO",
            string fecha = "", string urgencia = "NO", string observacion = "") =>
            new(numeroDeFila, delivery, cuenta, cenabast, sku, cantidad, lote,
                despacho, oc, inventario, esCenabast, fecha, urgencia, observacion);
    }
}
