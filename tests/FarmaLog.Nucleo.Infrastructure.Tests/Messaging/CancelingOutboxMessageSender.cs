using FarmaLog.Nucleo.Infrastructure.Messaging;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Messaging
{
    public sealed class CancelingOutboxMessageSender(CancellationTokenSource source) : IOutboxMessageSender
    {
        public Task SendAsync(string destination, string messageId, string payload, CancellationToken cancellationToken)
        {
            source.Cancel();                                    // llega el SIGTERM durante el envío
            cancellationToken.ThrowIfCancellationRequested();   // como lo haría el SDK
            return Task.CompletedTask;
        }
    }
}
