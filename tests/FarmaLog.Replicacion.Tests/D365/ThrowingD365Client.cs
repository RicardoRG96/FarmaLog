using FarmaLog.Replicacion.D365;

namespace FarmaLog.Replicacion.Tests.D365
{
    public sealed class ThrowingD365Client : ID365Client
    {
        public Task<RespuestaD365> CrearPedidoVentaAsync(PedidoVentaD365 pedido, CancellationToken cancellationToken)
        {
            throw new HttpRequestException();
        }
    }
}
