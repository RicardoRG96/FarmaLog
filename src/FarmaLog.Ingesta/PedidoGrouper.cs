namespace FarmaLog.Ingesta
{
    internal static class PedidoGrouper
    {
        public static IReadOnlyList<PedidoGroup> Group(IReadOnlyList<PlanillaRow> rows) =>
            rows.GroupBy(f => f.NumeroDelivery, StringComparer.Ordinal)
                .Select(g => new PedidoGroup(g.Key, g.ToList()))
                .ToList();
    }
}
