using FarmaLog.Nucleo.Domain.Solicitudes;

namespace FarmaLog.Nucleo.Application.Ports
{
    public interface IRepositorioDeSolicitudes
    {
        Task<ResultadoGuardado> Guardar(SolicitudDeIngresoPedido solicitud, CancellationToken ct = default);
    }
}
