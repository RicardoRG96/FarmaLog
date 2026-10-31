namespace FarmaLog.Replicacion.Contracts
{
    public sealed record ResultadoReplicacionMessage(
        string Resultado,
        string? NumeroPedidoD365,
        string CodigoLaboratorio,
        string NumeroDelivery,
        IReadOnlyList<string> MotivosDeRechazo);
}
