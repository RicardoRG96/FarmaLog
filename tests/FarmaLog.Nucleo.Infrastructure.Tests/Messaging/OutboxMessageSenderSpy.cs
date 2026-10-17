using FarmaLog.Nucleo.Infrastructure.Messaging;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Messaging
{
    public sealed record SentMessage(string Destination, string MessageId, string Payload);

    public sealed class OutboxMessageSenderSpy : IOutboxMessageSender
    {
        private readonly List<SentMessage> _sent = [];

        public IReadOnlyList<SentMessage> Sent => _sent;

        public Task SendAsync(
            string destination, string messageId, string payload, CancellationToken cancellationToken)
        {
            _sent.Add(new SentMessage(destination, messageId, payload));

            return Task.CompletedTask;
        }
    }
}
