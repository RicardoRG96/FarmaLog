using System.Net.Http.Json;
using System.Text.Json;

namespace FarmaLog.Replicacion.D365
{
    public sealed class D365HttpClient(HttpClient client) : ID365Client
    {
        public async Task<RespuestaD365> CrearPedidoVentaAsync(PedidoVentaD365 pedido, CancellationToken cancellationToken)
        {
            using HttpResponseMessage httpResponse = await client.PostAsJsonAsync(
                "api/salesorders", pedido, JsonSerializerOptions.Default, cancellationToken);

            D365Response? d365Response = await httpResponse.Content.ReadFromJsonAsync<D365Response>(
                JsonSerializerOptions.Default, cancellationToken);

            return new RespuestaD365.Aceptado(d365Response!.NumeroPedido);
        }

        private sealed record D365Response(string NumeroPedido);
    }
}