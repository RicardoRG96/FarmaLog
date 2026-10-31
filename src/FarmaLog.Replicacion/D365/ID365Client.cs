namespace FarmaLog.Replicacion.D365
{
    public interface ID365Client
    {
        Task<RespuestaD365> CrearPedidoVentaAsync(PedidoVentaD365 pedido, CancellationToken cancellationToken);
    }
}
