using System.Text.Json;

namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed class RegistrarSolicitudMessageDispatcher(
        RegistrarSolicitudMessageHandler messageHandler)
    {
        public async Task<MessageDestination> Dispatch(string body, CancellationToken ct)
        {
            try
            {
                RegistrarSolicitudMessage message = 
                    JsonSerializer.Deserialize<RegistrarSolicitudMessage>(body);

                return await messageHandler.Handle(message, ct);
            }
            catch (JsonException)
            {
                return MessageDestination.DescartarADeadLetter;
            }
        }
    }
}
