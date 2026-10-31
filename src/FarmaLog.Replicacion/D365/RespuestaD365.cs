namespace FarmaLog.Replicacion.D365
{
    public abstract record RespuestaD365
    {
        public sealed record Aceptado(string NumeroPedido) : RespuestaD365;
        public sealed record Rechazado(IReadOnlyCollection<string> Motivos) : RespuestaD365;
    }
}
