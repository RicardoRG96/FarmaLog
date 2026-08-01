using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Solicitudes;

namespace FarmaLog.Nucleo.Application.Tests.Doubles
{
    public class RepositorioDeSolicitudesEnMemoria : IRepositorioDeSolicitudes
    {
        private readonly List<SolicitudDeIngresoPedido> _guardadas = new();

        public IReadOnlyList<SolicitudDeIngresoPedido> Guardadas => _guardadas;

        public Task<ResultadoGuardado> Guardar(SolicitudDeIngresoPedido solicitud, CancellationToken ct = default)
        {
            _guardadas.Add(solicitud);
            return Task.FromResult(ResultadoGuardado.Guardada);
        }
    }
}
