namespace FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso
{
    public sealed record RegistrarSolicitudDeIngresoCommand(
        string CodigoLaboratorio,
        string CuentaCliente,
        string DireccionDespacho,
        string TipoOrdenVenta,
        bool EsCenabast,
        string DocumentoVentaCenabast,
        string Observacion,
        DateOnly FechaEntregaSolicitada,
        string NumeroDelivery,
        string OrdenCompra,
        bool Urgencia,
        IReadOnlyList<LineaDeSolicitudCommand> Lineas);

    public sealed record LineaDeSolicitudCommand(
        string Sku,
        int Cantidad,
        string EstadoInventario,
        string Lote);
}
