using FarmaLog.D365Fake.Configuration;
using FarmaLog.D365Fake.Contracts;
using Microsoft.Extensions.Options;

namespace FarmaLog.D365Fake.PedidosVenta
{
    public sealed class PedidoVentaValidator(IOptionsMonitor<D365FakeOptions> options)
    {
        public IReadOnlyList<string> Validate(PedidoVentaD365 pedido)
        {
            D365FakeOptions current = options.CurrentValue;
            List<string> motivos = [];

            if (current.UnknownCuentasCliente.Contains(pedido.CuentaCliente, StringComparer.Ordinal))
            {
                motivos.Add($"La cuenta de cliente {pedido.CuentaCliente} no existe");
            }

            IEnumerable<string> unknownSkus = pedido.Lineas
                .Select(linea => linea.Sku)
                .Where(sku => current.UnknownSkus.Contains(sku, StringComparer.Ordinal))
                .Distinct(StringComparer.Ordinal);

            foreach (string sku in unknownSkus)
            {
                motivos.Add($"El articulo {sku} no existe");
            }

            return motivos;
        }
    }
}