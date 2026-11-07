using FarmaLog.Replicacion.D365;

namespace FarmaLog.Replicacion.Tests.D365
{
    public sealed class ThrowingD365Client(Exception? exception = null) : ID365Client
    {
        private readonly Exception _exception = exception ?? new HttpRequestException();

        public Task<RespuestaD365> CrearPedidoVentaAsync(PedidoVentaD365 pedido, CancellationToken cancellationToken) =>
            throw _exception;
    }
}
