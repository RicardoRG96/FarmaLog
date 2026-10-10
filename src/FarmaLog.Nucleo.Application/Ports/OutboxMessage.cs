namespace FarmaLog.Nucleo.Application.Ports
{
    public sealed record OutboxMessage(string Destination, string MessageId, object Payload);
}
