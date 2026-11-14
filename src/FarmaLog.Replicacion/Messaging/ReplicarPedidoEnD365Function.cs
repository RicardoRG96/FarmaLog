using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FarmaLog.Replicacion.Messaging
{
    public sealed class ReplicarPedidoEnD365Function(
        ReplicarPedidoMessageDispatcher dispatcher,
        ILogger<ReplicarPedidoEnD365Function> logger)
    {
        [Function(nameof(ReplicarPedidoEnD365Function))]
        public async Task RunAsync(
            [ServiceBusTrigger("replicar-pedido-d365", Connection = "ServiceBus", AutoCompleteMessages = false)]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions,
            CancellationToken cancellationToken)
        {
            MessageDestination destination;
            try
            {
                destination = await dispatcher.DispatchAsync(
                    message.MessageId, message.Body.ToString(), cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception,
                    "Transient failure replicating {MessageId} (delivery {DeliveryCount}); message left unsettled",
                    message.MessageId, message.DeliveryCount);
                return;
            }

            switch (destination)
            {
                case MessageDestination.Complete:
                    await messageActions.CompleteMessageAsync(message, cancellationToken);
                    break;

                case MessageDestination.DeadLetter:
                    await messageActions.DeadLetterMessageAsync(
                        message,
                        deadLetterReason: "MensajeMalFormado",
                        cancellationToken: cancellationToken);
                    break;

                default:
                    throw new UnreachableException($"Destino no contemplado: {destination}");
            }
        }
    }
}
