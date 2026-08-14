using System.Text.Json;

namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed class RegistrarSolicitudMessageDispatcher(
        RegistrarSolicitudMessageHandler messageHandler)
    {
        public async Task<MessageDestination> DispatchAsync(string body, CancellationToken ct)
        {
            try
            {
                RegistrarSolicitudMessage? message = 
                    JsonSerializer.Deserialize<RegistrarSolicitudMessage>(body);

                if (message is null || message.Lineas is null) 
                    return MessageDestination.DescartarADeadLetter;

                return await messageHandler.Handle(message, ct);
            }
            catch (JsonException)
            {
                return MessageDestination.DescartarADeadLetter;
            }
        }
    }
}
