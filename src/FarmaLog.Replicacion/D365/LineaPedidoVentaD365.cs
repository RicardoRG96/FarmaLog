namespace FarmaLog.Replicacion.D365
{
    public sealed record LineaPedidoVentaD365(
        string Sku,
        int Cantidad,
        string EstadoInventario,
        string? Lote);
}
