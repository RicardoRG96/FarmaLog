using FarmaLog.Nucleo.Application.Ports;

namespace FarmaLog.Nucleo.Infrastructure.Identity
{
    public sealed class GeneradorDeIdentificadores : IGeneradorDeIdentificadores
    {
        public Guid Nuevo() => Guid.CreateVersion7();
    }
}
