namespace FarmaLog.Replicacion.Messaging
{
    public sealed class ReplicarPedidoMessageDispatcher(
        ReplicarPedidoMessageHandler handler)
    {
        public async Task<MessageDestination> DispatchAsync(string messageId, string body, CancellationToken cancellationToken)
        {
            return MessageDestination.DeadLetter;
        }
    }
}
