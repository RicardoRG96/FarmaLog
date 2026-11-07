using FarmaLog.Replicacion.Contracts;
using System.Text.Json;

namespace FarmaLog.Replicacion.Messaging
{
    public sealed class ReplicarPedidoMessageDispatcher(
        ReplicarPedidoMessageHandler handler)
    {
        public async Task<MessageDestination> DispatchAsync(string messageId, string body, CancellationToken ct)
        {
            try
            {
                ReplicarPedidoEnD365Message? message = 
                    JsonSerializer.Deserialize<ReplicarPedidoEnD365Message>(body);

                return await handler.HandleAsync(messageId, message!, ct);
            }
            catch (JsonException)
            {
                return MessageDestination.DeadLetter;
            }
        }
    }
}
