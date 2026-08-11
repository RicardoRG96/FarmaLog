namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed record LineaMessage(
        string Sku, int cantidad, string EstadoInventario, string? Lote);
}
