namespace FarmaLog.Portal.Cargas
{
    public sealed record ResultadoDeCarga(int PedidosPublicados, IReadOnlyList<string> Errors);
}
