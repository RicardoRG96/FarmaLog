namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class LineaSolicitud
    {
        public string Sku { get; }
        public string EstadoInventario { get; }
        public int Cantidad { get; }
        public string? NumeroLote { get; }

        private LineaSolicitud(string sku, string estadoInventario, int cantidad, string? numeroLote)
        {
            Sku = sku;
            EstadoInventario = estadoInventario;
            Cantidad = cantidad;
            NumeroLote = numeroLote;
        }

        public static LineaSolicitud Create(
            string sku, string estadoInventario, int cantidad, string? numeroLote)
        {
            return new LineaSolicitud(sku, estadoInventario, cantidad, numeroLote);
        }
    }
}
