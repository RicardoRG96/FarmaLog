using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Solicitudes;

namespace FarmaLog.Nucleo.Infrastructure.Persistence
{
    public sealed class RepositorioDeSolicitudes(NucleoDbContext context) : IRepositorioDeSolicitudes
    {
        public async Task<ResultadoGuardado> Guardar(
            SolicitudDeIngresoPedido solicitud, CancellationToken cancellationToken)
        {
            context.Solicitudes.Add(solicitud);
            await context.SaveChangesAsync(cancellationToken);
            return ResultadoGuardado.Guardada;
        }
    }
}
