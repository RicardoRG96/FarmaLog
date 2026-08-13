using System.Text.Json.Serialization;

namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed record RegistrarSolicitudMessage(
        [property: JsonRequired] string CodigoLaboratorio,
        [property: JsonRequired] string NumeroDelivery,
        [property: JsonRequired] string CuentaCliente,
        [property: JsonRequired] string DireccionDespacho,
        [property: JsonRequired] string TipoOrdenVenta,
        [property: JsonRequired] bool EsCenabast,
        string? DocumentoVentaCenabast,
        string? Observacion,
        DateOnly? FechaEntrega,
        string? OrdenCompra,
        bool Urgencia,
        IReadOnlyCollection<LineaMessage> Lineas);
}
