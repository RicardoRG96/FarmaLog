using Azure.Messaging.ServiceBus;
using FarmaLog.Replicacion.Contracts;
using System.Text.Json;

namespace FarmaLog.Replicacion.Publishing
{
    public sealed class ServiceBusResultadoReplicacionPublisher(ServiceBusSender sender) 
        : IResultadoReplicacionPublisher
    {
        public async Task PublishAsync(
            string correlationId, ResultadoReplicacionMessage resultado, CancellationToken cancellationToken)
        {
            ServiceBusMessage message = new(BinaryData.FromObjectAsJson(resultado, JsonSerializerOptions.Default))
            {
                CorrelationId = correlationId,
                ContentType = "application/json",
            };
            message.ApplicationProperties["Resultado"] = resultado.Resultado;

            await sender.SendMessageAsync(message, cancellationToken);
        }
    }
}
