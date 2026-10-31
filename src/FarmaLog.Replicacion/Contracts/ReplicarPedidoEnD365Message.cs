namespace FarmaLog.Replicacion.Contracts
{
    public sealed record ReplicarPedidoEnD365Message(
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
        IReadOnlyCollection<LineaDePedidoMessage> Lineas);
}
