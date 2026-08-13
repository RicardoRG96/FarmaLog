using System.Text.Json.Serialization;

namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed record LineaMessage(
        [property: JsonRequired] string Sku,
        [property: JsonRequired] int Cantidad,
        [property: JsonRequired] string EstadoInventario, 
        string? Lote);
}
