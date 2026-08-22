namespace FarmaLog.Ingesta
{
    internal sealed record PedidoAgrupado(string NumeroDelivery, IReadOnlyList<FilaCruda> Filas);
}
