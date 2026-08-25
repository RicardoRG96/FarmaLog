namespace FarmaLog.Ingesta.Planilla
{
    internal sealed record PedidoGroup(string NumeroDelivery, IReadOnlyList<PlanillaRow> Rows);
}
