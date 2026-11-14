using FarmaLog.D365Fake.Contracts;

namespace FarmaLog.D365Fake.PedidosVenta
{
    public sealed record PedidoVentaRegistrado(
        string NumeroPedido,
        PedidoVentaD365 Pedido,
        DateTimeOffset RegisteredAt);
}