using FarmaLog.Nucleo.Infrastructure.Messaging;

namespace FarmaLog.Nucleo.Worker
{
    public sealed class OutboxPublisherWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxPublisherWorker> logger) : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(Interval);

            try
            {
                // do-while: el primer barrido corre al arrancar, sin esperar el primer tick.
                do
                {
                    try
                    {
                        // Un scope por barrido: DbContext nuevo, sin entidades rastreadas del barrido anterior.
                        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
                        OutboxPublisher publisher = scope.ServiceProvider.GetRequiredService<OutboxPublisher>();
                        await publisher.PublishPendingAsync(stoppingToken);
                    }
                    catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                    {
                        // Base caída u otra falla del barrido completo: el bucle sigue vivo.
                        logger.LogError(ex, "Falló el barrido del outbox. Se reintenta en {Interval}.", Interval);
                    }
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // apagado normal (SIGTERM). Las filas pendientes las toma la próxima réplica.
            }
        }
    }
}
