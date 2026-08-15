using Azure.Messaging.ServiceBus;
using FarmaLog.Nucleo.Infrastructure.Messaging;

namespace FarmaLog.Nucleo.Worker
{
    public sealed class RegistrarSolicitudWorker(
        ServiceBusClient client,
        IServiceScopeFactory scopeFactory,
        ILogger<RegistrarSolicitudWorker> logger) : BackgroundService
    {
        private const string NombreDeLaCola = "registrar-solicitud-ingreso";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using ServiceBusProcessor processor = client.CreateProcessor(
                NombreDeLaCola,
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1
                });

            processor.ProcessMessageAsync += ProcesarMensajeAsync;
            processor.ProcessErrorAsync += ProcesarErrorAsync;

            await processor.StartProcessingAsync(stoppingToken);
            logger.LogInformation("Escuchando la cola {Cola}.", NombreDeLaCola);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {

            }

            await processor.StopProcessingAsync(CancellationToken.None);
        }

        private async Task ProcesarMensajeAsync(ProcessMessageEventArgs args)
        {
            await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
            RegistrarSolicitudMessageDispatcher dispatcher =
                scope.ServiceProvider.GetRequiredService<RegistrarSolicitudMessageDispatcher>();

            MessageDestination destino;

            try
            {
                destino = await dispatcher.DispatchAsync(
                    args.Message.Body.ToString(), args.CancellationToken);
            }
            catch (Exception ex)
            {
                // Falla no clasificada = transitoria hasta que se demuestre lo contrario.
                // NO se hace settlement: el lock expira y Service Bus reentrega en LockDuration.
                // Dejar escapar la excepcion haria que el SDK abandone el mensaje
                // (lo hace siempre, sin importar AutoCompleteMessages) y lo reentregue
                // al instante, quemando los tres intentos en segundos.
                logger.LogError(ex,
                    "Falla no clasificada procesando {MessageId} (intento {Intento}). Se deja expirar el lock.",
                    args.Message.MessageId, args.Message.DeliveryCount);
                return;
            }

            switch (destino)
            {
                case MessageDestination.Completar:
                    await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                    break;

                case MessageDestination.DescartarADeadLetter:
                    await args.DeadLetterMessageAsync(
                        args.Message,
                        "MensajeInvalido",
                        "Falla determinista: el mensaje no puede procesarse en ningun reintento.",
                        args.CancellationToken);
                    break;
            }
        }

        private Task ProcesarErrorAsync(ProcessErrorEventArgs args)
        {
            logger.LogError(args.Exception,
                "Error de transporte en {Origen} de {Entidad}.",
                args.ErrorSource, args.EntityPath);
            return Task.CompletedTask;
        }
    }
}
