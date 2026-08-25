namespace FarmaLog.Ingesta.Api
{
    internal sealed record CargaResponse(int PublishedPedidos, IReadOnlyList<string> Errors);
}
