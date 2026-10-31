namespace FarmaLog.Replicacion.Contracts
{
    public sealed record LineaDePedidoMessage(
        string Sku,
        int Cantidad,
        string EstadoInventario,
        string? Lote);
}
