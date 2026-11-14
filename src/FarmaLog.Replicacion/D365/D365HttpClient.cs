using System.Net;
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

            if (httpResponse.StatusCode is HttpStatusCode.BadRequest)
            {
                PedidoRechazadoResponse? pedidoRechazadoResponse = await httpResponse.Content.ReadFromJsonAsync<PedidoRechazadoResponse>(
                    JsonSerializerOptions.Default, cancellationToken);

                return new RespuestaD365.Rechazado(pedidoRechazadoResponse!.Motivos);
            }

            PedidoAceptadoResponse? pedidoAceptadoResponse = await httpResponse.Content.ReadFromJsonAsync<PedidoAceptadoResponse>(
                JsonSerializerOptions.Default, cancellationToken);

            return new RespuestaD365.Aceptado(pedidoAceptadoResponse!.NumeroPedido);
        }

        private sealed record PedidoRechazadoResponse(IReadOnlyCollection<string> Motivos);
        private sealed record PedidoAceptadoResponse(string NumeroPedido);
    }
}