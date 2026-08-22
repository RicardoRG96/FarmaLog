namespace FarmaLog.Ingesta
{
    internal sealed record ProcessingResult(
        int PedidosPublicados,
        IReadOnlyList<string> Errores)
    {
        public bool Exitoso => Errores.Count == 0;
    }
}
