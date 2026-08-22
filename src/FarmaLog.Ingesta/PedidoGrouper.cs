namespace FarmaLog.Ingesta
{
    internal static class PedidoGrouper
    {
        public static IReadOnlyList<PedidoGroup> Agrupar(IReadOnlyList<PlanillaRow> filas) =>
            filas.GroupBy(f => f.NumeroDelivery, StringComparer.Ordinal)
                .Select(g => new PedidoGroup(g.Key, g.ToList()))
                .ToList();
    }
}
