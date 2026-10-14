using FarmaLog.Nucleo.Application.Ports;

namespace FarmaLog.Nucleo.Infrastructure.Persistence
{
    public sealed class EfOutbox(NucleoDbContext context, TimeProvider timeProvider) : IOutbox
    {
        public void Add(OutboxMessage message)
        {
        }
    }
}
