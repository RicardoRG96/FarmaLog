namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed record LineaMessage(
        string Sku, int 
        
        Cantidad, string EstadoInventario, string? Lote);
}
