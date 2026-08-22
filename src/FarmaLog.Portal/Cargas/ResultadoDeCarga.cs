namespace FarmaLog.Portal.Cargas
{
    public sealed record ResultadoDeCarga(int PublishedPedidos, IReadOnlyList<string> Errors);
}
