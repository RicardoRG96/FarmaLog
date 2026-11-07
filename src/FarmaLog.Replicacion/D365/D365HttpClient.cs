using System.Net.Http.Json;
using System.Text.Json;

namespace FarmaLog.Replicacion.D365
{
    public sealed class D365HttpClient(HttpClient client) : ID365Client
    {
        public async Task<RespuestaD365> CrearPedidoVentaAsync(PedidoVentaD365 pedido, CancellationToken cancellationToken)
        {
            await client.PostAsJsonAsync("api/salesorders", pedido, JsonSerializerOptions.Default, cancellationToken);

            return new RespuestaD365.Aceptado("PV-0000000038");
        }
    }
}