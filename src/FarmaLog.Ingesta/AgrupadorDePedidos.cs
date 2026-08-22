namespace FarmaLog.Ingesta
{
    internal static class AgrupadorDePedidos
    {
        public static IReadOnlyList<PedidoAgrupado> Agrupar(IReadOnlyList<FilaCruda> filas) =>
            filas.GroupBy(f => f.NumeroDelivery, StringComparer.Ordinal)
                .Select(g => new PedidoAgrupado(g.Key, g.ToList()))
                .ToList();
    }
}
