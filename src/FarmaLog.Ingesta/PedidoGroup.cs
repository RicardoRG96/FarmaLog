namespace FarmaLog.Ingesta
{
    internal sealed record PedidoGroup(string NumeroDelivery, IReadOnlyList<PlanillaRow> Filas);
}
