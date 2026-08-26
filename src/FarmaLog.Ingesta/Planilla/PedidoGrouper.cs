namespace FarmaLog.Ingesta.Planilla
{
    internal static class PedidoGrouper
    {
        public static IReadOnlyList<PedidoGroup> Group(IReadOnlyList<PlanillaRow> rows) =>
            rows.GroupBy(r => r.NumeroDelivery, StringComparer.Ordinal)
                .Select(g => new PedidoGroup(g.Key, g.ToList()))
                .ToList();
    }
}
