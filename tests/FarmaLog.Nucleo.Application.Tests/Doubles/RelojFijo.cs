using FarmaLog.Nucleo.Application.Ports;

namespace FarmaLog.Nucleo.Application.Tests.Doubles
{
    public sealed class RelojFijo : IReloj
    {
        public RelojFijo(DateTime ahoraUtc) => AhoraUtc = ahoraUtc;
        public DateTime AhoraUtc { get; }
    }
}
