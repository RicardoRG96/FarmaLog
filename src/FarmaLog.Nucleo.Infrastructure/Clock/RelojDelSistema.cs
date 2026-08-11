using FarmaLog.Nucleo.Application.Ports;

namespace FarmaLog.Nucleo.Infrastructure.Clock
{
    public sealed class RelojDelSistema : IReloj
    {
        private readonly TimeProvider _tiempo;
        private readonly TimeZoneInfo _zonaDeOperacion;

        public RelojDelSistema(
            TimeProvider tiempo)
        {
            _tiempo = tiempo ?? throw new ArgumentNullException(nameof(tiempo));
            _zonaDeOperacion = TimeZoneInfo.FindSystemTimeZoneById("America/Santiago");
        }

        public DateOnly Hoy =>
            DateOnly.FromDateTime(
                TimeZoneInfo.ConvertTimeFromUtc(_tiempo.GetUtcNow().UtcDateTime, _zonaDeOperacion));
    }
}
