using FarmaLog.Nucleo.Infrastructure.Messaging;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Messaging
{
    public sealed record SentMessage(string Destination, string MessageId, string Payload);

    public sealed class OutboxMessageSenderSpy(params string[] failingMessageIds) : IOutboxMessageSender
    {
        private readonly List<SentMessage> _sent = [];

        public IReadOnlyList<SentMessage> Sent => _sent;

        public Task SendAsync(
            string destination, string messageId, string payload, CancellationToken cancellationToken)
        {
            if (failingMessageIds.Contains(messageId))
                throw new InvalidOperationException("Envío fallido de prueba");

            _sent.Add(new SentMessage(destination, messageId, payload));

            return Task.CompletedTask;
        }
    }
}
