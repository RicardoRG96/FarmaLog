namespace FarmaLog.Ingesta
{
    internal sealed record LineaDeMensaje(
        string Sku,
        int Cantidad,
        string EstadoInventario,
        string? Lote);
}
