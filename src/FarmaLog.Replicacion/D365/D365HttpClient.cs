using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace FarmaLog.Replicacion.D365
{
    public sealed class D365HttpClient(HttpClient client) : ID365Client
    {
        private const string MotivoPedidoYaExiste = "Ya existe un pedido con ese numero de pedido del cliente: ";

        public async Task<RespuestaD365> CrearPedidoVentaAsync(PedidoVentaD365 pedido, CancellationToken cancellationToken)
        {
            using HttpResponseMessage httpResponse = await client.PostAsJsonAsync(
                "api/salesorders", pedido, JsonSerializerOptions.Default, cancellationToken);

            if (httpResponse.StatusCode is HttpStatusCode.BadRequest)
            {
                PedidoRechazadoResponse? pedidoRechazadoResponse = await httpResponse.Content.ReadFromJsonAsync<PedidoRechazadoResponse>(
                    JsonSerializerOptions.Default, cancellationToken);

                string motivo = pedidoRechazadoResponse!.Motivos.First();

                if (motivo.StartsWith(MotivoPedidoYaExiste, StringComparison.Ordinal))
                {
                    return new RespuestaD365.YaExistia(motivo[MotivoPedidoYaExiste.Length..]);
                }

                return new RespuestaD365.Rechazado(pedidoRechazadoResponse.Motivos);
            }

            PedidoAceptadoResponse? pedidoAceptadoResponse = await httpResponse.Content.ReadFromJsonAsync<PedidoAceptadoResponse>(
                JsonSerializerOptions.Default, cancellationToken);

            return new RespuestaD365.Aceptado(pedidoAceptadoResponse!.NumeroPedido);
        }

        private sealed record PedidoRechazadoResponse(IReadOnlyCollection<string> Motivos);
        private sealed record PedidoAceptadoResponse(string NumeroPedido);
    }
}