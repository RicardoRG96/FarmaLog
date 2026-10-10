using FarmaLog.Nucleo.Application.Ports;

namespace FarmaLog.Nucleo.Application.Tests.Doubles
{
    public sealed class InMemoryOutbox : IOutbox
    {
        private readonly List<OutboxMessage> _added = new();

        public IReadOnlyList<OutboxMessage> Added => _added;

        public void Add(OutboxMessage message)
        {
            _added.Add(message);
        }
    }
}
