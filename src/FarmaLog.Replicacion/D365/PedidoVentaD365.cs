namespace FarmaLog.Replicacion.D365
{
    public sealed record PedidoVentaD365(
        string CodigoLaboratorio,
        string CuentaCliente,
        string DireccionDespacho,
        string TipoOrdenVenta,
        bool EsCenabast,
        string? DocumentoVentaCenabast,
        string? Observacion,
        DateOnly FechaEntregaSolicitada,
        string NumeroDelivery,
        string? OrdenCompra,
        bool Urgencia,
        IReadOnlyCollection<LineaPedidoVentaD365> Lineas,
        string Almacen);
}
