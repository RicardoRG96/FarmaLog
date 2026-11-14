namespace FarmaLog.D365Fake.Contracts
{
    public sealed record LineaPedidoVentaD365(
        string Sku,
        int Cantidad,
        string EstadoInventario,
        string? Lote);
}