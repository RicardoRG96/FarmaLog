namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed record RegistrarSolicitudMessage(
        string CodigoLaboratorio,
        string NumeroDelivery,
        string CuentaCliente,
        string DireccionDespacho,
        string TipoOrdenVenta,
        bool EsCenabast,
        string? DocumentoVentaCenabast,
        string? Observacion,
        DateOnly? FechaEntrega,
        string? OrdenCompra,
        bool Urgencia,
        IReadOnlyCollection<LineaMessage> Lineas);
}
