using FarmaLog.Replicacion.Contracts;

namespace FarmaLog.Replicacion.Publishing
{
    public interface IResultadoReplicacionPublisher
    {
        Task PublishAsync(
            string correlationId, ResultadoReplicacionMessage resultado, CancellationToken cancellationToken);
    }
}
