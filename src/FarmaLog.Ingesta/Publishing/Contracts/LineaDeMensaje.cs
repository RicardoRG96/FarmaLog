namespace FarmaLog.Ingesta.Publishing.Contracts
{
    internal sealed record LineaDeMensaje(
        string Sku,
        int Cantidad,
        string EstadoInventario,
        string? Lote);
}
