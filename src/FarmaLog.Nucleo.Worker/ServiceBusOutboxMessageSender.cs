using Azure.Messaging.ServiceBus;
using FarmaLog.Nucleo.Infrastructure.Messaging;
using System.Collections.Concurrent;

namespace FarmaLog.Nucleo.Worker
{
    public sealed class ServiceBusOutboxMessageSender(ServiceBusClient client)
        : IOutboxMessageSender, IAsyncDisposable
    {
        private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();

        public Task SendAsync(
            string destination, string messageId, string payload, CancellationToken cancellationToken)
        {
            ServiceBusSender sender = _senders.GetOrAdd(destination, name => client.CreateSender(name));

            ServiceBusMessage message = new(payload)
            {
                MessageId = messageId,
                ContentType = "application/json"
            };

            return sender.SendMessageAsync(message, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            foreach (ServiceBusSender sender in _senders.Values)
            {
                await sender.DisposeAsync();
            }
        }
    }
}
