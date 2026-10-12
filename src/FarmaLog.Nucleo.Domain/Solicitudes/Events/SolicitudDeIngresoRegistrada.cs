using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Events
{
    public sealed record SolicitudDeIngresoRegistrada(
        Guid SolicitudId,
        CodigoLaboratorio CodigoLaboratorio,
        CuentaCliente CuentaCliente,
        DireccionDespacho DireccionDespacho,
        TipoOrdenVenta TipoOrdenVenta,
        bool EsCenabast,
        DocumentoVentaCenabast? DocumentoVentaCenabast,
        IReadOnlyCollection<LineaSolicitud> Lineas,
        string? Observacion,
        DateOnly FechaEntregaSolicitada,
        NumeroDelivery NumeroDelivery,
        string? OrdenCompra,
        bool Urgencia) : IDomainEvent;
}
