using System.Diagnostics.CodeAnalysis;
using FarmaLog.D365Fake.Contracts;

namespace FarmaLog.D365Fake.PedidosVenta
{
    public sealed class PedidoVentaStore
    {
        private readonly Lock _gate = new();
        private readonly Dictionary<(string CodigoLaboratorio, string NumeroDelivery), PedidoVentaRegistrado> _pedidos = new();
        private int _lastNumber;

        public bool TryFind(
            string codigoLaboratorio,
            string numeroDelivery,
            [NotNullWhen(true)] out PedidoVentaRegistrado? registrado)
        {
            lock (_gate)
            {
                return _pedidos.TryGetValue((codigoLaboratorio, numeroDelivery), out registrado);
            }
        }

        public bool TryAdd(PedidoVentaD365 pedido, out PedidoVentaRegistrado registrado)
        {
            lock (_gate)
            {
                (string, string) key = (pedido.CodigoLaboratorio, pedido.NumeroDelivery);

                if (_pedidos.TryGetValue(key, out PedidoVentaRegistrado? existing))
                {
                    registrado = existing;
                    return false;
                }

                _lastNumber++;
                registrado = new PedidoVentaRegistrado($"PV-{_lastNumber:D10}", pedido, DateTimeOffset.UtcNow);
                _pedidos.Add(key, registrado);
                return true;
            }
        }

        public IReadOnlyList<PedidoVentaRegistrado> GetAll()
        {
            lock (_gate)
            {
                return [.. _pedidos.Values];
            }
        }
    }
}