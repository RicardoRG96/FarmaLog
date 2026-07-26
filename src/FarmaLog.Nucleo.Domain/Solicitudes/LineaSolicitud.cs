using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class LineaSolicitud
    {
        public string? Sku { get; }
        public int Cantidad { get; }
        public string? EstadoInventario { get; }

        private LineaSolicitud(string? sku, int cantidad, string estadoInventario)
        {
            Sku = sku;
            Cantidad = cantidad;
            EstadoInventario = estadoInventario;
        }

        public static LineaSolicitud Create(
            string? sku, int cantidad, string estadoInventario)
        {
            if (cantidad <= 0)
                throw new LineaSolicitudInvalidaException("La cantidad debe ser mayor a cero");

            if (string.IsNullOrWhiteSpace(sku))
                throw new LineaSolicitudInvalidaException("El SKU es obligatorio");

            if (estadoInventario == "")
                throw new LineaSolicitudInvalidaException("El Estado de Inventario es obligatorio");

            return new LineaSolicitud(sku, cantidad, estadoInventario);
        }
    }
}
