using FarmaLog.Replicacion.D365;

namespace FarmaLog.Replicacion.Tests.D365
{
    public sealed class D365ClientSpy(RespuestaD365? response = null) : ID365Client
    {
        private readonly List<PedidoVentaD365> _received = [];
        public IReadOnlyList<PedidoVentaD365> Received => _received;

        public Task<RespuestaD365> CrearPedidoVentaAsync(PedidoVentaD365 pedido, CancellationToken cancellationToken)
        {
            _received.Add(pedido);
            return Task.FromResult(response ?? new RespuestaD365.Aceptado("PV-0000000038"));
        }
    }
}
