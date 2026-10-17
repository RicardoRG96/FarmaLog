namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public interface IOutboxMessageSender
    {
        Task SendAsync(
            string destination, string messageId, string payload, CancellationToken cancellationToken);
    }
}
