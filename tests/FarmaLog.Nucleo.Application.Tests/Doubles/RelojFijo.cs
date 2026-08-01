using FarmaLog.Nucleo.Application.Ports;

namespace FarmaLog.Nucleo.Application.Tests.Doubles
{
    public sealed class RelojFijo : IReloj
    {
        public RelojFijo(DateOnly hoy) => Hoy = hoy;
        public DateOnly Hoy { get; }
    }
}
