using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class LineaSolicitud
    {
        public string? Sku { get; }
        public int Cantidad { get; }
        public string? EstadoInventario { get; }
        public string? Lote { get; }

        private LineaSolicitud(
            string? sku, int cantidad, string estadoInventario, string? lote)
        {
            Sku = sku;
            Cantidad = cantidad;
            EstadoInventario = estadoInventario;
            Lote = lote;
        }

        public static LineaSolicitud Create(
            string? sku, int cantidad, string? estadoInventario, string? lote)
        {
            if (cantidad <= 0)
                throw new LineaSolicitudInvalidaException("La cantidad debe ser mayor a cero");

            if (string.IsNullOrWhiteSpace(sku))
                throw new LineaSolicitudInvalidaException("El SKU es obligatorio");

            if (string.IsNullOrWhiteSpace(estadoInventario))
                throw new LineaSolicitudInvalidaException("El Estado de Inventario es obligatorio");

            return new LineaSolicitud(sku, cantidad, estadoInventario, lote);
        }
    }
}
