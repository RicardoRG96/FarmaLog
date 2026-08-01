using FarmaLog.Nucleo.Application.Ports;

namespace FarmaLog.Nucleo.Application.Tests.Doubles
{
    public sealed class GeneradorDeIdentificadoresFijo : IGeneradorDeIdentificadores
    {
        private readonly Guid _id;
        public GeneradorDeIdentificadoresFijo(Guid id) => _id = id;
        public Guid Nuevo() => _id;
    }
}
