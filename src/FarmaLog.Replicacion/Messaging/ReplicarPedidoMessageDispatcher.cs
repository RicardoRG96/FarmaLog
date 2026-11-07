using FarmaLog.Replicacion.Contracts;
using System.Text.Json;

namespace FarmaLog.Replicacion.Messaging
{
    public sealed class ReplicarPedidoMessageDispatcher(
        ReplicarPedidoMessageHandler handler)
    {
        public async Task<MessageDestination> DispatchAsync(string messageId, string body, CancellationToken cancellationToken)
        {
            ReplicarPedidoEnD365Message? message;

            try
            {
                message = JsonSerializer.Deserialize<ReplicarPedidoEnD365Message>(body);
            }
            catch (JsonException)
            {
                return MessageDestination.DeadLetter;
            }

            return await handler.HandleAsync(messageId, message!, cancellationToken);
        }
    }
}
