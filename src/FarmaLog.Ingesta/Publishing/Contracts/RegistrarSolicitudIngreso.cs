namespace FarmaLog.Ingesta.Publishing.Contracts
{
    internal sealed record RegistrarSolicitudIngreso(
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
        IReadOnlyCollection<LineaDeMensaje> Lineas);
}
