using FarmaLog.Replicacion.Contracts;
using FarmaLog.Replicacion.Publishing;

namespace FarmaLog.Replicacion.Tests.Publishing
{
    public sealed record PublishedResultado(string CorrelationId, ResultadoReplicacionMessage Resultado);

    public sealed class ResultadoReplicacionPublisherSpy : IResultadoReplicacionPublisher
    {
        private readonly List<PublishedResultado> _received = [];
        public IReadOnlyList<PublishedResultado> Received => _received;

        public Task PublishAsync(
            string correlationId, ResultadoReplicacionMessage resultado, CancellationToken cancellationToken)
        {
            _received.Add(
                new PublishedResultado(CorrelationId: correlationId, Resultado: resultado));
            return Task.CompletedTask;
        }
    }
}
