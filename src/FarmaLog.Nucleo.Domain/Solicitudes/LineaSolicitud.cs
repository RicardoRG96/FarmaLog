using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class LineaSolicitud
    {
        public string? Sku { get; }
        public int Cantidad { get; }

        private LineaSolicitud(string? sku, int cantidad)
        {
            Sku = sku;
            Cantidad = cantidad;
        }

        public static LineaSolicitud Create(
            string? sku, int cantidad)
        {
            if (cantidad <= 0)
                throw new LineaSolicitudInvalidaException("La cantidad debe ser mayor a cero");

            if (sku == "")
                throw new LineaSolicitudInvalidaException("El SKU es obligatorio");

            return new LineaSolicitud(sku, cantidad);
        }
    }
}
