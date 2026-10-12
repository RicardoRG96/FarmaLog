namespace FarmaLog.Nucleo.Application.UseCases.SolicitarReplicacionEnD365
{
    public sealed record LineaDePedidoMessage(
        string Sku,
        int Cantidad,
        string EstadoInventario,
        string? Lote);
}
